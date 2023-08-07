using Net.Share;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using Microsoft.CSharp;
using System.CodeDom.Compiler;
using System.Reflection;
#if UNITY_EDITOR
using dnlib.DotNet;
#endif

namespace Net.Helper
{
    public class SequencePoint
    {
        public string FilePath;
        public int StartLine;

        public SequencePoint() { }

        public SequencePoint(string filePath, int startLine)
        {
            StartLine = startLine;
            FilePath = filePath;
        }
    }

    public class ScriptHelper
    {
        private static Dictionary<string, SequencePoint> cache;
        public static Dictionary<string, SequencePoint> Cache 
        {
            get
            {
                if (cache == null)
                    cache = PersistHelper.Deserialize<Dictionary<string, SequencePoint>>("sourceCodeTextPoint.json");
                return cache;
            }
        }
    }

    public interface ISyncVarHandler
    {
        /// <summary>
        /// [SyncVar]字段同步优先级, 只会执行优先级最高的一个, 所以有需要时可继承生成的SyncVarHandlerGenerate类进行额外处理
        /// </summary>
        int SortingOrder { get; }
        void Init();
    }

    /// <summary>
    /// 字段和属性同步帮助类
    /// </summary>
    public class SyncVarGetSetHelper
    {
        /// <summary>
        /// 收集的字段和属性同步缓存
        /// </summary>
        public static Dictionary<Type, Dictionary<string, SyncVarInfo>> Cache = new Dictionary<Type, Dictionary<string, SyncVarInfo>>();

#if SERVICE
        static SyncVarGetSetHelper()
        {
            Init();
        }
#endif

        [UnityEngine.RuntimeInitializeOnLoadMethod]
        private static void Init()
        {
            var syncVars = AssemblyHelper.GetInterfaceInstances<ISyncVarHandler>();
            syncVars.Sort((a, b) => b.SortingOrder.CompareTo(a.SortingOrder));
            var hasHelper = false;
            if (syncVars.Count > 0)
            {
                hasHelper = true;
                syncVars[0].Init();
            }
            if (!hasHelper) //动态编译模式
            {
                var type = AssemblyHelper.GetTypeNotOptimized("HelperFileInfo");
                if (type != null)
                {
                    var path = (string)type.GetMethod("GetPath", BindingFlags.NonPublic | BindingFlags.Static).Invoke(null, null);
#if UNITY_EDITOR
                    DynamicalCompilation(path, type.Assembly, UnityEngine.Debug.Log);
#elif SERVICE
                    DynamicalCompilation(path, type.Assembly, Console.WriteLine);
#endif
                }
            }
        }

        public static void DynamicalCompilation(string path, Assembly mainAssembly, Action<object> log)
        {
            if (File.Exists(path))
            {
                try
                {
                    var text = File.ReadAllText(path);
                    text = text.Split(new string[] { "*/" }, 0)[0];
                    text = text.Replace("/**", "");
                    var provider = new CSharpCodeProvider();
                    var parameters = new CompilerParameters();
                    var dict = new Dictionary<string, string>();
                    var assemblies = AppDomain.CurrentDomain.GetAssemblies();
                    foreach (var assemblie in assemblies)
                    {
                        if (assemblie.IsDynamic)
                            continue;
                        dict.Add(assemblie.GetName().FullName, assemblie.Location);
                    }
                    var referencedAssemblies = mainAssembly.GetReferencedAssemblies();
                    foreach (var item in referencedAssemblies)
                        if (dict.TryGetValue(item.FullName, out var path2))
                            parameters.ReferencedAssemblies.Add(path2);
                    parameters.ReferencedAssemblies.Add(mainAssembly.Location);
                    parameters.GenerateInMemory = false;
                    parameters.GenerateExecutable = false;
                    parameters.OutputAssembly = "SyncVarHandler.dll";//编译后的dll库输出的名称，会在bin/Debug下生成Test.dll库
                    parameters.IncludeDebugInformation = true;
                    var results = provider.CompileAssemblyFromSource(parameters, text);
                    if (results.Errors.HasErrors)
                    {
                        var sb = new StringBuilder();
                        foreach (CompilerError error in results.Errors)
                            sb.AppendLine(string.Format("Error({0}): {1}", error.Line, error.ErrorText));
                        log(sb.ToString());
                    }
                    else
                    {
                        var type = results.CompiledAssembly.GetType("SyncVarHandlerGenerate");
                        if (type != null)
                        {
                            var obj = Activator.CreateInstance(type) as ISyncVarHandler;
                            obj?.Init();
                        }
                    }
                }
                catch (Exception ex)
                {
                    log(ex);
                }
            }
        }
    }

#if UNITY_EDITOR
    public static class InvokeHelperBuild
    {
        static TypeCode GetTypeCode(TypeSig type)
        {
            if (Enum.TryParse<TypeCode>(type.TypeName, out var typeCode))
                return typeCode;
            return TypeCode.Object;
        }

        static bool IsUnityObjectSubclassOf(TypeSig type)
        {
            if (type.FullName.StartsWith("UnityEngine.") | type.FullName.StartsWith("UnityEditor."))
                return type.IsClassSig;
            return false;
        }

        public static string SyncVarBuild(List<TypeDef> types, bool recordType)
        {
            var codeTemplate = @"/// <summary>此类必须在主项目程序集, 如在unity时必须是Assembly-CSharp程序集, 在控制台项目时必须在Main入口类的程序集</summary>
internal partial class SyncVarHandlerGenerate : ISyncVarHandler
{
    public virtual int SortingOrder => 0;

    public void Init()
    {
        SyncVarGetSetHelper.Cache.Clear();
--
        SyncVarGetSetHelper.Cache.Add(typeof(TARGETTYPE), new Dictionary<string, SyncVarInfo>() {
--
            { ""FIELDNAME"", new SyncVarInfoPtr<TARGETTYPE, FIELDTYPE>(FIELDNAME) },
--
        });
--
    }
--
    internal virtual void FIELDNAME(TARGETTYPE self, ref FIELDTYPE FIELDNAME, ushort id, ref ISegment segment, SyncVarInfo syncVar, bool isWrite, Action<FIELDTYPE, FIELDTYPE> onValueChanged) 
    {
        if (isWrite)
        {
            if (JUDGE)
                return;
            if (segment == null)
                segment = BufferPool.Take();
            segment.Write(id);
            GETTYPE
            TYPETOINDEX
            var pos = segment.Position;
            HANDLER
            var end = segment.Position;
            segment.Position = pos;
            FIELDNAME = READVALUE
            CHECKVALUE
            segment.Position = end;
            syncVar.writeCount++;
            syncVar.writeBytes += end - pos;
        }
        else 
        {
            INDEXTOTYPE
            var pos = segment.Position;
            var FIELDNAME1 = READVALUE
            CHECKVALUE1
            var end = segment.Position;
            segment.Position = pos;
            FIELDNAME = READVALUE
            CHECKVALUE
            segment.Position = end;
            if (onValueChanged != null)
                onValueChanged(self.FIELDNAME, FIELDNAME1);
            self.FIELDNAME = FIELDNAME1;
            syncVar.readCount++;
            syncVar.readBytes += end - pos;
        }
    }
--
    internal virtual void FIELDNAME(TARGETTYPE self, ref FIELDTYPE FIELDNAME, ushort id, ref ISegment segment, SyncVarInfo syncVar, bool isWrite, Action<FIELDTYPE, FIELDTYPE> onValueChanged) 
    {
        if (isWrite)
        {
            if (JUDGE)
                return;
            if (segment == null)
                segment = BufferPool.Take();
            FIELDNAME = self.FIELDNAME;
            segment.Write(id);
#if UNITY_EDITOR
            var path = UnityEditor.AssetDatabase.GetAssetPath(FIELDNAME);
            segment.Write(path);
#endif
        }
        else 
        {
#if UNITY_EDITOR
            var path = segment.ReadString();
            var FIELDNAME1 = UnityEditor.AssetDatabase.LoadAssetAtPath<FIELDTYPE>(path);
            self.FIELDNAME = FIELDNAME1;
            if (onValueChanged != null)
                onValueChanged(self.FIELDNAME, FIELDNAME1);
            FIELDNAME = FIELDNAME1;
#endif
        }
    }
--

--
}";

            var codes = codeTemplate.Split(new string[] { "--" }, 0);
            var sb = new StringBuilder();
            var setgetSb = new StringBuilder();
            sb.Append(codes[0]);
            var equalsSbs = new HashSet<string>();
            var streams = new Dictionary<string, ModuleDefMD>();
            var typeLoop = new HashSet<string>();
            foreach (var type in types)
            {
                var dictSB = new StringBuilder();
                foreach (var field in type.Fields)//字段里面包含了属性字段
                {
                    var hasAttribute = false;
                    var nonSerialized = false;
                    foreach (var item in field.CustomAttributes)
                    {
                        if (item.TypeFullName == "Net.Share.SyncVar")
                            hasAttribute = true;
                        else if (item.TypeFullName == "Net.Serialize.NonSerialized")
                            nonSerialized = true;
                    }
                    if (!hasAttribute)
                        continue;
                    if (nonSerialized)
                        continue;
                    if (field.IsPrivate)
                        continue;
                    TypeSig ft = field.FieldType;
                    var gt = ft as GenericInstSig;
                    var fieldType = AssemblyHelper.GetCodeTypeName(field.FieldType.FullName);
                    var fieldName = field.Name.String;
                    var code = codes[2].Replace("TARGETTYPE", type.FullName);
                    code = code.Replace("FIELDTYPE", fieldType);
                    code = code.Replace("FIELDNAME", fieldName);
                    dictSB.Append(code);

                    var isUnityObject = IsUnityObjectSubclassOf(ft);
                    if (ft.IsArray | ft.IsSZArray | ft.IsValueArray | ft.IsGenericInstanceType)
                    {
                        TypeSig itemType;
                        if (ft.IsGenericInstanceType)
                            itemType = gt.GenericArguments[0];
                        else
                            itemType = ft.Next;
                        isUnityObject = IsUnityObjectSubclassOf(itemType);
                    }
                    if (!isUnityObject)
                    {
                        code = codes[5].Replace("TARGETTYPE", type.FullName);
                        code = code.Replace("FIELDTYPE", fieldType);
                        code = code.Replace("FIELDNAME", fieldName);
                    }
                    else
                    {
                        code = codes[6].Replace("TARGETTYPE", type.FullName);
                        code = code.Replace("FIELDTYPE", fieldType);
                        code = code.Replace("FIELDNAME", fieldName);
                    }

                    var typeCode = GetTypeCode(ft);
                    if (typeCode != TypeCode.Object)
                    {
                        code = code.Replace("GETTYPE", "");
                        code = code.Replace("TYPETOINDEX", "");
                        code = code.Replace("INDEXTOTYPE", "");
                        code = code.Replace("HANDLER", $"segment.Write(self.{fieldName});");
                        code = code.Replace("READVALUE", $"segment.Read{typeCode}();");
                        code = code.Replace("JUDGE", $"self.{fieldName} == {fieldName}");
                        code = code.Replace("CHECKVALUE1", "");
                        code = code.Replace("CHECKVALUE", "");
                    }
                    else if (ft.IsArray | ft.IsSZArray | ft.IsValueArray)
                    {
                        var itemType = ft.Next;
                        typeCode = GetTypeCode(itemType);
                        if (typeCode != TypeCode.Object)
                        {
                            code = code.Replace("GETTYPE", "");
                            code = code.Replace("TYPETOINDEX", "");
                            code = code.Replace("INDEXTOTYPE", "");
                            code = code.Replace("HANDLER", $"segment.Write(self.{fieldName});");
                            code = code.Replace("READVALUE", $"segment.Read{typeCode}Array();");
                            code = code.Replace("CHECKVALUE1", "");
                            code = code.Replace("CHECKVALUE", "");
                        }
                        else
                        {
                            if (recordType)
                            {
                                code = code.Replace("GETTYPE", $"var type = self.{fieldName}.GetType();");
                                code = code.Replace("TYPETOINDEX", $"segment.Write(NetConvertBinary.TypeToIndex(type));");
                                code = code.Replace("INDEXTOTYPE", $"var type = NetConvertBinary.IndexToType(segment.ReadUInt16());");
                            }
                            else 
                            {
                                code = code.Replace("GETTYPE", "");
                                code = code.Replace("TYPETOINDEX", "");
                                code = code.Replace("INDEXTOTYPE", "");
                            }
                            code = code.Replace("HANDLER", $"NetConvertBinary.SerializeObject(segment, self.{fieldName}, {(recordType ? "true" : "false")}, true);");
                            code = code.Replace("READVALUE", $"NetConvertBinary.DeserializeObject<{fieldType}>(segment, {(recordType ? $"type," : "")} false, {(recordType ? "true" : "false")}, true);");
                            code = code.Replace("CHECKVALUE1", $"if({fieldName}1 == null) {fieldName}1 = new {fieldType}{{}};");
                            code = code.Replace("CHECKVALUE", $"if({fieldName} == null) {fieldName} = new {fieldType}{{}};");
                        }
                        code = code.Replace("JUDGE", $"Equals(self.{fieldName}, {fieldName})");
                        AddArrayOrListEquals(ft, streams, equalsSbs, typeLoop);
                    }
                    else if (ft.IsGenericInstanceType)
                    {
                        var gts = gt.GenericArguments;
                        if (gts.Count == 1)
                        {
                            var itemType = gts[0];
                            typeCode = GetTypeCode(itemType);
                            if (typeCode != TypeCode.Object)
                            {
                                code = code.Replace("GETTYPE", "");
                                code = code.Replace("TYPETOINDEX", "");
                                code = code.Replace("INDEXTOTYPE", "");
                                code = code.Replace("HANDLER", $"segment.Write(self.{fieldName});");
                                code = code.Replace("READVALUE", $"segment.Read{typeCode}List();");
                            }
                            else
                            {
                                if (recordType)
                                {
                                    code = code.Replace("GETTYPE", $"var type = self.{fieldName}.GetType();");
                                    code = code.Replace("TYPETOINDEX", $"segment.Write(NetConvertBinary.TypeToIndex(type));");
                                    code = code.Replace("INDEXTOTYPE", $"var type = NetConvertBinary.IndexToType(segment.ReadUInt16());");
                                }
                                else
                                {
                                    code = code.Replace("GETTYPE", "");
                                    code = code.Replace("TYPETOINDEX", "");
                                    code = code.Replace("INDEXTOTYPE", "");
                                }
                                code = code.Replace("HANDLER", $"NetConvertBinary.SerializeObject(segment, self.{fieldName}, {(recordType ? "true" : "false")}, true);");
                                code = code.Replace("READVALUE", $"NetConvertBinary.DeserializeObject<{fieldType}>(segment, {(recordType ? $"type," : "")} false, {(recordType ? "true" : "false")}, true);");
                            }
                            code = code.Replace("JUDGE", $"Equals(self.{fieldName}, {fieldName})");
                            AddArrayOrListEquals(ft, streams, equalsSbs, typeLoop);
                        }
                        else
                        {
                            if (recordType)
                            {
                                code = code.Replace("GETTYPE", $"var type = self.{fieldName}.GetType();");
                                code = code.Replace("TYPETOINDEX", $"segment.Write(NetConvertBinary.TypeToIndex(type));");
                                code = code.Replace("INDEXTOTYPE", $"var type = NetConvertBinary.IndexToType(segment.ReadUInt16());");
                            }
                            else
                            {
                                code = code.Replace("GETTYPE", "");
                                code = code.Replace("TYPETOINDEX", "");
                                code = code.Replace("INDEXTOTYPE", "");
                            }
                            code = code.Replace("HANDLER", $"NetConvertBinary.SerializeObject(segment, self.{fieldName}, {(recordType ? "true" : "false")}, true);");
                            code = code.Replace("READVALUE", $"NetConvertBinary.DeserializeObject<{fieldType}>(segment, {(recordType ? $"type," : "")} false, {(recordType ? "true" : "false")}, true);");
                        }
                        code = code.Replace("CHECKVALUE1", "");
                        code = code.Replace("CHECKVALUE", "");
                    }
                    else if (isUnityObject)
                    {
                        code = code.Replace("GETTYPE", "");
                        code = code.Replace("TYPETOINDEX", "");
                        code = code.Replace("INDEXTOTYPE", "");
                        code = code.Replace("HANDLER", $"segment.Write(UnityEditor.AssetDatabase.GetAssetPath(self.{fieldName}));");
                        code = code.Replace("READVALUE", $"segment.ReadString();");
                        code = code.Replace("JUDGE", $"Equals(self.{fieldName}, {fieldName})");
                        code = code.Replace("CHECKVALUE1", "");
                        code = code.Replace("CHECKVALUE", "");
                    }
                    else
                    {
                        if (recordType)
                        {
                            code = code.Replace("GETTYPE", $"var type = self.{fieldName}.GetType();");
                            code = code.Replace("TYPETOINDEX", $"segment.Write(NetConvertBinary.TypeToIndex(type));");
                            code = code.Replace("INDEXTOTYPE", $"var type = NetConvertBinary.IndexToType(segment.ReadUInt16());");
                        }
                        else
                        {
                            code = code.Replace("GETTYPE", "");
                            code = code.Replace("TYPETOINDEX", "");
                            code = code.Replace("INDEXTOTYPE", "");
                        }
                        code = code.Replace("HANDLER", $"NetConvertBinary.SerializeObject(segment, self.{fieldName}, {(recordType ? "true" : "false")}, true);");
                        code = code.Replace("READVALUE", $"NetConvertBinary.DeserializeObject<{fieldType}>(segment, {(recordType ? $"type," : "")} false, {(recordType ? "true" : "false")}, true);");
                        code = code.Replace("JUDGE", $"Equals(self.{fieldName}, {fieldName})");
                        code = code.Replace("CHECKVALUE1", "");
                        code = code.Replace("CHECKVALUE", "");
                        AddEquals(ft, streams, equalsSbs, typeLoop);
                    }
                    setgetSb.Append(code);
                }
                if (dictSB.Length > 0)
                {
                    dictSB.Append(codes[3]);

                    var dictSB1 = new StringBuilder();
                    var code = codes[1].Replace("TARGETTYPE", type.FullName);
                    dictSB1.Append(code);
                    dictSB1.Append(dictSB);

                    sb.Append(dictSB1.ToString());
                }
            }

            var equalsSb = new StringBuilder();
            equalsSb.AppendLine();
            foreach (var item in equalsSbs)
            {
                equalsSb.AppendLine(item);
            }
            codes[7] = equalsSb.ToString();

            sb.Append(codes[4]);
            sb.Append(setgetSb.ToString());
            sb.Append(codes[7]); 
            sb.Append(codes[8]);
            var text = sb.ToString();

            foreach (var item in streams.Values)
                item?.Dispose();

            return text;
        }

        private static void AddArrayOrListEquals(TypeSig typeSig, Dictionary<string, ModuleDefMD> streams, HashSet<string> equalsSbs, HashSet<string> typeLoop)
        {
            var typeSig1 = typeSig;
            var typeDef = typeSig.TryGetTypeDef();
        J: if (typeDef != null)
            {
                var equalsSb = new StringBuilder();
                var fullName = typeSig.FullName.Replace("`1", "");
                equalsSb.AppendLine($"\tinternal virtual bool Equals({fullName} a, {fullName} b)");
                equalsSb.AppendLine("\t{");
                equalsSb.AppendLine($"\t\tif (a == null) return true;");
                equalsSb.AppendLine($"\t\tif (b == null) return false; //到这里a不可能为null, 但是b如果为null, 就是不相等了");
                if (typeSig.IsGenericInstanceType)
                {
                    equalsSb.AppendLine($"\t\tif (a.Count != b.Count) return false;");
                    equalsSb.AppendLine($"\t\tfor(int i = 0; i < a.Count; i++)");
                }
                else
                {
                    equalsSb.AppendLine($"\t\tif (a.Length != b.Length) return false;");
                    equalsSb.AppendLine($"\t\tfor(int i = 0; i < a.Length; i++)");
                }
                equalsSb.AppendLine("\t\t{");
                equalsSb.AppendLine($"\t\t\tif (!Equals(a[i], b[i])) return false;");
                typeSig = typeDef.ToTypeSig();
                if (typeSig != null)
                {
                    if (typeLoop.Add(typeSig.FullName)) 
                    {
                        if (typeSig.IsArray | typeSig.IsValueArray | typeSig.IsSZArray | typeSig.IsSingleOrMultiDimensionalArray | typeSig.IsGenericInstanceType)
                            AddArrayOrListEquals(typeSig, streams, equalsSbs, typeLoop);
                        else
                            AddEquals(typeSig, streams, equalsSbs, typeLoop);
                    }
                }
                equalsSb.AppendLine("\t\t}");
                equalsSb.AppendLine("\t\treturn true;");
                equalsSb.AppendLine("\t}");
                equalsSbs.Add(equalsSb.ToString());
            }
            else if (streams.TryGetValue(typeSig1.DefinitionAssembly.Name, out var module))
            {
                string fullName;
                if (typeSig1.IsArray | typeSig1.IsValueArray | typeSig1.IsSZArray | typeSig1.IsSingleOrMultiDimensionalArray)
                    fullName = typeSig1.Next.FullName;
                else if (typeSig1.IsGenericInstanceType)
                {
                    var gt = typeSig1 as GenericInstSig;
                    typeSig1 = gt.GenericArguments[0];
                    goto J;
                }
                else fullName = typeSig1.FullName;
                typeDef = module.FindReflection(fullName);
                if (typeDef != null)
                    goto J;
            }
            else
            {
                var assembly = AssemblyHelper.GetRunAssembly(typeSig.DefinitionAssembly.Name);
                if (assembly != null)
                {
                    if (assembly.IsDynamic)
                        return;
                    if (string.IsNullOrEmpty(assembly.Location))
                        return;
                    var path = assembly.Location;
                    if (File.Exists(path))
                    {
                        module = ModuleDefMD.Load(path, typeSig.Module.Context);
                        streams.Add(typeSig.DefinitionAssembly.Name, module);
                        var fullName = string.Empty;
                        if (typeSig.IsArray | typeSig.IsValueArray | typeSig.IsSZArray | typeSig.IsSingleOrMultiDimensionalArray)
                            fullName = typeSig.Next.FullName;
                        else if (typeSig.IsGenericInstanceType)
                        {
                            var gt = typeSig as GenericInstSig;
                            var gat = gt.GenericArguments[0];
                            fullName = gat.FullName;
                            typeDef = gat.Module.FindReflection(fullName);
                            if (typeDef != null)
                                goto J;
                        }
                        typeDef = module.FindReflection(fullName);
                        if (typeDef != null)
                            goto J;
                    }
                }
            }
        }

        private static void AddEquals(TypeSig typeSig, Dictionary<string, ModuleDefMD> streams, HashSet<string> equalsSbs, HashSet<string> typeLoop)
        {
            if (!typeLoop.Add(typeSig.FullName))
                return;
            var typeDef = typeSig.TryGetTypeDef();
        J: if (typeDef != null)
            {
                var equalsSb = new StringBuilder();
                equalsSb.AppendLine($"\tinternal virtual bool Equals({typeSig.FullName} a, {typeSig.FullName} b)");
                equalsSb.AppendLine("\t{");
                if (typeSig.IsClassSig)
                {
                    equalsSb.AppendLine($"\t\tif (a == null) return true;");
                    equalsSb.AppendLine($"\t\tif (b == null) return false; //到这里a不可能为null, 但是b如果为null, 就是不相等了");
                }
                else if (GetTypeCode(typeSig) != TypeCode.Object) //基类
                {
                    equalsSb.AppendLine($"\t\tif (a != b) return false;");
                    goto J1;
                }
                var nullEquals = true;
                foreach (var ft1 in typeDef.Fields)
                {
                    if (!ft1.IsPublic | ft1.IsStatic)
                        continue;
                    var nonSerialized = false;
                    foreach (var item in ft1.CustomAttributes)
                        if (item.TypeFullName == "Net.Serialize.NonSerialized")
                            nonSerialized = true;
                    if (nonSerialized)
                        continue;
                    nullEquals = false;
                    if (ft1.FieldType.IsPrimitive)
                    {
                        if (ft1.Name.EndsWith("value__")) //如果后面包含了value__ 暂时识别为枚举类型
                            equalsSb.AppendLine($"\t\tif (a != b) return false;");
                        else
                            equalsSb.AppendLine($"\t\tif (a.{ft1.Name} != b.{ft1.Name}) return false;");
                        AddEquals(ft1.FieldType, streams, equalsSbs, typeLoop);
                    }
                    else if (ft1.FieldType.IsArray | ft1.FieldType.IsSZArray | ft1.FieldType.IsValueArray | ft1.FieldType.IsSingleOrMultiDimensionalArray)
                    {
                        equalsSb.AppendLine($"\t\tif (!Equals(a.{ft1.Name}, b.{ft1.Name})) return false;");
                        AddArrayOrListEquals(ft1.FieldType, streams, equalsSbs, typeLoop);
                    }
                    else if (ft1.FieldType.IsGenericInstanceType) 
                    {
                        equalsSb.AppendLine($"\t\tif (!Equals(a.{ft1.Name}, b.{ft1.Name})) return false;");
                        AddArrayOrListEquals(ft1.FieldType, streams, equalsSbs, typeLoop);
                    }
                    else
                    {
                        equalsSb.AppendLine($"\t\tif (!Equals(a.{ft1.Name}, b.{ft1.Name})) return false;");
                        AddEquals(ft1.FieldType, streams, equalsSbs, typeLoop);
                    }
                }
                if (nullEquals)
                    return;
                J1: equalsSb.AppendLine("\t\treturn true;");
                equalsSb.AppendLine("\t}");
                equalsSbs.Add(equalsSb.ToString());
            }
            else if (streams.TryGetValue(typeSig.DefinitionAssembly.Name, out var module))
            {
                typeDef = module.FindReflection(typeSig.FullName);
                if (typeDef != null)
                    goto J;
            }
            else
            {
                var assembly = AssemblyHelper.GetRunAssembly(typeSig.DefinitionAssembly.Name);
                if (assembly != null)
                {
                    if (assembly.IsDynamic)
                        return;
                    if (string.IsNullOrEmpty(assembly.Location))
                        return;
                    var path = assembly.Location;
                    if (File.Exists(path))
                    {
                        module = ModuleDefMD.Load(path, typeSig.Module.Context);
                        streams.Add(typeSig.DefinitionAssembly.Name, module);
                        typeDef = module.FindReflection(typeSig.FullName);
                        if (typeDef != null)
                            goto J;
                    }
                }
            }
        }

        public static string InvokeRpcClientBuild(List<TypeDef> types)
        {
            var str = @"internal static class RpcInvokeHelper
{
--
    internal static void METHOD(this Net.Client.ClientBase client PARAMETERS) 
    {
        BODY
        SEND
    }
--
}";
            var codes = str.Split(new string[] { "--" }, 0);
            var sb = new StringBuilder();
            sb.Append(codes[0]);
            var dic = new Dictionary<string, int>();
            foreach (var type in types)
            {
                var dictSB = new StringBuilder();
                foreach (var method in type.Methods)
                {
                    CustomAttribute attribute = null;
                    foreach (var item in method.CustomAttributes)
                    {
                        if (item.TypeFullName == "Net.Share.Rpc")
                        {
                            attribute = item;
                            break;
                        }
                    }
                    if (attribute == null)
                        continue;
                    var paramas = method.Parameters;
                    string parStr = "";
                    string bodyStr = "";
                    string sendStr = $"client.SendRT(\"{method.Name}\"";
                    bool safeCmd = false;
                    bool isServer = false;
                    bool isClient = false;
                    if (type.BaseType != null)
                    {
                        if (type.BaseType.ToTypeSig() is GenericInstSig gt)
                        {
                            var gts = gt.GenericArguments;
                            if (gts.Count == 2)
                                isServer = type.BaseType.ReflectionNamespace == "Net.Server";
                        }
                        isClient = type.BaseType.FullName == "Net.Server.NetPlayer";
                    }
                    if (attribute.HasConstructorArguments)
                    {
                        foreach (var item in attribute.ConstructorArguments)
                        {
                            var typeCode = GetTypeCode(item.Type);
                            if (typeCode == TypeCode.Byte)
                            {
                                if (item.Value.Equals((byte)2))
                                {
                                    safeCmd = true;
                                }
                            }
                            if (typeCode == TypeCode.UInt16)
                            {
                                if (!item.Value.Equals((ushort)0))
                                {
                                    sendStr = $"client.SendRT({(isClient ? "NetCmd.EntityRpc, " : "")} (ushort){item.Value}";
                                }
                            }
                        }
                    }
                    else if (attribute.HasNamedArguments)
                    {
                        foreach (var item in attribute.NamedArguments)
                        {
                            if (item.Name == "cmd")
                            {
                                if (item.Value.Equals((byte)2))
                                {
                                    safeCmd = true;
                                }
                            }
                            else if (item.Name == "hash")
                            {
                                if (!item.Value.Equals((ushort)0))
                                {
                                    sendStr = $"client.SendRT({(isClient ? "NetCmd.EntityRpc, " : "")} (ushort){item.Value}";
                                }
                            }
                        }
                    }
                    else if (isClient)
                    {
                        sendStr = $"client.SendRT(NetCmd.EntityRpc, \"{method.Name}\"";
                    }
                    foreach (var parama in paramas)
                    {
                        if (string.IsNullOrEmpty(parama.Name))
                            continue;
                        if (parama.Index == 1 & safeCmd & isServer)
                            continue;
                        var pt = parama.Type.FullName;
                        if (pt.Contains("`"))
                        {
                            var fff = pt.Split('`');
                            fff[0] += "<";
                            var gt = parama.Type as GenericInstSig;
                            foreach (var item in gt.GenericArguments)
                            {
                                fff[0] += $"{item.FullName},";
                            }
                            fff[0] = fff[0].TrimEnd(',');
                            fff[0] += ">";
                            pt = fff[0];
                        }
                        if (parama.Type.IsArray | parama.Type.IsGenericInstanceType | parama.Type.IsSZArray | parama.Type.IsValueArray)
                        {
                            bodyStr += $"object {parama.Name}1 = {parama.Name};\r\n\t\t";
                            sendStr += $", {parama.Name}1";
                        }
                        else
                        {
                            sendStr += $", {parama.Name}";
                        }
                        parStr += $", {pt} {parama.Name}";
                    }
                    sendStr += ");";
                    var code = codes[1].Replace("METHOD", method.Name);
                    code = code.Replace("PARAMETERS", parStr);
                    code = code.Replace("BODY", bodyStr);
                    code = code.Replace("SEND", sendStr);
                    var funcs = code.Split(new string[] { "\r\n" }, 0);
                    if (dic.TryGetValue(funcs[1], out var num))
                    {
                        var pos = code.IndexOf('(');
                        code = code.Insert(pos, (isServer ? "_S" : isClient ? "_C" : "_N") + (num > 0 ? num.ToString() : ""));
                        dic[funcs[1]]++;
                        funcs = code.Split(new string[] { "\r\n" }, 0);
                    }
                    dic.Add(funcs[1], 0);
                    dictSB.Append(code);
                }
                if (dictSB.Length > 0)
                {
                    sb.Append(dictSB.ToString());
                }
            }
            sb.Append(codes[2]);
            var text = sb.ToString();
            return text;
        }

        public static string InvokeRpcServerBuild(List<TypeDef> types, int no, string serverType, string playerType)
        {
            var str = @"internal static class RpcInvokeHelperNO
{
--
    internal static void METHOD(this PLAYER client PARAMETERS) 
    {
        BODY
        SEND
    }
--
}";
            str = str.Replace("NO", no.ToString());
            var codes = str.Split(new string[] { "--" }, 0);
            var sb = new StringBuilder();
            sb.Append(codes[0]);
            var dict = new Dictionary<string, List<MethodDef>>();
            foreach (var type in types)
            {
                var dictSB = new StringBuilder();
                foreach (var method in type.Methods)
                {
                    CustomAttribute attribute = null;
                    foreach (var item in method.CustomAttributes)
                    {
                        if (item.TypeFullName == "Net.Share.Rpc")
                        {
                            attribute = item;
                            break;
                        }
                    }
                    if (attribute == null)
                        continue;
                    var paramas = method.Parameters;
                    string parStr = "";
                    string bodyStr = "";
                    string sendStr = $"{serverType}.Instance.SendRT(client, \"{method.Name}\"";
                    if (dict.TryGetValue(method.Name.String, out var methods))
                    {
                        var has = true;
                        foreach (var method1 in methods)
                        {
                            if (method.Parameters.Count == method1.Parameters.Count)
                            {
                                for (int i = 0; i < method.Parameters.Count; i++)
                                {
                                    if (string.IsNullOrEmpty(method.Parameters[i].Name) & string.IsNullOrEmpty(method1.Parameters[i].Name))
                                        continue;
                                    if (method.Parameters[i].Type.FullName != method1.Parameters[i].Type.FullName)
                                    {
                                        has = false;
                                        break;
                                    }
                                }
                            }
                        }
                        if (has)
                            continue;
                    }
                    else
                    {
                        dict.Add(method.Name.String, new List<MethodDef>() { method });
                    }
                    foreach (var parama in paramas)
                    {
                        if (string.IsNullOrEmpty(parama.Name))
                            continue;
                        var pt = parama.Type.FullName;
                        if (pt.Contains("`"))
                        {
                            var fff = pt.Split('`');
                            fff[0] += "<";
                            var gt = parama.Type as GenericInstSig;
                            foreach (var item in gt.GenericArguments)
                            {
                                fff[0] += $"{item.FullName},";
                            }
                            fff[0] = fff[0].TrimEnd(',');
                            fff[0] += ">";
                            pt = fff[0];
                        }
                        if (parama.Type.IsArray | parama.Type.IsGenericInstanceType | parama.Type.IsSZArray | parama.Type.IsValueArray)
                        {
                            bodyStr += $"object {parama.Name}1 = {parama.Name};\r\n\t\t";
                            sendStr += $", {parama.Name}1";
                        }
                        else
                        {
                            sendStr += $", {parama.Name}";
                        }
                        parStr += $", {pt} {parama.Name}";
                    }
                    sendStr += ");";
                    var code = codes[1].Replace("METHOD", method.Name);
                    code = code.Replace("PARAMETERS", parStr);
                    code = code.Replace("BODY", bodyStr);
                    code = code.Replace("SEND", sendStr);
                    code = code.Replace("PLAYER", playerType);
                    dictSB.Append(code);
                }
                if (dictSB.Length > 0)
                {
                    sb.Append(dictSB.ToString());
                }
            }
            sb.Append(codes[2]);
            var text = sb.ToString();
            return text;
        }

        public static void ScriptSequencePoint(List<TypeDef> types)
        {
            var cache = new Dictionary<string, SequencePoint>();
            foreach (var type in types)
            {
                //rpc部分
                foreach (var method in type.Methods)
                {
                    CustomAttribute attribute = null;
                    foreach (var item in method.CustomAttributes)
                    {
                        if (item.TypeFullName == "Net.Share.Rpc")
                        {
                            attribute = item;
                            break;
                        }
                    }
                    if (attribute == null)
                        continue;
                    var pdb = method.Body.PdbMethod;
                    if (pdb == null)
                        break;
                    var sequence = pdb.Scope.Start.SequencePoint;
                    if (sequence == null)// async方法得不到源码行
                        continue;
                    var path = sequence.Document.Url;
                    //相对于Assets路径
                    var uri = new Uri(UnityEngine.Application.dataPath);
                    var relativeUri = uri.MakeRelativeUri(new Uri(path));
                    path = relativeUri.ToString();
                    path = path.Replace('\\', '/');
                    cache[type.FullName + "." + method.Name] = new SequencePoint(path, sequence.StartLine - 1);
                }
                //状态机部分
                {
                    var baseType = type.BaseType;
                    bool isHas = false;
                    while (baseType != null)
                    {
                        if (baseType.FullName == "GameDesigner.IBehaviour")
                        {
                            isHas = true;
                            break;
                        }
                        baseType = baseType.GetBaseType();
                    }
                    if (!isHas)
                        continue;
                    if (type.Methods.Count == 0)
                        continue;
                    var method = type.Methods[0];
                    var pdb = method.Body.PdbMethod;
                    if (pdb == null)
                        continue;
                    var sequence = pdb.Scope.Start.SequencePoint;
                    if (sequence == null)// async方法得不到源码行
                        continue;
                    var path = sequence.Document.Url;
                    //相对于Assets路径
                    var uri = new Uri(UnityEngine.Application.dataPath);
                    var relativeUri = uri.MakeRelativeUri(new Uri(path));
                    path = relativeUri.ToString();
                    path = path.Replace('\\', '/');
                    cache[type.FullName] = new SequencePoint(path, sequence.StartLine - 1);
                }
            }
            PersistHelper.Serialize(cache, "sourceCodeTextPoint.json");
        }

        public static void OnScriptCompilation(InvokeHelperConfig config, bool generateClientSyncVar, bool generateServerSyncVar)
        {
            var streams = new List<ModuleDefMD>();
            var clientTypes = new List<TypeDef>();
            foreach (var file in config.dllPaths)
            {
                if (string.IsNullOrEmpty(file))
                    continue;
                if (!File.Exists(file))
                    continue;
                var modCtx = ModuleDef.CreateModuleContext();
                var module = ModuleDefMD.Load(file, modCtx);
                streams.Add(module);
                var types = module.Types;
                foreach (var type in types.Where(t => !t.IsInterface & !t.IsEnum))
                {
                    if (type.DefinitionAssembly.Name == "GameDesigner")
                        continue;
                    if (type.HasInterfaces)
                    {
                        bool hasIDataRow = false;
                        foreach (var item in type.Interfaces)
                        {
                            if (item.Interface.Name == "IDataRow")
                            {
                                hasIDataRow = true;
                                break;
                            }
                        }
                        if (hasIDataRow)
                            continue;
                    }
                    clientTypes.Add(type);
                }
            }

            var serverTypes = new List<List<TypeDef>>();
            foreach (var data in config.rpcConfig)
            {
                var serverTypes1 = new List<TypeDef>();
                foreach (var file in data.dllPaths)
                {
                    if (!File.Exists(file))
                        continue;
                    var modCtx = ModuleDef.CreateModuleContext();
                    var module = ModuleDefMD.Load(file, modCtx);
                    streams.Add(module);
                    var types = module.Types;
                    foreach (var type in types.Where(t => !t.IsInterface & !t.IsEnum))
                    {
                        if (type.DefinitionAssembly.Name == "GameDesigner")
                            continue;
                        if (type.HasInterfaces)
                        {
                            bool hasIDataRow = false;
                            foreach (var item in type.Interfaces)
                            {
                                if (item.Interface.Name == "IDataRow")
                                {
                                    hasIDataRow = true;
                                    break;
                                }
                            }
                            if (hasIDataRow)
                                continue;
                        }
                        serverTypes1.Add(type);
                    }
                }
                serverTypes.Add(serverTypes1);
            }

            string text = @"using Net.Helper;
using Net.Serialize;
using Net.Share;
using Net.System;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

";
            if (generateClientSyncVar)
                text += SyncVarBuild(clientTypes, config.recordType) + "\r\n\r\n";
            else
                text += "/**\r\n" + SyncVarBuild(clientTypes, config.recordType) + "*/\r\n\r\n";
            var serverTypes3 = new List<TypeDef>();
            foreach (var item in serverTypes)
                serverTypes3.AddRange(item);
            if(config.collectRpc)
                text += InvokeRpcClientBuild(serverTypes3) + "\r\n\r\n";//客户端要收集服务器的rpc才能识别
            text += @"/// <summary>SyncVar动态编译定位路径</summary>
internal static class HelperFileInfo 
{
    internal static string GetPath()
    {
        return GetClassFileInfo();
    }

    internal static string GetClassFileInfo([CallerFilePath] string sourceFilePath = """")
    {
        return sourceFilePath;
    }
}";
            ScriptSequencePoint(clientTypes);
            if (string.IsNullOrEmpty(config.savePath))
                goto J;
            if (!Directory.Exists(config.savePath))
                Directory.CreateDirectory(config.savePath);
            File.WriteAllText(config.savePath + "/SyncVarHandlerGenerate.cs", text);

            for (int i = 0; i < config.rpcConfig.Count; i++)
            {
                var serverTypes2 = serverTypes[i];
                string text1 = @"using Net.Helper;
using Net.Serialize;
using Net.Share;
using Net.System;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

";
                if (generateServerSyncVar)
                    text1 += SyncVarBuild(serverTypes2, config.recordType) + "\r\n\r\n";
                else
                    text1 += "/**\r\n" + SyncVarBuild(serverTypes2, config.recordType) + "*/\r\n\r\n";
                if (config.rpcConfig[i].collectRpc)
                {
                    int num = 1;
                    foreach (var type in serverTypes2)
                    {
                        if (type.ReflectionNamespace == "Net.Server")
                            continue;
                        if (type.BaseType == null)
                            continue;
                        if (type.BaseType.ReflectionNamespace != "Net.Server")
                            continue;
                        var serverType = type.FullName;
                        if (!(type.BaseType.ToTypeSig() is GenericInstSig gt))
                            continue;
                        var gts = gt.GenericArguments;
                        if (gts.Count != 2)
                            continue;
                        var player = gts[0];
                        text1 += InvokeRpcServerBuild(clientTypes, num++, serverType, player.FullName) + "\r\n\r\n";//服务器需要收集客户端的rpc才能调用
                    }
                }
                text1 += @"/// <summary>定位辅助类路径</summary>
internal static class HelperFileInfo 
{
    internal static string GetPath()
    {
        return GetClassFileInfo();
    }

    internal static string GetClassFileInfo([CallerFilePath] string sourceFilePath = """")
    {
        return sourceFilePath;
    }
}";
                var csprojPath = config.rpcConfig[i].csprojPath;
                csprojPath = Path.GetFullPath(csprojPath.Replace('/', '\\'));
                var path1 = config.rpcConfig[i].savePath + "/SyncVarHandlerGenerate.cs";
                path1 = Path.GetFullPath(path1.Replace('/', '\\'));

                //相对于Assets路径
                var uri = new Uri(csprojPath);
                var relativeUri = uri.MakeRelativeUri(new Uri(path1));
                var relativePath = relativeUri.ToString();

                if (!File.Exists(csprojPath))
                    continue;
                var xml = new XmlDocument();
                xml.Load(csprojPath);
                XmlNodeList node_list;
                var documentElement = xml.DocumentElement;
                var namespaceURI = xml.DocumentElement.NamespaceURI;
                if (!string.IsNullOrEmpty(namespaceURI))
                {
                    var nsMgr = new XmlNamespaceManager(xml.NameTable); nsMgr.AddNamespace("ns", namespaceURI);
                    node_list = xml.SelectNodes("/ns:Project/ns:ItemGroup", nsMgr);
                }
                else node_list = xml.SelectNodes("/Project/ItemGroup");
                bool exist = false;
                foreach (XmlNode node in node_list)
                {
                    var node_child = node.ChildNodes;
                    foreach (XmlNode child_node in node_child)
                    {
                        if (child_node.LocalName != "Compile")
                            continue;
                        var value = child_node.Attributes["Include"].Value;
                        if (value.Contains("SyncVarHandlerGenerate.cs"))
                        {
                            if (value != relativePath | !File.Exists(value))
                            {
                                child_node.Attributes["Include"].Value = relativePath;
                                xml.Save(csprojPath);
                            }
                            exist = true;
                            break;
                        }
                    }
                }
                if (!exist)
                {
                    var node = xml.CreateElement("ItemGroup", namespaceURI);
                    var e = xml.CreateElement("Compile", namespaceURI);
                    e.SetAttribute("Include", relativePath);
                    node.AppendChild(e);
                    documentElement.AppendChild(node);
                    xml.Save(csprojPath);
                }
                if (!Directory.Exists(Path.GetDirectoryName(path1)))
                    Directory.CreateDirectory(path1);
                File.WriteAllText(path1, text1);
            }
        J: foreach (var stream in streams)
            {
                stream.Dispose();
            }
        }
    }
#endif
}

using Net.Event;
using Net.Helper;
using System.Reflection;
using System.Text;
#if CORE
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
#else
using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.CSharp;
using System.CodeDom.Compiler;
#endif

public static class Fast2BuildMethod
{
    private class Member
    {
        internal string Name;
        internal bool IsPrimitive;
        internal bool IsEnum;
        internal bool IsArray;
        internal bool IsGenericType;
        internal Type Type;
        internal TypeCode TypeCode;
        internal Type ItemType;
        internal Type ItemType1;
    }

    /// <summary>
    /// 动态编译, 在unity开发过程中不需要生成绑定cs文件, 直接运行时编译使用, 当编译apk. app时才进行生成绑定cs文件
    /// </summary>
    /// <param name="types"></param>
    /// <returns></returns>
    public static bool DynamicBuild(params Type[] types)
    {
        var bindTypes = new HashSet<Type>();
        var codes = new Dictionary<string, string>();
        foreach (var type in types)
        {
            var genericCodes = new List<string>();
            var code = BuildNew(type, true, true, new List<string>(), string.Empty, bindTypes, genericCodes);
            code.AppendLine(BuildArray(type).ToString());
            code.AppendLine(BuildGeneric(typeof(List<>).MakeGenericType(type)).ToString());
            foreach (var igenericCode in genericCodes)
            {
                var index1 = igenericCode.IndexOf("struct") + 7;
                var index2 = igenericCode.IndexOf(" ", index1);
                var className = igenericCode.Substring(index1, index2 - index1);
                codes.Add(className + ".cs", igenericCode);
            }
            codes.Add(type.ToString() + "Bind.cs", code.ToString());
            bindTypes.Add(type);
        }
        codes.Add("Binding.BindingType.cs", BuildBindingType(bindTypes));
        codes.Add("BindingExtension.cs", BuildBindingExtension(bindTypes));
        var dllpaths = new HashSet<string>();
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        foreach (var assemblie in assemblies)
        {
            if (assemblie.IsDynamic)
                continue;
            var path = assemblie.Location;
            var name = Path.GetFileName(path);
            if (name.Contains("Editor"))
                continue;
            if (name.Contains("mscorlib"))
                continue;
            if (!File.Exists(path))
                continue;
            if (path.Contains("PackageCache"))
                continue;
            dllpaths.Add(path);
        }
#if !CORE
        var provider = new CSharpCodeProvider();
        var param = new CompilerParameters();
        param.ReferencedAssemblies.AddRange(dllpaths.ToArray());
        param.GenerateExecutable = false;
        param.GenerateInMemory = true;
        param.CompilerOptions = "/optimize+ /platform:x64 /target:library /unsafe /langversion:default";
        var codeFiles = new List<string>();
        foreach (var code in codes)
        {
            var tempFile = $"{Path.GetTempPath()}{code.Key}";
            File.WriteAllText(tempFile, code.Value);
            codeFiles.Add(tempFile);
        }
        var cr = provider.CompileAssemblyFromFile(param, codeFiles.ToArray());
        if (cr.Errors.HasErrors)
        {
            NDebug.LogError("编译错误：");
            foreach (CompilerError err in cr.Errors)
                NDebug.LogError(err.ErrorText);
            return false;
        }
#else
        var metadataReferences = new List<MetadataReference>();
        foreach (var dllPath in dllpaths)
            metadataReferences.Add(MetadataReference.CreateFromFile(dllPath));
        var references = metadataReferences.ToArray();
        var assemblyName = Path.GetRandomFileName();
        var syntaxTrees = new List<SyntaxTree>();
        foreach (var code in codes)
            syntaxTrees.Add(CSharpSyntaxTree.ParseText(code.Value));
        var compilation = CSharpCompilation.Create(assemblyName, syntaxTrees, references, new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, optimizationLevel: OptimizationLevel.Release));
        using (var stream = new MemoryStream())
        {
            var result = compilation.Emit(stream);
            if (!result.Success)
            {
                var failures = result.Diagnostics.Where(diagnostic => diagnostic.IsWarningAsError || diagnostic.Severity == DiagnosticSeverity.Error);
                foreach (Diagnostic diagnostic in failures)
                    NDebug.LogError($"{diagnostic.Id}: {diagnostic.GetMessage()}");
                return false;
            }
            else
            {
                stream.Seek(0, SeekOrigin.Begin);
                Assembly.Load(stream.ToArray());
            }
        }
#endif
        Net.Serialize.NetConvertFast2.Init();
        NDebug.Log("动态编译完成!");
        return true;
    }

    /// <summary>
    /// 生成所有完整的绑定类型
    /// </summary>
    /// <param name="savePath"></param>
    /// <param name="types"></param>
    /// <returns></returns>
    public static void BuildAll(string savePath, params Type[] types)
    {
        var bindTypes = new HashSet<Type>();
        foreach (var type in types)
        {
            var code = BuildNew(type, true, true, new List<string>(), savePath, bindTypes);
            code.AppendLine(BuildArray(type).ToString());
            code.AppendLine(BuildGeneric(typeof(List<>).MakeGenericType(type)).ToString());
            var className = type.ToString().Replace(".", "").Replace("+", "");
            File.WriteAllText(savePath + $"//{className}Bind.cs", code.ToString());
            bindTypes.Add(type);
        }
        BuildBindingType(new HashSet<Type>(bindTypes), savePath, 1);
        BuildBindingExtension(new HashSet<Type>(bindTypes), savePath);
    }

    public static void Build(Type type, string savePath)
    {
        var str = BuildNew(type, true, true, new List<string>(), savePath);
        var className = type.ToString().Replace(".", "").Replace("+", "");
        File.WriteAllText(savePath + $"//{className}Bind.cs", str.ToString());
    }

    public static void Build(Type type, string savePath, bool serField, bool serProperty, List<string> ignores, HashSet<Type> types = null)
    {
        var str = BuildNew(type, serField, serProperty, ignores, savePath, types);
        var className = type.ToString().Replace(".", "").Replace("+", "");
        File.WriteAllText(savePath + $"//{className}Bind.cs", str.ToString());
    }

    public static StringBuilder BuildNew(Type type, bool serField, bool serProperty, List<string> ignores, string savePath = null, HashSet<Type> types = null, List<string> genericCodes = null)
    {
        var sb = new StringBuilder();
        var sb1 = new StringBuilder();
        FieldInfo[] fields;
        if (serField)
            fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
        else
            fields = new FieldInfo[0];
        PropertyInfo[] properties;
        if (serProperty)
            properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
        else
            properties = new PropertyInfo[0];
        var members = new List<Member>();
        foreach (var field in fields)
        {
            if (field.GetCustomAttribute<Net.Serialize.NonSerialized>() != null)
                continue;
            if (ignores.Contains(field.Name))
                continue;
            var member = new Member()
            {
                IsArray = field.FieldType.IsArray,
                IsEnum = field.FieldType.IsEnum,
                IsGenericType = field.FieldType.IsGenericType,
                IsPrimitive = Type.GetTypeCode(field.FieldType) != TypeCode.Object,
                Name = field.Name,
                Type = field.FieldType,
                TypeCode = Type.GetTypeCode(field.FieldType)
            };
            if (field.FieldType.IsArray)
            {
                var itemType = field.FieldType.GetArrayItemType();
                member.ItemType = itemType;
            }
            else if (field.FieldType.GenericTypeArguments.Length == 1)
            {
                Type itemType = field.FieldType.GenericTypeArguments[0];
                member.ItemType = itemType;
            }
            else if (field.FieldType.GenericTypeArguments.Length == 2)
            {
                Type itemType = field.FieldType.GenericTypeArguments[0];
                Type itemType1 = field.FieldType.GenericTypeArguments[1];
                member.ItemType = itemType;
                member.ItemType1 = itemType1;
            }
            members.Add(member);
        }
        foreach (var property in properties)
        {
            if (property.GetCustomAttribute<Net.Serialize.NonSerialized>() != null)
                continue;
            if (!property.CanRead | !property.CanWrite)
                continue;
            if (property.GetIndexParameters().Length > 0)
                continue;
            if (ignores.Contains(property.Name))
                continue;
            var member = new Member()
            {
                IsArray = property.PropertyType.IsArray,
                IsEnum = property.PropertyType.IsEnum,
                IsGenericType = property.PropertyType.IsGenericType,
                IsPrimitive = Type.GetTypeCode(property.PropertyType) != TypeCode.Object,
                Name = property.Name,
                Type = property.PropertyType,
                TypeCode = Type.GetTypeCode(property.PropertyType)
            };
            if (property.PropertyType.IsArray)
            {
                var itemType = property.PropertyType.GetArrayItemType();
                member.ItemType = itemType;
            }
            else if (property.PropertyType.GenericTypeArguments.Length == 1)
            {
                Type itemType = property.PropertyType.GenericTypeArguments[0];
                member.ItemType = itemType;
            }
            else if (property.PropertyType.GenericTypeArguments.Length == 2)
            {
                Type itemType = property.PropertyType.GenericTypeArguments[0];
                Type itemType1 = property.PropertyType.GenericTypeArguments[1];
                member.ItemType = itemType;
                member.ItemType1 = itemType1;
            }
            members.Add(member);
        }

        var templateText = @"using Net.Serialize;
using Net.System;
using System;
using System.Collections.Generic;

namespace Binding
{
    public readonly struct {TYPENAME}Bind : ISerialize<{TYPE}>, ISerialize
    {
        public void Write({TYPE} value, ISegment stream)
        {
            int pos = stream.Position;
            stream.Position += {SIZE};
            var bits = new byte[{SIZE}];
{Split}
            if ({Condition})
            {
                NetConvertBase.SetBit(ref bits[{BITPOS}], {FIELDINDEX}, true);
                stream.Write(value.{FIELDNAME});
            }
{Split}
			if({Condition})
			{
				NetConvertBase.SetBit(ref bits[{BITPOS}], {FIELDINDEX}, true);
				var bind = new {BINDTYPE}();
				bind.Write(value.{FIELDNAME}, stream);
			}
{Split}
			if({Condition})
			{
                NetConvertBase.SetBit(ref bits[{BITPOS}], {FIELDINDEX}, true);
				var bind = new {DICTIONARYBIND}();
				bind.Write(value.{FIELDNAME}, stream);
			}
{Split}
            int pos1 = stream.Position;
            stream.Position = pos;
            stream.Write(bits, 0, {SIZE});
            stream.Position = pos1;
        }
		
        public {TYPE} Read(ISegment stream) 
        {
            var value = new {TYPE}();
            Read(ref value, stream);
            return value;
        }

		public void Read(ref {TYPE} value, ISegment stream)
		{
			var bits = stream.Read({SIZE});
{Split}
			if(NetConvertBase.GetBit(bits[{BITPOS}], {FIELDINDEX}))
				value.{FIELDNAME} = stream.{READTYPE}();
{Split}
			if(NetConvertBase.GetBit(bits[{BITPOS}], {FIELDINDEX}))
			{
				var bind = new {BINDTYPE}();
				value.{FIELDNAME} = bind.Read(stream);
			}
{Split}
			if(NetConvertBase.GetBit(bits[{BITPOS}], {FIELDINDEX}))
			{
				var bind = new {DICTIONARYBIND}();
				value.{FIELDNAME} = bind.Read(stream);
			}
{Split}
		}

        public void WriteValue(object value, ISegment stream)
        {
            Write(({TYPE})value, stream);
        }

        public object ReadValue(ISegment stream)
        {
            return Read(stream);
        }
    }
}
";
        var typeName = type.ToString().Replace(".", "").Replace("+", "");
        var fullName = type.ToString();
        templateText = templateText.Replace("{TYPENAME}", typeName);
        templateText = templateText.Replace("{TYPE}", fullName);
        templateText = templateText.Replace("{SIZE}", $"{((members.Count - 1) / 8) + 1}");

        var templateTexts = templateText.Split(new string[] { "{Split}" }, 0);

        sb.Append(templateTexts[0]);

        for (int i = 0; i < members.Count; i++)
        {
            int bitInx1 = i % 8;
            int bitPos = i / 8;
            var typecode = Type.GetTypeCode(members[i].Type);
            if (typecode != TypeCode.Object)
            {
                var templateText1 = templateTexts[1];
                templateText1 = templateText1.Replace("{BITPOS}", $"{bitPos}");
                templateText1 = templateText1.Replace("{FIELDINDEX}", $"{++bitInx1}");
                templateText1 = templateText1.Replace("{FIELDNAME}", $"{members[i].Name}");
                if (typecode == TypeCode.String)
                    templateText1 = templateText1.Replace("{Condition}", $"!string.IsNullOrEmpty(value.{members[i].Name})");
                else if (typecode == TypeCode.Boolean)
                    templateText1 = templateText1.Replace("{Condition}", $"value.{members[i].Name} != false");
                else if (typecode == TypeCode.DateTime)
                    templateText1 = templateText1.Replace("{Condition}", $"value.{members[i].Name} != default");
                else
                    templateText1 = templateText1.Replace("{Condition}", $"value.{members[i].Name} != 0");
                sb.Append(templateText1);

                var templateText2 = templateTexts[5];
                templateText2 = templateText2.Replace("{BITPOS}", $"{bitPos}");
                templateText2 = templateText2.Replace("{FIELDINDEX}", $"{bitInx1}");
                templateText2 = templateText2.Replace("{FIELDNAME}", $"{members[i].Name}");
                if(members[i].IsEnum)
                    templateText2 = templateText2.Replace("{READTYPE}", $"ReadEnum<{members[i].Type.ToString().Replace("+", ".")}>");
                else
                    templateText2 = templateText2.Replace("{READTYPE}", $"Read{typecode}");
                sb1.Append(templateText2);
            }
            else if (members[i].IsArray)
            {
                typecode = Type.GetTypeCode(members[i].ItemType);
                if (typecode != TypeCode.Object)
                {
                    var templateText1 = templateTexts[1];
                    templateText1 = templateText1.Replace("{BITPOS}", $"{bitPos}");
                    templateText1 = templateText1.Replace("{FIELDINDEX}", $"{++bitInx1}");
                    templateText1 = templateText1.Replace("{FIELDNAME}", $"{members[i].Name}");
                    templateText1 = templateText1.Replace("{Condition}", $"value.{members[i].Name} != null");
                    sb.Append(templateText1);

                    var templateText2 = templateTexts[5];
                    templateText2 = templateText2.Replace("{BITPOS}", $"{bitPos}");
                    templateText2 = templateText2.Replace("{FIELDINDEX}", $"{bitInx1}");
                    templateText2 = templateText2.Replace("{FIELDNAME}", $"{members[i].Name}");
                    templateText2 = templateText2.Replace("{READTYPE}", $"Read{typecode}Array");
                    sb1.Append(templateText2);
                }
                else
                {
                    var templateText1 = templateTexts[2];
                    templateText1 = templateText1.Replace("{BITPOS}", $"{bitPos}");
                    templateText1 = templateText1.Replace("{FIELDINDEX}", $"{++bitInx1}");
                    templateText1 = templateText1.Replace("{FIELDNAME}", $"{members[i].Name}");
                    if (members[i].Type.IsValueType)
                        templateText1 = templateText1.Replace("{Condition}", $"value.{members[i].Name} != default");
                    else
                        templateText1 = templateText1.Replace("{Condition}", $"value.{members[i].Name} != null");
                    var local = members[i].ItemType.ToString().Replace(".", "").Replace("+", "") + "ArrayBind";
                    templateText1 = templateText1.Replace("{BINDTYPE}", $"{local}");
                    sb.Append(templateText1);

                    var templateText2 = templateTexts[6];
                    templateText2 = templateText2.Replace("{BITPOS}", $"{bitPos}");
                    templateText2 = templateText2.Replace("{FIELDINDEX}", $"{bitInx1}");
                    templateText2 = templateText2.Replace("{FIELDNAME}", $"{members[i].Name}");
                    templateText2 = templateText2.Replace("{BINDTYPE}", $"{local}");
                    sb1.Append(templateText2);
                }
            }
            else if (members[i].IsGenericType)
            {
                if (members[i].ItemType1 == null)//List<T>
                {
                    var codeSB = BuildGenericAll(members[i].Type);
                    var className = AssemblyHelper.GetCodeTypeName(members[i].Type.ToString());
                    className = className.Replace(".", "").Replace("+", "").Replace("<", "").Replace(">", "");
                    if (members[i].Type != typeof(List<>).MakeGenericType(members[i].ItemType))
                    {
                        File.WriteAllText(savePath + $"//{className}Bind.cs", codeSB.ToString());
                        genericCodes?.Add(codeSB.ToString());
                    }

                    var templateText1 = templateTexts[2];
                    templateText1 = templateText1.Replace("{BITPOS}", $"{bitPos}");
                    templateText1 = templateText1.Replace("{FIELDINDEX}", $"{++bitInx1}");
                    templateText1 = templateText1.Replace("{FIELDNAME}", $"{members[i].Name}");
                    if (members[i].Type.IsValueType)
                        templateText1 = templateText1.Replace("{Condition}", $"value.{members[i].Name} != default");
                    else
                        templateText1 = templateText1.Replace("{Condition}", $"value.{members[i].Name} != null");
                    var local = className + "Bind";
                    templateText1 = templateText1.Replace("{BINDTYPE}", $"{local}");
                    sb.Append(templateText1);

                    var templateText2 = templateTexts[6];
                    templateText2 = templateText2.Replace("{BITPOS}", $"{bitPos}");
                    templateText2 = templateText2.Replace("{FIELDINDEX}", $"{bitInx1}");
                    templateText2 = templateText2.Replace("{FIELDNAME}", $"{members[i].Name}");
                    templateText2 = templateText2.Replace("{BINDTYPE}", $"{local}");
                    sb1.Append(templateText2);
                }
                else //Dic
                {
                    var templateText1 = templateTexts[3];
                    templateText1 = templateText1.Replace("{BITPOS}", $"{bitPos}");
                    templateText1 = templateText1.Replace("{FIELDINDEX}", $"{++bitInx1}");
                    templateText1 = templateText1.Replace("{FIELDNAME}", $"{members[i].Name}");
                    templateText1 = templateText1.Replace("{Condition}", $"value.{members[i].Name} != null");

                    var templateText2 = templateTexts[7];
                    templateText2 = templateText2.Replace("{BITPOS}", $"{bitPos}");
                    templateText2 = templateText2.Replace("{FIELDINDEX}", $"{bitInx1}");
                    templateText2 = templateText2.Replace("{FIELDNAME}", $"{members[i].Name}");

                    var text = BuildDictionary(members[i].Type, out var className1);
                    if (!string.IsNullOrEmpty(savePath))
                        File.WriteAllText(savePath + $"//{className1}.cs", text);
                    genericCodes?.Add(text);

                    templateText1 = templateText1.Replace("{DICTIONARYBIND}", $"{className1}");
                    sb.Append(templateText1);

                    templateText2 = templateText2.Replace("{DICTIONARYBIND}", $"{className1}");
                    sb1.Append(templateText2);

                    types?.Add(members[i].Type);
                }
            }
            else
            {
                var templateText1 = templateTexts[2];
                templateText1 = templateText1.Replace("{BITPOS}", $"{bitPos}");
                templateText1 = templateText1.Replace("{FIELDINDEX}", $"{++bitInx1}");
                templateText1 = templateText1.Replace("{FIELDNAME}", $"{members[i].Name}");

                if(members[i].Type.IsValueType)
                    templateText1 = templateText1.Replace("{Condition}", $"value.{members[i].Name} != default({members[i].Type})");
                else
                    templateText1 = templateText1.Replace("{Condition}", $"value.{members[i].Name} != null");
                var local = members[i].Type.ToString().Replace(".", "").Replace("+", "") + "Bind";
                templateText1 = templateText1.Replace("{BINDTYPE}", $"{local}");
                sb.Append(templateText1);

                var templateText2 = templateTexts[6];
                templateText2 = templateText2.Replace("{BITPOS}", $"{bitPos}");
                templateText2 = templateText2.Replace("{FIELDINDEX}", $"{bitInx1}");
                templateText2 = templateText2.Replace("{FIELDNAME}", $"{members[i].Name}");
                templateText2 = templateText2.Replace("{BINDTYPE}", $"{local}");
                sb1.Append(templateText2);
            }
        }

        sb.Append(templateTexts[4]);
        sb.Append(sb1);
        sb.Append(templateTexts[8]);

        return sb;
    }

    public static StringBuilder BuildNewFast(Type type, bool serField, bool serProperty, List<string> ignores, string savePath = null, HashSet<Type> types = null, List<string> genericCodes = null)
    {
        var sb = new StringBuilder();
        var sb1 = new StringBuilder();
        FieldInfo[] fields;
        if (serField)
            fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
        else
            fields = new FieldInfo[0];
        PropertyInfo[] properties;
        if (serProperty)
            properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
        else
            properties = new PropertyInfo[0];
        var members = new List<Member>();
        foreach (var field in fields)
        {
            if (field.GetCustomAttribute<Net.Serialize.NonSerialized>() != null)
                continue;
            if (ignores.Contains(field.Name))
                continue;
            var member = new Member()
            {
                IsArray = field.FieldType.IsArray,
                IsEnum = field.FieldType.IsEnum,
                IsGenericType = field.FieldType.IsGenericType,
                IsPrimitive = Type.GetTypeCode(field.FieldType) != TypeCode.Object,
                Name = field.Name,
                Type = field.FieldType,
                TypeCode = Type.GetTypeCode(field.FieldType)
            };
            if (field.FieldType.IsArray)
            {
                var itemType = field.FieldType.GetArrayItemType();
                member.ItemType = itemType;
            }
            else if (field.FieldType.GenericTypeArguments.Length == 1)
            {
                Type itemType = field.FieldType.GenericTypeArguments[0];
                member.ItemType = itemType;
            }
            else if (field.FieldType.GenericTypeArguments.Length == 2)
            {
                Type itemType = field.FieldType.GenericTypeArguments[0];
                Type itemType1 = field.FieldType.GenericTypeArguments[1];
                member.ItemType = itemType;
                member.ItemType1 = itemType1;
            }
            members.Add(member);
        }
        foreach (var property in properties)
        {
            if (property.GetCustomAttribute<Net.Serialize.NonSerialized>() != null)
                continue;
            if (!property.CanRead | !property.CanWrite)
                continue;
            if (property.GetIndexParameters().Length > 0)
                continue;
            if (ignores.Contains(property.Name))
                continue;
            var member = new Member()
            {
                IsArray = property.PropertyType.IsArray,
                IsEnum = property.PropertyType.IsEnum,
                IsGenericType = property.PropertyType.IsGenericType,
                IsPrimitive = Type.GetTypeCode(property.PropertyType) != TypeCode.Object,
                Name = property.Name,
                Type = property.PropertyType,
                TypeCode = Type.GetTypeCode(property.PropertyType)
            };
            if (property.PropertyType.IsArray)
            {
                var itemType = property.PropertyType.GetArrayItemType();
                member.ItemType = itemType;
            }
            else if (property.PropertyType.GenericTypeArguments.Length == 1)
            {
                Type itemType = property.PropertyType.GenericTypeArguments[0];
                member.ItemType = itemType;
            }
            else if (property.PropertyType.GenericTypeArguments.Length == 2)
            {
                Type itemType = property.PropertyType.GenericTypeArguments[0];
                Type itemType1 = property.PropertyType.GenericTypeArguments[1];
                member.ItemType = itemType;
                member.ItemType1 = itemType1;
            }
            members.Add(member);
        }

        var templateText = @"using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Net.System;
using Net.Serialize;

namespace Binding
{
    public readonly struct {TYPENAME}Bind : ISerialize<{TYPE}>, ISerialize
    {
        public unsafe void Write({TYPE} value, ISegment stream)
        {
            fixed (byte* ptr = &stream.Buffer[stream.Position]) 
            {
                int offset = 0;
{Split} //1
                {WRITE}
{Split} //2
                NetConvertHelper.Write(ptr, ref offset, value.{FIELDNAME} != null);
                if (value.test != null)
                {
                    stream.Position++;
                    new {BINDTYPE}().Write(value.{FIELDNAME}, stream);
                }
{Split} //3
            }
        }
		
        public {TYPE} Read(ISegment stream) 
        {
            var value = new {TYPE}();
            Read(ref value, stream);
            return value;
        }

		public unsafe void Read(ref {TYPE} value, ISegment stream)
		{
			fixed (byte* ptr = &stream.Buffer[stream.Position]) 
            {
                int offset = 0;
{Split} //4
                {READ}
{Split} //5
                if (NetConvertHelper.Read<bool>(ptr, ref offset))
                {
                    stream.Position++;
                    value.{FIELDNAME} = new {BINDTYPE}().Read(stream);
                }
{Split} //6
            }
		}

        public void WriteValue(object value, ISegment stream)
        {
            Write(({TYPE})value, stream);
        }

        public object ReadValue(ISegment stream)
        {
            return Read(stream);
        }
    }
}
";
        var typeName = type.ToString().Replace(".", "").Replace("+", "");
        var fullName = type.ToString();
        templateText = templateText.Replace("{TYPENAME}", typeName);
        templateText = templateText.Replace("{TYPE}", fullName);
        templateText = templateText.Replace("{SIZE}", $"{((members.Count - 1) / 8) + 1}");

        var templateTexts = templateText.Split(new string[] { "{Split}" }, 0);

        sb.Append(templateTexts[0]);

        for (int i = 0; i < members.Count; i++)
        {
            int bitInx1 = i % 8;
            int bitPos = i / 8;
            var typecode = Type.GetTypeCode(members[i].Type);
            if (typecode != TypeCode.Object)
            {
                var templateText1 = templateTexts[1];
                templateText1 = templateText1.Replace("{WRITE}", $"NetConvertHelper.Write(ptr, ref offset, value.{members[i].Name});");
                sb.Append(templateText1);

                var templateText2 = templateTexts[4];
                templateText2 = templateText2.Replace("{READ}", $"value.{members[i].Name} = NetConvertHelper.Read{(typecode == TypeCode.String ? "" : $"<{members[i].Type}>")}(ptr, ref offset);");
                sb1.Append(templateText2);
            }
            else if (members[i].IsArray)
            {
                typecode = Type.GetTypeCode(members[i].ItemType);
                if (typecode != TypeCode.Object)
                {
                    var templateText1 = templateTexts[1];
                    templateText1 = templateText1.Replace("{WRITE}", $"NetConvertHelper.WriteArray(ptr, ref offset, value.{members[i].Name});");
                    sb.Append(templateText1);

                    var templateText2 = templateTexts[4];
                    templateText2 = templateText2.Replace("{READ}", $"value.{members[i].Name} = NetConvertHelper.ReadArray{(typecode == TypeCode.String ? "" : $"<{members[i].ItemType}>")}(ptr, ref offset);");
                    sb1.Append(templateText2);
                }
                else
                {
                    var local = members[i].ItemType.ToString().Replace(".", "").Replace("+", "") + "ArrayBind";

                    var templateText1 = templateTexts[2];
                    templateText1 = templateText1.Replace("{FIELDNAME}", $"{members[i].Name}");
                    templateText1 = templateText1.Replace("{BINDTYPE}", $"{local}");
                    sb.Append(templateText1);

                    var templateText2 = templateTexts[5];
                    templateText2 = templateText2.Replace("{FIELDNAME}", $"{members[i].Name}");
                    templateText2 = templateText2.Replace("{BINDTYPE}", $"{local}");
                    sb1.Append(templateText2);
                }
            }
            else if (members[i].IsGenericType)
            {
                if (members[i].ItemType1 == null)//List<T>
                {
                    var codeSB = BuildGenericAll(members[i].Type);
                    var className = AssemblyHelper.GetCodeTypeName(members[i].Type.ToString());
                    className = className.Replace(".", "").Replace("+", "").Replace("<", "").Replace(">", "");
                    if (members[i].Type != typeof(List<>).MakeGenericType(members[i].ItemType))
                    {
                        File.WriteAllText(savePath + $"//{className}Bind.cs", codeSB.ToString());
                        genericCodes?.Add(codeSB.ToString());
                    }

                    typecode = Type.GetTypeCode(members[i].ItemType);
                    if (typecode != TypeCode.Object)
                    {
                        var templateText1 = templateTexts[1];
                        var typeName1 = AssemblyHelper.GetCodeTypeName(members[i].Type.ToString());
                        templateText1 = templateText1.Replace("{WRITE}", $"NetConvertHelper.WriteCollection(ptr, ref offset, value.{members[i].Name});");
                        sb.Append(templateText1);

                        var templateText2 = templateTexts[4];
                        templateText2 = templateText2.Replace("{READ}", $"value.{members[i].Name} = NetConvertHelper.ReadCollection<{(typecode == TypeCode.String ? $"{typeName1}" : $"{typeName1}, {members[i].ItemType}")}>(ptr, ref offset);");
                        sb1.Append(templateText2);
                    }
                    else
                    {
                        var local = AssemblyHelper.GetCodeTypeName(members[i].Type.ToString());
                        local = local.Replace(".", "").Replace("+", "").Replace("<", "").Replace(">", "") + "Bind";

                        var templateText1 = templateTexts[2];
                        templateText1 = templateText1.Replace("{FIELDNAME}", $"{members[i].Name}");
                        templateText1 = templateText1.Replace("{BINDTYPE}", $"{local}");
                        sb.Append(templateText1);

                        var templateText2 = templateTexts[5];
                        templateText2 = templateText2.Replace("{FIELDNAME}", $"{members[i].Name}");
                        templateText2 = templateText2.Replace("{BINDTYPE}", $"{local}");
                        sb1.Append(templateText2);
                    }
                }
                else //Dic
                {
                    var templateText1 = templateTexts[3];
                    templateText1 = templateText1.Replace("{BITPOS}", $"{bitPos}");
                    templateText1 = templateText1.Replace("{FIELDINDEX}", $"{++bitInx1}");
                    templateText1 = templateText1.Replace("{FIELDNAME}", $"{members[i].Name}");
                    templateText1 = templateText1.Replace("{Condition}", $"value.{members[i].Name} != null");

                    var templateText2 = templateTexts[7];
                    templateText2 = templateText2.Replace("{BITPOS}", $"{bitPos}");
                    templateText2 = templateText2.Replace("{FIELDINDEX}", $"{bitInx1}");
                    templateText2 = templateText2.Replace("{FIELDNAME}", $"{members[i].Name}");

                    var text = BuildDictionary(members[i].Type, out var className1);
                    if (!string.IsNullOrEmpty(savePath))
                        File.WriteAllText(savePath + $"//{className1}.cs", text);
                    genericCodes?.Add(text);

                    templateText1 = templateText1.Replace("{DICTIONARYBIND}", $"{className1}");
                    sb.Append(templateText1);

                    templateText2 = templateText2.Replace("{DICTIONARYBIND}", $"{className1}");
                    sb1.Append(templateText2);

                    types?.Add(members[i].Type);
                }
            }
            else
            {
                var local = members[i].Type.ToString().Replace(".", "").Replace("+", "") + "Bind";

                var templateText1 = templateTexts[2];
                templateText1 = templateText1.Replace("{FIELDNAME}", $"{members[i].Name}");
                templateText1 = templateText1.Replace("{BINDTYPE}", $"{local}");
                sb.Append(templateText1);

                var templateText2 = templateTexts[5];
                templateText2 = templateText2.Replace("{FIELDNAME}", $"{members[i].Name}");
                templateText2 = templateText2.Replace("{BINDTYPE}", $"{local}"); 
                sb1.Append(templateText2);
            }
        }

        sb.Append(templateTexts[3]);
        sb.Append(sb1);
        sb.Append(templateTexts[6]);

        return sb;
    }


    public static void BuildBindingType(HashSet<Type> types, string savePath, int sortingOrder = 1)
    {
        var code = BuildBindingType(types, sortingOrder);
        File.WriteAllText(savePath + $"//BindingType.cs", code);
    }

    public static string BuildBindingType(HashSet<Type> types, int sortingOrder = 1)
    {
        var str = new StringBuilder();
        str.AppendLine("using System;");
        str.AppendLine("using System.Collections.Generic;");
        str.AppendLine("using Net.Serialize;");
        str.AppendLine("");
        str.AppendLine("namespace Binding");
        str.AppendLine("{");
        str.AppendLine("    public class BindingType : IBindingType");
        str.AppendLine("    {");
        str.AppendLine("        public int SortingOrder { get; private set; }");
        str.AppendLine("        public Dictionary<Type, Type> BindTypes { get; private set; }");
        str.AppendLine("        public BindingType()");
        str.AppendLine("        {");
        str.AppendLine($"            SortingOrder = {sortingOrder};");
        str.AppendLine("            BindTypes = new Dictionary<Type, Type>");
        str.AppendLine("            {");
        foreach (var item in types)
        {
            if (item.IsGenericType & item.GenericTypeArguments.Length == 2)
            {
                var typeName = AssemblyHelper.GetTypeName(item);
                var bindType = GetDictionaryBindTypeName(item);
                str.AppendLine($"\t\t\t\t{{ typeof({typeName}), typeof({bindType}) }},");
            }
            else
            {
                str.AppendLine($"\t\t\t\t{{ typeof({item.ToString()}), typeof({item.ToString().Replace(".", "")}Bind) }},");
                str.AppendLine($"\t\t\t\t{{ typeof({item.ToString()}[]), typeof({item.ToString().Replace(".", "")}ArrayBind) }},");
                var typeName = AssemblyHelper.GetCodeTypeName(typeof(List<>).MakeGenericType(item).ToString());
                typeName = typeName.Replace(".", "").Replace("+", "").Replace("<", "").Replace(">", "") + "Bind";
                str.AppendLine($"\t\t\t\t{{ typeof(List<{item.ToString()}>), typeof({typeName}) }},");
            }
        }
        str.AppendLine("            };");
        str.AppendLine("        }");
        str.AppendLine("    }");
        str.AppendLine("}");
        return str.ToString();
    }

    public static void BuildBindingExtension(HashSet<Type> types, string savePath)
    {
        var code = BuildBindingExtension(types);
        File.WriteAllText(savePath + $"//BindingExtension.cs", code);
    }

    public static string BuildBindingExtension(HashSet<Type> types)
    {
        var codeTemplate = @"using Binding;
using Net.System;

public static class BindingExtension
{
{Space}
    public static ISegment SerializeObject(this {TYPE} value)
    {
        var segment = BufferPool.Take();
        var bind = new {TYPEBIND}();
        bind.Write(value, segment);
        return segment;
    }

    public static {TYPE} DeserializeObject(this {TYPE} value, ISegment segment, bool isPush = true)
    {
        var bind = new {TYPEBIND}();
        bind.Read(ref value, segment);
        if (isPush) BufferPool.Push(segment);
        return value;
    }
{Space}
}";

        var str = new StringBuilder();
        var codes = codeTemplate.Split(new string[] { "{Space}" }, 0);
        str.Append(codes[0]);
        foreach (var item in types)
        {
            if (item.IsGenericType)
                continue;
            var code = codes[1];
            code = code.Replace("{TYPE}", item.ToString());
            code = code.Replace("{TYPEBIND}", $"{item.ToString().Replace(".", "")}Bind");
            str.Append(code);
        }
        str.Append(codes[2]);
        return str.ToString();
    }

    public static void BuildArray(Type type, string savePath)
    {
        var str = BuildArray(type);
        var className = type.FullName.Replace(".", "").Replace("+", "");
        File.AppendAllText(savePath + $"//{className}Bind.cs", str.ToString());
    }

    public static StringBuilder BuildArray(Type type)
    {
        var sb = new StringBuilder();
        var templateText = @"
namespace Binding
{
	public readonly struct {TYPENAME}ArrayBind : ISerialize<{TYPE}[]>, ISerialize
	{
		public void Write({TYPE}[] value, ISegment stream)
		{
			int count = value.Length;
			stream.Write(count);
			if (count == 0) return;
			var bind = new {BINDTYPE}();
			foreach (var value1 in value)
				bind.Write(value1, stream);
		}

		public {TYPE}[] Read(ISegment stream)
		{
			var count = stream.ReadInt32();
			var value = new {TYPE}[count];
			if (count == 0) return value;
			var bind = new {BINDTYPE}();
			for (int i = 0; i < count; i++)
				value[i] = bind.Read(stream);
			return value;
		}

		public void WriteValue(object value, ISegment stream)
		{
			Write(({TYPE}[])value, stream);
		}

		public object ReadValue(ISegment stream)
		{
			return Read(stream);
		}
	}
}";
        var typeName = type.FullName.Replace(".", "").Replace("+", "");
        var fullName = type.FullName;
        templateText = templateText.Replace("{TYPENAME}", typeName);
        templateText = templateText.Replace("{TYPE}", fullName);
        
        var local = typeName + "Bind";
        templateText = templateText.Replace("{BINDTYPE}", $"{local}");

        sb.Append(templateText);
        return sb;
    }

    public static void BuildGeneric(Type type, string savePath)
    {
        var str = BuildGeneric(type);
        var className = type.ToString().Replace(".", "").Replace("+", "");
        File.AppendAllText(savePath + $"//{className}Bind.cs", str.ToString());
    }

    public static StringBuilder BuildGeneric(Type type)
    {
        var sb = new StringBuilder();
        var templateText = @"
namespace Binding
{
	public readonly struct {TYPENAME}Bind : ISerialize<List<TYPE>>, ISerialize
	{
		public void Write(List<TYPE> value, ISegment stream)
		{
			int count = value.Count;
			stream.Write(count);
			if (count == 0) return;
			var bind = new {BINDTYPE}();
			foreach (var value1 in value)
				bind.Write(value1, stream);
		}

		public List<TYPE> Read(ISegment stream)
		{
			var count = stream.ReadInt32();
			var value = new List<TYPE>({COUNT});
			if (count == 0) return value;
			var bind = new {BINDTYPE}();
			for (int i = 0; i < count; i++)
				value.Add(bind.Read(stream));
			return value;
		}

		public void WriteValue(object value, ISegment stream)
		{
			Write((List<TYPE>)value, stream);
		}

		public object ReadValue(ISegment stream)
		{
			return Read(stream);
		}
	}
}";
        var typeName = AssemblyHelper.GetCodeTypeName(type.ToString());
        var fullName = typeName;
        typeName = typeName.Replace(".", "").Replace("+", "").Replace("<", "").Replace(">", "");
        templateText = templateText.Replace("{TYPENAME}", typeName);
        templateText = templateText.Replace("List<TYPE>", fullName);

        var itemTypeName = type.GetArrayItemType().ToString();
        itemTypeName = itemTypeName.Replace(".", "").Replace("+", "").Replace("<", "").Replace(">", "");
        var local = itemTypeName + "Bind";
        templateText = templateText.Replace("{BINDTYPE}", $"{local}");

        var ctor = type.GetConstructor(new Type[] { typeof(int) });
        templateText = templateText.Replace("{COUNT}", ctor != null ? "count" : "");

        sb.Append(templateText);
        return sb;
    }

    public static StringBuilder BuildGenericAll(Type type)
    {
        var sb = new StringBuilder();
        var templateText = @"using Net.System;
using Net.Serialize;

namespace Binding
{
	public readonly struct {TYPENAME}Bind : ISerialize<List<TYPE>>, ISerialize
	{
		public void Write(List<TYPE> value, ISegment stream)
		{
			int count = value.Count;
			stream.Write(count);
			if (count == 0) return;
			var bind = new {BINDTYPE}();
			foreach (var value1 in value)
				bind.Write(value1, stream);
		}

		public List<TYPE> Read(ISegment stream)
		{
			var count = stream.ReadInt32();
			var value = new List<TYPE>({COUNT});
			if (count == 0) return value;
			var bind = new {BINDTYPE}();
			for (int i = 0; i < count; i++)
				value.Add(bind.Read(stream));
			return value;
		}

		public void WriteValue(object value, ISegment stream)
		{
			Write((List<TYPE>)value, stream);
		}

		public object ReadValue(ISegment stream)
		{
			return Read(stream);
		}
	}
}";
        var typeName = AssemblyHelper.GetCodeTypeName(type.ToString());
        var fullName = typeName;
        typeName = typeName.Replace(".", "").Replace("+", "").Replace("<", "").Replace(">", "");
        templateText = templateText.Replace("{TYPENAME}", typeName);
        templateText = templateText.Replace("List<TYPE>", fullName);

        var itemTypeName = type.GetArrayItemType().ToString();
        itemTypeName = itemTypeName.Replace(".", "").Replace("+", "").Replace("<", "").Replace(">", "");
        var local = itemTypeName + "Bind";
        templateText = templateText.Replace("{BINDTYPE}", $"{local}");

        var ctor = type.GetConstructor(new Type[] { typeof(int) });
        templateText = templateText.Replace("{COUNT}", ctor != null ? "count" : "");

        sb.Append(templateText);
        return sb;
    }

    public static string BuildDictionary(Type type, out string fileTypeName)
    {
        var text =
@"using Binding;
using Net.Serialize;
using Net.System;
using System.Collections.Generic;

public readonly struct {Dictionary}_{TKeyName}_{TValueName}_Bind : ISerialize<{Dictionary}<{TKey}, {TValue}>>, ISerialize
{
    public void Write({Dictionary}<{TKey}, {TValue}> value, ISegment stream)
    {
        int count = value.Count;
        stream.Write(count);
        if (count == 0) return;
        foreach (var value1 in value)
        {
            stream.Write(value1.Key);
            var bind = new {BindTypeName}();
            bind.Write(value1.Value, stream);
        }
    }

    public {Dictionary}<{TKey}, {TValue}> Read(ISegment stream)
    {
        var count = stream.ReadInt32();
        var value = new {Dictionary}<{TKey}, {TValue}>();
        if (count == 0) return value;
        for (int i = 0; i < count; i++)
        {
            var key = stream.Read{READ}();
            var bind = new {BindTypeName}();
            var value1 = bind.Read(stream);
            value.Add(key, value1);
        }
        return value;
    }

    public void WriteValue(object value, ISegment stream)
    {
        Write(({Dictionary}<{TKey}, {TValue}>)value, stream);
    }

    public object ReadValue(ISegment stream)
    {
        return Read(stream);
    }
}";
        var args = type.GenericTypeArguments;
        string value;
        string typeBindName;
        string keyRead = $"{Type.GetTypeCode(args[0])}";
        if (args[1].IsArray)
        {
            value = args[1].ToString().Replace("+", ".");
            typeBindName = args[1].ToString().ReplaceClear("+", ".", "[", "]", "<", ">") + "ArrayBind";
        }
        else if (args[1].IsGenericType)
        {
            value = AssemblyHelper.GetCodeTypeName(args[1].ToString());
            typeBindName = value.ReplaceClear("+", ".", "[", "]", "<", ">") + "Bind";
        }
        else
        {
            value = args[1].ToString().Replace("+", ".");
            typeBindName = args[1].ToString().Replace(".", "").Replace("+", "") + "Bind";
        }
        text = text.Replace("{TKeyName}", $"{args[0].ToString().Replace(".", "").Replace("+", "")}");
        text = text.Replace("{TValueName}", $"{typeBindName}");
        text = text.Replace("{TKey}", $"{args[0]}");
        text = text.Replace("{TValue}", $"{value}");
        text = text.Replace("{BindTypeName}", $"{typeBindName}");
        text = text.Replace("{READ}", $"{keyRead}");
        var dictName = type.Name.Replace("`2", "");
        text = text.Replace("{Dictionary}", $"{dictName}");

        fileTypeName = $"{dictName}_{args[0].ToString().Replace(".", "")}_{typeBindName}_Bind"; ;
        return text;
    }

    public static string GetDictionaryBindTypeName(Type type)
    {
        var args = type.GenericTypeArguments;
        string typeBindName;
        if (args[1].IsArray)
        {
            typeBindName = args[1].ToString().ReplaceClear("+", ".", "[", "]", "<", ">") + "ArrayBind";
        }
        else if (args[1].IsGenericType)
        {
            var value = AssemblyHelper.GetCodeTypeName(args[1].ToString());
            typeBindName = value.ReplaceClear("+", ".", "[", "]", "<", ">") + "Bind";
        }
        else
        {
            typeBindName = args[1].ToString().Replace(".", "").Replace("+", "") + "Bind";
        }
        var dictName = type.Name.Replace("`2", "");
        var fileTypeName = $"{dictName}_{args[0].ToString().Replace(".", "")}_{typeBindName}_Bind"; ;
        return fileTypeName;
    }
}
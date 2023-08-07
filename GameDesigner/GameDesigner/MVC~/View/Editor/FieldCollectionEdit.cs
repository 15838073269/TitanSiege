#if (UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_WSA || UNITY_WEBGL) && UNITY_EDITOR
namespace MVC.View
{
    using Net.Helper;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using UnityEditor;
    using UnityEditor.Callbacks;
    using UnityEngine;
    using UnityEngine.UI;
    using Object = UnityEngine.Object;

    public class FieldCollectionWindow : EditorWindow
    {
        internal static void Init(FieldCollection field)
        {
            GetWindow<FieldCollectionWindow>("字段收集器", true);
            FieldCollectionEntity.OnEnable(field);
        }

        private void OnEnable()
        {
            FieldCollectionEntity.LoadData();
        }

        private void OnDisable()
        {
            FieldCollectionEntity.OnDisable();
        }

        void OnGUI()
        {
            FieldCollectionEntity.OnDragGuiWindow();
            FieldCollectionEntity.OnDragGUI();
        }
    }

    [CustomEditor(typeof(FieldCollection))]
    public class FieldCollectionEdit : Editor
    {
        private void OnEnable()
        {
            FieldCollectionEntity.OnEnable(target as FieldCollection);
        }

        private void OnDisable()
        {
            FieldCollectionEntity.OnDisable();
        }

        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("打开收集器界面"))
                FieldCollectionWindow.Init(target as FieldCollection);
            FieldCollectionEntity.OnDragGUI();
        }
    }

    public class FieldCollectionEntity
    {
        private static FieldCollection field;
        internal static string search = "", search1 = "", fieldName = "";
        internal static Object selectObject;
        internal static JsonSave data = new JsonSave();
        private static Vector2 scrollPosition;
        private static string searchAssemblies;

        public class JsonSave
        {
            public string nameSpace;
            public List<string> savePath = new List<string>();
            public List<string> savePathExt = new List<string>();
            public string csprojFile;
            public string searchAssemblies = "UnityEngine.CoreModule|Assembly-CSharp|Assembly-CSharp-firstpass";
            internal string nameSpace1;
            public bool changeField;
            public bool addField;
            public bool seleAddField;
            internal string addInheritType;
            public List<string> inheritTypes = new List<string>() { "Net.Component.SingleCase", "UnityEngine.MonoBehaviour" };
            internal string SavePath(int savePathIndex) => savePath.Count > 0 ? savePath[savePathIndex] : string.Empty;
            internal string SavePathExt(int savePathExtIndex) => savePathExt.Count > 0 ? savePathExt[savePathExtIndex] : string.Empty;
            internal string InheritType(int index) => inheritTypes[index];
        }

        internal static void OnEnable(FieldCollection target)
        {
            LoadData();
            field = target;
            searchAssemblies = data.searchAssemblies;
            if (!string.IsNullOrEmpty(searchAssemblies))
            {
                var assemblyNames = searchAssemblies.Split('|');
                var types1 = new List<string>();
                var assemblies = AppDomain.CurrentDomain.GetAssemblies();
                foreach (var assemblie in assemblies)
                {
                    var name = assemblie.GetName().Name;
                    if (assemblyNames.Contains(name))
                    {
                        var types = assemblie.GetTypes().Where(t => !t.IsAbstract & !t.IsInterface & !t.IsGenericType & !t.IsGenericTypeDefinition);
                        foreach (var type in types)
                            types1.Add(type.ToString());
                    }
                }
            }
        }

        internal static void OnDisable()
        {
            SaveData();
            if (field != null)
                EditorUtility.SetDirty(field);
        }

        internal static void LoadData()
        {
            data = PersistHelper.Deserialize<JsonSave>("fcdata.json");
        }

        static void SaveData()
        {
            PersistHelper.Serialize(data, "fcdata.json");
        }

        internal static void AddField(string typeName)
        {
            var name = fieldName;
            if (name == "")
                name = "name" + field.nameIndex++;
            foreach (var f in field.fields)
            {
                if (f.name == fieldName)
                {
                    name += field.nameIndex++;
                    break;
                }
            }
            var field1 = new FieldCollection.Field() { name = name, typeName = typeName };
            field.fields.Add(field1);
            if (selectObject != null)
                field1.target = selectObject;
            field1.Update();
            EditorUtility.SetDirty(field);
        }

        public static void OnDragGuiWindow()
        {
            GUILayout.Label("将组件拖到此窗口上! 如果是赋值模式, 拖入的对象将不会显示选择组件!");
            data.changeField = GUILayout.Toggle(data.changeField, "赋值变量");
            data.addField = GUILayout.Toggle(data.addField, "直接添加变量");
            data.seleAddField = GUILayout.Toggle(data.seleAddField, "选择添加变量组件");
            if ((Event.current.type == EventType.DragUpdated | Event.current.type == EventType.DragPerform) & !data.changeField)
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;//拖动时显示辅助图标
                if (Event.current.type == EventType.DragPerform)
                {
                    if (data.addField)
                    {
                        var componentPriority = new List<Type>()
                        {
                            typeof(Button), typeof(Toggle), typeof(Text), typeof(Slider), typeof(Scrollbar), typeof(Dropdown),
                            typeof(ScrollRect), typeof(InputField), typeof(Image)
                        };
                        foreach (var obj in DragAndDrop.objectReferences)
                        {
                            fieldName = obj.name.Replace(" ", "").Replace("(", "_").Replace(")", "_");
                            if (obj is GameObject go)
                            {
                                var objects = new List<Object>() { obj };
                                objects.AddRange(go.GetComponents<Component>());
                                foreach (var cp in componentPriority)
                                {
                                    var components = objects.Where(item => item.GetType() == cp).ToList();
                                    if (components.Count > 0)
                                    {
                                        selectObject = components[0];
                                        AddField(components[0].GetType().ToString());
                                        goto J;
                                    }
                                }
                                selectObject = objects[objects.Count - 1];
                                AddField(objects[objects.Count - 1].GetType().ToString());
                            }
                            else if (obj is Component component) 
                            {
                                selectObject = component;
                                AddField(component.GetType().ToString());
                            }
                        J:;
                        }
                        return;
                    }
                    else if (data.seleAddField)
                    {
                        var dict = new Dictionary<Type, List<Object[]>>();
                        foreach (var obj in DragAndDrop.objectReferences)
                        {
                            GameObject gameObject;
                            if (obj is Component component)
                                gameObject = component.gameObject;
                            else
                                gameObject = obj as GameObject;
                            var objects = new List<Object>() { gameObject };
                            objects.AddRange(gameObject.GetComponents<Component>());
                            foreach (var obj1 in objects)
                            {
                                var type = obj1.GetType();
                                if (!dict.TryGetValue(type, out var objects1))
                                    dict.Add(type, objects1 = new List<Object[]>());
                                objects1.Add(new Object[] { obj, obj1 });
                            }
                        }
                        var menu = new GenericMenu();
                        foreach (var item in dict)
                        {
                            var typeName = item.Key.ToString();
                            menu.AddItem(new GUIContent(typeName), false, () =>
                            {
                                foreach (var item1 in item.Value)
                                {
                                    fieldName = item1[0].name.Replace(" ", "").Replace("(", "_").Replace(")", "_");
                                    selectObject = item1[1];
                                    AddField(typeName);
                                }
                            });
                        }
                        menu.ShowAsContext();
                        Event.current.Use();
                        return;
                    }
                    else
                    {
                        search1 = "";
                        search = DragAndDrop.objectReferences[0].GetType().Name.ToLower();
                    }
                }
            }
        }

        public static void OnDragGUI()
        {
            if (field == null)
                return;
            field.fieldName = EditorGUILayout.TextField("收集器名称", field.fieldName);
            data.searchAssemblies = EditorGUILayout.TextField("搜索的程序集", data.searchAssemblies);
            if (data.searchAssemblies != searchAssemblies)
            {
                searchAssemblies = data.searchAssemblies;
                SaveData();
            }
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, true);
            for (int i = 0; i < field.fields.Count; i++)
            {
                var field1 = field.fields[i];
                EditorGUILayout.BeginHorizontal();
                EditorGUI.BeginChangeCheck();
                field1.name = EditorGUILayout.TextField(field1.name, GUI.skin.label, GUILayout.MaxWidth(100));
                if (field1.typeNames == null)
                    field1.Update();
                field1.componentIndex = EditorGUILayout.Popup(field1.componentIndex, field1.typeNames, GUILayout.MaxWidth(200));
                field1.typeName = field1.typeNames[field1.componentIndex];
                field1.target = EditorGUILayout.ObjectField(field1.target, field1.Type, true);
                if (GUILayout.Button("x", GUILayout.Width(25)))
                {
                    field.fields.RemoveAt(i);
                    EditorUtility.SetDirty(field);
                }
                if (EditorGUI.EndChangeCheck())
                {
                    field1.Update();
                    EditorUtility.SetDirty(field);
                }
                EditorGUILayout.EndHorizontal();
            }
            GUILayout.EndScrollView();
            data.nameSpace = EditorGUILayout.TextField("命名空间", data.nameSpace);
            if (data.nameSpace != data.nameSpace1)
            {
                data.nameSpace1 = data.nameSpace;
                SaveData();
            }
            var rect1 = EditorGUILayout.GetControlRect();
            field.savePathInx = EditorGUI.Popup(new Rect(rect1.x, rect1.y, rect1.width - 90, rect1.height), "组件生成路径:", field.savePathInx, data.savePath.ToArray());
            if (GUI.Button(new Rect(rect1.x + rect1.width - 90, rect1.y, 30, rect1.height), "x"))
            {
                if (data.savePath.Count > 0)
                    data.savePath.RemoveAt(field.savePathInx);
                SaveData();
            }
            if (GUI.Button(new Rect(rect1.x + rect1.width - 60, rect1.y, 60, rect1.height), "选择"))
            {
                var path = EditorUtility.OpenFolderPanel("选择保存路径", "", "");
                //相对于Assets路径
                var uri = new Uri(Application.dataPath.Replace('/', '\\'));
                var relativeUri = uri.MakeRelativeUri(new Uri(path));
                path = relativeUri.ToString();
                path = path.Replace('/', '\\');
                if (!data.savePath.Contains(path))
                {
                    data.savePath.Add(path);
                    field.savePathInx = data.savePath.Count - 1;
                }
                SaveData();
            }
            var rect4 = EditorGUILayout.GetControlRect();
            field.savePathExtInx = EditorGUI.Popup(new Rect(rect4.x, rect4.y, rect4.width - 90, rect4.height), "组件扩展路径:", field.savePathExtInx, data.savePathExt.ToArray());
            if (GUI.Button(new Rect(rect4.x + rect4.width - 90, rect4.y, 30, rect4.height), "x"))
            {
                if (data.savePathExt.Count > 0)
                    data.savePathExt.RemoveAt(field.savePathExtInx);
                SaveData();
            }
            if (GUI.Button(new Rect(rect4.x + rect4.width - 60, rect4.y, 60, rect4.height), "选择"))
            {
                var path = EditorUtility.OpenFolderPanel("选择保存路径", "", "");
                //相对于Assets路径
                var uri = new Uri(Application.dataPath.Replace('/', '\\'));
                var relativeUri = uri.MakeRelativeUri(new Uri(path));
                path = relativeUri.ToString();
                path = path.Replace('/', '\\');
                if (!data.savePathExt.Contains(path))
                {
                    data.savePathExt.Add(path);
                    field.savePathExtInx = data.savePathExt.Count - 1;
                }
                SaveData();
            }
            var rect3 = EditorGUILayout.GetControlRect();
            EditorGUI.LabelField(rect3, "csproj文件:", data.csprojFile);
            if (GUI.Button(new Rect(rect3.x + rect3.width - 60, rect3.y, 60, rect3.height), "选择"))
            {
                var path = EditorUtility.OpenFilePanel("选择文件", "", "csproj");
                //相对于Assets路径
                var uri = new Uri(Application.dataPath.Replace('/', '\\'));
                var relativeUri = uri.MakeRelativeUri(new Uri(path));
                data.csprojFile = relativeUri.ToString();
                SaveData();
            }
            EditorGUILayout.BeginHorizontal();
            data.addInheritType = EditorGUILayout.TextField("自定义继承类型", data.addInheritType);
            if (GUILayout.Button("添加", GUILayout.Width(50f)))
            {
                if (!data.inheritTypes.Contains(data.addInheritType))
                    data.inheritTypes.Add(data.addInheritType);
            }
            if (GUILayout.Button("删除", GUILayout.Width(50f)))
            {
                data.inheritTypes.Remove(data.addInheritType);
            }
            EditorGUILayout.EndHorizontal();
            field.genericType = EditorGUILayout.Toggle("继承泛型", field.genericType);
            field.inheritTypeInx = EditorGUILayout.Popup("继承类型", field.inheritTypeInx, data.inheritTypes.ToArray());
            if (GUILayout.Button("生成脚本(hotfix)"))
            {
                var codeTemplate = @"namespace {nameSpace} 
{
--
    public partial class {typeName} : {inherit}
    {
        public UnityEngine.GameObject panel;
--
        public {fieldType} {fieldName};
--
        public void Init(MVC.View.FieldCollection fc)
        {
            panel = fc.gameObject;
--
            {fieldName} = fc[""{fieldName}""].target as {fieldType};
--
        }
    }
--
}";
                var codeTemplate1 = @"namespace {nameSpace} 
{
--
    public partial class {typeName}
    {
--
        public void InitListener()
        {
--
            {AddListener}
--
        }
--
        private void {methodEvent}
        {
        }
--
    }
--
}";
                string scriptCode;
                {
                    var hasns = !string.IsNullOrEmpty(data.nameSpace);
                    if (string.IsNullOrEmpty(field.fieldName))
                        field.fieldName = field.name;
                    codeTemplate = codeTemplate.Replace("{nameSpace}", data.nameSpace);
                    var typeName = field.fieldName;
                    codeTemplate = codeTemplate.Replace("{typeName}", typeName);
                    var inheritType = field.genericType ? $"{data.InheritType(field.inheritTypeInx)}<{typeName}>" : data.InheritType(field.inheritTypeInx);
                    codeTemplate = codeTemplate.Replace("{inherit}", inheritType);
                    var codes = codeTemplate.Split(new string[] { "--\r\n" }, StringSplitOptions.None);
                    var sb = new StringBuilder();
                    var sb1 = new StringBuilder();
                    if (hasns)
                        sb.Append(codes[0]);
                    sb.Append(codes[1]);
                    for (int i = 0; i < field.fields.Count; i++)
                    {
                        var fieldCode = codes[2].Replace("{fieldType}", field.fields[i].Type.ToString());
                        fieldCode = fieldCode.Replace("{fieldName}", field.fields[i].name);
                        sb.Append(fieldCode);

                        fieldCode = codes[4].Replace("{fieldType}", field.fields[i].Type.ToString());
                        fieldCode = fieldCode.Replace("{fieldName}", field.fields[i].name);
                        fieldCode = fieldCode.Replace("{index}", i.ToString());
                        sb1.Append(fieldCode);
                    }
                    sb.AppendLine();
                    sb.Append(codes[3]);
                    sb.Append(sb1.ToString());
                    sb.Append(codes[5]);
                    if (hasns)
                        sb.Append(codes[6]);
                    scriptCode = sb.ToString();
                    if (!hasns)
                    {
                        var scriptCodes = scriptCode.Split(new string[] { "\r\n" }, StringSplitOptions.None);
                        sb.Clear();
                        for (int i = 0; i < scriptCodes.Length; i++)
                        {
                            if (scriptCodes[i].StartsWith("        "))
                                scriptCodes[i] = scriptCodes[i].Remove(0, 4);
                            else if (scriptCodes[i].StartsWith("    "))
                                scriptCodes[i] = scriptCodes[i].Remove(0, 4);
                            sb.AppendLine(scriptCodes[i]);
                        }
                        scriptCode = sb.ToString();
                    }
                    while (scriptCode.EndsWith("\n") | scriptCode.EndsWith("\r"))
                        scriptCode = scriptCode.Remove(scriptCode.Length - 1, 1);
                }

                string scriptCode1;
                {
                    var hasns = !string.IsNullOrEmpty(data.nameSpace);
                    if (string.IsNullOrEmpty(field.fieldName))
                        field.fieldName = field.name;
                    codeTemplate1 = codeTemplate1.Replace("{nameSpace}", data.nameSpace);
                    var typeName = field.fieldName;
                    codeTemplate1 = codeTemplate1.Replace("{typeName}", typeName);
                    var inheritType = field.genericType ? $"{data.InheritType(field.inheritTypeInx)}<{typeName}>" : data.InheritType(field.inheritTypeInx);
                    codeTemplate1 = codeTemplate1.Replace("{inherit}", inheritType);
                    var codes = codeTemplate1.Split(new string[] { "--\r\n" }, StringSplitOptions.None);
                    var sb = new StringBuilder();
                    var sb1 = new StringBuilder();
                    if (hasns)
                        sb.Append(codes[0]);
                    sb.Append(codes[1]);
                    sb.Append(codes[2]);
                    for (int i = 0; i < field.fields.Count; i++)
                    {
                        if (field.fields[i].Type == typeof(Button))
                        {
                            var addListenerText = $"{field.fields[i].name}.onClick.AddListener(On{field.fields[i].name}Click);";
                            var fieldCode = codes[3].Replace("{AddListener}", addListenerText);
                            sb.Append(fieldCode);

                            fieldCode = codes[5].Replace("{methodEvent}", $"On{field.fields[i].name}Click()");
                            sb1.Append(fieldCode);
                        }
                        else if (field.fields[i].Type == typeof(Toggle))
                        {
                            var addListenerText = $"{field.fields[i].name}.onValueChanged.AddListener(On{field.fields[i].name}Changed);";
                            var fieldCode = codes[3].Replace("{AddListener}", addListenerText);
                            sb.Append(fieldCode);

                            fieldCode = codes[5].Replace("{methodEvent}", $"On{field.fields[i].name}Changed(bool isOn)");
                            sb1.Append(fieldCode);
                        }
                    }
                    sb.Append(codes[4]);
                    sb.AppendLine();
                    sb.Append(sb1.ToString());
                    sb.Append(codes[6]);
                    if (hasns)
                        sb.Append(codes[7]);
                    scriptCode1 = sb.ToString();
                    if (!hasns)
                    {
                        var scriptCodes = scriptCode1.Split(new string[] { "\r\n" }, StringSplitOptions.None);
                        sb.Clear();
                        for (int i = 0; i < scriptCodes.Length; i++)
                        {
                            if (scriptCodes[i].StartsWith("        "))
                                scriptCodes[i] = scriptCodes[i].Remove(0, 4);
                            else if (scriptCodes[i].StartsWith("    "))
                                scriptCodes[i] = scriptCodes[i].Remove(0, 4);
                            sb.AppendLine(scriptCodes[i]);
                        }
                        scriptCode1 = sb.ToString();
                    }
                    while (scriptCode1.EndsWith("\n") | scriptCode1.EndsWith("\r"))
                        scriptCode1 = scriptCode1.Remove(scriptCode1.Length - 1, 1);
                }

                var files = Directory.GetFiles("Assets", $"{field.fieldName}.cs", SearchOption.AllDirectories);
                if (files.Length > 0)
                {
                    var path = files[0];
                    File.WriteAllText(path, scriptCode);
                    Debug.Log($"生成成功:{path}");
                }
                else if (!string.IsNullOrEmpty(data.SavePath(field.savePathInx)))
                {
                    var path = data.SavePath(field.savePathInx) + $"/{field.fieldName}.cs";
                    File.WriteAllText(path, scriptCode);
                    Debug.Log($"生成成功:{path}");
                }
                var path1 = string.Empty;
                var hasExt = false;
                files = Directory.GetFiles("Assets", $"{field.fieldName}Ext.cs", SearchOption.AllDirectories);
                if (files.Length > 0)
                {
                    path1 = files[0];
                    hasExt = true;
                }
                else if (!string.IsNullOrEmpty(data.SavePathExt(field.savePathExtInx)))
                {
                    path1 = data.SavePathExt(field.savePathExtInx) + $"/{field.fieldName}Ext.cs";
                    hasExt = true;
                }
                if (hasExt)
                {
                    if (!File.Exists(path1))
                    {
                        File.WriteAllText(path1, scriptCode1);
                    }
                    else
                    {
                        var code = EditorUtility.DisplayDialogComplex("写入脚本文件", "脚本已存在, 是否替换? 或 尾部添加?", "替换", "忽略", "尾部添加");
                        switch (code)
                        {
                            case 0:
                                File.WriteAllText(path1, scriptCode1);
                                break;
                            case 2:
                                File.AppendAllText(path1, $"\r\n/*{scriptCode1}*/");
                                break;
                            default:
                                goto J;
                        }
                    }
                    Debug.Log($"生成成功:{path1}");
                }
                J: AssetDatabase.Refresh();
            }
            if (GUILayout.Button("生成脚本(主工程)"))
            {
                var codeTemplate = @"namespace {nameSpace} 
{
--
    public partial class {typeName} : {inherit}
    {
--
        public {fieldType} {fieldName};
--
        void OnValidate()
        {
--
            {fieldName} = transform.GetComponentsInChildren<{fieldType}>(true)[{index}]{extend};
--
        }
    }
--
}";
                var codeTemplate1 = @"namespace {nameSpace} 
{
--
    //项目需要用到UTF8编码进行保存, 默认情况下是中文编码(GB2312), 如果更新MVC后发现脚本的中文乱码则需要处理一下
    //以下是设置UTF8编码的Url:方法二 安装插件
    //url:https://blog.csdn.net/hfy1237/article/details/129858976
    public partial class {typeName}
    {
--
        private void Start()
        {
--
            {AddListener}
--
        }
--
        private void {methodEvent}
        {
        }
--
    }
--
}";
                string scriptCode;
                {
                    var hasns = !string.IsNullOrEmpty(data.nameSpace);
                    if (string.IsNullOrEmpty(field.fieldName))
                        field.fieldName = field.name;
                    codeTemplate = codeTemplate.Replace("{nameSpace}", data.nameSpace);
                    var typeName = field.fieldName;
                    codeTemplate = codeTemplate.Replace("{typeName}", typeName);
                    var inheritType = field.genericType ? $"{data.InheritType(field.inheritTypeInx)}<{typeName}>" : data.InheritType(field.inheritTypeInx);
                    codeTemplate = codeTemplate.Replace("{inherit}", inheritType);
                    var codes = codeTemplate.Split(new string[] { "--\r\n" }, StringSplitOptions.None);
                    var sb = new StringBuilder();
                    var sb1 = new StringBuilder();
                    if (hasns)
                        sb.Append(codes[0]);
                    sb.Append(codes[1]);
                    for (int i = 0; i < field.fields.Count; i++)
                    {
                        var fieldCode = codes[2].Replace("{fieldType}", field.fields[i].Type.ToString());
                        fieldCode = fieldCode.Replace("{fieldName}", field.fields[i].name);
                        sb.Append(fieldCode);

                        int index = -1;
                        if (field.fields[i].Type == typeof(GameObject))
                        {
                            fieldCode = codes[4].Replace("{fieldType}", "UnityEngine.Transform");
                            fieldCode = fieldCode.Replace("{extend}", ".gameObject");
                            var components = field.GetComponentsInChildren(typeof(Transform), true);
                            for (int x = 0; x < components.Length; x++)
                            {
                                if (components[x].gameObject == field.fields[i].target)
                                {
                                    index = x;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            fieldCode = codes[4].Replace("{fieldType}", field.fields[i].Type.ToString());
                            fieldCode = fieldCode.Replace("{extend}", "");
                            var components = field.GetComponentsInChildren(field.fields[i].Type, true);
                            for (int x = 0; x < components.Length; x++)
                            {
                                if (components[x] == field.fields[i].target)
                                {
                                    index = x;
                                    break;
                                }
                            }
                        }
                        fieldCode = fieldCode.Replace("{fieldName}", field.fields[i].name);
                        fieldCode = fieldCode.Replace("{index}", index.ToString());
                        sb1.Append(fieldCode);
                    }
                    sb.AppendLine();
                    sb.Append(codes[3]);
                    sb.Append(sb1.ToString());
                    sb.Append(codes[5]);
                    if (hasns)
                        sb.Append(codes[6]);
                    scriptCode = sb.ToString();
                    if (!hasns)
                    {
                        var scriptCodes = scriptCode.Split(new string[] { "\r\n" }, StringSplitOptions.None);
                        sb.Clear();
                        for (int i = 0; i < scriptCodes.Length; i++)
                        {
                            if (scriptCodes[i].StartsWith("        "))
                                scriptCodes[i] = scriptCodes[i].Remove(0, 4);
                            else if (scriptCodes[i].StartsWith("    "))
                                scriptCodes[i] = scriptCodes[i].Remove(0, 4);
                            sb.AppendLine(scriptCodes[i]);
                        }
                        scriptCode = sb.ToString();
                    }
                    while (scriptCode.EndsWith("\n") | scriptCode.EndsWith("\r"))
                        scriptCode = scriptCode.Remove(scriptCode.Length - 1, 1);
                    var files = Directory.GetFiles("Assets", $"{field.fieldName}.cs", SearchOption.AllDirectories);
                    if (files.Length > 0)
                    {
                        var path = files[0];
                        File.WriteAllText(path, scriptCode);
                        Debug.Log($"生成成功:{path}");
                    }
                    else if (!string.IsNullOrEmpty(data.SavePath(field.savePathInx)))
                    {
                        var path = data.SavePath(field.savePathInx) + $"/{field.fieldName}.cs";
                        File.WriteAllText(path, scriptCode);
                        Debug.Log($"生成成功:{path}");
                    }
                }

                string scriptCode1;
                {
                    var hasns = !string.IsNullOrEmpty(data.nameSpace);
                    if (string.IsNullOrEmpty(field.fieldName))
                        field.fieldName = field.name;
                    codeTemplate1 = codeTemplate1.Replace("{nameSpace}", data.nameSpace);
                    var typeName = field.fieldName;
                    codeTemplate1 = codeTemplate1.Replace("{typeName}", typeName);
                    var inheritType = field.genericType ? $"{data.InheritType(field.inheritTypeInx)}<{typeName}>" : data.InheritType(field.inheritTypeInx);
                    codeTemplate1 = codeTemplate1.Replace("{inherit}", inheritType);
                    var codes = codeTemplate1.Split(new string[] { "--\r\n" }, StringSplitOptions.None);
                    var sb = new StringBuilder();
                    var sb1 = new StringBuilder();
                    if (hasns)
                        sb.Append(codes[0]);
                    sb.Append(codes[1]);
                    sb.Append(codes[2]);
                    for (int i = 0; i < field.fields.Count; i++)
                    {
                        if (field.fields[i].Type == typeof(Button))
                        {
                            var addListenerText = $"{field.fields[i].name}.onClick.AddListener(On{field.fields[i].name}Click);";
                            var fieldCode = codes[3].Replace("{AddListener}", addListenerText);
                            sb.Append(fieldCode);

                            fieldCode = codes[5].Replace("{methodEvent}", $"On{field.fields[i].name}Click()");
                            sb1.Append(fieldCode);
                        }
                        else if (field.fields[i].Type == typeof(Toggle))
                        {
                            var addListenerText = $"{field.fields[i].name}.onValueChanged.AddListener(On{field.fields[i].name}Changed);";
                            var fieldCode = codes[3].Replace("{AddListener}", addListenerText);
                            sb.Append(fieldCode);

                            fieldCode = codes[5].Replace("{methodEvent}", $"On{field.fields[i].name}Changed(bool isOn)");
                            sb1.Append(fieldCode);
                        }
                    }
                    sb.Append(codes[4]);
                    sb.AppendLine();
                    sb.Append(sb1.ToString());
                    sb.Append(codes[6]);
                    if (hasns)
                        sb.Append(codes[7]);
                    scriptCode1 = sb.ToString();
                    if (!hasns)
                    {
                        var scriptCodes = scriptCode1.Split(new string[] { "\r\n" }, StringSplitOptions.None);
                        sb.Clear();
                        for (int i = 0; i < scriptCodes.Length; i++)
                        {
                            if (scriptCodes[i].StartsWith("        "))
                                scriptCodes[i] = scriptCodes[i].Remove(0, 4);
                            else if (scriptCodes[i].StartsWith("    "))
                                scriptCodes[i] = scriptCodes[i].Remove(0, 4);
                            sb.AppendLine(scriptCodes[i]);
                        }
                        scriptCode1 = sb.ToString();
                    }
                    while (scriptCode1.EndsWith("\n") | scriptCode1.EndsWith("\r"))
                        scriptCode1 = scriptCode1.Remove(scriptCode1.Length - 1, 1);
                    var path1 = string.Empty;
                    var hasExt = false;
                    var files = Directory.GetFiles("Assets", $"{field.fieldName}Ext.cs", SearchOption.AllDirectories);
                    if (files.Length > 0)
                    {
                        path1 = files[0];
                        hasExt = true;
                        var lines = new List<string>(File.ReadAllLines(path1));
                        int startIndex = 0;
                        for (int i = 0; i < lines.Count; i++)
                        {
                            if (lines[i].Contains("private void Start()"))
                            {
                                startIndex = i + 2;
                                break;
                            }
                        }
                        for (int i = 0; i < field.fields.Count; i++)
                        {
                            var isBtn = field.fields[i].Type == typeof(Button);
                            var isToggle = field.fields[i].Type == typeof(Toggle);
                            if (!isBtn & !isToggle)
                                continue;
                            var addListenerText = $"{field.fields[i].name}.onClick.AddListener";
                            if (!Contains(lines, addListenerText))
                            {
                                addListenerText = $"{field.fields[i].name}.{(isBtn ? "onClick" : "onValueChanged")}.AddListener(On{field.fields[i].name}{(isBtn ? "Click" : "Changed")});";
                                lines.Insert(startIndex, (hasns ? "            " : "        ") + addListenerText);
                                startIndex++;
                                var fieldCode = codes[5].Replace("{methodEvent}", $"On{field.fields[i].name}{(isBtn ? "Click()" : "Changed(bool isOn)")}");
                                var fieldCodes = fieldCode.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                                for (int x = 0; x < fieldCodes.Length; x++)
                                {
                                    var fieldCode2 = fieldCodes[x];
                                    if (!hasns)
                                    {
                                        if (fieldCode2.StartsWith("        "))
                                            fieldCode2 = fieldCode2.Remove(0, 4);
                                        else if (fieldCode2.StartsWith("    "))
                                            fieldCode2 = fieldCode2.Remove(0, 4);
                                    }
                                    lines.Insert(lines.Count - (hasns ? 2 : 1), fieldCode2);
                                }
                            }
                        }
                        scriptCode1 = "";
                        for (int i = 0; i < lines.Count; i++)
                            scriptCode1 += lines[i] + "\r\n";
                    }
                    else if (!string.IsNullOrEmpty(data.SavePathExt(field.savePathExtInx)))
                    {
                        path1 = data.SavePathExt(field.savePathExtInx) + $"/{field.fieldName}Ext.cs";
                        hasExt = true;
                    }
                    if (hasExt) 
                    {
                        File.WriteAllText(path1, scriptCode1);
                        Debug.Log($"生成成功:{path1}");
                    }
                }
                AssetDatabase.Refresh();
                field.compiling = true;
            }
        }

        static bool Contains(List<string> list, string text) 
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].Contains(text))
                    return true;
            }
            return false;
        }

        [DidReloadScripts]
        static void OnScriptCompilation()
        {
            var gameObject = Selection.activeGameObject;
            if (gameObject == null)
                return;
            if (!gameObject.TryGetComponent<FieldCollection>(out var fieldCollection))
                return;
            if (fieldCollection.compiling)
            {
                fieldCollection.compiling = false;
                var data = PersistHelper.Deserialize<JsonSave>("fcdata.json");
                string componentTypeName;
                if (string.IsNullOrEmpty(data.nameSpace))
                    componentTypeName = fieldCollection.fieldName;
                else
                    componentTypeName = data.nameSpace + "." + fieldCollection.fieldName;
                var type = AssemblyHelper.GetType(componentTypeName);
                if (type == null)
                    return;
                if (fieldCollection.TryGetComponent(type, out var component))
                {
                    component.GetType().GetMethod("OnValidate", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(component, null);
                    return;
                }
                fieldCollection.gameObject.AddComponent(type);
            }
        }
    }
}
#endif
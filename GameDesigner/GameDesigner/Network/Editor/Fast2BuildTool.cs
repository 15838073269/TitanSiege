#if UNITY_EDITOR
using System.IO;
using System;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using Net.Helper;

public class Fast2BuildTools2 : EditorWindow
{
    private string search = "", search1 = "";
    private string searchBind = "", searchBind1 = "";
    private string searchAssemblies = "";
    private DateTime searchTime;
    private TypeData[] types;
    private Vector2 scrollPosition;
    private Vector2 scrollPosition1;
    private bool serField = true;
    private bool serProperty = true;
    private string typeEntry1;
    private string methodEntry1;
    private string selectType;
    private Data data = new Data();

    [MenuItem("GameDesigner/Network/Fast2BuildTool-2")]
    static void ShowWindow()
    {
        var window = GetWindow<Fast2BuildTools2>("快速序列化2生成工具");
        window.Show();
    }

    private void OnEnable()
    {
        LoadData();
        searchAssemblies = data.searchAssemblies;
        var assemblyNames = searchAssemblies.Split('|');
        var types1 = new List<TypeData>();
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        foreach (var assemblie in assemblies)
        {
            var name = assemblie.GetName().Name;
            if (assemblyNames.Contains(name))
                types1.AddRange(AddTypes(assemblie));
        }
        types = types1.ToArray();
    }

    private List<TypeData> AddTypes(Assembly assembly)
    {
        var types1 = new List<TypeData>();
        var types2 = assembly.GetTypes().Where(t => !t.IsAbstract & !t.IsInterface & !t.IsGenericType & !t.IsGenericType & !t.IsGenericTypeDefinition).ToArray();
        var types3 = typeof(Vector2).Assembly.GetTypes().Where(t => !t.IsAbstract & !t.IsInterface & !t.IsGenericType & !t.IsGenericType & !t.IsGenericTypeDefinition).ToArray();
        var typeslist = new List<Type>(types2);
        typeslist.AddRange(types3);
        foreach (var obj in typeslist)
        {
            var str = obj.FullName;
            types1.Add(new TypeData() { name = str, type = obj });
        }
        return types1;
    }

    private void OnGUI()
    {
        EditorGUI.BeginChangeCheck();
        EditorGUILayout.BeginHorizontal();
        search = EditorGUILayout.TextField("搜索绑定类型", search, GUI.skin.FindStyle("SearchTextField"));
        if (GUILayout.Button("搜索", GUILayout.Width(50f)))
            search1 = string.Empty;
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        searchBind = EditorGUILayout.TextField("搜索已绑定类型", searchBind, GUI.skin.FindStyle("SearchTextField"));
        if (GUILayout.Button("搜索", GUILayout.Width(50f)))
            searchBind1 = string.Empty;
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        data.searchAssemblies = EditorGUILayout.TextField("搜索的程序集", data.searchAssemblies);
        if (data.searchAssemblies != searchAssemblies)
        {
            searchAssemblies = data.searchAssemblies;
            SaveData();
        }
        if (GUILayout.Button("刷新", GUILayout.Width(50f)))
            OnEnable();
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("全部展开")) 
            foreach (var type1 in data.typeNames)
                type1.foldout = true;
        if (GUILayout.Button("全部收起"))
            foreach (var type1 in data.typeNames)
                type1.foldout = false;
        if (GUILayout.Button("全部字段更新"))
        {
            UpdateFields();
            SaveData();
            Debug.Log("全部字段已更新完成!");
        }
        if (GUILayout.Button("类型变动更新"))
        {
            var count = 0;
            foreach (var typeName in data.typeNames)
            {
                foreach (var type1 in types)
                {
                    var names = type1.name.Split('.');
                    var name = names[names.Length - 1];
                    var names1 = typeName.name.Split('.');
                    var name1 = names1[names1.Length - 1];
                    if (name == name1) 
                    {
                        typeName.name = type1.name;
                        count++;
                        break;
                    }
                }
            }
            SaveData();
            Debug.Log($"类型变动更新完成! 变动:{count}");
        }
        if (GUILayout.Button("显示类名"))
        {
            data.showType = 1;
            SaveData();
        }
        if (GUILayout.Button("完全显示"))
        {
            data.showType = 0;
            SaveData();
        }
        if (GUILayout.Button("引用文件夹"))
        {
            var path = EditorUtility.OpenFolderPanel("选择文件夹路径", "", "");
            var files = Directory.GetFiles(path, "*.cs", SearchOption.TopDirectoryOnly);
            AddSerTypeInDirectory(files);
        }
        if (GUILayout.Button("引用文件夹(全部)"))
        {
            var path = EditorUtility.OpenFolderPanel("选择文件夹路径", "", "");
            var files = Directory.GetFiles(path, "*.cs", SearchOption.AllDirectories);
            AddSerTypeInDirectory(files);
        }
        EditorGUILayout.EndHorizontal();
        scrollPosition1 = GUILayout.BeginScrollView(scrollPosition1, false, true, GUILayout.MaxHeight(position.height / 2));
        for (int i = 0; i < data.typeNames.Count; i++)
        {
            var type1 = data.typeNames[i];
            var rect = EditorGUILayout.GetControlRect();
            var color = GUI.color;
            if (type1.name == selectType)
                GUI.color = Color.green;
            string name;
            if (data.showType == 1)
            {
                var names = type1.name.Split('.');
                name = names[names.Length - 1];
            }
            else name = type1.name;
            EditorGUI.LabelField(new Rect(rect.position, rect.size - new Vector2(50, 0)), i.ToString());
            type1.foldout = EditorGUI.Foldout(new Rect(rect.position + new Vector2(15, 0), rect.size - new Vector2(50, 0)), type1.foldout, name, true);
            GUI.color = color;
            if (type1.foldout)
            {
                EditorGUI.indentLevel = 2;
                foreach (var field in type1.fields)
                    field.serialize = EditorGUILayout.Toggle(field.name, field.serialize);
                EditorGUI.indentLevel = 0;
            }
            if (GUI.Button(new Rect(rect.position + new Vector2(position.width - 50, 0), new Vector2(20, rect.height)), "x"))
            {
                data.typeNames.Remove(type1);
                SaveData();
                return;
            }
            if (rect.Contains(Event.current.mousePosition) & Event.current.button == 1)
            {
                GenericMenu menu = new GenericMenu();
                menu.AddItem(new GUIContent("全部勾上"), false, () =>
                {
                    type1.fields.ForEach(item => item.serialize = true);
                });
                menu.AddItem(new GUIContent("全部取消"), false, () =>
                {
                    type1.fields.ForEach(item => item.serialize = false);
                });
                menu.AddItem(new GUIContent("更新字段"), false, () =>
                {
                    UpdateField(type1);
                    SaveData();
                });
                menu.AddItem(new GUIContent("移除"), false, () =>
                {
                    data.typeNames.Remove(type1);
                    SaveData();
                });
                menu.ShowAsContext();
            }
        }
        GUILayout.EndScrollView();
        if (search != search1)
        {
            search1 = search;
            searchTime = DateTime.Now.AddMilliseconds(20);
        }
        if (DateTime.Now > searchTime & search.Length > 0)
        {
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, true, GUILayout.MaxHeight(position.height / 2));
            foreach (var type1 in types)
            {
                if (!type1.name.ToLower().Contains(search.ToLower()))
                    continue;
                if (GUILayout.Button(type1.name))
                {
                    AddSerType(type1);
                    return;
                }
            }
            GUILayout.EndScrollView();
        }
        if (searchBind != searchBind1)
        {
            searchBind1 = searchBind;
            searchTime = DateTime.Now.AddMilliseconds(20);
            if (searchBind.Length == 0)
                selectType = "";
        }
        if (DateTime.Now > searchTime & searchBind.Length > 0)
        {
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, true, GUILayout.MaxHeight(position.height / 6));
            foreach (var type1 in data.typeNames)
            {
                if (!type1.name.ToLower().Contains(searchBind.ToLower()))
                    continue;
                if (GUILayout.Button(type1.name))
                {
                    var scrollPosition2 = new Vector2();
                    for (int i = 0; i < data.typeNames.Count; i++)
                    {
                        if (data.typeNames[i].name == type1.name) 
                        {
                            scrollPosition1 = scrollPosition2;
                            selectType = type1.name;
                            break;
                        }
                        scrollPosition2.y += 20f;//20是foldout标签
                        if (data.typeNames[i].foldout)
                            scrollPosition2.y += data.typeNames[i].fields.Count * 20f;
                    }
                    break;
                }
            }
            GUILayout.EndScrollView();
        }
        serField = EditorGUILayout.Toggle("序列化字段:", serField);
        serProperty = EditorGUILayout.Toggle("序列化属性:", serProperty);
        GUILayout.BeginHorizontal();
        data.typeEntry = EditorGUILayout.TextField("收集类名:", data.typeEntry);
        data.methodEntry = EditorGUILayout.TextField("收集方法:", data.methodEntry);
        if (data.typeEntry != typeEntry1)
        {
            typeEntry1 = data.typeEntry;
            SaveData();
        }
        if (data.methodEntry != methodEntry1)
        {
            methodEntry1 = data.methodEntry;
            SaveData();
        }
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("保存路径:", data.savePath);
        if (GUILayout.Button("选择路径", GUILayout.Width(100)))
        {
            data.savePath = EditorUtility.OpenFolderPanel("保存路径", "", "");
            SaveData();
        }
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("保存路径1:", data.savePath1);
        if (GUILayout.Button("选择路径1", GUILayout.Width(100)))
        {
            data.savePath1 = EditorUtility.OpenFolderPanel("保存路径", "", "");
            SaveData();
        }
        GUILayout.EndHorizontal();
        if (GUILayout.Button("生成绑定代码", GUILayout.Height(30)))
        {
            if (string.IsNullOrEmpty(data.savePath))
            {
                EditorUtility.DisplayDialog("提示", "请选择生成脚本路径!", "确定");
                return;
            }
            List<Type> types = new List<Type>();
            foreach (var type1 in data.typeNames)
            {
                Type type = AssemblyHelper.GetType(type1.name);
                if (type == null)
                {
                    Debug.Log($"类型:{type1.name}已不存在!");
                    continue;
                }
                Fast2BuildMethod.Build(type, data.savePath, serField, serProperty, type1.fields.ConvertAll((item)=> !item.serialize ? item.name : ""));
                Fast2BuildMethod.BuildArray(type, data.savePath);
                Fast2BuildMethod.BuildGeneric(type, data.savePath);
                if (!string.IsNullOrEmpty(data.savePath1)) 
                {
                    Fast2BuildMethod.Build(type, data.savePath1, serField, serProperty, type1.fields.ConvertAll((item) => !item.serialize ? item.name : ""));
                    Fast2BuildMethod.BuildArray(type, data.savePath1);
                    Fast2BuildMethod.BuildGeneric(type, data.savePath1);
                }
                types.Add(type);
            }
            if (!string.IsNullOrEmpty(data.typeEntry)) 
            {
                var types1 = (Type[])AssemblyHelper.GetType(data.typeEntry).GetMethod(data.methodEntry).Invoke(null, null);
                foreach (var type in types1)
                {
                    if (type.IsGenericType)
                    {
                        var args = type.GenericTypeArguments;
                        if (args.Length == 1)
                        {
                            //TODO
                        }
                        else if (args.Length == 2)
                        {
                            var text = Fast2BuildMethod.BuildDictionary(type, out var className1);
                            File.WriteAllText(data.savePath + $"//{className1}.cs", text);
                        }
                    }
                    else 
                    {
                        Fast2BuildMethod.Build(type, data.savePath, serField, serProperty, new List<string>());
                        Fast2BuildMethod.BuildArray(type, data.savePath);
                        Fast2BuildMethod.BuildGeneric(type, data.savePath);
                        if (!string.IsNullOrEmpty(data.savePath1))
                        {
                            Fast2BuildMethod.Build(type, data.savePath1, serField, serProperty, new List<string>());
                            Fast2BuildMethod.BuildArray(type, data.savePath1);
                            Fast2BuildMethod.BuildGeneric(type, data.savePath1);
                        }
                    }
                    types.Add(type);
                }
            }
            Fast2BuildMethod.BuildBindingType(types, data.savePath);
            Debug.Log("生成完成.");
            AssetDatabase.Refresh();
        }
        if (EditorGUI.EndChangeCheck())
            SaveData();
    }

    private void AddSerTypeInDirectory(string[] files)
    {
        foreach (var file in files)
        {
            var texts = File.ReadLines(file);
            var nameSpace = "";
            var typeName = "";
            foreach (var text in texts)
            {
                if (text.Contains("namespace"))
                {
                    nameSpace = text.Replace("namespace", "").Trim();
                    continue;
                }
                var index = 0;
                var has = false;
                if (text.Contains("class"))
                {
                    index = text.IndexOf("class") + 6;
                    has = true;
                }
                if (text.Contains("struct"))
                {
                    index = text.IndexOf("struct") + 7;
                    has = true;
                }
                if (has)
                {
                    var end = text.Length - index;
                    var typeName1 = text.Substring(index, end);
                    var typeName2 = typeName1.Split(':');
                    typeName = typeName2[0].Trim();
                    string typeFull;
                    if (nameSpace == "")
                        typeFull = typeName;
                    else
                        typeFull = $"{nameSpace}.{typeName}";
                    foreach (var type1 in types)
                    {
                        if (type1.name != typeFull)
                            continue;
                        AddSerType(type1);
                        break;
                    }
                    typeName = "";
                }
            }
            if (typeName.Length > 0)
            {
                var typeFull = $"{nameSpace}.{typeName}";
                foreach (var type1 in types)
                {
                    if (type1.name != typeFull)
                        continue;
                    AddSerType(type1);
                    break;
                }
            }
        }
    }

    private void AddSerType(TypeData type1)
    {
        if (data.typeNames.Find(item => item.name == type1.name) == null)
        {
            var fields = type1.type.GetFields(BindingFlags.Public | BindingFlags.Instance);
            var properties = type1.type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var fields1 = new List<FieldData>();
            foreach (var item in fields)
            {
                if (item.GetCustomAttribute<Net.Serialize.NonSerialized>() != null)
                    continue;
                fields1.Add(new FieldData() { name = item.Name, serialize = true });
            }
            foreach (var item in properties)
            {
                if (!item.CanRead | !item.CanWrite)
                    continue;
                if (item.GetIndexParameters().Length > 0)
                    continue;
                if (item.GetCustomAttribute<Net.Serialize.NonSerialized>() != null)
                    continue;
                fields1.Add(new FieldData() { name = item.Name, serialize = true });
            }
            data.typeNames.Add(new FoldoutData() { name = type1.name, fields = fields1, foldout = false });
        }
        SaveData();
    }

    private void UpdateFields() 
    {
        foreach (var fd in data.typeNames) 
        {
            UpdateField(fd);
        }
        SaveData();
    }

    private void UpdateField(FoldoutData fd)
    {
        Type type = null;
        foreach (var type2 in types)
        {
            if (fd.name == type2.name)
            {
                type = type2.type;
                break;
            }
        }
        if (type == null)
        {
            Debug.Log(fd.name + "类型为空!");
            return;
        }
        var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
        var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
        var fields1 = new List<FieldData>();
        foreach (var item in fields)
        {
            if (item.GetCustomAttribute<Net.Serialize.NonSerialized>() != null)
                continue;
            fields1.Add(new FieldData() { name = item.Name, serialize = true });
        }
        foreach (var item in properties)
        {
            if (!item.CanRead | !item.CanWrite)
                continue;
            if (item.GetIndexParameters().Length > 0)
                continue;
            if (item.GetCustomAttribute<Net.Serialize.NonSerialized>() != null)
                continue;
            fields1.Add(new FieldData() { name = item.Name, serialize = true });
        }
        foreach (var item in fields1)
        {
            if (fd.fields.Find(item1 => item1.name == item.name, out var fd1))
            {
                item.serialize = fd1.serialize;
            }
        }
        fd.fields = fields1;
    }

    void LoadData() 
    {
        data = PersistHelper.Deserialize<Data>("fastProtoBuild.json");
    }

    void SaveData()
    {
        PersistHelper.Serialize(data, "fastProtoBuild.json");
    }

    public class FoldoutData 
    {
        public string name;
        public bool foldout;
        public List<FieldData> fields = new List<FieldData>();
    }

    public class FieldData 
    {
        public string name;
        public bool serialize;
        public int select;
    }

    public class TypeData 
    {
        public string name;
        public Type type;
    }

    public class Data
    {
        public string savePath, savePath1;
        public List<FoldoutData> typeNames = new List<FoldoutData>();
        public string typeEntry;
        public string methodEntry;
        public int showType;
        public string searchAssemblies = "Assembly-CSharp|Assembly-CSharp-firstpass";
    }
}
#endif
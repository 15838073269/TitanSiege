#if UNITY_EDITOR
using System.IO;
using System;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using Net.Helper;
using System.Text;

public class Fast2BuildTools2 : EditorWindow
{
    private string search = "", search1 = "";
    private string searchBind = "", searchBind1 = "";
    private DateTime searchTime;
    private TypeData[] types;
    private Vector2 scrollPosition;
    private Vector2 scrollPosition1;
    
    private string selectType;
    private PersistentData data = new PersistentData();

    [MenuItem("GameDesigner/Network/Fast2BuildTool-2")]
    static void ShowWindow()
    {
        var window = GetWindow<Fast2BuildTools2>("快速序列化2生成工具");
        window.Show();
    }

    private void OnEnable()
    {
        LoadData();
        var assemblyNames = data.SearchAssemblies.Split('|');
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
        var types2 = assembly.GetTypes().Where(t => !t.IsAbstract & !t.IsInterface & !t.IsGenericType & !t.IsGenericTypeDefinition).ToArray();
        foreach (var type in types2)
        {
            var str = type.FullName;
            types1.Add(new TypeData() { name = str, type = type });
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
        data.SearchAssemblies = EditorGUILayout.TextField("搜索的程序集", data.SearchAssemblies);
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
            data.ShowType = 1;
        }
        if (GUILayout.Button("完全显示"))
        {
            data.ShowType = 0;
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
        scrollPosition1 = GUILayout.BeginScrollView(scrollPosition1, false, true, GUILayout.MaxHeight(search.Length > 0 ? position.height / 2 : position.height));
        for (int i = 0; i < data.typeNames.Count; i++)
        {
            var type1 = data.typeNames[i];
            var rect = EditorGUILayout.GetControlRect();
            var color = GUI.color;
            if (type1.name == selectType)
                GUI.color = Color.green;
            string name;
            if (data.ShowType == 1)
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
        data.SerField = EditorGUILayout.Toggle("序列化字段:", data.SerField);
        data.SerProperty = EditorGUILayout.Toggle("序列化属性:", data.SerProperty);
        data.IsCompress = EditorGUILayout.Toggle("使用字节压缩:", data.IsCompress);
        data.SortingOrder = EditorGUILayout.IntField("绑定序号:", data.SortingOrder);
        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("保存路径:", data.SavePath);
        if (GUILayout.Button("选择路径", GUILayout.Width(100)))
        {
            var savePath = EditorUtility.OpenFolderPanel("保存路径", "", "");
            //相对于Assets路径
            var uri = new Uri(Application.dataPath.Replace('/', '\\'));
            var relativeUri = uri.MakeRelativeUri(new Uri(savePath));
            data.SavePath = relativeUri.ToString();
        }
        GUILayout.EndHorizontal();
        if (GUILayout.Button("生成绑定代码", GUILayout.Height(30)))
        {
            if (string.IsNullOrEmpty(data.SavePath))
            {
                EditorUtility.DisplayDialog("提示", "请选择生成脚本路径!", "确定");
                return;
            }
            var types = new HashSet<Type>();
            foreach (var type1 in data.typeNames)
            {
                Type type = AssemblyHelper.GetType(type1.name);
                if (type == null)
                {
                    Debug.Log($"类型:{type1.name}已不存在!");
                    continue;
                }
                StringBuilder code;
                if (data.IsCompress)
                    code = Fast2BuildMethod.BuildNew(type, data.SerField, data.SerProperty, type1.fields.ConvertAll((item) => !item.serialize ? item.name : ""), data.SavePath, types);
                else
                    code = Fast2BuildMethod.BuildNewFast(type, data.SerField, data.SerProperty, type1.fields.ConvertAll((item) => !item.serialize ? item.name : ""), data.SavePath, types);
                code.AppendLine(Fast2BuildMethod.BuildArray(type).ToString());
                code.AppendLine(Fast2BuildMethod.BuildGeneric(typeof(List<>).MakeGenericType(type)).ToString());
                var className = type.ToString().Replace(".", "").Replace("+", "");
                File.WriteAllText(data.SavePath + $"//{className}Bind.cs", code.ToString());
                types.Add(type);
            }
            Fast2BuildMethod.BuildBindingType(types, data.SavePath, data.SortingOrder);
            Fast2BuildMethod.BuildBindingExtension(types, data.SavePath);
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
        data = PersistHelper.Deserialize<PersistentData>("fastProtoBuild.json");
        data.Init();
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

    public class PersistentData
    {
        private string savePath;
        public List<FoldoutData> typeNames = new List<FoldoutData>();
        private int showType;
        private string searchAssemblies = "UnityEngine.CoreModule|Assembly-CSharp|Assembly-CSharp-firstpass";
        private bool serField = true;
        private bool serProperty = true;
        private bool isCompress = true;
        private int sortingOrder = 1;
        private bool init;

        public void Init() 
        {
            init = true;
        }

        public string SavePath 
        {
            get => savePath;
            set 
            {
                if (savePath != value & init)
                    PersistHelper.Serialize(this, "fastProtoBuild.json");
                savePath = value;
            }
        }
        public int ShowType
        {
            get => showType;
            set
            {
                if (showType != value & init)
                    PersistHelper.Serialize(this, "fastProtoBuild.json");
                showType = value;
            }
        }
        public string SearchAssemblies
        {
            get => searchAssemblies;
            set
            {
                if (searchAssemblies != value & init)
                    PersistHelper.Serialize(this, "fastProtoBuild.json");
                searchAssemblies = value;
            }
        }
        public bool SerField
        {
            get => serField;
            set
            {
                if (serField != value & init)
                    PersistHelper.Serialize(this, "fastProtoBuild.json");
                serField = value;
            }
        }
        public bool SerProperty
        {
            get => serProperty;
            set
            {
                if (serProperty != value & init)
                    PersistHelper.Serialize(this, "fastProtoBuild.json");
                serProperty = value;
            }
        }
        public bool IsCompress
        {
            get => isCompress;
            set
            {
                if (isCompress != value & init)
                    PersistHelper.Serialize(this, "fastProtoBuild.json");
                isCompress = value;
            }
        }

        public int SortingOrder 
        {
            get => sortingOrder;
            set 
            {
                if (sortingOrder != value & init)
                    PersistHelper.Serialize(this, "fastProtoBuild.json");
                sortingOrder = value;
            }
        }
    }
}
#endif
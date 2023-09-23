#if UNITY_EDITOR
using Net.Helper;
using System;
using System.IO;
using UnityEditor;
using UnityEngine;

public class ImportSettingWindow : EditorWindow
{
    private Vector2 scrollPosition;
    private Data data;
    private readonly string[] displayedOptions = new string[] { "使用者", "开发者" };

    public class Data 
    {
        public string path = "Assets/Plugins/GameDesigner";
        public int develop;
    }

    [MenuItem("GameDesigner/Import Window", priority = 0)]
    static void ShowWindow()
    {
        var window = GetWindow<ImportSettingWindow>("Import");
        window.Show();
    }

    private void OnEnable()
    {
        LoadData();
    }

    private void OnDisable()
    {
        SaveData();
    }

    void LoadData()
    {
        data = PersistHelper.Deserialize<Data>("importdata.json");
    }

    void SaveData()
    {
        PersistHelper.Serialize(data, "importdata.json");
    }

    private void DrawGUI(string path, string name, string sourceProtocolName, string copyToProtocolName, Action import /*= null*/, string pluginsPath /*= "Assets/Plugins/GameDesigner/"*/) 
    {
        if (Directory.Exists(path))
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button($"重新导入{name}模块"))
            {
                import?.Invoke();
                Import(sourceProtocolName, copyToProtocolName, pluginsPath);
            }
            if (data.develop == 1)
            {
                if (GUILayout.Button($"反导{name}模块", GUILayout.Width(200)))
                    ReverseImport(sourceProtocolName, copyToProtocolName, pluginsPath);
            }
            GUI.color = Color.red;
            if (GUILayout.Button($"移除{name}模块"))
            {
                Directory.Delete(path, true);
                File.Delete(path + ".meta");
                AssetDatabase.Refresh();
            }
            GUI.color = Color.white;
            GUILayout.EndHorizontal();
        }
        else if (GUILayout.Button($"导入{name}模块"))
        {
            import?.Invoke();
            Import(sourceProtocolName, copyToProtocolName, pluginsPath);
        }
    }

    private void OnGUI()
    {
        EditorGUI.BeginChangeCheck();
        data.develop = EditorGUILayout.Popup("使用模式", data.develop, displayedOptions);
        if (EditorGUI.EndChangeCheck())
            SaveData();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("导入路径:", data.path);
        if (GUILayout.Button("选择路径", GUILayout.Width(80)))
        {
            var importPath = EditorUtility.OpenFolderPanel("选择导入路径", "", "");
            //相对于Assets路径
            var uri = new Uri(Application.dataPath.Replace('/', '\\'));
            var relativeUri = uri.MakeRelativeUri(new Uri(importPath));
            data.path = relativeUri.ToString();
            SaveData();
        }
        EditorGUILayout.EndHorizontal();
        scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, true);
        EditorGUILayout.HelpBox("Tcp&Gcp模块 基础网络协议模块", MessageType.Info);
        var path = data.path + "/Network/Gcp";
        DrawGUI(path, "Gcp", "Network/Gcp~", "Network/Gcp", null, data.path + "/");

        EditorGUILayout.HelpBox("Udx模块 可用于帧同步，视频流，直播流，大数据传输", MessageType.Info);
        path = data.path + "/Network/Udx";
        DrawGUI(path, "Udx", "Network/Udx~", "Network/Udx", null, data.path + "/");
        
        EditorGUILayout.HelpBox("Kcp模块 可用于帧同步 即时游戏", MessageType.Info);
        path = data.path + "/Network/Kcp";
        DrawGUI(path, "Kcp", "Network/Kcp~", "Network/Kcp", null, data.path + "/");
        
        EditorGUILayout.HelpBox("Web模块 可用于网页游戏 WebGL", MessageType.Info);
        path = data.path + "/Network/Web";
        DrawGUI(path, "Web", "Network/Web~", "Network/Web", null, data.path + "/");
        
        EditorGUILayout.HelpBox("StateMachine模块 可用格斗游戏，或基础游戏动作设计", MessageType.Info);
        path = data.path + "/StateMachine";
        DrawGUI(path, "StateMachine", "StateMachine~", "StateMachine", null, data.path + "/");
        
        EditorGUILayout.HelpBox("NetworkComponents模块 封装了一套完整的客户端网络组件", MessageType.Info);
        path = data.path + "/Component";
        DrawGUI(path, "NetworkComponent", "Component~", "Component", ()=> {
            Import("Common~", "Common", data.path + "/");//依赖
        }, data.path + "/");

        EditorGUILayout.HelpBox("MVC模块 可用于帧同步设计，视图，逻辑分离", MessageType.Info);
        path = data.path + "/MVC";
        DrawGUI(path, "MVC", "MVC~", "MVC", null, data.path + "/"); 
        
        EditorGUILayout.HelpBox("ECS模块 可用于双端的独立代码运行", MessageType.Info);
        path = data.path + "/ECS";
        DrawGUI(path, "ECS", "ECS~", "ECS", null, data.path + "/");
        
        EditorGUILayout.HelpBox("Common模块 常用模块", MessageType.Info);
        path = data.path + "/Common";
        DrawGUI(path, "Common", "Common~", "Common", null, data.path + "/");
        
        EditorGUILayout.HelpBox("MMORPG模块 用于MMORPG设计怪物点, 巡逻点, 地图数据等", MessageType.Info);
        path = data.path + "/MMORPG";
        DrawGUI(path, "MMORPG", "MMORPG~", "MMORPG", () => {
            Import("AOI~", "AOI", data.path + "/");//依赖
        }, data.path + "/");

        EditorGUILayout.HelpBox("AOI模块 可用于MMORPG大地图同步方案，九宫格同步， 或者单机大地图分割显示", MessageType.Info);
        path = data.path + "/AOI";
        DrawGUI(path, "AOI", "AOI~", "AOI", null, data.path + "/");

        EditorGUILayout.HelpBox("Recast & Detour寻路模块 用于双端AI寻路", MessageType.Info);
        path = data.path + "/Recast";
        DrawGUI(path, "Recast", "Recast~", "Recast", null, data.path + "/");

        EditorGUILayout.HelpBox("Framework模块 客户端框架, 包含热更新，Excel读表，Global全局管理，其他管理", MessageType.Info);
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("HyBridCLR地址,下载或Git导入包"))
        {
            Application.OpenURL(@"https://gitee.com/focus-creative-games/hybridclr_unity");
        }
        path = data.path + "/Framework";
        DrawGUI(path, "Framework", "Framework~", "Framework", null, data.path + "/");
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.HelpBox("ParrelSync插件, 可以克隆两个一模一样的项目进行网络同步调式, 极快解决联机同步问题", MessageType.Info);
        path = data.path + "/ParrelSync";
        DrawGUI(path, "ParrelSync", "ParrelSync~", "ParrelSync", null, data.path + "/");

        EditorGUILayout.HelpBox("基础模块导入", MessageType.Warning);
        if (GUILayout.Button("基础模块导入", GUILayout.Height(20)))
        {
            Import("Network/Gcp~", "Network/Gcp", data.path + "/");
            Import("Component~", "Component", data.path + "/");
            Import("Common~", "Common", data.path + "/");
        }
        EditorGUILayout.HelpBox("所有模块导入", MessageType.Warning);
        if (GUILayout.Button("所有模块导入", GUILayout.Height(20)))
        {
            Import("Network/Gcp~", "Network/Gcp", data.path + "/");
            Import("Network/Udx~", "Network/Udx", data.path + "/");
            Import("Network/Kcp~", "Network/Kcp", data.path + "/");
            Import("Network/Web~", "Network/Web", data.path + "/");
            Import("Component~", "Component", data.path + "/");
            Import("StateMachine~", "StateMachine", data.path + "/");
            Import("MVC~", "MVC", data.path + "/");
            Import("ECS~", "ECS", data.path + "/");
            Import("Common~", "Common", data.path + "/");
            Import("MMORPG~", "MMORPG", data.path + "/");
            Import("AOI~", "AOI", data.path + "/");
            Import("Recast~", "Recast", data.path + "/");
            Import("Framework~", "Framework", data.path + "/");
        }
        EditorGUILayout.HelpBox("所有案例导入，用于学习和快速上手", MessageType.Warning);
        if (GUILayout.Button("案例导入", GUILayout.Height(20)))
        {
            Import("Network/Gcp~", "Network/Gcp", data.path + "/");
            Import("Network/Udx~", "Network/Udx", data.path + "/");
            Import("Network/Kcp~", "Network/Kcp", data.path + "/");
            Import("Network/Web~", "Network/Web", data.path + "/");
            Import("Component~", "Component", data.path + "/");
            Import("StateMachine~", "StateMachine", data.path + "/");
            Import("MVC~", "MVC", data.path + "/");
            Import("ECS~", "ECS", data.path + "/");
            Import("Common~", "Common", data.path + "/");
            Import("MMORPG~", "MMORPG", data.path + "/");
            Import("AOI~", "AOI", data.path + "/");
            Import("Recast~", "Recast", data.path + "/");
            Import("Example~", "Example", "Assets/Samples/GameDesigner/");
        }
        EditorGUILayout.HelpBox("重新导入已导入的模块", MessageType.Warning);
        if (GUILayout.Button("重新导入已导入的模块", GUILayout.Height(20)))
        {
            ReImport("Network/Gcp~", "Network/Gcp", data.path + "/");
            ReImport("Network/Udx~", "Network/Udx", data.path + "/");
            ReImport("Network/Kcp~", "Network/Kcp", data.path + "/");
            ReImport("Network/Web~", "Network/Web", data.path + "/");
            ReImport("Component~", "Component", data.path + "/");
            ReImport("StateMachine~", "StateMachine", data.path + "/");
            ReImport("MVC~", "MVC", data.path + "/");
            ReImport("ECS~", "ECS", data.path + "/");
            ReImport("Common~", "Common", data.path + "/");
            ReImport("MMORPG~", "MMORPG", data.path + "/");
            ReImport("AOI~", "AOI", data.path + "/");
            ReImport("Recast~", "Recast", data.path + "/");
            ReImport("Framework~", "Framework", data.path + "/");
        }
        if (data.develop == 1) 
        {
            EditorGUILayout.HelpBox("反导已导入的模块", MessageType.Warning);
            if (GUILayout.Button("反导已导入的模块", GUILayout.Height(20)))
            {
                ReverseImport("Network/Gcp~", "Network/Gcp", data.path + "/");
                ReverseImport("Network/Udx~", "Network/Udx", data.path + "/");
                ReverseImport("Network/Kcp~", "Network/Kcp", data.path + "/");
                ReverseImport("Network/Web~", "Network/Web", data.path + "/");
                ReverseImport("Component~", "Component", data.path + "/");
                ReverseImport("StateMachine~", "StateMachine", data.path + "/");
                ReverseImport("MVC~", "MVC", data.path + "/");
                ReverseImport("ECS~", "ECS", data.path + "/");
                ReverseImport("Common~", "Common", data.path + "/");
                ReverseImport("MMORPG~", "MMORPG", data.path + "/");
                ReverseImport("AOI~", "AOI", data.path + "/");
                ReverseImport("Recast~", "Recast", data.path + "/");
                ReverseImport("Framework~", "Framework", data.path + "/");
            }
        }
        GUILayout.EndScrollView();
        GUILayout.Space(10);
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Gitee", GUILayout.Height(20)))
        {
            Application.OpenURL(@"https://gitee.com/leng_yue/GameDesigner");
        }
        if (GUILayout.Button("加入QQ群:825240544", GUILayout.Height(20))) 
        {
            Application.OpenURL(@"https://jq.qq.com/?_wv=1027&k=nx1Psgjz");
        }
        if (GUILayout.Button("版本:2022.12.12", GUILayout.Height(20)))
        {
        }
        GUILayout.EndHorizontal(); 
        GUILayout.Space(10);
    }

    private static void ReImport(string sourceProtocolName, string copyToProtocolName, string pluginsPath /*= "Assets/Plugins/GameDesigner/"*/)
    {
        var rootPath = "Packages/com.gamedesigner.network";//包的根路径
        if (!Directory.Exists(rootPath))
            rootPath = Application.dataPath + "/GameDesigner";//直接放Assets目录的路径
        if (!Directory.Exists(rootPath))
        {
            Debug.LogError("找不到根路径, 无法执行, 请使用包管理器添加gdnet, 或者根路径必须在Assets目录下!");
            return;
        }
        var path = $"{pluginsPath}{copyToProtocolName}/";
        if (!Directory.Exists(path))
            return;
        try { Directory.Delete(path, true); } catch { } //删除原文件再导入新文件
        Import(sourceProtocolName, copyToProtocolName, pluginsPath);
    }

    private static void Import(string sourceProtocolName, string copyToProtocolName, string pluginsPath /*= "Assets/Plugins/GameDesigner/"*/)
    {
        var rootPath = "Packages/com.gamedesigner.network";//包的根路径
        if (!Directory.Exists(rootPath))
            rootPath = Application.dataPath + "/GameDesigner";//直接放Assets目录的路径
        if (!Directory.Exists(rootPath))
        {
            Debug.LogError("找不到根路径, 无法执行, 请使用包管理器添加gdnet, 或者根路径必须在Assets目录下!");
            return;
        }
        var path = $"{rootPath}/{sourceProtocolName}/";
        if (!Directory.Exists(path))
        {
            Debug.LogError("找不到路径:" + path);
            return;
        }
        var files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);
        foreach (var file in files)
        {
            var item = file.Split(new string[] { sourceProtocolName }, StringSplitOptions.RemoveEmptyEntries);
            var newPath = $"{pluginsPath}{copyToProtocolName}/{item[1]}";
            var path1 = Path.GetDirectoryName(newPath);
            if (!Directory.Exists(path1))
                Directory.CreateDirectory(path1);
            try { File.Copy(file, newPath, true); }
            catch (Exception ex) { Debug.LogError(ex); }
        }
        Debug.Log($"导入{Path.GetFileName(copyToProtocolName)}完成!");
        AssetDatabase.Refresh();
    }

    private static void ReverseImport(string sourceProtocolName, string copyToProtocolName, string pluginsPath /*= "Assets/Plugins/GameDesigner/"*/)
    {
        var rootPath = "Packages/com.gamedesigner.network";//包的根路径
        if (!Directory.Exists(rootPath))
            rootPath = Application.dataPath + "/GameDesigner";//直接放Assets目录的路径
        if (!Directory.Exists(rootPath))
        {
            Debug.LogError("找不到根路径, 无法执行, 请使用包管理器添加gdnet, 或者根路径必须在Assets目录下!");
            return;
        }
        var path = $"{pluginsPath}{copyToProtocolName}/";
        if (!Directory.Exists(path))
            return;
        var files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);
        foreach (var file in files)
        {
            var newFile = file.Replace(path, "");
            var newPath = $"{rootPath}/{sourceProtocolName}/{newFile}";
            if (!Directory.Exists(Path.GetDirectoryName(newPath)))
                Directory.CreateDirectory(Path.GetDirectoryName(newPath));
            File.Copy(file, newPath, true);
        }
        Debug.Log($"反导出{Path.GetFileName(copyToProtocolName)}完成!");
        AssetDatabase.Refresh();
    }
}
#endif
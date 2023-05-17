#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEngine;

public class ImportSettingWindow : EditorWindow
{
    private Vector2 scrollPosition;

    [MenuItem("GameDesigner/Import Window", priority = 0)]
    static void ShowWindow()
    {
        var window = GetWindow<ImportSettingWindow>("Import");
        window.Show();
    }

    private void DrawGUI(string path, string name, string sourceProtocolName, string copyToProtocolName, Action import = null, string pluginsPath = "Assets/Plugins/GameDesigner/") 
    {
        if (Directory.Exists(path))
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button($"重新导入{name}模块"))
            {
                import?.Invoke();
                Import(sourceProtocolName, copyToProtocolName, pluginsPath);
            }
            if (GUILayout.Button($"反导{name}模块", GUILayout.Width(200)))
            {
                import?.Invoke();
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
        EditorGUILayout.LabelField("导入模块:");
        scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, true);
        EditorGUILayout.HelpBox("Tcp&Gcp模块 基础网络协议模块", MessageType.Info);
        var path = "Assets/Plugins/GameDesigner/Network/Gcp";
        DrawGUI(path, "Gcp", "Network/Gcp~", "Network/Gcp");

        EditorGUILayout.HelpBox("Udx模块 可用于帧同步，视频流，直播流，大数据传输", MessageType.Info);
        path = "Assets/Plugins/GameDesigner/Network/Udx";
        DrawGUI(path, "Udx", "Network/Udx~", "Network/Udx");
        
        EditorGUILayout.HelpBox("Kcp模块 可用于帧同步 即时游戏", MessageType.Info);
        path = "Assets/Plugins/GameDesigner/Network/Kcp";
        DrawGUI(path, "Kcp", "Network/Kcp~", "Network/Kcp");
        
        EditorGUILayout.HelpBox("Web模块 可用于网页游戏 WebGL", MessageType.Info);
        path = "Assets/Plugins/GameDesigner/Network/Web";
        DrawGUI(path, "Web", "Network/Web~", "Network/Web");
        
        EditorGUILayout.HelpBox("StateMachine模块 可用格斗游戏，或基础游戏动作设计", MessageType.Info);
        path = "Assets/Plugins/GameDesigner/StateMachine";
        DrawGUI(path, "StateMachine", "StateMachine~", "StateMachine");
        
        EditorGUILayout.HelpBox("NetworkComponents模块 封装了一套完整的客户端网络组件", MessageType.Info);
        path = "Assets/Plugins/GameDesigner/Component";
        DrawGUI(path, "NetworkComponent", "Component~", "Component", ()=> {
            Import("Common~", "Common");//依赖
        });

        EditorGUILayout.HelpBox("MVC模块 可用于帧同步设计，视图，逻辑分离", MessageType.Info);
        path = "Assets/Plugins/GameDesigner/MVC";
        DrawGUI(path, "MVC", "MVC~", "MVC"); 
        
        EditorGUILayout.HelpBox("ECS模块 可用于双端的独立代码运行", MessageType.Info);
        path = "Assets/Plugins/GameDesigner/ECS";
        DrawGUI(path, "ECS", "ECS~", "ECS");
        
        EditorGUILayout.HelpBox("Common模块 常用模块", MessageType.Info);
        path = "Assets/Plugins/GameDesigner/Common";
        DrawGUI(path, "Common", "Common~", "Common");
        
        EditorGUILayout.HelpBox("MMORPG模块 用于MMORPG设计怪物点, 巡逻点, 地图数据等", MessageType.Info);
        path = "Assets/Plugins/GameDesigner/MMORPG";
        DrawGUI(path, "MMORPG", "MMORPG~", "MMORPG");

        EditorGUILayout.HelpBox("AOI模块 可用于MMORPG大地图同步方案，九宫格同步， 或者单机大地图分割显示", MessageType.Info);
        path = "Assets/Plugins/GameDesigner/AOI";
        DrawGUI(path, "AOI", "AOI~", "AOI");
        
        EditorGUILayout.HelpBox("Framework模块 客户端框架, 包含热更新，Excel读表，Global全局管理，其他管理", MessageType.Info);
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("HyBridCLR地址,下载或Git导入包"))
        {
            Application.OpenURL(@"https://gitee.com/focus-creative-games/hybridclr_unity");
        }
        path = "Assets/Plugins/GameDesigner/Framework";
        DrawGUI(path, "Framework", "Framework~", "Framework");
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.HelpBox("ParrelSync插件, 可以克隆两个一模一样的项目进行网络同步调式, 极快解决联机同步问题", MessageType.Info);
        path = "Assets/Plugins/GameDesigner/ParrelSync";
        DrawGUI(path, "ParrelSync", "ParrelSync~", "ParrelSync");

        EditorGUILayout.HelpBox("基础模块导入", MessageType.Warning);
        if (GUILayout.Button("基础模块导入", GUILayout.Height(20)))
        {
            Import("Network/Gcp~", "Network/Gcp");
            Import("Component~", "Component");
            Import("Common~", "Common");
        }
        EditorGUILayout.HelpBox("所有模块导入", MessageType.Warning);
        if (GUILayout.Button("所有模块导入", GUILayout.Height(20)))
        {
            Import("Network/Gcp~", "Network/Gcp");
            Import("Network/Udx~", "Network/Udx");
            Import("Network/Kcp~", "Network/Kcp");
            Import("Network/Web~", "Network/Web");
            Import("Component~", "Component");
            Import("StateMachine~", "StateMachine");
            Import("MVC~", "MVC");
            Import("ECS~", "ECS");
            Import("Common~", "Common");
            Import("MMORPG~", "MMORPG");
            Import("AOI~", "AOI");
        }
        EditorGUILayout.HelpBox("所有案例导入，用于学习和快速上手", MessageType.Warning);
        if (GUILayout.Button("案例导入", GUILayout.Height(20)))
        {
            Import("Network/Gcp~", "Network/Gcp");
            Import("Network/Udx~", "Network/Udx");
            Import("Network/Kcp~", "Network/Kcp");
            Import("Network/Web~", "Network/Web");
            Import("Component~", "Component");
            Import("StateMachine~", "StateMachine");
            Import("MVC~", "MVC");
            Import("ECS~", "ECS");
            Import("Common~", "Common");
            Import("MMORPG~", "MMORPG");
            Import("AOI~", "AOI");
            Import("Example~", "Example", "Assets/Samples/GameDesigner/");
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

    private static void Import(string sourceProtocolName, string copyToProtocolName, string pluginsPath = "Assets/Plugins/GameDesigner/")
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
            File.Copy(file, newPath, true);
        }
        Debug.Log($"导入{Path.GetFileName(copyToProtocolName)}完成!");
        AssetDatabase.Refresh();
    }

    private static void ReverseImport(string sourceProtocolName, string copyToProtocolName, string pluginsPath = "Assets/Plugins/GameDesigner/")
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
        {
            Debug.LogError("找不到导入路径!");
            return;
        }
        var files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);
        foreach (var file in files)
        {
            var newFile = file.Replace(path, "");
            var newPath = $"{rootPath}/{sourceProtocolName}/{newFile}";
            File.Copy(file, newPath, true);
        }
        Debug.Log($"反导出{Path.GetFileName(copyToProtocolName)}完成!");
        AssetDatabase.Refresh();
    }
}
#endif
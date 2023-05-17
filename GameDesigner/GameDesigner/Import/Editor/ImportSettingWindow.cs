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
            if (GUILayout.Button($"���µ���{name}ģ��"))
            {
                import?.Invoke();
                Import(sourceProtocolName, copyToProtocolName, pluginsPath);
            }
            if (GUILayout.Button($"����{name}ģ��", GUILayout.Width(200)))
            {
                import?.Invoke();
                ReverseImport(sourceProtocolName, copyToProtocolName, pluginsPath);
            }
            GUI.color = Color.red;
            if (GUILayout.Button($"�Ƴ�{name}ģ��"))
            {
                Directory.Delete(path, true);
                File.Delete(path + ".meta");
                AssetDatabase.Refresh();
            }
            GUI.color = Color.white;
            GUILayout.EndHorizontal();
        }
        else if (GUILayout.Button($"����{name}ģ��"))
        {
            import?.Invoke();
            Import(sourceProtocolName, copyToProtocolName, pluginsPath);
        }
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("����ģ��:");
        scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, true);
        EditorGUILayout.HelpBox("Tcp&Gcpģ�� ��������Э��ģ��", MessageType.Info);
        var path = "Assets/Plugins/GameDesigner/Network/Gcp";
        DrawGUI(path, "Gcp", "Network/Gcp~", "Network/Gcp");

        EditorGUILayout.HelpBox("Udxģ�� ������֡ͬ������Ƶ����ֱ�����������ݴ���", MessageType.Info);
        path = "Assets/Plugins/GameDesigner/Network/Udx";
        DrawGUI(path, "Udx", "Network/Udx~", "Network/Udx");
        
        EditorGUILayout.HelpBox("Kcpģ�� ������֡ͬ�� ��ʱ��Ϸ", MessageType.Info);
        path = "Assets/Plugins/GameDesigner/Network/Kcp";
        DrawGUI(path, "Kcp", "Network/Kcp~", "Network/Kcp");
        
        EditorGUILayout.HelpBox("Webģ�� ��������ҳ��Ϸ WebGL", MessageType.Info);
        path = "Assets/Plugins/GameDesigner/Network/Web";
        DrawGUI(path, "Web", "Network/Web~", "Network/Web");
        
        EditorGUILayout.HelpBox("StateMachineģ�� ���ø���Ϸ���������Ϸ�������", MessageType.Info);
        path = "Assets/Plugins/GameDesigner/StateMachine";
        DrawGUI(path, "StateMachine", "StateMachine~", "StateMachine");
        
        EditorGUILayout.HelpBox("NetworkComponentsģ�� ��װ��һ�������Ŀͻ����������", MessageType.Info);
        path = "Assets/Plugins/GameDesigner/Component";
        DrawGUI(path, "NetworkComponent", "Component~", "Component", ()=> {
            Import("Common~", "Common");//����
        });

        EditorGUILayout.HelpBox("MVCģ�� ������֡ͬ����ƣ���ͼ���߼�����", MessageType.Info);
        path = "Assets/Plugins/GameDesigner/MVC";
        DrawGUI(path, "MVC", "MVC~", "MVC"); 
        
        EditorGUILayout.HelpBox("ECSģ�� ������˫�˵Ķ�����������", MessageType.Info);
        path = "Assets/Plugins/GameDesigner/ECS";
        DrawGUI(path, "ECS", "ECS~", "ECS");
        
        EditorGUILayout.HelpBox("Commonģ�� ����ģ��", MessageType.Info);
        path = "Assets/Plugins/GameDesigner/Common";
        DrawGUI(path, "Common", "Common~", "Common");
        
        EditorGUILayout.HelpBox("MMORPGģ�� ����MMORPG��ƹ����, Ѳ�ߵ�, ��ͼ���ݵ�", MessageType.Info);
        path = "Assets/Plugins/GameDesigner/MMORPG";
        DrawGUI(path, "MMORPG", "MMORPG~", "MMORPG");

        EditorGUILayout.HelpBox("AOIģ�� ������MMORPG���ͼͬ���������Ź���ͬ���� ���ߵ������ͼ�ָ���ʾ", MessageType.Info);
        path = "Assets/Plugins/GameDesigner/AOI";
        DrawGUI(path, "AOI", "AOI~", "AOI");
        
        EditorGUILayout.HelpBox("Frameworkģ�� �ͻ��˿��, �����ȸ��£�Excel����Globalȫ�ֹ�����������", MessageType.Info);
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("HyBridCLR��ַ,���ػ�Git�����"))
        {
            Application.OpenURL(@"https://gitee.com/focus-creative-games/hybridclr_unity");
        }
        path = "Assets/Plugins/GameDesigner/Framework";
        DrawGUI(path, "Framework", "Framework~", "Framework");
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.HelpBox("ParrelSync���, ���Կ�¡����һģһ������Ŀ��������ͬ����ʽ, ����������ͬ������", MessageType.Info);
        path = "Assets/Plugins/GameDesigner/ParrelSync";
        DrawGUI(path, "ParrelSync", "ParrelSync~", "ParrelSync");

        EditorGUILayout.HelpBox("����ģ�鵼��", MessageType.Warning);
        if (GUILayout.Button("����ģ�鵼��", GUILayout.Height(20)))
        {
            Import("Network/Gcp~", "Network/Gcp");
            Import("Component~", "Component");
            Import("Common~", "Common");
        }
        EditorGUILayout.HelpBox("����ģ�鵼��", MessageType.Warning);
        if (GUILayout.Button("����ģ�鵼��", GUILayout.Height(20)))
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
        EditorGUILayout.HelpBox("���а������룬����ѧϰ�Ϳ�������", MessageType.Warning);
        if (GUILayout.Button("��������", GUILayout.Height(20)))
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
        if (GUILayout.Button("����QQȺ:825240544", GUILayout.Height(20))) 
        {
            Application.OpenURL(@"https://jq.qq.com/?_wv=1027&k=nx1Psgjz");
        }
        if (GUILayout.Button("�汾:2022.12.12", GUILayout.Height(20)))
        {
        }
        GUILayout.EndHorizontal(); 
        GUILayout.Space(10);
    }

    private static void Import(string sourceProtocolName, string copyToProtocolName, string pluginsPath = "Assets/Plugins/GameDesigner/")
    {
        var rootPath = "Packages/com.gamedesigner.network";//���ĸ�·��
        if (!Directory.Exists(rootPath))
            rootPath = Application.dataPath + "/GameDesigner";//ֱ�ӷ�AssetsĿ¼��·��
        if (!Directory.Exists(rootPath))
        {
            Debug.LogError("�Ҳ�����·��, �޷�ִ��, ��ʹ�ð����������gdnet, ���߸�·��������AssetsĿ¼��!");
            return;
        }
        var path = $"{rootPath}/{sourceProtocolName}/";
        if (!Directory.Exists(path))
        {
            Debug.LogError("�Ҳ���·��:" + path);
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
        Debug.Log($"����{Path.GetFileName(copyToProtocolName)}���!");
        AssetDatabase.Refresh();
    }

    private static void ReverseImport(string sourceProtocolName, string copyToProtocolName, string pluginsPath = "Assets/Plugins/GameDesigner/")
    {
        var rootPath = "Packages/com.gamedesigner.network";//���ĸ�·��
        if (!Directory.Exists(rootPath))
            rootPath = Application.dataPath + "/GameDesigner";//ֱ�ӷ�AssetsĿ¼��·��
        if (!Directory.Exists(rootPath))
        {
            Debug.LogError("�Ҳ�����·��, �޷�ִ��, ��ʹ�ð����������gdnet, ���߸�·��������AssetsĿ¼��!");
            return;
        }
        var path = $"{pluginsPath}{copyToProtocolName}/";
        if (!Directory.Exists(path))
        {
            Debug.LogError("�Ҳ�������·��!");
            return;
        }
        var files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);
        foreach (var file in files)
        {
            var newFile = file.Replace(path, "");
            var newPath = $"{rootPath}/{sourceProtocolName}/{newFile}";
            File.Copy(file, newPath, true);
        }
        Debug.Log($"������{Path.GetFileName(copyToProtocolName)}���!");
        AssetDatabase.Refresh();
    }
}
#endif
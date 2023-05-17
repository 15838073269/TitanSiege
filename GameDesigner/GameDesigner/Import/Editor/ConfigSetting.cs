#if UNITY_EDITOR
using System.Diagnostics;
using UnityEditor;
using UnityEngine;

public class ConfigSetting : EditorWindow
{
    [MenuItem("GameDesigner/Config", priority = 100)]
    static void ShowWindow()
    {
        var window = GetWindow<ConfigSetting>("Config Setting");
        window.Show();
    }

    private void OnGUI()
    {
        EditorGUILayout.HelpBox("1.如果在Unity主线程更新网络所有事件, 将会减少线程开销, 同时也会占用主线程的Cpu时间, " +
            "你可以使用分析器查看性能, 如果对你的项目没有任何影响, 可使用此方案! 2.如果在多线程更新所有网络事件, 可以避免主线程占用问题, " +
            "但也会产生线程开销, 目前来看CPU核心数量增大, 对这个问题应该不会产生多大影响, " +
            "我这里给出的方案是: 如果多客户端, 可以考虑使用单线程更新所有网络事件 如果是单个客户端的话, 可以使用多线程来更新!", MessageType.Info);
        Net.Config.Config.MainThreadTick = EditorGUILayout.Toggle("Unity主线程处理网络:", Net.Config.Config.MainThreadTick);
        Net.Config.Config.BaseCapacity = EditorGUILayout.IntField("接收缓冲区基础容量:", Net.Config.Config.BaseCapacity);
        EditorGUILayout.TextField("网络操作的基础路径:", Net.Config.Config.BasePath);
        if (GUILayout.Button("打开配置文件夹"))
        {
            var configPath = Net.Config.Config.ConfigPath;
            configPath = configPath.Replace('/', '\\');
            Process.Start("explorer.exe", configPath + "\\");
        }
        if (GUILayout.Button("打开配置文件"))
        {
            var configPath = Net.Config.Config.ConfigPath + "/network.config";
            Process.Start("notepad.exe", configPath);
        }
    }
}
#endif
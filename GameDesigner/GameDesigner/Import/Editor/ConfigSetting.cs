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
        EditorGUILayout.HelpBox("1.�����Unity���̸߳������������¼�, ��������߳̿���, ͬʱҲ��ռ�����̵߳�Cpuʱ��, " +
            "�����ʹ�÷������鿴����, ����������Ŀû���κ�Ӱ��, ��ʹ�ô˷���! 2.����ڶ��̸߳������������¼�, ���Ա������߳�ռ������, " +
            "��Ҳ������߳̿���, Ŀǰ����CPU������������, ���������Ӧ�ò���������Ӱ��, " +
            "����������ķ�����: �����ͻ���, ���Կ���ʹ�õ��̸߳������������¼� ����ǵ����ͻ��˵Ļ�, ����ʹ�ö��߳�������!", MessageType.Info);
        Net.Config.Config.MainThreadTick = EditorGUILayout.Toggle("Unity���̴߳�������:", Net.Config.Config.MainThreadTick);
        Net.Config.Config.BaseCapacity = EditorGUILayout.IntField("���ջ�������������:", Net.Config.Config.BaseCapacity);
        EditorGUILayout.TextField("��������Ļ���·��:", Net.Config.Config.BasePath);
        if (GUILayout.Button("�������ļ���"))
        {
            var configPath = Net.Config.Config.ConfigPath;
            configPath = configPath.Replace('/', '\\');
            Process.Start("explorer.exe", configPath + "\\");
        }
        if (GUILayout.Button("�������ļ�"))
        {
            var configPath = Net.Config.Config.ConfigPath + "/network.config";
            Process.Start("notepad.exe", configPath);
        }
    }
}
#endif
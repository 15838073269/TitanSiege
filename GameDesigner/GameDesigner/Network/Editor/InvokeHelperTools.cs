#if (UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_WSA || UNITY_WEBGL) && UNITY_EDITOR
using Net;
using Net.Helper;
using System;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Callbacks;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Rect = UnityEngine.Rect;
using Vector2 = UnityEngine.Vector2;

public class InvokeHelperTools : EditorWindow, IPostprocessBuildWithReport, IPreprocessBuildWithReport
{
    SerializedObject serializedObject;

    [MenuItem("GameDesigner/Network/InvokeHelper")]
    static void ShowWindow()
    {
        var window = GetWindow<InvokeHelperTools>("字段同步，远程过程调用帮助工具");
        window.Show();
    }

    private void OnEnable()
    {
        LoadData();
        serializedObject = new SerializedObject(ConfigObject);
    }

    private void OnGUI()
    {
        if (serializedObject == null)
            return;
        EditorGUI.BeginChangeCheck();
        serializedObject.Update();
        scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, true, GUILayout.MaxHeight(position.height));
        var configProperty = serializedObject.FindProperty("Config");
        SerializedProperty property;
        Rect rect;
        EditorGUI.indentLevel = 0;
        EditorGUILayout.Foldout(true, "Config");
        EditorGUI.indentLevel = 1;
        EditorGUILayout.PropertyField(configProperty.FindPropertyRelative("onReloadInvoke"));
        EditorGUILayout.PropertyField(configProperty.FindPropertyRelative("syncVarClientEnable"));
        EditorGUILayout.PropertyField(configProperty.FindPropertyRelative("syncVarServerEnable"));
        EditorGUILayout.PropertyField(configProperty.FindPropertyRelative("collectRpc"));
        EditorGUILayout.PropertyField(configProperty.FindPropertyRelative("recordType"));
        rect = EditorGUILayout.GetControlRect(true, 45f, GUILayout.Width(position.width - 100f));
        EditorGUI.PropertyField(rect, configProperty.FindPropertyRelative("savePath"));
        rect.position = new Vector2(rect.position.x + (position.width - 90f), rect.position.y + 25f);
        rect.size = new Vector2(70f, 20f);
        if (GUI.Button(rect, "选择路径"))
        {
            var path = EditorUtility.OpenFolderPanel("选择路径", "", "");
            if (!string.IsNullOrEmpty(path))
            {
                //相对于Assets路径
                var uri = new Uri(Application.dataPath.Replace('/', '\\'));
                var relativeUri = uri.MakeRelativeUri(new Uri(path));
                Config.savePath = relativeUri.ToString();
            }
            SaveData();
        }
        property = configProperty.FindPropertyRelative("dllPaths");
        float propertyHeight = EditorGUI.GetPropertyHeight(property, true);
        rect = EditorGUILayout.GetControlRect(GUILayout.Width(position.width - 100f), GUILayout.Height(propertyHeight));
        EditorGUI.PropertyField(rect, property, true);
        rect.position = new Vector2(rect.position.x + (position.width - 90f), rect.position.y + 25f);
        rect.size = new Vector2(70f, 20f);
        if (GUI.Button(rect, "选择文件"))
        {
            var path = EditorUtility.OpenFilePanelWithFilters("选择文件", "", new string[] { "dll files", "dll,exe", "All files", "*" });
            if (!string.IsNullOrEmpty(path))
            {
                //相对于Assets路径
                var uri = new Uri(Application.dataPath.Replace('/', '\\'));
                var relativeUri = uri.MakeRelativeUri(new Uri(path));
                Config.dllPaths.Add(relativeUri.ToString());
            }
            SaveData();
        }
        rect = EditorGUILayout.GetControlRect();
        EditorGUI.LabelField(rect, "Rpc辅助");
        rect = EditorGUILayout.GetControlRect();
        Config.foldout = EditorGUI.Foldout(rect, Config.foldout, "RpcConfig", true);
        if (Config.foldout)
        {
            EditorGUI.indentLevel = 2;
            rect = EditorGUILayout.GetControlRect(GUILayout.Width(position.width - 100f));
            Config.rpcConfigSize = EditorGUI.DelayedIntField(rect, "Size", Config.rpcConfigSize);
            if (Config.rpcConfigSize != Config.rpcConfig.Count)
            {
                if (Config.rpcConfigSize > Config.rpcConfig.Count)
                {
                    var count = Config.rpcConfigSize - Config.rpcConfig.Count;
                    for (int i = 0; i < count; i++)
                    {
                        if (Config.rpcConfig.Count > 0)
                            Config.rpcConfig.Add(CloneHelper.DeepCopy<InvokeHelperConfigData>(Config.rpcConfig[Config.rpcConfig.Count - 1]));
                        else
                            Config.rpcConfig.Add(new InvokeHelperConfigData());
                    }
                }
                else
                {
                    var count = Config.rpcConfig.Count - Config.rpcConfigSize;
                    Config.rpcConfig.RemoveRange(Config.rpcConfigSize, count);
                }
                return;
            }
            var rpcConfigProperty = configProperty.FindPropertyRelative("rpcConfig");
            for (int i = 0; i < Config.rpcConfig.Count; i++)
            {
                var rpc = Config.rpcConfig[i];
                var arrayElement = rpcConfigProperty.GetArrayElementAtIndex(i);
                rect = EditorGUILayout.GetControlRect();
                rpc.foldout = EditorGUI.Foldout(rect, rpc.foldout, string.IsNullOrEmpty(rpc.name) ? $"Element {i}" : rpc.name, true);
                if (rpc.foldout)
                {
                    EditorGUI.indentLevel = 3;
                    rect = EditorGUILayout.GetControlRect(true, 20, GUILayout.Width(position.width - 100f));
                    EditorGUI.PropertyField(rect, arrayElement.FindPropertyRelative("name"), false);

                    rect = EditorGUILayout.GetControlRect(true, 45f, GUILayout.Width(position.width - 100f));
                    EditorGUI.PropertyField(rect, arrayElement.FindPropertyRelative("csprojPath"));
                    rect.position = new Vector2(rect.position.x + (position.width - 90f), rect.position.y + 25f);
                    rect.size = new Vector2(70f, 20f);
                    if (GUI.Button(rect, "选择文件"))
                    {
                        var path = EditorUtility.OpenFilePanel("选择文件", "", "csproj");
                        if (!string.IsNullOrEmpty(path))
                        {
                            //相对于Assets路径
                            var uri = new Uri(Application.dataPath.Replace('/', '\\'));
                            var relativeUri = uri.MakeRelativeUri(new Uri(path));
                            rpc.csprojPath = relativeUri.ToString();
                        }
                        SaveData();
                    }

                    rect = EditorGUILayout.GetControlRect(true, 45f, GUILayout.Width(position.width - 100f));
                    EditorGUI.PropertyField(rect, arrayElement.FindPropertyRelative("savePath"));
                    rect.position = new Vector2(rect.position.x + (position.width - 90f), rect.position.y + 25f);
                    rect.size = new Vector2(70f, 20f);
                    if (GUI.Button(rect, "选择路径"))
                    {
                        var path = EditorUtility.OpenFolderPanel("选择路径", "", "");
                        if (!string.IsNullOrEmpty(path))
                        {
                            //相对于Assets路径
                            var uri = new Uri(Application.dataPath.Replace('/', '\\'));
                            var relativeUri = uri.MakeRelativeUri(new Uri(path));
                            rpc.savePath = relativeUri.ToString();
                        }
                        SaveData();
                    }

                    rect = EditorGUILayout.GetControlRect(true, 45f, GUILayout.Width(position.width - 100f));
                    EditorGUI.PropertyField(rect, arrayElement.FindPropertyRelative("collectRpc"));

                    property = arrayElement.FindPropertyRelative("dllPaths");
                    propertyHeight = EditorGUI.GetPropertyHeight(property, true);
                    rect = EditorGUILayout.GetControlRect(GUILayout.Width(position.width - 100f), GUILayout.Height(propertyHeight));
                    EditorGUI.PropertyField(rect, property, true);
                    rect.position = new Vector2(rect.position.x + (position.width - 90f), rect.position.y + 25f);
                    rect.size = new Vector2(70f, 20f);
                    if (GUI.Button(rect, "选择文件"))
                    {
                        var path = EditorUtility.OpenFilePanelWithFilters("选择文件", "", new string[] { "dll files", "dll,exe", "All files", "*" });
                        if (!string.IsNullOrEmpty(path))
                        {
                            //相对于Assets路径
                            var uri = new Uri(Application.dataPath.Replace('/', '\\'));
                            var relativeUri = uri.MakeRelativeUri(new Uri(path));
                            rpc.dllPaths.Add(relativeUri.ToString());
                        }
                        SaveData();
                    }
                    EditorGUI.indentLevel = 2;
                }
            }
        }
        EditorGUI.indentLevel = 0;
        EditorGUILayout.GetControlRect(GUILayout.Height(10));
        GUILayout.EndScrollView();
        if (GUILayout.Button("保存配置", GUILayout.Height(30)))
        {
            SaveData();
            Debug.Log("保存完成!");
        }
        if (GUILayout.Button("执行", GUILayout.Height(30)))
        {
            SaveData();
            InvokeHelperBuild.OnScriptCompilation(Config, Config.syncVarClientEnable, Config.syncVarServerEnable);
            Debug.Log("更新完成!");
            AssetDatabase.Refresh();
        }
        if (GUILayout.Button("项目路径打印", GUILayout.Height(30)))
        {
            Debug.Log(Net.Config.Config.BasePath);
        }
        if (EditorGUI.EndChangeCheck())
            SaveData();
        serializedObject.ApplyModifiedProperties();
    }

    static InvokeHelperConfigObject configObject;
    internal static InvokeHelperConfigObject ConfigObject
    {
        get
        {
            if (configObject == null)
                configObject = CreateInstance<InvokeHelperConfigObject>();
            return configObject;
        }
    }
    internal static InvokeHelperConfig Config { get => ConfigObject.Config; set => ConfigObject.Config = value; }
    private Vector2 scrollPosition;

    internal static void LoadData()
    {
        Config = PersistHelper.Deserialize<InvokeHelperConfig>("invoke hepler.json");
    }

    internal static void SaveData()
    {
        PersistHelper.Serialize(Config, "invoke hepler.json");
    }

    [DidReloadScripts]
    public static void OnScriptCompilation()
    {
        LoadData();
        int change = 0;
        var path = "Assets/Scripts/Helper/";
        if (string.IsNullOrEmpty(Config.savePath))
        {
            Config.savePath = path;
            change++;
        }
        bool contains = false;
        path = "Library/ScriptAssemblies/Assembly-CSharp.dll";
        foreach (var dllPath in Config.dllPaths)
        {
            if (dllPath.Contains("Assembly-CSharp.dll"))//如果没有这个程序集就需要添加, 这个是默认的, 如果克隆了项目或者移植项目出现路径不对需要移除清除dllPaths
            {
                contains = true;
                break;
            }
        }
        if (!contains)
        {
            Config.dllPaths.Add(path);
            change++;
        }
        if (change > 0)
            SaveData();
        if (!Config.onReloadInvoke)
            return;
        InvokeHelperBuild.OnScriptCompilation(Config, Config.syncVarClientEnable, Config.syncVarServerEnable);
    }

    public int callbackOrder { get; set; }

    public void OnPreprocessBuild(UnityEditor.Build.Reporting.BuildReport report)
    {
        InvokeHelperBuild.OnScriptCompilation(Config, true, true);
        AssetDatabase.Refresh();
    }

    public void OnPostprocessBuild(UnityEditor.Build.Reporting.BuildReport report)
    {
        InvokeHelperBuild.OnScriptCompilation(Config, Config.syncVarClientEnable, Config.syncVarServerEnable);
        AssetDatabase.Refresh();
    }
}
#endif
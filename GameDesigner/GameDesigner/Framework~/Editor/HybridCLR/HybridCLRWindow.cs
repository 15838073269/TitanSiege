#if HYBRIDCLR
using Framework;
using HybridCLR.Editor;
using HybridCLR.Editor.Commands;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class HybridCLREntity
{
    [Header("拷贝dll到热更新目录")]
    public string hotfixPath = "AssetBundles/Hotfix/";
    [Header("拷贝dll到编译exe目录")]
    public string buildPath;// = $"build/{Application.productName}_Data/StreamingAssets/AssetBundles/Hotfix/";
    [Header("拷贝dll到StreamingAssetsPath目录")]
    public string streamingAssetsPath;

    [Header("补充元数据")]
    public List<string> AOTMetaAssemblyNames = new List<string>()
    {
        "mscorlib.dll",
        "System.dll",
        "System.Core.dll",
        "UniTask.dll",
    };
}

public class HybridCLRScriptable : ScriptableObject
{
    public HybridCLREntity entity = new HybridCLREntity();
}

public class HybridCLREdit : EditorWindow
{
    private HybridCLRScriptable scriptable;
    private HybridCLRScriptable Scriptable
    {
        get
        {
            if (scriptable == null)
                scriptable = CreateInstance<HybridCLRScriptable>();
            return scriptable;
        }
    }
    private SerializedObject serializedObject;

    [MenuItem("GameDesigner/Framework/HybridCLRWindow", priority = 3)]
    static void ShowWindow()
    {
        var instance = GetWindow<HybridCLREdit>("HybridCLRWindow");
        instance.Show(true);
    }

    private void OnEnable()
    {
        Scriptable.entity = PersistHelper.Deserialize<HybridCLREntity>("HybridCLR.json");
        if (string.IsNullOrEmpty(Scriptable.entity.buildPath))
            Scriptable.entity.buildPath = $"build/{Application.productName}_Data/StreamingAssets/AssetBundles/Hotfix/";
        if (string.IsNullOrEmpty(Scriptable.entity.streamingAssetsPath))
            Scriptable.entity.streamingAssetsPath = $"Assets/StreamingAssets/AssetBundles/Hotfix/";
        serializedObject = new SerializedObject(Scriptable);
    }

    private void OnGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("entity"), true);

        var version = EditorGUILayout.TextField("Version", GlobalSetting.Instance.Version);
        if (GlobalSetting.Instance.Version != version) 
        {
            GlobalSetting.Instance.Version = version;
            EditorUtility.SetDirty(GlobalSetting.Instance);
        }
        var autoIncrement = EditorGUILayout.Toggle("自动更新版本号", GlobalSetting.Instance.AutoIncrement);
        if (GlobalSetting.Instance.AutoIncrement != autoIncrement)
        {
            GlobalSetting.Instance.AutoIncrement = autoIncrement;
            EditorUtility.SetDirty(GlobalSetting.Instance);
        }

        if (GUILayout.Button("生成dll到hotfix")) 
        {
            CompileDllCommand.CompileDllActiveBuildTarget();
            CopyHotUpdateAssembliesToStreamingAssets(Scriptable.entity.hotfixPath);
        }
        if (GUILayout.Button("生成dll到build"))
        {
            CompileDllCommand.CompileDllActiveBuildTarget();
            CopyHotUpdateAssembliesToStreamingAssets(Scriptable.entity.buildPath);
        }
        if (GUILayout.Button("生成dll到StreamingAssets"))
        {
            CompileDllCommand.CompileDllActiveBuildTarget();
            CopyHotUpdateAssembliesToStreamingAssets(Scriptable.entity.streamingAssetsPath);
        }
        if (GUILayout.Button("清除持久路径缓存"))
        {
            var path = Application.persistentDataPath + "/AssetBundles/";
            if(Directory.Exists(path))
                Directory.Delete(path, true);
            Debug.Log("清除缓存完成！");
        }
        serializedObject.ApplyModifiedProperties();
    }

    private void OnDisable()
    {
        PersistHelper.Serialize(Scriptable.entity, "HybridCLR.json");
    }

    public void CopyHotUpdateAssembliesToStreamingAssets(string hotfixAssembliesDstDir)
    {
        var target = EditorUserBuildSettings.activeBuildTarget;
        string hotfixDllSrcDir = SettingsUtil.GetHotUpdateDllsOutputDirByTarget(target);
        if (!Directory.Exists(hotfixAssembliesDstDir))
            Directory.CreateDirectory(hotfixAssembliesDstDir);
        var assemblyNames = new List<string>(Scriptable.entity.AOTMetaAssemblyNames);
        assemblyNames.AddRange(SettingsUtil.HotUpdateAssemblyFilesExcludePreserved);//.HotUpdateAssemblyFiles);
        var xml = new XmlDocument();
        xml.Load("Assembly-CSharp.csproj");
        XmlNodeList node_list;
        var namespaceURI = xml.DocumentElement.NamespaceURI;
        if (!string.IsNullOrEmpty(namespaceURI))
        {
            var nsMgr = new XmlNamespaceManager(xml.NameTable); nsMgr.AddNamespace("ns", namespaceURI);
            node_list = xml.SelectNodes("/ns:Project/ns:ItemGroup", nsMgr);
        }
        else node_list = xml.SelectNodes("/Project/ItemGroup");

        var version = GlobalSetting.Instance.CheckAutoIncrement();
        var dict = GlobalSetting.Instance.GetVersionPathDict();
        dict["Version"] = version;

        foreach (var dll in assemblyNames)
        {
            string dllPath = $"{hotfixDllSrcDir}/{dll}";
            if (!File.Exists(dllPath))
            {
                for (int i = 0; i < node_list.Count; i++)
                {
                    var node = node_list.Item(i);
                    var node_child = node.ChildNodes;
                    foreach (XmlNode child_node in node_child)
                    {
                        if (child_node.LocalName != "Reference")
                            continue;
                        var value = child_node.InnerText;
                        var name = Path.GetFileName(value);
                        if (name == dll)
                        {
                            dllPath = value;
                            goto J;
                        }
                    }
                }
            J:;
            }
            string dllBytesPath = $"{hotfixAssembliesDstDir}/{dll}.bytes";
            File.Copy(dllPath, dllBytesPath, true);
            dict[$"AssetBundles/Hotfix/{dll}.bytes"] = GlobalSetting.ToMD5(dllBytesPath);
            Debug.Log($"复制热更新dll到目录: {dllPath} -> {dllBytesPath}");
        }
        GlobalSetting.Instance.SaveVersionDict(dict);
        AssetDatabase.Refresh();
    }
}
#endif
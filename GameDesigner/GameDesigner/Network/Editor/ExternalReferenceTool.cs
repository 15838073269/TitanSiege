#if UNITY_EDITOR
using Net.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using UnityEditor;
using UnityEngine;

public class ExternalReferenceTool : EditorWindow
{
    private Data config = new Data();
    private Vector2 scrollPosition, scrollPosition1;

    [MenuItem("GameDesigner/Network/ExternalReference")]
    static void ShowWindow()
    {
        var window = GetWindow<ExternalReferenceTool>("多个项目外部引用工具");
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

    private void OnGUI()
    {
        EditorGUI.BeginChangeCheck();
        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("项目文件列表:");
        if (GUILayout.Button("选择文件", GUILayout.Width(100)))
        {
            var csprojPath = EditorUtility.OpenFilePanel("选择文件", "", "csproj");
            if (!string.IsNullOrEmpty(csprojPath))
            {
                //相对于Assets路径
                var uri = new Uri(Application.dataPath.Replace('/', '\\'));
                var relativeUri = uri.MakeRelativeUri(new Uri(csprojPath));
                config.csprojPaths.Add(relativeUri.ToString());
            }
            SaveData();
        }
        GUILayout.EndHorizontal();
        scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, true, GUILayout.MaxHeight(position.height / 2));
        foreach (var path in config.csprojPaths)
        {
            var rect = EditorGUILayout.GetControlRect();
            EditorGUI.LabelField(new Rect(rect.position, rect.size - new Vector2(50, 0)), path);
            if (GUI.Button(new Rect(rect.position + new Vector2(position.width - 50, 0), new Vector2(20, rect.height)), "x"))
            {
                config.csprojPaths.Remove(path);
                SaveData();
                return;
            }
        }
        GUILayout.EndScrollView();

        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("引用文件夹列表:");
        if (GUILayout.Button("添加引用路径", GUILayout.Width(100)))
        {
            var path = EditorUtility.OpenFolderPanel("引用路径", "", "");
            if (!string.IsNullOrEmpty(path))
            {
                if (!config.paths.Contains(path))
                {
                    //相对于Assets路径
                    var uri = new Uri(Application.dataPath.Replace('/', '\\'));
                    var relativeUri = uri.MakeRelativeUri(new Uri(path));
                    config.paths.Add(relativeUri.ToString());
                }
            }
            SaveData();
        }
        GUILayout.EndHorizontal();

        scrollPosition1 = GUILayout.BeginScrollView(scrollPosition1, false, true, GUILayout.MaxHeight(position.height / 2));
        foreach (var path in config.paths)
        {
            var rect = EditorGUILayout.GetControlRect();
            EditorGUI.LabelField(new Rect(rect.position, rect.size - new Vector2(50, 0)), path);
            if (GUI.Button(new Rect(rect.position + new Vector2(position.width - 50, 0), new Vector2(20, rect.height)), "x"))
            {
                config.paths.Remove(path);
                SaveData();
                return;
            }
        }
        GUILayout.EndScrollView();

        config.searchPattern = EditorGUILayout.DelayedTextField("引用文件格式:", config.searchPattern);

        if (GUILayout.Button("执行", GUILayout.Height(30)))
        {
            foreach (var csprojPath in config.csprojPaths)
            {
                var xml = new XmlDocument();
                xml.Load(csprojPath);
                XmlNodeList node_list;
                var documentElement = xml.DocumentElement;
                var namespaceURI = xml.DocumentElement.NamespaceURI;
                if (!string.IsNullOrEmpty(namespaceURI))
                {
                    var nsMgr = new XmlNamespaceManager(xml.NameTable); nsMgr.AddNamespace("ns", namespaceURI);
                    node_list = xml.SelectNodes("/ns:Project/ns:ItemGroup", nsMgr);
                }
                else node_list = xml.SelectNodes("/Project/ItemGroup");
                foreach (var path in config.paths)
                {
                    var path1 = Path.GetFullPath(path);
                    var dir = new DirectoryInfo(path1);
                    var dirName = dir.Parent.FullName + "\\";
                    var files = Directory.GetFiles(path1, "*.*", SearchOption.AllDirectories);
                    var patterns = config.searchPattern.Replace("*", "").Split('|');
                    var fileList = new List<string>();
                    //相对于服务器路径
                    var csPath = Path.GetFullPath(csprojPath);
                    var uri = new Uri(csPath);
                    var uri1 = new Uri(path1);
                    var relativeUri = uri.MakeRelativeUri(uri1);
                    path1 = relativeUri.ToString().Replace('/', '\\');
                    //只包含三级目录的相对路径
                    uri1 = new Uri(dirName);
                    relativeUri = uri.MakeRelativeUri(uri1);
                    dirName = relativeUri.ToString().Replace('/', '\\');
                    foreach (var file in files)
                    {
                        foreach (var pattern in patterns)
                        {
                            if (file.EndsWith(pattern)) 
                            {
                                uri1 = new Uri(file);
                                relativeUri = uri.MakeRelativeUri(uri1);
                                var rPath = relativeUri.ToString().Replace('/', '\\');
                                fileList.Add(rPath);
                                break;
                            }
                        }
                    }
                    files = fileList.ToArray();
                    bool exist = false;
                    for (int i = 0; i < node_list.Count; i++)
                    {
                        var node = node_list.Item(i);
                        var node_child = node.ChildNodes;
                        foreach (XmlNode child_node in node_child)
                        {
                            if (child_node.LocalName != "Compile" & child_node.LocalName != "Content")
                                continue;
                            var attribute = child_node.Attributes.GetNamedItem("Include");
                            if (attribute == null)
                                continue;
                            var value = attribute.Value;
                            if (value.Contains(path1))
                            {
                                exist = true;
                                break;
                            }
                        }
                        if (exist == true)
                        {
                            foreach (var file in files)
                            {
                                bool isExist = false;
                                foreach (XmlNode child_node in node_child)
                                {
                                    if (child_node.LocalName != "Compile" & child_node.LocalName != "Content")
                                        continue;
                                    var attribute = child_node.Attributes.GetNamedItem("Include");
                                    if (attribute == null)
                                        continue;
                                    var value = attribute.Value;
                                    if (file == value) //必须一致性
                                    {
                                        isExist = true;
                                        break;
                                    }
                                }
                                if (!isExist) 
                                {
                                    if (file.EndsWith(".cs"))
                                    {
                                        var e = xml.CreateElement("Compile", namespaceURI);
                                        e.SetAttribute("Include", file);
                                        var e1 = xml.CreateElement("Link", namespaceURI);
                                        e1.InnerText = file.Replace(dirName, "");
                                        e.AppendChild(e1);
                                        node.AppendChild(e);
                                    }
                                    else
                                    {
                                        var e = xml.CreateElement("Content", namespaceURI);
                                        e.SetAttribute("Include", file);
                                        var e1 = xml.CreateElement("Link", namespaceURI);
                                        e1.InnerText = file.Replace(dirName, "");
                                        e.AppendChild(e1);
                                        node.AppendChild(e);
                                    }
                                }
                            }
                            foreach (XmlNode child_node in node_child) //检查移除的文件
                            {
                                if (child_node.LocalName != "Compile" & child_node.LocalName != "Content")
                                    continue;
                                var attribute = child_node.Attributes.GetNamedItem("Include");
                                if (attribute == null)
                                    continue;
                                var value = attribute.Value;
                                if (!files.Contains(value))
                                {
                                    node.RemoveChild(child_node);
                                }
                            }
                            break;
                        }
                    }
                    if (!exist)
                    {
                        var node = xml.CreateElement("ItemGroup", namespaceURI);
                        foreach (var file in files)
                        {
                            if (file.EndsWith(".cs"))
                            {
                                var e = xml.CreateElement("Compile", namespaceURI);
                                e.SetAttribute("Include", file);
                                var e1 = xml.CreateElement("Link", namespaceURI);
                                e1.InnerText = file.Replace(dirName, "");
                                e.AppendChild(e1);
                                node.AppendChild(e);
                            }
                            else 
                            {
                                var e = xml.CreateElement("Content", namespaceURI);
                                e.SetAttribute("Include", file);
                                var e1 = xml.CreateElement("Link", namespaceURI);
                                e1.InnerText = file.Replace(dirName, "");
                                e.AppendChild(e1);
                                node.AppendChild(e);
                            }
                        }
                        documentElement.AppendChild(node);
                    }
                }
                xml.Save(csprojPath);
            }
            Debug.Log("更新完成!");
        }
        if (EditorGUI.EndChangeCheck())
            SaveData();
    }

    void LoadData()
    {
        config = PersistHelper.Deserialize<Data>("external References.json");
    }

    void SaveData()
    {
        PersistHelper.Serialize(config, "external References.json");
    }

    internal class Data
    {
        public List<string> csprojPaths = new List<string>();
        public List<string> paths = new List<string>();
        public string searchPattern = "*.cs|*.txt|*.ini|*.conf|*.xls|*.xlsx|*.json";
    }
}
#endif
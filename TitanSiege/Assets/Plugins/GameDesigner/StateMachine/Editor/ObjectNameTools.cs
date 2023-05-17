#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Net.Helper;

public class ObjectName
{
    public string eName;
    public string cName;
}

public class ObjectNameTools : EditorWindow
{
    private List<ObjectName> objNames = new List<ObjectName>();
    private Vector2 scrollPosition1;
    private ObjectName objName = new ObjectName();
    
    [MenuItem("GameDesigner/Other/ObjectNameTools")]
    static void ShowWindow()
    {
        var window = GetWindow<ObjectNameTools>("更改物体名称工具");
        window.Show();
    }

    private void OnEnable()
    {
        objNames = PersistHelper.Deserialize<List<ObjectName>>("nameToolsData.json");
    }

    private void OnDisable()
    {
        Save();
    }

    private void OnGUI()
    {
        GUILayout.BeginHorizontal();
        objName.eName = EditorGUILayout.TextField("英文:", objName.eName);
        objName.cName = EditorGUILayout.TextField("中文:", objName.cName);
        if (GUILayout.Button("添加名称", GUILayout.Width(100)))
        {
            if (string.IsNullOrEmpty(objName.eName))
                return;
            foreach (var item in objNames)
            {
                if (item.eName == objName.eName)
                {
                    return;
                }
            }
            objNames.Add(objName);
            objName = new ObjectName();
            Save();
        }
        GUILayout.EndHorizontal();
        scrollPosition1 = GUILayout.BeginScrollView(scrollPosition1, false, true);
        for (int i = 0; i < objNames.Count; i++)
        {
            if (GUILayout.Button(objNames[i].eName + "  " + objNames[i].cName))
            {
                foreach (var obj in Selection.gameObjects)
                {
                    obj.name = objNames[i].eName;
                }
            }
        }
        GUILayout.EndScrollView();
    }

    void Save()
    {
        PersistHelper.Serialize(objNames, "nameToolsData.json");
    }
}
#endif
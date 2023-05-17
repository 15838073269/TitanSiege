#if UNITY_EDITOR
using Net.Helper;
using System;
using UnityEditor;
using UnityEngine;

namespace Net.MMORPG 
{
    public class BuildMapDataTools : EditorWindow
    {
        private Data data = new Data();

        [MenuItem("GameDesigner/MMORPG/BuildMapData")]
        public static void Init()
        {
            GetWindow<BuildMapDataTools>("BuildMapData", true);
        }
        private void OnEnable()
        {
            LoadData();
        }
        private void OnDisable()
        {
            SaveData();
        }
        void OnGUI()
        {
            EditorGUILayout.HelpBox("生成当前打开场景的数据, 并且以当前场景名为地图数据文件名", MessageType.Info);
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("生成路径:", data.savePath);
            if (GUILayout.Button("选择路径", GUILayout.Width(100)))
            {
                var savePath = EditorUtility.OpenFolderPanel("地图数据路径", "", "");
                if (!string.IsNullOrEmpty(savePath))
                {
                    //相对于Assets路径
                    var uri = new Uri(Application.dataPath.Replace('/', '\\'));
                    var relativeUri = uri.MakeRelativeUri(new Uri(savePath));
                    data.savePath = relativeUri.ToString();
                }
                SaveData();
            }
            GUILayout.EndHorizontal();
            if (GUILayout.Button("生成地图数据", GUILayout.Height(40)))
            {
                if (string.IsNullOrEmpty(data.savePath))
                {
                    EditorUtility.DisplayDialog("提示", "请选择生成脚本路径!", "确定");
                    return;
                }
                var scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
                var path = data.savePath + "/" + scene.name + ".mapData";
                MapData.WriteData(path);
                AssetDatabase.Refresh();
            }
        }
        void LoadData()
        {
            data = PersistHelper.Deserialize<Data>("buildMapData.json");
        }
        void SaveData()
        {
            PersistHelper.Serialize(data, "buildMapData.json");
        }
        internal class Data
        {
            public string savePath;
        }
    }
}
#endif
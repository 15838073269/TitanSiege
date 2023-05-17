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
            EditorGUILayout.HelpBox("���ɵ�ǰ�򿪳���������, �����Ե�ǰ������Ϊ��ͼ�����ļ���", MessageType.Info);
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("����·��:", data.savePath);
            if (GUILayout.Button("ѡ��·��", GUILayout.Width(100)))
            {
                var savePath = EditorUtility.OpenFolderPanel("��ͼ����·��", "", "");
                if (!string.IsNullOrEmpty(savePath))
                {
                    //�����Assets·��
                    var uri = new Uri(Application.dataPath.Replace('/', '\\'));
                    var relativeUri = uri.MakeRelativeUri(new Uri(savePath));
                    data.savePath = relativeUri.ToString();
                }
                SaveData();
            }
            GUILayout.EndHorizontal();
            if (GUILayout.Button("���ɵ�ͼ����", GUILayout.Height(40)))
            {
                if (string.IsNullOrEmpty(data.savePath))
                {
                    EditorUtility.DisplayDialog("��ʾ", "��ѡ�����ɽű�·��!", "ȷ��");
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
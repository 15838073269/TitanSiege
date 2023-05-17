using System.Collections.Generic;
using System;
using UnityEngine;
using Object = UnityEngine.Object;
using System.IO;

namespace Framework
{
    public enum AssetBundleType
    {
        None,
        Prefabs,
        UIPrefabs,
        Animation,
        Audio,
        Shader,
        Font,
        Material,
        Mesh,
        Model,
        PhysicMaterial,
        Scene,
        Script,
        Sprite,
        Texture,
        Video,
        UI,
        Other,
        Other1,
        Other2,
        Other3,
        All
    }

    [Serializable]
    public class AssetBundleInfo
    {
        public string name;
        public AssetBundleType type;
        public string path;
        internal AssetBundle assetBundle;

        public AssetBundle AssetBundle => assetBundle;
    }

    public enum AssetBundleMode
    {
        /// <summary>
        /// ����·��, Ҳ���Ǳ༭��·��
        /// </summary>
        LocalPath,
        /// <summary>
        /// ��·��, ����Ҫ�������ص�ģʽ
        /// </summary>
        StreamingAssetsPath,
        /// <summary>
        /// HFS������������Դ����
        /// </summary>
        HFSPath,
        /// <summary>
        /// �ڲ���Դ����ģʽ
        /// </summary>
        Resources,
    }

    public class ResourcesManager : MonoBehaviour
    {
#if UNITY_2020_1_OR_NEWER
        [NonReorderable]
#endif
        public List<AssetBundleInfo> assetBundleInfos = new List<AssetBundleInfo>();
        private readonly Dictionary<AssetBundleType, AssetBundleInfo> assetBundleDict = new Dictionary<AssetBundleType, AssetBundleInfo>();

        public void InitAssetBundleInfos()
        {
            string abPath;
            if (Global.I.Mode == AssetBundleMode.StreamingAssetsPath)
                abPath = Application.streamingAssetsPath + "/";
            else if (Global.I.Mode == AssetBundleMode.HFSPath)
                abPath = Application.persistentDataPath + "/";
            else
                return;
            foreach (var info in assetBundleInfos)
            {
                assetBundleDict[info.type] = info;
                if (File.Exists(abPath + info.path))
                {
                    if (info.assetBundle != null)
                        info.assetBundle.Unload(true);
                    info.assetBundle = AssetBundle.LoadFromFile(abPath + info.path);
                }
            }
        }

        public T LoadAsset<T>(AssetBundleType type, string assetPath) where T : Object
        {
            T assetObj;
            if (assetBundleDict.TryGetValue(type, out var assetBundleInfo))
            {
                if (assetBundleInfo.assetBundle != null)
                {
                    assetObj = assetBundleInfo.assetBundle.LoadAsset<T>(assetPath);
                    if(assetObj != null)
                        return assetObj;
                }
            }
#if UNITY_EDITOR
            assetObj = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(assetPath);
            if (assetObj != null)
                return assetObj;
#endif
            throw new Exception("�Ҳ�����Դ:" + assetPath);
        }

        /// <summary>
        /// ������Դ������������Դ���в��ҳ��Լ�����Դ�� ����ɹ���ֱ�ӷ���
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assetPath"></param>
        /// <returns></returns>
        public T LoadAssetWithAll<T>(string assetPath) where T : Object
        {
            foreach (var assetBundleInfo in assetBundleInfos)
            {
                if (assetBundleInfo.assetBundle != null)
                {
                    var assetObj = assetBundleInfo.assetBundle.LoadAsset<T>(assetPath);
                    if (assetObj != null)
                        return assetObj;
                }
                if (assetPath.Contains("Resources/"))
                {
                    var path = assetPath.Split(new string[] { "Resources/" }, 0);
                    var resPath = path[1].Split('.');
                    var resObj = Resources.Load<T>(resPath[0]);
                    if (resObj != null)
                        return resObj;
                }
            }
            throw new Exception("�Ҳ�����Դ:" + assetPath);
        }

        public GameObject Instantiate(string assetPath, Transform parent = null)
        {
            return Instantiate(AssetBundleType.All, assetPath, parent);
        }

        public GameObject Instantiate(AssetBundleType type, string assetPath, Transform parent = null)
        {
            return Instantiate<GameObject>(type, assetPath, parent);
        }

        public T Instantiate<T>(string assetPath, Transform parent = null) where T : Object
        {
            return Instantiate<T>(AssetBundleType.All, assetPath, parent);
        }

        public T Instantiate<T>(AssetBundleType type, string assetPath, Transform parent = null) where T : Object
        {
            var assetObj = LoadAsset<GameObject>(type, assetPath);
            if (assetObj == null)
            {
                Global.Logger.LogError($"��Դ����ʧ��:{assetPath}");
                return null;
            }
            var obj = Instantiate(assetObj, parent);
            if (typeof(T).IsSubclassOf(typeof(Component)))
                return obj.GetComponent<T>();
            return obj as T;
        }
    }
}
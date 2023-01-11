/****************************************************
    文件：AssetBundleManager.cs
	作者：昔莘
    邮箱: 304183153@qq.com
    日期：2021/8/14 16:43:0
	功能：AB包加载管理模块，包含资源池，比较常用，所以作为服务层单例
*****************************************************/

using UnityEngine;
using GF.Utils;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using GF.Pool;
namespace GF.Unity.AB {
    public class AssetBundleManager : Singleton<AssetBundleManager> {
        /// <summary>
        /// key为Crc，value为数据元素
        /// </summary>
        protected Dictionary<uint, ResourceItem> m_ResourceItemDic = new Dictionary<uint, ResourceItem>();
        public string ABPathDic = Application.streamingAssetsPath;
        /// <summary>
        /// 加载过得assetbundle字典，用来判断是否重复加载，先定500个，不够了再调整
        /// </summary>
        protected Dictionary<uint, AssetBundleItem> m_AssetBundleItemDic = new Dictionary<uint, AssetBundleItem>();
        /// <summary>
        /// 已经加载的ab包对象池
        /// </summary>
        protected ClassObjectPool<AssetBundleItem> ABItemPool = ObjectManager.GetInstance.GetClassObjectPool<AssetBundleItem>(500);
        /// <summary>
        /// 加载ab包的配置文件
        /// </summary>
        /// <param name="loadpath">加载路径，一般为Appconfig.ABConfigPath</param>
        /// <returns></returns>
        public bool LoadAssetBundleConfig(string loadpath) {
            Debuger.Log($"Res资源前置路径默认为：{ResourceManager.GetInstance.ResPrePath},如需变更，请调用ResourceManager.GetInstance.ResPrePath修改");
            Debuger.Log($"AB包加载路径默认为：{ABPathDic},如需变更，请调用AssetBundleManager.GetInstance.ABPathDic修改");
            //如果是编辑器下，不需要加载assetbundle数据
            if (!ResourceManager.GetInstance.m_LoadFormAssetBundle) {
                return false;
            }
            UnityEngine.AssetBundle data = UnityEngine.AssetBundle.LoadFromFile(loadpath);
            TextAsset text = data.LoadAsset<TextAsset>("AssetBundleConfig");
            if (text == null) {
                Debuger.LogError("AB包配置文件加载失败！配置文件可能被删除，请联系开发人员！");
                return false;
            }
            MemoryStream mstream = new MemoryStream(text.bytes);
            BinaryFormatter bf = new BinaryFormatter();
            AssetBundleConfig abconfig = (AssetBundleConfig)bf.Deserialize(mstream);
            bf = null;
            mstream.Close();
            for (int i = 0; i < abconfig.ABList.Count; i++) {
                ResourceItem item = new ResourceItem();
                ABBase tmpbase = abconfig.ABList[i];
                item.m_Crc = tmpbase.Crc;
                item.m_ABName = tmpbase.ABname;
                item.m_ALLDepends = tmpbase.ABDepends;
                item.m_AssetName = tmpbase.AssetName;
                if (m_ResourceItemDic.ContainsKey(item.m_Crc)) {
                    Debuger.LogError($"发现重复Crc，值为：{item.m_Crc},请检查配置文件");
                }
                m_ResourceItemDic.Add(item.m_Crc, item);
            }
            data.Unload(true);
            return true;
        }
        /// <summary>
        /// 获取数据块，如果数据块还没加载ab包，就加载上
        /// </summary>
        /// <param name="Crc">数据块的crc</param>
        /// <returns>数据块</returns>
        public ResourceItem GetResourceItem(uint Crc) {
            ResourceItem item = null;
            if (!m_ResourceItemDic.TryGetValue(Crc, out item) || item == null) {
                Debuger.LogError($"加载AB包错误，CRC:{Crc}");
                return null;
            }
            if (item.m_LoadedAB != null) {
                return item;
            }
            item.m_LoadedAB = GetAssetBundle(item.m_ABName);
            //加载依赖包的处理
            for (int i = 0; i < item.m_ALLDepends.Count; i++) {
                ////循环判断现有数据元中是否已经加载过依赖包了
                //foreach (uint crc in m_ResourceItemDic.Keys) {
                //    if (m_ResourceItemDic[crc].m_ABName == item.m_ALLDepends[i]&& m_ResourceItemDic[crc].m_LoadedAB==null) {
                //        GetAssetBundle(item.m_ALLDepends[i]);//加载依赖
                //        break;//只要有一个匹配上跳出循环
                //    }
                //}
                //通过字典存储判断是否重复加载，没必要循环判断了
                GetAssetBundle(item.m_ALLDepends[i]);//加载依赖
            }
            return item;

        }
        /// <summary>
        /// 根据包名的crc存储字典获取AB包，字典没有就重新加载，用到了类对象池
        /// </summary>
        /// <param name="abname">包名</param>
        /// <returns>ab包</returns>
        public UnityEngine.AssetBundle GetAssetBundle(string abname) {
            AssetBundleItem abitem = null;
            uint abnamecrc = Crc32.GetCRC32(abname);//key值
            //通过字典存储判断，是否重复加载
            if (!m_AssetBundleItemDic.TryGetValue(abnamecrc, out abitem)) {
                string path = ABPathDic + "/" + abname;
                UnityEngine.AssetBundle ab = null;
                //if (File.Exists(path)) { 安卓平台下，path（path包含streamingasset路径）不允许访问，所以File.Exists(path)会永远进不去，这里直接去掉
                ab = UnityEngine.AssetBundle.LoadFromFile(path);
                //}
                //此处可以通过elseif来实现多路径加载ab包，项目比较大的时候可以改造，传参多路径数组
                if (ab == null) {
                    Debuger.LogError($"ab包加载错误，路径为：{path}");
                }
                abitem = ABItemPool.GetObj(true);
                abitem.AB = ab;
                m_AssetBundleItemDic.Add(abnamecrc, abitem);
            }
            abitem.RefCount++;
            return abitem.AB;
        }
        /// <summary>
        /// 获取资源块，这里与GetResourceItem方法区分开，GetResourceItem是获取并加载ab包，这里单独就是获取
        /// </summary>
        /// <param name="crc"></param>
        /// <returns></returns>
        public ResourceItem FindResourceItem(uint crc) {
            ResourceItem item = null;
            m_ResourceItemDic.TryGetValue(crc, out item);
            return item;
        }
        /// <summary>
        /// 卸载资源块
        /// </summary>
        /// <param name="item"></param>
        public void ReleaseAsset(ResourceItem item) {
            if (item == null) {
                return;
            }
            if (ResourceManager.GetInstance.m_LoadFormAssetBundle) {
                if (item.m_ALLDepends != null && item.m_ALLDepends.Count > 0) {
                    for (int i = 0; i < item.m_ALLDepends.Count; i++) {
                        UnloadAssetBundle(item.m_ALLDepends[i]);
                    }
                }
                UnloadAssetBundle(item.m_ABName);
            } else {

                //注意，Resources.UnloadAsset 可以正常卸载audio/mesh / texture / material / shader等，但唯独不能卸载GameObject，如果要卸载掉GameObject的话，使用Resources.UnloadUnusedAssets():
                //Resources.UnloadAsset(item.m_Obj);
                Resources.UnloadUnusedAssets();

            }
        }
        /// <summary>
        /// 进行资源释放
        /// </summary>
        /// <param name="abname"></param>
        public void UnloadAssetBundle(string abname) {
            AssetBundleItem abitem = null;
            uint abnamecrc = Crc32.GetCRC32(abname);
            if (m_AssetBundleItemDic.TryGetValue(abnamecrc, out abitem)&& abitem!=null) {
                abitem.RefCount--;
                if (abitem.RefCount <= 0 && abitem.AB != null && ResourceManager.GetInstance.m_LoadFormAssetBundle) {
                    abitem.AB.Unload(true);
                    abitem.Reset();
                    ABItemPool.Recycle(abitem);
                    m_AssetBundleItemDic.Remove(abnamecrc);
                } 
            }
        }
        /// <summary>
        /// 清除所有资源
        /// </summary>
        public void RealseAll() {
            foreach (uint crc in m_ResourceItemDic.Keys) {
                ReleaseAsset(m_ResourceItemDic[crc]);
            }
            m_ResourceItemDic.Clear();
            m_ResourceItemDic = null;
        }
    }
    /// <summary>
    /// 基础（静态）资源块
    /// </summary>
    public class ResourceItem {
        //资源路径的crc
        public uint m_Crc = 0;
        //资源名
        public string m_AssetName = string.Empty;
        //该资源所在assetBundle包
        public string m_ABName = string.Empty;
        //该资源所依赖的AssetBundle
        public List<string> m_ALLDepends = null;
        //该资源加载完的AB包
        public UnityEngine.AssetBundle m_LoadedAB = null;
        //-----------------------以下内容是针对资源的------------------
        //加载出来的资源对象
        public Object m_Obj = null;
        //资源最后使用的时间
        public float m_LastUseTime = 0.0f;
        //引用次数
        private int m_RefCount = 0;
        //资源唯一标识
        public int m_Guid = 0;
        //切换场景时是否清除资源
        public bool m_Clear = true;
        public int RefCount {
            get {
                return m_RefCount;
            }
            set {
                if (value >= 0) {
                    m_RefCount = value;
                } else {
                    m_RefCount = 0;
                    Debuger.LogError($"资源{m_AssetName}引用次数小于0，请检查");
                }
            }
        }
    }
    /// <summary>
    /// ab包的数据元
    /// </summary>
    public class AssetBundleItem {
        public UnityEngine.AssetBundle AB = null;
        public int RefCount=0;//ab包被引用的次数
        public void Reset() {
            AB = null;
            RefCount = 0;
        }
    }
}

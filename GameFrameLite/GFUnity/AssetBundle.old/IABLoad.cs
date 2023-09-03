using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
namespace GF.Unity.AssetBundle {
    /// <summary>
    /// 单个包的加载，assetbundle处理的倒数第二层
    /// </summary>
    public delegate void LoaderProgress(string bundle, float progress);//每一帧加载的进度的委托
    public delegate void LoadFinsh(string bundle);
    public class IABLoad {
        private string bundlename;
        private string bundlepath;
        private UnityWebRequest bundleloader;
        private LoaderProgress loaderp;//进度加载的委托，每桢调用
        private LoadFinsh loadfinsh;//加载完成的委托
                                    //下层对象 - 包内单个资源加载类
        private IABResLoad iabres;
        //加载进度
        private float loadprogress;
        //构造
        public IABLoad(LoaderProgress loaderp, LoadFinsh loadfinsh) {
            bundlename = "";
            bundlepath = "";
            loadprogress = 0;
            this.loadfinsh = loadfinsh;
            this.loaderp = loaderp;
        }
        //设置bundle名称
        public void SetBundleName(string name) {
            this.bundlename = name;
        }
        /// <summary>
        /// 初始化路径
        /// </summary>
        public void LoadPath(string path) {
            bundlepath = path;
        }
        //www协程加载包
        public IEnumerator WWWLoad() {
            bundleloader = new UnityWebRequest(bundlepath);
            while (!bundleloader.isDone) {
                loadprogress = bundleloader.downloadProgress;
                if (loaderp != null) {
                    loaderp(bundlename, loadprogress);
                }
                yield return bundleloader.downloadProgress;
                loadprogress = bundleloader.downloadProgress;
            }
            if (loadprogress >= 1.0f) {
                if (loaderp != null) {
                    loaderp(bundlename, loadprogress);
                }
                if (loadfinsh != null) {
                    loadfinsh(bundlename);
                }
                byte[] results = bundleloader.downloadHandler.data;
                UnityEngine.AssetBundle ab = UnityEngine.AssetBundle.LoadFromMemory(results);
                iabres = new IABResLoad(ab);
            } else {
                Debug.Log("加载失败！");
            }
            bundleloader = null;
        }
        //debug.log选项
        public void DebugAsset() {
            if (iabres != null) {
                iabres.DebugAllRes();
            }
        }
        #region 调用下层方法，为上层提供操作方法,从最底层一层层将方法传递过来
        //通过索引器加载单个资源
        public Object GetSingleRes(string name) {
            if (iabres != null) {
                return iabres[name];
            }
            return null;
        }
        //加载多个资源
        public Object[] GetMutiRes(string name) {
            if (iabres != null) {
                return iabres.LoadResources(name);
            }
            return null;
        }
        //释放资源
        public void Dispose() {
            if (iabres != null) {
                iabres.Dispose();
                iabres = null;
            }
        }
        //单个资源卸载方法
        public void UnLoadAssetRes(UnityEngine.Object tmp) {
            if (iabres != null) {
                iabres.UnloadRes(tmp);
            }
        }
        #endregion
    }
}



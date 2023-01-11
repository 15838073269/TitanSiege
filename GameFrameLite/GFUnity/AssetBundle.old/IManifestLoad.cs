using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
namespace GF.Unity.AssetBundle {
    public class IManifestLoad {
        //单例
        private static IManifestLoad _instance;
        public static IManifestLoad GetInstance {
            get {
                if (_instance == null) {
                    _instance = new IManifestLoad();
                }
                return _instance;
            }
        }

        private IManifestLoad() {
            assetManifest = null;
            ManifestloadAB = null;
            isfinsh = false;
            manifestPath = IPathTools.GetAssetBundlePath();
        }
        //manifest所在的bundle,manifest本质上是一个assetbundle
        public UnityEngine.AssetBundle ManifestloadAB;
        //manifset文件引用
        public AssetBundleManifest assetManifest;
        //manifset文件路径
        public string manifestPath;
        public void SetPath(string path) {//设置路径，不清楚这里的用意，明明是公共的再写方法完全多此一举
            manifestPath = path;
        }
        //是否加载完成
        private bool isfinsh;
        public bool IsFinsh() {
            return isfinsh;
        }
        //加载manifest
        public IEnumerator WWWMainfest() {
            UnityWebRequest manifest = new UnityWebRequest(manifestPath);
            yield return manifest;
            if (!string.IsNullOrEmpty(manifest.error)) {
                Debug.Log(manifest.error);
            } else {//manifest本质上是一个assetbundle
                if (manifest.downloadProgress >= 1.0f) {
                    ManifestloadAB = UnityEngine.AssetBundle.LoadFromMemory(manifest.downloadHandler.data);
                    assetManifest = ManifestloadAB.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
                    isfinsh = true;
                }
            }
        }
        /// <summary>
        /// 为上层提供某个包的依赖关系
        /// </summary>
        /// <param name="bundlename"></param>
        /// <returns></returns>
        public string[] GetDepences(string bundlename) {
            if (assetManifest != null) {
                return assetManifest.GetAllDependencies(bundlename);
            }
            return null;
        }
        /// <summary>
        /// 给上层使用的释放manifest
        /// </summary>
        public void UnLoadManifest() {
            if (ManifestloadAB != null) {
                ManifestloadAB.Unload(true);
            }
        }
    }
}

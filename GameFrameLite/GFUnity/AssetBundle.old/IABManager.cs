using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace GF.Unity.AssetBundle {
    /// <summary>
    /// 对1个场景内所有包的管理
    /// </summary>
    //加载bundle完成后的处理委托
    public delegate void LoadAssetBundleCallBack(string scenename, string bundlename);
    public class IABManager {
        //场景名称,由上层初始化时传递
        string scenename;
        public IABManager(string scenename) {
            this.scenename = scenename;
        }
        //把一个场景内的所有包都存起来
        Dictionary<string, IABRelationManager> SceneAllBundles = new Dictionary<string, IABRelationManager>();
        //把已经load过的object缓存下来
        Dictionary<string, AssetBundleResObj> SceneAllAssets = new Dictionary<string, AssetBundleResObj>();

        //对外的接口，不让上层直接调用协程
        public void LoadAssetBundle(string bundlename, LoaderProgress progress, LoadAssetBundleCallBack callback) {
            if (!SceneAllBundles.ContainsKey(bundlename)) {
                IABRelationManager loader = new IABRelationManager();
                loader.Initial(bundlename, progress);
                SceneAllBundles.Add(bundlename, loader);
                callback(scenename, bundlename);//上层在callback中开启下载资源的协程
            } else {
                Debug.Log("已经存在这个包了，无法重复加载" + bundlename);
            }
        }
        //获取依赖
        string[] GetDependences(string bundlename) {
            return IManifestLoad.GetInstance.GetDepences(bundlename);
        }

        /// <summary>
        /// 加载AssetBundle,先加载manifest，上层在callback中开启下载资源的协程
        /// </summary>
        /// <param name="bundlename"></param>
        /// <returns></returns>
        public IEnumerator LoadBundles(string bundlename) {
            //等待manifest
            while (!IManifestLoad.GetInstance.IsFinsh()) {
                yield return null;
            }
            //创建关系对象
            IABRelationManager loader = SceneAllBundles[bundlename];
            string[] dependences = GetDependences(bundlename);//获取依赖
                                                              //往关系对象中写入依赖
            loader.AddDepend(dependences);
            //循环递归加载依赖包
            for (int i = 0; i < dependences.Length; i++) {
                yield return LoadAssetBundleDepends(dependences[i], bundlename, loader.GetProgresse());
            }
            //所有依赖包加载完成后，加载本包
            yield return loader.LoadAssetBundle();
        }
        /// <summary>
        /// 加载依赖的assetbundle
        /// </summary>
        /// <param name="bundlename">依赖包体名称</param>
        /// <param name="refname">本包体的名称，添加到依赖包体的被依赖list中，方便释放</param>
        /// <param name="progress"></param>
        /// <returns></returns>
        public IEnumerator LoadAssetBundleDepends(string bundlename, string refname, LoaderProgress progress) {
            if (!SceneAllBundles.ContainsKey(bundlename)) {//如果依赖包没有加载，递归执行加载包的方法
                IABRelationManager loader = new IABRelationManager();
                loader.Initial(bundlename, progress);
                if (refname != null) {
                    loader.AddRefer(refname);
                }
                SceneAllBundles.Add(bundlename, loader);
                yield return LoadBundles(bundlename);
            } else {//如果依赖包已经加载过了，那就只执行添加被依赖关系
                if (refname != null) {
                    IABRelationManager loader = SceneAllBundles[bundlename];
                    loader.AddRefer(refname);
                }
            }
        }
        #region 调用下层方法，为上层提供操作方法,从最底层一层层将方法传递过来
        /// <summary>
        /// 遍历单个包体的资源
        /// </summary>
        /// <param name="bundlename">包体的名称</param>
        public void DebugAsset(string bundlename) {
            if (SceneAllBundles.ContainsKey(bundlename)) {
                IABRelationManager loader = SceneAllBundles[bundlename];
                loader.DebugAsset();
            }
        }
        /// <summary>
        /// 检查包体是否加载完
        /// </summary>
        /// <param name="bundlename"></param>
        /// <returns></returns>
        public bool IsLoadFinsh(string bundlename) {
            bool isfinsh = false;
            if (SceneAllBundles.ContainsKey(bundlename)) {
                IABRelationManager loader = SceneAllBundles[bundlename];
                isfinsh = loader.IsBundleLoadFinsh();
            } else {
                Debug.Log("没有这个包");
            }
            return isfinsh;
        }
        /// <summary>
        /// 是否加载过某个包
        /// </summary>
        /// <param name="bundlename"></param>
        /// <returns></returns>
        public bool IsLoaded(string bundlename) {
            if (SceneAllBundles.ContainsKey(bundlename)) {
                return true;
            } else {
                return false;
            }
        }
        /// <summary>
        /// 从包中获取单个资源
        /// </summary>
        /// <param name="bundlename">包名称</param>
        /// <param name="resname">资源名称</param>
        /// <returns></returns>
        public Object GetSignleAsset(string bundlename, string resname) {
            if (SceneAllAssets.ContainsKey(bundlename)) {//先检查缓存中是否存在
                List<Object> tmp = SceneAllAssets[bundlename].GetAssets(resname);
                if (tmp.Count > 0) {//缓存里有就返回
                    return tmp[0];
                }
            }
            //检查包体里有没有，有可能包加载了，但是缓存里还没加载
            if (SceneAllBundles.ContainsKey(bundlename)) {
                Object tmpobj = SceneAllBundles[bundlename].GetSingleRes(resname);
                AssetResObj tmpasset = new AssetResObj(tmpobj);
                if (SceneAllAssets.ContainsKey(bundlename)) {//检查缓存中是不是有这个包
                    SceneAllAssets[bundlename].AddRes(resname, tmpasset);//有这个包直接往包添加资源
                } else {//没有这个包，包装这个资源，写入包里，在把包加到缓存字典中
                    AssetBundleResObj tmpbundle = new AssetBundleResObj(resname, tmpasset);
                    SceneAllAssets.Add(bundlename, tmpbundle);
                }
                return tmpobj;
            } else {
                Debug.Log("没有没有这个包");
                return null;
            }
        }
        /// <summary>
        /// 从包中获取多个资源
        /// </summary>
        /// <param name="bundlename">包名称</param>
        /// <param name="resname">资源名称</param>
        /// <returns></returns>
        public Object[] GetMutiAsset(string bundlename, string resname) {
            if (SceneAllAssets.ContainsKey(bundlename)) {//先检查缓存中是否存在
                List<Object> tmp = SceneAllAssets[bundlename].GetAssets(resname);
                if (tmp.Count > 0) {//缓存里有就返回
                    return tmp.ToArray();
                }
            }
            //检查包体里有没有，有可能包加载了，但是缓存里还没加载
            if (SceneAllBundles.ContainsKey(bundlename)) {
                Object[] tmpobj = SceneAllBundles[bundlename].GetMutiRes(resname);
                AssetResObj tmpasset = new AssetResObj(tmpobj);
                if (SceneAllAssets.ContainsKey(bundlename)) {//检查缓存中是不是有这个包
                    SceneAllAssets[bundlename].AddRes(resname, tmpasset);//有这个包直接往包添加资源
                } else {//没有这个包，包装这个资源，写入包里，在把包加到缓存字典中
                    AssetBundleResObj tmpbundle = new AssetBundleResObj(resname, tmpasset);
                    SceneAllAssets.Add(bundlename, tmpbundle);
                }
                return tmpobj;
            } else {
                Debug.Log("没有没有这个包");
                return null;
            }
        }
        //释放单个资源
        public void DisposeRes(string bundlename, string resname) {
            if (SceneAllAssets.ContainsKey(bundlename)) {//先检查缓存中是否存在
                SceneAllAssets[bundlename].UnResObj(resname);
            }
        }
        //释放单个包的资源
        public void DisposeBundleRes(string bundlename) {
            if (SceneAllAssets.ContainsKey(bundlename)) {//先检查缓存中是否存在
                SceneAllAssets[bundlename].UnAllResObj();
            }
            Resources.UnloadUnusedAssets();
        }
        //释放所有的包的资源
        public void DisPoseAllBundleRes() {
            foreach (KeyValuePair<string, AssetBundleResObj> all in SceneAllAssets) {
                DisposeBundleRes(all.Key);
            }
            SceneAllAssets.Clear();
        }
        //释放一个包
        public void DisposeBundle(string bundlename) {
            if (SceneAllBundles.ContainsKey(bundlename)) {
                IABRelationManager loader = SceneAllBundles[bundlename];
                List<string> depends = loader.GetDepend();
                for (int i = 0; i < depends.Count; i++) {
                    if (SceneAllBundles.ContainsKey(depends[i])) {
                        IABRelationManager dependloader = SceneAllBundles[depends[i]];
                        if (dependloader.RemoveRefer(bundlename)) {
                            DisposeBundle(dependloader.GetBundlename());
                        }
                    }
                }
                if (loader.GetDepend().Count <= 0) {
                    loader.Dispos();
                    SceneAllBundles.Remove(bundlename);
                }
            }
        }
        //释放所有包
        public void DisposeAllBundle() {
            DisPoseAllBundleRes();
        }
        //释放所有包和包的资源
        public void DisposeAllBundleAndRes() {
            DisPoseAllBundleRes();
            foreach (KeyValuePair<string, IABRelationManager> s in SceneAllBundles) {
                s.Value.Dispos();
            }
        }
        #endregion
    }

    //把已经load过的多个object缓存下来所用到的类结构，以每个资源名称为键，存储一个包的资源
    public class AssetBundleResObj {
        public Dictionary<string, AssetResObj> objdic;
        public AssetBundleResObj(string name, AssetResObj tmpobj) {
            objdic = new Dictionary<string, AssetResObj>();
            objdic.Add(name, tmpobj);
        }
        //添加
        public void AddRes(string name, AssetResObj tmpobj) {
            objdic.Add(name, tmpobj);
        }
        //释放整个
        public void UnAllResObj() {
            foreach (KeyValuePair<string, AssetResObj> o in objdic) {
                o.Value.UnResObj();
            }
        }
        //释放单个
        public void UnResObj(string name) {
            if (objdic.ContainsKey(name)) {
                objdic[name].UnResObj();
            }
        }
        //获取资源
        public List<Object> GetAssets(string resname) {
            if (objdic.ContainsKey(resname)) {
                return objdic[resname].objs;
            }
            return null;
        }
    }
    //把已经load过的单个object缓存下来所用到的类结构，上述类用到每个资源的类
    public class AssetResObj {
        public List<Object> objs;
        public AssetResObj(params Object[] tmpobj) {
            objs = new List<Object>();
            objs.AddRange(tmpobj);
        }
        public void UnResObj() {
            for (int i = 0; i < objs.Count; i++) {
                Resources.UnloadAsset(objs[i]);
            }
        }
    }
}


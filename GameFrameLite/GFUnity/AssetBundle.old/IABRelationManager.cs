using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace GF.Unity.AssetBundle {
    /// <summary>
    /// 包的依赖关系管理类，assetbundle处理的倒数第三层
    /// </summary>
    public class IABRelationManager {

        //本包的依赖包depend
        List<string> dependlist;
        //本包的被依赖关系 refer  谁依赖了本包
        List<string> referlist;
        //下层--单个包加载类
        IABLoad BundleLoader;
        //加载的bundle名称
        string bundlename;
        public string GetBundlename() {
            return bundlename;
        }
        //加载单个包的进度委托
        LoaderProgress progress;
        public LoaderProgress GetProgresse() {
            return progress;
        }
        //构造函数
        public IABRelationManager() {
            dependlist = new List<string>();
            referlist = new List<string>();
        }
        //添加被引用关系
        public void AddRefer(string bundlename) {
            referlist.Add(bundlename);
        }
        //获取被引用关系list
        public List<string> GetRefer() {
            return referlist;
        }
        //删除被引用关系
        /// <summary>
        /// 删除被引用关系
        /// </summary>
        /// <param name="bunldename">包名</param>
        /// <returns>是否释放了自己</returns>
        public bool RemoveRefer(string bunldename) {
            if (referlist.Contains(bunldename)) {
                referlist.Remove(bunldename);
            }
            //如果没有任何包需要本包，就将包释放
            if (referlist.Count == 0) {
                Dispos();
                return true;
            }
            return false;
        }
        //依赖关系depend
        public void AddDepend(string[] bundlename) {
            if (dependlist.Count > 0) {
                dependlist.AddRange(bundlename);
            }
        }
        //获取依赖关系list
        public List<string> GetDepend() {
            return dependlist;
        }
        //删除依赖关系
        public bool RemoveDepend(string bunldename) {
            if (dependlist.Contains(bunldename)) {
                dependlist.Remove(bunldename);
            }
            //如果没有任何包需要本包，就将包释放
            if (dependlist.Count == 0) {
                Dispos();
                return true;
            }
            return false;
        }
        //depend初始化 上层需要使用
        public void Initial(string name, LoaderProgress progress) {
            IsLoadFinsh = false;
            bundlename = name;
            this.progress = progress;
            BundleLoader = new IABLoad(progress, BundleLoadFinsh);
        }
        //是否加载完成的标志
        bool IsLoadFinsh = false;
        //为上层提供加载完毕的标志
        public bool IsBundleLoadFinsh() {
            return IsLoadFinsh;
        }
        //单个包加载完毕后的处理方法
        public void BundleLoadFinsh(string bundlename) {
            IsLoadFinsh = true;
        }
        #region 调用下层方法，为上层提供操作方法,从最底层一层层将方法传递过来
        //释放资源的方法
        public void Dispos() {
            BundleLoader.Dispose();
        }
        //通过索引器加载单个资源
        public Object GetSingleRes(string name) {
            if (BundleLoader != null) {
                return BundleLoader.GetSingleRes(name);
            }
            return null;
        }
        //加载多个资源
        public Object[] GetMutiRes(string name) {
            if (BundleLoader != null) {
                return BundleLoader.GetMutiRes(name);
            }
            return null;
        }
        /// <summary>
        /// 多层传递加载，unity5.3之后才能用
        /// </summary>
        /// <returns></returns>
        public IEnumerator LoadAssetBundle() {
            yield return BundleLoader.WWWLoad();
        }
        //debug
        public void DebugAsset() {
            if (BundleLoader != null) {
                BundleLoader.DebugAsset();
            } else {
                Debug.Log("没有这个资源包");
            }
        }
        #endregion
    }
}

using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
namespace GF.Unity.AssetBundle {
    public class IABSceneManager {
        IABManager abmanager;
        //构造
        public IABSceneManager(string scenename) {
            abmanager = new IABManager(scenename);
        }
        //读取text文档存储所有包的对应关系
        private Dictionary<string, string> AllBundleDic = new Dictionary<string, string>();
        //读取配置文件
        public void ReadConfiger(string scenename) {
            string txtpath = IPathTools.GetAssetBundlePath() + "/" + scenename + "/" + scenename + "AssetBundleRecord.txt";
            string[] strs = File.ReadAllLines(txtpath);
            for (int i = 0; i < strs.Length; i++) {
                string[] lingarr = strs[i].Split('-');
                if (lingarr.Length > 1 && !AllBundleDic.ContainsKey(lingarr[0])) {
                    AllBundleDic.Add(lingarr[0], lingarr[1]);
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bundlename"></param>
        /// <param name="progress"></param>
        /// <param name="callback"></param>
        public void LoadAsset(string bundlename, LoaderProgress progress, LoadAssetBundleCallBack callback) {
            if (AllBundleDic.ContainsKey(bundlename)) {
                string tmpValue = AllBundleDic[bundlename];
                abmanager.LoadAssetBundle(tmpValue, progress, callback);
            } else {
                Debug.Log("没有这个assetbundle包" + bundlename);
            }
        }
        #region 由下层提供功能
        public IEnumerator LoadAssetSys(string bundlename) {
            yield return abmanager.LoadBundles(bundlename);
        }
        public Object GetSingleRes(string bundlename, string resname) {
            if (AllBundleDic.ContainsKey(bundlename)) {
                return abmanager.GetSignleAsset(AllBundleDic[bundlename], resname);
            } else {
                Debug.Log("没有这个包" + bundlename);
                return null;
            }
        }
        public Object[] GetMutiRes(string bundlename, string resname) {
            if (AllBundleDic.ContainsKey(bundlename)) {
                return abmanager.GetMutiAsset(AllBundleDic[bundlename], resname);
            } else {
                Debug.Log("没有这个包" + bundlename);
                return null;
            }
        }
        public void DisposeRes(string bundlename, string resname) {
            if (AllBundleDic.ContainsKey(bundlename)) {
                abmanager.DisposeRes(AllBundleDic[bundlename], resname);
            } else {
                Debug.Log("没有这个包" + bundlename);
            }
        }
        public void DisposeBundleRes(string bundlename) {
            if (AllBundleDic.ContainsKey(bundlename)) {
                abmanager.DisposeBundleRes(AllBundleDic[bundlename]);
            } else {
                Debug.Log("没有这个包" + bundlename);
            }
        }

        public void DisposeAllBundleRes() {
            abmanager.DisPoseAllBundleRes();
        }

        public void DisposeBundle(string bundlename) {
            if (AllBundleDic.ContainsKey(bundlename)) {
                abmanager.DisposeBundle(AllBundleDic[bundlename]);
            } else {
                Debug.Log("没有这个包" + bundlename);
            }

        }

        public void DisposeAllBundle() {
            abmanager.DisposeAllBundle();
            AllBundleDic.Clear();
        }

        public void DisposeAllBundleAndRes() {
            abmanager.DisposeAllBundleAndRes();
            AllBundleDic.Clear();
        }
        public void DebugAllAsset() {
            foreach (KeyValuePair<string, string> a in AllBundleDic) {
                abmanager.DebugAsset(a.Value);
            }

        }
        #endregion
    }
}

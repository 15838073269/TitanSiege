using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using GF;
namespace GF.Unity.AssetBundle {
    /// <summary>
    /// 单个资源的加载，继承IDisposable，系统在执行完成后会自动执行销毁Dispos，assetbundle处理的最底层
    /// </summary>
    public class IABResLoad : IDisposable {
        //私有
        private UnityEngine.AssetBundle ABRes;
        //构造，初始化私有字段
        public IABResLoad(UnityEngine.AssetBundle tmp) {
            ABRes = tmp;
        }
        //索引器，方便加载包体内的资源
        public UnityEngine.Object this[string resname] {
            get {
                if (this.ABRes == null | !this.ABRes.Contains(resname)) {
                    Debuger.Log("不存在这个" + resname + "资源");
                    return null;
                }
                return ABRes.LoadAsset(resname);
            }
        }


        /// <summary>
        /// 一次加载多个资源，例如图集类型的资源，里面包含多个对象
        /// </summary>
        /// <param name="resname"></param>
        /// <returns></returns>
        public UnityEngine.Object[] LoadResources(string resname) {
            if (this.ABRes == null | !this.ABRes.Contains(resname)) {
                Debug.Log("不存在这个" + resname + "资源");
                return null;
            }
            //该api主要用来加载UI的图集
            return ABRes.LoadAssetWithSubAssets(resname);
        }

        public void UnloadRes(UnityEngine.Object res) {
            //销毁加载出来的资源
            Resources.UnloadAsset(res);
        }
        /// <summary>
        /// 释放assetbundle包
        /// </summary>
        public void Dispose() {
            //释放内存中的asetbundle包，不处理加载出来的上层资源
            ABRes.Unload(false);
        }
        public void DebugAllRes() {
            string[] allname = ABRes.GetAllAssetNames();
            for (int i = 0; i < allname.Length; i++) {
                Debug.Log(allname[i]);
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GF.Utils;
namespace GF.Unity.AssetBundle { 
public class LoaderManager : MonoBehaviour {
    //单例
    private static LoaderManager _instance;
    public static LoaderManager GetInstance {
        get {
            return _instance;
        }
    }

    private void Awake() {
        _instance = this;
        //加载IManifest
        StartCoroutine(IManifestLoad.GetInstance.WWWMainfest());
    }
    IABSceneManager scenemanager;
    //scenename   manager  
    private Dictionary<string, IABSceneManager> scenedic = new Dictionary<string, IABSceneManager>();
    //读取配置文件
    public void ReadConfig(string scenename) {
        if (!scenedic.ContainsKey(scenename)) {
            IABSceneManager tmpmanager = new IABSceneManager(scenename);
            tmpmanager.ReadConfiger(scenename);
            scenedic.Add(scenename, tmpmanager);
        }
    }
    //加载资源
    public void LoadAsset(string scenename,string bundlename,LoaderProgress progress,LoadAssetBundleCallBack callback) {
        if (!scenedic.ContainsKey(scenename)) {
            ReadConfig(scenename);
        }
        IABSceneManager tmpmanager = scenedic[scenename];
        tmpmanager.LoadAsset(bundlename,progress, callback);
    }
    //回调
    public void LoadCallBack(string scenename,string bundlename) {
        if (!scenedic.ContainsKey(scenename)) {
            Debug.Log("没有这个包"+bundlename);
        } else {
            IABSceneManager tmpmanager = scenedic[scenename];
            StartCoroutine(tmpmanager.LoadAssetSys(bundlename));
        }
    }
    #region 由下层提供
    public Object GetSingleRes(string scenename,string bundlename, string resname) {
        if (scenedic.ContainsKey(scenename)) {
            IABSceneManager scenemanager = scenedic[scenename];
            return scenemanager.GetSingleRes(bundlename, resname);
        } else {
            Debug.Log(scenename+"没有这个包" + bundlename);
            return null;
        }
    }
    public Object[] GetMutiRes(string scenename, string bundlename, string resname) {
        if (scenedic.ContainsKey(scenename)) {
            IABSceneManager scenemanager = scenedic[scenename];
            return scenemanager.GetMutiRes(bundlename, resname);
        } else {
            Debug.Log(scenename+"没有这个包" + bundlename);
            return null;
        }
    }
    public void DisposeRes(string scenename, string bundlename, string resname) {
        if (scenedic.ContainsKey(scenename)) {
            IABSceneManager scenemanager = scenedic[scenename];
            scenemanager.DisposeRes(bundlename, resname);
        } else {
            Debug.Log(scenename+"没有这个包" + bundlename);
        }
    }
    public void DisposeBundleRes(string scenename, string bundlename) {
        if (scenedic.ContainsKey(scenename)) {
            IABSceneManager scenemanager = scenedic[scenename];
            scenemanager.DisposeBundleRes(bundlename);
        } else {
            Debug.Log(scenename + "没有这个包" + scenename);
        }
    }
    public void DisposeAllBundleRes(string scenename) {
        if (scenedic.ContainsKey(scenename)) {
            IABSceneManager scenemanager = scenedic[scenename];
            scenemanager.DisposeAllBundleRes();
        }
    }
    public void DisposeBundle(string scenename, string bundlename) {
        if (scenedic.ContainsKey(scenename)) {
            IABSceneManager scenemanager = scenedic[scenename];
            scenemanager.DisposeBundle(bundlename);
        } else {
            Debug.Log(scenename + "没有这个包" + bundlename);
        }

    }

    public void DisposeAllBundle(string scenename) {
        if (scenedic.ContainsKey(scenename)) {
            IABSceneManager scenemanager = scenedic[scenename];
            scenemanager.DisposeAllBundle();
        }
    }

    public void DisposeAllBundleAndRes(string scenename) {
        if (scenedic.ContainsKey(scenename)) {
            IABSceneManager scenemanager = scenedic[scenename];
            scenemanager.DisposeAllBundleAndRes();
            System.GC.Collect();
        }
    }
    public void DebugAllAssetBundle(string scenename) {
        if (scenedic.ContainsKey(scenename)) {
            IABSceneManager scenemanager = scenedic[scenename];
            scenemanager.DebugAllAsset();
        }
    }
    #endregion
    private void OnDestroy() {
        scenedic.Clear();
        System.GC.Collect();
    }
}
}

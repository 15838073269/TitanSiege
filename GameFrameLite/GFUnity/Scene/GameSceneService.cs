/****************************************************
    文件：SceneManager.cs
	作者：昔莘
    邮箱: 304183153@qq.com
    日期：2021/10/8 15:18:33
	功能：场景管理类
*****************************************************/

using GF.Unity.AB;
using GF.Unity.UI;
using GF.Utils;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace GF.Service {
    public class GameSceneService : Singleton<GameSceneService> {
        /// <summary>
        /// 异步加载初始化，主要初始化协程，使用协程，需要使用monobehiour
        /// </summary>
        private MonoBehaviour m_Startmono;
        public GameSceneService() {
           
        }
        public void Init(MonoBehaviour mono,string loadpath) {
            m_Startmono = mono;
            SceneLoadingpath = loadpath;
            //监听场景加载事件,SceneManager.sceneLoaded会在场景加载完成后调用，将SceneLoaded代理（Action）注册给SceneManager.sceneLoaded事件，进行转接。
            //之所以要转接，后面使用时有说明
            //SceneManager.sceneLoaded += (scene, mode) => {
            //    if (SceneLoadedOver != null) {
            //        SceneLoadedOver(scene.name);
            //    }
            //};
        }
        /// <summary>
        /// 场景加载UI预制体的名称
        /// </summary>
        public string EmptyScene = "Empty";
        /// <summary>
        /// 当前打开的场景名称
        /// </summary>
        public string CurrenSceneName { get; set; }
        /// <summary>
        /// 场景切换进度
        /// </summary>
        public static int LoadingProgress = 0;
        /// <summary>
        /// 场景加载成功的事件调用
        /// </summary>
        public Action<object> SceneLoadedOver;
        /// <summary>
        /// 场景加载成功的事件调用
        /// </summary>
        public Action<object> SceneLoadedBegain;
        /// <summary>
        /// 是否加载完成
        /// </summary>
        public bool IsLoadDone = false;
        /// <summary>
        /// 很多3d游戏都需要在场景加载完设置一下参数，例如摄像机位置，光照贴图等，这里留着拓展
        /// </summary>
        /// <param name="scenename"></param>
        void SetSceneSetting(string scenename) { 
            //根据场景名称为key值加载场景的配置表
            //todo
        }
        /// <summary>
        /// 同步场景加载,加载小场景时使用
        /// </summary>
        public void LoadScene(string scene, Action onLoadComplete = null) {
            IsLoadDone = false;
           // UIManager.GetInstance.OpenLoading(SceneLoading);
            SceneManager.LoadScene(scene);
            IsLoadDone = true;
            if (onLoadComplete != null)
                 onLoadComplete();
        }
        /// <summary>
        /// 异步场景加载，会通过加载空场景清理内存，用于复杂场景加载
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="arg">参数类</param>
        public void AsyncLoadSceneWithEmpty(string scene, object arg = null) {
            //UIManager.GetInstance.OpenLoading(SceneLoading);
            LoadingProgress = 0;
            m_Startmono.StartCoroutine(AsyncLoadSceneCor(scene,true,arg));
        }
        /// <summary>
        /// 场景加载的协程
        /// </summary>
        /// <param name="scene">场景名称</param>
        /// <param name="showui">是否显示UI界面，显示就加载标准loading界面，不显示，就只加载场景，由业务层自己处理ui</param>
        /// <param name="withempty">是否加载空场景来清理内存</param>
        /// <param name="arg">参数类</param>
        /// <returns></returns>
        IEnumerator AsyncLoadSceneCor(string scene, bool withempty = false,object arg = null) {
            IsLoadDone = false;
            if (SceneLoadedBegain!=null) {
                SceneLoadedBegain(arg);
            }
            ClearCache();
            if (withempty) {
                //为了避免内存爆掉，先加载一个空场景卸载当前场景全部资源，否则本场景资源还未全部卸载，又加载新场景资源，对内存不友好
                AsyncOperation unloadScene = SceneManager.LoadSceneAsync(EmptyScene, LoadSceneMode.Single);
                while (unloadScene != null && !unloadScene.isDone) {
                    yield return new WaitForEndOfFrame();//等待一帧
                }
            }
            //加载真正的场景
            LoadingProgress = 0;
            int targetProgress = 0;
            AsyncOperation targetScene = SceneManager.LoadSceneAsync(scene);
            if (targetScene!=null&&!targetScene.isDone) {
                targetScene.allowSceneActivation = false;//禁止加载完自动跳转
                while (targetScene.progress<0.9f) {//unity的加载到98%时会卡顿，然后直接完成，体验不好，为了平滑实现进度条，从0.9之后就手动实现平滑
                    targetProgress = (int)targetScene.progress * 100;
                    yield return new  WaitForEndOfFrame();
                    //实现平滑过渡
                    while (LoadingProgress<targetProgress) {
                        ++LoadingProgress;
                        yield return new WaitForEndOfFrame();
                    }
                }
                CurrenSceneName = scene;
                SetSceneSetting(scene);//场景加载超过90%就开始设置参数
                //自行加载剩余10%
                targetProgress = 100;
                while (LoadingProgress < targetProgress) {
                    ++LoadingProgress;
                    yield return new WaitForEndOfFrame();
                }
                targetScene.allowSceneActivation = true;
                IsLoadDone = true;
                if (SceneLoadedOver != null) {
                    SceneLoadedOver(arg);
                }
            }
        }
        /// <summary>
        /// 异步场景加载，不通过加载空场景清理内存，用于场景不复杂的
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="showui">是否显示UI界面，显示就加载标准loading界面，不显示，就只加载场景，由业务层自己处理ui</param>
        /// <param name="arg">参数类</param>
        public string SceneLoadingpath;
        public void AsyncLoadScene(string scene, bool showui = true,object uiarg = null,object arg = null) {
            LoadingProgress = 0;
            if (showui) {
                UIManager.GetInstance.OpenLoading(SceneLoadingpath, uiarg);
            }
            m_Startmono.StartCoroutine(AsyncLoadSceneCor(scene,false, arg));
        }
        /// <summary>
        /// 跳场景清除缓存
        /// </summary>
        private void ClearCache() {
            ObjectManager.GetInstance.ClearCache();
            ResourceManager.GetInstance.ClearCache();
        }

        private void CheckSceneAB(string scenename) { 
            
        }
    }
}

/****************************************************
    文件：AssetBundleConfig.cs
	作者：昔莘
    邮箱: 304183153@qq.com
    日期：2021/8/12 10:54:3
	功能：项目的启动类，初始化debuger，各类模块，网络等，同时系统关闭时，清理所有资源，APPMain一定要最优先启动，记得在Script Execution Order中设置
     INetworkHandle是网络消息接口，如果是网络项目，必须继承网络消息接口，实现对网络消息的处理
*****************************************************/
using UnityEngine;
using GF.Module;
using GF.Timer;
using System;
using GF.Unity.UI;
using GF.Unity.AB;
using GF.Service;
using GF.ConfigTable;
using GF.MainGame.Data;
using GF.MainGame.Module;
using GF.Msg;
using System.Collections.Generic;
using GF.MainGame.Module.NPC;
using System.Threading;
using Net.System;
using Cysharp.Threading.Tasks;
using Net.Client;
using System.Net.Sockets;
using cmd;
using Net.Share;
using Net.Component;
using System.Security.Principal;
using System.Security.Policy;

namespace GF.MainGame {
    public class AppMain : MonoBehaviour, INetworkHandle {
        private static AppMain m_Instance;
        public static AppMain GetInstance { get { return m_Instance; } }
        private void Awake() {
            Application.runInBackground = true;//用于后台挂起
            Screen.sleepTimeout = SleepTimeout.NeverSleep;//用于禁止屏幕休眠
            m_Instance = this;
            DontDestroyOnLoad(this.gameObject);
            //单独在Awake中调用一次，不直接写在Start中，是为了以后拓展，有其他代码需要在start执行时方便排序
            Init();
        }
        /// <summary>
        /// 角色的唯一id，网络游戏开发常用，所以存在这里
        /// </summary>
        public int Uid = 0;
        void Start() {
            //Init();
        }
        void Init() {
            //初始化 时间
            //Time.realtimeSinceStartup 这个变量代表游戏从开始经过的时间，但unity的限制，这个变量不能在子线程中调用，所以需要定义自己的时间类，用来多线程中调用。
            GFTime.DateTimeAppStart = DateTime.Now;
            //初始化debuger
            InitDebuger();
            //开启防作弊
            OpenDeCheat();
            //开启网络监听,处理网络事件
            ClientManager.I.client.BindNetworkHandle(this);
            //初始化AppConfig
            AppConfig.Init();
            MsgCenter.Init(ReturnAllModuleName());

            //初始化版本
            InitVesion();

            //这里的代码是判断在编辑器下，工程是否运行，如果运行就执行委托的函数。
#if UNITY_EDITOR
            //为了防止重复添加，所以先减再加，没有委托时减是无效的。
            UnityEditor.EditorApplication.playModeStateChanged -= OnEditorPlayModeChanged;
            UnityEditor.EditorApplication.playModeStateChanged += OnEditorPlayModeChanged;
#endif
        }
        /// <summary>
        /// 返回所有模块名称，为消息模块使用的
        /// </summary>
        /// <returns></returns>
        public Dictionary<int, string> ReturnAllModuleName() {
            Dictionary<int, string> moddic = new Dictionary<int, string>();
            Array intarray = Enum.GetValues(typeof(MDef));
            for (int i = 0; i < intarray.Length; i++) {
                var m = intarray.GetValue(i);
                moddic.Add((int)m, m.ToString());
            }
            return moddic;
        }

#if UNITY_EDITOR
        /// <summary>
        /// 一般在网络通讯时，涉及到子线程 ，如果运行关闭，需要及时结束子线程，否则会出现问题，这个时候就需要这个函数起作用。
        /// </summary>
        /// <param name="state">编辑器的状态</param>
        private static void OnEditorPlayModeChanged(UnityEditor.PlayModeStateChange state) {
            if (state == UnityEditor.PlayModeStateChange.EnteredPlayMode) {

            } else if (state == UnityEditor.PlayModeStateChange.ExitingPlayMode) {
                //todo 处理子线程相关操作
                UnityEditor.EditorApplication.playModeStateChanged -= OnEditorPlayModeChanged;
                Exit("editor已经关闭 ");
            }
        }
#endif
        /// <summary>
        /// 退出编辑器时的操作，如果运行关闭，需要及时结束子线程，否则会出现问题，这个时候就需要这个函数起作用。
        /// </summary>
        /// <param name="reason">退出的原因</param>
        public static void Exit(string reason) {
            //清理模块管理器
            ModuleManager.GetInstance.ReleaseAll();
            //清理UI管理器
            UIManager.GetInstance.ReleaseAll();
            //清理资源器
            AssetBundleManager.GetInstance.RealseAll();
            ResourceManager.GetInstance.ReleaseAll();
            ObjectManager.GetInstance.RealseAll();

            //清理网络管理器
            //清理热更新管理器
            //清楚版本管理器

            MsgCenter.ClearMsgCeneter();
        }
        /// <summary>
        /// 初始化debuger的函数
        /// </summary>
        private void InitDebuger() {
            Debuger.Init(Application.persistentDataPath + "/Debuger/", new UnityDebugerConsole());
            //设置日志开关
            Debuger.EnableLog = true;//打开日志
            Debuger.Log("日志系统已开启！");
            Debuger.EnableSave = true;
            if (Debuger.EnableSave) {
                Debuger.Log("日志写入本地已开启！");
            } else {
                Debuger.Log("日志写入本地未开启！");
            }
        }
        /// <summary>
        /// 初始化版本的函数
        /// </summary>
        private void InitVesion() {
            //获取版本并更新版本

            //版本更新完成后，初始化服务层模块，例如热更新
            InitServer();
        }
        //需要在面板上挂脚本
        public Transform RecyclePoolTrs = null;
        public Transform SceneTransform = null;
        public Transform HpUIRoot;
        public CameraController MainCamera = null;
        public UIRoot uiroot = null;
        public Camera uicamera = null;
        public GameObject eventsystem = null;
        /// <summary>
        /// 初始化服务层模块
        /// </summary>
        private void InitServer() {
            //初始化热更新
            //初始化模块管理器
            ModuleManager.GetInstance.Init();
            //注册普通模块的创建器，这里的普通模块就是不需要热更新的模块
            ModuleManager.GetInstance.RegisterModuleActivator(new NativeModuleActivator(ModuleDef.NameSpace, ModuleDef.NativeAssemblyName));
            //初始化UI模块
            if ((uiroot == null) || (uicamera == null)) {
                uiroot = GameObject.Find("UIRoot").GetComponent<UIRoot>();
                uicamera = uiroot.FindUI("UICamera").GetComponent<Camera>();
                eventsystem = uiroot.FindUI("EventSystem");
            }

            UIManager.GetInstance.Init(uiroot, uicamera, eventsystem);//UI预制体的路径
            UIManager.MainPage = AppConfig.MainUIPage;
            UIManager.MainScene = AppConfig.MainScene;

            //加载AB包资源配置表
            ObjectManager.GetInstance.IsEditor = AppConfig.IsEditor;
            if (ObjectManager.GetInstance.IsEditor) {
                Debuger.Log("当前是编辑器模式");
            }
            ObjectManager.GetInstance.Init(RecyclePoolTrs, SceneTransform);
            ResourceManager.GetInstance.m_LoadFormAssetBundle = AppConfig.LoadFormAssetBundle;
            ResourceManager.GetInstance.MAXLOADINGTIME = AppConfig.MAXMAXLOADINGTIME;//异步加载时最大卡着连续异步加载的时间，单位是微秒，默认20万微妙
            ResourceManager.GetInstance.MAXMAPLISTSIZE = AppConfig.MAXMAPLISTSIZE;
            ResourceManager.GetInstance.Init(this);//初始化资源异步加载
            ResourceManager.GetInstance.ResPrePath = AppConfig.ResPrePath;
            AssetBundleManager.GetInstance.ABPathDic = AppConfig.ABPathDic;
            AssetBundleManager.GetInstance.LoadAssetBundleConfig(AppConfig.ABConfigPath);
            //场景管理初始化
            GameSceneService.GetInstance.Init(this, AppConfig.SceneLoading);
            GameSceneService.GetInstance.CurrenSceneName = AppConfig.LoginScene;
            GameSceneService.GetInstance.EmptyScene = AppConfig.EmptyScene;
            //加载版本管理模块
            //todo
            //加载所有数据配置表
            LoadConfigTable();
            ConfigerManager.GetInstance.InitData();//加载完后，按照数据类型定义一些常用的数据变量,方便直接使用
            //加载登录模块
            AppTools.CreateModule<LoginModule>(MDef.LoginModule);
            AppTools.CreateModule<SceneModule>(MDef.SceneModule);
            AppTools.CreateModule<NPCModule>(MDef.NPCModule);
            AppTools.CreateModule<DieUIModule>(MDef.DieUIModule);
        }

        /// <summary>
        /// 加载配置表的方法
        /// </summary>
        private void LoadConfigTable() {
            ConfigerManager.GetInstance.LoadData<NPCData>(CT.TABLE_NPC);
            ConfigerManager.GetInstance.LoadData<SkillData>(CT.TABLE_SKILL);
            ConfigerManager.GetInstance.LoadData<LevelUpData>(CT.TABLE_LEVEL);
            //ConfigerManager.GetInstance.LoadData<NameData>(CT.TABLE_NAME);  //名字不是每次都用，换到其他地方初始化
        }

        private void Update() {
            if (GlobalEvent.OnUpdate != null) {
                GlobalEvent.OnUpdate?.Invoke();//让其他非mono的脚本 ，可以使用update的事件
            }
            //每帧调用事件事件
            ThreadManager.Run(17);//时间计数间隔，17为默认的60帧，1000/17，约为60
        }
        private void FixedUpdate() {
            if (GlobalEvent.OnFixedUpdate != null) {
                GlobalEvent.OnFixedUpdate?.Invoke();//让其他非mono的脚本 ，可以使用OnFixedUpdate的事件
            }
        }
        private void OnDestroy() {
            //Exit("正常退出！");
        }
        #region 网络方面的处理,处理各类网络消息
        /// <summary>
        /// 打开防作弊功能，防止内存修改，需添加宏定义ANTICHEAT
        /// </summary>
        private void OpenDeCheat() {
            Net.Helper.AntiCheatHelper.IsActive = true;
            Net.Helper.AntiCheatHelper.OnDetected = OnDetected;
            Debuger.Log("已开启防作弊！");
        }
        /// <summary>
        ///  检测到作弊后的执行函数
        /// </summary>
        /// <param name="name">字段名称</param>
        /// <param name="oldvalue">旧值</param>
        /// <param name="newvalue">修改值</param>
        private async void OnDetected(string name, object oldvalue, object newvalue) {
            Debuger.LogError($"发现作弊，修改字段{name},原值{oldvalue},修改值{newvalue}");
            await UniTask.Yield();//等待一帧，防止多个弹窗重复
            UIMsgBoxArg arg = new UIMsgBoxArg();
            arg.title = "警告";
            arg.content = "检测到修改作弊,系统已拦截，本游戏即将退出，杜绝作弊，健康游戏，健康生活！";
            arg.btnname = "退出游戏";
            UIMsgBox temp = UIManager.GetInstance.OpenWindow(AppConfig.UIMsgBox, arg) as UIMsgBox;
            temp.oncloseevent += OnFailed;
        }
        /// <summary>
        /// 退出游戏
        /// </summary>
        /// <param name="args"></param>
        private async void OnFailed(object args = null) {
            var task = await ClientBase.Instance.Call((ushort)ProtoType.signout);
            if (!task.IsCompleted) {//超时
                UIMsgBoxArg arg = new UIMsgBoxArg();
                arg.title = "警告";
                arg.content = "网络故障，请求超时！";
                arg.btnname = "退出游戏";
                UIMsgBox temp = UIManager.GetInstance.OpenWindow(AppConfig.UIMsgBox, arg) as UIMsgBox;
                temp.oncloseevent += OnFailed2;
                return;
            }
            Exit("作弊！");
            Application.Quit();
        }
        private async void OnFailed2(object args = null) {
            Exit("网络问题退出！");
            Application.Quit();
        }
        /// <summary>
        /// 当连接成功
        /// </summary>
        public void OnConnected() {

        }
        /// <summary>
        /// 当连接失败
        /// </summary>
        public void OnConnectFailed() {
            UIMsgBoxArg arg = new UIMsgBoxArg();
            arg.title = "网络错误";
            arg.content = "网络连接错误，可能是服务器未启动，请稍后重试，或者联系管理员！";
            arg.btnname = "关闭游戏";
            UIMsgBox box = UIManager.GetInstance.OpenWindow(AppConfig.UIMsgBox, arg) as UIMsgBox;
            box.oncloseevent += OnFailed2;
        }
        /// <summary>
        /// 当连接中断
        /// </summary>
        public void OnConnectLost() {
            UIManager.GetInstance.OpenUIWidget(AppConfig.UIMsgTips, "网络中断，正在准备重连");
        }
        /// <summary>
        /// 当主动断开连接
        /// </summary>
        public void OnDisconnect() {

        }
        /// <summary>
        /// 当尝试重连
        /// </summary>
        public void OnTryToConnect() {
            if (ClientBase.Instance.CurrReconnect <= ClientBase.Instance.ReconnectCount) {
                UIManager.GetInstance.OpenUIWidget(AppConfig.UIMsgTips, $"网络中断，正在准备第{ClientBase.Instance.CurrReconnect}次重连");
            } else {
                UIMsgBoxArg arg = new UIMsgBoxArg();
                arg.title = "网络错误";
                arg.content = "网络连接错误，重连失败，请稍后重试，或者联系管理员！";
                arg.btnname = "关闭游戏";
                UIMsgBox box = UIManager.GetInstance.OpenWindow(AppConfig.UIMsgBox, arg) as UIMsgBox;
                box.oncloseevent += OnFailed2;
            }
        }
        /// <summary>
        /// 当重连成功
        /// </summary>
        public async void OnReconnect() {
            UIManager.GetInstance.OpenUIWidget(AppConfig.UIMsgTips, $"网络连接已恢复！");
            //重新发送登录
            var task = await ClientBase.Instance.Call((ushort)ProtoType.relogin, UserService.GetInstance.m_UserModel.m_Users.Username, AppTools.GetModule<SceneModule>(MDef.SceneModule).m_Current);
            if (!task.IsCompleted) {//重新登录失败，就让玩家退出游戏再试
                UIMsgBoxArg arg = new UIMsgBoxArg();
                arg.title = "网络错误";
                arg.content = "网络连接错误，重连失败，请稍后重试，或者联系管理员！";
                arg.btnname = "关闭游戏";
                UIMsgBox box = UIManager.GetInstance.OpenWindow(AppConfig.UIMsgBox, arg) as UIMsgBox;
                box.oncloseevent += OnFailed2;
            } else {
                ClientBase.Instance.SendRT((ushort)ProtoType.reenterscene, AppTools.GetModule<SceneModule>(MDef.SceneModule).m_Current);
            }
        }
        /// <summary>
        /// 当关闭连接
        /// </summary>
        public void OnCloseConnect() {

        }
        /// <summary>
        /// 当排队时调用
        /// </summary>
        /// <param name="count"></param>
        public void OnWhenQueuing(int totalCount, int count) {

        }
        /// <summary>
        /// 当排队结束调用
        /// </summary>
        public void OnQueueCancellation() {

        }
        /// <summary>
        /// 当服务器爆炸，积极拒绝客户端连接
        /// </summary>
        public void OnServerFull() {
            UIMsgBoxArg arg = new UIMsgBoxArg();
            arg.title = "重要提示";
            arg.content = "服务器爆满，已超出运行负荷，请稍后再试！";
            arg.btnname = "关闭游戏";
            UIMsgBox box = UIManager.GetInstance.OpenWindow(AppConfig.UIMsgBox, arg) as UIMsgBox;
            box.oncloseevent += OnFailed2;
        }
       
        #endregion
    }

}

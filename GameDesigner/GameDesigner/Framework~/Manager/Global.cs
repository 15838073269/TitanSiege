using Net.Component;
using UnityEngine;

namespace Framework
{
    [DefaultExecutionOrder(-50)]
    public class Global : SingleCase<Global>
    {
        [SerializeField] protected Camera uiCamera;
        [SerializeField] protected ResourcesManager resources;
        [SerializeField] protected UIManager ui;
        [SerializeField] protected AssetBundleCheckUpdate checkUpdate;
        [SerializeField] protected TableManager table;
        [SerializeField] protected SceneManager scene;
        [SerializeField] protected new AudioManager audio;
        [SerializeField] protected TimerManager timer;
        [SerializeField] protected ConfigManager config;
        [SerializeField] protected NetworkManager network;
        [SerializeField] protected Logger logger;
        [SerializeField] protected ObjectPool pool;
        
        public static ResourcesManager Resources;
        public static UIManager UI;
        public static AssetBundleCheckUpdate CheckUpdate;
        public static TableManager Table;
        public static SceneManager Scene;
        public static AudioManager Audio;
        public static TimerManager Timer;
        public static ConfigManager Config;
        public static NetworkManager Network;
        public static Logger Logger;
        public static ObjectPool Pool;

        public AssetBundleMode Mode = AssetBundleMode.LocalPath;
        public string entryRes = "Assets/Resources/Prefabs/GameEntry.prefab";

        public static Camera UICamera { get => Instance.uiCamera; set => Instance.uiCamera = value; }

        protected override void Awake()
        {
            base.Awake();
            Resources = resources;
            UI = ui;
            CheckUpdate = checkUpdate;
            Table = table;
            Scene = scene;
            Audio = audio;
            Timer = timer;
            Config = config;
            Network = network;
            Logger = logger;
            Pool = pool;
            DontDestroyOnLoad(gameObject);
        }

        /// <summary>
        /// 当初始化完成，初始化包括检查热更新，文件下载等等
        /// </summary>
        public virtual void OnInit()
        {
            Resources.Instantiate(entryRes);
        }
    }
}
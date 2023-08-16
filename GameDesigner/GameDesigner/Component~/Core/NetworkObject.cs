#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_WSA || UNITY_WEBGL
namespace Net.UnityComponent
{
    using global::System.Collections.Generic;
    using Net.Client;
    using Net.Component;
    using Net.Helper;
    using Net.Share;
    using Net.System;
    using UnityEngine;

    /// <summary>
    /// 网络物体标识组件
    /// </summary>
    [DisallowMultipleComponent]
    [DefaultExecutionOrder(1000)]
    public class NetworkObject : MonoBehaviour
    {
        internal static int IDENTITY { get; private set; } = -1;
        internal static int IDENTITY_MAX { get; private set; }
        internal static Queue<int> IDENTITY_POOL = new Queue<int>();
        public static int Capacity { get; private set; }
        public static bool IsInitIdentity => IDENTITY != -1;
        private int m_identity = -1;
        [Tooltip("自定义唯一标识, 当值不为0后,可以不通过NetworkSceneManager的registerObjects去设置, 直接放在设计的场景里面, 不需要做成预制体")]
        [SerializeField] private int identity;//可以设置的id
        [Tooltip("注册的网络物体索引, registerObjectIndex要对应NetworkSceneManager的registerObjects数组索引, 如果设置了自定义唯一标识, 则此字段无效!")]
        public int registerObjectIndex;
        [SerializeField] internal bool isLocal = true;
        internal List<NetworkBehaviour> networkBehaviours = new List<NetworkBehaviour>();
        public MyDictionary<ushort, SyncVarInfo> syncVarInfos = new MyDictionary<ushort, SyncVarInfo>();
        [Tooltip("是否初始化? 如果不想让Identity在Start被自动分配ID, 则可以设置此字段为true")]
        [SerializeField] internal bool isInit;
        public bool IsDispose { get; internal set; }
        /// <summary>
        /// 此物体是否是本机实例化？
        /// </summary>
        public bool IsLocal { get => isLocal; set => isLocal = value; }
        /// <summary>
        /// 每个网络对象的唯一标识
        /// </summary>
        public int Identity { get => m_identity; set => m_identity = value; }
        /// <summary>
        /// 获取或设置是否初始化
        /// </summary>
        public bool IsInitialize { get => isInit; set => isInit = value; }
        /// <summary>
        /// 是否可以发送销毁指令给服务器
        /// </summary>
        public bool CanDestroy { get; set; } = true;

        public virtual void Start()
        {
            Init();
        }

        public void ReInit() 
        {
            isInit = false;
            Init();
        }

        public void Init()
        {
            if (isInit)
                return;
            isInit = true;
            if (IDENTITY == -1 & identity == 0)//全局netobj
            {
                Debug.LogError("网络标识未初始化，请调用NetworkObject.Init(5000);初始化");
                Destroy(gameObject);
                return;
            }
            var sm = NetworkSceneManager.I;
            if (sm == null)
            {
                Debug.Log("没有找到NetworkSceneManager组件！NetworkIdentity组件无效！");
                Destroy(gameObject);
                return;
            }
            if (!isLocal | m_identity > 0)
            {
                goto J1;
            }
            if (identity > 0)
            {
                m_identity = identity;
                goto J1;
            }
            if (IDENTITY_POOL.Count > 0)
            {
                m_identity = IDENTITY_POOL.Dequeue();
                goto J1;
            }
            if (IDENTITY < IDENTITY_MAX)
            {
                m_identity = IDENTITY++;
                goto J1;
            }
            else
            {
                Debug.LogError("网络标识已用完! 如果有需要请加大网络标识数量NetworkObject.Init(10000);");
                Destroy(gameObject);
                return;
            }
        J1:
            if (!sm.identitys.TryAdd(m_identity, this, out var oldNetObj))
            {
                if (oldNetObj == this | oldNetObj == null)
                    return;
                oldNetObj.m_identity = -1;
                Debug.Log($"uid:{m_identity}发生了两次实例化! 本地的实例化和网络同步下来的identity冲突");
                Destroy(oldNetObj.gameObject);
            }
        }

        public void InitAll(Operation opt = default)
        {
            Init();
            var nbs = GetComponentsInChildren<NetworkBehaviour>(true);
            foreach (var np in nbs)
            {
                np.Init(opt);
            }
        }
        /// <summary>
        /// 添加字段和属性同步
        /// </summary>
        /// <param name="networkBehaviour"></param>
        /// <param name="target"></param>
        public void AddSyncVar(NetworkBehaviour networkBehaviour, object target)
        {
            int syncVarID = networkBehaviour.NetComponentID * 50; //每个组件和定义[SyncVar]同步字段和属性最多50个
            ClientBase.Instance.AddRpcHandle(target, false, (info) =>
            {
                info.id = (ushort)(syncVarID + networkBehaviour.SyncVarID++);
                syncVarInfos.Add(info.id, info);
                if (!isLocal)
                {
                    ClientBase.Instance.AddOperation(new Operation(NetCmd.SyncVarGet, m_identity)
                    {
                        index = registerObjectIndex,
                        index1 = info.id,
                    });
                }
            });
        }

        internal void CheckSyncVar()
        {
            if (syncVarInfos.Count == 0)
                return;
            var buffer = SyncVarHelper.CheckSyncVar(isLocal, syncVarInfos);
            if (buffer != null)
                SyncVarSend(buffer);
        }

        private void SyncVarSend(byte[] buffer) 
        {
            ClientBase.Instance.AddOperation(new Operation(NetCmd.SyncVarNetObj, m_identity)
            {
                uid = ClientBase.Instance.UID,
                index = registerObjectIndex,
                buffer = buffer
            });
        }

        internal void SyncVarHandler(Operation opt)
        {
            if (opt.uid == ClientBase.Instance.UID)
                return;
            SyncVarHelper.SyncVarHandler(syncVarInfos, opt.buffer);
        }

        internal void RemoveSyncVar(NetworkBehaviour target)
        {
            SyncVarHelper.RemoveSyncVar(syncVarInfos, target);
        }

        internal void PropertyAutoCheckHandler() 
        {
            for (int i = 0; i < networkBehaviours.Count; i++)
            {
                var networkBehaviour = networkBehaviours[i];
                if (networkBehaviour == null)
                    continue;
                if (!networkBehaviour.CheckEnabled())
                    continue;
                networkBehaviour.OnPropertyAutoCheck();
            }
        }

        public virtual void OnDestroy()
        {
            if (IsDispose)
                return;
            IsDispose = true;
            if (m_identity == -1)
                return;
            var sm = NetworkSceneManager.Instance;
            if (sm == null)
                return;
            if (!isLocal | m_identity < 10000)//0-10000是场景可用标识
            {
                sm.waitDestroyList.Add(new WaitDestroy(m_identity, false, Time.time + 1f));
                return;
            }
            sm.waitDestroyList.Add(new WaitDestroy(m_identity, true, Time.time + 1f));
            if (!CanDestroy)
                return;
            if (ClientBase.Instance == null)
                return;
            if (!ClientBase.Instance.Connected)
                return;
            SendDestroyCommand();
        }

        public void SendDestroyCommand()
        {
            ClientBase.Instance.AddOperation(new Operation(Command.Destroy, m_identity));
        }

        internal static void PushIdentity(int identity)
        {
            if (IDENTITY == -1)
                return;
            IDENTITY_POOL.Enqueue(identity);
        }

        /// <summary>
        /// 初始化网络唯一标识
        /// </summary>
        /// <param name="capacity">一个客户端可以用的唯一标识容量</param>
        public static void Init(int capacity = 5000) 
        {
            //要实时可初始化，要不然每次切换场景都无法初始化id，或者切换账号后uid变了，就得不到真正的identity值了
            Capacity = capacity;
            //0-10000是公共id，10000-15000是玩家uid，也就是同时在线5000个玩家，每个玩家占用一个id，15000-20000是uid=10000的网络物体id，
            //每个玩家可以实例化5000个网络物体，并且id都是唯一的，如果超出则报错
            IDENTITY = 10000 + ((ClientBase.Instance.UID + 1 - 10000) * capacity);
            IDENTITY_MAX = IDENTITY + capacity;
            IDENTITY_POOL.Clear();
        }

        /// <summary>
        /// 释放初始化的identity
        /// </summary>
        public static void UnInit()
        {
            IDENTITY = -1;
            IDENTITY_MAX = 0;
            Capacity = 0;
            IDENTITY_POOL.Clear();
        }

        /// <summary>
        /// 获取玩家id的偏移量, 此方法算出来每个玩家可实例化多少个网络对象
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public static int GetUserIdOffset(int uid)
        {
            //0-10000是公共id，10000-15000是玩家uid，也就是同时在线5000个玩家，每个玩家占用一个id，15000-20000是uid=10000的网络物体id，
            //每个玩家可以实例化5000个网络物体，并且id都是唯一的，如果超出则报错
            return 10000 + ((uid + 1 - 10000) * Capacity);
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            var networkBehaviours = gameObject.GetComponentsInChildren<NetworkBehaviour>(true);
            for (int i = 0; i < networkBehaviours.Length; i++)
            {
                var networkBehaviour = networkBehaviours[i];
                if (networkBehaviour.NetComponentID == -1)
                {
                    networkBehaviour.NetComponentID = i;
                    UnityEditor.EditorUtility.SetDirty(networkBehaviour);
                }
            }
        }
#endif
    }
}
#endif
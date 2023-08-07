#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_WSA || UNITY_WEBGL
namespace Net.UnityComponent
{
    using global::System;
    using global::System.Collections.Generic;
    using Net.Client;
    using Net.Component;
    using Net.Share;
    using Net.System;
    using UnityEngine;
    using Cysharp.Threading.Tasks;

    [Serializable]
    public class WaitDestroy
    {
        public int identity;
        public bool isPush;
        public float time;

        public WaitDestroy(int identity, bool isPush, float time)
        {
            this.identity = identity;
            this.isPush = isPush;
            this.time = time;
        }
    }

    [DefaultExecutionOrder(1)]
    public class NetworkSceneManager : SingleCase<NetworkSceneManager>
    {
        public List<NetworkObject> registerObjects = new List<NetworkObject>();
        [HideInInspector]
        public MyDictionary<int, NetworkObject> identitys = new MyDictionary<int, NetworkObject>();
        [Tooltip("如果onExitDelectAll=true 当客户端退出游戏,客户端所创建的所有网络物体也将随之被删除? onExitDelectAll=false只删除玩家物体")]
        public bool onExitDelectAll = true;
        internal List<WaitDestroy> waitDestroyList = new List<WaitDestroy>();
        protected ClientBase client; //当多场景时, 退出战斗场景, 回主场景时, 先进入主场景再卸载战斗场景, 而ClientBase.Instance被赋值到其他多连接客户端对象上就会出现OnDestry时没有正确移除OnOperationSync事件
        protected Queue<Action> waitNetworkIdentityQueue = new Queue<Action>();

        public virtual void Start()
        {
            _ = WaitConnecting();
            if (NetworkTime.Instance == null)
                gameObject.AddComponent<NetworkTime>();
        }

        public virtual async UniTaskVoid WaitConnecting()
        {
            var outTime = DateTime.Now.AddSeconds(10);
            while (DateTime.Now < outTime)
            {
                if (ClientBase.Instance == null)
                    await UniTask.Yield();
                else if (!ClientBase.Instance.Connected)
                    await UniTask.Yield();
                else
                    break;
            }
            if (DateTime.Now > outTime)
            {
                Debug.Log("连接超时!");
                return;
            }
            OnConnected();
            client = ClientBase.Instance;
            client.OnOperationSync += OperationSync;
            while (waitNetworkIdentityQueue.Count > 0)
                waitNetworkIdentityQueue.Dequeue()?.Invoke();
        }

        public virtual void OnConnected()
        {
            NetworkObject.Init(5000);
        }

        /// <summary>
        /// 等待网络标识初始化, 当标识初始化完成调用onInitComplete委托
        /// </summary>
        /// <param name="onInitComplete"></param>
        public virtual void WaitNetworkIdentityInit(Action onInitComplete)
        {
            if (NetworkObject.IsInitIdentity)
                onInitComplete?.Invoke();
            else
                waitNetworkIdentityQueue.Enqueue(onInitComplete);
        }

        public virtual void Update() 
        {
            if (NetworkTime.CanSent) 
            {
                for (int i = 0; i < identitys.count; i++)
                {
                    if (identitys.entries[i].hashCode == -1)
                        continue;
                    var identity = identitys.entries[i].value;
                    if (identity == null)
                        continue;
                    if (!identity.enabled)
                        continue;
                    if (identity.IsDispose)
                        continue;
                    identity.CheckSyncVar();
                    identity.PropertyAutoCheckHandler();
                }
            }
            WaitDestroy waitDestroy;
            for (int i = 0; i < waitDestroyList.Count; i++)
            {
                waitDestroy = waitDestroyList[i];
                if (Time.time >= waitDestroy.time) 
                {
                    RemoveIdentity(waitDestroy.identity);
                    waitDestroyList.RemoveAt(i);
                    if (!waitDestroy.isPush)
                        continue;
                    NetworkObject.PushIdentity(waitDestroy.identity);
                }
            }
        }

        private void OperationSync(OperationList list)
        {
            foreach (var opt in list.operations)
                OnNetworkOperSync(opt);
        }

        private void OnNetworkOperSync(Operation opt)
        {
            switch (opt.cmd) 
            {
                case Command.Transform:
                    OnBuildOrTransformSync(opt);
                    break;
                case Command.BuildComponent:
                    OnBuildOrTransformSync(opt);
                    break;
                case Command.Destroy:
                    OnNetworkObjectDestroy(opt);
                    break;
                case Command.OnPlayerExit:
                    OnPlayerExit(opt);
                    break;
                case NetCmd.SyncVarNetObj:
                    OnSyncVarHandler(opt);
                    break;
                case NetCmd.SyncVarGet:
                    SyncVarGetHandler(opt);
                    break;
                case NetCmd.CallRpc:
                    var data = client.OnDeserializeRPC(opt.buffer, 0, opt.buffer.Length);
                    if(!string.IsNullOrEmpty(data.name))
                        client.DispatchRpc(data.name, data.pars);
                    else if(data.hash != 0)
                        client.DispatchRpc(data.hash, data.pars);
                    break;
                default:
                    OnOtherOperator(opt);
                    break;
            }
        }

        /// <summary>
        /// 当检查网络标识物体，如果不存在就会实例化 --- 在这里用到了<see cref="Operation.identity"/>作为网络物体标识, <see cref="Operation.index"/>作为要实例化<see cref="registerObjects"/>的物体索引
        /// </summary>
        /// <param name="opt"></param>
        /// <returns></returns>
        public virtual NetworkObject OnCheckIdentity(Operation opt) 
        {
            if (!identitys.TryGetValue(opt.identity, out NetworkObject identity))
            {
                if (opt.index >= registerObjects.Count)
                    return null;
                identity = Instantiate(registerObjects[opt.index]);
                identity.Identity = opt.identity;
                identity.isLocal = false;
                identity.isInit = true;
                identity.InitAll(opt);
                identitys.TryAdd(opt.identity, identity);
                OnNetworkObjectCreate(opt, identity);
            }
            return identity;
        }

        /// <summary>
        /// 当BuildComponent指令或Transform指令同步时调用
        /// </summary>
        /// <param name="opt"></param>
        public virtual void OnBuildOrTransformSync(Operation opt) 
        {
            var identity = OnCheckIdentity(opt);
            if (identity == null)
                return;
            if (identity.IsDispose)
                return;
            var nb = identity.networkBehaviours[opt.index1];
            if (nb == null)
                return;
            nb.OnNetworkOperationHandler(opt);
        }

        public virtual void OnSyncVarHandler(Operation opt) 
        {
            var identity = OnCheckIdentity(opt);
            if (identity == null)
                return;
            if (identity.IsDispose)
                return;
            identity.SyncVarHandler(opt);
        }

        public virtual void SyncVarGetHandler(Operation opt) 
        {
            var identity = OnCheckIdentity(opt);
            if (identity == null)
                return;
            if (identity.IsDispose)
                return;
            if (!identity.isLocal)
                return;
            identity.syncVarInfos[(ushort)opt.index1].SetDefaultValue();
        }

        /// <summary>
        /// 当其他网络物体被删除(入口1)
        /// </summary>
        /// <param name="opt"></param>
        public virtual void OnNetworkObjectDestroy(Operation opt) 
        {
            if (identitys.TryGetValue(opt.identity, out NetworkObject identity))
            {
                OnPlayerDestroy(identity, false);
            }
        }

        public virtual void OnPlayerExit(Operation opt)
        {
            if (identitys.TryGetValue(opt.identity, out NetworkObject identity))//删除退出游戏的玩家游戏物体
                OnPlayerDestroy(identity, true);
            if (onExitDelectAll)//删除此玩家所创建的所有游戏物体
            {
                var uid = NetworkObject.GetUserIdOffset(opt.identity);
                var count = uid + NetworkObject.Capacity;
                foreach (var item in identitys)
                    if (item.Key >= uid & item.Key < count)
                        OnPlayerDestroy(item.Value, false);
            }
        }

        private void OnPlayerDestroy(NetworkObject identity, bool isPlayer)
        {
            if (identity == null)
                return;
            if (identity.IsDispose)
                return;
            if(isPlayer)
                OnOtherExit(identity);
            OnOtherDestroy(identity);
        }

        public void RemoveIdentity(int identity)
        {
            identitys.Remove(identity);
        }

        /// <summary>
        /// 当其他网络物体被创建(实例化)
        /// </summary>
        /// <param name="opt"></param>
        /// <param name="identity"></param>
        public virtual void OnNetworkObjectCreate(Operation opt, NetworkObject identity)
        {
        }

        /// <summary>
        /// 当其他网络物体被删除(入口2)
        /// </summary>
        /// <param name="identity"></param>
        public virtual void OnOtherDestroy(NetworkObject identity)
        {
            Destroy(identity.gameObject);
        }

        /// <summary>
        /// 当其他玩家网络物体退出(删除)
        /// </summary>
        /// <param name="identity"></param>
        public virtual void OnOtherExit(NetworkObject identity)
        {
        }

        /// <summary>
        /// 当其他操作指令调用
        /// </summary>
        /// <param name="opt"></param>
        public virtual void OnOtherOperator(Operation opt)
        {
        }

        private void OnApplicationQuit()
        {
            ExitSceneHandler();
        }

        /// <summary>
        /// 当退出场景时有些网络物体是不应该被销毁的
        /// </summary>
        public void ExitSceneHandler()
        {
            foreach (var identity in identitys)
            {
                identity.Value.Identity = -1;
            }
        }

        public virtual void OnDestroy()
        {
            NetworkObject.UnInit();//每次离开战斗场景都要清除初始化identity
            if (client == null)
                return;
            client.OnOperationSync -= OperationSync;
        }
    }
}
#endif
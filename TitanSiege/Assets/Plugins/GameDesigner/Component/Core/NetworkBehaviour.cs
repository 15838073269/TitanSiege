#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_WSA || UNITY_WEBGL
namespace Net.UnityComponent
{
    using global::System;
    using Net.Share;
    using UnityEngine;

    /// <summary>
    /// 网络行为基础组件
    /// </summary>
    [DefaultExecutionOrder(1000)]
    public abstract class NetworkBehaviour : MonoBehaviour
    {
        public NetworkObject netObj;
        [Tooltip("网络组件的ID,当FPS游戏时,自己身上只有一双手和枪,而其他客户端要显示完整模型时,用到另外的预制体,就会出现组件获取不一致问题,所以这里提供了网络组件ID,即可解决此问题")]
        [SerializeField] private int netComponentID = -1;
        /// <summary>
        /// 网络组件id, 此组件是netobj的第几个组件
        /// </summary>
        public int NetComponentID
        {
            get { return netComponentID > 10 ? -1 : netComponentID; }
            set
            {
                if (value > 10)
                    throw new Exception("组件最多不能超过10个");
                netComponentID = value;
            }
        }
        public int SyncVarID { get; set; }
        /// <summary>
        /// 这个物体是本机生成的?
        /// true:这个物体是从你本机实例化后, 同步给其他客户端的, 其他客户端的IsLocal为false
        /// false:这个物体是其他客户端实例化后,同步到本机客户端上, IsLocal为false
        /// </summary>
        public bool IsLocal => netObj.isLocal;
        private bool isInit;
        private bool isEnabled;
        public virtual void Start()
        {
            Init();
        }
        private void OnValidate()
        {
            if (TryGetComponent(out netObj))
                return;
            netObj = gameObject.GetComponentInParent<NetworkObject>(true); //此处需要加上参数true, 否则做成预制体时会找不到父组件
            if (netObj == null)
                netObj = gameObject.AddComponent<NetworkObject>();
        }
        public void Init(Operation opt = default)
        {
            if (isInit)
                return;
            isInit = true;
            netObj = GetComponentInParent<NetworkObject>();
            if (NetComponentID == -1)
            {
                NetComponentID = netObj.networkBehaviours.Count;
                netObj.networkBehaviours.Add(this);
            }
            else
            {
                while (netObj.networkBehaviours.Count <= NetComponentID)
                    netObj.networkBehaviours.Add(null);
                if (netObj.networkBehaviours[NetComponentID] != null)
                    throw new Exception($"索引有冲突!打开预制体设置{this}组件的NetComponentID值为唯一ID!");
                netObj.networkBehaviours[NetComponentID] = this;
            }
            netObj.AddSyncVar(this, this);
            if (IsLocal)
                OnNetworkObjectInit(netObj.Identity);
            else
                OnNetworkObjectCreate(opt);
        }
        /// <summary>
        /// 当网络物体被初始化, 只有本机实例化的物体才会被调用
        /// </summary>
        /// <param name="identity"></param>
        public virtual void OnNetworkObjectInit(int identity) { }
        /// <summary>
        /// 当网络物体被创建后调用, 只有其他客户端发送创建信息给本机后才会被调用
        /// </summary>
        /// <param name="opt"></param>
        public virtual void OnNetworkObjectCreate(Operation opt) { }
        /// <summary>
        /// 当网络操作到达后应当开发者进行处理
        /// </summary>
        /// <param name="opt"></param>
        public virtual void OnNetworkOperationHandler(Operation opt) { }
        /// <summary>
        /// 当属性自动同步检查
        /// </summary>
        public virtual void OnPropertyAutoCheck() { }
        /// <summary>
        /// 检查组件是否启用
        /// </summary>
        /// <returns></returns>
        public virtual bool CheckEnabled() { return isEnabled; }
        public virtual void OnEnable()
        {
            isEnabled = true;
        }
        public virtual void OnDisable()
        {
            isEnabled = false;
        }
        public virtual void OnDestroy()
        {
            netObj.RemoveSyncVar(this);
        }
    }
}
#endif
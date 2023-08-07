namespace Net.Server
{
    using global::System;
    using global::System.Collections.Generic;
    using global::System.Net;
    using global::System.Net.Sockets;
    using global::System.Reflection;
    using global::System.Collections.Concurrent;
    using global::System.IO;
    using Net.Event;
    using Net.Share;
    using Net.System;
    using Net.Helper;

    /// <summary>
    /// 网络玩家 - 当客户端连接服务器后都会为每个客户端生成一个网络玩家对象，(玩家对象由服务器管理) 2019.9.9
    /// <code>注意:不要试图new player出来, new出来后是没有作用的!</code>
    /// </summary>
    public class NetPlayer : IDisposable, IRpcHandler
    {
        /// <summary>
        /// 玩家名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Tcp套接字
        /// </summary>
        public Socket Client { get; set; }
        /// <summary>
        /// io完成端口接收对象
        /// </summary>
        public SocketAsyncEventArgs ReceiveArgs { get; set; }
        /// <summary>
        /// 存储客户端终端
        /// </summary>
        public EndPoint RemotePoint { get; set; }
        /// <summary>
        /// 此玩家所在的场景名称
        /// </summary>
        public string SceneName { get; set; } = string.Empty;
        /// <summary>
        /// 客户端玩家的标识
        /// </summary>
        public string PlayerID { get; set; } = string.Empty;
        /// <summary>
        /// 玩家所在的场景实体
        /// </summary>
        public virtual object Scene { get; set; }
        /// <summary>
        /// 远程方法优化字典
        /// </summary>
        public MyDictionary<string, RPCMethodBody> RpcDic { get; set; } = new MyDictionary<string, RPCMethodBody>();
        /// <summary>
        /// 远程方法哈希字典
        /// </summary>
        public MyDictionary<ushort, RPCMethodBody> RpcHashDic { get; set; } = new MyDictionary<ushort, RPCMethodBody>();
        /// <summary>
        /// 已经收集过的类信息
        /// </summary>
        public MyDictionary<Type, List<MemberData>> MemberInfos { get; set; } = new MyDictionary<Type, List<MemberData>>();
        /// <summary>
        /// 当前收集rpc的对象信息
        /// </summary>
        public MyDictionary<object, MemberDataList> RpcTargetHash { get; set; } = new MyDictionary<object, MemberDataList>();
        /// <summary>
        /// 字段同步信息
        /// </summary>
        public MyDictionary<ushort, SyncVarInfo> SyncVarDic { get; set; } = new MyDictionary<ushort, SyncVarInfo>();
        /// <summary>
        /// Rpc任务队列
        /// </summary>
        public QueueSafe<IRPCData> RpcWorkQueue { get; set; } = new QueueSafe<IRPCData>();
        /// <summary>
        /// 跳动的心
        /// </summary>
        public byte heart { get; set; } = 0;
        /// <summary>
        /// TCP叠包值， 0:正常 >1:叠包次数 >25:清空叠包缓存流
        /// </summary>
        internal int stack = 0;
        internal int stackIndex;
        internal int stackCount;
        /// <summary>
        /// TCP叠包临时缓存流
        /// </summary>
        internal MemoryStream stackStream;
        /// <summary>
        /// 用户唯一身份标识
        /// </summary>
        public int UserID { get; internal set; }
        internal QueueSafe<RPCModel> RpcModels = new QueueSafe<RPCModel>();
        public QueueSafe<ISegment> RevdQueue = new QueueSafe<ISegment>();
        private ThreadGroup group;
        /// <summary>
        /// 当前玩家所在的线程组对象
        /// </summary>
        public ThreadGroup Group
        {
            get => group;
            set
            {
                group?.Remove(this);
                group = value;
                group?.Add(this); //当释放后Group = null;
            }
        }
        internal int SceneHash;
        public bool Login { get; internal set; }
        public bool isDispose { get; internal set; }
        /// <summary>
        /// 是否处于连接
        /// </summary>
        public bool Connected { get; set; }
        internal MyDictionary<int, FileData> ftpDic = new MyDictionary<int, FileData>();
        private byte[] addressBuffer;
        /// <summary>
        /// 确定是否是冗余连接
        /// </summary>
        public bool Redundant { get; set; }
        /// <summary>
        /// 当前排队座号
        /// </summary>
        public int QueueUpNo { get; internal set; }
        /// <summary>
        /// 是否属于排队状态
        /// </summary>
        public bool IsQueueUp => QueueUpNo > 0;
        public IGcp Gcp { get; set; }
        /// <summary>
        /// 客户端连接时间
        /// </summary>
        public DateTime ConnectTime { get; set; }
        /// <summary>
        /// 断线重连等待时间
        /// </summary>
        public uint ReconnectTimeout { get; set; }
        /// <summary>
        /// 此客户端接收到的字节总量
        /// </summary>
        public long BytesReceived { get; set; }
        /// <summary>
        /// CRC校验错误次数, 如果有错误每秒提示一次
        /// </summary>
        public int CRCError { get; set; }
        /// <summary>
        /// 发送窗口已满提示次数
        /// </summary>
        public int WindowFullError { get; set; }
        /// <summary>
        /// 数据大小错误, 数据被拦截修改或者其他问题导致错误
        /// </summary>
        public int DataSizeError { get; set; }

        #region 创建网络客户端(玩家)
        /// <summary>
        /// 构造网络客户端
        /// </summary>
        public NetPlayer() { }

        /// <summary>
        /// 构造网络客户端，Tcp
        /// </summary>
        /// <param name="client">客户端套接字</param>
        public NetPlayer(Socket client)
        {
            Client = client;
            RemotePoint = client.RemoteEndPoint;
        }

        /// <summary>
        /// 构造网络客户端
        /// </summary>
        /// <param name="remotePoint"></param>
        public NetPlayer(EndPoint remotePoint)
        {
            RemotePoint = remotePoint;
        }
        #endregion

        #region 客户端释放内存
        /// <summary>
        /// 析构网络客户端
        /// </summary>
        ~NetPlayer()
        {
            Dispose();
        }

        /// <summary>
        /// 释放
        /// </summary>
        public virtual void Dispose()
        {
            if (isDispose)
                return;
            isDispose = true;
            if (ReceiveArgs != null)
            {
                ReceiveArgs.Dispose();
                ReceiveArgs = null;
            }
            if (Client != null) 
            {
                Client.Shutdown(SocketShutdown.Both);
                Client.Close();
            }
            stackStream?.Close();
            stackStream = null;
            stack = 0;
            stackIndex = 0;
            stackCount = 0;
            Connected = false;
            heart = 0;
            RpcModels = new QueueSafe<RPCModel>();
            Login = false;
            addressBuffer = null;
            Gcp?.Dispose();
            Group = null;
        }
        #endregion

        #region 客户端(玩家)Rpc(远程过程调用)处理
        /// <summary>
        /// 添加远程过程调用函数,从对象进行收集
        /// </summary>
        /// <param name="append">可以重复添加rpc?</param>
        public void AddRpc(bool append = false)
        {
            AddRpc(this, append);
        }

        /// <summary>
        /// 添加远程过程调用函数,从对象进行收集
        /// </summary>
        /// <param name="target"></param>
        /// <param name="append">可以重复添加rpc?</param>
        public void AddRpc(object target, bool append = false)
        {
            RpcHelper.AddRpc(this, target, append, null);
        }

        /// <summary>
        /// 移除网络远程过程调用函数
        /// </summary>
        /// <param name="target">移除的rpc对象</param>
        public void RemoveRpc(object target)
        {
            RpcHelper.RemoveRpc(this, target);
        }

        //public void CheckRpc()
        //{
        //    RpcHelper.CheckRpc(this);
        //}

        private bool CheckIsClass(Type type, ref int layer, bool root = true)
        {
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
            foreach (var field in fields)
            {
                var code = Type.GetTypeCode(field.FieldType);
                if (code == TypeCode.Object)
                {
                    if (field.FieldType.IsClass)
                        return true;
                    if (root)
                        layer = 0;
                    if (layer++ < 5)
                    {
                        var isClass = CheckIsClass(field.FieldType, ref layer, false);
                        if (isClass)
                            return true;
                    }
                }
            }
            return false;
        }

        internal byte[] RemoteAddressBuffer()
        {
            if (addressBuffer != null)
                return addressBuffer;
            var socketAddress = RemotePoint.Serialize();
            addressBuffer = new byte[socketAddress.Size];
            for (int i = 0; i < socketAddress.Size; i++)
                addressBuffer[i] = socketAddress[i];
            return addressBuffer;
        }
        #endregion

        #region 客户端数据处理函数
        /// <summary>
        /// 当未知客户端发送数据请求，返回<see langword="false"/>，不做任何事，返回<see langword="true"/>，添加到<see cref="ServerBase{Player, Scene}.Players"/>中
        /// 客户端玩家的入口点，在这里可以控制客户端是否可以进入服务器与其他客户端进行网络交互
        /// 在这里可以用来判断客户端登录和注册等等进站许可
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Obsolete("此方法存在严重漏洞, 已被弃用, 统一使用Server类的OnUnClientRequest方法处理", true)]
        public virtual bool OnUnClientRequest(RPCModel model)
        {
            return true;
        }

        /// <summary>
        /// 当web服务器未知客户端发送数据请求，返回<see langword="false"/>，不做任何事，返回<see langword="true"/>，添加到<see cref="ServerBase{Player, Scene}.Players"/>中
        /// 客户端玩家的入口点，在这里可以控制客户端是否可以进入服务器与其他客户端进行网络交互
        /// 在这里可以用来判断客户端登录和注册等等进站许可
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public virtual bool OnWSUnClientRequest(MessageModel model)
        {
            return true;
        }

        /// <summary>
        /// 当接收到客户端自定义数据请求,在这里可以使用你自己的网络命令，系列化方式等进行解析网络数据。（你可以在这里使用ProtoBuf或Json来解析网络数据）
        /// </summary>
        /// <param name="model"></param>
        public virtual void OnRevdBufferHandle(RPCModel model) { }

        /// <summary>
        /// 当接收到webSocket客户端自定义数据请求,在这里可以使用你自己的网络命令，系列化方式等进行解析网络数据。（你可以在这里使用ProtoBuf或Json来解析网络数据）
        /// </summary>
        /// <param name="model"></param>
        public virtual void OnWSRevdBuffer(MessageModel model) { }

        /// <summary>
        /// 当客户端连接中断, 此时还会等待客户端重连, 如果10秒后没有重连上来就会真的断开
        /// </summary>
        public virtual void OnConnectLost() { }

        /// <summary>
        /// 当断线重连成功触发
        /// </summary>
        public virtual void OnReconnecting() { }

        /// <summary>
        /// 当服务器判定客户端为断线或连接异常时，移除客户端时调用
        /// </summary>
        public virtual void OnRemoveClient() { }

        /// <summary>
        /// 当执行Rpc(远程过程调用函数)时, 提高性能可重写此方法进行指定要调用的函数
        /// </summary>
        /// <param name="model"></param>
        public virtual void OnRpcExecute(RPCModel model) => RpcHelper.Invoke(this, this, model, AddRpcWorkQueue, RpcLog);

        private void RpcLog(int log, NetPlayer client, RPCModel model)
        {
            switch (log)
            {
                case 0:
                    NDebug.LogWarning($"{this} [mask:{model.methodHash}]的远程方法未被收集!请定义[Rpc(hash = {model.methodHash})] void xx方法和参数, 并使用client.AddRpc方法收集rpc方法!");
                    break;
                case 1:
                    NDebug.LogWarning($"{this} {model.func}的远程方法未被收集!请定义[Rpc]void {model.func}方法和参数, 并使用client.AddRpc方法收集rpc方法!");
                    break;
                case 2:
                    NDebug.LogWarning($"{this} {model}的远程方法未被收集!请定义[Rpc]void xx方法和参数, 并使用client.AddRpc方法收集rpc方法!");
                    break;
            }
        }

        private void AddRpcWorkQueue(MyDictionary<object, IRPCMethod> methods, NetPlayer client, RPCModel model)
        {
            foreach (RPCMethod rpc in methods.Values)
            {
                rpc.Invoke(model.pars);
            }
        }

        #endregion

        #region 提供简便的重写方法
        /// <summary>
        /// 当玩家登录成功初始化调用
        /// </summary>
        public virtual void OnStart()
        {
            NDebug.Log($"玩家[{Name}]登录了游戏...");
        }

        /// <summary>
        /// 当玩家更新操作
        /// </summary>
        public virtual void OnUpdate() { }

        /// <summary>
        /// 当玩家进入场景 ->场景对象在Scene属性
        /// </summary>
        public virtual void OnEnter() { }

        /// <summary>
        /// 当玩家退出场景 ->场景对象在Scene属性
        /// </summary>
        public virtual void OnExit() { }

        /// <summary>
        /// 当玩家退出登录时调用
        /// </summary>
        public virtual void OnSignOut() { }

        /// <summary>
        /// 当场景被移除 ->场景对象在Scene属性
        /// </summary>
        [Obsolete("此方法已不再使用, 请使用Scene的OnRemove方法")]
        public virtual void OnRemove() { }

        /// <summary>
        /// 当接收到客户端使用<see cref="Net.Client.ClientBase.AddOperation(Operation)"/>方法发送的请求时调用. 如果重写此方法, 
        /// <code>返回false, 则服务器对象类会重新把操作列表加入到场景中, 你可以重写服务器的<see cref="ServerBase{Player, Scene}.OnOperationSync(Player, OperationList)"/>方法让此方法失效</code>
        /// <code>返回true, 服务器不再把数据加入到场景列表, 认为你已经在此处把数据处理了</code>
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public virtual bool OnOperationSync(OperationList list) { return false; }

        /// <summary>
        /// 当属性同步-- 当MysqlBuild生成的类属性在客户端被修改后同步上来会调用此方法
        /// </summary>
        /// <param name="model"></param>
        public virtual void OnSyncPropertyHandler(RPCModel model) { }
        #endregion

        /// <summary>
        /// 此方法需要自己实现, 实现内容如下: <see langword="xxServer.Instance.RemoveClient(this);"/>
        /// </summary>
        public virtual void Close() { }

        public override string ToString()
        {
            return $"玩家ID:{PlayerID} 用户ID:{UserID} IP:{RemotePoint} 场景ID:{SceneName} 登录:{Login}";
        }
    }
}
namespace Net.Share
{
    using global::System;

    /// <summary>
    /// 网络客户端接口处理 2019.7.9
    /// </summary>
    public interface INetClient
    {
        /// <summary>
        /// 当连接服务器成功事件
        /// </summary>
        Action OnConnectedHandle { get; set; }
        /// <summary>
        /// 当连接失败事件
        /// </summary>
        Action OnConnectFailedHandle { get; set; }
        /// <summary>
        /// 当尝试连接服务器事件
        /// </summary>
        Action OnTryToConnectHandle { get; set; }
        /// <summary>
        /// 当连接中断 (异常) 事件
        /// </summary>
        Action OnConnectLostHandle { get; set; }
        /// <summary>
        /// 当断开连接事件
        /// </summary>
        Action OnDisconnectHandle { get; set; }
        /// <summary>
        /// 当接收到网络数据处理事件
        /// </summary>
        Action<RPCModel> OnReceiveDataHandle { get; set; }
        /// <summary>
        /// 当断线重连成功触发事件
        /// </summary>
        Action OnReconnectHandle { get; set; }
        /// <summary>
        /// 当关闭连接事件
        /// </summary>
        Action OnCloseConnectHandle { get; set; }
        /// <summary>
        /// 当统计网络流量时触发
        /// </summary>
        NetworkDataTraffic OnNetworkDataTraffic { get; set; }
    }
}
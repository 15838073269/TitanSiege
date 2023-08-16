using System;

namespace Net.Share
{
    /// <summary>
    /// 网络流量数据统计
    /// </summary>
    public delegate void NetworkDataTraffic(Dataflow dataflow);

    /// <summary>
    /// 当处理缓冲区数据
    /// </summary>
    /// <param name="client">处理此客户端的数据请求</param>
    /// <param name="model"></param>
    public delegate void RevdBufferHandle<Player>(Player client, RPCModel model);

    /// <summary>
    /// webSocket当处理缓冲区数据
    /// </summary>
    /// <param name="client">处理此客户端的数据请求</param>
    /// <param name="model"></param>
    public delegate void WSRevdBufferHandle<Player>(Player client, MessageModel model);

    /// <summary>
    /// 数据流量统计结构类
    /// </summary>
    public struct Dataflow
    {
        /// <summary>
        /// 每秒发送数据次数
        /// </summary>
        public int sendNumber;
        /// <summary>
        /// 每秒发送字节长度
        /// </summary>
        public int sendCount;
        /// <summary>
        /// 每秒接收数据次数
        /// </summary>
        public int receiveNumber;
        /// <summary>
        /// 每秒接收到的字节长度
        /// </summary>
        public int receiveCount;
        /// <summary>
        /// 解析RPC函数次数
        /// </summary>
        public int resolveNumber;
        /// <summary>
        /// 从启动到现在总流出的数据流量
        /// </summary>
        public long outflowTotal;
        /// <summary>
        /// 从启动到现在总流入的数据流量
        /// </summary>
        public long inflowTotal;
        /// <summary>
        /// 只适应于客户端, 服务器无效
        /// </summary>
        public int FPS;
        /// <summary>
        /// 所有网络线程组的FPS, 适用于服务器
        /// </summary>
        public FPSEntity[] FPSArray;

        /// <summary>
        /// 显示所有线程组的FPS
        /// </summary>
        /// <returns></returns>
        public string FPSToString()
        {
            var text = string.Empty;
            for (int i = 0; i < FPSArray.Length; i++)
                text += $"FPS{FPSArray[i].Id}:{FPSArray[i].FPS} ";
            return text;
        }
    }

    public struct FPSEntity 
    {
        public int Id;
        public int FPS;

        public FPSEntity(int id, int fps)
        {
            Id = id;
            FPS = fps;
        }
    }
}
using Net.System;
using System;
using System.Net;

namespace Net.Share 
{
    /// <summary>
    /// gcp可靠协议接口
    /// </summary>
    public interface IGcp
    {
        ushort MTU { get; set; }
        int RTO { get; set; }
        int MTPS { get; set; }
        FlowControlMode FlowControl { get; set; }
        Action<RTProgress> OnRevdProgress { get; set; }
        Action<RTProgress> OnSendProgress { get; set; }
        Action<byte[]> OnSender { get; set; }
        EndPoint RemotePoint { get; set; }

        /// <summary>
        /// 判断是否有发送，比如有目前有很多个数据需要发送
        /// </summary>
        /// <returns></returns>
        bool HasSend();
        /// <summary>
        /// 输入要发送的数据
        /// </summary>
        /// <param name="buffer"></param>
        void Input(byte[] buffer);
        /// <summary>
        /// 更新发送和结束事件
        /// </summary>
        void Update();
        /// <summary>
        /// 真正的接入发送接口
        /// </summary>
        /// <param name="buffer"></param>
        void Send(byte[] buffer);
        /// <summary>
        /// 检查接收是否有数据
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        int Receive(out Segment buffer);
        /// <summary>
        /// 施放gcp资源
        /// </summary>
        void Dispose();
    }
}
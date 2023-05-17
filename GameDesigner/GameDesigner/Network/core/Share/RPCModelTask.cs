using System;

namespace Net.Share
{
    /// <summary>
    /// 远程过程调用异步模型 async和await实现和委托事件处理
    /// </summary>
    public class RPCModelTask
    {
        /// <summary>
        /// 是否完成操作
        /// </summary>
        public bool IsCompleted { get; internal set; }
        /// <summary>
        /// 对方回应的数据都在这里
        /// </summary>
        public RPCModel model;
        internal bool intercept;
        internal Delegate callback;
        internal uint tick;//会有一个定时事件检查是否内存遗留
    }
}

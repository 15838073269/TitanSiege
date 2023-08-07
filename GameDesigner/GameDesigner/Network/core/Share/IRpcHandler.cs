using Net.System;
using System;
using System.Collections.Generic;

namespace Net.Share
{
    public class RPCMethodBody
    {
        public QueueSafe<RPCModelTask> CallWaitQueue = new QueueSafe<RPCModelTask>();
        public MyDictionary<object, IRPCMethod> RpcDict = new MyDictionary<object, IRPCMethod>();
        public int Count => RpcDict.Count;

        internal void Add(object key, IRPCMethod value)
        {
            RpcDict.Add(key, value);
        }

        internal void Remove(object target)
        {
            RpcDict.Remove(target);
        }
    }

    public interface IRpcHandler
    {
        /// <summary>
        /// 远程方法优化字典
        /// </summary>
        MyDictionary<string, RPCMethodBody> RpcDic { get; set; }
        /// <summary>
        /// 远程方法哈希字典
        /// </summary>
        MyDictionary<ushort, RPCMethodBody> RpcHashDic { get; set; }
        /// <summary>
        /// 已经收集过的类信息
        /// </summary>
        MyDictionary<Type, List<MemberData>> MemberInfos { get; set; }
        /// <summary>
        /// 当前收集rpc的对象信息
        /// </summary>
        MyDictionary<object, MemberDataList> RpcTargetHash { get; set; }
        /// <summary>
        /// 字段同步信息
        /// </summary>
        MyDictionary<ushort, SyncVarInfo> SyncVarDic { get; set; }
        /// <summary>
        /// Rpc任务队列
        /// </summary>
        QueueSafe<IRPCData> RpcWorkQueue { get; set; }
        /// <summary>
        /// 移除target的所有rpc
        /// </summary>
        /// <param name="target"></param>
        void RemoveRpc(object target);
    }
}
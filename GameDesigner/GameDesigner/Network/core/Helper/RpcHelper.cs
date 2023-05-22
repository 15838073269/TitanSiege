using Net.Server;
using Net.Share;
using Net.System;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Net.Helper
{
    /// <summary>
    /// 远程过程调用(RPC)帮助类
    /// </summary>
    public class RpcHelper
    {
        public static void AddRpc(IRpcHandler handle, object target, bool append, Action<SyncVarInfo> onSyncVarCollect)
        {
            AddRpc(handle, target, target.GetType(), append, onSyncVarCollect);
        }

        public static void AddRpc(IRpcHandler handle, object target, Type type, bool append, Action<SyncVarInfo> onSyncVarCollect)
        {
            AddRpc(handle, target, type, append, onSyncVarCollect, null, (member) => new RPCMethod(target, member.member as MethodInfo, member.rpc.cmd));
        }

        public static void AddRpc(IRpcHandler handle, object target, Type type, bool append, Action<SyncVarInfo> onSyncVarCollect, Action<MemberInfo, MemberData> action, Func<MemberData, IRPCMethod> func)
        {
            if (!append)
                if (handle.RpcTargetHash.ContainsKey(target))
                    return;
            if (!handle.MemberInfos.TryGetValue(type, out var list))
            {
                list = new List<MemberData>();
                var members = type.GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                foreach (var info in members)
                {
                    var data = new MemberData() { member = info };
                    if (info.MemberType == MemberTypes.Method)
                    {
                        var attributes = info.GetCustomAttributes(typeof(RPCFun), true);//兼容ILR写法
                        if (attributes.Length > 0)
                        {
                            data.rpc = attributes[0] as RPCFun;
                            action?.Invoke(info, data);
                        }
                    }
                    else if (info.MemberType == MemberTypes.Field | info.MemberType == MemberTypes.Property)
                    {
                        SyncVarHelper.InitSyncVar(info, target, (syncVar) =>
                        {
                            data.syncVar = syncVar;
                        });
                    }
                    if (data.rpc != null | data.syncVar != null)
                        list.Add(data);
                }
                handle.MemberInfos.Add(type, list);
            }
            foreach (var member in list)
            {
                var rpc = member.rpc;
                if (rpc != null)
                {
                    var item = func(member);
                    RPCMethodBody body;
                    if (rpc.hash != 0)
                    {
                        if (!handle.RpcHashDic.TryGetValue(rpc.hash, out body))
                            handle.RpcHashDic.Add(rpc.hash, body = new RPCMethodBody());
                        body.Add(target, item);
                    }
                    if (!handle.RpcDic.TryGetValue(item.method.Name, out body))
                        handle.RpcDic.Add(item.method.Name, body = new RPCMethodBody());
                    body.Add(target, item);
                }
                var syncVar = member.syncVar;
                if (syncVar != null)
                {
                    var syncVar1 = syncVar.Clone(target);
                    if (syncVar1.id == 0)
                        onSyncVarCollect?.Invoke(syncVar1);
                    else
                        handle.SyncVarDic.TryAdd(syncVar1.id, syncVar1);
                }
            }
            if (list.Count > 0)
                handle.RpcTargetHash.Add(target, new MemberDataList() { members = list });
        }

        public static void RemoveRpc(IRpcHandler handle, object target)
        {
            if (handle.RpcTargetHash.TryRemove(target, out var list))
            {
                foreach (var item in list.members)
                {
                    if (item.rpc != null)
                    {
                        if (handle.RpcHashDic.TryGetValue(item.rpc.hash, out var dict))
                        {
                            dict.Remove(target);
                        }
                        if (handle.RpcDic.TryGetValue(item.member.Name, out dict))
                        {
                            dict.Remove(target);
                        }
                    }
                    if (item.syncVar != null)
                    {
                        handle.SyncVarDic.Remove(item.syncVar.id);
                    }
                }
            }
        }

        public static void Invoke(IRpcHandler handle, NetPlayer client, RPCModel model, Action<MyDictionary<object, IRPCMethod>, NetPlayer, RPCModel> action, Action<int, NetPlayer, RPCModel> log)
        {
            RPCMethodBody body;
            if (model.methodHash != 0)
            {
                if (!handle.RpcHashDic.TryGetValue(model.methodHash, out body))
                {
                    log(0, client, model);
                    return;
                }
            }
            else
            {
                if (string.IsNullOrEmpty(model.func))
                    return;
                if (!handle.RpcDic.TryGetValue(model.func, out body))
                {
                    log(1, client, model);
                    return;
                }
            }
            uint tick = (uint)Environment.TickCount;
            while (body.TaskQueue.TryDequeue(out var modelTask))
            {
                if (tick > modelTask.tick) //超时要移出队列, 检查下一个, 否则一个超时任务把其他Call也搞得超时
                    continue;
                modelTask.model = model;
                modelTask.IsCompleted = true;
                var callback = modelTask.callback;
                if (callback != null)
                {
                    var data = new RPCData(callback.Target, callback.Method, model.pars);
                    handle.RpcWorkQueue.Enqueue(data);
                    return;
                }
                if (modelTask.intercept)
                    return;
            }
            if (body.Count <= 0)
            {
                log(2, client, model);
                return;
            }
            action(body.RpcDict, client, model);
        }
    }
}
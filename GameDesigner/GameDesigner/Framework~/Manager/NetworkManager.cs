using Net.Client;
using Net.Event;
using Net.Share;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System;
using Net.Helper;
using Cysharp.Threading.Tasks;

namespace Framework
{
    /// <summary>
    /// 传输协议
    /// </summary>
    public enum TransportProtocol
    {
        Tcp, Gcp, Udx, Kcp, Web
    }

    public enum LogMode
    {
        None,
        /// <summary>
        /// 消息输出, 警告输出, 错误输出, 三种模式各自输出
        /// </summary>
        Default,
        /// <summary>
        /// 所有消息输出都以白色消息输出
        /// </summary>
        LogAll,
        /// <summary>
        /// 警告信息和消息一起输出为白色
        /// </summary>
        LogAndWarning,
        /// <summary>
        /// 警告和错误都输出为红色提示
        /// </summary>
        WarnAndError,
    }

    /// <summary>
    /// 网络解析适配器
    /// </summary>
    public enum SerializeAdapterType
    {
        Default,//默认序列化, protobuff + json
        PB_JSON_FAST,//快速序列化 protobuff + json
        Binary,//快速序列化 需要注册远程类型
        Binary2,//极速序列化 Binary + Binary2 需要生成序列化类型, 菜单GameDesigner/Netowrk/Fast2BuildTools
        Binary3//极速序列化 需要生成序列化类型, 菜单GameDesigner/Netowrk/Fast2BuildTools
    }

    [Serializable]
    public class ClientGourp
    {
        public string name;
        public ClientBase _client;
        public TransportProtocol protocol = TransportProtocol.Gcp;
        public string ip = "127.0.0.1";
        public int port = 9543;
        public bool localTest;//本机测试
        public bool debugRpc = true;
        public bool authorize;
        public bool startConnect = true;
        public int reconnectCount = 10;
        public int reconnectInterval = 2000;
        public byte heartLimit = 5;
        public int heartInterval = 1000;
        public string scheme = "ws";

        public ClientBase Client
        {
            get
            {
                if (_client != null)
                    return _client;
                var typeName = $"Net.Client.{protocol}Client";
                var type = AssemblyHelper.GetType(typeName);
                if (type == null)
                    throw new Exception($"请导入:{protocol}协议!!!");
                _client = Activator.CreateInstance(type, new object[] { true }) as ClientBase;
                _client.host = ip;
                _client.port = port;
                _client.LogRpc = debugRpc;
                _client.ReconnectCount = reconnectCount;
                _client.ReconnectInterval = reconnectInterval;
                _client.SetHeartTime(heartLimit, heartInterval);
                _client.Scheme = scheme;
                return _client;
            }
            set { _client = value; }
        }

        public UniTask<bool> Connect()
        {
            _client = Client;
            if (!localTest)
            {
                var ips = Dns.GetHostAddresses(ip);
                if (ips.Length > 0)
                    _client.host = ips[RandomHelper.Range(0, ips.Length)].ToString();
                else
                    _client.host = ip;
            }
            else _client.host = "127.0.0.1";
            _client.port = port;
            return _client.Connect(result =>
            {
                if (result)
                {
                    _client.Send(new byte[1]);//发送一个字节:调用服务器的OnUnClientRequest方法, 如果不需要账号登录, 则会直接允许进入服务器
                }
            });
        }

        public UniTask<bool> Connect(string ip, int port)
        {
            this.ip = ip;
            this.port = port;
            return Connect();
        }
    }

    public class NetworkManager : MonoBehaviour
    {
        public LogMode logMode = LogMode.Default;
#if UNITY_2020_1_OR_NEWER
        [NonReorderable]
#endif
        public List<ClientGourp> clients = new List<ClientGourp>();

        public ClientBase this[int index]
        {
            get { return clients[index].Client; }
            set { clients[index].Client = value; }
        }

        // Use this for initialization
        void Start()
        {
            switch (logMode)
            {
                case LogMode.Default:
                    NDebug.BindLogAll(Debug.Log, Debug.LogWarning, Debug.LogError);
                    break;
                case LogMode.LogAll:
                    NDebug.BindLogAll(Debug.Log);
                    break;
                case LogMode.LogAndWarning:
                    NDebug.BindLogAll(Debug.Log, Debug.Log, Debug.LogError);
                    break;
                case LogMode.WarnAndError:
                    NDebug.BindLogAll(Debug.Log, Debug.LogError, Debug.LogError);
                    break;
            }
            foreach (var client in clients)
            {
                if (client.startConnect)
                    client.Connect();
            }
        }

        // Update is called once per frame
        void Update()
        {
            for (int i = 0; i < clients.Count; i++)
            {
                if (clients[i]._client == null)
                    continue;
                clients[i]._client.NetworkUpdate();
            }
        }

        void OnDestroy()
        {
            for (int i = 0; i < clients.Count; i++)
            {
                if (clients[i]._client == null)
                    continue;
                clients[i]._client.Close();
            }
            switch (logMode)
            {
                case LogMode.Default:
                    NDebug.RemoveLogAll(Debug.Log, Debug.LogWarning, Debug.LogError);
                    break;
                case LogMode.LogAll:
                    NDebug.RemoveLogAll(Debug.Log);
                    break;
                case LogMode.LogAndWarning:
                    NDebug.RemoveLogAll(Debug.Log, Debug.Log, Debug.LogError);
                    break;
                case LogMode.WarnAndError:
                    NDebug.RemoveLogAll(Debug.Log, Debug.LogError, Debug.LogError);
                    break;
            }
        }

        public void BindNetworkAll(INetworkHandle handle)
        {
            foreach (var item in clients)
            {
                item.Client.BindNetworkHandle(handle);
            }
        }

        /// <summary>
        /// 添加索引0的客户端rpc, 也就是1的客户端
        /// </summary>
        /// <param name="target"></param>
        public void AddRpcOne(object target)
        {
            clients[0].Client.AddRpc(target);
        }

        /// <summary>
        /// 添加索引1的客户端, 也就是2的客户端
        /// </summary>
        /// <param name="target"></param>
        public void AddRpcTwo(object target)
        {
            clients[1].Client.AddRpc(target);
        }

        /// <summary>
        /// 添加指定索引的客户端rpc, 如果索引小于0则为全部添加
        /// </summary>
        /// <param name="clientIndex"></param>
        /// <param name="target"></param>
        public void AddRpc(int clientIndex, object target)
        {
            if (clientIndex < 0)
                foreach (var item in clients)
                    item.Client.AddRpc(target);
            else clients[clientIndex].Client.AddRpc(target);
        }

        /// <summary>
        /// 移除索引0的客户端rpc, 也就是1的客户端
        /// </summary>
        /// <param name="target"></param>
        public void RemoveRpcOne(object target)
        {
            clients[0].Client.RemoveRpc(target);
        }

        /// <summary>
        /// 移除索引1的客户端rpc, 也就是2的客户端
        /// </summary>
        /// <param name="target"></param>
        public void RemoveRpcTwo(object target)
        {
            clients[1].Client.RemoveRpc(target);
        }

        /// <summary>
        /// 移除指定索引的客户端rpc, 如果索引小于0则为全部移除
        /// </summary>
        /// <param name="clientIndex"></param>
        /// <param name="target"></param>
        public void RemoveRpc(int clientIndex, object target)
        {
            if (clientIndex < 0)
                foreach (var item in clients)
                    item.Client.RemoveRpc(target);
            else clients[clientIndex].Client.RemoveRpc(target);
        }

        public void Close(bool await, int millisecondsTimeout)
        {
            foreach (var item in clients)
            {
                item.Client.Close(await, millisecondsTimeout);
            }
        }

        public void CallUnity(Action ptr)
        {
            clients[0].Client.WorkerQueue.Enqueue(ptr);
        }

        public void DispatcherRpc(ushort hash, params object[] parms)
        {
            clients[1].Client.DispatchRpc(hash, parms);
        }

        public void CallOne(string func, params object[] pars)
        {
            clients[0].Client.SendRT(func, pars);
        }

        public void CallOne(ushort hash, params object[] pars)
        {
            clients[0].Client.SendRT(hash, pars);
        }

        public void CallTwo(string func, params object[] pars)
        {
            clients[1].Client.SendRT(func, pars);
        }

        public void CallTwo(ushort hash, params object[] pars)
        {
            clients[1].Client.SendRT(hash, pars);
        }
    }
}
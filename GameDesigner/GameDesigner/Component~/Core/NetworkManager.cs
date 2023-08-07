#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_WSA || UNITY_WEBGL
using Net.Client;
using Net.Event;
using Net.Share;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System;
using Net.Helper;
using Cysharp.Threading.Tasks;

namespace Net.Component
{
    [Serializable]
    public class ClientGourp 
    {
        public string name;
        public ClientBase _client;
        public TransportProtocol protocol = TransportProtocol.Tcp;
        public string ip = "127.0.0.1";
        public int port = 9543;
#if UNITY_EDITOR
        public bool localTest;//本机测试
#endif
        public bool debugRpc = true;
        public bool authorize;
        public bool startConnect = true;
        public bool singleThread;
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
                _client.IsMultiThread = !singleThread;
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
            var ips = Dns.GetHostAddresses(ip);
            if (ips.Length > 0)
                _client.host = ips[RandomHelper.Range(0, ips.Length)].ToString();
            else
                _client.host = ip;
#if UNITY_EDITOR
            if (localTest) _client.host = "127.0.0.1";
#endif
            _client.port = port;
            //switch (type)
            //{
            //    case SerializeAdapterType.Default:
            //        break;
            //    case SerializeAdapterType.PB_JSON_FAST:
            //        _client.AddAdapter(new Adapter.SerializeFastAdapter() { IsEncrypt = isEncrypt, Password = password });
            //        break;
            //    case SerializeAdapterType.Binary:
            //        _client.AddAdapter(new Adapter.SerializeAdapter() { IsEncrypt = isEncrypt, Password = password });
            //        break;
            //    case SerializeAdapterType.Binary2:
            //        _client.AddAdapter(new Adapter.SerializeAdapter2() { IsEncrypt = isEncrypt, Password = password });
            //        break;
            //    case SerializeAdapterType.Binary3:
            //        _client.AddAdapter(new Adapter.SerializeAdapter3() { IsEncrypt = isEncrypt, Password = password });
            //        break;
            //}
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

    public class NetworkManager : SingleCase<NetworkManager>
    {
        public LogMode logMode = LogMode.Default;
        public bool dontDestroyOnLoad = true;
#if UNITY_2020_1_OR_NEWER
        [NonReorderable]
#endif
        public List<ClientGourp> clients = new List<ClientGourp>();

        public ClientBase this[int index]
        {
            get { return clients[index].Client; }
            set { clients[index].Client = value; }
        }

        protected override void Awake()
        {
            base.Awake();
            if (dontDestroyOnLoad) DontDestroyOnLoad(gameObject);
            Application.runInBackground = true;
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
                case LogMode.OnlyError:
                    NDebug.BindLogAll(null, null, Debug.LogError);
                    break;
                case LogMode.OnlyWarnAndError:
                    NDebug.BindLogAll(null, Debug.LogError, Debug.LogError);
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
                case LogMode.OnlyError:
                    NDebug.RemoveLogAll(null, null, Debug.LogError);
                    break;
                case LogMode.OnlyWarnAndError:
                    NDebug.RemoveLogAll(null, Debug.LogError, Debug.LogError);
                    break;
            }
        }

        public static void BindNetworkAll(INetworkHandle handle)
        {
            foreach (var item in I.clients)
            {
                item.Client.BindNetworkHandle(handle);
            }
        }

        /// <summary>
        /// 添加索引0的客户端rpc, 也就是1的客户端
        /// </summary>
        /// <param name="target"></param>
        public static void AddRpcOne(object target)
        {
            I.clients[0].Client.AddRpc(target);
        }

        /// <summary>
        /// 添加索引1的客户端, 也就是2的客户端
        /// </summary>
        /// <param name="target"></param>
        public static void AddRpcTwo(object target)
        {
            I.clients[1].Client.AddRpc(target);
        }

        /// <summary>
        /// 添加指定索引的客户端rpc, 如果索引小于0则为全部添加
        /// </summary>
        /// <param name="clientIndex"></param>
        /// <param name="target"></param>
        public static void AddRpc(int clientIndex, object target)
        {
            if (clientIndex < 0)
                foreach (var item in I.clients)
                    item.Client.AddRpc(target);
            else I.clients[clientIndex].Client.AddRpc(target);
        }

        /// <summary>
        /// 移除索引0的客户端rpc, 也就是1的客户端
        /// </summary>
        /// <param name="target"></param>
        public static void RemoveRpcOne(object target)
        {
            I.clients[0].Client.RemoveRpc(target);
        }

        /// <summary>
        /// 移除索引1的客户端rpc, 也就是2的客户端
        /// </summary>
        /// <param name="target"></param>
        public static void RemoveRpcTwo(object target)
        {
            var i = Instance;
            if (i == null)
                return;
            i.clients[1].Client.RemoveRpc(target);
        }

        /// <summary>
        /// 移除指定索引的客户端rpc, 如果索引小于0则为全部添加
        /// </summary>
        /// <param name="clientIndex"></param>
        /// <param name="target"></param>
        public static void RemoveRpc(int clientIndex, object target)
        {
            var i = Instance;
            if (i == null)
                return;
            if (clientIndex < 0)
                foreach (var item in i.clients)
                    item.Client.RemoveRpc(target);
            else i.clients[clientIndex].Client.RemoveRpc(target);
        }

        public static void Close(bool v1, int v2)
        {
            foreach (var item in I.clients)
            {
                item.Client.Close(v1, v2);
            }
        }

        public static void CallUnity(Action ptr)
        {
            I.clients[0].Client.WorkerQueue.Enqueue(ptr);
        }

        public static void DispatcherRpc(ushort hash, params object[] parms)
        {
            I.clients[1].Client.DispatchRpc(hash, parms);
        }
    }
}
#endif
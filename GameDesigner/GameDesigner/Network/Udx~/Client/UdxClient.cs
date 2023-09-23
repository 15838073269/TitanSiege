namespace Net.Client
{
    using global::System;
    using global::System.Collections.Generic;
    using global::System.Runtime.InteropServices;
    using global::System.Threading;
    using global::System.Threading.Tasks;
#if SERVICE
    using global::System.IO;
    using global::System.Net.Sockets;
    using global::System.Collections.Concurrent;
#endif
    using Cysharp.Threading.Tasks;
    using Udx;
    using Net.System;
    using Net.Event;
    using Net.Share;
    using AOT;

    /// <summary>
    /// udx客户端类型 -> 只能300人以下连接, 如果想要300个客户端以上, 请进入udx网址:www.goodudx.com 联系作者下载专业版FastUdxApi.dll, 然后更换下框架内的FastUdxApi.dll即可
    /// 第三版本 2020.9.14
    /// </summary>
    public class UdxClient : ClientBase
    {
        protected IntPtr udxObj;
        protected IntPtr ClientPtr;
        protected UDXPRC udxPrc;

        /// <summary>
        /// 构造可靠传输客户端
        /// </summary>
        public UdxClient()
        {
        }

        /// <summary>
        /// 构造可靠传输客户端
        /// </summary>
        /// <param name="useUnityThread">使用unity多线程?</param>
        public UdxClient(bool useUnityThread) : this()
        {
            UseUnityThread = useUnityThread;
        }

        ~UdxClient()
        {
#if !UNITY_EDITOR
            Close();
#endif
        }

        public override UniTask<bool> Connect(string host, int port, int localPort, Action<bool> result)
        {
            if (!UdxLib.INIT)
            {
                UdxLib.INIT = true;
#if SERVICE && WINDOWS
                string path = AppDomain.CurrentDomain.BaseDirectory;
                if (!File.Exists(path + "\\udxapi.dll"))
                    throw new FileNotFoundException($"udxapi.dll没有在程序根目录中! 请从GameDesigner文件夹下找到 udxapi.dll复制到{path}目录下.");
#endif
                UdxLib.UInit(1);
                UdxLib.UEnableLog(false);
            }
            return base.Connect(host, port, localPort, result);
        }

        protected override UniTask<bool> ConnectResult(string host, int port, int localPort, Action<bool> result)
        {
            try
            {
                ReleaseUdx();
                udxObj = UdxLib.UCreateFUObj();
                UdxLib.UBind(udxObj, null, 0);
                udxPrc = new UDXPRC(ProcessReceive);
                UdxLib.USetFUCB(udxObj, udxPrc);
                GC.KeepAlive(udxPrc);
                if (host == "127.0.0.1" | host == "localhost")
                    host = NetPort.GetIP();
                ClientPtr = UdxLib.UConnect(udxObj, host, port, 0, false, 0);
                if (ClientPtr != IntPtr.Zero) 
                {
                    UdxLib.UDump(ClientPtr);
                    var handle = GCHandle.Alloc(this);
                    var user = GCHandle.ToIntPtr(handle);
                    UdxLib.USetUserData(ClientPtr, user.ToInt64());
                }
#if SERVICE
                return UniTask.Run(() =>
#else
                return UniTask.RunOnThreadPool(() =>
#endif
                {
                    try
                    {
                        var timeout = DateTime.Now.AddSeconds(5);
                        while (!Connected & DateTime.Now < timeout) { Thread.Sleep(1); }
                        if (Connected)
                            StartupThread();
                        timeout = DateTime.Now.AddSeconds(3);
                        while (Connected & UID == 0 & DateTime.Now < timeout)
                        {
                            Send(NetCmd.Identify);
                            Thread.Sleep(200);
                            if (!openClient)
                                throw new Exception("客户端调用Close!");
                        }
                        if (UID == 0)
                            throw new Exception("uid赋值失败!");
                        result(Connected);
                        return Connected;
                    }
                    catch (Exception ex)
                    {
                        ReleaseUdx();
                        NDebug.LogError("连接失败原因:" + ex.ToString());
                        Connected = false;
                        Client?.Close();
                        Client = null;
                        result(false);
                        return false;
                    }
                });
            }
            catch (Exception ex)
            {
                ReleaseUdx();
                NDebug.Log("连接错误: " + ex.ToString());
                result(false);
                return UniTask.FromResult(false);
            }
        }

        public override void ReceiveHandler()
        {
        }

        //IL2CPP使用Marshal.GetFunctionPointerForDelegate需要设置委托方法为静态方法，并且要添加上特性MonoPInvokeCallback
        [MonoPInvokeCallback(typeof(UDXPRC))]
        protected static void ProcessReceive(UDXEVENT_TYPE type, int erro, IntPtr cli, IntPtr pData, int len)//cb回调
        {
            try
            {
                var user = UdxLib.UGetUserData(cli);
                var handle = GCHandle.FromIntPtr(new IntPtr(user));
                var client = handle.Target as UdxClient;
                client.heart = 0;
                switch (type)
                {
                    case UDXEVENT_TYPE.E_CONNECT:
                        if (erro != 0)
                            return;
                        client.ClientPtr = cli;
                        client.Connected = true;
                        UdxLib.USetGameMode(cli, true);
                        break;
                    case UDXEVENT_TYPE.E_LINKBROKEN:
                        client.Connected = false;
                        client.NetworkState = NetworkState.ConnectLost;
                        client.InvokeInMainThread(client.OnConnectLostHandle);
                        client.RpcModels = new QueueSafe<RPCModel>();
                        client.ReleaseUdx();
                        handle.Free();
                        NDebug.Log("断开连接！");
                        break;
                    case UDXEVENT_TYPE.E_DATAREAD:
                        var buffer = BufferPool.Take(len);
                        buffer.Count = len;
                        Marshal.Copy(pData, buffer.Buffer, 0, len);
                        client.receiveCount += len;
                        client.receiveAmount++;
                        client.ResolveBuffer(ref buffer, false);
                        BufferPool.Push(buffer);
                        break;
                }
            }
            catch (Exception ex)
            {
                NDebug.LogError("处理异常:" + ex);
            }
        }

        protected override bool HeartHandler()
        {
            try
            {
                if (++heart <= HeartLimit)
                    return true;
                if (Connected & heart < HeartLimit + 5)
                    Send(NetCmd.SendHeartbeat, new byte[0]);
                else if (!Connected)//尝试连接执行
                    InternalReconnection();
            }
            catch { }
            return openClient & CurrReconnect < ReconnectCount;
        }

        protected unsafe override void SendByteData(byte[] buffer)
        {
            if (ClientPtr == IntPtr.Zero)
                return;
            sendAmount++;
            sendCount += buffer.Length;
            fixed (byte* ptr = buffer)
            {
                int count = UdxLib.USend(ClientPtr, ptr, buffer.Length);
                if (count <= 0)
                    OnSendErrorHandle?.Invoke(buffer);
            }
        }

        public override void Close(bool await = true, int millisecondsTimeout = 100)
        {
            var isDispose = openClient;
            Connected = false;
            openClient = false;
            NetworkState = NetworkState.ConnectClosed;
            InvokeInMainThread(OnCloseConnectHandle);
            if (await) Thread.Sleep(millisecondsTimeout);//给update线程一秒的时间处理关闭事件
            AbortedThread();
            StackStream?.Close();
            StackStream = null;
            stack = 0;
            UID = 0;
            PreUserId = 0;
            CurrReconnect = 0;
            if (Instance == this) Instance = null;
            if (Gcp != null) Gcp.Dispose();
            ReleaseUdx();
            if (isDispose) NDebug.Log("客户端已关闭！");
        }

        protected void ReleaseUdx()
        {
            if (ClientPtr != IntPtr.Zero)
            {
                UdxLib.UClose(ClientPtr);
                UdxLib.UUndump(ClientPtr);
                ClientPtr = IntPtr.Zero;
            }
            if (udxObj != IntPtr.Zero)
            {
                UdxLib.UDestroyFUObj(udxObj);
                udxObj = IntPtr.Zero;
            }
        }

        public static CancellationTokenSource Testing(string ip, int port, int clientLen, int dataLen, int millisecondsTimeout, Action<UdxClientTest> onInit = null, Action<List<UdxClientTest>> fpsAct = null, IAdapter adapter = null)
        {
            if (!UdxLib.INIT)
            {
                UdxLib.INIT = true;
#if SERVICE
                string path = AppDomain.CurrentDomain.BaseDirectory;
                if (!File.Exists(path + "\\udxapi.dll"))
                    throw new FileNotFoundException($"udxapi.dll没有在程序根目录中! 请从GameDesigner文件夹下找到 udxapi.dll复制到{path}目录下.");
#endif
                UdxLib.UInit(8);
                UdxLib.UEnableLog(false);
            }
            var cts = new CancellationTokenSource();
            Task.Run(() =>
            {
                var clients = new List<UdxClientTest>();
                for (int i = 0; i < clientLen; i++)
                {
                    var client = new UdxClientTest();
                    onInit?.Invoke(client);
                    if (adapter != null)
                        client.AddAdapter(adapter);
                    try
                    {
                        client.Connect(ip, port);
                    }
                    catch (Exception ex)
                    {
                        NDebug.LogError(ex);
                        return;
                    }
                    clients.Add(client);
                }
                var buffer = new byte[dataLen];
                Task.Run(() =>
                {
                    for (int i = 0; i < clients.Count; i++)
                    {
                        var client = clients[i];
                        client.OnPingCallback += (d)=> 
                        {
                            client.delay = d;
                        };
                    }
                    while (!cts.IsCancellationRequested)
                    {
                        Thread.Sleep(1000);
                        fpsAct?.Invoke(clients);
                        for (int i = 0; i < clients.Count; i++)
                        {
                            clients[i].Ping();
                        }
                    }
                });
                int threadNum = (clientLen / 1000) + 1;
                for (int i = 0; i < threadNum; i++)
                {
                    int index = i * 1000;
                    int end = index + 1000;
                    if (index >= clientLen)
                        break;
                    Task.Run(() =>
                    {
                        if (end > clientLen)
                            end = clientLen;
                        var tick = Environment.TickCount + millisecondsTimeout;
                        while (!cts.IsCancellationRequested)
                        {
                            bool canSend = false;
                            if (Environment.TickCount >= tick)
                            {
                                tick = Environment.TickCount + millisecondsTimeout;
                                canSend = true;
                            }
                            for (int ii = index; ii < end; ii++)
                            {
                                try
                                {
                                    var client = clients[ii];
                                    if (canSend)
                                    {
                                        //client.SendRT(NetCmd.Local, buffer);
                                        client.AddOperation(new Operation(66, buffer));
                                    }
                                    client.NetworkTick();
                                }
                                catch (Exception ex)
                                {
                                    NDebug.LogError(ex);
                                }
                            }
                        }
                    });
                }
                while (!cts.IsCancellationRequested)
                    Thread.Sleep(30);
                Thread.Sleep(100);
                for (int i = 0; i < clients.Count; i++)
                    clients[i].Close(false);
            }, cts.Token);
            return cts;
        }
    }

    public class UdxClientTest : UdxClient
    {
        public int revdSize { get { return receiveCount; } }
        public int sendSize { get { return sendCount; } }
        public int sendNum { get { return sendAmount; } }
        public int revdNum { get { return receiveAmount; } }
        public int resolveNum { get { return resolveAmount; } }
        public uint delay { get; set; }

        public UdxClientTest()
        {
        }
        //protected override UniTask<bool> ConnectResult(string host, int port, int localPort, Action<bool> result)
        //{
        //    ReleaseUdx();
        //    udxObj = UdxLib.UCreateFUObj();
        //    UdxLib.UBind(udxObj, null, 0);
        //    udxPrc = new UDXPRC(ProcessReceive);
        //    UdxLib.USetFUCB(udxObj, udxPrc);
        //    GC.KeepAlive(udxPrc);
        //    if (host == "127.0.0.1" | host == "localhost")
        //        host = NetPort.GetIP();
        //    ClientPtr = UdxLib.UConnect(udxObj, host, port, 0, false, 0);
        //    if (ClientPtr != IntPtr.Zero)
        //    {
        //        UdxLib.UDump(ClientPtr);
        //        var handle = GCHandle.Alloc(this);
        //        var user = GCHandle.ToIntPtr(handle);
        //        UdxLib.USetUserData(ClientPtr, user.ToInt64());
        //    }
        //    return UniTask.FromResult(true);
        //}
        protected override void StartupThread() { }

        public override string ToString()
        {
            return $"uid:{UID} conv:{Connected}";
        }
    }
}
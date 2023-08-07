namespace Net.Client
{
    using global::System;
    using global::System.IO;
    using global::System.Net.Sockets;
    using global::System.Runtime.InteropServices;
    using global::System.Threading;
    using global::System.Threading.Tasks;
    using global::System.Net;
    using global::System.Reflection;
    using global::System.Collections.Generic;
    using Kcp;
    using AOT;
    using Net.Share;
    using Net.System;
    using Net.Event;
    using Cysharp.Threading.Tasks;
    using static Kcp.KcpLib;

    /// <summary>
    /// kcp客户端
    /// </summary>
    [Serializable]
    public unsafe class KcpClient : ClientBase
    {
        private IntPtr kcp;
        private IntPtr user;
        private outputCallback output;

        public KcpClient() : base()
        {
        }

        public KcpClient(bool useUnityThread) : this()
        {
            UseUnityThread = useUnityThread;
        }

        ~KcpClient()
        {
            ReleaseKcp();
        }

        protected override UniTask<bool> ConnectResult(string host, int port, int localPort, Action<bool> result) 
        {
            ReleaseKcp();
            user = Marshal.GetIUnknownForObject(this);
            kcp = ikcp_create(MTU, user);
            output = new outputCallback(Output);
            var outputPtr = Marshal.GetFunctionPointerForDelegate(output);
            ikcp_setoutput(kcp, outputPtr);
            ikcp_wndsize(kcp, ushort.MaxValue, ushort.MaxValue);
            ikcp_nodelay(kcp, 1, 10, 2, 1);
            return base.ConnectResult(host, port, localPort, result);
        }

        private byte[] addressBuffer;
        internal byte[] RemoteAddressBuffer()
        {
            if (addressBuffer != null)
                return addressBuffer;
            var socketAddress = Client.RemoteEndPoint.Serialize();
            addressBuffer = (byte[])socketAddress.GetType().GetField("m_Buffer", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(socketAddress);
            return addressBuffer;
        }

        //IL2CPP使用Marshal.GetFunctionPointerForDelegate需要设置委托方法为静态方法，并且要添加上特性MonoPInvokeCallback
        [MonoPInvokeCallback(typeof(outputCallback))]
        public static unsafe int Output(IntPtr buf, int len, IntPtr kcp, IntPtr user)
        {
            var client = Marshal.GetObjectForIUnknown(user) as KcpClient;
            client.sendCount += len;
            client.sendAmount++;
#if WINDOWS
            return Win32KernelAPI.sendto(client.Client.Handle, (byte*)buf, len, SocketFlags.None, client.RemoteAddressBuffer(), 16);
#else
            var buff = new byte[len];
            Marshal.Copy(buf, buff, 0, len);
            return client.Client.Send(buff, 0, len, SocketFlags.None);
#endif
        }

        public override void ReceiveHandler()
        {
            if (Client.Poll(0, SelectMode.SelectRead))
            {
                var segment = BufferPool.Take(65507);
                segment.Count = Client.Receive(segment.Buffer, 0, segment.Length, SocketFlags.None, out SocketError error);
                if (error != SocketError.Success | segment.Count == 0)
                {
                    BufferPool.Push(segment);
                    return;
                }
                receiveCount += segment.Count;
                receiveAmount++;
                heart = 0;
                fixed (byte* p = &segment.Buffer[0])
                    ikcp_input(kcp, p, segment.Count);
                BufferPool.Push(segment);
            }
            int len;
            if ((len = ikcp_peeksize(kcp)) > 0)
            {
                var segment1 = BufferPool.Take(len);
                fixed (byte* p1 = &segment1.Buffer[0])
                {
                    segment1.Count = ikcp_recv(kcp, p1, len);
                    ResolveBuffer(ref segment1, false);
                    BufferPool.Push(segment1);
                }
            }
        }

        public override void OnNetworkTick()
        {
            ikcp_update(kcp, (uint)Environment.TickCount);
        }

        protected override void SendByteData(byte[] buffer)
        {
            fixed (byte* p = &buffer[0])
            {
                int count = ikcp_send(kcp, p, buffer.Length);
                if (count < 0)
                    OnSendErrorHandle?.Invoke(buffer);
            }
        }

        public override void Close(bool await = true, int millisecondsTimeout = 100)
        {
            base.Close(await);
            addressBuffer = null;
            ReleaseKcp();
        }

        private void ReleaseKcp()
        {
            if (kcp != IntPtr.Zero)
            {
                ikcp_release(kcp);
                kcp = IntPtr.Zero;
            }
            if (user != IntPtr.Zero)
            {
                Marshal.Release(user);
                user = IntPtr.Zero;
            }
        }

        /// <summary>
        /// udp压力测试
        /// </summary>
        /// <param name="ip">服务器ip</param>
        /// <param name="port">服务器端口</param>
        /// <param name="clientLen">测试客户端数量</param>
        /// <param name="dataLen">每个客户端数据大小</param>
        public static CancellationTokenSource Testing(string ip, int port, int clientLen, int dataLen, int millisecondsTimeout, Action<KcpClientTest> onInit = null, Action<List<KcpClientTest>> fpsAct = null, IAdapter adapter = null)
        {
            var cts = new CancellationTokenSource();
            Task.Run(() =>
            {
                var clients = new List<KcpClientTest>();
                for (int i = 0; i < clientLen; i++)
                {
                    var client = new KcpClientTest();
                    onInit?.Invoke(client);
                    if (adapter != null)
                        client.AddAdapter(adapter);
                    client.Connect(ip, port);
                    clients.Add(client);
                }
                var buffer = new byte[dataLen];
                Task.Run(() =>
                {
                    for (int i = 0; i < clients.Count; i++)
                    {
                        var client = clients[i];
                        client.OnPingCallback += (d) =>
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
                                        client.AddOperation(new Operation(NetCmd.Local, buffer));
                                    }
                                    client.Update();
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

    public class KcpClientTest : KcpClient
    {
        public int revdSize { get { return receiveCount; } }
        public int sendSize { get { return sendCount; } }
        public int sendNum { get { return sendAmount; } }
        public int revdNum { get { return receiveAmount; } }
        public int resolveNum { get { return resolveAmount; } }
        public uint delay { get; internal set; }

        private byte[] addressBuffer;
        public KcpClientTest() : base()
        {
        }
        protected override UniTask<bool> ConnectResult(string host, int port, int localPort, Action<bool> result)
        {
            Client = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            this.localPort = localPort;
            Client.Connect(host, port);
            Client.Blocking = false;
#if WINDOWS
            var socketAddress = Client.RemoteEndPoint.Serialize();
            addressBuffer = (byte[])socketAddress.GetType().GetField("m_Buffer", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(socketAddress);
#endif
            RpcModels.Enqueue(new RPCModel(NetCmd.Connect, new byte[0]));
            SendDirect();
            Connected = true;
            result(true);
            return UniTask.FromResult(Connected);
        }
        protected override void StartupThread() { }
        /// <summary>
        /// 单线程更新，需要开发者自动调用更新
        /// </summary>
        public void Update()
        {
            if (!Connected)
                return;
            NetworkTick();
        }
        public override string ToString()
        {
            return $"uid:{UID} conv:{Connected}";
        }
    }
}

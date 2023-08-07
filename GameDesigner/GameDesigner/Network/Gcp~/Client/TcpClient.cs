namespace Net.Client
{
    using global::System;
    using global::System.Collections.Generic;
    using global::System.IO;
    using global::System.Net;
    using global::System.Net.Sockets;
    using global::System.Threading;
    using global::System.Threading.Tasks;
    using Net.Share;
    using Net.System;
    using Net.Event;
    using Net.Helper;
    using Cysharp.Threading.Tasks;

    /// <summary>
    /// TCP客户端类型 
    /// 第三版本 2020.9.14
    /// </summary>
    [Serializable]
    public class TcpClient : ClientBase
    {
        public override int HeartInterval { get; set; } = 1000 * 60 * 10;//10分钟跳一次
        public override byte HeartLimit { get; set; } = 2;//确认两次

        /// <summary>
        /// 构造可靠传输客户端
        /// </summary>
        public TcpClient()
        {
        }

        /// <summary>
        /// 构造不可靠传输客户端
        /// </summary>
        /// <param name="useUnityThread">使用unity多线程?</param>
        public TcpClient(bool useUnityThread) : this()
        {
            UseUnityThread = useUnityThread;
        }

        ~TcpClient()
        {
#if !UNITY_EDITOR
            Close();
#endif
        }

        protected override UniTask<bool> ConnectResult(string host, int port, int localPort, Action<bool> result)
        {
#if SERVICE
            return UniTask.Run(() =>
#else
            return UniTask.RunOnThreadPool(() =>
#endif
            {
                try
                {
                    Client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    this.localPort = localPort;
                    Client.NoDelay = true;
                    if (localPort != -1)
                        Client.Bind(new IPEndPoint(IPAddress.Any, localPort));
                    Client.Connect(host, port);
                    var segment = BufferPool.Take();
                    segment.Write(PreUserId);
                    Client.Send(segment.ToArray(true));
                    var tick = Environment.TickCount + 8000;
                    while (UID == 0)
                    {
                        NetworkTick();
                        if (Environment.TickCount >= tick)
                            throw new Exception("uid赋值失败!连接超时处理");
                        if (!openClient)
                            throw new Exception("客户端调用Close!");
                    }
                    StackStream = new MemoryStream(Config.Config.BaseCapacity);
                    StartupThread();
                    result(true);
                    return true;
                }
                catch(Exception ex)
                {
                    NDebug.LogError("连接错误:" + ex);
                    Connected = false;
                    Client?.Close();
                    Client = null;
                    result(false);
                    return false;
                }
            });
        }

        protected override bool HeartHandler()
        {
            try
            {
                if (++heart <= HeartLimit)
                    return true;
                if (!Connected)
                    Reconnection();
                else
                    Send(NetCmd.SendHeartbeat, new byte[0]);
            }
            catch
            {
            }
            return openClient & CurrReconnect < ReconnectCount;
        }

        protected override byte[] PackData(ISegment stream)
        {
            stream.Flush(false);
            SetDataHead(stream);
            PackageAdapter.Pack(stream);
            var len = stream.Count - frame;
            var lenBytes = BitConverter.GetBytes(len);
            var crc = CRCHelper.CRC8(lenBytes, 0, lenBytes.Length);
            stream.Position = 0;
            stream.Write(lenBytes, 0, 4);
            stream.WriteByte(crc);
            stream.Position += len;
            return stream.ToArray();
        }

        protected override void SendByteData(byte[] buffer)
        {
            sendCount += buffer.Length;
            sendAmount++;
            if (Client.Poll(0, SelectMode.SelectWrite))
            {
                int count = Client.Send(buffer, 0, buffer.Length, SocketFlags.None);
                if (count <= 0)
                    OnSendErrorHandle?.Invoke(buffer);
                else if (count != buffer.Length)
                    NDebug.LogError($"发送了{buffer.Length - count}个字节失败!");
            }
            else
            {
                NDebug.LogError("发送窗口已满,等待对方接收中!");
            }
        }

        protected override void ResolveBuffer(ref ISegment buffer, bool isTcp)
        {
            heart = 0;
            if (stack > 0)
            {
                stack++;
                StackStream.Seek(stackIndex, SeekOrigin.Begin);
                int size = buffer.Count - buffer.Position;
                stackIndex += size;
                StackStream.Write(buffer.Buffer, buffer.Position, size);
                if (stackIndex < stackCount)
                {
                    InvokeRevdRTProgress(stackIndex, stackCount);
                    return;
                }
                var count = (int)StackStream.Position;//.Length; //错误问题,不能用length, 这是文件总长度, 之前可能已经有很大一波数据
                BufferPool.Push(buffer);//要回收掉, 否则会提示内存泄露
                buffer = BufferPool.Take(count);//ref 才不会导致提示内存泄露
                StackStream.Seek(0, SeekOrigin.Begin);
                StackStream.Read(buffer.Buffer, 0, count);
                buffer.Count = count;
            }
            while (buffer.Position < buffer.Count)
            {
                if (buffer.Position + frame > buffer.Count)//流数据偶尔小于frame头部字节
                {
                    var position = buffer.Position;
                    var count = buffer.Count - position;
                    stackIndex = count;
                    stackCount = 0;
                    StackStream.Seek(0, SeekOrigin.Begin);
                    StackStream.Write(buffer.Buffer, position, count);
                    stack++;
                    break;
                }
                var lenBytes = buffer.Read(4);
                var crcCode = buffer.ReadByte();//CRC检验索引
                var retVal = CRCHelper.CRC8(lenBytes, 0, 4);
                if (crcCode != retVal)
                {
                    stack = 0;
                    NDebug.LogError($"[{UID}]CRC校验失败!");
                    return;
                }
                var size = BitConverter.ToInt32(lenBytes, 0);
                if (size < 0 | size > PackageSize)//如果出现解析的数据包大小有问题，则不处理
                {
                    stack = 0;
                    NDebug.LogError($"[{UID}]数据被拦截修改或数据量太大: size:{size}，如果想传输大数据，请设置PackageSize属性");
                    return;
                }
                if (buffer.Position + size <= buffer.Count)
                {
                    stack = 0;
                    var count = buffer.Count;//此长度可能会有连续的数据(粘包)
                    buffer.Count = buffer.Position + size;//需要指定一个完整的数据长度给内部解析
                    base.ResolveBuffer(ref buffer, true);
                    buffer.Count = count;//解析完成后再赋值原来的总长
                }
                else
                {
                    var position = buffer.Position - frame;
                    var count = buffer.Count - position;
                    stackIndex = count;
                    stackCount = size;
                    StackStream.Seek(0, SeekOrigin.Begin);
                    StackStream.Write(buffer.Buffer, position, count);
                    stack++;
                    break;
                }
            }
        }

        public override void Close(bool await = true, int millisecondsTimeout = 100)
        {
            SendRT(NetCmd.Disconnect, new byte[0]);
            SendDirect();
            Connected = false;
            openClient = false;
            NetworkState = NetworkState.ConnectClosed;
            InvokeInMainThread(OnCloseConnectHandle);
            if (await) Thread.Sleep(millisecondsTimeout);//给update线程一秒的时间处理关闭事件
            AbortedThread();
            Client?.Close();
            Client = null;
            StackStream?.Close();
            StackStream = null;
            stack = 0;
            stackIndex = 0;
            stackCount = 0;
            UID = 0;
            CurrReconnect = 0;
            if (Instance == this) Instance = null;
            if (Gcp != null) Gcp.Dispose();
            NDebug.Log("客户端已关闭！");
        }

        /// <summary>
        /// tcp压力测试
        /// </summary>
        /// <param name="ip">服务器ip</param>
        /// <param name="port">服务器端口</param>
        /// <param name="clientLen">测试客户端数量</param>
        /// <param name="dataLen">每个客户端数据大小</param>
        public static CancellationTokenSource Testing(string ip, int port, int clientLen, int dataLen, int millisecondsTimeout, Action<TcpClientTest> onInit = null, Action<List<TcpClientTest>> fpsAct = null, IAdapter adapter = null)
        {
            var cts = new CancellationTokenSource();
            Task.Run(() =>
            {
                var clients = new List<TcpClientTest>();
                for (int i = 0; i < clientLen; i++)
                {
                    var client = new TcpClientTest();
                    onInit?.Invoke(client);
                    if(adapter != null)
                        client.AddAdapter(adapter);
                    try { 
                        client.Connect(ip, port);
                    } catch (Exception ex) {
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
                        var timer = new TimerTick();
                        while (!cts.IsCancellationRequested)
                        {
                            bool canSend = false;
                            var tick = (uint)Environment.TickCount;
                            if (timer.CheckTimeout(tick, (uint)millisecondsTimeout, true))
                            {
                                canSend = true;
                            }
                            for (int ii = index; ii < end; ii++)
                            {
                                try
                                {
                                    var client = clients[ii];
                                    if (client.Client == null)
                                        continue;
                                    if (!client.Client.Connected)
                                        continue;
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

    public class TcpClientTest : TcpClient
    {
        public int revdSize { get { return receiveCount; } }
        public int sendSize { get { return sendCount; } }
        public int sendNum { get { return sendAmount; } }
        public int revdNum { get { return receiveAmount; } }
        public int resolveNum { get { return resolveAmount; } }
        public uint delay { get; internal set; }

        public TcpClientTest()
        {
        }
        protected override UniTask<bool> ConnectResult(string host, int port, int localPort, Action<bool> result)
        {
            Client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this.localPort = localPort;
            Client.Connect(host, port);
            Client.Blocking = false;
            Client.NoDelay = true;
            var segment = BufferPool.Take();
            segment.Write(PreUserId);
            Client.Send(segment.ToArray(true));
            Connected = true;
            StackStream = new MemoryStream(Config.Config.BaseCapacity);
            return UniTask.FromResult(Connected);
        }
        protected override void StartupThread() { }
        protected unsafe override void SendByteData(byte[] buffer)
        {
            sendCount += buffer.Length;
            sendAmount++;
#if WINDOWS
            fixed (byte* ptr = buffer)
                Win32KernelAPI.send(Client.Handle, ptr, buffer.Length, SocketFlags.None);
#else
                Client.Send(buffer, 0, buffer.Length, SocketFlags.None);
#endif
        }
        public override string ToString()
        {
            return $"uid:{UID} conv:{Connected}";
        }
    }
}
namespace Net.Server
{
    using global::System;
    using global::System.Collections.Generic;
    using global::System.Net;
    using global::System.Net.Sockets;
    using global::System.Threading;
    using Net.Share;
    using Net.System;
    using Debug = Event.NDebug;

    internal class UserToken<Player> where Player : NetPlayer
    {
        internal Player client;
        internal ISegment segment;
    }

    /// <summary>
    /// tcp 输入输出完成端口服务器
    /// <para>Player:当有客户端连接服务器就会创建一个Player对象出来, Player对象和XXXClient是对等端, 每当有数据处理都会通知Player对象. </para>
    /// <para>Scene:你可以定义自己的场景类型, 比如帧同步场景处理, mmorpg场景什么处理, 可以重写Scene的Update等等方法实现每个场景的更新和处理. </para>
    /// </summary>
    public class TcpServerIocp<Player, Scene> : TcpServer<Player, Scene> where Player : NetPlayer, new() where Scene : NetScene<Player>, new()
    {
        protected override void AcceptHander(Player client)
        {
            client.ReceiveArgs = new SocketAsyncEventArgs();
            var userToken = new UserToken<Player>() { client = client, segment = BufferPool.Take() };
            client.ReceiveArgs.UserToken = userToken;
            client.ReceiveArgs.RemoteEndPoint = client.Client.RemoteEndPoint;
            client.ReceiveArgs.SetBuffer(userToken.segment.Buffer, 0, userToken.segment.Length);
            client.ReceiveArgs.Completed += OnIOCompleted;
            if (!client.Client.ReceiveAsync(client.ReceiveArgs))
                OnIOCompleted(null, client.ReceiveArgs);
        }

        protected override void ResolveDataQueue(Player client, ref bool isSleep, uint tick)
        {
        }

        protected override void OnIOCompleted(object sender, SocketAsyncEventArgs args)
        {
            Socket clientSocket;
            switch (args.LastOperation)
            {
                case SocketAsyncOperation.Receive:
                    var userToken = args.UserToken as UserToken<Player>;
                    var client = userToken.client;
                    int count = args.BytesTransferred;
                    var segment = userToken.segment;
                    segment.Position = args.Offset;
                    segment.Count = count;
                    if (count == 0 | args.SocketError != SocketError.Success)
                    {
                        segment.Dispose();
                        args.Dispose();
                        ConnectLost(client, (uint)Environment.TickCount);
                        return;
                    }
                    receiveAmount++;
                    receiveCount += count;
                    count = segment.Length;
                    ResolveBuffer(client, ref segment);
                    if (count != segment.Length)
                    {
                        userToken.segment = segment;
                        args.SetBuffer(segment.Buffer, 0, segment.Length);
                    }
                    clientSocket = client.Client;
                    if (!clientSocket.Connected)
                        return;
                    if (!clientSocket.ReceiveAsync(args))
                        OnIOCompleted(null, args);
                    break;
            }
        }
    }

    /// <summary>
    /// 默认tcpiocp服务器，当不需要处理Player对象和Scene对象时可使用
    /// </summary>
    public class TcpServerIocp : TcpServerIocp<NetPlayer, DefaultScene> { }
}
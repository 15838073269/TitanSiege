using Net.Event;
using Net.Share;
using Net.System;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Net.Server
{
    /// <summary>
    /// udp 输入输出完成端口服务器
    /// <para>Player:当有客户端连接服务器就会创建一个Player对象出来, Player对象和XXXClient是对等端, 每当有数据处理都会通知Player对象. </para>
    /// <para>Scene:你可以定义自己的场景类型, 比如帧同步场景处理, mmorpg场景什么处理, 可以重写Scene的Update等等方法实现每个场景的更新和处理. </para>
    /// </summary>
    /// <typeparam name="Player"></typeparam>
    /// <typeparam name="Scene"></typeparam>
    public class UdpServerIocp<Player, Scene> : UdpServer<Player, Scene> where Player : NetPlayer, new() where Scene : NetScene<Player>, new()
    {
        protected override void StartSocketHandler()
        {
            ServerArgs = new SocketAsyncEventArgs { UserToken = Server };
            var userToken = new UserToken<Player>() { segment = BufferPool.Take() };
            ServerArgs.UserToken = userToken;
            ServerArgs.SetBuffer(userToken.segment.Buffer, 0, userToken.segment.Length);
            ServerArgs.RemoteEndPoint = Server.LocalEndPoint;
            ServerArgs.Completed += OnIOCompleted;
            if (!Server.ReceiveFromAsync(ServerArgs))
                OnIOCompleted(null, ServerArgs);
        }

        protected override void ReceiveProcessed(EndPoint remotePoint, ref bool isSleep) { }

        protected override void OnIOCompleted(object sender, SocketAsyncEventArgs args)
        {
            switch (args.LastOperation)
            {
                case SocketAsyncOperation.ReceiveFrom:
                    try
                    {
                        var userToken = args.UserToken as UserToken<Player>;
                        int count = args.BytesTransferred;
                        if (count == 0 | args.SocketError != SocketError.Success)
                            return;
                        var segment = userToken.segment;
                        segment.Position = args.Offset;
                        segment.Count = count;
                        receiveAmount++;
                        receiveCount += count;
                        var remotePoint = args.RemoteEndPoint;
                        ReceiveProcessedDirect(remotePoint, segment, false);
                    }
                    catch(Exception e)
                    {
                        NDebug.LogError(e);
                    } 
                    finally 
                    {
                        if (!Server.ReceiveFromAsync(ServerArgs))
                            OnIOCompleted(null, ServerArgs);
                    }
                    break;
            }
        }
    }

    /// <summary>
    /// 默认udpiocp服务器，当不需要处理Player对象和Scene对象时可使用
    /// </summary>
    public class UdpServerIocp : UdpServerIocp<NetPlayer, DefaultScene> { }
}

using Fleck;

namespace Net.Server
{
    /// <summary>
    /// web客户端对象
    /// </summary>
    public class WebPlayer : NetPlayer
    {
        /// <summary>
        /// webSocket套接字
        /// </summary>
        public WebSocketConnection WSClient { get; set; }

        public override void Dispose()
        {
            base.Dispose();
            WSClient?.Close();
        }
    }
}

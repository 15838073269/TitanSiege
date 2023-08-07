using System;
using System.Runtime.InteropServices;

namespace Net.Server
{
    /// <summary>
    /// udx客户端对象
    /// </summary>
    public class UdxPlayer : NetPlayer
    {
        public IntPtr Udx { get; set; }
        public GCHandle Handle;

        public override void Dispose()
        {
            if (isDispose)
                return;
            base.Dispose();
            if (Handle.IsAllocated)
                Handle.Free();
        }
    }
}

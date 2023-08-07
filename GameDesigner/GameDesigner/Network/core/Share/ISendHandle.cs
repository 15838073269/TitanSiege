using System;
using System.Threading;

namespace Net.Share
{
    /// <summary>
    /// 发送处理程序接口 
    /// 2019.9.23
    /// </summary>
    public interface ISendHandle
    {
        /// <summary>
        /// 发送自定义网络数据
        /// </summary>
        /// <param name="buffer">数据缓冲区</param>
        void Send(byte[] buffer);

        /// <summary>
        /// 发送自定义网络数据
        /// </summary>
        /// <param name="cmd">网络命令</param>
        /// <param name="buffer">发送字节数组缓冲区</param>
        void Send(byte cmd, byte[] buffer);

        /// <summary>
        /// 发送远程过程调用函数数据
        /// </summary>
        /// <param name="func">RPCFun函数</param>
        /// <param name="pars">RPCFun参数</param>
        void Send(string func, params object[] pars);

        /// <summary>
        /// 发送带有网络命令的远程过程调用数据
        /// </summary>
        /// <param name="cmd">网络命令</param>
        /// <param name="func">RPCFun函数</param>
        /// <param name="pars">RPCFun参数</param>
        void Send(byte cmd, string func, params object[] pars);

        /// <summary>
        /// 发送网络可靠传输数据, 可以发送大型文件数据
        /// 调用此方法通常情况下是一定把数据发送成功为止
        /// </summary>
        /// <param name="func">函数名</param>
        /// <param name="pars">参数</param>
        void SendRT(string func, params object[] pars);

        /// <summary>
        /// 发送可靠网络传输, 可以发送大型文件数据
        /// 调用此方法通常情况下是一定把数据发送成功为止
        /// </summary>
        /// <param name="cmd">网络命令</param>
        /// <param name="func">函数名</param>
        /// <param name="pars">参数</param>
        void SendRT(byte cmd, string func, params object[] pars);

        /// <summary>
        /// 发送可靠网络传输, 可发送大数据流
        /// 调用此方法通常情况下是一定把数据发送成功为止
        /// </summary>
        /// <param name="buffer"></param>
        void SendRT(byte[] buffer);

        /// <summary>
        /// 发送可靠网络传输, 可发送大数据流
        /// 调用此方法通常情况下是一定把数据发送成功为止
        /// </summary>
        /// <param name="cmd">网络命令</param>
        /// <param name="buffer"></param>
        void SendRT(byte cmd, byte[] buffer);
    }
}
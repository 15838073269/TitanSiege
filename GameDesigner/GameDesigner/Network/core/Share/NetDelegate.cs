﻿namespace Net.Share
{
    using global::System;
    using global::System.Reflection;
    using global::System.Runtime.InteropServices;

    public interface IRPCMethod
    {
        /// <summary>
        /// 网络命令
        /// </summary>
        byte cmd { get; set; }
        /// <summary>
        /// 委托对象
        /// </summary>
        object target { get; set; }
        /// <summary>
        /// 委托方法
        /// </summary>
        MethodInfo method { get; set; }

        void Invoke(object[] pars);
    }

    /// <summary>
    /// 远程过程调用方法
    /// </summary>
    [Serializable, StructLayout(LayoutKind.Sequential)]
    public struct RPCMethod : IRPCMethod
    {
        /// <summary>
        /// 委托函数名
        /// </summary>
        public string Name;
        /// <summary>
        /// 委托对象
        /// </summary>
        public object target { get; set; }
        /// <summary>
        /// 委托方法
        /// </summary>
        public MethodInfo method { get; set; }
        /// <summary>
        /// 网络命令
        /// </summary>
        public byte cmd { get; set; }

        /// <summary>
        /// 远程过程调用方法
        /// </summary>
        public RPCMethod(Action method)
        {
            Name = method.Method.ToString();
            cmd = 0;
            target = method.Target;
            this.method = method.Method;
        }

        /// <summary>
        /// 网络委托
        /// </summary>
        public void AddMethodEvent(Delegate method)
        {
            Name = method.Method.ToString();
            cmd = 0;
            target = method.Target;
            this.method = method.Method;
        }

        /// <summary>
        /// 远程过程调用方法
        /// </summary>
        public RPCMethod(object target, MethodInfo method)
        {
            Name = method.ToString();
            cmd = 0;
            this.target = target;
            this.method = method;
        }

        /// <summary>
        /// 远程过程调用方法
        /// </summary>
        public RPCMethod(object target, MethodInfo method, byte cmd)
        {
            Name = method.ToString();
            this.cmd = cmd;
            this.target = target;
            this.method = method;
        }

        public void Invoke()
        {
            method.Invoke(target, null);
        }

        public void Invoke(object[] pars)
        {
            method.Invoke(target, pars);
        }

        public override string ToString()
        {
            return $"{Name}";
        }
    }
}
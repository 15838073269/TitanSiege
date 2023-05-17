using Net.Adapter;
using System.Collections.Generic;
using System.Reflection;

namespace Net.Share
{
    public class MemberData
    {
        public MemberInfo member;
        public RPCFun rpc;
        public SyncVarInfo syncVar;
        public RPCPTR ptr;

        public override string ToString()
        {
            return $"{member}";
        }
    }

    public class MemberDataList
    {
        public List<MemberData> members = new List<MemberData>();
    }

    public struct RPCMethodPtr : IRPCMethod
    {
        public byte cmd { get; set; }
        public object target { get; set; }
        public MethodInfo method { get; set; }
        public RPCPTR ptr;

        public RPCMethodPtr(object target, MethodInfo method, byte cmd, RPCPTR ptr) : this()
        {
            this.target = target;
            this.cmd = cmd;
            this.ptr = ptr;
            this.method = method;
        }

        public void Invoke()
        {
            ptr.Invoke(target, null);
        }

        public void Invoke(params object[] pars)
        {
            ptr.Invoke(target, pars);
        }
    }

    public struct RPCDataPtr : IRPCData 
    {
        /// <summary>
        /// �����Ͳ���������
        /// </summary>
        public string name;
        /// <summary>
        /// �洢��������л������Ķ���
        /// </summary>
        public object target { get; set; }
        /// <summary>
        /// �洢�����л��ĺ���
        /// </summary>
        public MethodInfo method { get; set; }
        public RPCPTR ptr;
        /// <summary>
        /// �洢�����л�����
        /// </summary>
        public object[] pars;

        /// <summary>
        /// ���캯��
        /// </summary>
        /// <param name="target">Զ�̵��ö���</param>
        /// <param name="method">Զ�̵��÷���</param>
        /// <param name="pars">Զ�̵��ò���</param>
        public RPCDataPtr(object target, MethodInfo method, RPCPTR ptr, params object[] pars)
        {
            name = method.ToString();
            this.target = target;
            this.method = method;
            this.pars = pars;
            this.ptr = ptr;
        }

        /// <summary>
        /// ���÷���
        /// </summary>
        /// <returns></returns>
        public void Invoke()
        {
            if (ptr == null)
                return;
            ptr.Invoke(target, pars);
        }

        public override string ToString()
        {
            return $"{target}->{name}";
        }
    }
}
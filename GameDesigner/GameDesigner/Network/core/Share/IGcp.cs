using Net.System;
using System;
using System.Net;

namespace Net.Share 
{
    /// <summary>
    /// gcp�ɿ�Э��ӿ�
    /// </summary>
    public interface IGcp
    {
        ushort MTU { get; set; }
        int RTO { get; set; }
        int MTPS { get; set; }
        FlowControlMode FlowControl { get; set; }
        Action<RTProgress> OnRevdProgress { get; set; }
        Action<RTProgress> OnSendProgress { get; set; }
        Action<byte[]> OnSender { get; set; }
        EndPoint RemotePoint { get; set; }

        /// <summary>
        /// �ж��Ƿ��з��ͣ�������Ŀǰ�кܶ��������Ҫ����
        /// </summary>
        /// <returns></returns>
        bool HasSend();
        /// <summary>
        /// ����Ҫ���͵�����
        /// </summary>
        /// <param name="buffer"></param>
        void Input(byte[] buffer);
        /// <summary>
        /// ���·��ͺͽ����¼�
        /// </summary>
        void Update();
        /// <summary>
        /// �����Ľ��뷢�ͽӿ�
        /// </summary>
        /// <param name="buffer"></param>
        void Send(byte[] buffer);
        /// <summary>
        /// �������Ƿ�������
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        int Receive(out Segment buffer);
        /// <summary>
        /// ʩ��gcp��Դ
        /// </summary>
        void Dispose();
    }
}
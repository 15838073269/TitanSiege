using System.Data;

namespace Net.Share
{
    /// <summary>
    /// ��Ϸ�������ýӿ�
    /// </summary>
    public interface IDataConfig
    {
        /// <summary>
        /// ��ʼ�����������תʵ�����
        /// </summary>
        /// <param name="row"></param>
        void Init(DataRow row);
    }
}
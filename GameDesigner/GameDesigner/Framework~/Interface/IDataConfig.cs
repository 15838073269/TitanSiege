using System.Data;

namespace Framework
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
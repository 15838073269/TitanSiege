using System.Data;

namespace Net.Share
{
    /// <summary>
    /// 游戏数据配置接口
    /// </summary>
    public interface IDataConfig
    {
        /// <summary>
        /// 唯一ID
        /// </summary>
        int ID { get; set; }

        /// <summary>
        /// 初始化表格行数据转实体对象
        /// </summary>
        /// <param name="row"></param>
        void Init(DataRow row);
    }
}
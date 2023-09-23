using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Net.Share 
{
    /// <summary>
    /// 数据行接口
    /// </summary>
    public interface IDataRow
    {
        /// <summary>
        /// 当前数据行状态
        /// </summary>
        DataRowState RowState { get; set; }
        /// <summary>
        /// 初始化数据行
        /// </summary>
        /// <param name="row"></param>
        void Init(DataRow row);
        /// <summary>
        /// sql的插入语句处理
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="parms"></param>
        /// <param name="parmsLen"></param>
        void AddedSql(StringBuilder sb);
        /// <summary>
        /// sql的修改语句处理
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="parms"></param>
        /// <param name="parmsLen"></param>
        void ModifiedSql(StringBuilder sb);
        /// <summary>
        /// sql的删除语句处理
        /// </summary>
        /// <param name="sb"></param>
        void DeletedSql(StringBuilder sb);

        object this[string name] { get; set; }
    }
}
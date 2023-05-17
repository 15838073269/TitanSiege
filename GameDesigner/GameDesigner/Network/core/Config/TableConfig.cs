using System;
using System.Data;
using System.IO;
using System.Collections.Generic;
using Net.Event;
using Net.Share;

namespace Net.Config
{
    /// <summary>
    /// 表配置类, 可双端使用
    /// </summary>
    public class TableConfig
    {
        private DataSet dataSet;
        private readonly Dictionary<Type, Dictionary<string, IDataConfig[]>> directory = new Dictionary<Type, Dictionary<string, IDataConfig[]>>(); //表缓存字典

        /// <summary>
        /// 加载表文件
        /// </summary>
        /// <param name="path"></param>
        public void LoadTableFile(string path)
        {
            var jsonStr = File.ReadAllText(path);
            LoadTable(jsonStr);
        }

        /// <summary>
        /// 加载表数据
        /// </summary>
        /// <param name="jsonStr"></param>
        public void LoadTable(string jsonStr)
        {
            dataSet = Newtonsoft_X.Json.JsonConvert.DeserializeObject<DataSet>(jsonStr);
            foreach (DataTable table in dataSet.Tables)
            {
                for (int i = 0; i < table.Columns.Count; i++)
                {
                    table.Columns[i].ColumnName = table.Rows[0][i].ToString();
                }
            }
        }

        /// <summary>
        /// 获取某个表
        /// </summary>
        /// <param name="sheetName"></param>
        /// <returns></returns>
        public DataTable GetTable(string sheetName)
        {
            return dataSet.Tables[sheetName];
        }

        /// <summary>
        /// 获取excel表格数据，filterExpression参数例子: "Name = 'UI_Message'"
        /// </summary>
        /// <typeparam name="T">要获取的类型</typeparam>
        /// <param name="filterExpression">过滤表达式</param>
        /// <returns></returns>
        public T GetDataConfig<T>(string filterExpression) where T : IDataConfig, new()
        {
            var datas = GetDataConfigs<T>(filterExpression);
            if (datas == null)
                return default;
            if (datas.Length == 0)
                return default;
            return datas[0];
        }

        /// <summary>
        /// 获取excel表格数据，filterExpression参数例子: "Name = 'UI_Message'"
        /// </summary>
        /// <typeparam name="T">要获取的类型</typeparam>
        /// <param name="filterExpression">过滤表达式</param>
        /// <returns></returns>
        public T[] GetDataConfigs<T>(string filterExpression) where T : IDataConfig, new()
        {
            try
            {
                var type = typeof(T);
                if (!directory.TryGetValue(type, out var dict))
                    directory.Add(type, dict = new Dictionary<string, IDataConfig[]>());
                if (dict.TryGetValue(filterExpression, out var datas))
                    return datas as T[];
                var sheetName = type.Name.Replace("DataConfig", "");
                var table = GetTable(sheetName);
                var rows = table.Select(filterExpression);
                var items = new T[rows.Length];
                for (int i = 0; i < rows.Length; i++)
                {
                    var t = new T();
                    t.Init(rows[i]);
                    items[i] = t;
                }
                dict.Add(filterExpression, items as IDataConfig[]);
                return items;
            }
            catch (Exception ex)
            {
                NDebug.LogError("获取Excel表数据异常: " + ex);
            }
            return null;
        }
    }
}

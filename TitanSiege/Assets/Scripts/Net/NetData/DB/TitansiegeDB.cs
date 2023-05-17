#if SERVER
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Text;
using System.Threading;
using MySql.Data.MySqlClient;
using Net.Event;
using Net.System;
using Net.Share;
using System.Collections.Concurrent;
using Cysharp.Threading.Tasks;

namespace Titansiege
{
    /// <summary>
    /// TitansiegeDB数据库管理类
    /// 此类由MySqlDataBuild工具生成, 请不要在此类编辑代码! 请新建一个类文件进行分写
    /// <para>MySqlDataBuild工具提供Rpc自动同步到mysql数据库的功能, 提供数据库注释功能</para>
    /// MySqlDataBuild工具gitee地址:https://gitee.com/leng_yue/my-sql-data-build
    /// </summary>
    public partial class TitansiegeDB
    {
        public static TitansiegeDB I { get; private set; } = new TitansiegeDB();
        private readonly HashSetSafe<IDataRow> dataRowHandler = new HashSetSafe<IDataRow>();
        private readonly ConcurrentStack<MySqlConnection> conns = new ConcurrentStack<MySqlConnection>();
        public static string connStr = @"Database='titansiege';Data Source='127.0.0.1';Port=3306;User Id='root';Password='titansiege';charset='utf8mb4';pooling=true;useCompression=true;allowBatch=true;connectionTimeout=60;";
        /// <summary>
        /// 打印输出Sql语句批处理字符串
        /// </summary>
        public bool DebugSqlBatch { get; set; }
        /// <summary>
        /// 从运行到现在的所有Sql执行次数
        /// </summary>
        public long QueryCount { get; set; }
        /// <summary>
        /// Sql批处理sql语句字符串大小, 默认是128k的字符串长度
        /// </summary>
        public int SqlBatchSize { get; set; } = ushort.MaxValue * 2;

        private MySqlConnection CheckConn(MySqlConnection conn)
        {
            if (conn == null)
            {
                conn = new MySqlConnection(connStr); //数据库连接
                conn.Open();
            }
            conn.Ping();//长时间没有连接后断开连接检查状态
            if (conn.State != ConnectionState.Open)
            {
                conn.Close();
                conn = new MySqlConnection(connStr); //数据库连接
                conn.Open();
            }
            return conn;
        }

        public void Init(Action<List<object>> onInit, int connLen = 5)
        {
            InitConnection(connLen);
            List<object> list = new List<object>();
    
            var charactersTable = ExecuteReader($"SELECT * FROM characters");
            foreach (DataRow row in charactersTable.Rows)
            {
                var data = new CharactersData();
                data.Init(row);
                list.Add(data);
            }
            charactersTable.Dispose();
    
            var configTable = ExecuteReader($"SELECT * FROM config");
            foreach (DataRow row in configTable.Rows)
            {
                var data = new ConfigData();
                data.Init(row);
                list.Add(data);
            }
            configTable.Dispose();
    
            var usersTable = ExecuteReader($"SELECT * FROM users");
            foreach (DataRow row in usersTable.Rows)
            {
                var data = new UsersData();
                data.Init(row);
                list.Add(data);
            }
            usersTable.Dispose();
    
            onInit?.Invoke(list);
        }

        public void InitConnection(int connLen = 1) //初学者避免发生死锁, 默认只创建一条连接
        {
            while (conns.TryPop(out var conn))
                conn.Close();
            for (int i = 0; i < connLen; i++)
                conns.Push(CheckConn(null));
        }

        public DataTable ExecuteReader(string cmdText)
        {
            MySqlConnection conn1;
            while (!conns.TryPop(out conn1))
                Thread.Sleep(1);
            var conn = CheckConn(conn1);
            var dt = new DataTable();
            try
            {
                using (var cmd = new MySqlCommand())
                {
                    cmd.CommandText = cmdText;
                    cmd.Connection = conn;
                    cmd.Parameters.Clear();
                    using (var sdr = cmd.ExecuteReader())
                    {
                        dt.Load(sdr);
                        QueryCount++;
                    }
                }
            }
            catch (Exception ex)
            {
                NDebug.LogError(cmdText + " 错误: " + ex);
            }
            finally
            {
                conns.Push(conn);
            }
            return dt;
        }

        public async UniTask<DataTable> ExecuteReaderAsync(string cmdText)
        {
            await UniTask.SwitchToThreadPool();
            return ExecuteReader(cmdText);
        }

        /// <summary>
        /// 查询1: select * from titansiege where id=1;
        /// <para></para>
        /// 查询2: select * from titansiege where id=1 and `index`=1;
        /// <para></para>
        /// 查询3: select * from titansiege where id=1 or `index`=1;
        /// <para></para>
        /// 查询4: select * from titansiege where id in(1,2,3,4,5);
        /// <para></para>
        /// 查询5: select * from titansiege where id not in(1,2,3,4,5);
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cmdText"></param>
        /// <returns></returns>
        public T ExecuteQuery<T>(string cmdText) where T : IDataRow, new()
        {
            var array = ExecuteQueryList<T>(cmdText);
            if (array == null)
                return default;
            return array[0];
        }

        /// <summary>
        /// 查询1: select * from titansiege where id=1;
        /// <para></para>
        /// 查询2: select * from titansiege where id=1 and `index`=1;
        /// <para></para>
        /// 查询3: select * from titansiege where id=1 or `index`=1;
        /// <para></para>
        /// 查询4: select * from titansiege where id in(1,2,3,4,5);
        /// <para></para>
        /// 查询5: select * from titansiege where id not in(1,2,3,4,5);
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cmdText"></param>
        /// <returns></returns>
        public T[] ExecuteQueryList<T>(string cmdText) where T : IDataRow, new()
        {
            using (var dt = ExecuteReader(cmdText))
            {
                if (dt.Rows.Count == 0)
                    return default;
                var datas = new T[dt.Rows.Count];
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    datas[i] = new T();
                    datas[i].Init(dt.Rows[i]);
                }
                return datas;
            }
        }

        /// <summary>
        /// 查询1: select * from titansiege where id=1;
        /// <para></para>
        /// 查询2: select * from titansiege where id=1 and `index`=1;
        /// <para></para>
        /// 查询3: select * from titansiege where id=1 or `index`=1;
        /// <para></para>
        /// 查询4: select * from titansiege where id in(1,2,3,4,5);
        /// <para></para>
        /// 查询5: select * from titansiege where id not in(1,2,3,4,5);
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cmdText"></param>
        /// <returns></returns>
        public async UniTask<T> ExecuteQueryAsync<T>(string cmdText) where T : IDataRow, new()
        {
            var array = await ExecuteQueryListAsync<T>(cmdText);
            if (array == null)
                return default;
            return array[0];
        }

        /// <summary>
        /// 查询1: select * from titansiege where id=1;
        /// <para></para>
        /// 查询2: select * from titansiege where id=1 and `index`=1;
        /// <para></para>
        /// 查询3: select * from titansiege where id=1 or `index`=1;
        /// <para></para>
        /// 查询4: select * from titansiege where id in(1,2,3,4,5);
        /// <para></para>
        /// 查询5: select * from titansiege where id not in(1,2,3,4,5);
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cmdText"></param>
        /// <returns></returns>
        public async UniTask<T[]> ExecuteQueryListAsync<T>(string cmdText) where T : IDataRow, new()
        {
            using (var dt = await ExecuteReaderAsync(cmdText))
            {
                if (dt.Rows.Count == 0)
                    return default;
                var datas = new T[dt.Rows.Count];
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    datas[i] = new T();
                    datas[i].Init(dt.Rows[i]);
                }
                return datas;
            }
        }

        public async UniTask<int> ExecuteNonQuery(string cmdText, List<IDbDataParameter> parameters)
        {
            await UniTask.SwitchToThreadPool();
            MySqlConnection conn1;
            int tick = Environment.TickCount + 60000;//1分钟内如果一直在循环, 则提示
            while (!conns.TryPop(out conn1))
            {
                if (Environment.TickCount >= tick)
                {
                    cmdText = GetCommandText(cmdText, parameters.ToArray());
                    NDebug.LogError(cmdText + " 连接池不足, 等待超过1分钟, 此次提交失败! 如果有必要, 请将sql语句复制到Navicat的查询窗口执行");
                    return await UniTask.FromResult(0);
                }
                await UniTask.Yield();
            }
            var conn = CheckConn(conn1);
            var pars = parameters != null ? parameters.ToArray() : new IDbDataParameter[0];
            var count = ExecuteNonQuery(conn, cmdText, pars);
            conns.Push(conn);
            return count;
        }

        public async UniTaskVoid ExecuteNonQuery(string cmdText, List<IDbDataParameter> parameters, Action<int, Stopwatch> onComplete)
        {
            var stopwatch = Stopwatch.StartNew();
            var count = await ExecuteNonQuery(cmdText, parameters);
            stopwatch.Stop();
            onComplete(count, stopwatch);
        }

        private int ExecuteNonQuery(MySqlConnection conn, string cmdText, IDbDataParameter[] parameters)
        {
            try
            {
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    cmd.CommandText = cmdText;
                    cmd.Connection = conn;
                    cmd.CommandTimeout = 30;//避免死锁一直无畏的等待, 在30秒内必须完成
                    cmd.Parameters.AddRange(parameters);
                    var count = cmd.ExecuteNonQuery();
                    QueryCount += count;
                    return count;
                }
            }
            catch (Exception ex)
            {
                cmdText = GetCommandText(cmdText, parameters);
                NDebug.LogError(cmdText + " 发生错误,如果有必要,请将sql语句复制到Navicat的查询窗口执行: " + ex);
            }
            return -1;
        }

        private static string GetCommandText(string cmdText, IDbDataParameter[] parameters) 
        {
            for (int i = 0; i < parameters.Length; i++)
            {
                if (parameters[i].Value is byte[] buffer)
                {
                    var sb = new StringBuilder();
                    for (int n = 0; n < buffer.Length; n++)
                    {
                        var x = buffer[n].ToString("x").PadLeft(2, '0');
                        sb.Append(x);
                    }
                    var hex = sb.ToString();
                    cmdText = cmdText.Replace($"@buffer{i} ", $"UNHEX('{hex}') "); //必须留空格, 否则buffer1和buffer10都一样被替换, buffer10会留个0的问题
                }
            }
            return cmdText;
        }

        public void Update(IDataRow entity)//更新的行,列
        {
            dataRowHandler.Add(entity);
        }

        public bool Executed()//每秒调用一次, 需要自己调用此方法
        {
            try
            {
                var sb = new StringBuilder();
                var parms = new List<IDbDataParameter>();
                int parmsLen = 0;
                foreach (var row in dataRowHandler)
                {
                    switch (row.RowState)
                    {
                        case DataRowState.Added:
                            row.AddedSql(sb, parms, ref parmsLen);
                            break;
                        case DataRowState.Detached:
                            row.DeletedSql(sb);
                            break;
                        case DataRowState.Modified:
                            row.ModifiedSql(sb, parms, ref parmsLen);
                            break;
                    }
                    if (sb.Length + parmsLen >= SqlBatchSize)
                    {
                        _ = ExecuteNonQuery(sb.ToString(), parms, (count1, stopwatch) => NDebug.Log($"sql批处理完成:{count1} 用时:{stopwatch.Elapsed}"));
                        sb.Clear();
                        parms.Clear();
                        parmsLen = 0;
                    }
                    dataRowHandler.Remove(row);
                }
                if (sb.Length > 0)
                {
                    _ = ExecuteNonQuery(sb.ToString(), parms, (count1, stopwatch) => { if (count1 > 2000) NDebug.Log($"sql批处理完成:{count1} 用时:{stopwatch.Elapsed}"); } );
                }
            }
            catch (Exception ex)
            {
                NDebug.LogError("SQL异常: " + ex);
            }
            return true;
        }
    }
}
#endif
#if SERVER
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using Net.Event;
using Net.System;
using System.Collections.Concurrent;

/// <summary>
/// TitansiegeDB数据库管理类
/// 此类由MySqlDataBuild工具生成, 请不要在此类编辑代码! 请定义一个扩展类进行处理
/// MySqlDataBuild工具提供Rpc自动同步到mysql数据库的功能, 提供数据库注释功能
/// MySqlDataBuild工具gitee地址:https://gitee.com/leng_yue/my-sql-data-build
/// </summary>
public partial class TitansiegeDB
{
    public static TitansiegeDB I { get; private set; } = new TitansiegeDB();
    private readonly HashSetSafe<IDataRow> dataRowHandler = new HashSetSafe<IDataRow>();
    private static readonly ConcurrentStack<MySqlConnection> conns = new ConcurrentStack<MySqlConnection>();
    public static string connStr = @"Database='titansiege';Data Source='127.0.0.1';Port=3306;User Id='root';Password='xiewenhao32';charset='utf8';pooling=true;useCompression=true;allowBatch=true;connectionTimeout=1200;";
    public bool DebugSqlBatch { get; set; }

    private static MySqlConnection CheckConn(MySqlConnection conn)
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

        var usersTable = ExecuteReader($"SELECT * FROM users");
        foreach (DataRow row in usersTable.Rows)
        {
            var data = new UsersData();
            data.Init(row);
            list.Add(data);
        }

        onInit?.Invoke(list);
    }

    public void InitConnection(int connLen = 5)
    {
        while (conns.TryPop(out var conn))
        {
            conn.Close();
        }
        for (int i = 0; i < connLen; i++)
        {
            conns.Push(CheckConn(null));
        }
    }

    public static DataTableEntity ExecuteReader(string cmdText)
    {
        MySqlConnection conn1;
        while (!conns.TryPop(out conn1))
        {
            Thread.Sleep(1);
        }
        var conn = CheckConn(conn1);
        var dt = new DataTableEntity();
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

    public static async Task<DataTableEntity> ExecuteReaderAsync(string cmdText)
    {
        MySqlConnection conn1;
        while (!conns.TryPop(out conn1))
        {
            Thread.Sleep(1);
        }
        var conn = CheckConn(conn1);
        var dt = new DataTableEntity();
        try
        {
            using (var cmd = new MySqlCommand())
            {
                cmd.CommandText = cmdText;
                cmd.Connection = conn;
                cmd.Parameters.Clear();
                using (var sdr = await cmd.ExecuteReaderAsync())
                {
                    dt.Load(sdr);
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
    public static T ExecuteQuery<T>(string cmdText) where T : IDataRow, new()
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
    public static T[] ExecuteQueryList<T>(string cmdText) where T : IDataRow, new()
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
    public static async Task<T> ExecuteQueryAsync<T>(string cmdText) where T : IDataRow, new()
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
    public static async Task<T[]> ExecuteQueryListAsync<T>(string cmdText) where T : IDataRow, new()
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

    public static async Task<int> ExecuteNonQuery(string cmdText, List<IDbDataParameter> parameters)
    {
        MySqlConnection conn1;
        while (!conns.TryPop(out conn1))
        {
            Thread.Sleep(1);
        }
        var conn = CheckConn(conn1);
        var pars = parameters.ToArray();
        return await Task.Run(() =>
        {
            var count = ExecuteNonQuery(conn, cmdText, pars);
            conns.Push(conn);
            return count;
        });
    }

    public static void ExecuteNonQuery(string cmdText, List<IDbDataParameter> parameters, Action<int, Stopwatch> onComplete)
    {
        MySqlConnection conn1;
        while (!conns.TryPop(out conn1))
        {
            Thread.Sleep(1);
        }
        var conn = CheckConn(conn1);
        var pars = parameters.ToArray();
        if (I.DebugSqlBatch)
            NDebug.Log(cmdText);
        Task.Run(() =>
        {
            var stopwatch = Stopwatch.StartNew();
            var count = ExecuteNonQuery(conn, cmdText, pars);
            conns.Push(conn);
            stopwatch.Stop();
            onComplete(count, stopwatch);
        });
    }

    private static int ExecuteNonQuery(MySqlConnection conn, string cmdText, IDbDataParameter[] parameters)
    {
        var transaction = conn.BeginTransaction();
        try
        {
            using (MySqlCommand cmd = new MySqlCommand())
            {
                cmd.Transaction = transaction;
                cmd.CommandText = cmdText;
                cmd.Connection = conn;
                cmd.CommandTimeout = 1200;
                cmd.Parameters.AddRange(parameters);
                int res = cmd.ExecuteNonQuery();
                transaction.Commit();
                return res;
            }
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            NDebug.LogError(cmdText + " 错误: " + ex);
        }
        return -1;
    }

    public void Update(IDataRow entity)//更新的行,列
    {
        dataRowHandler.Add(entity);
    }

    public bool Executed()//每秒调用一次, 需要自己调用此方法
    {
        try
        {
            StringBuilder sb = new StringBuilder();
            var parms = new List<IDbDataParameter>();
            int count = 0, parmsLen = 0;
            foreach (var row in dataRowHandler)
            {
                switch (row.RowState)
                {
                    case DataRowState.Added:
                        {
                            row.AddedSql(sb, parms, ref parmsLen, ref count);
                        }
                        break;
                    case DataRowState.Detached:
                        {
                            row.DeletedSql(sb);
                        }
                        break;
                    case DataRowState.Modified:
                        {
                            row.ModifiedSql(sb, parms, ref parmsLen, ref count);
                        }
                        break;
                }
                if (sb.Length + parmsLen >= 2000000)
                {
                    ExecuteNonQuery(sb.ToString(), parms, (count1, stopwatch) =>
                    {
                        NDebug.Log($"sql批处理完成:{count1} 用时:{stopwatch.ElapsedMilliseconds}");
                    });
                    sb.Clear();
                    parms.Clear();
                    count = 0;
                    parmsLen = 0;
                }
                dataRowHandler.Remove(row);
            }
            if (sb.Length > 0)
            {
                ExecuteNonQuery(sb.ToString(), parms, (count1, stopwatch) =>
                {
                    if (count1 > 2000)
                        NDebug.Log($"sql批处理完成:{count1} 用时:{stopwatch.ElapsedMilliseconds}");
                });
            }
        }
        catch (Exception ex)
        {
            NDebug.LogError("SQL异常: " + ex);
        }
        return true;
    }
}
#endif
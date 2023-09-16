#if SERVER
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.IO;
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
        private readonly MyDictionary<Type, HashSetSafe<IDataRow>> DataRowHandler = new MyDictionary<Type, HashSetSafe<IDataRow>>();
        private readonly ConcurrentStack<MySqlConnection> conns = new ConcurrentStack<MySqlConnection>();
        public static string connStr = @"Database='titansiege';Data Source='127.0.0.1';Port=3306;User Id='root';Password='titansiege';charset='utf8mb4';pooling=true;useCompression=true;allowBatch=true;connectionTimeout=60;allowloadlocalinfile=true;";
        /// <summary>
        /// 从运行到现在的所有Sql执行次数
        /// </summary>
        public long QueryCount { get; set; }
        /// <summary>
        /// Sql批处理sql语句字符串大小, 默认是128k的字符串长度
        /// </summary>
        public int SqlBatchSize { get; set; } = ushort.MaxValue * 2;
        /// <summary>
        /// 命令超时, 默认为30秒内必须完成
        /// </summary>
        public int CommandTimeout { get; set; } = 30;
        /// <summary>
        /// 每次执行最多可批量的次数 默认是10万
        /// </summary>
        public int BatchSize { get; set; } = 100000;
        private readonly StringBuilder updateCmdText = new StringBuilder();
        private readonly StringBuilder deleteCmdText = new StringBuilder();

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
     // -- 1
            var charactersTable = ExecuteReader($"SELECT * FROM characters");
            foreach (DataRow row in charactersTable.Rows)
            {
                var data = new CharactersData();
                data.Init(row);
                list.Add(data);
            }
            charactersTable.Dispose();
     // -- 1
            var configTable = ExecuteReader($"SELECT * FROM config");
            foreach (DataRow row in configTable.Rows)
            {
                var data = new ConfigData();
                data.Init(row);
                list.Add(data);
            }
            configTable.Dispose();
     // -- 1
            var npcsTable = ExecuteReader($"SELECT * FROM npcs");
            foreach (DataRow row in npcsTable.Rows)
            {
                var data = new NpcsData();
                data.Init(row);
                list.Add(data);
            }
            npcsTable.Dispose();
     // -- 1
            var usersTable = ExecuteReader($"SELECT * FROM users");
            foreach (DataRow row in usersTable.Rows)
            {
                var data = new UsersData();
                data.Init(row);
                list.Add(data);
            }
            usersTable.Dispose();
     // -- 2
            onInit?.Invoke(list);
        }

        public void InitConnection(int connLen = 1) //初学者避免发生死锁, 默认只创建一条连接
        {
            while (conns.TryPop(out var conn))
                conn.Close();
            for (int i = 0; i < connLen; i++)
                conns.Push(CheckConn(null));
        }

        public MySqlConnection PopConnect()
        {
            MySqlConnection conn1;
            while (!conns.TryPop(out conn1))
                Thread.Sleep(1);
            return CheckConn(conn1);
        }

        public void CloseConnect()
        {
            while (conns.TryPop(out MySqlConnection conn))
                conn.Close();
        }

        public DataTable ExecuteReader(string cmdText)
        {
            var conn = PopConnect();
            var dt = new DataTable();
            try
            {
                using (var cmd = new MySqlCommand())
                {
                    cmd.CommandText = cmdText;
                    cmd.Connection = conn;
                    cmd.CommandTimeout = CommandTimeout;
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
            if (array.Length == 0)
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
            if (array.Length == 0)
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
                var datas = new T[dt.Rows.Count];
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    datas[i] = new T();
                    datas[i].Init(dt.Rows[i]);
                }
                return datas;
            }
        }

        public async UniTaskVoid ExecuteNonQuery(string cmdText, List<IDbDataParameter> parameters, Action<int, Stopwatch> onComplete)
        {
            var stopwatch = Stopwatch.StartNew();
            var count = await ExecuteNonQuery(cmdText, parameters);
            stopwatch.Stop();
            onComplete(count, stopwatch);
        }

        public async UniTask<int> ExecuteNonQuery(string cmdText, List<IDbDataParameter> parameters)
        {
            await UniTask.SwitchToThreadPool();
            var conn = PopConnect();
            var pars = parameters != null ? parameters.ToArray() : new IDbDataParameter[0];
            var count = ExecuteNonQuery(conn, cmdText, pars);
            conns.Push(conn);
            return count;
        }

        public int ExecuteNonQuery(string cmdText)
        {
            var conn = PopConnect();
            var pars = new IDbDataParameter[0];
            var count = ExecuteNonQuery(conn, cmdText, pars);
            conns.Push(conn);
            return count;
        }

        private int ExecuteNonQuery(MySqlConnection conn, string cmdText, IDbDataParameter[] parameters)
        {
            try
            {
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    cmd.CommandText = cmdText;
                    cmd.Connection = conn;
                    cmd.CommandTimeout = CommandTimeout;//避免死锁一直无畏的等待, 在30秒内必须完成
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

        public T ExecuteScalar<T>(string cmdText)
        {
            var conn = PopConnect();
            var pars = new IDbDataParameter[0];
            var count = ExecuteScalar<T>(conn, cmdText, pars);
            conns.Push(conn);
            return count;
        }

        private T ExecuteScalar<T>(MySqlConnection conn, string cmdText, IDbDataParameter[] parameters)
        {
            try
            {
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    cmd.CommandText = cmdText;
                    cmd.Connection = conn;
                    cmd.CommandTimeout = CommandTimeout;//避免死锁一直无畏的等待, 在30秒内必须完成
                    cmd.Parameters.AddRange(parameters);
                    var count = (T)cmd.ExecuteScalar();
                    QueryCount++;
                    return count;
                }
            }
            catch (Exception ex)
            {
                cmdText = GetCommandText(cmdText, parameters);
                NDebug.LogError(cmdText + " 发生错误,如果有必要,请将sql语句复制到Navicat的查询窗口执行: " + ex);
            }
            return default;
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
            var type = entity.GetType();
            if (!DataRowHandler.TryGetValue(type, out var hash))
                DataRowHandler[type] = hash = new HashSetSafe<IDataRow>();
            hash.Add(entity);
        }

        public bool Executed()//每秒调用一次, 需要自己调用此方法
        {
            try
            {
                updateCmdText.Clear();
                deleteCmdText.Clear();
                foreach (var item in DataRowHandler)
                {
                    var tableName = item.Key.Name;
                    tableName = tableName.Remove(tableName.Length - 4, 4);
                    var index = 0;
                    var count = item.Value.Count;
                    count = count > BatchSize ? BatchSize : count;
                    foreach (var row in item.Value)
                    {
                        switch (row.RowState)
                        {
                            case DataRowState.Added:
                                row.AddedSql(updateCmdText);
                                break;
                            case DataRowState.Modified:
                                row.ModifiedSql(updateCmdText);
                                break;
                            case DataRowState.Detached:
                                row.DeletedSql(deleteCmdText);
                                break;
                        }
                        item.Value.Remove(row);
                        if (updateCmdText.Length + deleteCmdText.Length >= SqlBatchSize) 
                        {
                            ExecuteNonQuery(tableName, updateCmdText, deleteCmdText);
                            updateCmdText.Clear();
                            deleteCmdText.Clear();
                        }
                        if (index++ >= count)
                            break;
                    }
                    ExecuteNonQuery(tableName, updateCmdText, deleteCmdText);
                    updateCmdText.Clear();
                    deleteCmdText.Clear();
                }
            }
            catch (Exception ex)
            {
                NDebug.LogError("SQL异常: " + ex);
            }
            return true;
        }

        private void ExecuteNonQuery(string tableName, StringBuilder stringBuilder, StringBuilder deleteSb)
        {
            if (stringBuilder.Length > 0)
            {
 // -- 3
                var conn = PopConnect();
                try
                {
                    var path = AppDomain.CurrentDomain.BaseDirectory + "MySqlBulkLoader\\";
                    var fileName = path + $"{tableName}.txt";
                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);
                    using (var stream = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                    {
                        stream.SetLength(0);
                        var text = stringBuilder.ToString();
                        var bytes = Encoding.UTF8.GetBytes(text);
                        stream.Write(bytes, 0, bytes.Length);
                        stream.Flush();
                    }
                    var bulkLoader = new MySqlBulkLoader(conn)
                    {
                        TableName = tableName,
                        FieldTerminator = "|",
                        LineTerminator = "\r\n",
                        NumberOfLinesToSkip = 0,
                        FileName = fileName,
                        EscapeCharacter = '\\',
                        Local = true,
                        CharacterSet = "utf8mb4",
                        ConflictOption = MySqlBulkLoaderConflictOption.Replace,
                    };
                    var stopwatch = Stopwatch.StartNew();
                    var rowCount = bulkLoader.Load();
                    QueryCount += rowCount;
                    stopwatch.Stop();
                    if (rowCount > 2000) NDebug.Log($"SQL批处理完成:{rowCount} 用时:{stopwatch.Elapsed}");
                }
                catch (Exception ex)
                {
                    NDebug.LogError("批量错误:" + ex);
                }
                finally
                {
                    conns.Push(conn);
                }
 // -- 5
            }
            if (deleteSb.Length > 0)
            {
                var stopwatch = Stopwatch.StartNew();
                var rowCount = ExecuteNonQuery(deleteSb.ToString());
                stopwatch.Stop();
                if (rowCount > 2000) NDebug.Log($"SQL批处理完成:{rowCount} 用时:{stopwatch.Elapsed}");
            }
        }

        public string CheckStringValue(string value, uint length)
        {
            CheckStringValue(ref value, length);
            return value;
        }

        public void CheckStringValue(ref string value, uint length)
        {
            if (value == null)
                value = string.Empty;
            value = value.Replace("\\", "\\\\"); //如果包含\必须转义, 否则出现 \'时就会出错
            value = value.Replace("'", "\\\'"); //出现'时必须转转义成\'
            value = value.Replace("|", "\\|"); //批量分隔符|
            if (value.Length >= length - 3) //必须保留三个字符做最后的判断, 如最后一个字符出现了\或'时出错问题
                value = value.Substring(0, (int)length);
        }

        /// <summary>
        /// 当项目在其他电脑上使用时可快速还原数据库信息和所有数据表
        /// </summary>
        public void CreateTables()
        {
            connStr = @"Data Source='127.0.0.1';Port=3306;User Id='root';Password='titansiege';charset='utf8mb4';pooling=true;useCompression=true;allowBatch=true;connectionTimeout=60;allowloadlocalinfile=true;";
            InitConnection();
            int count = (int)ExecuteScalar<long>($"SELECT COUNT(*) FROM information_schema.schemata WHERE schema_name = 'Titansiege'");
            if (count <= 0) 
            {
                count = ExecuteNonQuery(@"CREATE DATABASE `titansiege` /*!40100 DEFAULT CHARACTER SET latin1 */");
                NDebug.Log($"创建数据库:Titansiege{(count >= 0 ? "成功" : "失败")}!");
                if (count <= 0)
                    return;
                CloseConnect();
                connStr = @"Database='titansiege';Data Source='127.0.0.1';Port=3306;User Id='root';Password='titansiege';charset='utf8mb4';pooling=true;useCompression=true;allowBatch=true;connectionTimeout=60;allowloadlocalinfile=true;";
                InitConnection();
 // -- 6
                count = (int)ExecuteScalar<long>($"SELECT COUNT(*) FROM information_schema.tables WHERE table_schema = 'Titansiege' AND table_name = 'characters'");
                if (count <= 0)
                {
                    count = ExecuteNonQuery(@"CREATE TABLE `characters` (
  `ID` bigint(11) NOT NULL,
  `Name` varchar(50) NOT NULL,
  `Zhiye` tinyint(4) NOT NULL DEFAULT '0' COMMENT '职业',
  `Level` tinyint(4) NOT NULL DEFAULT '1' COMMENT '等级',
  `levelupid` int(11) NOT NULL DEFAULT '0' COMMENT '升级配置表的id',
  `Exp` int(11) NOT NULL DEFAULT '0' COMMENT '经验',
  `Shengming` int(11) NOT NULL DEFAULT '100' COMMENT '生命',
  `Fali` int(11) NOT NULL DEFAULT '100' COMMENT '法力',
  `Tizhi` smallint(6) NOT NULL DEFAULT '1' COMMENT '体质',
  `Liliang` smallint(6) NOT NULL DEFAULT '1' COMMENT '力量',
  `Minjie` smallint(6) NOT NULL DEFAULT '1' COMMENT '敏捷',
  `Moli` smallint(6) NOT NULL DEFAULT '1' COMMENT '魔力',
  `Meili` smallint(6) NOT NULL DEFAULT '1' COMMENT '魅力',
  `Xingyun` smallint(6) NOT NULL DEFAULT '1' COMMENT '幸运',
  `Lianjin` smallint(6) NOT NULL DEFAULT '0' COMMENT '炼金',
  `Duanzao` smallint(6) NOT NULL DEFAULT '0' COMMENT '锻造',
  `Jinbi` int(11) NOT NULL DEFAULT '0' COMMENT '金币',
  `Zuanshi` int(11) NOT NULL DEFAULT '0' COMMENT '钻石',
  `Chenghao` varchar(50) DEFAULT NULL COMMENT '称号',
  `Friends` varchar(400) DEFAULT NULL COMMENT '亲朋',
  `Skills` varchar(200) DEFAULT NULL COMMENT '技能',
  `Prefabpath` varchar(100) DEFAULT NULL COMMENT '预制体路径',
  `Headpath` varchar(100) DEFAULT NULL COMMENT '头像路径',
  `Lihuipath` varchar(100) DEFAULT NULL COMMENT '立绘路径',
  `Wuqi` smallint(6) NOT NULL DEFAULT '-1' COMMENT '武器',
  `Toukui` smallint(6) NOT NULL DEFAULT '-1' COMMENT '头盔',
  `Yifu` smallint(6) NOT NULL DEFAULT '-1' COMMENT '衣服',
  `Xiezi` smallint(6) NOT NULL DEFAULT '-1' COMMENT '鞋子',
  `MapID` int(11) NOT NULL DEFAULT '0',
  `MapPosX` int(11) NOT NULL DEFAULT '0',
  `MapPosY` int(11) NOT NULL DEFAULT '0',
  `MapPosZ` int(11) NOT NULL DEFAULT '0',
  `Uid` bigint(11) NOT NULL,
  `LastDate` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '最后登录时间',
  `DelRole` tinyint(1) NOT NULL DEFAULT '0' COMMENT '是否删除',
  PRIMARY KEY (`ID`),
  UNIQUE KEY `ID` (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4");
                    NDebug.Log($"创建数据表:characters{(count >= 0 ? "成功" : "失败")}!");
                }
                else NDebug.Log($"数据表:characters已存在!");
 // -- 6
                count = (int)ExecuteScalar<long>($"SELECT COUNT(*) FROM information_schema.tables WHERE table_schema = 'Titansiege' AND table_name = 'config'");
                if (count <= 0)
                {
                    count = ExecuteNonQuery(@"CREATE TABLE `config` (
  `id` int(10) unsigned NOT NULL AUTO_INCREMENT COMMENT '表id',
  `tname` varchar(20) CHARACTER SET utf8mb4 NOT NULL COMMENT '表名称',
  `count` int(11) NOT NULL COMMENT '表计数',
  `describle` varchar(200) CHARACTER SET utf8mb4 DEFAULT NULL COMMENT '表描述',
  PRIMARY KEY (`id`),
  UNIQUE KEY `id` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=latin1 COMMENT='配置表，将数据库所有数据表配置到这里面备用'");
                    NDebug.Log($"创建数据表:config{(count >= 0 ? "成功" : "失败")}!");
                }
                else NDebug.Log($"数据表:config已存在!");
 // -- 6
                count = (int)ExecuteScalar<long>($"SELECT COUNT(*) FROM information_schema.tables WHERE table_schema = 'Titansiege' AND table_name = 'npcs'");
                if (count <= 0)
                {
                    count = ExecuteNonQuery(@"CREATE TABLE `npcs` (
  `ID` bigint(11) NOT NULL,
  `Zhiye` tinyint(4) NOT NULL DEFAULT '0' COMMENT '职业',
  `Level` tinyint(4) NOT NULL DEFAULT '1' COMMENT '等级',
  `Exp` int(11) NOT NULL DEFAULT '0' COMMENT '经验',
  `Shengming` int(11) NOT NULL DEFAULT '100' COMMENT '生命',
  `Fali` int(11) NOT NULL DEFAULT '100' COMMENT '法力',
  `Tizhi` smallint(6) NOT NULL DEFAULT '1' COMMENT '体质',
  `Liliang` smallint(6) NOT NULL DEFAULT '1' COMMENT '力量',
  `Minjie` smallint(6) NOT NULL DEFAULT '1' COMMENT '敏捷',
  `Moli` smallint(6) NOT NULL DEFAULT '1' COMMENT '魔力',
  `Meili` smallint(6) NOT NULL DEFAULT '1' COMMENT '魅力',
  `Xingyun` smallint(6) NOT NULL DEFAULT '1' COMMENT '幸运',
  `Jinbi` int(11) NOT NULL DEFAULT '0' COMMENT '金币',
  `Zuanshi` int(11) NOT NULL DEFAULT '0' COMMENT '钻石',
  `Skills` varchar(200) DEFAULT NULL COMMENT '技能',
  `Prefabpath` varchar(100) DEFAULT NULL COMMENT '预制体路径',
  `Headpath` varchar(100) DEFAULT NULL COMMENT '头像路径',
  `Lihuipath` varchar(100) DEFAULT NULL COMMENT '立绘路径',
  `LastDate` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '最后登录时间',
  PRIMARY KEY (`ID`),
  UNIQUE KEY `ID` (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4");
                    NDebug.Log($"创建数据表:npcs{(count >= 0 ? "成功" : "失败")}!");
                }
                else NDebug.Log($"数据表:npcs已存在!");
 // -- 6
                count = (int)ExecuteScalar<long>($"SELECT COUNT(*) FROM information_schema.tables WHERE table_schema = 'Titansiege' AND table_name = 'users'");
                if (count <= 0)
                {
                    count = ExecuteNonQuery(@"CREATE TABLE `users` (
  `ID` bigint(20) NOT NULL,
  `Username` varchar(50) NOT NULL,
  `Password` varchar(50) NOT NULL,
  `RegisterDate` datetime DEFAULT CURRENT_TIMESTAMP,
  `Email` varchar(50) DEFAULT NULL,
  PRIMARY KEY (`ID`),
  UNIQUE KEY `ID` (`ID`),
  UNIQUE KEY `Username_2` (`Username`),
  UNIQUE KEY `Email` (`Email`),
  KEY `Username` (`Username`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4");
                    NDebug.Log($"创建数据表:users{(count >= 0 ? "成功" : "失败")}!");
                }
                else NDebug.Log($"数据表:users已存在!");
 // -- 7
            }
            else NDebug.Log($"数据库:Titansiege已存在!");
        }
    }
}
#endif
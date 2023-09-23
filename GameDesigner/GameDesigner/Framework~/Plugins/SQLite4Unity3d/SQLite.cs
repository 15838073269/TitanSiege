//
// 版权所有(C)2009-2012克鲁格系统公司。
// 
// 特此向任何获得副本的人免费授予许可
// 本软件及相关文档文件(本软件)，以处理
// 在软件中不受限制，包括但不限于
// 使用、复制、修改、合并、发布、分发、再许可和/或销售
// 本软件的副本，并允许本软件的接受者
// 在满足以下条件的情况下提供这样的服务：
// 
// 上述版权声明和本许可声明应包含在
// 本软件的所有副本或主要部分。
// 
// 本软件按原样提供，不提供任何形式的担保，明示或
// 默示，包括但不限于适销性保证，
// 特定目的的适合性和不侵权。在任何情况下，
// 作者或版权所有者对任何索赔、损害或其他责任|！|
// 责任，无论是在合同、侵权或其他诉讼中，
// 本软件之外的或与之相关的，或与本软件的使用或其他交易有关的
// 软件。
//
#if WINDOWS_PHONE && !USE_WP8_NATIVE_SQLITE
#define USE_CSHARP_SQLITE
#endif

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;

#if USE_CSHARP_SQLITE
using Sqlite3 = Community.CsharpSqlite.Sqlite3;
using Sqlite3DatabaseHandle = Community.CsharpSqlite.Sqlite3.sqlite3;
using Sqlite3Statement = Community.CsharpSqlite.Sqlite3.Vdbe;
#elif USE_WP8_NATIVE_SQLITE
using Sqlite3 = Sqlite.Sqlite3;
using Sqlite3DatabaseHandle = Sqlite.Database;
using Sqlite3Statement = Sqlite.Statement;
#else
using Sqlite3DatabaseHandle = System.IntPtr;
using Sqlite3Statement = System.IntPtr;
#endif

namespace SQLite4Unity3d
{
    public class SQLiteException : Exception
    {
        public SQLite3.Result Result { get; private set; }

        protected SQLiteException(SQLite3.Result r, string message) : base(message)
        {
            Result = r;
        }

        public static SQLiteException New(SQLite3.Result r, string message)
        {
            return new SQLiteException(r, message);
        }
    }

    public class NotNullConstraintViolationException : SQLiteException
    {
        public IEnumerable<TableMapping.Column> Columns { get; protected set; }

        protected NotNullConstraintViolationException(SQLite3.Result r, string message)
            : this(r, message, null, null)
        {

        }

        protected NotNullConstraintViolationException(SQLite3.Result r, string message, TableMapping mapping, object obj)
            : base(r, message)
        {
            if (mapping != null && obj != null)
            {
                this.Columns = from c in mapping.Columns
                               where c.IsNullable == false && c.GetValue(obj) == null
                               select c;
            }
        }

        public static new NotNullConstraintViolationException New(SQLite3.Result r, string message)
        {
            return new NotNullConstraintViolationException(r, message);
        }

        public static NotNullConstraintViolationException New(SQLite3.Result r, string message, TableMapping mapping, object obj)
        {
            return new NotNullConstraintViolationException(r, message, mapping, obj);
        }

        public static NotNullConstraintViolationException New(SQLiteException exception, TableMapping mapping, object obj)
        {
            return new NotNullConstraintViolationException(exception.Result, exception.Message, mapping, obj);
        }
    }

    [Flags]
    public enum SQLiteOpenFlags
    {
        ReadOnly = 1, ReadWrite = 2, Create = 4,
        NoMutex = 0x8000, FullMutex = 0x10000,
        SharedCache = 0x20000, PrivateCache = 0x40000,
        ProtectionComplete = 0x00100000,
        ProtectionCompleteUnlessOpen = 0x00200000,
        ProtectionCompleteUntilFirstUserAuthentication = 0x00300000,
        ProtectionNone = 0x00400000
    }

    [Flags]
    public enum CreateFlags
    {
        None = 0,
        ImplicitPK = 1,    // 为名为‘ID’(Orm.ImplitPkName)的字段创建主键
        ImplicitIndex = 2, // 为以‘ID’结尾的字段创建索引(Orm.ImplittIndexSuffix)
        AllImplicit = 3,   // 执行上述两项操作

        AutoIncPK = 4      // 强制主键字段为汽车公司
    }

    /// <summary>
    /// 表示到SQLite数据库的打开连接。
    /// </summary>
    public partial class SQLiteConnection : IDisposable
    {
        private bool _open;
        private TimeSpan _busyTimeout;
        private Dictionary<string, TableMapping> _mappings = null;
        private Dictionary<string, TableMapping> _tables = null;
        private System.Diagnostics.Stopwatch _sw;
        private TimeSpan _elapsed = default(TimeSpan);

        private int _transactionDepth = 0;
        private Random _rand = new Random();

        public Sqlite3DatabaseHandle Handle { get; private set; }
        internal static readonly Sqlite3DatabaseHandle NullHandle = default(Sqlite3DatabaseHandle);

        public string DatabasePath { get; private set; }

        // 同步对象词典。
        //
        // 为防止数据库中断，必须*同步*访问数据库文件。
        // 为此，我们为每个数据库文件创建同步对象，并将其存储在
        // 在所有连接之间共享静态词典。
        // 字典的key为数据库文件路径，其值为要使用的对象
        // by lock()语句。
        //
        // 使用案例：
        // -数据库文件锁定是隐式自动完成的。
        // -要预先锁定死锁，应用程序可以通过以下任一方式显式锁定数据库文件：
        // 
        // -RunInDatabaseLock(Action)类似地锁定数据库，但没有事务(用于查询)
        private static Dictionary<string, object> syncObjects = new Dictionary<string, object>();

        #region debug tracing

        public bool Trace { get; set; }
        public bool TimeExecution { get; set; }

        public delegate void TraceHandler(string message);
        public event TraceHandler TraceEvent;

        internal void InvokeTrace(string message)
        {
            if (TraceEvent != null)
            {
                TraceEvent(message);
            }
        }

        public delegate void TimeExecutionHandler(TimeSpan executionTime, TimeSpan totalExecutionTime);
        public event TimeExecutionHandler TimeExecutionEvent;

        internal void InvokeTimeExecution(TimeSpan executionTime, TimeSpan totalExecutionTime)
        {
            if (TimeExecutionEvent != null)
            {
                TimeExecutionEvent(executionTime, totalExecutionTime);
            }
        }

        #endregion

        public bool StoreDateTimeAsTicks { get; private set; }

        /// <summary>
        /// 构造一个新的SQLiteConnection并打开由lobasePath指定的SQLite数据库。
        /// </summary>
        /// <param name="databasePath">
        /// 指定数据库文件的路径。
        /// </param>
        /// <param name="storeDateTimeAsTicks">
        /// 指定是否将DateTime属性存储为记号(True)或字符串(False)。你
        /// 绝对希望在所有新项目中将它们存储为勾号。FALSE的缺省值为
        /// 此处仅用于向后兼容。它的速度优势非常显著，没有
        /// 设置store DateTimeAsTicks=TRUE时的下侧。
        /// </param>
        public SQLiteConnection(string databasePath, bool storeDateTimeAsTicks = false)
            : this(databasePath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create, storeDateTimeAsTicks)
        {
        }

        /// <summary>
        /// 构造一个新的SQLiteConnection并打开由lobasePath指定的SQLite数据库。
        /// </summary>
        /// <param name="databasePath">
        /// 指定数据库文件的路径。
        /// </param>
        /// <param name="storeDateTimeAsTicks">
        /// 指定是否将DateTime属性存储为记号(True)或字符串(False)。你
        /// 
        /// 此处仅用于向后兼容。它的速度优势非常显著，没有
        /// 设置store DateTimeAsTicks=TRUE时的下侧。
        /// </param>
        public SQLiteConnection(string databasePath, SQLiteOpenFlags openFlags, bool storeDateTimeAsTicks = false)
        {
            if (string.IsNullOrEmpty(databasePath))
                throw new ArgumentException("Must be specified", "databasePath");

            DatabasePath = databasePath;
            mayCreateSyncObject(databasePath);

#if NETFX_CORE
			SQLite3.SetDirectory(/*temp directory type*/2, Windows.Storage.ApplicationData.Current.TemporaryFolder.Path);
#endif

            Sqlite3DatabaseHandle handle;

#if SILVERLIGHT || USE_CSHARP_SQLITE
			var r = SQLite3.Open (databasePath, out handle, (int)openFlags, IntPtr.Zero);
#else
            // 使用字节[]打开
            // 在路径可能包含Unicode的情况下
            // 使用SQlite3_OPEN_v2强制开放使用UTF-8
            var databasePathAsBytes = GetNullTerminatedUtf8(DatabasePath);
            var r = SQLite3.Open(databasePathAsBytes, out handle, (int)openFlags, IntPtr.Zero);
#endif

            Handle = handle;
            if (r != SQLite3.Result.OK)
            {
                throw SQLiteException.New(r, String.Format("Could not open database file: {0} ({1})", DatabasePath, r));
            }
            _open = true;

            StoreDateTimeAsTicks = storeDateTimeAsTicks;

            BusyTimeout = TimeSpan.FromSeconds(0.1);
        }

        static SQLiteConnection()
        {
            if (_preserveDuringLinkMagic)
            {
                var ti = new ColumnInfo();
                ti.Name = "magic";
            }
        }

        void mayCreateSyncObject(string databasePath)
        {
            if (!syncObjects.ContainsKey(databasePath))
            {
                syncObjects[databasePath] = new object();
            }
        }

        /// <summary>
        /// 获取要锁定数据库文件以进行更新的同步对象。
        /// </summary>
        /// <value> 同步对象。</value>
        public object SyncObject { get { return syncObjects[DatabasePath]; } }

        public void EnableLoadExtension(int onoff)
        {
            SQLite3.Result r = SQLite3.EnableLoadExtension(Handle, onoff);
            if (r != SQLite3.Result.OK)
            {
                string msg = SQLite3.GetErrmsg(Handle);
                throw SQLiteException.New(r, msg);
            }
        }

        static byte[] GetNullTerminatedUtf8(string s)
        {
            var utf8Length = System.Text.Encoding.UTF8.GetByteCount(s);
            var bytes = new byte[utf8Length + 1];
            utf8Length = System.Text.Encoding.UTF8.GetBytes(s, 0, s.Length, bytes, 0);
            return bytes;
        }

        /// <summary>
        /// 用于列出我们需要的MonoTouch链接器的一些代码
        /// 去看，但我们永远不想真正执行。
        /// </summary>
#pragma warning disable 649
        static bool _preserveDuringLinkMagic;
#pragma warning restore 649

        /// <summary>
        /// 当表被锁定时，将繁忙处理程序设置为休眠指定的时间量。
        /// The handler will sleep multiple times until a total time of <see cref="BusyTimeout"/> has accumulated.
        /// </summary>
        public TimeSpan BusyTimeout
        {
            get { return _busyTimeout; }
            set
            {
                _busyTimeout = value;
                if (Handle != NullHandle)
                {
                    SQLite3.BusyTimeout(Handle, (int)_busyTimeout.TotalMilliseconds);
                }
            }
        }

        /// <summary>
        /// 返回连接从类型到表的映射
        /// 当前理解。
        /// </summary>
        public IEnumerable<TableMapping> TableMappings
        {
            get
            {
                return _tables != null ? _tables.Values : Enumerable.Empty<TableMapping>();
            }
        }

        /// <summary>
        /// 检索为给定类型自动生成的映射。
        /// </summary>
        /// <param name="type">
        /// 返回映射到数据库的类型。
        /// </param>         
        /// <param name="createFlags">
        /// 可选标志，允许基于命名约定的隐式主键和索引
        /// </param>     
        /// <returns>
        /// 映射表示数据库列的模式，包含
        /// 设置和获取对象属性的方法。
        /// </returns>
        public TableMapping GetMapping(Type type, CreateFlags createFlags = CreateFlags.None)
        {
            if (_mappings == null)
            {
                _mappings = new Dictionary<string, TableMapping>();
            }
            TableMapping map;
            if (!_mappings.TryGetValue(type.FullName, out map))
            {
                map = new TableMapping(type, createFlags);
                _mappings[type.FullName] = map;
            }
            return map;
        }

        /// <summary>
        /// 检索为给定类型自动生成的映射。
        /// </summary>
        /// <returns>
        /// 映射表示数据库列的模式，并包含
        /// 设置和获取对象属性的方法。
        /// </returns>
        public TableMapping GetMapping<T>()
        {
            return GetMapping(typeof(T));
        }

        private struct IndexedColumn
        {
            public int Order;
            public string ColumnName;
        }

        private struct IndexInfo
        {
            public string IndexName;
            public string TableName;
            public bool Unique;
            public List<IndexedColumn> Columns;
        }

        /// <summary>
        /// 在数据库上执行"DROP TABLE"。这是不可恢复的。
        /// </summary>
        public int DropTable<T>()
        {
            var map = GetMapping(typeof(T));

            var query = string.Format("drop table if exists \"{0}\"", map.TableName);

            return Execute(query);
        }

        /// <summary>
        /// 在数据库上执行"如果不存在CREATE TABLE"。它还
        /// 为表的列创建任何指定的索引。它使用
        /// 从指定类型自动生成的架构。你可以的
        /// 稍后通过调用Getmap访问此架构。
        /// </summary>
        /// <returns>
        /// 添加到数据库模式的条目数。
        /// </returns>
        public int CreateTable<T>(CreateFlags createFlags = CreateFlags.None)
        {
            return CreateTable(typeof(T), createFlags);
        }

        /// <summary>
        /// 在数据库上执行"CREATE TABLE如果不存在"。它还
        /// 为表的列创建任何指定的索引。它使用
        /// 从指定类型自动生成的模式。你可以的
        /// 稍后通过调用Getmap访问此架构。
        /// </summary>
        /// <param name="ty"> </param>
        /// <param name="createFlags"> 可选标志，允许基于命名约定的隐式主键和索引。</param>  
        /// <returns>
        /// 添加到数据库架构的条目数。
        /// </returns>
        public int CreateTable(Type ty, CreateFlags createFlags = CreateFlags.None)
        {
            if (_tables == null)
            {
                _tables = new Dictionary<string, TableMapping>();
            }
            TableMapping map;
            if (!_tables.TryGetValue(ty.FullName, out map))
            {
                map = GetMapping(ty, createFlags);
                _tables.Add(ty.FullName, map);
            }
            var query = "create table if not exists \"" + map.TableName + "\"(\n";

            var decls = map.Columns.Select(p => Orm.SqlDecl(p, StoreDateTimeAsTicks));
            var decl = string.Join(",\n", decls.ToArray());
            query += decl;
            query += ")";

            var count = Execute(query);

            if (count == 0)
            { // 可能的错误：这似乎总是返回0？
              // 表已存在，迁移
                MigrateTable(map);
            }

            var indexes = new Dictionary<string, IndexInfo>();
            foreach (var c in map.Columns)
            {
                foreach (var i in c.Indices)
                {
                    var iname = i.Name ?? map.TableName + "_" + c.Name;
                    IndexInfo iinfo;
                    if (!indexes.TryGetValue(iname, out iinfo))
                    {
                        iinfo = new IndexInfo
                        {
                            IndexName = iname,
                            TableName = map.TableName,
                            Unique = i.Unique,
                            Columns = new List<IndexedColumn>()
                        };
                        indexes.Add(iname, iinfo);
                    }

                    if (i.Unique != iinfo.Unique)
                        throw new Exception("All the columns in an index must have the same value for their Unique property");

                    iinfo.Columns.Add(new IndexedColumn
                    {
                        Order = i.Order,
                        ColumnName = c.Name
                    });
                }
            }

            foreach (var indexName in indexes.Keys)
            {
                var index = indexes[indexName];
                string[] columnNames = new string[index.Columns.Count];
                if (index.Columns.Count == 1)
                {
                    columnNames[0] = index.Columns[0].ColumnName;
                }
                else
                {
                    index.Columns.Sort((lhs, rhs) => {
                        return lhs.Order - rhs.Order;
                    });
                    for (int i = 0, end = index.Columns.Count; i < end; ++i)
                    {
                        columnNames[i] = index.Columns[i].ColumnName;
                    }
                }
                count += CreateIndex(indexName, index.TableName, columnNames, index.Unique);
            }

            return count;
        }

        /// <summary>
        /// 为指定的表和列创建索引。
        /// </summary>
        /// <param name="indexName"> 要创建的索引的名称</param>
        /// <param name="tableName"> 数据库表名</param>
        /// <param name="columnNames"> </param>
        /// <param name="unique"> </param>
        public int CreateIndex(string indexName, string tableName, string[] columnNames, bool unique = false)
        {
            const string sqlFormat = "create {2} index if not exists \"{3}\" on \"{0}\"(\"{1}\")";
            var sql = String.Format(sqlFormat, tableName, string.Join("\", \"", columnNames), unique ? "unique" : "", indexName);
            return Execute(sql);
        }

        /// <summary>
        /// 为指定的表和列创建索引。
        /// </summary>
        /// <param name="indexName"> </param>
        /// <param name="tableName"> 数据库表名</param>
        /// <param name="columnName"> 要编制索引的列的名称</param>
        /// <param name="unique"> </param>
        public int CreateIndex(string indexName, string tableName, string columnName, bool unique = false)
        {
            return CreateIndex(indexName, tableName, new string[] { columnName }, unique);
        }

        /// <summary>
        /// 为指定的表和列创建索引。
        /// </summary>
        /// <param name="tableName"> </param>
        /// <param name="columnName"> </param>
        /// <param name="unique"> </param>
        public int CreateIndex(string tableName, string columnName, bool unique = false)
        {
            return CreateIndex(tableName + "_" + columnName, tableName, columnName, unique);
        }

        /// <summary>
        /// 为指定的表和列创建索引。
        /// </summary>
        /// <param name="tableName"> </param>
        /// <param name="columnNames"> </param>
        /// <param name="unique"> </param>
        public int CreateIndex(string tableName, string[] columnNames, bool unique = false)
        {
            return CreateIndex(tableName + "_" + string.Join("_", columnNames), tableName, columnNames, unique);
        }

        /// <summary>
        /// 为指定的对象属性创建索引。
        /// 例如CreateIndex<客户端>(c=>c.Name)；
        /// </summary>
        /// <typeparam name="T">键入以反映到数据库表。</typeparam>
        /// <param name="property"> 要编制索引的属性</param>
        /// <param name="unique"> 索引是否应唯一</param>
        public void CreateIndex<T>(Expression<Func<T, object>> property, bool unique = false)
        {
            MemberExpression mx;
            if (property.Body.NodeType == ExpressionType.Convert)
            {
                mx = ((UnaryExpression)property.Body).Operand as MemberExpression;
            }
            else
            {
                mx = (property.Body as MemberExpression);
            }
            var propertyInfo = mx.Member as PropertyInfo;
            if (propertyInfo == null)
            {
                throw new ArgumentException("The lambda expression 'property' should point to a valid Property");
            }

            var propName = propertyInfo.Name;

            var map = GetMapping<T>();
            var colName = map.FindColumnWithPropertyName(propName).Name;

            CreateIndex(map.TableName, colName, unique);
        }

        public class ColumnInfo
        {
            // PUBLIC INT CID{GET；SET；}

            [Column("name")]
            public string Name { get; set; }

            // [列("类型")]
            // 公共字符串列类型{get；set；}

            public int notnull { get; set; }

            // 公共字符串dflt_value{get；set；}

            // 

            public override string ToString()
            {
                return Name;
            }
        }

        public List<ColumnInfo> GetTableInfo(string tableName)
        {
            var query = "pragma table_info(\"" + tableName + "\")";
            return Query<ColumnInfo>(query);
        }

        void MigrateTable(TableMapping map)
        {
            var existingCols = GetTableInfo(map.TableName);

            var toBeAdded = new List<TableMapping.Column>();

            foreach (var p in map.Columns)
            {
                var found = false;
                foreach (var c in existingCols)
                {
                    found = (string.Compare(p.Name, c.Name, StringComparison.OrdinalIgnoreCase) == 0);
                    if (found)
                        break;
                }
                if (!found)
                {
                    toBeAdded.Add(p);
                }
            }

            foreach (var p in toBeAdded)
            {
                var addCol = "alter table \"" + map.TableName + "\" add column " + Orm.SqlDecl(p, StoreDateTimeAsTicks);
                Execute(addCol);
            }
        }

        /// <summary>
        /// 创建新的SQLiteCommand。可以重写以提供子类。
        /// </summary>
        /// <see cref="SQLiteCommand.OnInstanceCreated"/>
        protected virtual SQLiteCommand NewCommand()
        {
            return new SQLiteCommand(this);
        }

        /// <summary>
        /// 在给定带有参数的命令文本的情况下创建新的SQLiteCommand。放一个‘？’
        /// 在每个参数的命令文本中。
        /// </summary>
        /// <param name="cmdText">
        /// 完全转义的SQL。
        /// </param>
        /// <param name="args">
        /// -取代"？"出现的论点。在命令文本中。
        /// </param>
        /// <returns>
        /// A <see cref="SQLiteCommand"/>
        /// </returns>
        public SQLiteCommand CreateCommand(string cmdText, params object[] ps)
        {
            if (!_open)
                throw SQLiteException.New(SQLite3.Result.Error, "Cannot create commands from unopened database");

            var cmd = NewCommand();
            cmd.CommandText = cmdText;
            foreach (var o in ps)
            {
                cmd.Bind(o);
            }
            return cmd;
        }

        /// <summary>
        /// 使用带参数的命令文本(SQL)创建一个SQLiteCommand。放一个‘？’
        /// 在每个参数的命令文本中，然后执行该命令。
        /// 当您不期望返回行时，使用此方法而不是Query。这类案件包括
        /// 插入、更新和删除。
        /// 您可以设置连接的跟踪或TimeExecution属性
        /// 分析执行情况。
        /// </summary>
        /// <param name="query">
        /// 完全转义的SQL。
        /// </param>
        /// <param name="args">
        /// -取代"？"的论据。在查询中。
        /// </param>
        /// <returns>
        /// 执行此操作后在数据库中修改的行数。
        /// </returns>
        public int Execute(string query, params object[] args)
        {
            var cmd = CreateCommand(query, args);

            if (TimeExecution)
            {
                if (_sw == null)
                {
                    _sw = new Stopwatch();
                }
                _sw.Reset();
                _sw.Start();
            }

            var r = cmd.ExecuteNonQuery();

            if (TimeExecution)
            {
                _sw.Stop();
                _elapsed += _sw.Elapsed;
                this.InvokeTimeExecution(_sw.Elapsed, _elapsed);
            }

            return r;
        }

        public T ExecuteScalar<T>(string query, params object[] args)
        {
            var cmd = CreateCommand(query, args);

            if (TimeExecution)
            {
                if (_sw == null)
                {
                    _sw = new Stopwatch();
                }
                _sw.Reset();
                _sw.Start();
            }

            var r = cmd.ExecuteScalar<T>();

            if (TimeExecution)
            {
                _sw.Stop();
                _elapsed += _sw.Elapsed;
                this.InvokeTimeExecution(_sw.Elapsed, _elapsed);
            }

            return r;
        }

        /// <summary>
        /// 使用带参数的命令文本(SQL)创建一个SQLiteCommand。放一个‘？’
        /// 在每个参数的命令文本中，然后执行该命令。
        /// 
        /// 给定类型。
        /// </summary>
        /// <param name="query">
        /// 完全转义的SQL。
        /// </param>
        /// <param name="args">
        /// 用来取代"？"的论点。在查询中。
        /// </param>
        /// <returns>
        /// 一个可枚举值，查询返回的每一行对应一个结果。
        /// </returns>
        public List<T> Query<T>(string query, params object[] args) where T : new()
        {
            var cmd = CreateCommand(query, args);
            return cmd.ExecuteQuery<T>();
        }

        /// <summary>
        /// 在给定带有参数的命令文本(SQL)的情况下创建SQLiteCommand。放一个‘？’
        /// 在每个参数的命令文本中，然后执行该命令。
        /// 它使用自动生成的映射返回结果的每一行
        /// 给定类型。
        /// </summary>
        /// <param name="query">
        /// 完全转义的SQL。
        /// </param>
        /// <param name="args">
        /// 
        /// </param>
        /// <returns>
        /// 一个可枚举值，查询返回的每一行对应一个结果。
        /// 枚举数将在每次调用MoveNext时调用SQlite3_Step，因此数据库
        /// 
        /// </returns>
        public IEnumerable<T> DeferredQuery<T>(string query, params object[] args) where T : new()
        {
            var cmd = CreateCommand(query, args);
            return cmd.ExecuteDeferredQuery<T>();
        }

        /// <summary>
        /// 使用带参数的命令文本(SQL)创建一个SQLiteCommand。放一个‘？’
        /// 在每个参数的命令文本中，然后执行该命令。
        /// 它使用指定的映射返回结果的每一行。此函数为
        /// 仅供图书馆使用，用于通过自省查询数据库。它是
        /// 通常不使用。
        /// </summary>
        /// <param name="map">
        /// A <see cref="TableMapping"/> to use to convert the resulting rows
        /// 转化为对象。
        /// </param>
        /// <param name="query">
        /// 
        /// </param>
        /// <param name="args">
        /// 
        /// </param>
        /// <returns>
        /// 一个可枚举值，查询返回的每一行对应一个结果。
        /// </returns>
        public List<object> Query(TableMapping map, string query, params object[] args)
        {
            var cmd = CreateCommand(query, args);
            return cmd.ExecuteQuery<object>(map);
        }

        /// <summary>
        /// 在给定带有参数的命令文本(SQL)的情况下创建SQLiteCommand。放一个‘？’
        /// 在每个参数的命令文本中，然后执行该命令。
        /// 它使用指定的映射返回结果的每一行。此函数为
        /// 
        /// 通常不使用。
        /// </summary>
        /// <param name="map">
        /// A <see cref="TableMapping"/> to use to convert the resulting rows
        /// 转化为对象。
        /// </param>
        /// <param name="query">
        /// 完全转义的SQL。
        /// </param>
        /// <param name="args">
        /// 
        /// </param>
        /// <returns>
        /// 一个可枚举值，查询返回的每一行都有一个结果。
        /// 枚举数将在每次调用MoveNext时调用SQlite3_Step，因此数据库
        /// 连接必须在枚举器的生存期内保持打开。
        /// </returns>
        public IEnumerable<object> DeferredQuery(TableMapping map, string query, params object[] args)
        {
            var cmd = CreateCommand(query, args);
            return cmd.ExecuteDeferredQuery<object>(map);
        }

        /// <summary>
        /// 返回由给定类型表示的表的可查询接口。
        /// </summary>
        /// <returns>
        /// 能够转换Where、OrderBy和Take的可查询对象
        /// 查询原生SQL。
        /// </returns>
        public TableQuery<T> Table<T>() where T : new()
        {
            return new TableQuery<T>(this);
        }

        /// <summary>
        /// 尝试从表中检索具有给定主键的对象
        /// 与指定类型关联。使用此方法需要
        /// 
        /// </summary>
        /// <param name="pk">
        /// 主键。
        /// </param>
        /// <returns>
        /// 
        /// 如果找不到对象。
        /// </returns>
        public T Get<T>(object pk) where T : new()
        {
            var map = GetMapping(typeof(T));
            return Query<T>(map.GetByPrimaryKeySql, pk).First();
        }

        /// <summary>
        /// 尝试从表中检索与谓词匹配的第一个对象
        /// 与指定类型关联。
        /// </summary>
        /// <param name="predicate">
        /// 要查找的对象的谓词。
        /// </param>
        /// <returns>
        /// 
        /// 如果未找到对象。
        /// </returns>
        public T Get<T>(Expression<Func<T, bool>> predicate) where T : new()
        {
            return Table<T>().Where(predicate).First();
        }

        /// <summary>
        /// 尝试从表中检索具有给定主键的对象
        /// 与指定类型关联。使用此方法需要
        /// 给定类型具有指定的PrimaryKey(使用PrimaryKeyAttribute)。
        /// </summary>
        /// <param name="pk">
        /// 
        /// </param>
        /// <returns>
        /// 
        /// 如果未找到对象。
        /// </returns>
        public T Find<T>(object pk) where T : new()
        {
            var map = GetMapping(typeof(T));
            return Query<T>(map.GetByPrimaryKeySql, pk).FirstOrDefault();
        }

        /// <summary>
        /// 尝试从表中检索具有给定主键的对象
        /// 与指定类型关联。使用此方法需要
        /// 给定类型具有指定的PrimaryKey(使用PrimaryKeyAttribute)。
        /// </summary>
        /// <param name="pk">
        /// 主键。
        /// </param>
        /// <param name="map">
        /// 用于标识对象类型的TableMap。
        /// </param>
        /// <returns>
        /// 
        /// 如果未找到对象。
        /// </returns>
        public object Find(object pk, TableMapping map)
        {
            return Query(map, map.GetByPrimaryKeySql, pk).FirstOrDefault();
        }

        /// <summary>
        /// 尝试从表中检索与谓词匹配的第一个对象
        /// 与指定类型关联。
        /// </summary>
        /// <param name="predicate">
        /// 要查找的对象的谓词。
        /// </param>
        /// <returns>
        /// 与给定谓词或空匹配的宾语
        /// 如果未找到对象。
        /// </returns>
        public T Find<T>(Expression<Func<T, bool>> predicate) where T : new()
        {
            return Table<T>().Where(predicate).FirstOrDefault();
        }

        /// <summary>
        /// Whether <see cref="BeginTransaction"/> has been called and the database is waiting for a <see cref="Commit"/>.
        /// </summary>
        public bool IsInTransaction
        {
            get { return _transactionDepth > 0; }
        }

        /// <summary>
        /// Begins a new transaction. Call <see cref="Commit"/> to end the transaction.
        /// </summary>
        /// <如果事务已经开始，则示例cref="System.InvalidOperationException">Throws。</Example>
        public void BeginTransaction()
        {
            // 
            // 
            // 
            // 则命令失败并出现错误。
            // 
            // 这将导致错误。
            if (Interlocked.CompareExchange(ref _transactionDepth, 1, 0) == 0)
            {
                try
                {
                    Execute("begin transaction");
                }
                catch (Exception ex)
                {
                    var sqlExp = ex as SQLiteException;
                    if (sqlExp != null)
                    {
                        // 
                        // 通过显式发出回滚命令。
                        // TODO：此回滚故障保护应本地化到所有抛出站点。
                        switch (sqlExp.Result)
                        {
                            case SQLite3.Result.IOError:
                            case SQLite3.Result.Full:
                            case SQLite3.Result.Busy:
                            case SQLite3.Result.NoMem:
                            case SQLite3.Result.Interrupt:
                                RollbackTo(null, true);
                                break;
                        }
                    }
                    else
                    {
                        // 
                        // 在捕获后在SaveTransactionPoint中创建了一个事务点。
                        Interlocked.Decrement(ref _transactionDepth);
                    }

                    throw;
                }
            }
            else
            {
                // 对已打开的事务调用BeginTransaction无效
                throw new InvalidOperationException("Cannot begin a transaction while already in a transaction.");
            }
        }

        /// <summary>
        /// 在事务时间线的当前点在数据库中创建保存点。
        /// 
        /// 
        /// Call <see cref="RollbackTo"/> to undo transactions since the returned savepoint.
        /// Call <see cref="Release"/> to commit transactions after the savepoint returned here.
        /// Call <see cref="Commit"/> to end the transaction, committing all changes.
        /// </summary>
        /// <returns> 命名保存点的字符串。</returns>
        public string SaveTransactionPoint()
        {
            int depth = Interlocked.Increment(ref _transactionDepth) - 1;
            string retVal = "S" + _rand.Next(short.MaxValue) + "D" + depth;

            try
            {
                Execute("savepoint " + retVal);
            }
            catch (Exception ex)
            {
                var sqlExp = ex as SQLiteException;
                if (sqlExp != null)
                {
                    // 
                    // 通过显式发出回滚命令。
                    // 
                    switch (sqlExp.Result)
                    {
                        case SQLite3.Result.IOError:
                        case SQLite3.Result.Full:
                        case SQLite3.Result.Busy:
                        case SQLite3.Result.NoMem:
                        case SQLite3.Result.Interrupt:
                            RollbackTo(null, true);
                            break;
                    }
                }
                else
                {
                    Interlocked.Decrement(ref _transactionDepth);
                }

                throw;
            }

            return retVal;
        }

        /// <summary>
        /// Rolls back the transaction that was begun by <see cref="BeginTransaction"/> or <see cref="SaveTransactionPoint"/>.
        /// </summary>
        public void Rollback()
        {
            RollbackTo(null, false);
        }

        /// <summary>
        /// Rolls back the savepoint created by <see cref="BeginTransaction"/> or SaveTransactionPoint.
        /// </summary>
        /// <param name="savepoint">The name of the savepoint to roll back to, as returned by <see cref="SaveTransactionPoint"/>.  If savepoint is null or empty, this method is equivalent to a call to <see cref="Rollback"/></param>
        public void RollbackTo(string savepoint)
        {
            RollbackTo(savepoint, false);
        }

        /// <summary>
        /// Rolls back the transaction that was begun by <see cref="BeginTransaction"/>.
        /// </summary>
        /// <param name="noThrow"> </param>
        void RollbackTo(string savepoint, bool noThrow)
        {
            // 
            // 并将事务堆栈留空。
            try
            {
                if (String.IsNullOrEmpty(savepoint))
                {
                    if (Interlocked.Exchange(ref _transactionDepth, 0) > 0)
                    {
                        Execute("rollback");
                    }
                }
                else
                {
                    DoSavePointExecute(savepoint, "rollback to ");
                }
            }
            catch (SQLiteException)
            {
                if (!noThrow)
                    throw;

            }
            // 
        }

        /// <summary>
        /// Releases a savepoint returned from <see cref="SaveTransactionPoint"/>.  Releasing a savepoint 
        /// 
        ///    or otherwise the changes are permanent pending a call to <see cref="Commit"/>.
        /// 
        /// 释放命令类似于保存点的提交。
        /// </summary>
        /// <param name="savepoint">The name of the savepoint to release.  The string should be the result of a call to <see cref="SaveTransactionPoint"/></param>
        public void Release(string savepoint)
        {
            DoSavePointExecute(savepoint, "release ");
        }

        void DoSavePointExecute(string savepoint, string cmd)
        {
            // 
            int firstLen = savepoint.IndexOf('D');
            if (firstLen >= 2 && savepoint.Length > firstLen + 1)
            {
                int depth;
                if (Int32.TryParse(savepoint.Substring(firstLen + 1), out depth))
                {
                    // 
                    if (0 <= depth && depth < _transactionDepth)
                    {
#if NETFX_CORE
						Volatile.Write (ref _transactionDepth, depth);
#elif SILVERLIGHT
						_transactionDepth = depth;
#else
                        Thread.VolatileWrite(ref _transactionDepth, depth);
#endif
                        Execute(cmd + savepoint);
                        return;
                    }
                }
            }

            throw new ArgumentException("savePoint is not valid, and should be the result of a call to SaveTransactionPoint.", "savePoint");
        }

        /// <summary>
        /// Commits the transaction that was begun by <see cref="BeginTransaction"/>.
        /// </summary>
        public void Commit()
        {
            if (Interlocked.Exchange(ref _transactionDepth, 0) != 0)
            {
                Execute("commit");
            }
            // 
        }

        /// <summary>
        /// Executes <param name="action"> 在(可能嵌套的)事务中，将其包装在保存点中。如果一个
        /// 
        /// 被重新抛出。
        /// </summary>
        /// <param name="action">
        /// The <see cref="Action"/> to perform within a transaction. <param name="action"> can contain any number
        /// of operations on the connection but should never call <see cref="BeginTransaction"/> or
        /// <see cref="Commit"/>.
        /// </param>
        public void RunInTransaction(Action action)
        {
            try
            {
                lock (syncObjects[DatabasePath])
                {
                    var savePoint = SaveTransactionPoint();
                    action();
                    Release(savePoint);
                }
            }
            catch (Exception)
            {
                Rollback();
                throw;
            }
        }

        /// <summary>
        /// Executes <param name="action"> 
        /// </summary>
        /// <param name="action">
        /// The <see cref="Action"/> to perform within a lock.
        /// </param>
        public void RunInDatabaseLock(Action action)
        {
            lock (syncObjects[DatabasePath])
            {
                action();
            }
        }

        /// <summary>
        /// 插入所有指定的对象。
        /// </summary>
        /// <param name="objects">
        /// An <see cref="IEnumerable"/> of the objects to insert.
        /// </param>
        /// <returns>
        /// 添加到表中的行数。
        /// </returns>
        public int InsertAll(System.Collections.IEnumerable objects)
        {
            var c = 0;
            RunInTransaction(() => {
                foreach (var r in objects)
                {
                    c += Insert(r);
                }
            });
            return c;
        }

        /// <summary>
        /// 插入所有指定的对象。
        /// </summary>
        /// <param name="objects">
        /// An <see cref="IEnumerable"/> of the objects to insert.
        /// </param>
        /// <param name="extra">
        /// 放置到命令中的文字SQL代码。将{Extra}插入到...
        /// </param>
        /// <returns>
        /// 添加到表中的行数。
        /// </returns>
        public int InsertAll(System.Collections.IEnumerable objects, string extra)
        {
            var c = 0;
            RunInTransaction(() => {
                foreach (var r in objects)
                {
                    c += Insert(r, extra);
                }
            });
            return c;
        }

        /// <summary>
        /// 插入所有指定的对象。
        /// </summary>
        /// <param name="objects">
        /// An <see cref="IEnumerable"/> of the objects to insert.
        /// </param>
        /// <param name="objType">
        /// 
        /// </param>
        /// <returns>
        /// 添加到表中的行数。
        /// </returns>
        public int InsertAll(System.Collections.IEnumerable objects, Type objType)
        {
            var c = 0;
            RunInTransaction(() => {
                foreach (var r in objects)
                {
                    c += Insert(r, objType);
                }
            });
            return c;
        }

        /// <summary>
        /// 插入给定对象并检索其
        /// 
        /// </summary>
        /// <param name="obj">
        /// 要插入的对象。
        /// </param>
        /// <returns>
        /// 
        /// </returns>
        public int Insert(object obj)
        {
            if (obj == null)
            {
                return 0;
            }
            return Insert(obj, "", obj.GetType());
        }

        /// <summary>
        /// 插入给定对象并检索其
        /// 
        /// 如果发生唯一约束冲突，
        /// 某些预先存在的对象，此函数删除
        /// 旧对象。
        /// </summary>
        /// <param name="obj">
        /// 要插入的对象。
        /// </param>
        /// <returns>
        /// 
        /// </returns>
        public int InsertOrReplace(object obj)
        {
            if (obj == null)
            {
                return 0;
            }
            return Insert(obj, "OR REPLACE", obj.GetType());
        }

        /// <summary>
        /// 插入给定对象并检索其
        /// 
        /// </summary>
        /// <param name="obj">
        /// 要插入的对象。
        /// </param>
        /// <param name="objType">
        /// 要插入的对象类型。
        /// </param>
        /// <returns>
        /// 
        /// </returns>
        public int Insert(object obj, Type objType)
        {
            return Insert(obj, "", objType);
        }

        /// <summary>
        /// 插入给定对象并检索其
        /// 
        /// 如果与发生唯一约束冲突
        /// 
        /// 旧对象。
        /// </summary>
        /// <param name="obj">
        /// 要插入的对象。
        /// </param>
        /// <param name="objType">
        /// 
        /// </param>
        /// <returns>
        /// 修改的行数。
        /// </returns>
        public int InsertOrReplace(object obj, Type objType)
        {
            return Insert(obj, "OR REPLACE", objType);
        }

        /// <summary>
        /// 
        /// 
        /// </summary>
        /// <param name="obj">
        /// 要插入的对象。
        /// </param>
        /// <param name="extra">
        /// 放置到命令中的文字SQL代码。将{Extra}插入到...
        /// </param>
        /// <returns>
        /// 添加到表中的行数。
        /// </returns>
        public int Insert(object obj, string extra)
        {
            if (obj == null)
            {
                return 0;
            }
            return Insert(obj, extra, obj.GetType());
        }

        /// <summary>
        /// 插入给定对象并检索其
        /// 
        /// </summary>
        /// <param name="obj">
        /// 要插入的对象。
        /// </param>
        /// <param name="extra">
        /// 放入命令中的文字SQL代码。将{Extra}插入到...
        /// </param>
        /// <param name="objType">
        /// 要插入的对象类型。
        /// </param>
        /// <returns>
        /// 
        /// </returns>
        public int Insert(object obj, string extra, Type objType)
        {
            if (obj == null || objType == null)
            {
                return 0;
            }


            var map = GetMapping(objType);

#if NETFX_CORE
			if (map.PK != null && map.PK.IsAutoGuid)
			{
				// 
				PropertyInfo prop;
				while (objType != null)
				{
					var info = objType.GetTypeInfo();
					prop = info.GetDeclaredProperty(map.PK.PropertyName);
					if (prop != null) 
					{
						if (prop.GetValue(obj, null).Equals(Guid.Empty))
						{
							prop.SetValue(obj, Guid.NewGuid(), null);
						}
						break; 
					}

					objType = info.BaseType;
				}
			}
#else
            if (map.PK != null && map.PK.IsAutoGuid)
            {
                var prop = objType.GetProperty(map.PK.PropertyName);
                if (prop != null)
                {
                    // if(pro.GetValue(obj，NULL).Equals(Guid.Empty)){
                    if (prop.GetGetMethod().Invoke(obj, null).Equals(Guid.Empty))
                    {
                        prop.SetValue(obj, Guid.NewGuid(), null);
                    }
                }
            }
#endif


            var replacing = string.Compare(extra, "OR REPLACE", StringComparison.OrdinalIgnoreCase) == 0;

            var cols = replacing ? map.InsertOrReplaceColumns : map.InsertColumns;
            var vals = new object[cols.Length];
            for (var i = 0; i < vals.Length; i++)
            {
                vals[i] = cols[i].GetValue(obj);
            }

            var insertCmd = map.GetInsertCommand(this, extra);
            int count;

            try
            {
                count = insertCmd.ExecuteNonQuery(vals);
            }
            catch (SQLiteException ex)
            {

                if (SQLite3.ExtendedErrCode(this.Handle) == SQLite3.ExtendedResult.ConstraintNotNull)
                {
                    throw NotNullConstraintViolationException.New(ex.Result, ex.Message, map, obj);
                }
                throw;
            }

            if (map.HasAutoIncPK)
            {
                var id = SQLite3.LastInsertRowid(Handle);
                map.SetAutoIncPK(obj, id);
            }

            return count;
        }

        /// <summary>
        /// 使用指定的对象更新表的所有列
        /// 除了它的主键。
        /// 对象需要有主键。
        /// </summary>
        /// <param name="obj">
        /// 要更新的对象。它必须具有使用PrimaryKeyAttribute指定的主键。
        /// </param>
        /// <returns>
        /// 更新的行数。
        /// </returns>
        public int Update(object obj)
        {
            if (obj == null)
            {
                return 0;
            }
            return Update(obj, obj.GetType());
        }

        /// <summary>
        /// 使用指定的对象更新表的所有列
        /// 
        /// 对象需要有主键。
        /// </summary>
        /// <param name="obj">
        /// 要更新的对象。它必须具有使用PrimaryKeyAttribute指定的主键。
        /// </param>
        /// <param name="objType">
        /// 要插入的对象类型。
        /// </param>
        /// <returns>
        /// 更新的行数。
        /// </returns>
        public int Update(object obj, Type objType)
        {
            int rowsAffected = 0;
            if (obj == null || objType == null)
            {
                return 0;
            }

            var map = GetMapping(objType);

            var pk = map.PK;

            if (pk == null)
            {
                throw new NotSupportedException("Cannot update " + map.TableName + ": it has no PK");
            }

            var cols = from p in map.Columns
                       where p != pk
                       select p;
            var vals = from c in cols
                       select c.GetValue(obj);
            var ps = new List<object>(vals);
            ps.Add(pk.GetValue(obj));
            var q = string.Format("update \"{0}\" set {1} where {2} = ? ", map.TableName, string.Join(",", (from c in cols
                                                                                                            select "\"" + c.Name + "\" = ? ").ToArray()), pk.Name);

            try
            {
                rowsAffected = Execute(q, ps.ToArray());
            }
            catch (SQLiteException ex)
            {

                if (ex.Result == SQLite3.Result.Constraint && SQLite3.ExtendedErrCode(this.Handle) == SQLite3.ExtendedResult.ConstraintNotNull)
                {
                    throw NotNullConstraintViolationException.New(ex, map, obj);
                }

                throw ex;
            }

            return rowsAffected;
        }

        /// <summary>
        /// 更新所有指定的对象。
        /// </summary>
        /// <param name="objects">
        /// An <see cref="IEnumerable"/> of the objects to insert.
        /// </param>
        /// <returns>
        /// 修改的行数。
        /// </returns>
        public int UpdateAll(System.Collections.IEnumerable objects)
        {
            var c = 0;
            RunInTransaction(() => {
                foreach (var r in objects)
                {
                    c += Update(r);
                }
            });
            return c;
        }

        /// <summary>
        /// 使用其主键从数据库中删除给定对象。
        /// </summary>
        /// <param name="objectToDelete">
        /// 要删除的对象。它必须具有使用PrimaryKeyAttribute指定的主键。
        /// </param>
        /// <returns>
        /// 删除的行数。
        /// </returns>
        public int Delete(object objectToDelete)
        {
            var map = GetMapping(objectToDelete.GetType());
            var pk = map.PK;
            if (pk == null)
            {
                throw new NotSupportedException("Cannot delete " + map.TableName + ": it has no PK");
            }
            var q = string.Format("delete from \"{0}\" where \"{1}\" = ?", map.TableName, pk.Name);
            return Execute(q, pk.GetValue(objectToDelete));
        }

        /// <summary>
        /// 删除具有指定主键的对象。
        /// </summary>
        /// <param name="primaryKey">
        /// 要删除的对象的主键。
        /// </param>
        /// <returns>
        /// 删除的对象数。
        /// </returns>
        /// <类型参数名称=‘T’>
        /// 对象类型。
        /// </typeparam>
        public int Delete<T>(object primaryKey)
        {
            var map = GetMapping(typeof(T));
            var pk = map.PK;
            if (pk == null)
            {
                throw new NotSupportedException("Cannot delete " + map.TableName + ": it has no PK");
            }
            var q = string.Format("delete from \"{0}\" where \"{1}\" = ?", map.TableName, pk.Name);
            return Execute(q, primaryKey);
        }

        /// <summary>
        /// 从指定表中删除所有对象。
        /// 警告：让我重复一遍。它将所有对象从
        /// 指定表。你真的想这么做吗？
        /// </summary>
        /// <returns>
        /// 删除的对象数量。
        /// </returns>
        /// <类型参数名称=‘T’>
        /// 要删除的对象类型。
        /// </typeparam>
        public int DeleteAll<T>()
        {
            var map = GetMapping(typeof(T));
            var query = string.Format("delete from \"{0}\"", map.TableName);
            return Execute(query);
        }

        ~SQLiteConnection()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            Close();
        }

        public void Close()
        {
            if (_open && Handle != NullHandle)
            {
                try
                {
                    if (_mappings != null)
                    {
                        foreach (var sqlInsertCommand in _mappings.Values)
                        {
                            sqlInsertCommand.Dispose();
                        }
                    }
                    var r = SQLite3.Close(Handle);
                    if (r != SQLite3.Result.OK)
                    {
                        string msg = SQLite3.GetErrmsg(Handle);
                        throw SQLiteException.New(r, msg);
                    }
                }
                finally
                {
                    Handle = NullHandle;
                    _open = false;
                }
            }
        }
    }

    /// <summary>
    /// 表示解析的连接字符串。
    /// </summary>
    class SQLiteConnectionString
    {
        public string ConnectionString { get; private set; }
        public string DatabasePath { get; private set; }
        public bool StoreDateTimeAsTicks { get; private set; }

#if NETFX_CORE
		static readonly string MetroStyleDataPath = Windows.Storage.ApplicationData.Current.LocalFolder.Path;
#endif

        public SQLiteConnectionString(string databasePath, bool storeDateTimeAsTicks)
        {
            ConnectionString = databasePath;
            StoreDateTimeAsTicks = storeDateTimeAsTicks;

#if NETFX_CORE
			DatabasePath = System.IO.Path.Combine (MetroStyleDataPath, databasePath);
#else
            DatabasePath = databasePath;
#endif
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class TableAttribute : Attribute
    {
        public string Name { get; set; }

        public TableAttribute(string name)
        {
            Name = name;
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class ColumnAttribute : Attribute
    {
        public string Name { get; set; }

        public ColumnAttribute(string name)
        {
            Name = name;
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class PrimaryKeyAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class AutoIncrementAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class IndexedAttribute : Attribute
    {
        public string Name { get; set; }
        public int Order { get; set; }
        public virtual bool Unique { get; set; }

        public IndexedAttribute()
        {
        }

        public IndexedAttribute(string name, int order)
        {
            Name = name;
            Order = order;
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class IgnoreAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class UniqueAttribute : IndexedAttribute
    {
        public override bool Unique
        {
            get { return true; }
            set { /* throw?  */ }
        }

        public UniqueAttribute() : base()
        {
        }

        public UniqueAttribute(string name, int order) : base(name, order)
        {
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class MaxLengthAttribute : Attribute
    {
        public int Value { get; private set; }

        public MaxLengthAttribute(int length)
        {
            Value = length;
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class CollationAttribute : Attribute
    {
        public string Value { get; private set; }

        public CollationAttribute(string collation)
        {
            Value = collation;
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class NotNullAttribute : Attribute
    {
    }

    public class TableMapping
    {
        public Type MappedType { get; private set; }

        public string TableName { get; private set; }

        public Column[] Columns { get; private set; }

        public Column PK { get; private set; }

        public string GetByPrimaryKeySql { get; private set; }

        Column _autoPk;
        Column[] _insertColumns;
        Column[] _insertOrReplaceColumns;

        public TableMapping(Type type, CreateFlags createFlags = CreateFlags.None)
        {
            MappedType = type;

#if NETFX_CORE
			var tableAttr = (TableAttribute)System.Reflection.CustomAttributeExtensions
				.GetCustomAttribute(type.GetTypeInfo(), typeof(TableAttribute), true);
#else
            var tableAttr = (TableAttribute)type.GetCustomAttributes(typeof(TableAttribute), true).FirstOrDefault();
#endif

            TableName = tableAttr != null ? tableAttr.Name : MappedType.Name;

#if !NETFX_CORE
            var props = MappedType.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.SetProperty);
#else
			var props = from p in MappedType.GetRuntimeProperties()
						where ((p.GetMethod != null && p.GetMethod.IsPublic) || (p.SetMethod != null && p.SetMethod.IsPublic) || (p.GetMethod != null && p.GetMethod.IsStatic) || (p.SetMethod != null && p.SetMethod.IsStatic))
						select p;
#endif
            var cols = new List<Column>();
            foreach (var p in props)
            {
#if !NETFX_CORE
                var ignore = p.GetCustomAttributes(typeof(IgnoreAttribute), true).Length > 0;
#else
				var ignore = p.GetCustomAttributes (typeof(IgnoreAttribute), true).Count() > 0;
#endif
                if (p.CanWrite && !ignore)
                {
                    cols.Add(new Column(p, createFlags));
                }
            }
            Columns = cols.ToArray();
            foreach (var c in Columns)
            {
                if (c.IsAutoInc && c.IsPK)
                {
                    _autoPk = c;
                }
                if (c.IsPK)
                {
                    PK = c;
                }
            }

            HasAutoIncPK = _autoPk != null;

            if (PK != null)
            {
                GetByPrimaryKeySql = string.Format("select * from \"{0}\" where \"{1}\" = ?", TableName, PK.Name);
            }
            else
            {
                // 
                GetByPrimaryKeySql = string.Format("select * from \"{0}\" limit 1", TableName);
            }
        }

        public bool HasAutoIncPK { get; private set; }

        public void SetAutoIncPK(object obj, long id)
        {
            if (_autoPk != null)
            {
                _autoPk.SetValue(obj, Convert.ChangeType(id, _autoPk.ColumnType, null));
            }
        }

        public Column[] InsertColumns
        {
            get
            {
                if (_insertColumns == null)
                {
                    _insertColumns = Columns.Where(c => !c.IsAutoInc).ToArray();
                }
                return _insertColumns;
            }
        }

        public Column[] InsertOrReplaceColumns
        {
            get
            {
                if (_insertOrReplaceColumns == null)
                {
                    _insertOrReplaceColumns = Columns.ToArray();
                }
                return _insertOrReplaceColumns;
            }
        }

        public Column FindColumnWithPropertyName(string propertyName)
        {
            var exact = Columns.FirstOrDefault(c => c.PropertyName == propertyName);
            return exact;
        }

        public Column FindColumn(string columnName)
        {
            var exact = Columns.FirstOrDefault(c => c.Name == columnName);
            return exact;
        }

        PreparedSqlLiteInsertCommand _insertCommand;
        string _insertCommandExtra;

        public PreparedSqlLiteInsertCommand GetInsertCommand(SQLiteConnection conn, string extra)
        {
            if (_insertCommand == null)
            {
                _insertCommand = CreateInsertCommand(conn, extra);
                _insertCommandExtra = extra;
            }
            else if (_insertCommandExtra != extra)
            {
                _insertCommand.Dispose();
                _insertCommand = CreateInsertCommand(conn, extra);
                _insertCommandExtra = extra;
            }
            return _insertCommand;
        }

        PreparedSqlLiteInsertCommand CreateInsertCommand(SQLiteConnection conn, string extra)
        {
            var cols = InsertColumns;
            string insertSql;
            if (!cols.Any() && Columns.Count() == 1 && Columns[0].IsAutoInc)
            {
                insertSql = string.Format("insert {1} into \"{0}\" default values", TableName, extra);
            }
            else
            {
                var replacing = string.Compare(extra, "OR REPLACE", StringComparison.OrdinalIgnoreCase) == 0;

                if (replacing)
                {
                    cols = InsertOrReplaceColumns;
                }

                insertSql = string.Format("insert {3} into \"{0}\"({1}) values ({2})", TableName,
                                   string.Join(",", (from c in cols
                                                     select "\"" + c.Name + "\"").ToArray()),
                                   string.Join(",", (from c in cols
                                                     select "?").ToArray()), extra);

            }

            var insertCommand = new PreparedSqlLiteInsertCommand(conn);
            insertCommand.CommandText = insertSql;
            return insertCommand;
        }

        protected internal void Dispose()
        {
            if (_insertCommand != null)
            {
                _insertCommand.Dispose();
                _insertCommand = null;
            }
        }

        public class Column
        {
            PropertyInfo _prop;

            public string Name { get; private set; }

            public string PropertyName { get { return _prop.Name; } }

            public Type ColumnType { get; private set; }

            public string Collation { get; private set; }

            public bool IsAutoInc { get; private set; }
            public bool IsAutoGuid { get; private set; }

            public bool IsPK { get; private set; }

            public IEnumerable<IndexedAttribute> Indices { get; set; }

            public bool IsNullable { get; private set; }

            public int? MaxStringLength { get; private set; }

            public Column(PropertyInfo prop, CreateFlags createFlags = CreateFlags.None)
            {
                var colAttr = (ColumnAttribute)prop.GetCustomAttributes(typeof(ColumnAttribute), true).FirstOrDefault();

                _prop = prop;
                Name = colAttr == null ? prop.Name : colAttr.Name;
                // 如果此类型可以为空<T>，则Nullable.GetUnderlyingType返回T，否则返回空，因此改为获取实际类型
                ColumnType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
                Collation = Orm.Collation(prop);

                IsPK = Orm.IsPK(prop) ||
                    (((createFlags & CreateFlags.ImplicitPK) == CreateFlags.ImplicitPK) &&
                        string.Compare(prop.Name, Orm.ImplicitPkName, StringComparison.OrdinalIgnoreCase) == 0);

                var isAuto = Orm.IsAutoInc(prop) || (IsPK && ((createFlags & CreateFlags.AutoIncPK) == CreateFlags.AutoIncPK));
                IsAutoGuid = isAuto && ColumnType == typeof(Guid);
                IsAutoInc = isAuto && !IsAutoGuid;

                Indices = Orm.GetIndices(prop);
                if (!Indices.Any()
                    && !IsPK
                    && ((createFlags & CreateFlags.ImplicitIndex) == CreateFlags.ImplicitIndex)
                    && Name.EndsWith(Orm.ImplicitIndexSuffix, StringComparison.OrdinalIgnoreCase)
                    )
                {
                    Indices = new IndexedAttribute[] { new IndexedAttribute() };
                }
                IsNullable = !(IsPK || Orm.IsMarkedNotNull(prop));
                MaxStringLength = Orm.MaxStringLength(prop);
            }

            public void SetValue(object obj, object val)
            {
                _prop.SetValue(obj, val, null);
            }

            public object GetValue(object obj)
            {
                return _prop.GetGetMethod().Invoke(obj, null);
            }
        }
    }

    public static class Orm
    {
        public const int DefaultMaxStringLength = 140;
        public const string ImplicitPkName = "Id";
        public const string ImplicitIndexSuffix = "Id";

        public static string SqlDecl(TableMapping.Column p, bool storeDateTimeAsTicks)
        {
            string decl = "\"" + p.Name + "\" " + SqlType(p, storeDateTimeAsTicks) + " ";

            if (p.IsPK)
            {
                decl += "primary key ";
            }
            if (p.IsAutoInc)
            {
                decl += "autoincrement ";
            }
            if (!p.IsNullable)
            {
                decl += "not null ";
            }
            if (!string.IsNullOrEmpty(p.Collation))
            {
                decl += "collate " + p.Collation + " ";
            }

            return decl;
        }

        public static string SqlType(TableMapping.Column p, bool storeDateTimeAsTicks)
        {
            var clrType = p.ColumnType;
            if (clrType == typeof(Boolean) || clrType == typeof(Byte) || clrType == typeof(UInt16) || clrType == typeof(SByte) || clrType == typeof(Int16) || clrType == typeof(Int32))
            {
                return "integer";
            }
            else if (clrType == typeof(UInt32) || clrType == typeof(Int64))
            {
                return "bigint";
            }
            else if (clrType == typeof(Single) || clrType == typeof(Double) || clrType == typeof(Decimal))
            {
                return "float";
            }
            else if (clrType == typeof(String))
            {
                int? len = p.MaxStringLength;

                if (len.HasValue)
                    return "varchar(" + len.Value + ")";

                return "varchar";
            }
            else if (clrType == typeof(TimeSpan))
            {
                return "bigint";
            }
            else if (clrType == typeof(DateTime))
            {
                return storeDateTimeAsTicks ? "bigint" : "datetime";
            }
            else if (clrType == typeof(DateTimeOffset))
            {
                return "bigint";
#if !NETFX_CORE
            }
            else if (clrType.IsEnum)
            {
#else
			} else if (clrType.GetTypeInfo().IsEnum) {
#endif
                return "integer";
            }
            else if (clrType == typeof(byte[]))
            {
                return "blob";
            }
            else if (clrType == typeof(Guid))
            {
                return "varchar(36)";
            }
            else
            {
                throw new NotSupportedException("Don't know about " + clrType);
            }
        }

        public static bool IsPK(MemberInfo p)
        {
            var attrs = p.GetCustomAttributes(typeof(PrimaryKeyAttribute), true);
#if !NETFX_CORE
            return attrs.Length > 0;
#else
			return attrs.Count() > 0;
#endif
        }

        public static string Collation(MemberInfo p)
        {
            var attrs = p.GetCustomAttributes(typeof(CollationAttribute), true);
#if !NETFX_CORE
            if (attrs.Length > 0)
            {
                return ((CollationAttribute)attrs[0]).Value;
#else
			if (attrs.Count() > 0) {
				return ((CollationAttribute)attrs.First()).Value;
#endif
            }
            else
            {
                return string.Empty;
            }
        }

        public static bool IsAutoInc(MemberInfo p)
        {
            var attrs = p.GetCustomAttributes(typeof(AutoIncrementAttribute), true);
#if !NETFX_CORE
            return attrs.Length > 0;
#else
			return attrs.Count() > 0;
#endif
        }

        public static IEnumerable<IndexedAttribute> GetIndices(MemberInfo p)
        {
            var attrs = p.GetCustomAttributes(typeof(IndexedAttribute), true);
            return attrs.Cast<IndexedAttribute>();
        }

        public static int? MaxStringLength(PropertyInfo p)
        {
            var attrs = p.GetCustomAttributes(typeof(MaxLengthAttribute), true);
#if !NETFX_CORE
            if (attrs.Length > 0)
                return ((MaxLengthAttribute)attrs[0]).Value;
#else
			if (attrs.Count() > 0)
				return ((MaxLengthAttribute)attrs.First()).Value;
#endif

            return null;
        }

        public static bool IsMarkedNotNull(MemberInfo p)
        {
            var attrs = p.GetCustomAttributes(typeof(NotNullAttribute), true);
#if !NETFX_CORE
            return attrs.Length > 0;
#else
	return attrs.Count() > 0;
#endif
        }
    }

    public partial class SQLiteCommand
    {
        SQLiteConnection _conn;
        private List<Binding> _bindings;

        public string CommandText { get; set; }

        internal SQLiteCommand(SQLiteConnection conn)
        {
            _conn = conn;
            _bindings = new List<Binding>();
            CommandText = "";
        }

        public int ExecuteNonQuery()
        {
            if (_conn.Trace)
            {
                _conn.InvokeTrace("Executing: " + this);
            }

            var r = SQLite3.Result.OK;
            lock (_conn.SyncObject)
            {
                var stmt = Prepare();
                r = SQLite3.Step(stmt);
                Finalize(stmt);
            }
            if (r == SQLite3.Result.Done)
            {
                int rowsAffected = SQLite3.Changes(_conn.Handle);
                return rowsAffected;
            }
            else if (r == SQLite3.Result.Error)
            {
                string msg = SQLite3.GetErrmsg(_conn.Handle);
                throw SQLiteException.New(r, msg);
            }
            else if (r == SQLite3.Result.Constraint)
            {
                if (SQLite3.ExtendedErrCode(_conn.Handle) == SQLite3.ExtendedResult.ConstraintNotNull)
                {
                    throw NotNullConstraintViolationException.New(r, SQLite3.GetErrmsg(_conn.Handle));
                }
            }

            throw SQLiteException.New(r, r.ToString());
        }

        public IEnumerable<T> ExecuteDeferredQuery<T>()
        {
            return ExecuteDeferredQuery<T>(_conn.GetMapping(typeof(T)));
        }

        public List<T> ExecuteQuery<T>()
        {
            return ExecuteDeferredQuery<T>(_conn.GetMapping(typeof(T))).ToList();
        }

        public List<T> ExecuteQuery<T>(TableMapping map)
        {
            return ExecuteDeferredQuery<T>(map).ToList();
        }

        /// <summary>
        /// 每次从数据库加载实例时调用。
        /// </summary>
        /// <param name='obj'>
        /// 新创建的对象。
        /// </param>
        /// <备注>
        /// This can be overridden in combination with the <see cref="SQLiteConnection.NewCommand"/>
        /// 挂钩对象生命周期的方法。
        ///
        /// 
        /// 
        protected virtual void OnInstanceCreated(object obj)
        {
            // 可以被覆盖。
        }

        public IEnumerable<T> ExecuteDeferredQuery<T>(TableMapping map)
        {
            if (_conn.Trace)
            {
                _conn.InvokeTrace("Executing Query: " + this);
            }

            lock (_conn.SyncObject)
            {
                var stmt = Prepare();
                try
                {
                    var cols = new TableMapping.Column[SQLite3.ColumnCount(stmt)];

                    for (int i = 0; i < cols.Length; i++)
                    {
                        var name = SQLite3.ColumnName16(stmt, i);
                        cols[i] = map.FindColumn(name);
                    }

                    while (SQLite3.Step(stmt) == SQLite3.Result.Row)
                    {
                        var obj = Activator.CreateInstance(map.MappedType);
                        for (int i = 0; i < cols.Length; i++)
                        {
                            if (cols[i] == null)
                                continue;
                            var colType = SQLite3.ColumnType(stmt, i);
                            var val = ReadCol(stmt, i, colType, cols[i].ColumnType);
                            cols[i].SetValue(obj, val);
                        }
                        OnInstanceCreated(obj);
                        yield return (T)obj;
                    }
                }
                finally
                {
                    SQLite3.Finalize(stmt);
                }
            }
        }

        public T ExecuteScalar<T>()
        {
            if (_conn.Trace)
            {
                _conn.InvokeTrace("Executing Query: " + this);
            }

            T val = default(T);

            lock (_conn.SyncObject)
            {
                var stmt = Prepare();

                try
                {
                    var r = SQLite3.Step(stmt);
                    if (r == SQLite3.Result.Row)
                    {
                        var colType = SQLite3.ColumnType(stmt, 0);
                        val = (T)ReadCol(stmt, 0, colType, typeof(T));
                    }
                    else if (r == SQLite3.Result.Done)
                    {
                    }
                    else
                    {
                        throw SQLiteException.New(r, SQLite3.GetErrmsg(_conn.Handle));
                    }
                }
                finally
                {
                    Finalize(stmt);
                }
            }

            return val;
        }

        public void Bind(string name, object val)
        {
            _bindings.Add(new Binding
            {
                Name = name,
                Value = val
            });
        }

        public void Bind(object val)
        {
            Bind(null, val);
        }

        public override string ToString()
        {
            var parts = new string[1 + _bindings.Count];
            parts[0] = CommandText;
            var i = 1;
            foreach (var b in _bindings)
            {
                parts[i] = string.Format("  {0}: {1}", i - 1, b.Value);
                i++;
            }
            return string.Join(Environment.NewLine, parts);
        }

        Sqlite3Statement Prepare()
        {
            var stmt = SQLite3.Prepare2(_conn.Handle, CommandText);
            BindAll(stmt);
            return stmt;
        }

        void Finalize(Sqlite3Statement stmt)
        {
            SQLite3.Finalize(stmt);
        }

        void BindAll(Sqlite3Statement stmt)
        {
            int nextIdx = 1;
            foreach (var b in _bindings)
            {
                if (b.Name != null)
                {
                    b.Index = SQLite3.BindParameterIndex(stmt, b.Name);
                }
                else
                {
                    b.Index = nextIdx++;
                }

                BindParameter(stmt, b.Index, b.Value, _conn.StoreDateTimeAsTicks);
            }
        }

        internal static IntPtr NegativePointer = new IntPtr(-1);

        internal static void BindParameter(Sqlite3Statement stmt, int index, object value, bool storeDateTimeAsTicks)
        {
            if (value == null)
            {
                SQLite3.BindNull(stmt, index);
            }
            else
            {
                if (value is Int32)
                {
                    SQLite3.BindInt(stmt, index, (int)value);
                }
                else if (value is String)
                {
                    SQLite3.BindText(stmt, index, (string)value, -1, NegativePointer);
                }
                else if (value is Byte || value is UInt16 || value is SByte || value is Int16)
                {
                    SQLite3.BindInt(stmt, index, Convert.ToInt32(value));
                }
                else if (value is Boolean)
                {
                    SQLite3.BindInt(stmt, index, (bool)value ? 1 : 0);
                }
                else if (value is UInt32 || value is Int64)
                {
                    SQLite3.BindInt64(stmt, index, Convert.ToInt64(value));
                }
                else if (value is Single || value is Double || value is Decimal)
                {
                    SQLite3.BindDouble(stmt, index, Convert.ToDouble(value));
                }
                else if (value is TimeSpan)
                {
                    SQLite3.BindInt64(stmt, index, ((TimeSpan)value).Ticks);
                }
                else if (value is DateTime)
                {
                    if (storeDateTimeAsTicks)
                    {
                        SQLite3.BindInt64(stmt, index, ((DateTime)value).Ticks);
                    }
                    else
                    {
                        SQLite3.BindText(stmt, index, ((DateTime)value).ToString("yyyy-MM-dd HH:mm:ss"), -1, NegativePointer);
                    }
                }
                else if (value is DateTimeOffset)
                {
                    SQLite3.BindInt64(stmt, index, ((DateTimeOffset)value).UtcTicks);
#if !NETFX_CORE
                }
                else if (value.GetType().IsEnum)
                {
#else
				} else if (value.GetType().GetTypeInfo().IsEnum) {
#endif
                    SQLite3.BindInt(stmt, index, Convert.ToInt32(value));
                }
                else if (value is byte[])
                {
                    SQLite3.BindBlob(stmt, index, (byte[])value, ((byte[])value).Length, NegativePointer);
                }
                else if (value is Guid)
                {
                    SQLite3.BindText(stmt, index, ((Guid)value).ToString(), 72, NegativePointer);
                }
                else
                {
                    throw new NotSupportedException("Cannot store type: " + value.GetType());
                }
            }
        }

        class Binding
        {
            public string Name { get; set; }

            public object Value { get; set; }

            public int Index { get; set; }
        }

        object ReadCol(Sqlite3Statement stmt, int index, SQLite3.ColType type, Type clrType)
        {
            if (type == SQLite3.ColType.Null)
            {
                return null;
            }
            else
            {
                if (clrType == typeof(String))
                {
                    return SQLite3.ColumnString(stmt, index);
                }
                else if (clrType == typeof(Int32))
                {
                    return (int)SQLite3.ColumnInt(stmt, index);
                }
                else if (clrType == typeof(Boolean))
                {
                    return SQLite3.ColumnInt(stmt, index) == 1;
                }
                else if (clrType == typeof(double))
                {
                    return SQLite3.ColumnDouble(stmt, index);
                }
                else if (clrType == typeof(float))
                {
                    return (float)SQLite3.ColumnDouble(stmt, index);
                }
                else if (clrType == typeof(TimeSpan))
                {
                    return new TimeSpan(SQLite3.ColumnInt64(stmt, index));
                }
                else if (clrType == typeof(DateTime))
                {
                    if (_conn.StoreDateTimeAsTicks)
                    {
                        return new DateTime(SQLite3.ColumnInt64(stmt, index));
                    }
                    else
                    {
                        var text = SQLite3.ColumnString(stmt, index);
                        return DateTime.Parse(text);
                    }
                }
                else if (clrType == typeof(DateTimeOffset))
                {
                    return new DateTimeOffset(SQLite3.ColumnInt64(stmt, index), TimeSpan.Zero);
#if !NETFX_CORE
                }
                else if (clrType.IsEnum)
                {
#else
				} else if (clrType.GetTypeInfo().IsEnum) {
#endif
                    return SQLite3.ColumnInt(stmt, index);
                }
                else if (clrType == typeof(Int64))
                {
                    return SQLite3.ColumnInt64(stmt, index);
                }
                else if (clrType == typeof(UInt32))
                {
                    return (uint)SQLite3.ColumnInt64(stmt, index);
                }
                else if (clrType == typeof(decimal))
                {
                    return (decimal)SQLite3.ColumnDouble(stmt, index);
                }
                else if (clrType == typeof(Byte))
                {
                    return (byte)SQLite3.ColumnInt(stmt, index);
                }
                else if (clrType == typeof(UInt16))
                {
                    return (ushort)SQLite3.ColumnInt(stmt, index);
                }
                else if (clrType == typeof(Int16))
                {
                    return (short)SQLite3.ColumnInt(stmt, index);
                }
                else if (clrType == typeof(sbyte))
                {
                    return (sbyte)SQLite3.ColumnInt(stmt, index);
                }
                else if (clrType == typeof(byte[]))
                {
                    return SQLite3.ColumnByteArray(stmt, index);
                }
                else if (clrType == typeof(Guid))
                {
                    var text = SQLite3.ColumnString(stmt, index);
                    return new Guid(text);
                }
                else
                {
                    throw new NotSupportedException("Don't know how to read " + clrType);
                }
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class PreparedSqlLiteInsertCommand : IDisposable
    {
        public bool Initialized { get; set; }

        protected SQLiteConnection Connection { get; set; }

        public string CommandText { get; set; }

        protected Sqlite3Statement Statement { get; set; }
        internal static readonly Sqlite3Statement NullStatement = default(Sqlite3Statement);

        internal PreparedSqlLiteInsertCommand(SQLiteConnection conn)
        {
            Connection = conn;
        }

        public int ExecuteNonQuery(object[] source)
        {
            if (Connection.Trace)
            {
                Connection.InvokeTrace("Executing: " + CommandText);
            }

            var r = SQLite3.Result.OK;

            if (!Initialized)
            {
                Statement = Prepare();
                Initialized = true;
            }

            // 绑定值。
            if (source != null)
            {
                for (int i = 0; i < source.Length; i++)
                {
                    SQLiteCommand.BindParameter(Statement, i + 1, source[i], Connection.StoreDateTimeAsTicks);
                }
            }
            r = SQLite3.Step(Statement);

            if (r == SQLite3.Result.Done)
            {
                int rowsAffected = SQLite3.Changes(Connection.Handle);
                SQLite3.Reset(Statement);
                return rowsAffected;
            }
            else if (r == SQLite3.Result.Error)
            {
                string msg = SQLite3.GetErrmsg(Connection.Handle);
                SQLite3.Reset(Statement);
                throw SQLiteException.New(r, msg);
            }
            else if (r == SQLite3.Result.Constraint && SQLite3.ExtendedErrCode(Connection.Handle) == SQLite3.ExtendedResult.ConstraintNotNull)
            {
                SQLite3.Reset(Statement);
                throw NotNullConstraintViolationException.New(r, SQLite3.GetErrmsg(Connection.Handle));
            }
            else
            {
                SQLite3.Reset(Statement);
                throw SQLiteException.New(r, r.ToString());
            }
        }

        protected virtual Sqlite3Statement Prepare()
        {
            var stmt = SQLite3.Prepare2(Connection.Handle, CommandText);
            return stmt;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (Statement != NullStatement)
            {
                try
                {
                    SQLite3.Finalize(Statement);
                }
                finally
                {
                    Statement = NullStatement;
                    Connection = null;
                }
            }
        }

        ~PreparedSqlLiteInsertCommand()
        {
            Dispose(false);
        }
    }

    public abstract class BaseTableQuery
    {
        protected class Ordering
        {
            public string ColumnName { get; set; }
            public bool Ascending { get; set; }
        }
    }

    public class TableQuery<T> : BaseTableQuery, IEnumerable<T>
    {
        public SQLiteConnection Connection { get; private set; }

        public TableMapping Table { get; private set; }

        Expression _where;
        List<Ordering> _orderBys;
        int? _limit;
        int? _offset;

        BaseTableQuery _joinInner;
        Expression _joinInnerKeySelector;
        BaseTableQuery _joinOuter;
        Expression _joinOuterKeySelector;
        Expression _joinSelector;

        Expression _selector;

        TableQuery(SQLiteConnection conn, TableMapping table)
        {
            Connection = conn;
            Table = table;
        }

        public TableQuery(SQLiteConnection conn)
        {
            Connection = conn;
            Table = Connection.GetMapping(typeof(T));
        }

        public TableQuery<U> Clone<U>()
        {
            var q = new TableQuery<U>(Connection, Table);
            q._where = _where;
            q._deferred = _deferred;
            if (_orderBys != null)
            {
                q._orderBys = new List<Ordering>(_orderBys);
            }
            q._limit = _limit;
            q._offset = _offset;
            q._joinInner = _joinInner;
            q._joinInnerKeySelector = _joinInnerKeySelector;
            q._joinOuter = _joinOuter;
            q._joinOuterKeySelector = _joinOuterKeySelector;
            q._joinSelector = _joinSelector;
            q._selector = _selector;
            return q;
        }

        public TableQuery<T> Where(Expression<Func<T, bool>> predExpr)
        {
            if (predExpr.NodeType == ExpressionType.Lambda)
            {
                var lambda = (LambdaExpression)predExpr;
                var pred = lambda.Body;
                var q = Clone<T>();
                q.AddWhere(pred);
                return q;
            }
            else
            {
                throw new NotSupportedException("Must be a predicate");
            }
        }

        public TableQuery<T> Take(int n)
        {
            var q = Clone<T>();
            q._limit = n;
            return q;
        }

        public TableQuery<T> Skip(int n)
        {
            var q = Clone<T>();
            q._offset = n;
            return q;
        }

        public T ElementAt(int index)
        {
            return Skip(index).Take(1).First();
        }

        bool _deferred;
        public TableQuery<T> Deferred()
        {
            var q = Clone<T>();
            q._deferred = true;
            return q;
        }

        public TableQuery<T> OrderBy<U>(Expression<Func<T, U>> orderExpr)
        {
            return AddOrderBy<U>(orderExpr, true);
        }

        public TableQuery<T> OrderByDescending<U>(Expression<Func<T, U>> orderExpr)
        {
            return AddOrderBy<U>(orderExpr, false);
        }

        public TableQuery<T> ThenBy<U>(Expression<Func<T, U>> orderExpr)
        {
            return AddOrderBy<U>(orderExpr, true);
        }

        public TableQuery<T> ThenByDescending<U>(Expression<Func<T, U>> orderExpr)
        {
            return AddOrderBy<U>(orderExpr, false);
        }

        private TableQuery<T> AddOrderBy<U>(Expression<Func<T, U>> orderExpr, bool asc)
        {
            if (orderExpr.NodeType == ExpressionType.Lambda)
            {
                var lambda = (LambdaExpression)orderExpr;

                MemberExpression mem = null;

                var unary = lambda.Body as UnaryExpression;
                if (unary != null && unary.NodeType == ExpressionType.Convert)
                {
                    mem = unary.Operand as MemberExpression;
                }
                else
                {
                    mem = lambda.Body as MemberExpression;
                }

                if (mem != null && (mem.Expression.NodeType == ExpressionType.Parameter))
                {
                    var q = Clone<T>();
                    if (q._orderBys == null)
                    {
                        q._orderBys = new List<Ordering>();
                    }
                    q._orderBys.Add(new Ordering
                    {
                        ColumnName = Table.FindColumnWithPropertyName(mem.Member.Name).Name,
                        Ascending = asc
                    });
                    return q;
                }
                else
                {
                    throw new NotSupportedException("Order By does not support: " + orderExpr);
                }
            }
            else
            {
                throw new NotSupportedException("Must be a predicate");
            }
        }

        private void AddWhere(Expression pred)
        {
            if (_where == null)
            {
                _where = pred;
            }
            else
            {
                _where = Expression.AndAlso(_where, pred);
            }
        }

        public TableQuery<TResult> Join<TInner, TKey, TResult>(
            TableQuery<TInner> inner,
            Expression<Func<T, TKey>> outerKeySelector,
            Expression<Func<TInner, TKey>> innerKeySelector,
            Expression<Func<T, TInner, TResult>> resultSelector)
        {
            var q = new TableQuery<TResult>(Connection, Connection.GetMapping(typeof(TResult)))
            {
                _joinOuter = this,
                _joinOuterKeySelector = outerKeySelector,
                _joinInner = inner,
                _joinInnerKeySelector = innerKeySelector,
                _joinSelector = resultSelector,
            };
            return q;
        }

        public TableQuery<TResult> Select<TResult>(Expression<Func<T, TResult>> selector)
        {
            var q = Clone<TResult>();
            q._selector = selector;
            return q;
        }

        private SQLiteCommand GenerateCommand(string selectionList)
        {
            if (_joinInner != null && _joinOuter != null)
            {
                throw new NotSupportedException("Joins are not supported.");
            }
            else
            {
                var cmdText = "select " + selectionList + " from \"" + Table.TableName + "\"";
                var args = new List<object>();
                if (_where != null)
                {
                    var w = CompileExpr(_where, args);
                    cmdText += " where " + w.CommandText;
                }
                if ((_orderBys != null) && (_orderBys.Count > 0))
                {
                    var t = string.Join(", ", _orderBys.Select(o => "\"" + o.ColumnName + "\"" + (o.Ascending ? "" : " desc")).ToArray());
                    cmdText += " order by " + t;
                }
                if (_limit.HasValue)
                {
                    cmdText += " limit " + _limit.Value;
                }
                if (_offset.HasValue)
                {
                    if (!_limit.HasValue)
                    {
                        cmdText += " limit -1 ";
                    }
                    cmdText += " offset " + _offset.Value;
                }
                return Connection.CreateCommand(cmdText, args.ToArray());
            }
        }

        class CompileResult
        {
            public string CommandText { get; set; }

            public object Value { get; set; }
        }

        private CompileResult CompileExpr(Expression expr, List<object> queryArgs)
        {
            if (expr == null)
            {
                throw new NotSupportedException("Expression is NULL");
            }
            else if (expr is BinaryExpression)
            {
                var bin = (BinaryExpression)expr;

                var leftr = CompileExpr(bin.Left, queryArgs);
                var rightr = CompileExpr(bin.Right, queryArgs);

                // 如果任何一方为参数且为空，则对另一侧进行特殊处理(对于"is NULL"/"is NOT NULL")
                string text;
                if (leftr.CommandText == "?" && leftr.Value == null)
                    text = CompileNullBinaryExpression(bin, rightr);
                else if (rightr.CommandText == "?" && rightr.Value == null)
                    text = CompileNullBinaryExpression(bin, leftr);
                else
                    text = "(" + leftr.CommandText + " " + GetSqlName(bin) + " " + rightr.CommandText + ")";
                return new CompileResult { CommandText = text };
            }
            else if (expr.NodeType == ExpressionType.Call)
            {

                var call = (MethodCallExpression)expr;
                var args = new CompileResult[call.Arguments.Count];
                var obj = call.Object != null ? CompileExpr(call.Object, queryArgs) : null;

                for (var i = 0; i < args.Length; i++)
                {
                    args[i] = CompileExpr(call.Arguments[i], queryArgs);
                }

                var sqlCall = "";

                if (call.Method.Name == "Like" && args.Length == 2)
                {
                    sqlCall = "(" + args[0].CommandText + " like " + args[1].CommandText + ")";
                }
                else if (call.Method.Name == "Contains" && args.Length == 2)
                {
                    sqlCall = "(" + args[1].CommandText + " in " + args[0].CommandText + ")";
                }
                else if (call.Method.Name == "Contains" && args.Length == 1)
                {
                    if (call.Object != null && call.Object.Type == typeof(string))
                    {
                        sqlCall = "(" + obj.CommandText + " like ('%' || " + args[0].CommandText + " || '%'))";
                    }
                    else
                    {
                        sqlCall = "(" + args[0].CommandText + " in " + obj.CommandText + ")";
                    }
                }
                else if (call.Method.Name == "StartsWith" && args.Length == 1)
                {
                    sqlCall = "(" + obj.CommandText + " like (" + args[0].CommandText + " || '%'))";
                }
                else if (call.Method.Name == "EndsWith" && args.Length == 1)
                {
                    sqlCall = "(" + obj.CommandText + " like ('%' || " + args[0].CommandText + "))";
                }
                else if (call.Method.Name == "Equals" && args.Length == 1)
                {
                    sqlCall = "(" + obj.CommandText + " = (" + args[0].CommandText + "))";
                }
                else if (call.Method.Name == "ToLower")
                {
                    sqlCall = "(lower(" + obj.CommandText + "))";
                }
                else if (call.Method.Name == "ToUpper")
                {
                    sqlCall = "(upper(" + obj.CommandText + "))";
                }
                else
                {
                    sqlCall = call.Method.Name.ToLower() + "(" + string.Join(",", args.Select(a => a.CommandText).ToArray()) + ")";
                }
                return new CompileResult { CommandText = sqlCall };

            }
            else if (expr.NodeType == ExpressionType.Constant)
            {
                var c = (ConstantExpression)expr;
                queryArgs.Add(c.Value);
                return new CompileResult
                {
                    CommandText = "?",
                    Value = c.Value
                };
            }
            else if (expr.NodeType == ExpressionType.Convert)
            {
                var u = (UnaryExpression)expr;
                var ty = u.Type;
                var valr = CompileExpr(u.Operand, queryArgs);
                return new CompileResult
                {
                    CommandText = valr.CommandText,
                    Value = valr.Value != null ? ConvertTo(valr.Value, ty) : null
                };
            }
            else if (expr.NodeType == ExpressionType.Not)
            {
                var u = (UnaryExpression)expr;
                var ty = u.Type;
                var valr = CompileExpr(u.Operand, queryArgs);

                return new CompileResult
                {
                    CommandText = "NOT " + valr.CommandText,
                    Value = valr.Value != null ? valr.Value : null
                };
            }
            else if (expr.NodeType == ExpressionType.MemberAccess)
            {
                var mem = (MemberExpression)expr;

                if (mem.Expression != null && mem.Expression.NodeType == ExpressionType.Parameter)
                {
                    //
                    // 
                    // 如果该列名已映射，则需要对其进行翻译
                    //
                    var columnName = Table.FindColumnWithPropertyName(mem.Member.Name).Name;
                    return new CompileResult { CommandText = "\"" + columnName + "\"" };
                }
                else
                {
                    object obj = null;
                    if (mem.Expression != null)
                    {
                        var r = CompileExpr(mem.Expression, queryArgs);
                        if (r.Value == null)
                        {
                            throw new NotSupportedException("Member access failed to compile expression");
                        }
                        if (r.CommandText == "?")
                        {
                            queryArgs.RemoveAt(queryArgs.Count - 1);
                        }
                        obj = r.Value;
                    }

                    //
                    // 获取成员值
                    //
                    object val = null;

#if !NETFX_CORE
                    if (mem.Member.MemberType == MemberTypes.Property)
                    {
#else
					if (mem.Member is PropertyInfo) {
#endif
                        var m = (PropertyInfo)mem.Member;
                        // val=m.GetValue(obj，空)；
                        val = m.GetGetMethod().Invoke(obj, null);
#if !NETFX_CORE
                    }
                    else if (mem.Member.MemberType == MemberTypes.Field)
                    {
#else
					} else if (mem.Member is FieldInfo) {
#endif
#if SILVERLIGHT
						val = Expression.Lambda (expr).Compile ().DynamicInvoke ();
#else
                        var m = (FieldInfo)mem.Member;
                        val = m.GetValue(obj);
#endif
                    }
                    else
                    {
#if !NETFX_CORE
                        throw new NotSupportedException("MemberExpr: " + mem.Member.MemberType);
#else
						throw new NotSupportedException ("MemberExpr: " + mem.Member.DeclaringType);
#endif
                    }

                    //
                    // 为可枚举数使用特殊魔法
                    //
                    if (val != null && val is System.Collections.IEnumerable && !(val is string) && !(val is System.Collections.Generic.IEnumerable<byte>))
                    {
                        var sb = new System.Text.StringBuilder();
                        sb.Append("(");
                        var head = "";
                        foreach (var a in (System.Collections.IEnumerable)val)
                        {
                            queryArgs.Add(a);
                            sb.Append(head);
                            sb.Append("?");
                            head = ",";
                        }
                        sb.Append(")");
                        return new CompileResult
                        {
                            CommandText = sb.ToString(),
                            Value = val
                        };
                    }
                    else
                    {
                        queryArgs.Add(val);
                        return new CompileResult
                        {
                            CommandText = "?",
                            Value = val
                        };
                    }
                }
            }
            throw new NotSupportedException("Cannot compile: " + expr.NodeType.ToString());
        }

        static object ConvertTo(object obj, Type t)
        {
            Type nut = Nullable.GetUnderlyingType(t);

            if (nut != null)
            {
                if (obj == null) return null;
                return Convert.ChangeType(obj, nut);
            }
            else
            {
                return Convert.ChangeType(obj, t);
            }
        }

        /// <summary>
        /// 编译其中一个参数为空的BinaryExpression。
        /// </summary>
        /// <param name="parameter"> 非空参数</param>
        private string CompileNullBinaryExpression(BinaryExpression expression, CompileResult parameter)
        {
            if (expression.NodeType == ExpressionType.Equal)
                return "(" + parameter.CommandText + " is ?)";
            else if (expression.NodeType == ExpressionType.NotEqual)
                return "(" + parameter.CommandText + " is not ?)";
            else
                throw new NotSupportedException("Cannot compile Null-BinaryExpression with type " + expression.NodeType.ToString());
        }

        string GetSqlName(Expression expr)
        {
            var n = expr.NodeType;
            if (n == ExpressionType.GreaterThan)
                return ">";
            else if (n == ExpressionType.GreaterThanOrEqual)
            {
                return ">=";
            }
            else if (n == ExpressionType.LessThan)
            {
                return "<";
            }
            else if (n == ExpressionType.LessThanOrEqual)
            {
                return "<=";
            }
            else if (n == ExpressionType.And)
            {
                return "&";
            }
            else if (n == ExpressionType.AndAlso)
            {
                return "and";
            }
            else if (n == ExpressionType.Or)
            {
                return "|";
            }
            else if (n == ExpressionType.OrElse)
            {
                return "or";
            }
            else if (n == ExpressionType.Equal)
            {
                return "=";
            }
            else if (n == ExpressionType.NotEqual)
            {
                return "!=";
            }
            else
            {
                throw new NotSupportedException("Cannot get SQL for: " + n);
            }
        }

        public int Count()
        {
            return GenerateCommand("count(*)").ExecuteScalar<int>();
        }

        public int Count(Expression<Func<T, bool>> predExpr)
        {
            return Where(predExpr).Count();
        }

        public IEnumerator<T> GetEnumerator()
        {
            if (!_deferred)
                return GenerateCommand("*").ExecuteQuery<T>().GetEnumerator();

            return GenerateCommand("*").ExecuteDeferredQuery<T>().GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public T First()
        {
            var query = Take(1);
            return query.ToList<T>().First();
        }

        public T FirstOrDefault()
        {
            var query = Take(1);
            return query.ToList<T>().FirstOrDefault();
        }
    }

    public static class SQLite3
    {
        public enum Result : int
        {
            OK = 0,
            Error = 1,
            Internal = 2,
            Perm = 3,
            Abort = 4,
            Busy = 5,
            Locked = 6,
            NoMem = 7,
            ReadOnly = 8,
            Interrupt = 9,
            IOError = 10,
            Corrupt = 11,
            NotFound = 12,
            Full = 13,
            CannotOpen = 14,
            LockErr = 15,
            Empty = 16,
            SchemaChngd = 17,
            TooBig = 18,
            Constraint = 19,
            Mismatch = 20,
            Misuse = 21,
            NotImplementedLFS = 22,
            AccessDenied = 23,
            Format = 24,
            Range = 25,
            NonDBFile = 26,
            Notice = 27,
            Warning = 28,
            Row = 100,
            Done = 101
        }

        public enum ExtendedResult : int
        {
            IOErrorRead = (Result.IOError | (1 << 8)),
            IOErrorShortRead = (Result.IOError | (2 << 8)),
            IOErrorWrite = (Result.IOError | (3 << 8)),
            IOErrorFsync = (Result.IOError | (4 << 8)),
            IOErrorDirFSync = (Result.IOError | (5 << 8)),
            IOErrorTruncate = (Result.IOError | (6 << 8)),
            IOErrorFStat = (Result.IOError | (7 << 8)),
            IOErrorUnlock = (Result.IOError | (8 << 8)),
            IOErrorRdlock = (Result.IOError | (9 << 8)),
            IOErrorDelete = (Result.IOError | (10 << 8)),
            IOErrorBlocked = (Result.IOError | (11 << 8)),
            IOErrorNoMem = (Result.IOError | (12 << 8)),
            IOErrorAccess = (Result.IOError | (13 << 8)),
            IOErrorCheckReservedLock = (Result.IOError | (14 << 8)),
            IOErrorLock = (Result.IOError | (15 << 8)),
            IOErrorClose = (Result.IOError | (16 << 8)),
            IOErrorDirClose = (Result.IOError | (17 << 8)),
            IOErrorSHMOpen = (Result.IOError | (18 << 8)),
            IOErrorSHMSize = (Result.IOError | (19 << 8)),
            IOErrorSHMLock = (Result.IOError | (20 << 8)),
            IOErrorSHMMap = (Result.IOError | (21 << 8)),
            IOErrorSeek = (Result.IOError | (22 << 8)),
            IOErrorDeleteNoEnt = (Result.IOError | (23 << 8)),
            IOErrorMMap = (Result.IOError | (24 << 8)),
            LockedSharedcache = (Result.Locked | (1 << 8)),
            BusyRecovery = (Result.Busy | (1 << 8)),
            CannottOpenNoTempDir = (Result.CannotOpen | (1 << 8)),
            CannotOpenIsDir = (Result.CannotOpen | (2 << 8)),
            CannotOpenFullPath = (Result.CannotOpen | (3 << 8)),
            CorruptVTab = (Result.Corrupt | (1 << 8)),
            ReadonlyRecovery = (Result.ReadOnly | (1 << 8)),
            ReadonlyCannotLock = (Result.ReadOnly | (2 << 8)),
            ReadonlyRollback = (Result.ReadOnly | (3 << 8)),
            AbortRollback = (Result.Abort | (2 << 8)),
            ConstraintCheck = (Result.Constraint | (1 << 8)),
            ConstraintCommitHook = (Result.Constraint | (2 << 8)),
            ConstraintForeignKey = (Result.Constraint | (3 << 8)),
            ConstraintFunction = (Result.Constraint | (4 << 8)),
            ConstraintNotNull = (Result.Constraint | (5 << 8)),
            ConstraintPrimaryKey = (Result.Constraint | (6 << 8)),
            ConstraintTrigger = (Result.Constraint | (7 << 8)),
            ConstraintUnique = (Result.Constraint | (8 << 8)),
            ConstraintVTab = (Result.Constraint | (9 << 8)),
            NoticeRecoverWAL = (Result.Notice | (1 << 8)),
            NoticeRecoverRollback = (Result.Notice | (2 << 8))
        }


        public enum ConfigOption : int
        {
            SingleThread = 1,
            MultiThread = 2,
            Serialized = 3
        }

#if !USE_CSHARP_SQLITE && !USE_WP8_NATIVE_SQLITE
        [DllImport("sqlite3", EntryPoint = "sqlite3_open", CallingConvention = CallingConvention.Cdecl)]
        public static extern Result Open([MarshalAs(UnmanagedType.LPStr)] string filename, out IntPtr db);

        [DllImport("sqlite3", EntryPoint = "sqlite3_open_v2", CallingConvention = CallingConvention.Cdecl)]
        public static extern Result Open([MarshalAs(UnmanagedType.LPStr)] string filename, out IntPtr db, int flags, IntPtr zvfs);

        [DllImport("sqlite3", EntryPoint = "sqlite3_open_v2", CallingConvention = CallingConvention.Cdecl)]
        public static extern Result Open(byte[] filename, out IntPtr db, int flags, IntPtr zvfs);

        [DllImport("sqlite3", EntryPoint = "sqlite3_open16", CallingConvention = CallingConvention.Cdecl)]
        public static extern Result Open16([MarshalAs(UnmanagedType.LPWStr)] string filename, out IntPtr db);

        [DllImport("sqlite3", EntryPoint = "sqlite3_enable_load_extension", CallingConvention = CallingConvention.Cdecl)]
        public static extern Result EnableLoadExtension(IntPtr db, int onoff);

        [DllImport("sqlite3", EntryPoint = "sqlite3_close", CallingConvention = CallingConvention.Cdecl)]
        public static extern Result Close(IntPtr db);

        [DllImport("sqlite3", EntryPoint = "sqlite3_initialize", CallingConvention = CallingConvention.Cdecl)]
        public static extern Result Initialize();

        [DllImport("sqlite3", EntryPoint = "sqlite3_shutdown", CallingConvention = CallingConvention.Cdecl)]
        public static extern Result Shutdown();

        [DllImport("sqlite3", EntryPoint = "sqlite3_config", CallingConvention = CallingConvention.Cdecl)]
        public static extern Result Config(ConfigOption option);

        [DllImport("sqlite3", EntryPoint = "sqlite3_win32_set_directory", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern int SetDirectory(uint directoryType, string directoryPath);

        [DllImport("sqlite3", EntryPoint = "sqlite3_busy_timeout", CallingConvention = CallingConvention.Cdecl)]
        public static extern Result BusyTimeout(IntPtr db, int milliseconds);

        [DllImport("sqlite3", EntryPoint = "sqlite3_changes", CallingConvention = CallingConvention.Cdecl)]
        public static extern int Changes(IntPtr db);

        [DllImport("sqlite3", EntryPoint = "sqlite3_prepare_v2", CallingConvention = CallingConvention.Cdecl)]
        public static extern Result Prepare2(IntPtr db, [MarshalAs(UnmanagedType.LPStr)] string sql, int numBytes, out IntPtr stmt, IntPtr pzTail);

#if NETFX_CORE
		[DllImport ("sqlite3", EntryPoint = "sqlite3_prepare_v2", CallingConvention = CallingConvention.Cdecl)]
		public static extern Result Prepare2 (IntPtr db, byte[] queryBytes, int numBytes, out IntPtr stmt, IntPtr pzTail);
#endif

        public static IntPtr Prepare2(IntPtr db, string query)
        {
            IntPtr stmt;
#if NETFX_CORE
			byte[] queryBytes = System.Text.UTF8Encoding.UTF8.GetBytes (query);
			var r = Prepare2 (db, queryBytes, queryBytes.Length, out stmt, IntPtr.Zero);
#else
            var r = Prepare2(db, query, System.Text.UTF8Encoding.UTF8.GetByteCount(query), out stmt, IntPtr.Zero);
#endif
            if (r != Result.OK)
            {
                throw SQLiteException.New(r, GetErrmsg(db));
            }
            return stmt;
        }

        [DllImport("sqlite3", EntryPoint = "sqlite3_step", CallingConvention = CallingConvention.Cdecl)]
        public static extern Result Step(IntPtr stmt);

        [DllImport("sqlite3", EntryPoint = "sqlite3_reset", CallingConvention = CallingConvention.Cdecl)]
        public static extern Result Reset(IntPtr stmt);

        [DllImport("sqlite3", EntryPoint = "sqlite3_finalize", CallingConvention = CallingConvention.Cdecl)]
        public static extern Result Finalize(IntPtr stmt);

        [DllImport("sqlite3", EntryPoint = "sqlite3_last_insert_rowid", CallingConvention = CallingConvention.Cdecl)]
        public static extern long LastInsertRowid(IntPtr db);

        [DllImport("sqlite3", EntryPoint = "sqlite3_errmsg16", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr Errmsg(IntPtr db);

        public static string GetErrmsg(IntPtr db)
        {
            return Marshal.PtrToStringUni(Errmsg(db));
        }

        [DllImport("sqlite3", EntryPoint = "sqlite3_bind_parameter_index", CallingConvention = CallingConvention.Cdecl)]
        public static extern int BindParameterIndex(IntPtr stmt, [MarshalAs(UnmanagedType.LPStr)] string name);

        [DllImport("sqlite3", EntryPoint = "sqlite3_bind_null", CallingConvention = CallingConvention.Cdecl)]
        public static extern int BindNull(IntPtr stmt, int index);

        [DllImport("sqlite3", EntryPoint = "sqlite3_bind_int", CallingConvention = CallingConvention.Cdecl)]
        public static extern int BindInt(IntPtr stmt, int index, int val);

        [DllImport("sqlite3", EntryPoint = "sqlite3_bind_int64", CallingConvention = CallingConvention.Cdecl)]
        public static extern int BindInt64(IntPtr stmt, int index, long val);

        [DllImport("sqlite3", EntryPoint = "sqlite3_bind_double", CallingConvention = CallingConvention.Cdecl)]
        public static extern int BindDouble(IntPtr stmt, int index, double val);

        [DllImport("sqlite3", EntryPoint = "sqlite3_bind_text16", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern int BindText(IntPtr stmt, int index, [MarshalAs(UnmanagedType.LPWStr)] string val, int n, IntPtr free);

        [DllImport("sqlite3", EntryPoint = "sqlite3_bind_blob", CallingConvention = CallingConvention.Cdecl)]
        public static extern int BindBlob(IntPtr stmt, int index, byte[] val, int n, IntPtr free);

        [DllImport("sqlite3", EntryPoint = "sqlite3_column_count", CallingConvention = CallingConvention.Cdecl)]
        public static extern int ColumnCount(IntPtr stmt);

        [DllImport("sqlite3", EntryPoint = "sqlite3_column_name", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr ColumnName(IntPtr stmt, int index);

        [DllImport("sqlite3", EntryPoint = "sqlite3_column_name16", CallingConvention = CallingConvention.Cdecl)]
        static extern IntPtr ColumnName16Internal(IntPtr stmt, int index);
        public static string ColumnName16(IntPtr stmt, int index)
        {
            return Marshal.PtrToStringUni(ColumnName16Internal(stmt, index));
        }

        [DllImport("sqlite3", EntryPoint = "sqlite3_column_type", CallingConvention = CallingConvention.Cdecl)]
        public static extern ColType ColumnType(IntPtr stmt, int index);

        [DllImport("sqlite3", EntryPoint = "sqlite3_column_int", CallingConvention = CallingConvention.Cdecl)]
        public static extern int ColumnInt(IntPtr stmt, int index);

        [DllImport("sqlite3", EntryPoint = "sqlite3_column_int64", CallingConvention = CallingConvention.Cdecl)]
        public static extern long ColumnInt64(IntPtr stmt, int index);

        [DllImport("sqlite3", EntryPoint = "sqlite3_column_double", CallingConvention = CallingConvention.Cdecl)]
        public static extern double ColumnDouble(IntPtr stmt, int index);

        [DllImport("sqlite3", EntryPoint = "sqlite3_column_text", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr ColumnText(IntPtr stmt, int index);

        [DllImport("sqlite3", EntryPoint = "sqlite3_column_text16", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr ColumnText16(IntPtr stmt, int index);

        [DllImport("sqlite3", EntryPoint = "sqlite3_column_blob", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr ColumnBlob(IntPtr stmt, int index);

        [DllImport("sqlite3", EntryPoint = "sqlite3_column_bytes", CallingConvention = CallingConvention.Cdecl)]
        public static extern int ColumnBytes(IntPtr stmt, int index);

        public static string ColumnString(IntPtr stmt, int index)
        {
            return Marshal.PtrToStringUni(SQLite3.ColumnText16(stmt, index));
        }

        public static byte[] ColumnByteArray(IntPtr stmt, int index)
        {
            int length = ColumnBytes(stmt, index);
            var result = new byte[length];
            if (length > 0)
                Marshal.Copy(ColumnBlob(stmt, index), result, 0, length);
            return result;
        }

        [DllImport("sqlite3", EntryPoint = "sqlite3_extended_errcode", CallingConvention = CallingConvention.Cdecl)]
        public static extern ExtendedResult ExtendedErrCode(IntPtr db);

        [DllImport("sqlite3", EntryPoint = "sqlite3_libversion_number", CallingConvention = CallingConvention.Cdecl)]
        public static extern int LibVersionNumber();
#else
		public static Result Open(string filename, out Sqlite3DatabaseHandle db)
		{
			return (Result) Sqlite3.sqlite3_open(filename, out db);
		}

		public static Result Open(string filename, out Sqlite3DatabaseHandle db, int flags, IntPtr zVfs)
		{
#if USE_WP8_NATIVE_SQLITE
			return (Result)Sqlite3.sqlite3_open_v2(filename, out db, flags, "");
#else
			return (Result)Sqlite3.sqlite3_open_v2(filename, out db, flags, null);
#endif
		}

		public static Result Close(Sqlite3DatabaseHandle db)
		{
			return (Result)Sqlite3.sqlite3_close(db);
		}

		public static Result BusyTimeout(Sqlite3DatabaseHandle db, int milliseconds)
		{
			return (Result)Sqlite3.sqlite3_busy_timeout(db, milliseconds);
		}

		public static int Changes(Sqlite3DatabaseHandle db)
		{
			return Sqlite3.sqlite3_changes(db);
		}

		public static Sqlite3Statement Prepare2(Sqlite3DatabaseHandle db, string query)
		{
			Sqlite3Statement stmt = default(Sqlite3Statement);
#if USE_WP8_NATIVE_SQLITE
			var r = Sqlite3.sqlite3_prepare_v2(db, query, out stmt);
#else
			stmt = new Sqlite3Statement();
			var r = Sqlite3.sqlite3_prepare_v2(db, query, -1, ref stmt, 0);
#endif
			if (r != 0)
			{
				throw SQLiteException.New((Result)r, GetErrmsg(db));
			}
			return stmt;
		}

		public static Result Step(Sqlite3Statement stmt)
		{
			return (Result)Sqlite3.sqlite3_step(stmt);
		}

		public static Result Reset(Sqlite3Statement stmt)
		{
			return (Result)Sqlite3.sqlite3_reset(stmt);
		}

		public static Result Finalize(Sqlite3Statement stmt)
		{
			return (Result)Sqlite3.sqlite3_finalize(stmt);
		}

		public static long LastInsertRowid(Sqlite3DatabaseHandle db)
		{
			return Sqlite3.sqlite3_last_insert_rowid(db);
		}

		public static string GetErrmsg(Sqlite3DatabaseHandle db)
		{
			return Sqlite3.sqlite3_errmsg(db);
		}

		public static int BindParameterIndex(Sqlite3Statement stmt, string name)
		{
			return Sqlite3.sqlite3_bind_parameter_index(stmt, name);
		}

		public static int BindNull(Sqlite3Statement stmt, int index)
		{
			return Sqlite3.sqlite3_bind_null(stmt, index);
		}

		public static int BindInt(Sqlite3Statement stmt, int index, int val)
		{
			return Sqlite3.sqlite3_bind_int(stmt, index, val);
		}

		public static int BindInt64(Sqlite3Statement stmt, int index, long val)
		{
			return Sqlite3.sqlite3_bind_int64(stmt, index, val);
		}

		public static int BindDouble(Sqlite3Statement stmt, int index, double val)
		{
			return Sqlite3.sqlite3_bind_double(stmt, index, val);
		}

		public static int BindText(Sqlite3Statement stmt, int index, string val, int n, IntPtr free)
		{
#if USE_WP8_NATIVE_SQLITE
			return Sqlite3.sqlite3_bind_text(stmt, index, val, n);
#else
			return Sqlite3.sqlite3_bind_text(stmt, index, val, n, null);
#endif
		}

		public static int BindBlob(Sqlite3Statement stmt, int index, byte[] val, int n, IntPtr free)
		{
#if USE_WP8_NATIVE_SQLITE
			return Sqlite3.sqlite3_bind_blob(stmt, index, val, n);
#else
			return Sqlite3.sqlite3_bind_blob(stmt, index, val, n, null);
#endif
		}

		public static int ColumnCount(Sqlite3Statement stmt)
		{
			return Sqlite3.sqlite3_column_count(stmt);
		}

		public static string ColumnName(Sqlite3Statement stmt, int index)
		{
			return Sqlite3.sqlite3_column_name(stmt, index);
		}

		public static string ColumnName16(Sqlite3Statement stmt, int index)
		{
			return Sqlite3.sqlite3_column_name(stmt, index);
		}

		public static ColType ColumnType(Sqlite3Statement stmt, int index)
		{
			return (ColType)Sqlite3.sqlite3_column_type(stmt, index);
		}

		public static int ColumnInt(Sqlite3Statement stmt, int index)
		{
			return Sqlite3.sqlite3_column_int(stmt, index);
		}

		public static long ColumnInt64(Sqlite3Statement stmt, int index)
		{
			return Sqlite3.sqlite3_column_int64(stmt, index);
		}

		public static double ColumnDouble(Sqlite3Statement stmt, int index)
		{
			return Sqlite3.sqlite3_column_double(stmt, index);
		}

		public static string ColumnText(Sqlite3Statement stmt, int index)
		{
			return Sqlite3.sqlite3_column_text(stmt, index);
		}

		public static string ColumnText16(Sqlite3Statement stmt, int index)
		{
			return Sqlite3.sqlite3_column_text(stmt, index);
		}

		public static byte[] ColumnBlob(Sqlite3Statement stmt, int index)
		{
			return Sqlite3.sqlite3_column_blob(stmt, index);
		}

		public static int ColumnBytes(Sqlite3Statement stmt, int index)
		{
			return Sqlite3.sqlite3_column_bytes(stmt, index);
		}

		public static string ColumnString(Sqlite3Statement stmt, int index)
		{
			return Sqlite3.sqlite3_column_text(stmt, index);
		}

		public static byte[] ColumnByteArray(Sqlite3Statement stmt, int index)
		{
			return ColumnBlob(stmt, index);
		}

		public static Result EnableLoadExtension(Sqlite3DatabaseHandle db, int onoff)
		{
			return (Result)Sqlite3.sqlite3_enable_load_extension(db, onoff);
		}

		public static ExtendedResult ExtendedErrCode(Sqlite3DatabaseHandle db)
		{
			return (ExtendedResult)Sqlite3.sqlite3_extended_errcode(db);
		}
#endif

        public enum ColType : int
        {
            Integer = 1,
            Float = 2,
            Text = 3,
            Blob = 4,
            Null = 5
        }
    }
}
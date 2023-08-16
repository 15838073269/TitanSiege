using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using Net.Share;
using Net.Event;
#if SERVER
using Net.System;
using Cysharp.Threading.Tasks;
using MySql.Data.MySqlClient;
#endif
using BooleanObs = Net.Common.PropertyObserverAuto<bool>;
using ByteObs = Net.Common.PropertyObserverAuto<byte>;
using SByteObs = Net.Common.PropertyObserverAuto<sbyte>;
using Int16Obs = Net.Common.PropertyObserverAuto<short>;
using UInt16Obs = Net.Common.PropertyObserverAuto<ushort>;
using CharObs = Net.Common.PropertyObserverAuto<char>;
using Int32Obs = Net.Common.PropertyObserverAuto<int>;
using UInt32Obs = Net.Common.PropertyObserverAuto<uint>;
using SingleObs = Net.Common.PropertyObserverAuto<float>;
using Int64Obs = Net.Common.PropertyObserverAuto<long>;
using UInt64Obs = Net.Common.PropertyObserverAuto<ulong>;
using DoubleObs = Net.Common.PropertyObserverAuto<double>;
using DateTimeObs = Net.Common.PropertyObserverAuto<System.DateTime>;
using BytesObs = Net.Common.PropertyObserverAuto<byte[]>;
using StringObs = Net.Common.PropertyObserverAuto<string>;

namespace Titansiege
{
    /// <summary>
    /// 此类由MySqlDataBuild工具生成, 请不要在此类编辑代码! 请新建一个类文件进行分写
    /// <para>MySqlDataBuild工具提供Rpc自动同步到mysql数据库的功能, 提供数据库注释功能</para>
    /// <para><see href="此脚本支持防内存修改器, 需要在uniyt的预编译处添加:ANTICHEAT关键字即可"/> </para>
    /// MySqlDataBuild工具gitee地址:https://gitee.com/leng_yue/my-sql-data-build
    /// </summary>
    public partial class ConfigData : IDataRow
    {
        [Net.Serialize.NonSerialized]
        [Newtonsoft_X.Json.JsonIgnore]
        public DataRowState RowState { get; set; }
    #if SERVER
        internal Net.Server.NetPlayer client;
    #endif
        /// <summary>当属性被修改时调用, 参数1: 哪个字段被修改(表名_字段名), 参数2:被修改的值</summary>
        [Net.Serialize.NonSerialized]
        [Newtonsoft_X.Json.JsonIgnore]
        public Action<TitansiegeHashProto, object> OnValueChanged;

        private Int32 id;
        /// <summary>表id</summary>
        public Int32 Id { get { return id; } set { this.id = value; } }

     
        private readonly StringObs tname = new StringObs("ConfigData_tname", false, null);

        /// <summary>表名称 --获得属性观察对象</summary>
        internal StringObs TnameObserver => tname;

        /// <summary>表名称</summary>
        public String Tname { get => GetTnameValue(); set => CheckTnameValue(value, 0); }

        /// <summary>表名称 --同步到数据库</summary>
        internal String SyncTname { get => GetTnameValue(); set => CheckTnameValue(value, 1); }

        /// <summary>表名称 --同步带有Key字段的值到服务器Player对象上，需要处理</summary>
        internal String SyncIDTname { get => GetTnameValue(); set => CheckTnameValue(value, 2); }

        private String GetTnameValue() => this.tname.Value;

        private void CheckTnameValue(String value, int type) 
        {
            if (this.tname.Value == value)
                return;
            this.tname.Value = value;
            if (type == 0)
                CheckUpdate(1);
            else if (type == 1)
                TnameCall(false);
            else if (type == 2)
                TnameCall(true);
            OnValueChanged?.Invoke(TitansiegeHashProto.CONFIG_TNAME, value);
        }

        /// <summary>表名称 --同步当前值到服务器Player对象上，需要处理</summary>
        public void TnameCall(bool syncId = false)
        {
            
            object[] objects;
            if (syncId) objects = new object[] { this[TitansiegeDBEvent.ConfigData_SyncID], tname.Value };
            else objects = new object[] { tname.Value };
#if SERVER
            CheckUpdate(1);
            TitansiegeDBEvent.OnSyncProperty?.Invoke(client, NetCmd.SyncPropertyData, (ushort)TitansiegeHashProto.CONFIG_TNAME, objects);
#else
            TitansiegeDBEvent.Client.SendRT(NetCmd.SyncPropertyData, (ushort)TitansiegeHashProto.CONFIG_TNAME, objects);
#endif
        }

        [Rpc(hash = (ushort)TitansiegeHashProto.CONFIG_TNAME)]
        private void TnameRpc(String value)//重写NetPlayer的OnStart方法来处理客户端自动同步到服务器数据库, 方法内部添加AddRpc(data(ConfigData));收集Rpc
        {
            Tname = value;
        }
     
        private readonly Int32Obs count = new Int32Obs("ConfigData_count", true, null);

        /// <summary>表计数 --获得属性观察对象</summary>
        internal Int32Obs CountObserver => count;

        /// <summary>表计数</summary>
        public Int32 Count { get => GetCountValue(); set => CheckCountValue(value, 0); }

        /// <summary>表计数 --同步到数据库</summary>
        internal Int32 SyncCount { get => GetCountValue(); set => CheckCountValue(value, 1); }

        /// <summary>表计数 --同步带有Key字段的值到服务器Player对象上，需要处理</summary>
        internal Int32 SyncIDCount { get => GetCountValue(); set => CheckCountValue(value, 2); }

        private Int32 GetCountValue() => this.count.Value;

        private void CheckCountValue(Int32 value, int type) 
        {
            if (this.count.Value == value)
                return;
            this.count.Value = value;
            if (type == 0)
                CheckUpdate(2);
            else if (type == 1)
                CountCall(false);
            else if (type == 2)
                CountCall(true);
            OnValueChanged?.Invoke(TitansiegeHashProto.CONFIG_COUNT, value);
        }

        /// <summary>表计数 --同步当前值到服务器Player对象上，需要处理</summary>
        public void CountCall(bool syncId = false)
        {
            
            object[] objects;
            if (syncId) objects = new object[] { this[TitansiegeDBEvent.ConfigData_SyncID], count.Value };
            else objects = new object[] { count.Value };
#if SERVER
            CheckUpdate(2);
            TitansiegeDBEvent.OnSyncProperty?.Invoke(client, NetCmd.SyncPropertyData, (ushort)TitansiegeHashProto.CONFIG_COUNT, objects);
#else
            TitansiegeDBEvent.Client.SendRT(NetCmd.SyncPropertyData, (ushort)TitansiegeHashProto.CONFIG_COUNT, objects);
#endif
        }

        [Rpc(hash = (ushort)TitansiegeHashProto.CONFIG_COUNT)]
        private void CountRpc(Int32 value)//重写NetPlayer的OnStart方法来处理客户端自动同步到服务器数据库, 方法内部添加AddRpc(data(ConfigData));收集Rpc
        {
            Count = value;
        }
     
        private readonly StringObs describle = new StringObs("ConfigData_describle", false, null);

        /// <summary>表描述 --获得属性观察对象</summary>
        internal StringObs DescribleObserver => describle;

        /// <summary>表描述</summary>
        public String Describle { get => GetDescribleValue(); set => CheckDescribleValue(value, 0); }

        /// <summary>表描述 --同步到数据库</summary>
        internal String SyncDescrible { get => GetDescribleValue(); set => CheckDescribleValue(value, 1); }

        /// <summary>表描述 --同步带有Key字段的值到服务器Player对象上，需要处理</summary>
        internal String SyncIDDescrible { get => GetDescribleValue(); set => CheckDescribleValue(value, 2); }

        private String GetDescribleValue() => this.describle.Value;

        private void CheckDescribleValue(String value, int type) 
        {
            if (this.describle.Value == value)
                return;
            this.describle.Value = value;
            if (type == 0)
                CheckUpdate(3);
            else if (type == 1)
                DescribleCall(false);
            else if (type == 2)
                DescribleCall(true);
            OnValueChanged?.Invoke(TitansiegeHashProto.CONFIG_DESCRIBLE, value);
        }

        /// <summary>表描述 --同步当前值到服务器Player对象上，需要处理</summary>
        public void DescribleCall(bool syncId = false)
        {
            
            object[] objects;
            if (syncId) objects = new object[] { this[TitansiegeDBEvent.ConfigData_SyncID], describle.Value };
            else objects = new object[] { describle.Value };
#if SERVER
            CheckUpdate(3);
            TitansiegeDBEvent.OnSyncProperty?.Invoke(client, NetCmd.SyncPropertyData, (ushort)TitansiegeHashProto.CONFIG_DESCRIBLE, objects);
#else
            TitansiegeDBEvent.Client.SendRT(NetCmd.SyncPropertyData, (ushort)TitansiegeHashProto.CONFIG_DESCRIBLE, objects);
#endif
        }

        [Rpc(hash = (ushort)TitansiegeHashProto.CONFIG_DESCRIBLE)]
        private void DescribleRpc(String value)//重写NetPlayer的OnStart方法来处理客户端自动同步到服务器数据库, 方法内部添加AddRpc(data(ConfigData));收集Rpc
        {
            Describle = value;
        }
     

        public ConfigData() { }

    #if SERVER
        public ConfigData(params object[] parms) : base()
        {
            NewTableRow(parms);
        }
        public void NewTableRow()
        {
            RowState = DataRowState.Added;
            TitansiegeDB.I.Update(this);
        }
        public object GetDefaultValue(Type type)
        {
            return type.IsValueType ? Activator.CreateInstance(type) : null;
        }
        public void NewTableRow(params object[] parms)
        {
            if (parms == null)
                return;
            if (parms.Length == 0)
                return;
            for (int i = 0; i < parms.Length; i++)
            {
                if (parms[i] == null)
                    continue;
                this[i] = parms[i];
            }
            RowState = DataRowState.Added;
            TitansiegeDB.I.Update(this);
        }
        public string GetCellNameAndTextLength(int index, out uint length)
        {
            switch (index)
            {
     
                case 0: length = 0; return "id";
     
                case 1: length = 20; return "tname";
     
                case 2: length = 0; return "count";
     
                case 3: length = 200; return "describle";
     
            }
            throw new Exception("错误");
        }
    #endif

        public object this[int index]
        {
            get
            {
                switch (index)
                {
     
                    case 0: return this.id;
     
                    case 1: return this.tname.Value;
     
                    case 2: return this.count.Value;
     
                    case 3: return this.describle.Value;
     
                }
                throw new Exception("错误");
            }
            set
            {
                switch (index)
                {
     
                    case 0:
                        this.id = (Int32)value;
                        break;
     
                    case 1:
                        CheckTnameValue((String)value, -1);
                        break;
     
                    case 2:
                        CheckCountValue((Int32)value, -1);
                        break;
     
                    case 3:
                        CheckDescribleValue((String)value, -1);
                        break;
     
                }
            }
        }

    #if SERVER
        public void Delete(bool immediately = false)
        {
            if (immediately)
            {
                var sb = new StringBuilder();
                DeletedSql(sb);
                _ = TitansiegeDB.I.ExecuteNonQuery(sb.ToString(), null);
            }
            else
            {
                RowState = DataRowState.Detached;
                TitansiegeDB.I.Update(this);
            }
        }

        /// <summary>
        /// 查询1: Query("`id`=1");
        /// <para></para>
        /// 查询2: Query("`id`=1 and `index`=1");
        /// <para></para>
        /// 查询3: Query("`id`=1 or `index`=1");
        /// </summary>
        /// <param name="filterExpression"></param>
        /// <returns></returns>
        public static ConfigData Query(string filterExpression)
        {
            var cmdText = $"select * from config where {filterExpression}; ";
            var data = TitansiegeDB.I.ExecuteQuery<ConfigData>(cmdText);
            return data;
        }
        /// <summary>
        /// 查询1: Query("`id`=1");
        /// <para></para>
        /// 查询2: Query("`id`=1 and `index`=1");
        /// <para></para>
        /// 查询3: Query("`id`=1 or `index`=1");
        /// </summary>
        /// <param name="filterExpression"></param>
        /// <returns></returns>
        public static async UniTask<ConfigData> QueryAsync(string filterExpression)
        {
            var cmdText = $"select * from config where {filterExpression}; ";
            var data = await TitansiegeDB.I.ExecuteQueryAsync<ConfigData>(cmdText);
            return data;
        }
        public static ConfigData[] QueryList(string filterExpression)
        {
            var cmdText = $"select * from config where {filterExpression}; ";
            var datas = TitansiegeDB.I.ExecuteQueryList<ConfigData>(cmdText);
            return datas;
        }
        public static async UniTask<ConfigData[]> QueryListAsync(string filterExpression)
        {
            var cmdText = $"select * from config where {filterExpression}; ";
            var datas = await TitansiegeDB.I.ExecuteQueryListAsync<ConfigData>(cmdText);
            return datas;
        }
        public void Update()
        {
            if (RowState == DataRowState.Deleted | RowState == DataRowState.Detached | RowState == DataRowState.Added | RowState == 0) return;
     
            RowState = DataRowState.Modified;
            TitansiegeDB.I.Update(this);
     
        }
    #endif

        public void Init(DataRow row)
        {
            RowState = DataRowState.Unchanged;
     
            if (row[0] is Int32 id)
                this.id = id;
     
            if (row[1] is String tname)
                CheckTnameValue(tname, -1);
     
            if (row[2] is Int32 count)
                CheckCountValue(count, -1);
     
            if (row[3] is String describle)
                CheckDescribleValue(describle, -1);
     
        }

        public void AddedSql(StringBuilder sb)
        {
    #if SERVER
    
            BulkLoaderBuilder(sb);
    
            RowState = DataRowState.Unchanged;
    #endif
        }

        public void ModifiedSql(StringBuilder sb)
        {
    #if SERVER
            if (RowState == DataRowState.Detached | RowState == DataRowState.Deleted | RowState == DataRowState.Added | RowState == 0)
                return;
            BulkLoaderBuilder(sb);
            RowState = DataRowState.Unchanged;
    #endif
        }

        public void DeletedSql(StringBuilder sb)
        {
    #if SERVER
            if (RowState == DataRowState.Deleted)
                return;
            string cmdText = $"DELETE FROM config {CheckSqlKey(0, id)}";
            sb.Append(cmdText);
            RowState = DataRowState.Deleted;
    #endif
        }

    #if SERVER
        public void BulkLoaderBuilder(StringBuilder sb)
        {
 
            for (int i = 0; i < 4; i++)
            {
                var name = GetCellNameAndTextLength(i, out var length);
                var value = this[i];
                if (value == null) //空的值会导致sql语句错误
                {
                    sb.Append(@"\N|");
                    continue;
                }
                if (value is string text)
                {
                    TitansiegeDB.I.CheckStringValue(ref text, length);
                    sb.Append(text + "|");
                }
                else if (value is DateTime dateTime)
                {
                    sb.Append(dateTime.ToString("yyyy-MM-dd HH:mm:ss") + "|");
                }
                else if (value is bool boolVal)
                {
                    sb.Append(boolVal ? "1|" : "0|");
                }
                else if (value is byte[] buffer)
                {
                    var base64Str = Convert.ToBase64String(buffer, Base64FormattingOptions.None);
                    if (buffer.Length >= length)
                    {
                        NDebug.LogError($"config表{name}列长度溢出!");
                        sb.Append(@"\N|");
                        continue;
                    }
                    sb.Append(base64Str + "|");
                }
                else 
                {
                    sb.Append(value + "|");
                }
            }
            sb.AppendLine();
 
        }
    #endif

    #if SERVER
        private string CheckSqlKey(int column, object value)
        {
            var name = GetCellNameAndTextLength(column, out var length);
            if (value == null) //空的值会导致sql语句错误
                return "";
            if (value is string text)
            {
                TitansiegeDB.I.CheckStringValue(ref text, length);
                return $" WHERE `{name}`='{text}'; ";
            }
            else if (value is DateTime)
                return $" WHERE `{name}`='{value}'; ";
            else if (value is byte[])
                return "";
            return $" WHERE `{name}`={value}; ";
        }
    #endif

        private void CheckUpdate(int cellIndex)
        {
#if SERVER
            if (RowState == DataRowState.Deleted | RowState == DataRowState.Detached) return;
            if (RowState != DataRowState.Added & RowState != 0)//如果还没初始化或者创建新行,只能修改值不能更新状态
                RowState = DataRowState.Modified;
            TitansiegeDB.I.Update(this);
#endif
        }

        public override string ToString()
        {
            return $"Id:{Id} Tname:{Tname} Count:{Count} Describle:{Describle} ";
        }
    }
}
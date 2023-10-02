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
    public partial class BagitemData : IDataRow
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

     //1
        private readonly Int32Obs cid = new Int32Obs("BagitemData_cid", true, null);

        /// <summary>角色表id --获得属性观察对象</summary>
        internal Int32Obs CidObserver => cid;

        /// <summary>角色表id</summary>
        public Int32 Cid { get => GetCidValue(); set => CheckCidValue(value, 0); }

        /// <summary>角色表id --同步到数据库</summary>
        internal Int32 SyncCid { get => GetCidValue(); set => CheckCidValue(value, 1); }

        /// <summary>角色表id --同步带有Key字段的值到服务器Player对象上，需要处理</summary>
        internal Int32 SyncIDCid { get => GetCidValue(); set => CheckCidValue(value, 2); }

        private Int32 GetCidValue() => this.cid.Value;

        private void CheckCidValue(Int32 value, int type) 
        {
            if (this.cid.Value == value)
                return;
            this.cid.Value = value;
            if (type == 0)
                CheckUpdate(1);
            else if (type == 1)
                CidCall(false);
            else if (type == 2)
                CidCall(true);
            OnValueChanged?.Invoke(TitansiegeHashProto.BAGITEM_CID, value);
        }

        /// <summary>角色表id --同步当前值到服务器Player对象上，需要处理</summary>
        public void CidCall(bool syncId = false)
        {
            
            object[] objects;
            if (syncId) objects = new object[] { this[TitansiegeDBEvent.BagitemData_SyncID], cid.Value };
            else objects = new object[] { cid.Value };
#if SERVER
            CheckUpdate(1);
            TitansiegeDBEvent.OnSyncProperty?.Invoke(client, NetCmd.SyncPropertyData, (ushort)TitansiegeHashProto.BAGITEM_CID, objects);
#else
            TitansiegeDBEvent.Client.SendRT(NetCmd.SyncPropertyData, (ushort)TitansiegeHashProto.BAGITEM_CID, objects);
#endif
        }

        [Rpc(hash = (ushort)TitansiegeHashProto.BAGITEM_CID)]
        private void CidRpc(Int32 value)//重写NetPlayer的OnStart方法来处理客户端自动同步到服务器数据库, 方法内部添加AddRpc(data(BagitemData));收集Rpc
        {
            Cid = value;
        }
     //1
        private readonly StringObs inbag = new StringObs("BagitemData_inbag", false, null);

        /// <summary>背包内的道具及数量 --获得属性观察对象</summary>
        internal StringObs InbagObserver => inbag;

        /// <summary>背包内的道具及数量</summary>
        public String Inbag { get => GetInbagValue(); set => CheckInbagValue(value, 0); }

        /// <summary>背包内的道具及数量 --同步到数据库</summary>
        internal String SyncInbag { get => GetInbagValue(); set => CheckInbagValue(value, 1); }

        /// <summary>背包内的道具及数量 --同步带有Key字段的值到服务器Player对象上，需要处理</summary>
        internal String SyncIDInbag { get => GetInbagValue(); set => CheckInbagValue(value, 2); }

        private String GetInbagValue() => this.inbag.Value;

        private void CheckInbagValue(String value, int type) 
        {
            if (this.inbag.Value == value)
                return;
            this.inbag.Value = value;
            if (type == 0)
                CheckUpdate(2);
            else if (type == 1)
                InbagCall(false);
            else if (type == 2)
                InbagCall(true);
            OnValueChanged?.Invoke(TitansiegeHashProto.BAGITEM_INBAG, value);
        }

        /// <summary>背包内的道具及数量 --同步当前值到服务器Player对象上，需要处理</summary>
        public void InbagCall(bool syncId = false)
        {
            
            object[] objects;
            if (syncId) objects = new object[] { this[TitansiegeDBEvent.BagitemData_SyncID], inbag.Value };
            else objects = new object[] { inbag.Value };
#if SERVER
            CheckUpdate(2);
            TitansiegeDBEvent.OnSyncProperty?.Invoke(client, NetCmd.SyncPropertyData, (ushort)TitansiegeHashProto.BAGITEM_INBAG, objects);
#else
            TitansiegeDBEvent.Client.SendRT(NetCmd.SyncPropertyData, (ushort)TitansiegeHashProto.BAGITEM_INBAG, objects);
#endif
        }

        [Rpc(hash = (ushort)TitansiegeHashProto.BAGITEM_INBAG)]
        private void InbagRpc(String value)//重写NetPlayer的OnStart方法来处理客户端自动同步到服务器数据库, 方法内部添加AddRpc(data(BagitemData));收集Rpc
        {
            Inbag = value;
        }
     //2

        public BagitemData() { }

    #if SERVER
        public BagitemData(params object[] parms) : base()
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
     //3
                case 0: length = 0; return "id";
     //3
                case 1: length = 0; return "cid";
     //3
                case 2: length = 500; return "inbag";
     //4
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
     //5
                    case 0: return this.id;
     //5
                    case 1: return this.cid.Value;
     //5
                    case 2: return this.inbag.Value;
     //6
                }
                throw new Exception("错误");
            }
            set
            {
                switch (index)
                {
     //7
                    case 0:
                        this.id = (Int32)value;
                        break;
     //7
                    case 1:
                        CheckCidValue((Int32)value, -1);
                        break;
     //7
                    case 2:
                        CheckInbagValue((String)value, -1);
                        break;
     //8
                }
            }
        }

        public object this[string name]
        {
            get
            {
                switch (name)
                {
     //9
                    case "id": return this.id;
     //9
                    case "cid": return this.cid.Value;
     //9
                    case "inbag": return this.inbag.Value;
     //10
                }
                throw new Exception("错误");
            }
            set
            {
                switch (name)
                {
     //11
                    case "id":
                        this.id = (Int32)value;
                        break;
     //11
                    case "cid":
                        CheckCidValue((Int32)value, -1);
                        break;
     //11
                    case "inbag":
                        CheckInbagValue((String)value, -1);
                        break;
     //12
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
        public static BagitemData Query(string filterExpression)
        {
            var cmdText = $"select * from bagitem where {filterExpression}; ";
            var data = TitansiegeDB.I.ExecuteQuery<BagitemData>(cmdText);
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
        public static async UniTask<BagitemData> QueryAsync(string filterExpression)
        {
            var cmdText = $"select * from bagitem where {filterExpression}; ";
            var data = await TitansiegeDB.I.ExecuteQueryAsync<BagitemData>(cmdText);
            return data;
        }
        public static BagitemData[] QueryList(string filterExpression)
        {
            var cmdText = $"select * from bagitem where {filterExpression}; ";
            var datas = TitansiegeDB.I.ExecuteQueryList<BagitemData>(cmdText);
            return datas;
        }
        public static async UniTask<BagitemData[]> QueryListAsync(string filterExpression)
        {
            var cmdText = $"select * from bagitem where {filterExpression}; ";
            var datas = await TitansiegeDB.I.ExecuteQueryListAsync<BagitemData>(cmdText);
            return datas;
        }
        public void Update()
        {
            if (RowState == DataRowState.Deleted | RowState == DataRowState.Detached | RowState == DataRowState.Added | RowState == 0) return;
     //14
        }
    #endif

        public void Init(DataRow row)
        {
            RowState = DataRowState.Unchanged;
     //15
            if (row[0] is Int32 id)
                this.id = id;
     //15
            if (row[1] is Int32 cid)
                CheckCidValue(cid, -1);
     //15
            if (row[2] is String inbag)
                CheckInbagValue(inbag, -1);
     //16
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
            string cmdText = $"DELETE FROM bagitem {CheckSqlKey(0, id)}";
            sb.Append(cmdText);
            RowState = DataRowState.Deleted;
    #endif
        }

    #if SERVER
        public void BulkLoaderBuilder(StringBuilder sb)
        {
 
            for (int i = 0; i < 3; i++)
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
                        NDebug.LogError($"bagitem表{name}列长度溢出!");
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
            return $"Id:{Id} Cid:{Cid} Inbag:{Inbag} ";
        }
    }
}
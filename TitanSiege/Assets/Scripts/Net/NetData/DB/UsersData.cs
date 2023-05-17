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
    public partial class UsersData : IDataRow
    {
        [Net.Serialize.NonSerialized]
        [Newtonsoft_X.Json.JsonIgnore]
        public DataRowState RowState { get; set; }
    #if SERVER
        private readonly HashSetSafe<int> columns = new HashSetSafe<int>();
    #endif
        /// <summary>当属性被修改时调用, 参数1: 哪个字段被修改(表名_字段名), 参数2:被修改的值</summary>
        [Net.Serialize.NonSerialized]
        [Newtonsoft_X.Json.JsonIgnore]
        public Action<TitansiegeHashProto, object> OnValueChanged;

        private Int64 iD;
        /// <summary></summary>
        public Int64 ID { get { return iD; } set { this.iD = value; } }

    
        private readonly StringObs username = new StringObs("UsersData_username", false, null);

        /// <summary> --获得属性观察对象</summary>
        internal StringObs UsernameObserver => username;

        /// <summary></summary>
        public String Username { get => GetUsernameValue(); set => CheckUsernameValue(value, 0); }

        /// <summary> --同步到数据库</summary>
        internal String SyncUsername { get => GetUsernameValue(); set => CheckUsernameValue(value, 1); }

        /// <summary> --同步带有Key字段的值到服务器Player对象上，需要处理</summary>
        internal String SyncIDUsername { get => GetUsernameValue(); set => CheckUsernameValue(value, 2); }

        private String GetUsernameValue() => this.username.Value;

        private void CheckUsernameValue(String value, int type) 
        {
            if (this.username.Value == value)
                return;
            this.username.Value = value;
            if (type == 0)
                CheckUpdate(1);
            else if (type == 1)
                UsernameCall(false);
            else if (type == 2)
                UsernameCall(true);
            OnValueChanged?.Invoke(TitansiegeHashProto.USERS_USERNAME, value);
        }

        /// <summary> --同步当前值到服务器Player对象上，需要处理</summary>
        public void UsernameCall(bool syncId = false)
        {
            
            object[] objects;
            if (syncId) objects = new object[] { iD, username.Value };
            else objects = new object[] { username.Value };
            TitansiegeDBEvent.Client.SendRT(NetCmd.EntityRpc, (ushort)TitansiegeHashProto.USERS_USERNAME, objects);
        }

        [Rpc(hash = (ushort)TitansiegeHashProto.USERS_USERNAME)]
        private void UsernameRpc(String value)//重写NetPlayer的OnStart方法来处理客户端自动同步到服务器数据库, 方法内部添加AddRpc(data(UsersData));收集Rpc
        {
            Username = value;
        }
    
        private readonly StringObs password = new StringObs("UsersData_password", false, null);

        /// <summary> --获得属性观察对象</summary>
        internal StringObs PasswordObserver => password;

        /// <summary></summary>
        public String Password { get => GetPasswordValue(); set => CheckPasswordValue(value, 0); }

        /// <summary> --同步到数据库</summary>
        internal String SyncPassword { get => GetPasswordValue(); set => CheckPasswordValue(value, 1); }

        /// <summary> --同步带有Key字段的值到服务器Player对象上，需要处理</summary>
        internal String SyncIDPassword { get => GetPasswordValue(); set => CheckPasswordValue(value, 2); }

        private String GetPasswordValue() => this.password.Value;

        private void CheckPasswordValue(String value, int type) 
        {
            if (this.password.Value == value)
                return;
            this.password.Value = value;
            if (type == 0)
                CheckUpdate(2);
            else if (type == 1)
                PasswordCall(false);
            else if (type == 2)
                PasswordCall(true);
            OnValueChanged?.Invoke(TitansiegeHashProto.USERS_PASSWORD, value);
        }

        /// <summary> --同步当前值到服务器Player对象上，需要处理</summary>
        public void PasswordCall(bool syncId = false)
        {
            
            object[] objects;
            if (syncId) objects = new object[] { iD, password.Value };
            else objects = new object[] { password.Value };
            TitansiegeDBEvent.Client.SendRT(NetCmd.EntityRpc, (ushort)TitansiegeHashProto.USERS_PASSWORD, objects);
        }

        [Rpc(hash = (ushort)TitansiegeHashProto.USERS_PASSWORD)]
        private void PasswordRpc(String value)//重写NetPlayer的OnStart方法来处理客户端自动同步到服务器数据库, 方法内部添加AddRpc(data(UsersData));收集Rpc
        {
            Password = value;
        }
    
        private readonly DateTimeObs registerDate = new DateTimeObs("UsersData_registerDate", false, null);

        /// <summary> --获得属性观察对象</summary>
        internal DateTimeObs RegisterDateObserver => registerDate;

        /// <summary></summary>
        public DateTime RegisterDate { get => GetRegisterDateValue(); set => CheckRegisterDateValue(value, 0); }

        /// <summary> --同步到数据库</summary>
        internal DateTime SyncRegisterDate { get => GetRegisterDateValue(); set => CheckRegisterDateValue(value, 1); }

        /// <summary> --同步带有Key字段的值到服务器Player对象上，需要处理</summary>
        internal DateTime SyncIDRegisterDate { get => GetRegisterDateValue(); set => CheckRegisterDateValue(value, 2); }

        private DateTime GetRegisterDateValue() => this.registerDate.Value;

        private void CheckRegisterDateValue(DateTime value, int type) 
        {
            if (this.registerDate.Value == value)
                return;
            this.registerDate.Value = value;
            if (type == 0)
                CheckUpdate(3);
            else if (type == 1)
                RegisterDateCall(false);
            else if (type == 2)
                RegisterDateCall(true);
            OnValueChanged?.Invoke(TitansiegeHashProto.USERS_REGISTERDATE, value);
        }

        /// <summary> --同步当前值到服务器Player对象上，需要处理</summary>
        public void RegisterDateCall(bool syncId = false)
        {
            
            object[] objects;
            if (syncId) objects = new object[] { iD, registerDate.Value };
            else objects = new object[] { registerDate.Value };
            TitansiegeDBEvent.Client.SendRT(NetCmd.EntityRpc, (ushort)TitansiegeHashProto.USERS_REGISTERDATE, objects);
        }

        [Rpc(hash = (ushort)TitansiegeHashProto.USERS_REGISTERDATE)]
        private void RegisterDateRpc(DateTime value)//重写NetPlayer的OnStart方法来处理客户端自动同步到服务器数据库, 方法内部添加AddRpc(data(UsersData));收集Rpc
        {
            RegisterDate = value;
        }
    
        private readonly StringObs email = new StringObs("UsersData_email", false, null);

        /// <summary> --获得属性观察对象</summary>
        internal StringObs EmailObserver => email;

        /// <summary></summary>
        public String Email { get => GetEmailValue(); set => CheckEmailValue(value, 0); }

        /// <summary> --同步到数据库</summary>
        internal String SyncEmail { get => GetEmailValue(); set => CheckEmailValue(value, 1); }

        /// <summary> --同步带有Key字段的值到服务器Player对象上，需要处理</summary>
        internal String SyncIDEmail { get => GetEmailValue(); set => CheckEmailValue(value, 2); }

        private String GetEmailValue() => this.email.Value;

        private void CheckEmailValue(String value, int type) 
        {
            if (this.email.Value == value)
                return;
            this.email.Value = value;
            if (type == 0)
                CheckUpdate(4);
            else if (type == 1)
                EmailCall(false);
            else if (type == 2)
                EmailCall(true);
            OnValueChanged?.Invoke(TitansiegeHashProto.USERS_EMAIL, value);
        }

        /// <summary> --同步当前值到服务器Player对象上，需要处理</summary>
        public void EmailCall(bool syncId = false)
        {
            
            object[] objects;
            if (syncId) objects = new object[] { iD, email.Value };
            else objects = new object[] { email.Value };
            TitansiegeDBEvent.Client.SendRT(NetCmd.EntityRpc, (ushort)TitansiegeHashProto.USERS_EMAIL, objects);
        }

        [Rpc(hash = (ushort)TitansiegeHashProto.USERS_EMAIL)]
        private void EmailRpc(String value)//重写NetPlayer的OnStart方法来处理客户端自动同步到服务器数据库, 方法内部添加AddRpc(data(UsersData));收集Rpc
        {
            Email = value;
        }
    

        public UsersData() { }

    #if SERVER
        public UsersData(params object[] parms) : base()
        {
            NewTableRow(parms);
        }
        public void NewTableRow()
        {
            for (int i = 0; i < 5; i++)
            {
                var obj = this[i];
                if (obj == null)
                    continue;
                var defaultVal = GetDefaultValue(obj.GetType());
                if (obj.Equals(defaultVal))
                    continue;
                columns.Add(i);
            }
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
                columns.Add(i);
            }
            RowState = DataRowState.Added;
            TitansiegeDB.I.Update(this);
        }
        public string GetCellNameAndTextLength(int index, out int length)
        {
            switch (index)
            {
    
                case 0: length = 0; return "iD";
    
                case 1: length = 50; return "username";
    
                case 2: length = 50; return "password";
    
                case 3: length = 0; return "registerDate";
    
                case 4: length = 50; return "email";
    
            }
            throw new Exception("错误");
        }
        public object this[int index]
        {
            get
            {
                switch (index)
                {
    
                    case 0: return this.iD;
    
                    case 1: return this.username.Value;
    
                    case 2: return this.password.Value;
    
                    case 3: return this.registerDate.Value;
    
                    case 4: return this.email.Value;
    
                }
                throw new Exception("错误");
            }
            set
            {
                switch (index)
                {
    
                    case 0:
                        this.iD = (Int64)value;
                        break;
    
                    case 1:
                        CheckUsernameValue((String)value, -1);
                        break;
    
                    case 2:
                        CheckPasswordValue((String)value, -1);
                        break;
    
                    case 3:
                        CheckRegisterDateValue((DateTime)value, -1);
                        break;
    
                    case 4:
                        CheckEmailValue((String)value, -1);
                        break;
    
                }
            }
        }

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
        /// 查询1: Query("`iD`=1");
        /// <para></para>
        /// 查询2: Query("`iD`=1 and `index`=1");
        /// <para></para>
        /// 查询3: Query("`iD`=1 or `index`=1");
        /// </summary>
        /// <param name="filterExpression"></param>
        /// <returns></returns>
        public static UsersData Query(string filterExpression)
        {
            var cmdText = $"select * from users where {filterExpression}; ";
            var data = TitansiegeDB.I.ExecuteQuery<UsersData>(cmdText);
            return data;
        }
        /// <summary>
        /// 查询1: Query("`iD`=1");
        /// <para></para>
        /// 查询2: Query("`iD`=1 and `index`=1");
        /// <para></para>
        /// 查询3: Query("`iD`=1 or `index`=1");
        /// </summary>
        /// <param name="filterExpression"></param>
        /// <returns></returns>
        public static async UniTask<UsersData> QueryAsync(string filterExpression)
        {
            var cmdText = $"select * from users where {filterExpression}; ";
            var data = await TitansiegeDB.I.ExecuteQueryAsync<UsersData>(cmdText);
            return data;
        }
        public static UsersData[] QueryList(string filterExpression)
        {
            var cmdText = $"select * from users where {filterExpression}; ";
            var datas = TitansiegeDB.I.ExecuteQueryList<UsersData>(cmdText);
            return datas;
        }
        public static async UniTask<UsersData[]> QueryListAsync(string filterExpression)
        {
            var cmdText = $"select * from users where {filterExpression}; ";
            var datas = await TitansiegeDB.I.ExecuteQueryListAsync<UsersData>(cmdText);
            return datas;
        }
        public void Update() => Update(0, 5);
        public void Update(int start, int end)
        {
            if (RowState == DataRowState.Deleted | RowState == DataRowState.Detached | RowState == DataRowState.Added | RowState == 0) return;
    
            for (int i = start; i < end; i++)
                columns.Add(i);
            RowState = DataRowState.Modified;
            TitansiegeDB.I.Update(this);
    
        }
    #endif

        public void Init(DataRow row)
        {
            RowState = DataRowState.Unchanged;
    
            if (row[0] is Int64 iD)
                this.iD = iD;
    
            if (row[1] is String username)
                CheckUsernameValue(username, -1);
    
            if (row[2] is String password)
                CheckPasswordValue(password, -1);
    
            if (row[3] is DateTime registerDate)
                CheckRegisterDateValue(registerDate, -1);
    
            if (row[4] is String email)
                CheckEmailValue(email, -1);
    
        }

        public void AddedSql(StringBuilder sb, List<IDbDataParameter> parms, ref int parmsLen)
        {
    #if SERVER
            if(columns.Count == 0)//如果没有修改一个字段是不允许提交的
                return;
    
            string cmdText = "REPLACE INTO users SET ";
            foreach (var column in columns)
            {
                var name = GetCellNameAndTextLength(column, out var length);
                var value = this[column];
                if (value == null) //空的值会导致sql语句错误
                    continue;
                if (value is string text)
                {
                    CheckStringValue(ref text, length);
                    cmdText += $"`{name}`='{text}',";
                }
                else if (value is DateTime)
                    cmdText += $"`{name}`='{value}',";
                else if (value is byte[] buffer)
                {
                    if (buffer.Length >= length)
                    {
                        NDebug.LogError($"users表{name}列长度溢出!");
                        continue;
                    }
                    var count = parms.Count;
                    cmdText += $"`{name}`=@buffer{count},";
                    parms.Add(new MySqlParameter($"@buffer{count}", buffer));
                    parmsLen += buffer.Length;
                }
                else cmdText += $"`{name}`={value},";
                columns.Remove(column);
            }
            cmdText = cmdText.TrimEnd(',');
            cmdText += "; ";
    
            sb.Append(cmdText);
            RowState = DataRowState.Unchanged;
    #endif
        }

        public void ModifiedSql(StringBuilder sb, List<IDbDataParameter> parms, ref int parmsLen)
        {
    #if SERVER
            if (RowState == DataRowState.Detached | RowState == DataRowState.Deleted | RowState == DataRowState.Added | RowState == 0)
                return;
            if(columns.Count == 0)//如果没有修改一个字段是不允许提交的
                return;
            string cmdText = $"UPDATE users SET ";
            foreach (var column in columns)
            {
                var name = GetCellNameAndTextLength(column, out var length);
                var value = this[column];
                if (value == null) //空的值会导致sql语句错误
                    continue;
                if (value is string text)
                {
                    CheckStringValue(ref text, length);
                    cmdText += $"`{name}`='{text}',";
                }
                else if (value is DateTime)
                    cmdText += $"`{name}`='{value}',";
                else if (value is byte[] buffer)
                {
                    if (buffer.Length >= length)
                    {
                        NDebug.LogError($"users表{name}列长度溢出!");
                        continue;
                    }
                    var count = parms.Count;
                    cmdText += $"`{name}`=@buffer{count},";
                    parms.Add(new MySqlParameter($"@buffer{count}", buffer));
                    parmsLen += buffer.Length;
                }
                else cmdText += $"`{name}`={value},";
                columns.Remove(column);
            }
            cmdText = cmdText.TrimEnd(',');
            cmdText += CheckSqlKey(0, iD);
            sb.Append(cmdText);
            RowState = DataRowState.Unchanged;
    #endif
        }

        public void DeletedSql(StringBuilder sb)
        {
    #if SERVER
            if (RowState == DataRowState.Deleted)
                return;
            string cmdText = $"DELETE FROM users {CheckSqlKey(0, iD)}";
            sb.Append(cmdText);
            RowState = DataRowState.Deleted;
    #endif
        }

    #if SERVER
        private string CheckSqlKey(int column, object value)
        {
            var name = GetCellNameAndTextLength(column, out var length);
            if (value == null) //空的值会导致sql语句错误
                return "";
            if (value is string text)
            {
                CheckStringValue(ref text, length);
                return $" WHERE `{name}`='{text}'; ";
            }
            else if (value is DateTime)
                return $" WHERE `{name}`='{value}'; ";
            else if (value is byte[] buffer)
                return "";
            return $" WHERE `{name}`={value}; ";
        }
    #endif

        private void CheckStringValue(ref string value, int length)
        {
            if (value == null)
                value = string.Empty;
            value = value.Replace("\\", "\\\\"); //如果包含\必须转义, 否则出现 \'时就会出错
            value = value.Replace("'", "\\\'"); //出现'时必须转转义成\'
            if (value.Length >= length - 3) //必须保留三个字符做最后的判断, 如最后一个字符出现了\或'时出错问题
                value = value.Substring(0, length);
        }

        private void CheckUpdate(int cellIndex)
        {
#if SERVER
            if (RowState == DataRowState.Deleted | RowState == DataRowState.Detached) return;
            columns.Add(cellIndex);
            if (RowState != DataRowState.Added & RowState != 0)//如果还没初始化或者创建新行,只能修改值不能更新状态
                RowState = DataRowState.Modified;
            TitansiegeDB.I.Update(this);
#endif
        }
    }
}
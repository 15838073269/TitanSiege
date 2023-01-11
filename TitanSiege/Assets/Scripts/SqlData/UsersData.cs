using System;
using System.Data;
using System.Threading.Tasks;
using Net.System;
using System.Collections.Generic;
using System.Text;
#if SERVER
using MySql.Data.MySqlClient;
#endif

/// <summary>
/// 此类由MySqlDataBuild工具生成, 请不要在此类编辑代码! 请定义一个扩展类进行处理
/// MySqlDataBuild工具提供Rpc自动同步到mysql数据库的功能, 提供数据库注释功能
/// MySqlDataBuild工具gitee地址:https://gitee.com/leng_yue/my-sql-data-build
/// </summary>
public partial class UsersData : IDataRow
{
    [Net.Serialize.NonSerialized]
    [Newtonsoft_X.Json.JsonIgnore]
    public DataRowState RowState { get; set; }
    private readonly HashSetSafe<int> columns = new HashSetSafe<int>();

    private System.Int64 iD;
    /// <summary></summary>
    public System.Int64 ID
    {
        get { return iD; }
        set { this.iD = value; }
    }


    private System.String username;
    /// <summary></summary>
    public System.String Username
    {
        get { return username; }
        set
        {
            if (this.username == value)
                return;
            if(value==null) value = string.Empty;
            this.username = value;
#if SERVER
            if (RowState == DataRowState.Deleted | RowState == DataRowState.Detached) return;
            columns.Add(1);
            if(RowState != DataRowState.Added & RowState != 0)//如果还没初始化或者创建新行,只能修改值不能更新状态
                RowState = DataRowState.Modified;
            TitansiegeDB.I.Update(this);
#elif CLIENT
            UsernameCall();
#endif
        }
    }

    /// <summary> --同步到数据库</summary>
    [Net.Serialize.NonSerialized]
    [Newtonsoft_X.Json.JsonIgnore]
    public System.String SyncUsername
    {
        get { return username; }
        set
        {
            if (this.username == value)
                return;
            if(value==null) value = string.Empty;
            this.username = value;
            UsernameCall();
        }
    }

    /// <summary> --同步带有Key字段的值到服务器Player对象上，需要处理</summary>
    [Net.Serialize.NonSerialized]
    [Newtonsoft_X.Json.JsonIgnore]
    public System.String SyncIDUsername
    {
        get { return username; }
        set
        {
            if (this.username == value)
                return;
            if(value==null) value = string.Empty;
            this.username = value;
            SyncUsernameCall();
        }
    }

    /// <summary> --同步到数据库</summary>
    public void UsernameCall()
    {
        
        Net.Client.ClientBase.Instance.SendRT(Net.Share.NetCmd.EntityRpc, (ushort)TitansiegeHashProto.USERNAME, username);
    }

	/// <summary> --同步带有Key字段的值到服务器Player对象上，需要处理</summary>
    public void SyncUsernameCall()
    {
        
        Net.Client.ClientBase.Instance.SendRT(Net.Share.NetCmd.EntityRpc, (ushort)TitansiegeHashProto.USERNAME, iD, username);
    }

    [Net.Share.Rpc(hash = (ushort)TitansiegeHashProto.USERNAME)]
    private void UsernameCall(System.String value)//重写NetPlayer的OnStart方法来处理客户端自动同步到服务器数据库, 方法内部添加AddRpc(data(UsersData));收集Rpc
    {
        Username = value;
        OnUsername?.Invoke();
    }

    [Net.Serialize.NonSerialized]
    [Newtonsoft_X.Json.JsonIgnore]
    public Action OnUsername;

    private System.String password;
    /// <summary></summary>
    public System.String Password
    {
        get { return password; }
        set
        {
            if (this.password == value)
                return;
            if(value==null) value = string.Empty;
            this.password = value;
#if SERVER
            if (RowState == DataRowState.Deleted | RowState == DataRowState.Detached) return;
            columns.Add(2);
            if(RowState != DataRowState.Added & RowState != 0)//如果还没初始化或者创建新行,只能修改值不能更新状态
                RowState = DataRowState.Modified;
            TitansiegeDB.I.Update(this);
#elif CLIENT
            PasswordCall();
#endif
        }
    }

    /// <summary> --同步到数据库</summary>
    [Net.Serialize.NonSerialized]
    [Newtonsoft_X.Json.JsonIgnore]
    public System.String SyncPassword
    {
        get { return password; }
        set
        {
            if (this.password == value)
                return;
            if(value==null) value = string.Empty;
            this.password = value;
            PasswordCall();
        }
    }

    /// <summary> --同步带有Key字段的值到服务器Player对象上，需要处理</summary>
    [Net.Serialize.NonSerialized]
    [Newtonsoft_X.Json.JsonIgnore]
    public System.String SyncIDPassword
    {
        get { return password; }
        set
        {
            if (this.password == value)
                return;
            if(value==null) value = string.Empty;
            this.password = value;
            SyncPasswordCall();
        }
    }

    /// <summary> --同步到数据库</summary>
    public void PasswordCall()
    {
        
        Net.Client.ClientBase.Instance.SendRT(Net.Share.NetCmd.EntityRpc, (ushort)TitansiegeHashProto.PASSWORD, password);
    }

	/// <summary> --同步带有Key字段的值到服务器Player对象上，需要处理</summary>
    public void SyncPasswordCall()
    {
        
        Net.Client.ClientBase.Instance.SendRT(Net.Share.NetCmd.EntityRpc, (ushort)TitansiegeHashProto.PASSWORD, iD, password);
    }

    [Net.Share.Rpc(hash = (ushort)TitansiegeHashProto.PASSWORD)]
    private void PasswordCall(System.String value)//重写NetPlayer的OnStart方法来处理客户端自动同步到服务器数据库, 方法内部添加AddRpc(data(UsersData));收集Rpc
    {
        Password = value;
        OnPassword?.Invoke();
    }

    [Net.Serialize.NonSerialized]
    [Newtonsoft_X.Json.JsonIgnore]
    public Action OnPassword;

    private System.DateTime registerDate;
    /// <summary></summary>
    public System.DateTime RegisterDate
    {
        get { return registerDate; }
        set
        {
            if (this.registerDate == value)
                return;
            
            this.registerDate = value;
#if SERVER
            if (RowState == DataRowState.Deleted | RowState == DataRowState.Detached) return;
            columns.Add(3);
            if(RowState != DataRowState.Added & RowState != 0)//如果还没初始化或者创建新行,只能修改值不能更新状态
                RowState = DataRowState.Modified;
            TitansiegeDB.I.Update(this);
#elif CLIENT
            RegisterDateCall();
#endif
        }
    }

    /// <summary> --同步到数据库</summary>
    [Net.Serialize.NonSerialized]
    [Newtonsoft_X.Json.JsonIgnore]
    public System.DateTime SyncRegisterDate
    {
        get { return registerDate; }
        set
        {
            if (this.registerDate == value)
                return;
            
            this.registerDate = value;
            RegisterDateCall();
        }
    }

    /// <summary> --同步带有Key字段的值到服务器Player对象上，需要处理</summary>
    [Net.Serialize.NonSerialized]
    [Newtonsoft_X.Json.JsonIgnore]
    public System.DateTime SyncIDRegisterDate
    {
        get { return registerDate; }
        set
        {
            if (this.registerDate == value)
                return;
            
            this.registerDate = value;
            SyncRegisterDateCall();
        }
    }

    /// <summary> --同步到数据库</summary>
    public void RegisterDateCall()
    {
        
        Net.Client.ClientBase.Instance.SendRT(Net.Share.NetCmd.EntityRpc, (ushort)TitansiegeHashProto.REGISTERDATE, registerDate);
    }

	/// <summary> --同步带有Key字段的值到服务器Player对象上，需要处理</summary>
    public void SyncRegisterDateCall()
    {
        
        Net.Client.ClientBase.Instance.SendRT(Net.Share.NetCmd.EntityRpc, (ushort)TitansiegeHashProto.REGISTERDATE, iD, registerDate);
    }

    [Net.Share.Rpc(hash = (ushort)TitansiegeHashProto.REGISTERDATE)]
    private void RegisterDateCall(System.DateTime value)//重写NetPlayer的OnStart方法来处理客户端自动同步到服务器数据库, 方法内部添加AddRpc(data(UsersData));收集Rpc
    {
        RegisterDate = value;
        OnRegisterDate?.Invoke();
    }

    [Net.Serialize.NonSerialized]
    [Newtonsoft_X.Json.JsonIgnore]
    public Action OnRegisterDate;

    private System.String email;
    /// <summary></summary>
    public System.String Email
    {
        get { return email; }
        set
        {
            if (this.email == value)
                return;
            if(value==null) value = string.Empty;
            this.email = value;
#if SERVER
            if (RowState == DataRowState.Deleted | RowState == DataRowState.Detached) return;
            columns.Add(4);
            if(RowState != DataRowState.Added & RowState != 0)//如果还没初始化或者创建新行,只能修改值不能更新状态
                RowState = DataRowState.Modified;
            TitansiegeDB.I.Update(this);
#elif CLIENT
            EmailCall();
#endif
        }
    }

    /// <summary> --同步到数据库</summary>
    [Net.Serialize.NonSerialized]
    [Newtonsoft_X.Json.JsonIgnore]
    public System.String SyncEmail
    {
        get { return email; }
        set
        {
            if (this.email == value)
                return;
            if(value==null) value = string.Empty;
            this.email = value;
            EmailCall();
        }
    }

    /// <summary> --同步带有Key字段的值到服务器Player对象上，需要处理</summary>
    [Net.Serialize.NonSerialized]
    [Newtonsoft_X.Json.JsonIgnore]
    public System.String SyncIDEmail
    {
        get { return email; }
        set
        {
            if (this.email == value)
                return;
            if(value==null) value = string.Empty;
            this.email = value;
            SyncEmailCall();
        }
    }

    /// <summary> --同步到数据库</summary>
    public void EmailCall()
    {
        
        Net.Client.ClientBase.Instance.SendRT(Net.Share.NetCmd.EntityRpc, (ushort)TitansiegeHashProto.EMAIL, email);
    }

	/// <summary> --同步带有Key字段的值到服务器Player对象上，需要处理</summary>
    public void SyncEmailCall()
    {
        
        Net.Client.ClientBase.Instance.SendRT(Net.Share.NetCmd.EntityRpc, (ushort)TitansiegeHashProto.EMAIL, iD, email);
    }

    [Net.Share.Rpc(hash = (ushort)TitansiegeHashProto.EMAIL)]
    private void EmailCall(System.String value)//重写NetPlayer的OnStart方法来处理客户端自动同步到服务器数据库, 方法内部添加AddRpc(data(UsersData));收集Rpc
    {
        Email = value;
        OnEmail?.Invoke();
    }

    [Net.Serialize.NonSerialized]
    [Newtonsoft_X.Json.JsonIgnore]
    public Action OnEmail;


    public UsersData() { }

#if SERVER
    public UsersData(params object[] parms)
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
            this[i] = parms[i];
            columns.Add(i);
        }
        RowState = DataRowState.Added;
        TitansiegeDB.I.Update(this);
    }
    public string GetCellName(int index)
    {
        switch (index)
        {

            case 0:
                return "ID";

            case 1:
                return "Username";

            case 2:
                return "Password";

            case 3:
                return "RegisterDate";

            case 4:
                return "Email";

        }
        throw new Exception("错误");
    }
    public object this[int index]
    {
        get
        {
            switch (index)
            {

                case 0:
                    return this.ID;

                case 1:
                    return this.Username;

                case 2:
                    return this.Password;

                case 3:
                    return this.RegisterDate;

                case 4:
                    return this.Email;

            }
            throw new Exception("错误");
        }
        set
        {
            switch (index)
            {

                case 0:
                    this.ID = (System.Int64)value;
                    break;

                case 1:
                    this.Username = (System.String)value;
                    break;

                case 2:
                    this.Password = (System.String)value;
                    break;

                case 3:
                    this.RegisterDate = (System.DateTime)value;
                    break;

                case 4:
                    this.Email = (System.String)value;
                    break;

            }
        }
    }

    public void Delete()
    {
        RowState = DataRowState.Detached;
        TitansiegeDB.I.Update(this);
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
        var data = TitansiegeDB.ExecuteQuery<UsersData>(cmdText);
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
    public static async Task<UsersData> QueryAsync(string filterExpression)
    {
        var cmdText = $"select * from users where {filterExpression}; ";
        var data = await TitansiegeDB.ExecuteQueryAsync<UsersData>(cmdText);
        return data;
    }
    public static UsersData[] QueryList(string filterExpression)
    {
        var cmdText = $"select * from users where {filterExpression}; ";
        var datas = TitansiegeDB.ExecuteQueryList<UsersData>(cmdText);
        return datas;
    }
    public static async Task<UsersData[]> QueryListAsync(string filterExpression)
    {
        var cmdText = $"select * from users where {filterExpression}; ";
        var datas = await TitansiegeDB.ExecuteQueryListAsync<UsersData>(cmdText);
        return datas;
    }
    public void Update()
    {
        if (RowState == DataRowState.Deleted | RowState == DataRowState.Detached | RowState == DataRowState.Added | RowState == 0) return;

        for (int i = 0; i < 5; i++)
        {
            columns.Add(i);
        }
        RowState = DataRowState.Modified;
        TitansiegeDB.I.Update(this);

    }
#endif

    public void Init(DataRow row)
    {
        RowState = DataRowState.Unchanged;

        if (row[0] is System.Int64 ID)
            this.ID = ID;

        if (row[1] is System.String Username)
            this.Username = Username;

        if (row[2] is System.String Password)
            this.Password = Password;

        if (row[3] is System.DateTime RegisterDate)
            this.RegisterDate = RegisterDate;

        if (row[4] is System.String Email)
            this.Email = Email;

    }

    public void AddedSql(StringBuilder sb, List<IDbDataParameter> parms, ref int parmsLen, ref int count)
    {
#if SERVER
        string cmdText = "REPLACE INTO users SET ";
        foreach (var column in columns)
        {
            var name = GetCellName(column);
            var value = this[column];
            if (value is string | value is DateTime)
                cmdText += $"`{name}`='{value}',";
            else if (value is byte[] buffer)
            {
                cmdText += $"`{name}`=@buffer{count},";
                parms.Add(new MySqlParameter($"@buffer{count}", buffer));
                parmsLen += buffer.Length;
                count++;
            }
            else cmdText += $"`{name}`={value},";
            columns.Remove(column);
        }
        cmdText = cmdText.TrimEnd(',');
        cmdText += "; ";
        sb.Append(cmdText);
        count++;
        RowState = DataRowState.Unchanged;
#endif
    }

    public void ModifiedSql(StringBuilder sb, List<IDbDataParameter> parms, ref int parmsLen, ref int count)
    {
#if SERVER
        if (RowState == DataRowState.Detached | RowState == DataRowState.Deleted | RowState == DataRowState.Added | RowState == 0)
            return;
        string cmdText = $"UPDATE users SET ";
        foreach (var column in columns)
        {
            var name = GetCellName(column);
            var value = this[column];
            if (value is string | value is DateTime)
                cmdText += $"`{name}`='{value}',";
            else if (value is byte[] buffer)
            {
                cmdText += $"`{name}`=@buffer{count},";
                parms.Add(new MySqlParameter($"@buffer{count}", buffer));
                parmsLen += buffer.Length;
                count++;
            }
            else cmdText += $"`{name}`={value},";
            columns.Remove(column);
        }
        cmdText = cmdText.TrimEnd(',');
        cmdText += $" WHERE `iD`={iD}; ";
        sb.Append(cmdText);
        count++;
        RowState = DataRowState.Unchanged;
#endif
    }

    public void DeletedSql(StringBuilder sb)
    {
#if SERVER
        if (RowState == DataRowState.Deleted)
            return;
        string cmdText = $"DELETE FROM users WHERE `iD` = {iD}; ";
        sb.Append(cmdText);
        RowState = DataRowState.Deleted;
#endif
    }
}
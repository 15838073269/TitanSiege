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
public partial class CharactersData : IDataRow
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


    private System.String name;
    /// <summary></summary>
    public System.String Name
    {
        get { return name; }
        set
        {
            if (this.name == value)
                return;
            if(value==null) value = string.Empty;
            this.name = value;
#if SERVER
            if (RowState == DataRowState.Deleted | RowState == DataRowState.Detached) return;
            columns.Add(1);
            if(RowState != DataRowState.Added & RowState != 0)//如果还没初始化或者创建新行,只能修改值不能更新状态
                RowState = DataRowState.Modified;
            TitansiegeDB.I.Update(this);
#elif CLIENT
            NameCall();
#endif
        }
    }

    /// <summary> --同步到数据库</summary>
    [Net.Serialize.NonSerialized]
    [Newtonsoft_X.Json.JsonIgnore]
    public System.String SyncName
    {
        get { return name; }
        set
        {
            if (this.name == value)
                return;
            if(value==null) value = string.Empty;
            this.name = value;
            NameCall();
        }
    }

    /// <summary> --同步带有Key字段的值到服务器Player对象上，需要处理</summary>
    [Net.Serialize.NonSerialized]
    [Newtonsoft_X.Json.JsonIgnore]
    public System.String SyncIDName
    {
        get { return name; }
        set
        {
            if (this.name == value)
                return;
            if(value==null) value = string.Empty;
            this.name = value;
            SyncNameCall();
        }
    }

    /// <summary> --同步到数据库</summary>
    public void NameCall()
    {
        
        Net.Client.ClientBase.Instance.SendRT(Net.Share.NetCmd.EntityRpc, (ushort)TitansiegeHashProto.NAME, name);
    }

	/// <summary> --同步带有Key字段的值到服务器Player对象上，需要处理</summary>
    public void SyncNameCall()
    {
        
        Net.Client.ClientBase.Instance.SendRT(Net.Share.NetCmd.EntityRpc, (ushort)TitansiegeHashProto.NAME, iD, name);
    }

    [Net.Share.Rpc(hash = (ushort)TitansiegeHashProto.NAME)]
    private void NameCall(System.String value)//重写NetPlayer的OnStart方法来处理客户端自动同步到服务器数据库, 方法内部添加AddRpc(data(CharactersData));收集Rpc
    {
        Name = value;
        OnName?.Invoke();
    }

    [Net.Serialize.NonSerialized]
    [Newtonsoft_X.Json.JsonIgnore]
    public Action OnName;

    private System.SByte zhiye;
    /// <summary>职业</summary>
    public System.SByte Zhiye
    {
        get { return zhiye; }
        set
        {
            if (this.zhiye == value)
                return;
            
            this.zhiye = value;
#if SERVER
            if (RowState == DataRowState.Deleted | RowState == DataRowState.Detached) return;
            columns.Add(2);
            if(RowState != DataRowState.Added & RowState != 0)//如果还没初始化或者创建新行,只能修改值不能更新状态
                RowState = DataRowState.Modified;
            TitansiegeDB.I.Update(this);
#elif CLIENT
            ZhiyeCall();
#endif
        }
    }

    /// <summary>职业 --同步到数据库</summary>
    [Net.Serialize.NonSerialized]
    [Newtonsoft_X.Json.JsonIgnore]
    public System.SByte SyncZhiye
    {
        get { return zhiye; }
        set
        {
            if (this.zhiye == value)
                return;
            
            this.zhiye = value;
            ZhiyeCall();
        }
    }

    /// <summary>职业 --同步带有Key字段的值到服务器Player对象上，需要处理</summary>
    [Net.Serialize.NonSerialized]
    [Newtonsoft_X.Json.JsonIgnore]
    public System.SByte SyncIDZhiye
    {
        get { return zhiye; }
        set
        {
            if (this.zhiye == value)
                return;
            
            this.zhiye = value;
            SyncZhiyeCall();
        }
    }

    /// <summary>职业 --同步到数据库</summary>
    public void ZhiyeCall()
    {
        
        Net.Client.ClientBase.Instance.SendRT(Net.Share.NetCmd.EntityRpc, (ushort)TitansiegeHashProto.ZHIYE, zhiye);
    }

	/// <summary>职业 --同步带有Key字段的值到服务器Player对象上，需要处理</summary>
    public void SyncZhiyeCall()
    {
        
        Net.Client.ClientBase.Instance.SendRT(Net.Share.NetCmd.EntityRpc, (ushort)TitansiegeHashProto.ZHIYE, iD, zhiye);
    }

    [Net.Share.Rpc(hash = (ushort)TitansiegeHashProto.ZHIYE)]
    private void ZhiyeCall(System.SByte value)//重写NetPlayer的OnStart方法来处理客户端自动同步到服务器数据库, 方法内部添加AddRpc(data(CharactersData));收集Rpc
    {
        Zhiye = value;
        OnZhiye?.Invoke();
    }

    [Net.Serialize.NonSerialized]
    [Newtonsoft_X.Json.JsonIgnore]
    public Action OnZhiye;

    private System.SByte level;
    /// <summary>等级</summary>
    public System.SByte Level
    {
        get { return level; }
        set
        {
            if (this.level == value)
                return;
            
            this.level = value;
#if SERVER
            if (RowState == DataRowState.Deleted | RowState == DataRowState.Detached) return;
            columns.Add(3);
            if(RowState != DataRowState.Added & RowState != 0)//如果还没初始化或者创建新行,只能修改值不能更新状态
                RowState = DataRowState.Modified;
            TitansiegeDB.I.Update(this);
#elif CLIENT
            LevelCall();
#endif
        }
    }

    /// <summary>等级 --同步到数据库</summary>
    [Net.Serialize.NonSerialized]
    [Newtonsoft_X.Json.JsonIgnore]
    public System.SByte SyncLevel
    {
        get { return level; }
        set
        {
            if (this.level == value)
                return;
            
            this.level = value;
            LevelCall();
        }
    }

    /// <summary>等级 --同步带有Key字段的值到服务器Player对象上，需要处理</summary>
    [Net.Serialize.NonSerialized]
    [Newtonsoft_X.Json.JsonIgnore]
    public System.SByte SyncIDLevel
    {
        get { return level; }
        set
        {
            if (this.level == value)
                return;
            
            this.level = value;
            SyncLevelCall();
        }
    }

    /// <summary>等级 --同步到数据库</summary>
    public void LevelCall()
    {
        
        Net.Client.ClientBase.Instance.SendRT(Net.Share.NetCmd.EntityRpc, (ushort)TitansiegeHashProto.LEVEL, level);
    }

	/// <summary>等级 --同步带有Key字段的值到服务器Player对象上，需要处理</summary>
    public void SyncLevelCall()
    {
        
        Net.Client.ClientBase.Instance.SendRT(Net.Share.NetCmd.EntityRpc, (ushort)TitansiegeHashProto.LEVEL, iD, level);
    }

    [Net.Share.Rpc(hash = (ushort)TitansiegeHashProto.LEVEL)]
    private void LevelCall(System.SByte value)//重写NetPlayer的OnStart方法来处理客户端自动同步到服务器数据库, 方法内部添加AddRpc(data(CharactersData));收集Rpc
    {
        Level = value;
        OnLevel?.Invoke();
    }

    [Net.Serialize.NonSerialized]
    [Newtonsoft_X.Json.JsonIgnore]
    public Action OnLevel;

    private System.Int32 exp;
    /// <summary>经验</summary>
    public System.Int32 Exp
    {
        get { return exp; }
        set
        {
            if (this.exp == value)
                return;
            
            this.exp = value;
#if SERVER
            if (RowState == DataRowState.Deleted | RowState == DataRowState.Detached) return;
            columns.Add(4);
            if(RowState != DataRowState.Added & RowState != 0)//如果还没初始化或者创建新行,只能修改值不能更新状态
                RowState = DataRowState.Modified;
            TitansiegeDB.I.Update(this);
#elif CLIENT
            ExpCall();
#endif
        }
    }

    /// <summary>经验 --同步到数据库</summary>
    [Net.Serialize.NonSerialized]
    [Newtonsoft_X.Json.JsonIgnore]
    public System.Int32 SyncExp
    {
        get { return exp; }
        set
        {
            if (this.exp == value)
                return;
            
            this.exp = value;
            ExpCall();
        }
    }

    /// <summary>经验 --同步带有Key字段的值到服务器Player对象上，需要处理</summary>
    [Net.Serialize.NonSerialized]
    [Newtonsoft_X.Json.JsonIgnore]
    public System.Int32 SyncIDExp
    {
        get { return exp; }
        set
        {
            if (this.exp == value)
                return;
            
            this.exp = value;
            SyncExpCall();
        }
    }

    /// <summary>经验 --同步到数据库</summary>
    public void ExpCall()
    {
        
        Net.Client.ClientBase.Instance.SendRT(Net.Share.NetCmd.EntityRpc, (ushort)TitansiegeHashProto.EXP, exp);
    }

	/// <summary>经验 --同步带有Key字段的值到服务器Player对象上，需要处理</summary>
    public void SyncExpCall()
    {
        
        Net.Client.ClientBase.Instance.SendRT(Net.Share.NetCmd.EntityRpc, (ushort)TitansiegeHashProto.EXP, iD, exp);
    }

    [Net.Share.Rpc(hash = (ushort)TitansiegeHashProto.EXP)]
    private void ExpCall(System.Int32 value)//重写NetPlayer的OnStart方法来处理客户端自动同步到服务器数据库, 方法内部添加AddRpc(data(CharactersData));收集Rpc
    {
        Exp = value;
        OnExp?.Invoke();
    }

    [Net.Serialize.NonSerialized]
    [Newtonsoft_X.Json.JsonIgnore]
    public Action OnExp;

    private System.Int32 shengming;
    /// <summary>生命</summary>
    public System.Int32 Shengming
    {
        get { return shengming; }
        set
        {
            if (this.shengming == value)
                return;
            
            this.shengming = value;
#if SERVER
            if (RowState == DataRowState.Deleted | RowState == DataRowState.Detached) return;
            columns.Add(5);
            if(RowState != DataRowState.Added & RowState != 0)//如果还没初始化或者创建新行,只能修改值不能更新状态
                RowState = DataRowState.Modified;
            TitansiegeDB.I.Update(this);
#elif CLIENT
            ShengmingCall();
#endif
        }
    }

    /// <summary>生命 --同步到数据库</summary>
    [Net.Serialize.NonSerialized]
    [Newtonsoft_X.Json.JsonIgnore]
    public System.Int32 SyncShengming
    {
        get { return shengming; }
        set
        {
            if (this.shengming == value)
                return;
            
            this.shengming = value;
            ShengmingCall();
        }
    }

    /// <summary>生命 --同步带有Key字段的值到服务器Player对象上，需要处理</summary>
    [Net.Serialize.NonSerialized]
    [Newtonsoft_X.Json.JsonIgnore]
    public System.Int32 SyncIDShengming
    {
        get { return shengming; }
        set
        {
            if (this.shengming == value)
                return;
            
            this.shengming = value;
            SyncShengmingCall();
        }
    }

    /// <summary>生命 --同步到数据库</summary>
    public void ShengmingCall()
    {
        
        Net.Client.ClientBase.Instance.SendRT(Net.Share.NetCmd.EntityRpc, (ushort)TitansiegeHashProto.SHENGMING, shengming);
    }

	/// <summary>生命 --同步带有Key字段的值到服务器Player对象上，需要处理</summary>
    public void SyncShengmingCall()
    {
        
        Net.Client.ClientBase.Instance.SendRT(Net.Share.NetCmd.EntityRpc, (ushort)TitansiegeHashProto.SHENGMING, iD, shengming);
    }

    [Net.Share.Rpc(hash = (ushort)TitansiegeHashProto.SHENGMING)]
    private void ShengmingCall(System.Int32 value)//重写NetPlayer的OnStart方法来处理客户端自动同步到服务器数据库, 方法内部添加AddRpc(data(CharactersData));收集Rpc
    {
        Shengming = value;
        OnShengming?.Invoke();
    }

    [Net.Serialize.NonSerialized]
    [Newtonsoft_X.Json.JsonIgnore]
    public Action OnShengming;

    private System.Int32 fali;
    /// <summary>法力</summary>
    public System.Int32 Fali
    {
        get { return fali; }
        set
        {
            if (this.fali == value)
                return;
            
            this.fali = value;
#if SERVER
            if (RowState == DataRowState.Deleted | RowState == DataRowState.Detached) return;
            columns.Add(6);
            if(RowState != DataRowState.Added & RowState != 0)//如果还没初始化或者创建新行,只能修改值不能更新状态
                RowState = DataRowState.Modified;
            TitansiegeDB.I.Update(this);
#elif CLIENT
            FaliCall();
#endif
        }
    }

    /// <summary>法力 --同步到数据库</summary>
    [Net.Serialize.NonSerialized]
    [Newtonsoft_X.Json.JsonIgnore]
    public System.Int32 SyncFali
    {
        get { return fali; }
        set
        {
            if (this.fali == value)
                return;
            
            this.fali = value;
            FaliCall();
        }
    }

    /// <summary>法力 --同步带有Key字段的值到服务器Player对象上，需要处理</summary>
    [Net.Serialize.NonSerialized]
    [Newtonsoft_X.Json.JsonIgnore]
    public System.Int32 SyncIDFali
    {
        get { return fali; }
        set
        {
            if (this.fali == value)
                return;
            
            this.fali = value;
            SyncFaliCall();
        }
    }

    /// <summary>法力 --同步到数据库</summary>
    public void FaliCall()
    {
        
        Net.Client.ClientBase.Instance.SendRT(Net.Share.NetCmd.EntityRpc, (ushort)TitansiegeHashProto.FALI, fali);
    }

	/// <summary>法力 --同步带有Key字段的值到服务器Player对象上，需要处理</summary>
    public void SyncFaliCall()
    {
        
        Net.Client.ClientBase.Instance.SendRT(Net.Share.NetCmd.EntityRpc, (ushort)TitansiegeHashProto.FALI, iD, fali);
    }

    [Net.Share.Rpc(hash = (ushort)TitansiegeHashProto.FALI)]
    private void FaliCall(System.Int32 value)//重写NetPlayer的OnStart方法来处理客户端自动同步到服务器数据库, 方法内部添加AddRpc(data(CharactersData));收集Rpc
    {
        Fali = value;
        OnFali?.Invoke();
    }

    [Net.Serialize.NonSerialized]
    [Newtonsoft_X.Json.JsonIgnore]
    public Action OnFali;

    private System.Int16 tizhi;
    /// <summary>体质</summary>
    public System.Int16 Tizhi
    {
        get { return tizhi; }
        set
        {
            if (this.tizhi == value)
                return;
            
            this.tizhi = value;
#if SERVER
            if (RowState == DataRowState.Deleted | RowState == DataRowState.Detached) return;
            columns.Add(7);
            if(RowState != DataRowState.Added & RowState != 0)//如果还没初始化或者创建新行,只能修改值不能更新状态
                RowState = DataRowState.Modified;
            TitansiegeDB.I.Update(this);
#elif CLIENT
            TizhiCall();
#endif
        }
    }

    /// <summary>体质 --同步到数据库</summary>
    [Net.Serialize.NonSerialized]
    [Newtonsoft_X.Json.JsonIgnore]
    public System.Int16 SyncTizhi
    {
        get { return tizhi; }
        set
        {
            if (this.tizhi == value)
                return;
            
            this.tizhi = value;
            TizhiCall();
        }
    }

    /// <summary>体质 --同步带有Key字段的值到服务器Player对象上，需要处理</summary>
    [Net.Serialize.NonSerialized]
    [Newtonsoft_X.Json.JsonIgnore]
    public System.Int16 SyncIDTizhi
    {
        get { return tizhi; }
        set
        {
            if (this.tizhi == value)
                return;
            
            this.tizhi = value;
            SyncTizhiCall();
        }
    }

    /// <summary>体质 --同步到数据库</summary>
    public void TizhiCall()
    {
        
        Net.Client.ClientBase.Instance.SendRT(Net.Share.NetCmd.EntityRpc, (ushort)TitansiegeHashProto.TIZHI, tizhi);
    }

	/// <summary>体质 --同步带有Key字段的值到服务器Player对象上，需要处理</summary>
    public void SyncTizhiCall()
    {
        
        Net.Client.ClientBase.Instance.SendRT(Net.Share.NetCmd.EntityRpc, (ushort)TitansiegeHashProto.TIZHI, iD, tizhi);
    }

    [Net.Share.Rpc(hash = (ushort)TitansiegeHashProto.TIZHI)]
    private void TizhiCall(System.Int16 value)//重写NetPlayer的OnStart方法来处理客户端自动同步到服务器数据库, 方法内部添加AddRpc(data(CharactersData));收集Rpc
    {
        Tizhi = value;
        OnTizhi?.Invoke();
    }

    [Net.Serialize.NonSerialized]
    [Newtonsoft_X.Json.JsonIgnore]
    public Action OnTizhi;

    private System.Int16 liliang;
    /// <summary>力量</summary>
    public System.Int16 Liliang
    {
        get { return liliang; }
        set
        {
            if (this.liliang == value)
                return;
            
            this.liliang = value;
#if SERVER
            if (RowState == DataRowState.Deleted | RowState == DataRowState.Detached) return;
            columns.Add(8);
            if(RowState != DataRowState.Added & RowState != 0)//如果还没初始化或者创建新行,只能修改值不能更新状态
                RowState = DataRowState.Modified;
            TitansiegeDB.I.Update(this);
#elif CLIENT
            LiliangCall();
#endif
        }
    }

    /// <summary>力量 --同步到数据库</summary>
    [Net.Serialize.NonSerialized]
    [Newtonsoft_X.Json.JsonIgnore]
    public System.Int16 SyncLiliang
    {
        get { return liliang; }
        set
        {
            if (this.liliang == value)
                return;
            
            this.liliang = value;
            LiliangCall();
        }
    }

    /// <summary>力量 --同步带有Key字段的值到服务器Player对象上，需要处理</summary>
    [Net.Serialize.NonSerialized]
    [Newtonsoft_X.Json.JsonIgnore]
    public System.Int16 SyncIDLiliang
    {
        get { return liliang; }
        set
        {
            if (this.liliang == value)
                return;
            
            this.liliang = value;
            SyncLiliangCall();
        }
    }

    /// <summary>力量 --同步到数据库</summary>
    public void LiliangCall()
    {
        
        Net.Client.ClientBase.Instance.SendRT(Net.Share.NetCmd.EntityRpc, (ushort)TitansiegeHashProto.LILIANG, liliang);
    }

	/// <summary>力量 --同步带有Key字段的值到服务器Player对象上，需要处理</summary>
    public void SyncLiliangCall()
    {
        
        Net.Client.ClientBase.Instance.SendRT(Net.Share.NetCmd.EntityRpc, (ushort)TitansiegeHashProto.LILIANG, iD, liliang);
    }

    [Net.Share.Rpc(hash = (ushort)TitansiegeHashProto.LILIANG)]
    private void LiliangCall(System.Int16 value)//重写NetPlayer的OnStart方法来处理客户端自动同步到服务器数据库, 方法内部添加AddRpc(data(CharactersData));收集Rpc
    {
        Liliang = value;
        OnLiliang?.Invoke();
    }

    [Net.Serialize.NonSerialized]
    [Newtonsoft_X.Json.JsonIgnore]
    public Action OnLiliang;

    private System.Int16 minjie;
    /// <summary>敏捷</summary>
    public System.Int16 Minjie
    {
        get { return minjie; }
        set
        {
            if (this.minjie == value)
                return;
            
            this.minjie = value;
#if SERVER
            if (RowState == DataRowState.Deleted | RowState == DataRowState.Detached) return;
            columns.Add(9);
            if(RowState != DataRowState.Added & RowState != 0)//如果还没初始化或者创建新行,只能修改值不能更新状态
                RowState = DataRowState.Modified;
            TitansiegeDB.I.Update(this);
#elif CLIENT
            MinjieCall();
#endif
        }
    }

    /// <summary>敏捷 --同步到数据库</summary>
    [Net.Serialize.NonSerialized]
    [Newtonsoft_X.Json.JsonIgnore]
    public System.Int16 SyncMinjie
    {
        get { return minjie; }
        set
        {
            if (this.minjie == value)
                return;
            
            this.minjie = value;
            MinjieCall();
        }
    }

    /// <summary>敏捷 --同步带有Key字段的值到服务器Player对象上，需要处理</summary>
    [Net.Serialize.NonSerialized]
    [Newtonsoft_X.Json.JsonIgnore]
    public System.Int16 SyncIDMinjie
    {
        get { return minjie; }
        set
        {
            if (this.minjie == value)
                return;
            
            this.minjie = value;
            SyncMinjieCall();
        }
    }

    /// <summary>敏捷 --同步到数据库</summary>
    public void MinjieCall()
    {
        
        Net.Client.ClientBase.Instance.SendRT(Net.Share.NetCmd.EntityRpc, (ushort)TitansiegeHashProto.MINJIE, minjie);
    }

	/// <summary>敏捷 --同步带有Key字段的值到服务器Player对象上，需要处理</summary>
    public void SyncMinjieCall()
    {
        
        Net.Client.ClientBase.Instance.SendRT(Net.Share.NetCmd.EntityRpc, (ushort)TitansiegeHashProto.MINJIE, iD, minjie);
    }

    [Net.Share.Rpc(hash = (ushort)TitansiegeHashProto.MINJIE)]
    private void MinjieCall(System.Int16 value)//重写NetPlayer的OnStart方法来处理客户端自动同步到服务器数据库, 方法内部添加AddRpc(data(CharactersData));收集Rpc
    {
        Minjie = value;
        OnMinjie?.Invoke();
    }

    [Net.Serialize.NonSerialized]
    [Newtonsoft_X.Json.JsonIgnore]
    public Action OnMinjie;

    private System.Int16 moli;
    /// <summary>魔力</summary>
    public System.Int16 Moli
    {
        get { return moli; }
        set
        {
            if (this.moli == value)
                return;
            
            this.moli = value;
#if SERVER
            if (RowState == DataRowState.Deleted | RowState == DataRowState.Detached) return;
            columns.Add(10);
            if(RowState != DataRowState.Added & RowState != 0)//如果还没初始化或者创建新行,只能修改值不能更新状态
                RowState = DataRowState.Modified;
            TitansiegeDB.I.Update(this);
#elif CLIENT
            MoliCall();
#endif
        }
    }

    /// <summary>魔力 --同步到数据库</summary>
    [Net.Serialize.NonSerialized]
    [Newtonsoft_X.Json.JsonIgnore]
    public System.Int16 SyncMoli
    {
        get { return moli; }
        set
        {
            if (this.moli == value)
                return;
            
            this.moli = value;
            MoliCall();
        }
    }

    /// <summary>魔力 --同步带有Key字段的值到服务器Player对象上，需要处理</summary>
    [Net.Serialize.NonSerialized]
    [Newtonsoft_X.Json.JsonIgnore]
    public System.Int16 SyncIDMoli
    {
        get { return moli; }
        set
        {
            if (this.moli == value)
                return;
            
            this.moli = value;
            SyncMoliCall();
        }
    }

    /// <summary>魔力 --同步到数据库</summary>
    public void MoliCall()
    {
        
        Net.Client.ClientBase.Instance.SendRT(Net.Share.NetCmd.EntityRpc, (ushort)TitansiegeHashProto.MOLI, moli);
    }

	/// <summary>魔力 --同步带有Key字段的值到服务器Player对象上，需要处理</summary>
    public void SyncMoliCall()
    {
        
        Net.Client.ClientBase.Instance.SendRT(Net.Share.NetCmd.EntityRpc, (ushort)TitansiegeHashProto.MOLI, iD, moli);
    }

    [Net.Share.Rpc(hash = (ushort)TitansiegeHashProto.MOLI)]
    private void MoliCall(System.Int16 value)//重写NetPlayer的OnStart方法来处理客户端自动同步到服务器数据库, 方法内部添加AddRpc(data(CharactersData));收集Rpc
    {
        Moli = value;
        OnMoli?.Invoke();
    }

    [Net.Serialize.NonSerialized]
    [Newtonsoft_X.Json.JsonIgnore]
    public Action OnMoli;

    private System.Int16 meili;
    /// <summary>魅力</summary>
    public System.Int16 Meili
    {
        get { return meili; }
        set
        {
            if (this.meili == value)
                return;
            
            this.meili = value;
#if SERVER
            if (RowState == DataRowState.Deleted | RowState == DataRowState.Detached) return;
            columns.Add(11);
            if(RowState != DataRowState.Added & RowState != 0)//如果还没初始化或者创建新行,只能修改值不能更新状态
                RowState = DataRowState.Modified;
            TitansiegeDB.I.Update(this);
#elif CLIENT
            MeiliCall();
#endif
        }
    }

    /// <summary>魅力 --同步到数据库</summary>
    [Net.Serialize.NonSerialized]
    [Newtonsoft_X.Json.JsonIgnore]
    public System.Int16 SyncMeili
    {
        get { return meili; }
        set
        {
            if (this.meili == value)
                return;
            
            this.meili = value;
            MeiliCall();
        }
    }

    /// <summary>魅力 --同步带有Key字段的值到服务器Player对象上，需要处理</summary>
    [Net.Serialize.NonSerialized]
    [Newtonsoft_X.Json.JsonIgnore]
    public System.Int16 SyncIDMeili
    {
        get { return meili; }
        set
        {
            if (this.meili == value)
                return;
            
            this.meili = value;
            SyncMeiliCall();
        }
    }

    /// <summary>魅力 --同步到数据库</summary>
    public void MeiliCall()
    {
        
        Net.Client.ClientBase.Instance.SendRT(Net.Share.NetCmd.EntityRpc, (ushort)TitansiegeHashProto.MEILI, meili);
    }

	/// <summary>魅力 --同步带有Key字段的值到服务器Player对象上，需要处理</summary>
    public void SyncMeiliCall()
    {
        
        Net.Client.ClientBase.Instance.SendRT(Net.Share.NetCmd.EntityRpc, (ushort)TitansiegeHashProto.MEILI, iD, meili);
    }

    [Net.Share.Rpc(hash = (ushort)TitansiegeHashProto.MEILI)]
    private void MeiliCall(System.Int16 value)//重写NetPlayer的OnStart方法来处理客户端自动同步到服务器数据库, 方法内部添加AddRpc(data(CharactersData));收集Rpc
    {
        Meili = value;
        OnMeili?.Invoke();
    }

    [Net.Serialize.NonSerialized]
    [Newtonsoft_X.Json.JsonIgnore]
    public Action OnMeili;

    private System.Int16 xingyun;
    /// <summary>幸运</summary>
    public System.Int16 Xingyun
    {
        get { return xingyun; }
        set
        {
            if (this.xingyun == value)
                return;
            
            this.xingyun = value;
#if SERVER
            if (RowState == DataRowState.Deleted | RowState == DataRowState.Detached) return;
            columns.Add(12);
            if(RowState != DataRowState.Added & RowState != 0)//如果还没初始化或者创建新行,只能修改值不能更新状态
                RowState = DataRowState.Modified;
            TitansiegeDB.I.Update(this);
#elif CLIENT
            XingyunCall();
#endif
        }
    }

    /// <summary>幸运 --同步到数据库</summary>
    [Net.Serialize.NonSerialized]
    [Newtonsoft_X.Json.JsonIgnore]
    public System.Int16 SyncXingyun
    {
        get { return xingyun; }
        set
        {
            if (this.xingyun == value)
                return;
            
            this.xingyun = value;
            XingyunCall();
        }
    }

    /// <summary>幸运 --同步带有Key字段的值到服务器Player对象上，需要处理</summary>
    [Net.Serialize.NonSerialized]
    [Newtonsoft_X.Json.JsonIgnore]
    public System.Int16 SyncIDXingyun
    {
        get { return xingyun; }
        set
        {
            if (this.xingyun == value)
                return;
            
            this.xingyun = value;
            SyncXingyunCall();
        }
    }

    /// <summary>幸运 --同步到数据库</summary>
    public void XingyunCall()
    {
        
        Net.Client.ClientBase.Instance.SendRT(Net.Share.NetCmd.EntityRpc, (ushort)TitansiegeHashProto.XINGYUN, xingyun);
    }

	/// <summary>幸运 --同步带有Key字段的值到服务器Player对象上，需要处理</summary>
    public void SyncXingyunCall()
    {
        
        Net.Client.ClientBase.Instance.SendRT(Net.Share.NetCmd.EntityRpc, (ushort)TitansiegeHashProto.XINGYUN, iD, xingyun);
    }

    [Net.Share.Rpc(hash = (ushort)TitansiegeHashProto.XINGYUN)]
    private void XingyunCall(System.Int16 value)//重写NetPlayer的OnStart方法来处理客户端自动同步到服务器数据库, 方法内部添加AddRpc(data(CharactersData));收集Rpc
    {
        Xingyun = value;
        OnXingyun?.Invoke();
    }

    [Net.Serialize.NonSerialized]
    [Newtonsoft_X.Json.JsonIgnore]
    public Action OnXingyun;

    private System.Int16 lianjin;
    /// <summary>炼金</summary>
    public System.Int16 Lianjin
    {
        get { return lianjin; }
        set
        {
            if (this.lianjin == value)
                return;
            
            this.lianjin = value;
#if SERVER
            if (RowState == DataRowState.Deleted | RowState == DataRowState.Detached) return;
            columns.Add(13);
            if(RowState != DataRowState.Added & RowState != 0)//如果还没初始化或者创建新行,只能修改值不能更新状态
                RowState = DataRowState.Modified;
            TitansiegeDB.I.Update(this);
#elif CLIENT
            LianjinCall();
#endif
        }
    }

    /// <summary>炼金 --同步到数据库</summary>
    [Net.Serialize.NonSerialized]
    [Newtonsoft_X.Json.JsonIgnore]
    public System.Int16 SyncLianjin
    {
        get { return lianjin; }
        set
        {
            if (this.lianjin == value)
                return;
            
            this.lianjin = value;
            LianjinCall();
        }
    }

    /// <summary>炼金 --同步带有Key字段的值到服务器Player对象上，需要处理</summary>
    [Net.Serialize.NonSerialized]
    [Newtonsoft_X.Json.JsonIgnore]
    public System.Int16 SyncIDLianjin
    {
        get { return lianjin; }
        set
        {
            if (this.lianjin == value)
                return;
            
            this.lianjin = value;
            SyncLianjinCall();
        }
    }

    /// <summary>炼金 --同步到数据库</summary>
    public void LianjinCall()
    {
        
        Net.Client.ClientBase.Instance.SendRT(Net.Share.NetCmd.EntityRpc, (ushort)TitansiegeHashProto.LIANJIN, lianjin);
    }

	/// <summary>炼金 --同步带有Key字段的值到服务器Player对象上，需要处理</summary>
    public void SyncLianjinCall()
    {
        
        Net.Client.ClientBase.Instance.SendRT(Net.Share.NetCmd.EntityRpc, (ushort)TitansiegeHashProto.LIANJIN, iD, lianjin);
    }

    [Net.Share.Rpc(hash = (ushort)TitansiegeHashProto.LIANJIN)]
    private void LianjinCall(System.Int16 value)//重写NetPlayer的OnStart方法来处理客户端自动同步到服务器数据库, 方法内部添加AddRpc(data(CharactersData));收集Rpc
    {
        Lianjin = value;
        OnLianjin?.Invoke();
    }

    [Net.Serialize.NonSerialized]
    [Newtonsoft_X.Json.JsonIgnore]
    public Action OnLianjin;

    private System.Int16 duanzao;
    /// <summary>锻造</summary>
    public System.Int16 Duanzao
    {
        get { return duanzao; }
        set
        {
            if (this.duanzao == value)
                return;
            
            this.duanzao = value;
#if SERVER
            if (RowState == DataRowState.Deleted | RowState == DataRowState.Detached) return;
            columns.Add(14);
            if(RowState != DataRowState.Added & RowState != 0)//如果还没初始化或者创建新行,只能修改值不能更新状态
                RowState = DataRowState.Modified;
            TitansiegeDB.I.Update(this);
#elif CLIENT
            DuanzaoCall();
#endif
        }
    }

    /// <summary>锻造 --同步到数据库</summary>
    [Net.Serialize.NonSerialized]
    [Newtonsoft_X.Json.JsonIgnore]
    public System.Int16 SyncDuanzao
    {
        get { return duanzao; }
        set
        {
            if (this.duanzao == value)
                return;
            
            this.duanzao = value;
            DuanzaoCall();
        }
    }

    /// <summary>锻造 --同步带有Key字段的值到服务器Player对象上，需要处理</summary>
    [Net.Serialize.NonSerialized]
    [Newtonsoft_X.Json.JsonIgnore]
    public System.Int16 SyncIDDuanzao
    {
        get { return duanzao; }
        set
        {
            if (this.duanzao == value)
                return;
            
            this.duanzao = value;
            SyncDuanzaoCall();
        }
    }

    /// <summary>锻造 --同步到数据库</summary>
    public void DuanzaoCall()
    {
        
        Net.Client.ClientBase.Instance.SendRT(Net.Share.NetCmd.EntityRpc, (ushort)TitansiegeHashProto.DUANZAO, duanzao);
    }

	/// <summary>锻造 --同步带有Key字段的值到服务器Player对象上，需要处理</summary>
    public void SyncDuanzaoCall()
    {
        
        Net.Client.ClientBase.Instance.SendRT(Net.Share.NetCmd.EntityRpc, (ushort)TitansiegeHashProto.DUANZAO, iD, duanzao);
    }

    [Net.Share.Rpc(hash = (ushort)TitansiegeHashProto.DUANZAO)]
    private void DuanzaoCall(System.Int16 value)//重写NetPlayer的OnStart方法来处理客户端自动同步到服务器数据库, 方法内部添加AddRpc(data(CharactersData));收集Rpc
    {
        Duanzao = value;
        OnDuanzao?.Invoke();
    }

    [Net.Serialize.NonSerialized]
    [Newtonsoft_X.Json.JsonIgnore]
    public Action OnDuanzao;

    private System.Int32 jinbi;
    /// <summary>金币</summary>
    public System.Int32 Jinbi
    {
        get { return jinbi; }
        set
        {
            if (this.jinbi == value)
                return;
            
            this.jinbi = value;
#if SERVER
            if (RowState == DataRowState.Deleted | RowState == DataRowState.Detached) return;
            columns.Add(15);
            if(RowState != DataRowState.Added & RowState != 0)//如果还没初始化或者创建新行,只能修改值不能更新状态
                RowState = DataRowState.Modified;
            TitansiegeDB.I.Update(this);
#elif CLIENT
            JinbiCall();
#endif
        }
    }

    /// <summary>金币 --同步到数据库</summary>
    [Net.Serialize.NonSerialized]
    [Newtonsoft_X.Json.JsonIgnore]
    public System.Int32 SyncJinbi
    {
        get { return jinbi; }
        set
        {
            if (this.jinbi == value)
                return;
            
            this.jinbi = value;
            JinbiCall();
        }
    }

    /// <summary>金币 --同步带有Key字段的值到服务器Player对象上，需要处理</summary>
    [Net.Serialize.NonSerialized]
    [Newtonsoft_X.Json.JsonIgnore]
    public System.Int32 SyncIDJinbi
    {
        get { return jinbi; }
        set
        {
            if (this.jinbi == value)
                return;
            
            this.jinbi = value;
            SyncJinbiCall();
        }
    }

    /// <summary>金币 --同步到数据库</summary>
    public void JinbiCall()
    {
        
        Net.Client.ClientBase.Instance.SendRT(Net.Share.NetCmd.EntityRpc, (ushort)TitansiegeHashProto.JINBI, jinbi);
    }

	/// <summary>金币 --同步带有Key字段的值到服务器Player对象上，需要处理</summary>
    public void SyncJinbiCall()
    {
        
        Net.Client.ClientBase.Instance.SendRT(Net.Share.NetCmd.EntityRpc, (ushort)TitansiegeHashProto.JINBI, iD, jinbi);
    }

    [Net.Share.Rpc(hash = (ushort)TitansiegeHashProto.JINBI)]
    private void JinbiCall(System.Int32 value)//重写NetPlayer的OnStart方法来处理客户端自动同步到服务器数据库, 方法内部添加AddRpc(data(CharactersData));收集Rpc
    {
        Jinbi = value;
        OnJinbi?.Invoke();
    }

    [Net.Serialize.NonSerialized]
    [Newtonsoft_X.Json.JsonIgnore]
    public Action OnJinbi;

    private System.Int32 zuanshi;
    /// <summary>钻石</summary>
    public System.Int32 Zuanshi
    {
        get { return zuanshi; }
        set
        {
            if (this.zuanshi == value)
                return;
            
            this.zuanshi = value;
#if SERVER
            if (RowState == DataRowState.Deleted | RowState == DataRowState.Detached) return;
            columns.Add(16);
            if(RowState != DataRowState.Added & RowState != 0)//如果还没初始化或者创建新行,只能修改值不能更新状态
                RowState = DataRowState.Modified;
            TitansiegeDB.I.Update(this);
#elif CLIENT
            ZuanshiCall();
#endif
        }
    }

    /// <summary>钻石 --同步到数据库</summary>
    [Net.Serialize.NonSerialized]
    [Newtonsoft_X.Json.JsonIgnore]
    public System.Int32 SyncZuanshi
    {
        get { return zuanshi; }
        set
        {
            if (this.zuanshi == value)
                return;
            
            this.zuanshi = value;
            ZuanshiCall();
        }
    }

    /// <summary>钻石 --同步带有Key字段的值到服务器Player对象上，需要处理</summary>
    [Net.Serialize.NonSerialized]
    [Newtonsoft_X.Json.JsonIgnore]
    public System.Int32 SyncIDZuanshi
    {
        get { return zuanshi; }
        set
        {
            if (this.zuanshi == value)
                return;
            
            this.zuanshi = value;
            SyncZuanshiCall();
        }
    }

    /// <summary>钻石 --同步到数据库</summary>
    public void ZuanshiCall()
    {
        
        Net.Client.ClientBase.Instance.SendRT(Net.Share.NetCmd.EntityRpc, (ushort)TitansiegeHashProto.ZUANSHI, zuanshi);
    }

	/// <summary>钻石 --同步带有Key字段的值到服务器Player对象上，需要处理</summary>
    public void SyncZuanshiCall()
    {
        
        Net.Client.ClientBase.Instance.SendRT(Net.Share.NetCmd.EntityRpc, (ushort)TitansiegeHashProto.ZUANSHI, iD, zuanshi);
    }

    [Net.Share.Rpc(hash = (ushort)TitansiegeHashProto.ZUANSHI)]
    private void ZuanshiCall(System.Int32 value)//重写NetPlayer的OnStart方法来处理客户端自动同步到服务器数据库, 方法内部添加AddRpc(data(CharactersData));收集Rpc
    {
        Zuanshi = value;
        OnZuanshi?.Invoke();
    }

    [Net.Serialize.NonSerialized]
    [Newtonsoft_X.Json.JsonIgnore]
    public Action OnZuanshi;

    private System.String chenghao;
    /// <summary>称号</summary>
    public System.String Chenghao
    {
        get { return chenghao; }
        set
        {
            if (this.chenghao == value)
                return;
            if(value==null) value = string.Empty;
            this.chenghao = value;
#if SERVER
            if (RowState == DataRowState.Deleted | RowState == DataRowState.Detached) return;
            columns.Add(17);
            if(RowState != DataRowState.Added & RowState != 0)//如果还没初始化或者创建新行,只能修改值不能更新状态
                RowState = DataRowState.Modified;
            TitansiegeDB.I.Update(this);
#elif CLIENT
            ChenghaoCall();
#endif
        }
    }

    /// <summary>称号 --同步到数据库</summary>
    [Net.Serialize.NonSerialized]
    [Newtonsoft_X.Json.JsonIgnore]
    public System.String SyncChenghao
    {
        get { return chenghao; }
        set
        {
            if (this.chenghao == value)
                return;
            if(value==null) value = string.Empty;
            this.chenghao = value;
            ChenghaoCall();
        }
    }

    /// <summary>称号 --同步带有Key字段的值到服务器Player对象上，需要处理</summary>
    [Net.Serialize.NonSerialized]
    [Newtonsoft_X.Json.JsonIgnore]
    public System.String SyncIDChenghao
    {
        get { return chenghao; }
        set
        {
            if (this.chenghao == value)
                return;
            if(value==null) value = string.Empty;
            this.chenghao = value;
            SyncChenghaoCall();
        }
    }

    /// <summary>称号 --同步到数据库</summary>
    public void ChenghaoCall()
    {
        
        Net.Client.ClientBase.Instance.SendRT(Net.Share.NetCmd.EntityRpc, (ushort)TitansiegeHashProto.CHENGHAO, chenghao);
    }

	/// <summary>称号 --同步带有Key字段的值到服务器Player对象上，需要处理</summary>
    public void SyncChenghaoCall()
    {
        
        Net.Client.ClientBase.Instance.SendRT(Net.Share.NetCmd.EntityRpc, (ushort)TitansiegeHashProto.CHENGHAO, iD, chenghao);
    }

    [Net.Share.Rpc(hash = (ushort)TitansiegeHashProto.CHENGHAO)]
    private void ChenghaoCall(System.String value)//重写NetPlayer的OnStart方法来处理客户端自动同步到服务器数据库, 方法内部添加AddRpc(data(CharactersData));收集Rpc
    {
        Chenghao = value;
        OnChenghao?.Invoke();
    }

    [Net.Serialize.NonSerialized]
    [Newtonsoft_X.Json.JsonIgnore]
    public Action OnChenghao;

    private System.String friends;
    /// <summary>亲朋</summary>
    public System.String Friends
    {
        get { return friends; }
        set
        {
            if (this.friends == value)
                return;
            if(value==null) value = string.Empty;
            this.friends = value;
#if SERVER
            if (RowState == DataRowState.Deleted | RowState == DataRowState.Detached) return;
            columns.Add(18);
            if(RowState != DataRowState.Added & RowState != 0)//如果还没初始化或者创建新行,只能修改值不能更新状态
                RowState = DataRowState.Modified;
            TitansiegeDB.I.Update(this);
#elif CLIENT
            FriendsCall();
#endif
        }
    }

    /// <summary>亲朋 --同步到数据库</summary>
    [Net.Serialize.NonSerialized]
    [Newtonsoft_X.Json.JsonIgnore]
    public System.String SyncFriends
    {
        get { return friends; }
        set
        {
            if (this.friends == value)
                return;
            if(value==null) value = string.Empty;
            this.friends = value;
            FriendsCall();
        }
    }

    /// <summary>亲朋 --同步带有Key字段的值到服务器Player对象上，需要处理</summary>
    [Net.Serialize.NonSerialized]
    [Newtonsoft_X.Json.JsonIgnore]
    public System.String SyncIDFriends
    {
        get { return friends; }
        set
        {
            if (this.friends == value)
                return;
            if(value==null) value = string.Empty;
            this.friends = value;
            SyncFriendsCall();
        }
    }

    /// <summary>亲朋 --同步到数据库</summary>
    public void FriendsCall()
    {
        
        Net.Client.ClientBase.Instance.SendRT(Net.Share.NetCmd.EntityRpc, (ushort)TitansiegeHashProto.FRIENDS, friends);
    }

	/// <summary>亲朋 --同步带有Key字段的值到服务器Player对象上，需要处理</summary>
    public void SyncFriendsCall()
    {
        
        Net.Client.ClientBase.Instance.SendRT(Net.Share.NetCmd.EntityRpc, (ushort)TitansiegeHashProto.FRIENDS, iD, friends);
    }

    [Net.Share.Rpc(hash = (ushort)TitansiegeHashProto.FRIENDS)]
    private void FriendsCall(System.String value)//重写NetPlayer的OnStart方法来处理客户端自动同步到服务器数据库, 方法内部添加AddRpc(data(CharactersData));收集Rpc
    {
        Friends = value;
        OnFriends?.Invoke();
    }

    [Net.Serialize.NonSerialized]
    [Newtonsoft_X.Json.JsonIgnore]
    public Action OnFriends;

    private System.String skills;
    /// <summary>技能</summary>
    public System.String Skills
    {
        get { return skills; }
        set
        {
            if (this.skills == value)
                return;
            if(value==null) value = string.Empty;
            this.skills = value;
#if SERVER
            if (RowState == DataRowState.Deleted | RowState == DataRowState.Detached) return;
            columns.Add(19);
            if(RowState != DataRowState.Added & RowState != 0)//如果还没初始化或者创建新行,只能修改值不能更新状态
                RowState = DataRowState.Modified;
            TitansiegeDB.I.Update(this);
#elif CLIENT
            SkillsCall();
#endif
        }
    }

    /// <summary>技能 --同步到数据库</summary>
    [Net.Serialize.NonSerialized]
    [Newtonsoft_X.Json.JsonIgnore]
    public System.String SyncSkills
    {
        get { return skills; }
        set
        {
            if (this.skills == value)
                return;
            if(value==null) value = string.Empty;
            this.skills = value;
            SkillsCall();
        }
    }

    /// <summary>技能 --同步带有Key字段的值到服务器Player对象上，需要处理</summary>
    [Net.Serialize.NonSerialized]
    [Newtonsoft_X.Json.JsonIgnore]
    public System.String SyncIDSkills
    {
        get { return skills; }
        set
        {
            if (this.skills == value)
                return;
            if(value==null) value = string.Empty;
            this.skills = value;
            SyncSkillsCall();
        }
    }

    /// <summary>技能 --同步到数据库</summary>
    public void SkillsCall()
    {
        
        Net.Client.ClientBase.Instance.SendRT(Net.Share.NetCmd.EntityRpc, (ushort)TitansiegeHashProto.SKILLS, skills);
    }

	/// <summary>技能 --同步带有Key字段的值到服务器Player对象上，需要处理</summary>
    public void SyncSkillsCall()
    {
        
        Net.Client.ClientBase.Instance.SendRT(Net.Share.NetCmd.EntityRpc, (ushort)TitansiegeHashProto.SKILLS, iD, skills);
    }

    [Net.Share.Rpc(hash = (ushort)TitansiegeHashProto.SKILLS)]
    private void SkillsCall(System.String value)//重写NetPlayer的OnStart方法来处理客户端自动同步到服务器数据库, 方法内部添加AddRpc(data(CharactersData));收集Rpc
    {
        Skills = value;
        OnSkills?.Invoke();
    }

    [Net.Serialize.NonSerialized]
    [Newtonsoft_X.Json.JsonIgnore]
    public Action OnSkills;

    private System.String prefabpath;
    /// <summary>预制体路径</summary>
    public System.String Prefabpath
    {
        get { return prefabpath; }
        set
        {
            if (this.prefabpath == value)
                return;
            if(value==null) value = string.Empty;
            this.prefabpath = value;
#if SERVER
            if (RowState == DataRowState.Deleted | RowState == DataRowState.Detached) return;
            columns.Add(20);
            if(RowState != DataRowState.Added & RowState != 0)//如果还没初始化或者创建新行,只能修改值不能更新状态
                RowState = DataRowState.Modified;
            TitansiegeDB.I.Update(this);
#elif CLIENT
            PrefabpathCall();
#endif
        }
    }

    /// <summary>预制体路径 --同步到数据库</summary>
    [Net.Serialize.NonSerialized]
    [Newtonsoft_X.Json.JsonIgnore]
    public System.String SyncPrefabpath
    {
        get { return prefabpath; }
        set
        {
            if (this.prefabpath == value)
                return;
            if(value==null) value = string.Empty;
            this.prefabpath = value;
            PrefabpathCall();
        }
    }

    /// <summary>预制体路径 --同步带有Key字段的值到服务器Player对象上，需要处理</summary>
    [Net.Serialize.NonSerialized]
    [Newtonsoft_X.Json.JsonIgnore]
    public System.String SyncIDPrefabpath
    {
        get { return prefabpath; }
        set
        {
            if (this.prefabpath == value)
                return;
            if(value==null) value = string.Empty;
            this.prefabpath = value;
            SyncPrefabpathCall();
        }
    }

    /// <summary>预制体路径 --同步到数据库</summary>
    public void PrefabpathCall()
    {
        
        Net.Client.ClientBase.Instance.SendRT(Net.Share.NetCmd.EntityRpc, (ushort)TitansiegeHashProto.PREFABPATH, prefabpath);
    }

	/// <summary>预制体路径 --同步带有Key字段的值到服务器Player对象上，需要处理</summary>
    public void SyncPrefabpathCall()
    {
        
        Net.Client.ClientBase.Instance.SendRT(Net.Share.NetCmd.EntityRpc, (ushort)TitansiegeHashProto.PREFABPATH, iD, prefabpath);
    }

    [Net.Share.Rpc(hash = (ushort)TitansiegeHashProto.PREFABPATH)]
    private void PrefabpathCall(System.String value)//重写NetPlayer的OnStart方法来处理客户端自动同步到服务器数据库, 方法内部添加AddRpc(data(CharactersData));收集Rpc
    {
        Prefabpath = value;
        OnPrefabpath?.Invoke();
    }

    [Net.Serialize.NonSerialized]
    [Newtonsoft_X.Json.JsonIgnore]
    public Action OnPrefabpath;

    private System.String headpath;
    /// <summary>头像路径</summary>
    public System.String Headpath
    {
        get { return headpath; }
        set
        {
            if (this.headpath == value)
                return;
            if(value==null) value = string.Empty;
            this.headpath = value;
#if SERVER
            if (RowState == DataRowState.Deleted | RowState == DataRowState.Detached) return;
            columns.Add(21);
            if(RowState != DataRowState.Added & RowState != 0)//如果还没初始化或者创建新行,只能修改值不能更新状态
                RowState = DataRowState.Modified;
            TitansiegeDB.I.Update(this);
#elif CLIENT
            HeadpathCall();
#endif
        }
    }

    /// <summary>头像路径 --同步到数据库</summary>
    [Net.Serialize.NonSerialized]
    [Newtonsoft_X.Json.JsonIgnore]
    public System.String SyncHeadpath
    {
        get { return headpath; }
        set
        {
            if (this.headpath == value)
                return;
            if(value==null) value = string.Empty;
            this.headpath = value;
            HeadpathCall();
        }
    }

    /// <summary>头像路径 --同步带有Key字段的值到服务器Player对象上，需要处理</summary>
    [Net.Serialize.NonSerialized]
    [Newtonsoft_X.Json.JsonIgnore]
    public System.String SyncIDHeadpath
    {
        get { return headpath; }
        set
        {
            if (this.headpath == value)
                return;
            if(value==null) value = string.Empty;
            this.headpath = value;
            SyncHeadpathCall();
        }
    }

    /// <summary>头像路径 --同步到数据库</summary>
    public void HeadpathCall()
    {
        
        Net.Client.ClientBase.Instance.SendRT(Net.Share.NetCmd.EntityRpc, (ushort)TitansiegeHashProto.HEADPATH, headpath);
    }

	/// <summary>头像路径 --同步带有Key字段的值到服务器Player对象上，需要处理</summary>
    public void SyncHeadpathCall()
    {
        
        Net.Client.ClientBase.Instance.SendRT(Net.Share.NetCmd.EntityRpc, (ushort)TitansiegeHashProto.HEADPATH, iD, headpath);
    }

    [Net.Share.Rpc(hash = (ushort)TitansiegeHashProto.HEADPATH)]
    private void HeadpathCall(System.String value)//重写NetPlayer的OnStart方法来处理客户端自动同步到服务器数据库, 方法内部添加AddRpc(data(CharactersData));收集Rpc
    {
        Headpath = value;
        OnHeadpath?.Invoke();
    }

    [Net.Serialize.NonSerialized]
    [Newtonsoft_X.Json.JsonIgnore]
    public Action OnHeadpath;

    private System.String lihuipath;
    /// <summary>立绘路径</summary>
    public System.String Lihuipath
    {
        get { return lihuipath; }
        set
        {
            if (this.lihuipath == value)
                return;
            if(value==null) value = string.Empty;
            this.lihuipath = value;
#if SERVER
            if (RowState == DataRowState.Deleted | RowState == DataRowState.Detached) return;
            columns.Add(22);
            if(RowState != DataRowState.Added & RowState != 0)//如果还没初始化或者创建新行,只能修改值不能更新状态
                RowState = DataRowState.Modified;
            TitansiegeDB.I.Update(this);
#elif CLIENT
            LihuipathCall();
#endif
        }
    }

    /// <summary>立绘路径 --同步到数据库</summary>
    [Net.Serialize.NonSerialized]
    [Newtonsoft_X.Json.JsonIgnore]
    public System.String SyncLihuipath
    {
        get { return lihuipath; }
        set
        {
            if (this.lihuipath == value)
                return;
            if(value==null) value = string.Empty;
            this.lihuipath = value;
            LihuipathCall();
        }
    }

    /// <summary>立绘路径 --同步带有Key字段的值到服务器Player对象上，需要处理</summary>
    [Net.Serialize.NonSerialized]
    [Newtonsoft_X.Json.JsonIgnore]
    public System.String SyncIDLihuipath
    {
        get { return lihuipath; }
        set
        {
            if (this.lihuipath == value)
                return;
            if(value==null) value = string.Empty;
            this.lihuipath = value;
            SyncLihuipathCall();
        }
    }

    /// <summary>立绘路径 --同步到数据库</summary>
    public void LihuipathCall()
    {
        
        Net.Client.ClientBase.Instance.SendRT(Net.Share.NetCmd.EntityRpc, (ushort)TitansiegeHashProto.LIHUIPATH, lihuipath);
    }

	/// <summary>立绘路径 --同步带有Key字段的值到服务器Player对象上，需要处理</summary>
    public void SyncLihuipathCall()
    {
        
        Net.Client.ClientBase.Instance.SendRT(Net.Share.NetCmd.EntityRpc, (ushort)TitansiegeHashProto.LIHUIPATH, iD, lihuipath);
    }

    [Net.Share.Rpc(hash = (ushort)TitansiegeHashProto.LIHUIPATH)]
    private void LihuipathCall(System.String value)//重写NetPlayer的OnStart方法来处理客户端自动同步到服务器数据库, 方法内部添加AddRpc(data(CharactersData));收集Rpc
    {
        Lihuipath = value;
        OnLihuipath?.Invoke();
    }

    [Net.Serialize.NonSerialized]
    [Newtonsoft_X.Json.JsonIgnore]
    public Action OnLihuipath;

    private System.Int16 wuqi;
    /// <summary>武器</summary>
    public System.Int16 Wuqi
    {
        get { return wuqi; }
        set
        {
            if (this.wuqi == value)
                return;
            
            this.wuqi = value;
#if SERVER
            if (RowState == DataRowState.Deleted | RowState == DataRowState.Detached) return;
            columns.Add(23);
            if(RowState != DataRowState.Added & RowState != 0)//如果还没初始化或者创建新行,只能修改值不能更新状态
                RowState = DataRowState.Modified;
            TitansiegeDB.I.Update(this);
#elif CLIENT
            WuqiCall();
#endif
        }
    }

    /// <summary>武器 --同步到数据库</summary>
    [Net.Serialize.NonSerialized]
    [Newtonsoft_X.Json.JsonIgnore]
    public System.Int16 SyncWuqi
    {
        get { return wuqi; }
        set
        {
            if (this.wuqi == value)
                return;
            
            this.wuqi = value;
            WuqiCall();
        }
    }

    /// <summary>武器 --同步带有Key字段的值到服务器Player对象上，需要处理</summary>
    [Net.Serialize.NonSerialized]
    [Newtonsoft_X.Json.JsonIgnore]
    public System.Int16 SyncIDWuqi
    {
        get { return wuqi; }
        set
        {
            if (this.wuqi == value)
                return;
            
            this.wuqi = value;
            SyncWuqiCall();
        }
    }

    /// <summary>武器 --同步到数据库</summary>
    public void WuqiCall()
    {
        
        Net.Client.ClientBase.Instance.SendRT(Net.Share.NetCmd.EntityRpc, (ushort)TitansiegeHashProto.WUQI, wuqi);
    }

	/// <summary>武器 --同步带有Key字段的值到服务器Player对象上，需要处理</summary>
    public void SyncWuqiCall()
    {
        
        Net.Client.ClientBase.Instance.SendRT(Net.Share.NetCmd.EntityRpc, (ushort)TitansiegeHashProto.WUQI, iD, wuqi);
    }

    [Net.Share.Rpc(hash = (ushort)TitansiegeHashProto.WUQI)]
    private void WuqiCall(System.Int16 value)//重写NetPlayer的OnStart方法来处理客户端自动同步到服务器数据库, 方法内部添加AddRpc(data(CharactersData));收集Rpc
    {
        Wuqi = value;
        OnWuqi?.Invoke();
    }

    [Net.Serialize.NonSerialized]
    [Newtonsoft_X.Json.JsonIgnore]
    public Action OnWuqi;

    private System.Int16 toukui;
    /// <summary>头盔</summary>
    public System.Int16 Toukui
    {
        get { return toukui; }
        set
        {
            if (this.toukui == value)
                return;
            
            this.toukui = value;
#if SERVER
            if (RowState == DataRowState.Deleted | RowState == DataRowState.Detached) return;
            columns.Add(24);
            if(RowState != DataRowState.Added & RowState != 0)//如果还没初始化或者创建新行,只能修改值不能更新状态
                RowState = DataRowState.Modified;
            TitansiegeDB.I.Update(this);
#elif CLIENT
            ToukuiCall();
#endif
        }
    }

    /// <summary>头盔 --同步到数据库</summary>
    [Net.Serialize.NonSerialized]
    [Newtonsoft_X.Json.JsonIgnore]
    public System.Int16 SyncToukui
    {
        get { return toukui; }
        set
        {
            if (this.toukui == value)
                return;
            
            this.toukui = value;
            ToukuiCall();
        }
    }

    /// <summary>头盔 --同步带有Key字段的值到服务器Player对象上，需要处理</summary>
    [Net.Serialize.NonSerialized]
    [Newtonsoft_X.Json.JsonIgnore]
    public System.Int16 SyncIDToukui
    {
        get { return toukui; }
        set
        {
            if (this.toukui == value)
                return;
            
            this.toukui = value;
            SyncToukuiCall();
        }
    }

    /// <summary>头盔 --同步到数据库</summary>
    public void ToukuiCall()
    {
        
        Net.Client.ClientBase.Instance.SendRT(Net.Share.NetCmd.EntityRpc, (ushort)TitansiegeHashProto.TOUKUI, toukui);
    }

	/// <summary>头盔 --同步带有Key字段的值到服务器Player对象上，需要处理</summary>
    public void SyncToukuiCall()
    {
        
        Net.Client.ClientBase.Instance.SendRT(Net.Share.NetCmd.EntityRpc, (ushort)TitansiegeHashProto.TOUKUI, iD, toukui);
    }

    [Net.Share.Rpc(hash = (ushort)TitansiegeHashProto.TOUKUI)]
    private void ToukuiCall(System.Int16 value)//重写NetPlayer的OnStart方法来处理客户端自动同步到服务器数据库, 方法内部添加AddRpc(data(CharactersData));收集Rpc
    {
        Toukui = value;
        OnToukui?.Invoke();
    }

    [Net.Serialize.NonSerialized]
    [Newtonsoft_X.Json.JsonIgnore]
    public Action OnToukui;

    private System.Int16 yifu;
    /// <summary>衣服</summary>
    public System.Int16 Yifu
    {
        get { return yifu; }
        set
        {
            if (this.yifu == value)
                return;
            
            this.yifu = value;
#if SERVER
            if (RowState == DataRowState.Deleted | RowState == DataRowState.Detached) return;
            columns.Add(25);
            if(RowState != DataRowState.Added & RowState != 0)//如果还没初始化或者创建新行,只能修改值不能更新状态
                RowState = DataRowState.Modified;
            TitansiegeDB.I.Update(this);
#elif CLIENT
            YifuCall();
#endif
        }
    }

    /// <summary>衣服 --同步到数据库</summary>
    [Net.Serialize.NonSerialized]
    [Newtonsoft_X.Json.JsonIgnore]
    public System.Int16 SyncYifu
    {
        get { return yifu; }
        set
        {
            if (this.yifu == value)
                return;
            
            this.yifu = value;
            YifuCall();
        }
    }

    /// <summary>衣服 --同步带有Key字段的值到服务器Player对象上，需要处理</summary>
    [Net.Serialize.NonSerialized]
    [Newtonsoft_X.Json.JsonIgnore]
    public System.Int16 SyncIDYifu
    {
        get { return yifu; }
        set
        {
            if (this.yifu == value)
                return;
            
            this.yifu = value;
            SyncYifuCall();
        }
    }

    /// <summary>衣服 --同步到数据库</summary>
    public void YifuCall()
    {
        
        Net.Client.ClientBase.Instance.SendRT(Net.Share.NetCmd.EntityRpc, (ushort)TitansiegeHashProto.YIFU, yifu);
    }

	/// <summary>衣服 --同步带有Key字段的值到服务器Player对象上，需要处理</summary>
    public void SyncYifuCall()
    {
        
        Net.Client.ClientBase.Instance.SendRT(Net.Share.NetCmd.EntityRpc, (ushort)TitansiegeHashProto.YIFU, iD, yifu);
    }

    [Net.Share.Rpc(hash = (ushort)TitansiegeHashProto.YIFU)]
    private void YifuCall(System.Int16 value)//重写NetPlayer的OnStart方法来处理客户端自动同步到服务器数据库, 方法内部添加AddRpc(data(CharactersData));收集Rpc
    {
        Yifu = value;
        OnYifu?.Invoke();
    }

    [Net.Serialize.NonSerialized]
    [Newtonsoft_X.Json.JsonIgnore]
    public Action OnYifu;

    private System.Int16 xiezi;
    /// <summary>鞋子</summary>
    public System.Int16 Xiezi
    {
        get { return xiezi; }
        set
        {
            if (this.xiezi == value)
                return;
            
            this.xiezi = value;
#if SERVER
            if (RowState == DataRowState.Deleted | RowState == DataRowState.Detached) return;
            columns.Add(26);
            if(RowState != DataRowState.Added & RowState != 0)//如果还没初始化或者创建新行,只能修改值不能更新状态
                RowState = DataRowState.Modified;
            TitansiegeDB.I.Update(this);
#elif CLIENT
            XieziCall();
#endif
        }
    }

    /// <summary>鞋子 --同步到数据库</summary>
    [Net.Serialize.NonSerialized]
    [Newtonsoft_X.Json.JsonIgnore]
    public System.Int16 SyncXiezi
    {
        get { return xiezi; }
        set
        {
            if (this.xiezi == value)
                return;
            
            this.xiezi = value;
            XieziCall();
        }
    }

    /// <summary>鞋子 --同步带有Key字段的值到服务器Player对象上，需要处理</summary>
    [Net.Serialize.NonSerialized]
    [Newtonsoft_X.Json.JsonIgnore]
    public System.Int16 SyncIDXiezi
    {
        get { return xiezi; }
        set
        {
            if (this.xiezi == value)
                return;
            
            this.xiezi = value;
            SyncXieziCall();
        }
    }

    /// <summary>鞋子 --同步到数据库</summary>
    public void XieziCall()
    {
        
        Net.Client.ClientBase.Instance.SendRT(Net.Share.NetCmd.EntityRpc, (ushort)TitansiegeHashProto.XIEZI, xiezi);
    }

	/// <summary>鞋子 --同步带有Key字段的值到服务器Player对象上，需要处理</summary>
    public void SyncXieziCall()
    {
        
        Net.Client.ClientBase.Instance.SendRT(Net.Share.NetCmd.EntityRpc, (ushort)TitansiegeHashProto.XIEZI, iD, xiezi);
    }

    [Net.Share.Rpc(hash = (ushort)TitansiegeHashProto.XIEZI)]
    private void XieziCall(System.Int16 value)//重写NetPlayer的OnStart方法来处理客户端自动同步到服务器数据库, 方法内部添加AddRpc(data(CharactersData));收集Rpc
    {
        Xiezi = value;
        OnXiezi?.Invoke();
    }

    [Net.Serialize.NonSerialized]
    [Newtonsoft_X.Json.JsonIgnore]
    public Action OnXiezi;

    private System.Int32 mapID;
    /// <summary></summary>
    public System.Int32 MapID
    {
        get { return mapID; }
        set
        {
            if (this.mapID == value)
                return;
            
            this.mapID = value;
#if SERVER
            if (RowState == DataRowState.Deleted | RowState == DataRowState.Detached) return;
            columns.Add(27);
            if(RowState != DataRowState.Added & RowState != 0)//如果还没初始化或者创建新行,只能修改值不能更新状态
                RowState = DataRowState.Modified;
            TitansiegeDB.I.Update(this);
#elif CLIENT
            MapIDCall();
#endif
        }
    }

    /// <summary> --同步到数据库</summary>
    [Net.Serialize.NonSerialized]
    [Newtonsoft_X.Json.JsonIgnore]
    public System.Int32 SyncMapID
    {
        get { return mapID; }
        set
        {
            if (this.mapID == value)
                return;
            
            this.mapID = value;
            MapIDCall();
        }
    }

    /// <summary> --同步带有Key字段的值到服务器Player对象上，需要处理</summary>
    [Net.Serialize.NonSerialized]
    [Newtonsoft_X.Json.JsonIgnore]
    public System.Int32 SyncIDMapID
    {
        get { return mapID; }
        set
        {
            if (this.mapID == value)
                return;
            
            this.mapID = value;
            SyncMapIDCall();
        }
    }

    /// <summary> --同步到数据库</summary>
    public void MapIDCall()
    {
        
        Net.Client.ClientBase.Instance.SendRT(Net.Share.NetCmd.EntityRpc, (ushort)TitansiegeHashProto.MAPID, mapID);
    }

	/// <summary> --同步带有Key字段的值到服务器Player对象上，需要处理</summary>
    public void SyncMapIDCall()
    {
        
        Net.Client.ClientBase.Instance.SendRT(Net.Share.NetCmd.EntityRpc, (ushort)TitansiegeHashProto.MAPID, iD, mapID);
    }

    [Net.Share.Rpc(hash = (ushort)TitansiegeHashProto.MAPID)]
    private void MapIDCall(System.Int32 value)//重写NetPlayer的OnStart方法来处理客户端自动同步到服务器数据库, 方法内部添加AddRpc(data(CharactersData));收集Rpc
    {
        MapID = value;
        OnMapID?.Invoke();
    }

    [Net.Serialize.NonSerialized]
    [Newtonsoft_X.Json.JsonIgnore]
    public Action OnMapID;

    private System.Int32 mapPosX;
    /// <summary></summary>
    public System.Int32 MapPosX
    {
        get { return mapPosX; }
        set
        {
            if (this.mapPosX == value)
                return;
            
            this.mapPosX = value;
#if SERVER
            if (RowState == DataRowState.Deleted | RowState == DataRowState.Detached) return;
            columns.Add(28);
            if(RowState != DataRowState.Added & RowState != 0)//如果还没初始化或者创建新行,只能修改值不能更新状态
                RowState = DataRowState.Modified;
            TitansiegeDB.I.Update(this);
#elif CLIENT
            MapPosXCall();
#endif
        }
    }

    /// <summary> --同步到数据库</summary>
    [Net.Serialize.NonSerialized]
    [Newtonsoft_X.Json.JsonIgnore]
    public System.Int32 SyncMapPosX
    {
        get { return mapPosX; }
        set
        {
            if (this.mapPosX == value)
                return;
            
            this.mapPosX = value;
            MapPosXCall();
        }
    }

    /// <summary> --同步带有Key字段的值到服务器Player对象上，需要处理</summary>
    [Net.Serialize.NonSerialized]
    [Newtonsoft_X.Json.JsonIgnore]
    public System.Int32 SyncIDMapPosX
    {
        get { return mapPosX; }
        set
        {
            if (this.mapPosX == value)
                return;
            
            this.mapPosX = value;
            SyncMapPosXCall();
        }
    }

    /// <summary> --同步到数据库</summary>
    public void MapPosXCall()
    {
        
        Net.Client.ClientBase.Instance.SendRT(Net.Share.NetCmd.EntityRpc, (ushort)TitansiegeHashProto.MAPPOSX, mapPosX);
    }

	/// <summary> --同步带有Key字段的值到服务器Player对象上，需要处理</summary>
    public void SyncMapPosXCall()
    {
        
        Net.Client.ClientBase.Instance.SendRT(Net.Share.NetCmd.EntityRpc, (ushort)TitansiegeHashProto.MAPPOSX, iD, mapPosX);
    }

    [Net.Share.Rpc(hash = (ushort)TitansiegeHashProto.MAPPOSX)]
    private void MapPosXCall(System.Int32 value)//重写NetPlayer的OnStart方法来处理客户端自动同步到服务器数据库, 方法内部添加AddRpc(data(CharactersData));收集Rpc
    {
        MapPosX = value;
        OnMapPosX?.Invoke();
    }

    [Net.Serialize.NonSerialized]
    [Newtonsoft_X.Json.JsonIgnore]
    public Action OnMapPosX;

    private System.Int32 mapPosY;
    /// <summary></summary>
    public System.Int32 MapPosY
    {
        get { return mapPosY; }
        set
        {
            if (this.mapPosY == value)
                return;
            
            this.mapPosY = value;
#if SERVER
            if (RowState == DataRowState.Deleted | RowState == DataRowState.Detached) return;
            columns.Add(29);
            if(RowState != DataRowState.Added & RowState != 0)//如果还没初始化或者创建新行,只能修改值不能更新状态
                RowState = DataRowState.Modified;
            TitansiegeDB.I.Update(this);
#elif CLIENT
            MapPosYCall();
#endif
        }
    }

    /// <summary> --同步到数据库</summary>
    [Net.Serialize.NonSerialized]
    [Newtonsoft_X.Json.JsonIgnore]
    public System.Int32 SyncMapPosY
    {
        get { return mapPosY; }
        set
        {
            if (this.mapPosY == value)
                return;
            
            this.mapPosY = value;
            MapPosYCall();
        }
    }

    /// <summary> --同步带有Key字段的值到服务器Player对象上，需要处理</summary>
    [Net.Serialize.NonSerialized]
    [Newtonsoft_X.Json.JsonIgnore]
    public System.Int32 SyncIDMapPosY
    {
        get { return mapPosY; }
        set
        {
            if (this.mapPosY == value)
                return;
            
            this.mapPosY = value;
            SyncMapPosYCall();
        }
    }

    /// <summary> --同步到数据库</summary>
    public void MapPosYCall()
    {
        
        Net.Client.ClientBase.Instance.SendRT(Net.Share.NetCmd.EntityRpc, (ushort)TitansiegeHashProto.MAPPOSY, mapPosY);
    }

	/// <summary> --同步带有Key字段的值到服务器Player对象上，需要处理</summary>
    public void SyncMapPosYCall()
    {
        
        Net.Client.ClientBase.Instance.SendRT(Net.Share.NetCmd.EntityRpc, (ushort)TitansiegeHashProto.MAPPOSY, iD, mapPosY);
    }

    [Net.Share.Rpc(hash = (ushort)TitansiegeHashProto.MAPPOSY)]
    private void MapPosYCall(System.Int32 value)//重写NetPlayer的OnStart方法来处理客户端自动同步到服务器数据库, 方法内部添加AddRpc(data(CharactersData));收集Rpc
    {
        MapPosY = value;
        OnMapPosY?.Invoke();
    }

    [Net.Serialize.NonSerialized]
    [Newtonsoft_X.Json.JsonIgnore]
    public Action OnMapPosY;

    private System.Int32 mapPosZ;
    /// <summary></summary>
    public System.Int32 MapPosZ
    {
        get { return mapPosZ; }
        set
        {
            if (this.mapPosZ == value)
                return;
            
            this.mapPosZ = value;
#if SERVER
            if (RowState == DataRowState.Deleted | RowState == DataRowState.Detached) return;
            columns.Add(30);
            if(RowState != DataRowState.Added & RowState != 0)//如果还没初始化或者创建新行,只能修改值不能更新状态
                RowState = DataRowState.Modified;
            TitansiegeDB.I.Update(this);
#elif CLIENT
            MapPosZCall();
#endif
        }
    }

    /// <summary> --同步到数据库</summary>
    [Net.Serialize.NonSerialized]
    [Newtonsoft_X.Json.JsonIgnore]
    public System.Int32 SyncMapPosZ
    {
        get { return mapPosZ; }
        set
        {
            if (this.mapPosZ == value)
                return;
            
            this.mapPosZ = value;
            MapPosZCall();
        }
    }

    /// <summary> --同步带有Key字段的值到服务器Player对象上，需要处理</summary>
    [Net.Serialize.NonSerialized]
    [Newtonsoft_X.Json.JsonIgnore]
    public System.Int32 SyncIDMapPosZ
    {
        get { return mapPosZ; }
        set
        {
            if (this.mapPosZ == value)
                return;
            
            this.mapPosZ = value;
            SyncMapPosZCall();
        }
    }

    /// <summary> --同步到数据库</summary>
    public void MapPosZCall()
    {
        
        Net.Client.ClientBase.Instance.SendRT(Net.Share.NetCmd.EntityRpc, (ushort)TitansiegeHashProto.MAPPOSZ, mapPosZ);
    }

	/// <summary> --同步带有Key字段的值到服务器Player对象上，需要处理</summary>
    public void SyncMapPosZCall()
    {
        
        Net.Client.ClientBase.Instance.SendRT(Net.Share.NetCmd.EntityRpc, (ushort)TitansiegeHashProto.MAPPOSZ, iD, mapPosZ);
    }

    [Net.Share.Rpc(hash = (ushort)TitansiegeHashProto.MAPPOSZ)]
    private void MapPosZCall(System.Int32 value)//重写NetPlayer的OnStart方法来处理客户端自动同步到服务器数据库, 方法内部添加AddRpc(data(CharactersData));收集Rpc
    {
        MapPosZ = value;
        OnMapPosZ?.Invoke();
    }

    [Net.Serialize.NonSerialized]
    [Newtonsoft_X.Json.JsonIgnore]
    public Action OnMapPosZ;

    private System.Int64 uid;
    /// <summary></summary>
    public System.Int64 Uid
    {
        get { return uid; }
        set
        {
            if (this.uid == value)
                return;
            
            this.uid = value;
#if SERVER
            if (RowState == DataRowState.Deleted | RowState == DataRowState.Detached) return;
            columns.Add(31);
            if(RowState != DataRowState.Added & RowState != 0)//如果还没初始化或者创建新行,只能修改值不能更新状态
                RowState = DataRowState.Modified;
            TitansiegeDB.I.Update(this);
#elif CLIENT
            UidCall();
#endif
        }
    }

    /// <summary> --同步到数据库</summary>
    [Net.Serialize.NonSerialized]
    [Newtonsoft_X.Json.JsonIgnore]
    public System.Int64 SyncUid
    {
        get { return uid; }
        set
        {
            if (this.uid == value)
                return;
            
            this.uid = value;
            UidCall();
        }
    }

    /// <summary> --同步带有Key字段的值到服务器Player对象上，需要处理</summary>
    [Net.Serialize.NonSerialized]
    [Newtonsoft_X.Json.JsonIgnore]
    public System.Int64 SyncIDUid
    {
        get { return uid; }
        set
        {
            if (this.uid == value)
                return;
            
            this.uid = value;
            SyncUidCall();
        }
    }

    /// <summary> --同步到数据库</summary>
    public void UidCall()
    {
        
        Net.Client.ClientBase.Instance.SendRT(Net.Share.NetCmd.EntityRpc, (ushort)TitansiegeHashProto.UID, uid);
    }

	/// <summary> --同步带有Key字段的值到服务器Player对象上，需要处理</summary>
    public void SyncUidCall()
    {
        
        Net.Client.ClientBase.Instance.SendRT(Net.Share.NetCmd.EntityRpc, (ushort)TitansiegeHashProto.UID, iD, uid);
    }

    [Net.Share.Rpc(hash = (ushort)TitansiegeHashProto.UID)]
    private void UidCall(System.Int64 value)//重写NetPlayer的OnStart方法来处理客户端自动同步到服务器数据库, 方法内部添加AddRpc(data(CharactersData));收集Rpc
    {
        Uid = value;
        OnUid?.Invoke();
    }

    [Net.Serialize.NonSerialized]
    [Newtonsoft_X.Json.JsonIgnore]
    public Action OnUid;

    private System.DateTime lastDate;
    /// <summary>最后登录时间</summary>
    public System.DateTime LastDate
    {
        get { return lastDate; }
        set
        {
            if (this.lastDate == value)
                return;
            
            this.lastDate = value;
#if SERVER
            if (RowState == DataRowState.Deleted | RowState == DataRowState.Detached) return;
            columns.Add(32);
            if(RowState != DataRowState.Added & RowState != 0)//如果还没初始化或者创建新行,只能修改值不能更新状态
                RowState = DataRowState.Modified;
            TitansiegeDB.I.Update(this);
#elif CLIENT
            LastDateCall();
#endif
        }
    }

    /// <summary>最后登录时间 --同步到数据库</summary>
    [Net.Serialize.NonSerialized]
    [Newtonsoft_X.Json.JsonIgnore]
    public System.DateTime SyncLastDate
    {
        get { return lastDate; }
        set
        {
            if (this.lastDate == value)
                return;
            
            this.lastDate = value;
            LastDateCall();
        }
    }

    /// <summary>最后登录时间 --同步带有Key字段的值到服务器Player对象上，需要处理</summary>
    [Net.Serialize.NonSerialized]
    [Newtonsoft_X.Json.JsonIgnore]
    public System.DateTime SyncIDLastDate
    {
        get { return lastDate; }
        set
        {
            if (this.lastDate == value)
                return;
            
            this.lastDate = value;
            SyncLastDateCall();
        }
    }

    /// <summary>最后登录时间 --同步到数据库</summary>
    public void LastDateCall()
    {
        
        Net.Client.ClientBase.Instance.SendRT(Net.Share.NetCmd.EntityRpc, (ushort)TitansiegeHashProto.LASTDATE, lastDate);
    }

	/// <summary>最后登录时间 --同步带有Key字段的值到服务器Player对象上，需要处理</summary>
    public void SyncLastDateCall()
    {
        
        Net.Client.ClientBase.Instance.SendRT(Net.Share.NetCmd.EntityRpc, (ushort)TitansiegeHashProto.LASTDATE, iD, lastDate);
    }

    [Net.Share.Rpc(hash = (ushort)TitansiegeHashProto.LASTDATE)]
    private void LastDateCall(System.DateTime value)//重写NetPlayer的OnStart方法来处理客户端自动同步到服务器数据库, 方法内部添加AddRpc(data(CharactersData));收集Rpc
    {
        LastDate = value;
        OnLastDate?.Invoke();
    }

    [Net.Serialize.NonSerialized]
    [Newtonsoft_X.Json.JsonIgnore]
    public Action OnLastDate;

    private System.Boolean delRole;
    /// <summary>是否删除</summary>
    public System.Boolean DelRole
    {
        get { return delRole; }
        set
        {
            if (this.delRole == value)
                return;
            
            this.delRole = value;
#if SERVER
            if (RowState == DataRowState.Deleted | RowState == DataRowState.Detached) return;
            columns.Add(33);
            if(RowState != DataRowState.Added & RowState != 0)//如果还没初始化或者创建新行,只能修改值不能更新状态
                RowState = DataRowState.Modified;
            TitansiegeDB.I.Update(this);
#elif CLIENT
            DelRoleCall();
#endif
        }
    }

    /// <summary>是否删除 --同步到数据库</summary>
    [Net.Serialize.NonSerialized]
    [Newtonsoft_X.Json.JsonIgnore]
    public System.Boolean SyncDelRole
    {
        get { return delRole; }
        set
        {
            if (this.delRole == value)
                return;
            
            this.delRole = value;
            DelRoleCall();
        }
    }

    /// <summary>是否删除 --同步带有Key字段的值到服务器Player对象上，需要处理</summary>
    [Net.Serialize.NonSerialized]
    [Newtonsoft_X.Json.JsonIgnore]
    public System.Boolean SyncIDDelRole
    {
        get { return delRole; }
        set
        {
            if (this.delRole == value)
                return;
            
            this.delRole = value;
            SyncDelRoleCall();
        }
    }

    /// <summary>是否删除 --同步到数据库</summary>
    public void DelRoleCall()
    {
        
        Net.Client.ClientBase.Instance.SendRT(Net.Share.NetCmd.EntityRpc, (ushort)TitansiegeHashProto.DELROLE, delRole);
    }

	/// <summary>是否删除 --同步带有Key字段的值到服务器Player对象上，需要处理</summary>
    public void SyncDelRoleCall()
    {
        
        Net.Client.ClientBase.Instance.SendRT(Net.Share.NetCmd.EntityRpc, (ushort)TitansiegeHashProto.DELROLE, iD, delRole);
    }

    [Net.Share.Rpc(hash = (ushort)TitansiegeHashProto.DELROLE)]
    private void DelRoleCall(System.Boolean value)//重写NetPlayer的OnStart方法来处理客户端自动同步到服务器数据库, 方法内部添加AddRpc(data(CharactersData));收集Rpc
    {
        DelRole = value;
        OnDelRole?.Invoke();
    }

    [Net.Serialize.NonSerialized]
    [Newtonsoft_X.Json.JsonIgnore]
    public Action OnDelRole;


    public CharactersData() { }

#if SERVER
    public CharactersData(params object[] parms)
    {
        NewTableRow(parms);
    }
    public void NewTableRow()
    {
        for (int i = 0; i < 34; i++)
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
                return "Name";

            case 2:
                return "Zhiye";

            case 3:
                return "Level";

            case 4:
                return "Exp";

            case 5:
                return "Shengming";

            case 6:
                return "Fali";

            case 7:
                return "Tizhi";

            case 8:
                return "Liliang";

            case 9:
                return "Minjie";

            case 10:
                return "Moli";

            case 11:
                return "Meili";

            case 12:
                return "Xingyun";

            case 13:
                return "Lianjin";

            case 14:
                return "Duanzao";

            case 15:
                return "Jinbi";

            case 16:
                return "Zuanshi";

            case 17:
                return "Chenghao";

            case 18:
                return "Friends";

            case 19:
                return "Skills";

            case 20:
                return "Prefabpath";

            case 21:
                return "Headpath";

            case 22:
                return "Lihuipath";

            case 23:
                return "Wuqi";

            case 24:
                return "Toukui";

            case 25:
                return "Yifu";

            case 26:
                return "Xiezi";

            case 27:
                return "MapID";

            case 28:
                return "MapPosX";

            case 29:
                return "MapPosY";

            case 30:
                return "MapPosZ";

            case 31:
                return "Uid";

            case 32:
                return "LastDate";

            case 33:
                return "DelRole";

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
                    return this.Name;

                case 2:
                    return this.Zhiye;

                case 3:
                    return this.Level;

                case 4:
                    return this.Exp;

                case 5:
                    return this.Shengming;

                case 6:
                    return this.Fali;

                case 7:
                    return this.Tizhi;

                case 8:
                    return this.Liliang;

                case 9:
                    return this.Minjie;

                case 10:
                    return this.Moli;

                case 11:
                    return this.Meili;

                case 12:
                    return this.Xingyun;

                case 13:
                    return this.Lianjin;

                case 14:
                    return this.Duanzao;

                case 15:
                    return this.Jinbi;

                case 16:
                    return this.Zuanshi;

                case 17:
                    return this.Chenghao;

                case 18:
                    return this.Friends;

                case 19:
                    return this.Skills;

                case 20:
                    return this.Prefabpath;

                case 21:
                    return this.Headpath;

                case 22:
                    return this.Lihuipath;

                case 23:
                    return this.Wuqi;

                case 24:
                    return this.Toukui;

                case 25:
                    return this.Yifu;

                case 26:
                    return this.Xiezi;

                case 27:
                    return this.MapID;

                case 28:
                    return this.MapPosX;

                case 29:
                    return this.MapPosY;

                case 30:
                    return this.MapPosZ;

                case 31:
                    return this.Uid;

                case 32:
                    return this.LastDate;

                case 33:
                    return this.DelRole;

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
                    this.Name = (System.String)value;
                    break;

                case 2:
                    this.Zhiye = (System.SByte)value;
                    break;

                case 3:
                    this.Level = (System.SByte)value;
                    break;

                case 4:
                    this.Exp = (System.Int32)value;
                    break;

                case 5:
                    this.Shengming = (System.Int32)value;
                    break;

                case 6:
                    this.Fali = (System.Int32)value;
                    break;

                case 7:
                    this.Tizhi = (System.Int16)value;
                    break;

                case 8:
                    this.Liliang = (System.Int16)value;
                    break;

                case 9:
                    this.Minjie = (System.Int16)value;
                    break;

                case 10:
                    this.Moli = (System.Int16)value;
                    break;

                case 11:
                    this.Meili = (System.Int16)value;
                    break;

                case 12:
                    this.Xingyun = (System.Int16)value;
                    break;

                case 13:
                    this.Lianjin = (System.Int16)value;
                    break;

                case 14:
                    this.Duanzao = (System.Int16)value;
                    break;

                case 15:
                    this.Jinbi = (System.Int32)value;
                    break;

                case 16:
                    this.Zuanshi = (System.Int32)value;
                    break;

                case 17:
                    this.Chenghao = (System.String)value;
                    break;

                case 18:
                    this.Friends = (System.String)value;
                    break;

                case 19:
                    this.Skills = (System.String)value;
                    break;

                case 20:
                    this.Prefabpath = (System.String)value;
                    break;

                case 21:
                    this.Headpath = (System.String)value;
                    break;

                case 22:
                    this.Lihuipath = (System.String)value;
                    break;

                case 23:
                    this.Wuqi = (System.Int16)value;
                    break;

                case 24:
                    this.Toukui = (System.Int16)value;
                    break;

                case 25:
                    this.Yifu = (System.Int16)value;
                    break;

                case 26:
                    this.Xiezi = (System.Int16)value;
                    break;

                case 27:
                    this.MapID = (System.Int32)value;
                    break;

                case 28:
                    this.MapPosX = (System.Int32)value;
                    break;

                case 29:
                    this.MapPosY = (System.Int32)value;
                    break;

                case 30:
                    this.MapPosZ = (System.Int32)value;
                    break;

                case 31:
                    this.Uid = (System.Int64)value;
                    break;

                case 32:
                    this.LastDate = (System.DateTime)value;
                    break;

                case 33:
                    this.DelRole = (System.Boolean)value;
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
    public static CharactersData Query(string filterExpression)
    {
        var cmdText = $"select * from characters where {filterExpression}; ";
        var data = TitansiegeDB.ExecuteQuery<CharactersData>(cmdText);
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
    public static async Task<CharactersData> QueryAsync(string filterExpression)
    {
        var cmdText = $"select * from characters where {filterExpression}; ";
        var data = await TitansiegeDB.ExecuteQueryAsync<CharactersData>(cmdText);
        return data;
    }
    public static CharactersData[] QueryList(string filterExpression)
    {
        var cmdText = $"select * from characters where {filterExpression}; ";
        var datas = TitansiegeDB.ExecuteQueryList<CharactersData>(cmdText);
        return datas;
    }
    public static async Task<CharactersData[]> QueryListAsync(string filterExpression)
    {
        var cmdText = $"select * from characters where {filterExpression}; ";
        var datas = await TitansiegeDB.ExecuteQueryListAsync<CharactersData>(cmdText);
        return datas;
    }
    public void Update()
    {
        if (RowState == DataRowState.Deleted | RowState == DataRowState.Detached | RowState == DataRowState.Added | RowState == 0) return;

        for (int i = 0; i < 34; i++)
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

        if (row[1] is System.String Name)
            this.Name = Name;

        if (row[2] is System.SByte Zhiye)
            this.Zhiye = Zhiye;

        if (row[3] is System.SByte Level)
            this.Level = Level;

        if (row[4] is System.Int32 Exp)
            this.Exp = Exp;

        if (row[5] is System.Int32 Shengming)
            this.Shengming = Shengming;

        if (row[6] is System.Int32 Fali)
            this.Fali = Fali;

        if (row[7] is System.Int16 Tizhi)
            this.Tizhi = Tizhi;

        if (row[8] is System.Int16 Liliang)
            this.Liliang = Liliang;

        if (row[9] is System.Int16 Minjie)
            this.Minjie = Minjie;

        if (row[10] is System.Int16 Moli)
            this.Moli = Moli;

        if (row[11] is System.Int16 Meili)
            this.Meili = Meili;

        if (row[12] is System.Int16 Xingyun)
            this.Xingyun = Xingyun;

        if (row[13] is System.Int16 Lianjin)
            this.Lianjin = Lianjin;

        if (row[14] is System.Int16 Duanzao)
            this.Duanzao = Duanzao;

        if (row[15] is System.Int32 Jinbi)
            this.Jinbi = Jinbi;

        if (row[16] is System.Int32 Zuanshi)
            this.Zuanshi = Zuanshi;

        if (row[17] is System.String Chenghao)
            this.Chenghao = Chenghao;

        if (row[18] is System.String Friends)
            this.Friends = Friends;

        if (row[19] is System.String Skills)
            this.Skills = Skills;

        if (row[20] is System.String Prefabpath)
            this.Prefabpath = Prefabpath;

        if (row[21] is System.String Headpath)
            this.Headpath = Headpath;

        if (row[22] is System.String Lihuipath)
            this.Lihuipath = Lihuipath;

        if (row[23] is System.Int16 Wuqi)
            this.Wuqi = Wuqi;

        if (row[24] is System.Int16 Toukui)
            this.Toukui = Toukui;

        if (row[25] is System.Int16 Yifu)
            this.Yifu = Yifu;

        if (row[26] is System.Int16 Xiezi)
            this.Xiezi = Xiezi;

        if (row[27] is System.Int32 MapID)
            this.MapID = MapID;

        if (row[28] is System.Int32 MapPosX)
            this.MapPosX = MapPosX;

        if (row[29] is System.Int32 MapPosY)
            this.MapPosY = MapPosY;

        if (row[30] is System.Int32 MapPosZ)
            this.MapPosZ = MapPosZ;

        if (row[31] is System.Int64 Uid)
            this.Uid = Uid;

        if (row[32] is System.DateTime LastDate)
            this.LastDate = LastDate;

        if (row[33] is System.Boolean DelRole)
            this.DelRole = DelRole;

    }

    public void AddedSql(StringBuilder sb, List<IDbDataParameter> parms, ref int parmsLen, ref int count)
    {
#if SERVER
        string cmdText = "REPLACE INTO characters SET ";
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
        string cmdText = $"UPDATE characters SET ";
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
        string cmdText = $"DELETE FROM characters WHERE `iD` = {iD}; ";
        sb.Append(cmdText);
        RowState = DataRowState.Deleted;
#endif
    }
}
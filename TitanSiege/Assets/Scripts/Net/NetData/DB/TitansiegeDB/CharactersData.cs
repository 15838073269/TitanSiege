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
    public partial class CharactersData : IDataRow
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

        private Int64 iD;
        /// <summary></summary>
        public Int64 ID { get { return iD; } set { this.iD = value; } }

     
        private readonly StringObs name = new StringObs("CharactersData_name", false, null);

        /// <summary> --获得属性观察对象</summary>
        internal StringObs NameObserver => name;

        /// <summary></summary>
        public String Name { get => GetNameValue(); set => CheckNameValue(value, 0); }

        /// <summary> --同步到数据库</summary>
        internal String SyncName { get => GetNameValue(); set => CheckNameValue(value, 1); }

        /// <summary> --同步带有Key字段的值到服务器Player对象上，需要处理</summary>
        internal String SyncIDName { get => GetNameValue(); set => CheckNameValue(value, 2); }

        private String GetNameValue() => this.name.Value;

        private void CheckNameValue(String value, int type) 
        {
            if (this.name.Value == value)
                return;
            this.name.Value = value;
            if (type == 0)
                CheckUpdate(1);
            else if (type == 1)
                NameCall(false);
            else if (type == 2)
                NameCall(true);
            OnValueChanged?.Invoke(TitansiegeHashProto.CHARACTERS_NAME, value);
        }

        /// <summary> --同步当前值到服务器Player对象上，需要处理</summary>
        public void NameCall(bool syncId = false)
        {
            
            object[] objects;
            if (syncId) objects = new object[] { this[TitansiegeDBEvent.CharactersData_SyncID], name.Value };
            else objects = new object[] { name.Value };
#if SERVER
            CheckUpdate(1);
            TitansiegeDBEvent.OnSyncProperty?.Invoke(client, NetCmd.SyncPropertyData, (ushort)TitansiegeHashProto.CHARACTERS_NAME, objects);
#else
            TitansiegeDBEvent.Client.SendRT(NetCmd.SyncPropertyData, (ushort)TitansiegeHashProto.CHARACTERS_NAME, objects);
#endif
        }

        [Rpc(hash = (ushort)TitansiegeHashProto.CHARACTERS_NAME)]
        private void NameRpc(String value)//重写NetPlayer的OnStart方法来处理客户端自动同步到服务器数据库, 方法内部添加AddRpc(data(CharactersData));收集Rpc
        {
            Name = value;
        }
     
        private readonly SByteObs zhiye = new SByteObs("CharactersData_zhiye", true, null);

        /// <summary>职业 --获得属性观察对象</summary>
        internal SByteObs ZhiyeObserver => zhiye;

        /// <summary>职业</summary>
        public SByte Zhiye { get => GetZhiyeValue(); set => CheckZhiyeValue(value, 0); }

        /// <summary>职业 --同步到数据库</summary>
        internal SByte SyncZhiye { get => GetZhiyeValue(); set => CheckZhiyeValue(value, 1); }

        /// <summary>职业 --同步带有Key字段的值到服务器Player对象上，需要处理</summary>
        internal SByte SyncIDZhiye { get => GetZhiyeValue(); set => CheckZhiyeValue(value, 2); }

        private SByte GetZhiyeValue() => this.zhiye.Value;

        private void CheckZhiyeValue(SByte value, int type) 
        {
            if (this.zhiye.Value == value)
                return;
            this.zhiye.Value = value;
            if (type == 0)
                CheckUpdate(2);
            else if (type == 1)
                ZhiyeCall(false);
            else if (type == 2)
                ZhiyeCall(true);
            OnValueChanged?.Invoke(TitansiegeHashProto.CHARACTERS_ZHIYE, value);
        }

        /// <summary>职业 --同步当前值到服务器Player对象上，需要处理</summary>
        public void ZhiyeCall(bool syncId = false)
        {
            
            object[] objects;
            if (syncId) objects = new object[] { this[TitansiegeDBEvent.CharactersData_SyncID], zhiye.Value };
            else objects = new object[] { zhiye.Value };
#if SERVER
            CheckUpdate(2);
            TitansiegeDBEvent.OnSyncProperty?.Invoke(client, NetCmd.SyncPropertyData, (ushort)TitansiegeHashProto.CHARACTERS_ZHIYE, objects);
#else
            TitansiegeDBEvent.Client.SendRT(NetCmd.SyncPropertyData, (ushort)TitansiegeHashProto.CHARACTERS_ZHIYE, objects);
#endif
        }

        [Rpc(hash = (ushort)TitansiegeHashProto.CHARACTERS_ZHIYE)]
        private void ZhiyeRpc(SByte value)//重写NetPlayer的OnStart方法来处理客户端自动同步到服务器数据库, 方法内部添加AddRpc(data(CharactersData));收集Rpc
        {
            Zhiye = value;
        }
     
        private readonly SByteObs level = new SByteObs("CharactersData_level", true, null);

        /// <summary>等级 --获得属性观察对象</summary>
        internal SByteObs LevelObserver => level;

        /// <summary>等级</summary>
        public SByte Level { get => GetLevelValue(); set => CheckLevelValue(value, 0); }

        /// <summary>等级 --同步到数据库</summary>
        internal SByte SyncLevel { get => GetLevelValue(); set => CheckLevelValue(value, 1); }

        /// <summary>等级 --同步带有Key字段的值到服务器Player对象上，需要处理</summary>
        internal SByte SyncIDLevel { get => GetLevelValue(); set => CheckLevelValue(value, 2); }

        private SByte GetLevelValue() => this.level.Value;

        private void CheckLevelValue(SByte value, int type) 
        {
            if (this.level.Value == value)
                return;
            this.level.Value = value;
            if (type == 0)
                CheckUpdate(3);
            else if (type == 1)
                LevelCall(false);
            else if (type == 2)
                LevelCall(true);
            OnValueChanged?.Invoke(TitansiegeHashProto.CHARACTERS_LEVEL, value);
        }

        /// <summary>等级 --同步当前值到服务器Player对象上，需要处理</summary>
        public void LevelCall(bool syncId = false)
        {
            
            object[] objects;
            if (syncId) objects = new object[] { this[TitansiegeDBEvent.CharactersData_SyncID], level.Value };
            else objects = new object[] { level.Value };
#if SERVER
            CheckUpdate(3);
            TitansiegeDBEvent.OnSyncProperty?.Invoke(client, NetCmd.SyncPropertyData, (ushort)TitansiegeHashProto.CHARACTERS_LEVEL, objects);
#else
            TitansiegeDBEvent.Client.SendRT(NetCmd.SyncPropertyData, (ushort)TitansiegeHashProto.CHARACTERS_LEVEL, objects);
#endif
        }

        [Rpc(hash = (ushort)TitansiegeHashProto.CHARACTERS_LEVEL)]
        private void LevelRpc(SByte value)//重写NetPlayer的OnStart方法来处理客户端自动同步到服务器数据库, 方法内部添加AddRpc(data(CharactersData));收集Rpc
        {
            Level = value;
        }
     
        private readonly Int32Obs levelupid = new Int32Obs("CharactersData_levelupid", true, null);

        /// <summary>升级配置表的id --获得属性观察对象</summary>
        internal Int32Obs LevelupidObserver => levelupid;

        /// <summary>升级配置表的id</summary>
        public Int32 Levelupid { get => GetLevelupidValue(); set => CheckLevelupidValue(value, 0); }

        /// <summary>升级配置表的id --同步到数据库</summary>
        internal Int32 SyncLevelupid { get => GetLevelupidValue(); set => CheckLevelupidValue(value, 1); }

        /// <summary>升级配置表的id --同步带有Key字段的值到服务器Player对象上，需要处理</summary>
        internal Int32 SyncIDLevelupid { get => GetLevelupidValue(); set => CheckLevelupidValue(value, 2); }

        private Int32 GetLevelupidValue() => this.levelupid.Value;

        private void CheckLevelupidValue(Int32 value, int type) 
        {
            if (this.levelupid.Value == value)
                return;
            this.levelupid.Value = value;
            if (type == 0)
                CheckUpdate(4);
            else if (type == 1)
                LevelupidCall(false);
            else if (type == 2)
                LevelupidCall(true);
            OnValueChanged?.Invoke(TitansiegeHashProto.CHARACTERS_LEVELUPID, value);
        }

        /// <summary>升级配置表的id --同步当前值到服务器Player对象上，需要处理</summary>
        public void LevelupidCall(bool syncId = false)
        {
            
            object[] objects;
            if (syncId) objects = new object[] { this[TitansiegeDBEvent.CharactersData_SyncID], levelupid.Value };
            else objects = new object[] { levelupid.Value };
#if SERVER
            CheckUpdate(4);
            TitansiegeDBEvent.OnSyncProperty?.Invoke(client, NetCmd.SyncPropertyData, (ushort)TitansiegeHashProto.CHARACTERS_LEVELUPID, objects);
#else
            TitansiegeDBEvent.Client.SendRT(NetCmd.SyncPropertyData, (ushort)TitansiegeHashProto.CHARACTERS_LEVELUPID, objects);
#endif
        }

        [Rpc(hash = (ushort)TitansiegeHashProto.CHARACTERS_LEVELUPID)]
        private void LevelupidRpc(Int32 value)//重写NetPlayer的OnStart方法来处理客户端自动同步到服务器数据库, 方法内部添加AddRpc(data(CharactersData));收集Rpc
        {
            Levelupid = value;
        }
     
        private readonly Int32Obs exp = new Int32Obs("CharactersData_exp", true, null);

        /// <summary>经验 --获得属性观察对象</summary>
        internal Int32Obs ExpObserver => exp;

        /// <summary>经验</summary>
        public Int32 Exp { get => GetExpValue(); set => CheckExpValue(value, 0); }

        /// <summary>经验 --同步到数据库</summary>
        internal Int32 SyncExp { get => GetExpValue(); set => CheckExpValue(value, 1); }

        /// <summary>经验 --同步带有Key字段的值到服务器Player对象上，需要处理</summary>
        internal Int32 SyncIDExp { get => GetExpValue(); set => CheckExpValue(value, 2); }

        private Int32 GetExpValue() => this.exp.Value;

        private void CheckExpValue(Int32 value, int type) 
        {
            if (this.exp.Value == value)
                return;
            this.exp.Value = value;
            if (type == 0)
                CheckUpdate(5);
            else if (type == 1)
                ExpCall(false);
            else if (type == 2)
                ExpCall(true);
            OnValueChanged?.Invoke(TitansiegeHashProto.CHARACTERS_EXP, value);
        }

        /// <summary>经验 --同步当前值到服务器Player对象上，需要处理</summary>
        public void ExpCall(bool syncId = false)
        {
            
            object[] objects;
            if (syncId) objects = new object[] { this[TitansiegeDBEvent.CharactersData_SyncID], exp.Value };
            else objects = new object[] { exp.Value };
#if SERVER
            CheckUpdate(5);
            TitansiegeDBEvent.OnSyncProperty?.Invoke(client, NetCmd.SyncPropertyData, (ushort)TitansiegeHashProto.CHARACTERS_EXP, objects);
#else
            TitansiegeDBEvent.Client.SendRT(NetCmd.SyncPropertyData, (ushort)TitansiegeHashProto.CHARACTERS_EXP, objects);
#endif
        }

        [Rpc(hash = (ushort)TitansiegeHashProto.CHARACTERS_EXP)]
        private void ExpRpc(Int32 value)//重写NetPlayer的OnStart方法来处理客户端自动同步到服务器数据库, 方法内部添加AddRpc(data(CharactersData));收集Rpc
        {
            Exp = value;
        }
     
        private readonly Int32Obs shengming = new Int32Obs("CharactersData_shengming", true, null);

        /// <summary>生命 --获得属性观察对象</summary>
        internal Int32Obs ShengmingObserver => shengming;

        /// <summary>生命</summary>
        public Int32 Shengming { get => GetShengmingValue(); set => CheckShengmingValue(value, 0); }

        /// <summary>生命 --同步到数据库</summary>
        internal Int32 SyncShengming { get => GetShengmingValue(); set => CheckShengmingValue(value, 1); }

        /// <summary>生命 --同步带有Key字段的值到服务器Player对象上，需要处理</summary>
        internal Int32 SyncIDShengming { get => GetShengmingValue(); set => CheckShengmingValue(value, 2); }

        private Int32 GetShengmingValue() => this.shengming.Value;

        private void CheckShengmingValue(Int32 value, int type) 
        {
            if (this.shengming.Value == value)
                return;
            this.shengming.Value = value;
            if (type == 0)
                CheckUpdate(6);
            else if (type == 1)
                ShengmingCall(false);
            else if (type == 2)
                ShengmingCall(true);
            OnValueChanged?.Invoke(TitansiegeHashProto.CHARACTERS_SHENGMING, value);
        }

        /// <summary>生命 --同步当前值到服务器Player对象上，需要处理</summary>
        public void ShengmingCall(bool syncId = false)
        {
            
            object[] objects;
            if (syncId) objects = new object[] { this[TitansiegeDBEvent.CharactersData_SyncID], shengming.Value };
            else objects = new object[] { shengming.Value };
#if SERVER
            CheckUpdate(6);
            TitansiegeDBEvent.OnSyncProperty?.Invoke(client, NetCmd.SyncPropertyData, (ushort)TitansiegeHashProto.CHARACTERS_SHENGMING, objects);
#else
            TitansiegeDBEvent.Client.SendRT(NetCmd.SyncPropertyData, (ushort)TitansiegeHashProto.CHARACTERS_SHENGMING, objects);
#endif
        }

        [Rpc(hash = (ushort)TitansiegeHashProto.CHARACTERS_SHENGMING)]
        private void ShengmingRpc(Int32 value)//重写NetPlayer的OnStart方法来处理客户端自动同步到服务器数据库, 方法内部添加AddRpc(data(CharactersData));收集Rpc
        {
            Shengming = value;
        }
     
        private readonly Int32Obs fali = new Int32Obs("CharactersData_fali", true, null);

        /// <summary>法力 --获得属性观察对象</summary>
        internal Int32Obs FaliObserver => fali;

        /// <summary>法力</summary>
        public Int32 Fali { get => GetFaliValue(); set => CheckFaliValue(value, 0); }

        /// <summary>法力 --同步到数据库</summary>
        internal Int32 SyncFali { get => GetFaliValue(); set => CheckFaliValue(value, 1); }

        /// <summary>法力 --同步带有Key字段的值到服务器Player对象上，需要处理</summary>
        internal Int32 SyncIDFali { get => GetFaliValue(); set => CheckFaliValue(value, 2); }

        private Int32 GetFaliValue() => this.fali.Value;

        private void CheckFaliValue(Int32 value, int type) 
        {
            if (this.fali.Value == value)
                return;
            this.fali.Value = value;
            if (type == 0)
                CheckUpdate(7);
            else if (type == 1)
                FaliCall(false);
            else if (type == 2)
                FaliCall(true);
            OnValueChanged?.Invoke(TitansiegeHashProto.CHARACTERS_FALI, value);
        }

        /// <summary>法力 --同步当前值到服务器Player对象上，需要处理</summary>
        public void FaliCall(bool syncId = false)
        {
            
            object[] objects;
            if (syncId) objects = new object[] { this[TitansiegeDBEvent.CharactersData_SyncID], fali.Value };
            else objects = new object[] { fali.Value };
#if SERVER
            CheckUpdate(7);
            TitansiegeDBEvent.OnSyncProperty?.Invoke(client, NetCmd.SyncPropertyData, (ushort)TitansiegeHashProto.CHARACTERS_FALI, objects);
#else
            TitansiegeDBEvent.Client.SendRT(NetCmd.SyncPropertyData, (ushort)TitansiegeHashProto.CHARACTERS_FALI, objects);
#endif
        }

        [Rpc(hash = (ushort)TitansiegeHashProto.CHARACTERS_FALI)]
        private void FaliRpc(Int32 value)//重写NetPlayer的OnStart方法来处理客户端自动同步到服务器数据库, 方法内部添加AddRpc(data(CharactersData));收集Rpc
        {
            Fali = value;
        }
     
        private readonly Int16Obs tizhi = new Int16Obs("CharactersData_tizhi", true, null);

        /// <summary>体质 --获得属性观察对象</summary>
        internal Int16Obs TizhiObserver => tizhi;

        /// <summary>体质</summary>
        public Int16 Tizhi { get => GetTizhiValue(); set => CheckTizhiValue(value, 0); }

        /// <summary>体质 --同步到数据库</summary>
        internal Int16 SyncTizhi { get => GetTizhiValue(); set => CheckTizhiValue(value, 1); }

        /// <summary>体质 --同步带有Key字段的值到服务器Player对象上，需要处理</summary>
        internal Int16 SyncIDTizhi { get => GetTizhiValue(); set => CheckTizhiValue(value, 2); }

        private Int16 GetTizhiValue() => this.tizhi.Value;

        private void CheckTizhiValue(Int16 value, int type) 
        {
            if (this.tizhi.Value == value)
                return;
            this.tizhi.Value = value;
            if (type == 0)
                CheckUpdate(8);
            else if (type == 1)
                TizhiCall(false);
            else if (type == 2)
                TizhiCall(true);
            OnValueChanged?.Invoke(TitansiegeHashProto.CHARACTERS_TIZHI, value);
        }

        /// <summary>体质 --同步当前值到服务器Player对象上，需要处理</summary>
        public void TizhiCall(bool syncId = false)
        {
            
            object[] objects;
            if (syncId) objects = new object[] { this[TitansiegeDBEvent.CharactersData_SyncID], tizhi.Value };
            else objects = new object[] { tizhi.Value };
#if SERVER
            CheckUpdate(8);
            TitansiegeDBEvent.OnSyncProperty?.Invoke(client, NetCmd.SyncPropertyData, (ushort)TitansiegeHashProto.CHARACTERS_TIZHI, objects);
#else
            TitansiegeDBEvent.Client.SendRT(NetCmd.SyncPropertyData, (ushort)TitansiegeHashProto.CHARACTERS_TIZHI, objects);
#endif
        }

        [Rpc(hash = (ushort)TitansiegeHashProto.CHARACTERS_TIZHI)]
        private void TizhiRpc(Int16 value)//重写NetPlayer的OnStart方法来处理客户端自动同步到服务器数据库, 方法内部添加AddRpc(data(CharactersData));收集Rpc
        {
            Tizhi = value;
        }
     
        private readonly Int16Obs liliang = new Int16Obs("CharactersData_liliang", true, null);

        /// <summary>力量 --获得属性观察对象</summary>
        internal Int16Obs LiliangObserver => liliang;

        /// <summary>力量</summary>
        public Int16 Liliang { get => GetLiliangValue(); set => CheckLiliangValue(value, 0); }

        /// <summary>力量 --同步到数据库</summary>
        internal Int16 SyncLiliang { get => GetLiliangValue(); set => CheckLiliangValue(value, 1); }

        /// <summary>力量 --同步带有Key字段的值到服务器Player对象上，需要处理</summary>
        internal Int16 SyncIDLiliang { get => GetLiliangValue(); set => CheckLiliangValue(value, 2); }

        private Int16 GetLiliangValue() => this.liliang.Value;

        private void CheckLiliangValue(Int16 value, int type) 
        {
            if (this.liliang.Value == value)
                return;
            this.liliang.Value = value;
            if (type == 0)
                CheckUpdate(9);
            else if (type == 1)
                LiliangCall(false);
            else if (type == 2)
                LiliangCall(true);
            OnValueChanged?.Invoke(TitansiegeHashProto.CHARACTERS_LILIANG, value);
        }

        /// <summary>力量 --同步当前值到服务器Player对象上，需要处理</summary>
        public void LiliangCall(bool syncId = false)
        {
            
            object[] objects;
            if (syncId) objects = new object[] { this[TitansiegeDBEvent.CharactersData_SyncID], liliang.Value };
            else objects = new object[] { liliang.Value };
#if SERVER
            CheckUpdate(9);
            TitansiegeDBEvent.OnSyncProperty?.Invoke(client, NetCmd.SyncPropertyData, (ushort)TitansiegeHashProto.CHARACTERS_LILIANG, objects);
#else
            TitansiegeDBEvent.Client.SendRT(NetCmd.SyncPropertyData, (ushort)TitansiegeHashProto.CHARACTERS_LILIANG, objects);
#endif
        }

        [Rpc(hash = (ushort)TitansiegeHashProto.CHARACTERS_LILIANG)]
        private void LiliangRpc(Int16 value)//重写NetPlayer的OnStart方法来处理客户端自动同步到服务器数据库, 方法内部添加AddRpc(data(CharactersData));收集Rpc
        {
            Liliang = value;
        }
     
        private readonly Int16Obs minjie = new Int16Obs("CharactersData_minjie", true, null);

        /// <summary>敏捷 --获得属性观察对象</summary>
        internal Int16Obs MinjieObserver => minjie;

        /// <summary>敏捷</summary>
        public Int16 Minjie { get => GetMinjieValue(); set => CheckMinjieValue(value, 0); }

        /// <summary>敏捷 --同步到数据库</summary>
        internal Int16 SyncMinjie { get => GetMinjieValue(); set => CheckMinjieValue(value, 1); }

        /// <summary>敏捷 --同步带有Key字段的值到服务器Player对象上，需要处理</summary>
        internal Int16 SyncIDMinjie { get => GetMinjieValue(); set => CheckMinjieValue(value, 2); }

        private Int16 GetMinjieValue() => this.minjie.Value;

        private void CheckMinjieValue(Int16 value, int type) 
        {
            if (this.minjie.Value == value)
                return;
            this.minjie.Value = value;
            if (type == 0)
                CheckUpdate(10);
            else if (type == 1)
                MinjieCall(false);
            else if (type == 2)
                MinjieCall(true);
            OnValueChanged?.Invoke(TitansiegeHashProto.CHARACTERS_MINJIE, value);
        }

        /// <summary>敏捷 --同步当前值到服务器Player对象上，需要处理</summary>
        public void MinjieCall(bool syncId = false)
        {
            
            object[] objects;
            if (syncId) objects = new object[] { this[TitansiegeDBEvent.CharactersData_SyncID], minjie.Value };
            else objects = new object[] { minjie.Value };
#if SERVER
            CheckUpdate(10);
            TitansiegeDBEvent.OnSyncProperty?.Invoke(client, NetCmd.SyncPropertyData, (ushort)TitansiegeHashProto.CHARACTERS_MINJIE, objects);
#else
            TitansiegeDBEvent.Client.SendRT(NetCmd.SyncPropertyData, (ushort)TitansiegeHashProto.CHARACTERS_MINJIE, objects);
#endif
        }

        [Rpc(hash = (ushort)TitansiegeHashProto.CHARACTERS_MINJIE)]
        private void MinjieRpc(Int16 value)//重写NetPlayer的OnStart方法来处理客户端自动同步到服务器数据库, 方法内部添加AddRpc(data(CharactersData));收集Rpc
        {
            Minjie = value;
        }
     
        private readonly Int16Obs moli = new Int16Obs("CharactersData_moli", true, null);

        /// <summary>魔力 --获得属性观察对象</summary>
        internal Int16Obs MoliObserver => moli;

        /// <summary>魔力</summary>
        public Int16 Moli { get => GetMoliValue(); set => CheckMoliValue(value, 0); }

        /// <summary>魔力 --同步到数据库</summary>
        internal Int16 SyncMoli { get => GetMoliValue(); set => CheckMoliValue(value, 1); }

        /// <summary>魔力 --同步带有Key字段的值到服务器Player对象上，需要处理</summary>
        internal Int16 SyncIDMoli { get => GetMoliValue(); set => CheckMoliValue(value, 2); }

        private Int16 GetMoliValue() => this.moli.Value;

        private void CheckMoliValue(Int16 value, int type) 
        {
            if (this.moli.Value == value)
                return;
            this.moli.Value = value;
            if (type == 0)
                CheckUpdate(11);
            else if (type == 1)
                MoliCall(false);
            else if (type == 2)
                MoliCall(true);
            OnValueChanged?.Invoke(TitansiegeHashProto.CHARACTERS_MOLI, value);
        }

        /// <summary>魔力 --同步当前值到服务器Player对象上，需要处理</summary>
        public void MoliCall(bool syncId = false)
        {
            
            object[] objects;
            if (syncId) objects = new object[] { this[TitansiegeDBEvent.CharactersData_SyncID], moli.Value };
            else objects = new object[] { moli.Value };
#if SERVER
            CheckUpdate(11);
            TitansiegeDBEvent.OnSyncProperty?.Invoke(client, NetCmd.SyncPropertyData, (ushort)TitansiegeHashProto.CHARACTERS_MOLI, objects);
#else
            TitansiegeDBEvent.Client.SendRT(NetCmd.SyncPropertyData, (ushort)TitansiegeHashProto.CHARACTERS_MOLI, objects);
#endif
        }

        [Rpc(hash = (ushort)TitansiegeHashProto.CHARACTERS_MOLI)]
        private void MoliRpc(Int16 value)//重写NetPlayer的OnStart方法来处理客户端自动同步到服务器数据库, 方法内部添加AddRpc(data(CharactersData));收集Rpc
        {
            Moli = value;
        }
     
        private readonly Int16Obs meili = new Int16Obs("CharactersData_meili", true, null);

        /// <summary>魅力 --获得属性观察对象</summary>
        internal Int16Obs MeiliObserver => meili;

        /// <summary>魅力</summary>
        public Int16 Meili { get => GetMeiliValue(); set => CheckMeiliValue(value, 0); }

        /// <summary>魅力 --同步到数据库</summary>
        internal Int16 SyncMeili { get => GetMeiliValue(); set => CheckMeiliValue(value, 1); }

        /// <summary>魅力 --同步带有Key字段的值到服务器Player对象上，需要处理</summary>
        internal Int16 SyncIDMeili { get => GetMeiliValue(); set => CheckMeiliValue(value, 2); }

        private Int16 GetMeiliValue() => this.meili.Value;

        private void CheckMeiliValue(Int16 value, int type) 
        {
            if (this.meili.Value == value)
                return;
            this.meili.Value = value;
            if (type == 0)
                CheckUpdate(12);
            else if (type == 1)
                MeiliCall(false);
            else if (type == 2)
                MeiliCall(true);
            OnValueChanged?.Invoke(TitansiegeHashProto.CHARACTERS_MEILI, value);
        }

        /// <summary>魅力 --同步当前值到服务器Player对象上，需要处理</summary>
        public void MeiliCall(bool syncId = false)
        {
            
            object[] objects;
            if (syncId) objects = new object[] { this[TitansiegeDBEvent.CharactersData_SyncID], meili.Value };
            else objects = new object[] { meili.Value };
#if SERVER
            CheckUpdate(12);
            TitansiegeDBEvent.OnSyncProperty?.Invoke(client, NetCmd.SyncPropertyData, (ushort)TitansiegeHashProto.CHARACTERS_MEILI, objects);
#else
            TitansiegeDBEvent.Client.SendRT(NetCmd.SyncPropertyData, (ushort)TitansiegeHashProto.CHARACTERS_MEILI, objects);
#endif
        }

        [Rpc(hash = (ushort)TitansiegeHashProto.CHARACTERS_MEILI)]
        private void MeiliRpc(Int16 value)//重写NetPlayer的OnStart方法来处理客户端自动同步到服务器数据库, 方法内部添加AddRpc(data(CharactersData));收集Rpc
        {
            Meili = value;
        }
     
        private readonly Int16Obs xingyun = new Int16Obs("CharactersData_xingyun", true, null);

        /// <summary>幸运 --获得属性观察对象</summary>
        internal Int16Obs XingyunObserver => xingyun;

        /// <summary>幸运</summary>
        public Int16 Xingyun { get => GetXingyunValue(); set => CheckXingyunValue(value, 0); }

        /// <summary>幸运 --同步到数据库</summary>
        internal Int16 SyncXingyun { get => GetXingyunValue(); set => CheckXingyunValue(value, 1); }

        /// <summary>幸运 --同步带有Key字段的值到服务器Player对象上，需要处理</summary>
        internal Int16 SyncIDXingyun { get => GetXingyunValue(); set => CheckXingyunValue(value, 2); }

        private Int16 GetXingyunValue() => this.xingyun.Value;

        private void CheckXingyunValue(Int16 value, int type) 
        {
            if (this.xingyun.Value == value)
                return;
            this.xingyun.Value = value;
            if (type == 0)
                CheckUpdate(13);
            else if (type == 1)
                XingyunCall(false);
            else if (type == 2)
                XingyunCall(true);
            OnValueChanged?.Invoke(TitansiegeHashProto.CHARACTERS_XINGYUN, value);
        }

        /// <summary>幸运 --同步当前值到服务器Player对象上，需要处理</summary>
        public void XingyunCall(bool syncId = false)
        {
            
            object[] objects;
            if (syncId) objects = new object[] { this[TitansiegeDBEvent.CharactersData_SyncID], xingyun.Value };
            else objects = new object[] { xingyun.Value };
#if SERVER
            CheckUpdate(13);
            TitansiegeDBEvent.OnSyncProperty?.Invoke(client, NetCmd.SyncPropertyData, (ushort)TitansiegeHashProto.CHARACTERS_XINGYUN, objects);
#else
            TitansiegeDBEvent.Client.SendRT(NetCmd.SyncPropertyData, (ushort)TitansiegeHashProto.CHARACTERS_XINGYUN, objects);
#endif
        }

        [Rpc(hash = (ushort)TitansiegeHashProto.CHARACTERS_XINGYUN)]
        private void XingyunRpc(Int16 value)//重写NetPlayer的OnStart方法来处理客户端自动同步到服务器数据库, 方法内部添加AddRpc(data(CharactersData));收集Rpc
        {
            Xingyun = value;
        }
     
        private readonly Int16Obs lianjin = new Int16Obs("CharactersData_lianjin", true, null);

        /// <summary>炼金 --获得属性观察对象</summary>
        internal Int16Obs LianjinObserver => lianjin;

        /// <summary>炼金</summary>
        public Int16 Lianjin { get => GetLianjinValue(); set => CheckLianjinValue(value, 0); }

        /// <summary>炼金 --同步到数据库</summary>
        internal Int16 SyncLianjin { get => GetLianjinValue(); set => CheckLianjinValue(value, 1); }

        /// <summary>炼金 --同步带有Key字段的值到服务器Player对象上，需要处理</summary>
        internal Int16 SyncIDLianjin { get => GetLianjinValue(); set => CheckLianjinValue(value, 2); }

        private Int16 GetLianjinValue() => this.lianjin.Value;

        private void CheckLianjinValue(Int16 value, int type) 
        {
            if (this.lianjin.Value == value)
                return;
            this.lianjin.Value = value;
            if (type == 0)
                CheckUpdate(14);
            else if (type == 1)
                LianjinCall(false);
            else if (type == 2)
                LianjinCall(true);
            OnValueChanged?.Invoke(TitansiegeHashProto.CHARACTERS_LIANJIN, value);
        }

        /// <summary>炼金 --同步当前值到服务器Player对象上，需要处理</summary>
        public void LianjinCall(bool syncId = false)
        {
            
            object[] objects;
            if (syncId) objects = new object[] { this[TitansiegeDBEvent.CharactersData_SyncID], lianjin.Value };
            else objects = new object[] { lianjin.Value };
#if SERVER
            CheckUpdate(14);
            TitansiegeDBEvent.OnSyncProperty?.Invoke(client, NetCmd.SyncPropertyData, (ushort)TitansiegeHashProto.CHARACTERS_LIANJIN, objects);
#else
            TitansiegeDBEvent.Client.SendRT(NetCmd.SyncPropertyData, (ushort)TitansiegeHashProto.CHARACTERS_LIANJIN, objects);
#endif
        }

        [Rpc(hash = (ushort)TitansiegeHashProto.CHARACTERS_LIANJIN)]
        private void LianjinRpc(Int16 value)//重写NetPlayer的OnStart方法来处理客户端自动同步到服务器数据库, 方法内部添加AddRpc(data(CharactersData));收集Rpc
        {
            Lianjin = value;
        }
     
        private readonly Int16Obs duanzao = new Int16Obs("CharactersData_duanzao", true, null);

        /// <summary>锻造 --获得属性观察对象</summary>
        internal Int16Obs DuanzaoObserver => duanzao;

        /// <summary>锻造</summary>
        public Int16 Duanzao { get => GetDuanzaoValue(); set => CheckDuanzaoValue(value, 0); }

        /// <summary>锻造 --同步到数据库</summary>
        internal Int16 SyncDuanzao { get => GetDuanzaoValue(); set => CheckDuanzaoValue(value, 1); }

        /// <summary>锻造 --同步带有Key字段的值到服务器Player对象上，需要处理</summary>
        internal Int16 SyncIDDuanzao { get => GetDuanzaoValue(); set => CheckDuanzaoValue(value, 2); }

        private Int16 GetDuanzaoValue() => this.duanzao.Value;

        private void CheckDuanzaoValue(Int16 value, int type) 
        {
            if (this.duanzao.Value == value)
                return;
            this.duanzao.Value = value;
            if (type == 0)
                CheckUpdate(15);
            else if (type == 1)
                DuanzaoCall(false);
            else if (type == 2)
                DuanzaoCall(true);
            OnValueChanged?.Invoke(TitansiegeHashProto.CHARACTERS_DUANZAO, value);
        }

        /// <summary>锻造 --同步当前值到服务器Player对象上，需要处理</summary>
        public void DuanzaoCall(bool syncId = false)
        {
            
            object[] objects;
            if (syncId) objects = new object[] { this[TitansiegeDBEvent.CharactersData_SyncID], duanzao.Value };
            else objects = new object[] { duanzao.Value };
#if SERVER
            CheckUpdate(15);
            TitansiegeDBEvent.OnSyncProperty?.Invoke(client, NetCmd.SyncPropertyData, (ushort)TitansiegeHashProto.CHARACTERS_DUANZAO, objects);
#else
            TitansiegeDBEvent.Client.SendRT(NetCmd.SyncPropertyData, (ushort)TitansiegeHashProto.CHARACTERS_DUANZAO, objects);
#endif
        }

        [Rpc(hash = (ushort)TitansiegeHashProto.CHARACTERS_DUANZAO)]
        private void DuanzaoRpc(Int16 value)//重写NetPlayer的OnStart方法来处理客户端自动同步到服务器数据库, 方法内部添加AddRpc(data(CharactersData));收集Rpc
        {
            Duanzao = value;
        }
     
        private readonly Int32Obs jinbi = new Int32Obs("CharactersData_jinbi", true, null);

        /// <summary>金币 --获得属性观察对象</summary>
        internal Int32Obs JinbiObserver => jinbi;

        /// <summary>金币</summary>
        public Int32 Jinbi { get => GetJinbiValue(); set => CheckJinbiValue(value, 0); }

        /// <summary>金币 --同步到数据库</summary>
        internal Int32 SyncJinbi { get => GetJinbiValue(); set => CheckJinbiValue(value, 1); }

        /// <summary>金币 --同步带有Key字段的值到服务器Player对象上，需要处理</summary>
        internal Int32 SyncIDJinbi { get => GetJinbiValue(); set => CheckJinbiValue(value, 2); }

        private Int32 GetJinbiValue() => this.jinbi.Value;

        private void CheckJinbiValue(Int32 value, int type) 
        {
            if (this.jinbi.Value == value)
                return;
            this.jinbi.Value = value;
            if (type == 0)
                CheckUpdate(16);
            else if (type == 1)
                JinbiCall(false);
            else if (type == 2)
                JinbiCall(true);
            OnValueChanged?.Invoke(TitansiegeHashProto.CHARACTERS_JINBI, value);
        }

        /// <summary>金币 --同步当前值到服务器Player对象上，需要处理</summary>
        public void JinbiCall(bool syncId = false)
        {
            
            object[] objects;
            if (syncId) objects = new object[] { this[TitansiegeDBEvent.CharactersData_SyncID], jinbi.Value };
            else objects = new object[] { jinbi.Value };
#if SERVER
            CheckUpdate(16);
            TitansiegeDBEvent.OnSyncProperty?.Invoke(client, NetCmd.SyncPropertyData, (ushort)TitansiegeHashProto.CHARACTERS_JINBI, objects);
#else
            TitansiegeDBEvent.Client.SendRT(NetCmd.SyncPropertyData, (ushort)TitansiegeHashProto.CHARACTERS_JINBI, objects);
#endif
        }

        [Rpc(hash = (ushort)TitansiegeHashProto.CHARACTERS_JINBI)]
        private void JinbiRpc(Int32 value)//重写NetPlayer的OnStart方法来处理客户端自动同步到服务器数据库, 方法内部添加AddRpc(data(CharactersData));收集Rpc
        {
            Jinbi = value;
        }
     
        private readonly Int32Obs zuanshi = new Int32Obs("CharactersData_zuanshi", true, null);

        /// <summary>钻石 --获得属性观察对象</summary>
        internal Int32Obs ZuanshiObserver => zuanshi;

        /// <summary>钻石</summary>
        public Int32 Zuanshi { get => GetZuanshiValue(); set => CheckZuanshiValue(value, 0); }

        /// <summary>钻石 --同步到数据库</summary>
        internal Int32 SyncZuanshi { get => GetZuanshiValue(); set => CheckZuanshiValue(value, 1); }

        /// <summary>钻石 --同步带有Key字段的值到服务器Player对象上，需要处理</summary>
        internal Int32 SyncIDZuanshi { get => GetZuanshiValue(); set => CheckZuanshiValue(value, 2); }

        private Int32 GetZuanshiValue() => this.zuanshi.Value;

        private void CheckZuanshiValue(Int32 value, int type) 
        {
            if (this.zuanshi.Value == value)
                return;
            this.zuanshi.Value = value;
            if (type == 0)
                CheckUpdate(17);
            else if (type == 1)
                ZuanshiCall(false);
            else if (type == 2)
                ZuanshiCall(true);
            OnValueChanged?.Invoke(TitansiegeHashProto.CHARACTERS_ZUANSHI, value);
        }

        /// <summary>钻石 --同步当前值到服务器Player对象上，需要处理</summary>
        public void ZuanshiCall(bool syncId = false)
        {
            
            object[] objects;
            if (syncId) objects = new object[] { this[TitansiegeDBEvent.CharactersData_SyncID], zuanshi.Value };
            else objects = new object[] { zuanshi.Value };
#if SERVER
            CheckUpdate(17);
            TitansiegeDBEvent.OnSyncProperty?.Invoke(client, NetCmd.SyncPropertyData, (ushort)TitansiegeHashProto.CHARACTERS_ZUANSHI, objects);
#else
            TitansiegeDBEvent.Client.SendRT(NetCmd.SyncPropertyData, (ushort)TitansiegeHashProto.CHARACTERS_ZUANSHI, objects);
#endif
        }

        [Rpc(hash = (ushort)TitansiegeHashProto.CHARACTERS_ZUANSHI)]
        private void ZuanshiRpc(Int32 value)//重写NetPlayer的OnStart方法来处理客户端自动同步到服务器数据库, 方法内部添加AddRpc(data(CharactersData));收集Rpc
        {
            Zuanshi = value;
        }
     
        private readonly StringObs chenghao = new StringObs("CharactersData_chenghao", false, null);

        /// <summary>称号 --获得属性观察对象</summary>
        internal StringObs ChenghaoObserver => chenghao;

        /// <summary>称号</summary>
        public String Chenghao { get => GetChenghaoValue(); set => CheckChenghaoValue(value, 0); }

        /// <summary>称号 --同步到数据库</summary>
        internal String SyncChenghao { get => GetChenghaoValue(); set => CheckChenghaoValue(value, 1); }

        /// <summary>称号 --同步带有Key字段的值到服务器Player对象上，需要处理</summary>
        internal String SyncIDChenghao { get => GetChenghaoValue(); set => CheckChenghaoValue(value, 2); }

        private String GetChenghaoValue() => this.chenghao.Value;

        private void CheckChenghaoValue(String value, int type) 
        {
            if (this.chenghao.Value == value)
                return;
            this.chenghao.Value = value;
            if (type == 0)
                CheckUpdate(18);
            else if (type == 1)
                ChenghaoCall(false);
            else if (type == 2)
                ChenghaoCall(true);
            OnValueChanged?.Invoke(TitansiegeHashProto.CHARACTERS_CHENGHAO, value);
        }

        /// <summary>称号 --同步当前值到服务器Player对象上，需要处理</summary>
        public void ChenghaoCall(bool syncId = false)
        {
            
            object[] objects;
            if (syncId) objects = new object[] { this[TitansiegeDBEvent.CharactersData_SyncID], chenghao.Value };
            else objects = new object[] { chenghao.Value };
#if SERVER
            CheckUpdate(18);
            TitansiegeDBEvent.OnSyncProperty?.Invoke(client, NetCmd.SyncPropertyData, (ushort)TitansiegeHashProto.CHARACTERS_CHENGHAO, objects);
#else
            TitansiegeDBEvent.Client.SendRT(NetCmd.SyncPropertyData, (ushort)TitansiegeHashProto.CHARACTERS_CHENGHAO, objects);
#endif
        }

        [Rpc(hash = (ushort)TitansiegeHashProto.CHARACTERS_CHENGHAO)]
        private void ChenghaoRpc(String value)//重写NetPlayer的OnStart方法来处理客户端自动同步到服务器数据库, 方法内部添加AddRpc(data(CharactersData));收集Rpc
        {
            Chenghao = value;
        }
     
        private readonly StringObs friends = new StringObs("CharactersData_friends", false, null);

        /// <summary>亲朋 --获得属性观察对象</summary>
        internal StringObs FriendsObserver => friends;

        /// <summary>亲朋</summary>
        public String Friends { get => GetFriendsValue(); set => CheckFriendsValue(value, 0); }

        /// <summary>亲朋 --同步到数据库</summary>
        internal String SyncFriends { get => GetFriendsValue(); set => CheckFriendsValue(value, 1); }

        /// <summary>亲朋 --同步带有Key字段的值到服务器Player对象上，需要处理</summary>
        internal String SyncIDFriends { get => GetFriendsValue(); set => CheckFriendsValue(value, 2); }

        private String GetFriendsValue() => this.friends.Value;

        private void CheckFriendsValue(String value, int type) 
        {
            if (this.friends.Value == value)
                return;
            this.friends.Value = value;
            if (type == 0)
                CheckUpdate(19);
            else if (type == 1)
                FriendsCall(false);
            else if (type == 2)
                FriendsCall(true);
            OnValueChanged?.Invoke(TitansiegeHashProto.CHARACTERS_FRIENDS, value);
        }

        /// <summary>亲朋 --同步当前值到服务器Player对象上，需要处理</summary>
        public void FriendsCall(bool syncId = false)
        {
            
            object[] objects;
            if (syncId) objects = new object[] { this[TitansiegeDBEvent.CharactersData_SyncID], friends.Value };
            else objects = new object[] { friends.Value };
#if SERVER
            CheckUpdate(19);
            TitansiegeDBEvent.OnSyncProperty?.Invoke(client, NetCmd.SyncPropertyData, (ushort)TitansiegeHashProto.CHARACTERS_FRIENDS, objects);
#else
            TitansiegeDBEvent.Client.SendRT(NetCmd.SyncPropertyData, (ushort)TitansiegeHashProto.CHARACTERS_FRIENDS, objects);
#endif
        }

        [Rpc(hash = (ushort)TitansiegeHashProto.CHARACTERS_FRIENDS)]
        private void FriendsRpc(String value)//重写NetPlayer的OnStart方法来处理客户端自动同步到服务器数据库, 方法内部添加AddRpc(data(CharactersData));收集Rpc
        {
            Friends = value;
        }
     
        private readonly StringObs skills = new StringObs("CharactersData_skills", false, null);

        /// <summary>技能 --获得属性观察对象</summary>
        internal StringObs SkillsObserver => skills;

        /// <summary>技能</summary>
        public String Skills { get => GetSkillsValue(); set => CheckSkillsValue(value, 0); }

        /// <summary>技能 --同步到数据库</summary>
        internal String SyncSkills { get => GetSkillsValue(); set => CheckSkillsValue(value, 1); }

        /// <summary>技能 --同步带有Key字段的值到服务器Player对象上，需要处理</summary>
        internal String SyncIDSkills { get => GetSkillsValue(); set => CheckSkillsValue(value, 2); }

        private String GetSkillsValue() => this.skills.Value;

        private void CheckSkillsValue(String value, int type) 
        {
            if (this.skills.Value == value)
                return;
            this.skills.Value = value;
            if (type == 0)
                CheckUpdate(20);
            else if (type == 1)
                SkillsCall(false);
            else if (type == 2)
                SkillsCall(true);
            OnValueChanged?.Invoke(TitansiegeHashProto.CHARACTERS_SKILLS, value);
        }

        /// <summary>技能 --同步当前值到服务器Player对象上，需要处理</summary>
        public void SkillsCall(bool syncId = false)
        {
            
            object[] objects;
            if (syncId) objects = new object[] { this[TitansiegeDBEvent.CharactersData_SyncID], skills.Value };
            else objects = new object[] { skills.Value };
#if SERVER
            CheckUpdate(20);
            TitansiegeDBEvent.OnSyncProperty?.Invoke(client, NetCmd.SyncPropertyData, (ushort)TitansiegeHashProto.CHARACTERS_SKILLS, objects);
#else
            TitansiegeDBEvent.Client.SendRT(NetCmd.SyncPropertyData, (ushort)TitansiegeHashProto.CHARACTERS_SKILLS, objects);
#endif
        }

        [Rpc(hash = (ushort)TitansiegeHashProto.CHARACTERS_SKILLS)]
        private void SkillsRpc(String value)//重写NetPlayer的OnStart方法来处理客户端自动同步到服务器数据库, 方法内部添加AddRpc(data(CharactersData));收集Rpc
        {
            Skills = value;
        }
     
        private readonly StringObs prefabpath = new StringObs("CharactersData_prefabpath", false, null);

        /// <summary>预制体路径 --获得属性观察对象</summary>
        internal StringObs PrefabpathObserver => prefabpath;

        /// <summary>预制体路径</summary>
        public String Prefabpath { get => GetPrefabpathValue(); set => CheckPrefabpathValue(value, 0); }

        /// <summary>预制体路径 --同步到数据库</summary>
        internal String SyncPrefabpath { get => GetPrefabpathValue(); set => CheckPrefabpathValue(value, 1); }

        /// <summary>预制体路径 --同步带有Key字段的值到服务器Player对象上，需要处理</summary>
        internal String SyncIDPrefabpath { get => GetPrefabpathValue(); set => CheckPrefabpathValue(value, 2); }

        private String GetPrefabpathValue() => this.prefabpath.Value;

        private void CheckPrefabpathValue(String value, int type) 
        {
            if (this.prefabpath.Value == value)
                return;
            this.prefabpath.Value = value;
            if (type == 0)
                CheckUpdate(21);
            else if (type == 1)
                PrefabpathCall(false);
            else if (type == 2)
                PrefabpathCall(true);
            OnValueChanged?.Invoke(TitansiegeHashProto.CHARACTERS_PREFABPATH, value);
        }

        /// <summary>预制体路径 --同步当前值到服务器Player对象上，需要处理</summary>
        public void PrefabpathCall(bool syncId = false)
        {
            
            object[] objects;
            if (syncId) objects = new object[] { this[TitansiegeDBEvent.CharactersData_SyncID], prefabpath.Value };
            else objects = new object[] { prefabpath.Value };
#if SERVER
            CheckUpdate(21);
            TitansiegeDBEvent.OnSyncProperty?.Invoke(client, NetCmd.SyncPropertyData, (ushort)TitansiegeHashProto.CHARACTERS_PREFABPATH, objects);
#else
            TitansiegeDBEvent.Client.SendRT(NetCmd.SyncPropertyData, (ushort)TitansiegeHashProto.CHARACTERS_PREFABPATH, objects);
#endif
        }

        [Rpc(hash = (ushort)TitansiegeHashProto.CHARACTERS_PREFABPATH)]
        private void PrefabpathRpc(String value)//重写NetPlayer的OnStart方法来处理客户端自动同步到服务器数据库, 方法内部添加AddRpc(data(CharactersData));收集Rpc
        {
            Prefabpath = value;
        }
     
        private readonly StringObs headpath = new StringObs("CharactersData_headpath", false, null);

        /// <summary>头像路径 --获得属性观察对象</summary>
        internal StringObs HeadpathObserver => headpath;

        /// <summary>头像路径</summary>
        public String Headpath { get => GetHeadpathValue(); set => CheckHeadpathValue(value, 0); }

        /// <summary>头像路径 --同步到数据库</summary>
        internal String SyncHeadpath { get => GetHeadpathValue(); set => CheckHeadpathValue(value, 1); }

        /// <summary>头像路径 --同步带有Key字段的值到服务器Player对象上，需要处理</summary>
        internal String SyncIDHeadpath { get => GetHeadpathValue(); set => CheckHeadpathValue(value, 2); }

        private String GetHeadpathValue() => this.headpath.Value;

        private void CheckHeadpathValue(String value, int type) 
        {
            if (this.headpath.Value == value)
                return;
            this.headpath.Value = value;
            if (type == 0)
                CheckUpdate(22);
            else if (type == 1)
                HeadpathCall(false);
            else if (type == 2)
                HeadpathCall(true);
            OnValueChanged?.Invoke(TitansiegeHashProto.CHARACTERS_HEADPATH, value);
        }

        /// <summary>头像路径 --同步当前值到服务器Player对象上，需要处理</summary>
        public void HeadpathCall(bool syncId = false)
        {
            
            object[] objects;
            if (syncId) objects = new object[] { this[TitansiegeDBEvent.CharactersData_SyncID], headpath.Value };
            else objects = new object[] { headpath.Value };
#if SERVER
            CheckUpdate(22);
            TitansiegeDBEvent.OnSyncProperty?.Invoke(client, NetCmd.SyncPropertyData, (ushort)TitansiegeHashProto.CHARACTERS_HEADPATH, objects);
#else
            TitansiegeDBEvent.Client.SendRT(NetCmd.SyncPropertyData, (ushort)TitansiegeHashProto.CHARACTERS_HEADPATH, objects);
#endif
        }

        [Rpc(hash = (ushort)TitansiegeHashProto.CHARACTERS_HEADPATH)]
        private void HeadpathRpc(String value)//重写NetPlayer的OnStart方法来处理客户端自动同步到服务器数据库, 方法内部添加AddRpc(data(CharactersData));收集Rpc
        {
            Headpath = value;
        }
     
        private readonly StringObs lihuipath = new StringObs("CharactersData_lihuipath", false, null);

        /// <summary>立绘路径 --获得属性观察对象</summary>
        internal StringObs LihuipathObserver => lihuipath;

        /// <summary>立绘路径</summary>
        public String Lihuipath { get => GetLihuipathValue(); set => CheckLihuipathValue(value, 0); }

        /// <summary>立绘路径 --同步到数据库</summary>
        internal String SyncLihuipath { get => GetLihuipathValue(); set => CheckLihuipathValue(value, 1); }

        /// <summary>立绘路径 --同步带有Key字段的值到服务器Player对象上，需要处理</summary>
        internal String SyncIDLihuipath { get => GetLihuipathValue(); set => CheckLihuipathValue(value, 2); }

        private String GetLihuipathValue() => this.lihuipath.Value;

        private void CheckLihuipathValue(String value, int type) 
        {
            if (this.lihuipath.Value == value)
                return;
            this.lihuipath.Value = value;
            if (type == 0)
                CheckUpdate(23);
            else if (type == 1)
                LihuipathCall(false);
            else if (type == 2)
                LihuipathCall(true);
            OnValueChanged?.Invoke(TitansiegeHashProto.CHARACTERS_LIHUIPATH, value);
        }

        /// <summary>立绘路径 --同步当前值到服务器Player对象上，需要处理</summary>
        public void LihuipathCall(bool syncId = false)
        {
            
            object[] objects;
            if (syncId) objects = new object[] { this[TitansiegeDBEvent.CharactersData_SyncID], lihuipath.Value };
            else objects = new object[] { lihuipath.Value };
#if SERVER
            CheckUpdate(23);
            TitansiegeDBEvent.OnSyncProperty?.Invoke(client, NetCmd.SyncPropertyData, (ushort)TitansiegeHashProto.CHARACTERS_LIHUIPATH, objects);
#else
            TitansiegeDBEvent.Client.SendRT(NetCmd.SyncPropertyData, (ushort)TitansiegeHashProto.CHARACTERS_LIHUIPATH, objects);
#endif
        }

        [Rpc(hash = (ushort)TitansiegeHashProto.CHARACTERS_LIHUIPATH)]
        private void LihuipathRpc(String value)//重写NetPlayer的OnStart方法来处理客户端自动同步到服务器数据库, 方法内部添加AddRpc(data(CharactersData));收集Rpc
        {
            Lihuipath = value;
        }
     
        private readonly Int16Obs wuqi = new Int16Obs("CharactersData_wuqi", true, null);

        /// <summary>武器 --获得属性观察对象</summary>
        internal Int16Obs WuqiObserver => wuqi;

        /// <summary>武器</summary>
        public Int16 Wuqi { get => GetWuqiValue(); set => CheckWuqiValue(value, 0); }

        /// <summary>武器 --同步到数据库</summary>
        internal Int16 SyncWuqi { get => GetWuqiValue(); set => CheckWuqiValue(value, 1); }

        /// <summary>武器 --同步带有Key字段的值到服务器Player对象上，需要处理</summary>
        internal Int16 SyncIDWuqi { get => GetWuqiValue(); set => CheckWuqiValue(value, 2); }

        private Int16 GetWuqiValue() => this.wuqi.Value;

        private void CheckWuqiValue(Int16 value, int type) 
        {
            if (this.wuqi.Value == value)
                return;
            this.wuqi.Value = value;
            if (type == 0)
                CheckUpdate(24);
            else if (type == 1)
                WuqiCall(false);
            else if (type == 2)
                WuqiCall(true);
            OnValueChanged?.Invoke(TitansiegeHashProto.CHARACTERS_WUQI, value);
        }

        /// <summary>武器 --同步当前值到服务器Player对象上，需要处理</summary>
        public void WuqiCall(bool syncId = false)
        {
            
            object[] objects;
            if (syncId) objects = new object[] { this[TitansiegeDBEvent.CharactersData_SyncID], wuqi.Value };
            else objects = new object[] { wuqi.Value };
#if SERVER
            CheckUpdate(24);
            TitansiegeDBEvent.OnSyncProperty?.Invoke(client, NetCmd.SyncPropertyData, (ushort)TitansiegeHashProto.CHARACTERS_WUQI, objects);
#else
            TitansiegeDBEvent.Client.SendRT(NetCmd.SyncPropertyData, (ushort)TitansiegeHashProto.CHARACTERS_WUQI, objects);
#endif
        }

        [Rpc(hash = (ushort)TitansiegeHashProto.CHARACTERS_WUQI)]
        private void WuqiRpc(Int16 value)//重写NetPlayer的OnStart方法来处理客户端自动同步到服务器数据库, 方法内部添加AddRpc(data(CharactersData));收集Rpc
        {
            Wuqi = value;
        }
     
        private readonly Int16Obs toukui = new Int16Obs("CharactersData_toukui", true, null);

        /// <summary>头盔 --获得属性观察对象</summary>
        internal Int16Obs ToukuiObserver => toukui;

        /// <summary>头盔</summary>
        public Int16 Toukui { get => GetToukuiValue(); set => CheckToukuiValue(value, 0); }

        /// <summary>头盔 --同步到数据库</summary>
        internal Int16 SyncToukui { get => GetToukuiValue(); set => CheckToukuiValue(value, 1); }

        /// <summary>头盔 --同步带有Key字段的值到服务器Player对象上，需要处理</summary>
        internal Int16 SyncIDToukui { get => GetToukuiValue(); set => CheckToukuiValue(value, 2); }

        private Int16 GetToukuiValue() => this.toukui.Value;

        private void CheckToukuiValue(Int16 value, int type) 
        {
            if (this.toukui.Value == value)
                return;
            this.toukui.Value = value;
            if (type == 0)
                CheckUpdate(25);
            else if (type == 1)
                ToukuiCall(false);
            else if (type == 2)
                ToukuiCall(true);
            OnValueChanged?.Invoke(TitansiegeHashProto.CHARACTERS_TOUKUI, value);
        }

        /// <summary>头盔 --同步当前值到服务器Player对象上，需要处理</summary>
        public void ToukuiCall(bool syncId = false)
        {
            
            object[] objects;
            if (syncId) objects = new object[] { this[TitansiegeDBEvent.CharactersData_SyncID], toukui.Value };
            else objects = new object[] { toukui.Value };
#if SERVER
            CheckUpdate(25);
            TitansiegeDBEvent.OnSyncProperty?.Invoke(client, NetCmd.SyncPropertyData, (ushort)TitansiegeHashProto.CHARACTERS_TOUKUI, objects);
#else
            TitansiegeDBEvent.Client.SendRT(NetCmd.SyncPropertyData, (ushort)TitansiegeHashProto.CHARACTERS_TOUKUI, objects);
#endif
        }

        [Rpc(hash = (ushort)TitansiegeHashProto.CHARACTERS_TOUKUI)]
        private void ToukuiRpc(Int16 value)//重写NetPlayer的OnStart方法来处理客户端自动同步到服务器数据库, 方法内部添加AddRpc(data(CharactersData));收集Rpc
        {
            Toukui = value;
        }
     
        private readonly Int16Obs yifu = new Int16Obs("CharactersData_yifu", true, null);

        /// <summary>衣服 --获得属性观察对象</summary>
        internal Int16Obs YifuObserver => yifu;

        /// <summary>衣服</summary>
        public Int16 Yifu { get => GetYifuValue(); set => CheckYifuValue(value, 0); }

        /// <summary>衣服 --同步到数据库</summary>
        internal Int16 SyncYifu { get => GetYifuValue(); set => CheckYifuValue(value, 1); }

        /// <summary>衣服 --同步带有Key字段的值到服务器Player对象上，需要处理</summary>
        internal Int16 SyncIDYifu { get => GetYifuValue(); set => CheckYifuValue(value, 2); }

        private Int16 GetYifuValue() => this.yifu.Value;

        private void CheckYifuValue(Int16 value, int type) 
        {
            if (this.yifu.Value == value)
                return;
            this.yifu.Value = value;
            if (type == 0)
                CheckUpdate(26);
            else if (type == 1)
                YifuCall(false);
            else if (type == 2)
                YifuCall(true);
            OnValueChanged?.Invoke(TitansiegeHashProto.CHARACTERS_YIFU, value);
        }

        /// <summary>衣服 --同步当前值到服务器Player对象上，需要处理</summary>
        public void YifuCall(bool syncId = false)
        {
            
            object[] objects;
            if (syncId) objects = new object[] { this[TitansiegeDBEvent.CharactersData_SyncID], yifu.Value };
            else objects = new object[] { yifu.Value };
#if SERVER
            CheckUpdate(26);
            TitansiegeDBEvent.OnSyncProperty?.Invoke(client, NetCmd.SyncPropertyData, (ushort)TitansiegeHashProto.CHARACTERS_YIFU, objects);
#else
            TitansiegeDBEvent.Client.SendRT(NetCmd.SyncPropertyData, (ushort)TitansiegeHashProto.CHARACTERS_YIFU, objects);
#endif
        }

        [Rpc(hash = (ushort)TitansiegeHashProto.CHARACTERS_YIFU)]
        private void YifuRpc(Int16 value)//重写NetPlayer的OnStart方法来处理客户端自动同步到服务器数据库, 方法内部添加AddRpc(data(CharactersData));收集Rpc
        {
            Yifu = value;
        }
     
        private readonly Int16Obs xiezi = new Int16Obs("CharactersData_xiezi", true, null);

        /// <summary>鞋子 --获得属性观察对象</summary>
        internal Int16Obs XieziObserver => xiezi;

        /// <summary>鞋子</summary>
        public Int16 Xiezi { get => GetXieziValue(); set => CheckXieziValue(value, 0); }

        /// <summary>鞋子 --同步到数据库</summary>
        internal Int16 SyncXiezi { get => GetXieziValue(); set => CheckXieziValue(value, 1); }

        /// <summary>鞋子 --同步带有Key字段的值到服务器Player对象上，需要处理</summary>
        internal Int16 SyncIDXiezi { get => GetXieziValue(); set => CheckXieziValue(value, 2); }

        private Int16 GetXieziValue() => this.xiezi.Value;

        private void CheckXieziValue(Int16 value, int type) 
        {
            if (this.xiezi.Value == value)
                return;
            this.xiezi.Value = value;
            if (type == 0)
                CheckUpdate(27);
            else if (type == 1)
                XieziCall(false);
            else if (type == 2)
                XieziCall(true);
            OnValueChanged?.Invoke(TitansiegeHashProto.CHARACTERS_XIEZI, value);
        }

        /// <summary>鞋子 --同步当前值到服务器Player对象上，需要处理</summary>
        public void XieziCall(bool syncId = false)
        {
            
            object[] objects;
            if (syncId) objects = new object[] { this[TitansiegeDBEvent.CharactersData_SyncID], xiezi.Value };
            else objects = new object[] { xiezi.Value };
#if SERVER
            CheckUpdate(27);
            TitansiegeDBEvent.OnSyncProperty?.Invoke(client, NetCmd.SyncPropertyData, (ushort)TitansiegeHashProto.CHARACTERS_XIEZI, objects);
#else
            TitansiegeDBEvent.Client.SendRT(NetCmd.SyncPropertyData, (ushort)TitansiegeHashProto.CHARACTERS_XIEZI, objects);
#endif
        }

        [Rpc(hash = (ushort)TitansiegeHashProto.CHARACTERS_XIEZI)]
        private void XieziRpc(Int16 value)//重写NetPlayer的OnStart方法来处理客户端自动同步到服务器数据库, 方法内部添加AddRpc(data(CharactersData));收集Rpc
        {
            Xiezi = value;
        }
     
        private readonly Int32Obs mapID = new Int32Obs("CharactersData_mapID", true, null);

        /// <summary> --获得属性观察对象</summary>
        internal Int32Obs MapIDObserver => mapID;

        /// <summary></summary>
        public Int32 MapID { get => GetMapIDValue(); set => CheckMapIDValue(value, 0); }

        /// <summary> --同步到数据库</summary>
        internal Int32 SyncMapID { get => GetMapIDValue(); set => CheckMapIDValue(value, 1); }

        /// <summary> --同步带有Key字段的值到服务器Player对象上，需要处理</summary>
        internal Int32 SyncIDMapID { get => GetMapIDValue(); set => CheckMapIDValue(value, 2); }

        private Int32 GetMapIDValue() => this.mapID.Value;

        private void CheckMapIDValue(Int32 value, int type) 
        {
            if (this.mapID.Value == value)
                return;
            this.mapID.Value = value;
            if (type == 0)
                CheckUpdate(28);
            else if (type == 1)
                MapIDCall(false);
            else if (type == 2)
                MapIDCall(true);
            OnValueChanged?.Invoke(TitansiegeHashProto.CHARACTERS_MAPID, value);
        }

        /// <summary> --同步当前值到服务器Player对象上，需要处理</summary>
        public void MapIDCall(bool syncId = false)
        {
            
            object[] objects;
            if (syncId) objects = new object[] { this[TitansiegeDBEvent.CharactersData_SyncID], mapID.Value };
            else objects = new object[] { mapID.Value };
#if SERVER
            CheckUpdate(28);
            TitansiegeDBEvent.OnSyncProperty?.Invoke(client, NetCmd.SyncPropertyData, (ushort)TitansiegeHashProto.CHARACTERS_MAPID, objects);
#else
            TitansiegeDBEvent.Client.SendRT(NetCmd.SyncPropertyData, (ushort)TitansiegeHashProto.CHARACTERS_MAPID, objects);
#endif
        }

        [Rpc(hash = (ushort)TitansiegeHashProto.CHARACTERS_MAPID)]
        private void MapIDRpc(Int32 value)//重写NetPlayer的OnStart方法来处理客户端自动同步到服务器数据库, 方法内部添加AddRpc(data(CharactersData));收集Rpc
        {
            MapID = value;
        }
     
        private readonly Int32Obs mapPosX = new Int32Obs("CharactersData_mapPosX", true, null);

        /// <summary> --获得属性观察对象</summary>
        internal Int32Obs MapPosXObserver => mapPosX;

        /// <summary></summary>
        public Int32 MapPosX { get => GetMapPosXValue(); set => CheckMapPosXValue(value, 0); }

        /// <summary> --同步到数据库</summary>
        internal Int32 SyncMapPosX { get => GetMapPosXValue(); set => CheckMapPosXValue(value, 1); }

        /// <summary> --同步带有Key字段的值到服务器Player对象上，需要处理</summary>
        internal Int32 SyncIDMapPosX { get => GetMapPosXValue(); set => CheckMapPosXValue(value, 2); }

        private Int32 GetMapPosXValue() => this.mapPosX.Value;

        private void CheckMapPosXValue(Int32 value, int type) 
        {
            if (this.mapPosX.Value == value)
                return;
            this.mapPosX.Value = value;
            if (type == 0)
                CheckUpdate(29);
            else if (type == 1)
                MapPosXCall(false);
            else if (type == 2)
                MapPosXCall(true);
            OnValueChanged?.Invoke(TitansiegeHashProto.CHARACTERS_MAPPOSX, value);
        }

        /// <summary> --同步当前值到服务器Player对象上，需要处理</summary>
        public void MapPosXCall(bool syncId = false)
        {
            
            object[] objects;
            if (syncId) objects = new object[] { this[TitansiegeDBEvent.CharactersData_SyncID], mapPosX.Value };
            else objects = new object[] { mapPosX.Value };
#if SERVER
            CheckUpdate(29);
            TitansiegeDBEvent.OnSyncProperty?.Invoke(client, NetCmd.SyncPropertyData, (ushort)TitansiegeHashProto.CHARACTERS_MAPPOSX, objects);
#else
            TitansiegeDBEvent.Client.SendRT(NetCmd.SyncPropertyData, (ushort)TitansiegeHashProto.CHARACTERS_MAPPOSX, objects);
#endif
        }

        [Rpc(hash = (ushort)TitansiegeHashProto.CHARACTERS_MAPPOSX)]
        private void MapPosXRpc(Int32 value)//重写NetPlayer的OnStart方法来处理客户端自动同步到服务器数据库, 方法内部添加AddRpc(data(CharactersData));收集Rpc
        {
            MapPosX = value;
        }
     
        private readonly Int32Obs mapPosY = new Int32Obs("CharactersData_mapPosY", true, null);

        /// <summary> --获得属性观察对象</summary>
        internal Int32Obs MapPosYObserver => mapPosY;

        /// <summary></summary>
        public Int32 MapPosY { get => GetMapPosYValue(); set => CheckMapPosYValue(value, 0); }

        /// <summary> --同步到数据库</summary>
        internal Int32 SyncMapPosY { get => GetMapPosYValue(); set => CheckMapPosYValue(value, 1); }

        /// <summary> --同步带有Key字段的值到服务器Player对象上，需要处理</summary>
        internal Int32 SyncIDMapPosY { get => GetMapPosYValue(); set => CheckMapPosYValue(value, 2); }

        private Int32 GetMapPosYValue() => this.mapPosY.Value;

        private void CheckMapPosYValue(Int32 value, int type) 
        {
            if (this.mapPosY.Value == value)
                return;
            this.mapPosY.Value = value;
            if (type == 0)
                CheckUpdate(30);
            else if (type == 1)
                MapPosYCall(false);
            else if (type == 2)
                MapPosYCall(true);
            OnValueChanged?.Invoke(TitansiegeHashProto.CHARACTERS_MAPPOSY, value);
        }

        /// <summary> --同步当前值到服务器Player对象上，需要处理</summary>
        public void MapPosYCall(bool syncId = false)
        {
            
            object[] objects;
            if (syncId) objects = new object[] { this[TitansiegeDBEvent.CharactersData_SyncID], mapPosY.Value };
            else objects = new object[] { mapPosY.Value };
#if SERVER
            CheckUpdate(30);
            TitansiegeDBEvent.OnSyncProperty?.Invoke(client, NetCmd.SyncPropertyData, (ushort)TitansiegeHashProto.CHARACTERS_MAPPOSY, objects);
#else
            TitansiegeDBEvent.Client.SendRT(NetCmd.SyncPropertyData, (ushort)TitansiegeHashProto.CHARACTERS_MAPPOSY, objects);
#endif
        }

        [Rpc(hash = (ushort)TitansiegeHashProto.CHARACTERS_MAPPOSY)]
        private void MapPosYRpc(Int32 value)//重写NetPlayer的OnStart方法来处理客户端自动同步到服务器数据库, 方法内部添加AddRpc(data(CharactersData));收集Rpc
        {
            MapPosY = value;
        }
     
        private readonly Int32Obs mapPosZ = new Int32Obs("CharactersData_mapPosZ", true, null);

        /// <summary> --获得属性观察对象</summary>
        internal Int32Obs MapPosZObserver => mapPosZ;

        /// <summary></summary>
        public Int32 MapPosZ { get => GetMapPosZValue(); set => CheckMapPosZValue(value, 0); }

        /// <summary> --同步到数据库</summary>
        internal Int32 SyncMapPosZ { get => GetMapPosZValue(); set => CheckMapPosZValue(value, 1); }

        /// <summary> --同步带有Key字段的值到服务器Player对象上，需要处理</summary>
        internal Int32 SyncIDMapPosZ { get => GetMapPosZValue(); set => CheckMapPosZValue(value, 2); }

        private Int32 GetMapPosZValue() => this.mapPosZ.Value;

        private void CheckMapPosZValue(Int32 value, int type) 
        {
            if (this.mapPosZ.Value == value)
                return;
            this.mapPosZ.Value = value;
            if (type == 0)
                CheckUpdate(31);
            else if (type == 1)
                MapPosZCall(false);
            else if (type == 2)
                MapPosZCall(true);
            OnValueChanged?.Invoke(TitansiegeHashProto.CHARACTERS_MAPPOSZ, value);
        }

        /// <summary> --同步当前值到服务器Player对象上，需要处理</summary>
        public void MapPosZCall(bool syncId = false)
        {
            
            object[] objects;
            if (syncId) objects = new object[] { this[TitansiegeDBEvent.CharactersData_SyncID], mapPosZ.Value };
            else objects = new object[] { mapPosZ.Value };
#if SERVER
            CheckUpdate(31);
            TitansiegeDBEvent.OnSyncProperty?.Invoke(client, NetCmd.SyncPropertyData, (ushort)TitansiegeHashProto.CHARACTERS_MAPPOSZ, objects);
#else
            TitansiegeDBEvent.Client.SendRT(NetCmd.SyncPropertyData, (ushort)TitansiegeHashProto.CHARACTERS_MAPPOSZ, objects);
#endif
        }

        [Rpc(hash = (ushort)TitansiegeHashProto.CHARACTERS_MAPPOSZ)]
        private void MapPosZRpc(Int32 value)//重写NetPlayer的OnStart方法来处理客户端自动同步到服务器数据库, 方法内部添加AddRpc(data(CharactersData));收集Rpc
        {
            MapPosZ = value;
        }
     
        private readonly Int64Obs uid = new Int64Obs("CharactersData_uid", true, null);

        /// <summary> --获得属性观察对象</summary>
        internal Int64Obs UidObserver => uid;

        /// <summary></summary>
        public Int64 Uid { get => GetUidValue(); set => CheckUidValue(value, 0); }

        /// <summary> --同步到数据库</summary>
        internal Int64 SyncUid { get => GetUidValue(); set => CheckUidValue(value, 1); }

        /// <summary> --同步带有Key字段的值到服务器Player对象上，需要处理</summary>
        internal Int64 SyncIDUid { get => GetUidValue(); set => CheckUidValue(value, 2); }

        private Int64 GetUidValue() => this.uid.Value;

        private void CheckUidValue(Int64 value, int type) 
        {
            if (this.uid.Value == value)
                return;
            this.uid.Value = value;
            if (type == 0)
                CheckUpdate(32);
            else if (type == 1)
                UidCall(false);
            else if (type == 2)
                UidCall(true);
            OnValueChanged?.Invoke(TitansiegeHashProto.CHARACTERS_UID, value);
        }

        /// <summary> --同步当前值到服务器Player对象上，需要处理</summary>
        public void UidCall(bool syncId = false)
        {
            
            object[] objects;
            if (syncId) objects = new object[] { this[TitansiegeDBEvent.CharactersData_SyncID], uid.Value };
            else objects = new object[] { uid.Value };
#if SERVER
            CheckUpdate(32);
            TitansiegeDBEvent.OnSyncProperty?.Invoke(client, NetCmd.SyncPropertyData, (ushort)TitansiegeHashProto.CHARACTERS_UID, objects);
#else
            TitansiegeDBEvent.Client.SendRT(NetCmd.SyncPropertyData, (ushort)TitansiegeHashProto.CHARACTERS_UID, objects);
#endif
        }

        [Rpc(hash = (ushort)TitansiegeHashProto.CHARACTERS_UID)]
        private void UidRpc(Int64 value)//重写NetPlayer的OnStart方法来处理客户端自动同步到服务器数据库, 方法内部添加AddRpc(data(CharactersData));收集Rpc
        {
            Uid = value;
        }
     
        private readonly DateTimeObs lastDate = new DateTimeObs("CharactersData_lastDate", false, null);

        /// <summary>最后登录时间 --获得属性观察对象</summary>
        internal DateTimeObs LastDateObserver => lastDate;

        /// <summary>最后登录时间</summary>
        public DateTime LastDate { get => GetLastDateValue(); set => CheckLastDateValue(value, 0); }

        /// <summary>最后登录时间 --同步到数据库</summary>
        internal DateTime SyncLastDate { get => GetLastDateValue(); set => CheckLastDateValue(value, 1); }

        /// <summary>最后登录时间 --同步带有Key字段的值到服务器Player对象上，需要处理</summary>
        internal DateTime SyncIDLastDate { get => GetLastDateValue(); set => CheckLastDateValue(value, 2); }

        private DateTime GetLastDateValue() => this.lastDate.Value;

        private void CheckLastDateValue(DateTime value, int type) 
        {
            if (this.lastDate.Value == value)
                return;
            this.lastDate.Value = value;
            if (type == 0)
                CheckUpdate(33);
            else if (type == 1)
                LastDateCall(false);
            else if (type == 2)
                LastDateCall(true);
            OnValueChanged?.Invoke(TitansiegeHashProto.CHARACTERS_LASTDATE, value);
        }

        /// <summary>最后登录时间 --同步当前值到服务器Player对象上，需要处理</summary>
        public void LastDateCall(bool syncId = false)
        {
            
            object[] objects;
            if (syncId) objects = new object[] { this[TitansiegeDBEvent.CharactersData_SyncID], lastDate.Value };
            else objects = new object[] { lastDate.Value };
#if SERVER
            CheckUpdate(33);
            TitansiegeDBEvent.OnSyncProperty?.Invoke(client, NetCmd.SyncPropertyData, (ushort)TitansiegeHashProto.CHARACTERS_LASTDATE, objects);
#else
            TitansiegeDBEvent.Client.SendRT(NetCmd.SyncPropertyData, (ushort)TitansiegeHashProto.CHARACTERS_LASTDATE, objects);
#endif
        }

        [Rpc(hash = (ushort)TitansiegeHashProto.CHARACTERS_LASTDATE)]
        private void LastDateRpc(DateTime value)//重写NetPlayer的OnStart方法来处理客户端自动同步到服务器数据库, 方法内部添加AddRpc(data(CharactersData));收集Rpc
        {
            LastDate = value;
        }
     
        private readonly BooleanObs delRole = new BooleanObs("CharactersData_delRole", false, null);

        /// <summary>是否删除 --获得属性观察对象</summary>
        internal BooleanObs DelRoleObserver => delRole;

        /// <summary>是否删除</summary>
        public Boolean DelRole { get => GetDelRoleValue(); set => CheckDelRoleValue(value, 0); }

        /// <summary>是否删除 --同步到数据库</summary>
        internal Boolean SyncDelRole { get => GetDelRoleValue(); set => CheckDelRoleValue(value, 1); }

        /// <summary>是否删除 --同步带有Key字段的值到服务器Player对象上，需要处理</summary>
        internal Boolean SyncIDDelRole { get => GetDelRoleValue(); set => CheckDelRoleValue(value, 2); }

        private Boolean GetDelRoleValue() => this.delRole.Value;

        private void CheckDelRoleValue(Boolean value, int type) 
        {
            if (this.delRole.Value == value)
                return;
            this.delRole.Value = value;
            if (type == 0)
                CheckUpdate(34);
            else if (type == 1)
                DelRoleCall(false);
            else if (type == 2)
                DelRoleCall(true);
            OnValueChanged?.Invoke(TitansiegeHashProto.CHARACTERS_DELROLE, value);
        }

        /// <summary>是否删除 --同步当前值到服务器Player对象上，需要处理</summary>
        public void DelRoleCall(bool syncId = false)
        {
            
            object[] objects;
            if (syncId) objects = new object[] { this[TitansiegeDBEvent.CharactersData_SyncID], delRole.Value };
            else objects = new object[] { delRole.Value };
#if SERVER
            CheckUpdate(34);
            TitansiegeDBEvent.OnSyncProperty?.Invoke(client, NetCmd.SyncPropertyData, (ushort)TitansiegeHashProto.CHARACTERS_DELROLE, objects);
#else
            TitansiegeDBEvent.Client.SendRT(NetCmd.SyncPropertyData, (ushort)TitansiegeHashProto.CHARACTERS_DELROLE, objects);
#endif
        }

        [Rpc(hash = (ushort)TitansiegeHashProto.CHARACTERS_DELROLE)]
        private void DelRoleRpc(Boolean value)//重写NetPlayer的OnStart方法来处理客户端自动同步到服务器数据库, 方法内部添加AddRpc(data(CharactersData));收集Rpc
        {
            DelRole = value;
        }
     

        public CharactersData() { }

    #if SERVER
        public CharactersData(params object[] parms) : base()
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
     
                case 0: length = 0; return "iD";
     
                case 1: length = 50; return "name";
     
                case 2: length = 0; return "zhiye";
     
                case 3: length = 0; return "level";
     
                case 4: length = 0; return "levelupid";
     
                case 5: length = 0; return "exp";
     
                case 6: length = 0; return "shengming";
     
                case 7: length = 0; return "fali";
     
                case 8: length = 0; return "tizhi";
     
                case 9: length = 0; return "liliang";
     
                case 10: length = 0; return "minjie";
     
                case 11: length = 0; return "moli";
     
                case 12: length = 0; return "meili";
     
                case 13: length = 0; return "xingyun";
     
                case 14: length = 0; return "lianjin";
     
                case 15: length = 0; return "duanzao";
     
                case 16: length = 0; return "jinbi";
     
                case 17: length = 0; return "zuanshi";
     
                case 18: length = 50; return "chenghao";
     
                case 19: length = 400; return "friends";
     
                case 20: length = 200; return "skills";
     
                case 21: length = 100; return "prefabpath";
     
                case 22: length = 100; return "headpath";
     
                case 23: length = 100; return "lihuipath";
     
                case 24: length = 0; return "wuqi";
     
                case 25: length = 0; return "toukui";
     
                case 26: length = 0; return "yifu";
     
                case 27: length = 0; return "xiezi";
     
                case 28: length = 0; return "mapID";
     
                case 29: length = 0; return "mapPosX";
     
                case 30: length = 0; return "mapPosY";
     
                case 31: length = 0; return "mapPosZ";
     
                case 32: length = 0; return "uid";
     
                case 33: length = 0; return "lastDate";
     
                case 34: length = 0; return "delRole";
     
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
     
                    case 0: return this.iD;
     
                    case 1: return this.name.Value;
     
                    case 2: return this.zhiye.Value;
     
                    case 3: return this.level.Value;
     
                    case 4: return this.levelupid.Value;
     
                    case 5: return this.exp.Value;
     
                    case 6: return this.shengming.Value;
     
                    case 7: return this.fali.Value;
     
                    case 8: return this.tizhi.Value;
     
                    case 9: return this.liliang.Value;
     
                    case 10: return this.minjie.Value;
     
                    case 11: return this.moli.Value;
     
                    case 12: return this.meili.Value;
     
                    case 13: return this.xingyun.Value;
     
                    case 14: return this.lianjin.Value;
     
                    case 15: return this.duanzao.Value;
     
                    case 16: return this.jinbi.Value;
     
                    case 17: return this.zuanshi.Value;
     
                    case 18: return this.chenghao.Value;
     
                    case 19: return this.friends.Value;
     
                    case 20: return this.skills.Value;
     
                    case 21: return this.prefabpath.Value;
     
                    case 22: return this.headpath.Value;
     
                    case 23: return this.lihuipath.Value;
     
                    case 24: return this.wuqi.Value;
     
                    case 25: return this.toukui.Value;
     
                    case 26: return this.yifu.Value;
     
                    case 27: return this.xiezi.Value;
     
                    case 28: return this.mapID.Value;
     
                    case 29: return this.mapPosX.Value;
     
                    case 30: return this.mapPosY.Value;
     
                    case 31: return this.mapPosZ.Value;
     
                    case 32: return this.uid.Value;
     
                    case 33: return this.lastDate.Value;
     
                    case 34: return this.delRole.Value;
     
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
                        CheckNameValue((String)value, -1);
                        break;
     
                    case 2:
                        CheckZhiyeValue((SByte)value, -1);
                        break;
     
                    case 3:
                        CheckLevelValue((SByte)value, -1);
                        break;
     
                    case 4:
                        CheckLevelupidValue((Int32)value, -1);
                        break;
     
                    case 5:
                        CheckExpValue((Int32)value, -1);
                        break;
     
                    case 6:
                        CheckShengmingValue((Int32)value, -1);
                        break;
     
                    case 7:
                        CheckFaliValue((Int32)value, -1);
                        break;
     
                    case 8:
                        CheckTizhiValue((Int16)value, -1);
                        break;
     
                    case 9:
                        CheckLiliangValue((Int16)value, -1);
                        break;
     
                    case 10:
                        CheckMinjieValue((Int16)value, -1);
                        break;
     
                    case 11:
                        CheckMoliValue((Int16)value, -1);
                        break;
     
                    case 12:
                        CheckMeiliValue((Int16)value, -1);
                        break;
     
                    case 13:
                        CheckXingyunValue((Int16)value, -1);
                        break;
     
                    case 14:
                        CheckLianjinValue((Int16)value, -1);
                        break;
     
                    case 15:
                        CheckDuanzaoValue((Int16)value, -1);
                        break;
     
                    case 16:
                        CheckJinbiValue((Int32)value, -1);
                        break;
     
                    case 17:
                        CheckZuanshiValue((Int32)value, -1);
                        break;
     
                    case 18:
                        CheckChenghaoValue((String)value, -1);
                        break;
     
                    case 19:
                        CheckFriendsValue((String)value, -1);
                        break;
     
                    case 20:
                        CheckSkillsValue((String)value, -1);
                        break;
     
                    case 21:
                        CheckPrefabpathValue((String)value, -1);
                        break;
     
                    case 22:
                        CheckHeadpathValue((String)value, -1);
                        break;
     
                    case 23:
                        CheckLihuipathValue((String)value, -1);
                        break;
     
                    case 24:
                        CheckWuqiValue((Int16)value, -1);
                        break;
     
                    case 25:
                        CheckToukuiValue((Int16)value, -1);
                        break;
     
                    case 26:
                        CheckYifuValue((Int16)value, -1);
                        break;
     
                    case 27:
                        CheckXieziValue((Int16)value, -1);
                        break;
     
                    case 28:
                        CheckMapIDValue((Int32)value, -1);
                        break;
     
                    case 29:
                        CheckMapPosXValue((Int32)value, -1);
                        break;
     
                    case 30:
                        CheckMapPosYValue((Int32)value, -1);
                        break;
     
                    case 31:
                        CheckMapPosZValue((Int32)value, -1);
                        break;
     
                    case 32:
                        CheckUidValue((Int64)value, -1);
                        break;
     
                    case 33:
                        CheckLastDateValue((DateTime)value, -1);
                        break;
     
                    case 34:
                        CheckDelRoleValue((Boolean)value, -1);
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
            var data = TitansiegeDB.I.ExecuteQuery<CharactersData>(cmdText);
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
        public static async UniTask<CharactersData> QueryAsync(string filterExpression)
        {
            var cmdText = $"select * from characters where {filterExpression}; ";
            var data = await TitansiegeDB.I.ExecuteQueryAsync<CharactersData>(cmdText);
            return data;
        }
        public static CharactersData[] QueryList(string filterExpression)
        {
            var cmdText = $"select * from characters where {filterExpression}; ";
            var datas = TitansiegeDB.I.ExecuteQueryList<CharactersData>(cmdText);
            return datas;
        }
        public static async UniTask<CharactersData[]> QueryListAsync(string filterExpression)
        {
            var cmdText = $"select * from characters where {filterExpression}; ";
            var datas = await TitansiegeDB.I.ExecuteQueryListAsync<CharactersData>(cmdText);
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
     
            if (row[0] is Int64 iD)
                this.iD = iD;
     
            if (row[1] is String name)
                CheckNameValue(name, -1);
     
            if (row[2] is SByte zhiye)
                CheckZhiyeValue(zhiye, -1);
     
            if (row[3] is SByte level)
                CheckLevelValue(level, -1);
     
            if (row[4] is Int32 levelupid)
                CheckLevelupidValue(levelupid, -1);
     
            if (row[5] is Int32 exp)
                CheckExpValue(exp, -1);
     
            if (row[6] is Int32 shengming)
                CheckShengmingValue(shengming, -1);
     
            if (row[7] is Int32 fali)
                CheckFaliValue(fali, -1);
     
            if (row[8] is Int16 tizhi)
                CheckTizhiValue(tizhi, -1);
     
            if (row[9] is Int16 liliang)
                CheckLiliangValue(liliang, -1);
     
            if (row[10] is Int16 minjie)
                CheckMinjieValue(minjie, -1);
     
            if (row[11] is Int16 moli)
                CheckMoliValue(moli, -1);
     
            if (row[12] is Int16 meili)
                CheckMeiliValue(meili, -1);
     
            if (row[13] is Int16 xingyun)
                CheckXingyunValue(xingyun, -1);
     
            if (row[14] is Int16 lianjin)
                CheckLianjinValue(lianjin, -1);
     
            if (row[15] is Int16 duanzao)
                CheckDuanzaoValue(duanzao, -1);
     
            if (row[16] is Int32 jinbi)
                CheckJinbiValue(jinbi, -1);
     
            if (row[17] is Int32 zuanshi)
                CheckZuanshiValue(zuanshi, -1);
     
            if (row[18] is String chenghao)
                CheckChenghaoValue(chenghao, -1);
     
            if (row[19] is String friends)
                CheckFriendsValue(friends, -1);
     
            if (row[20] is String skills)
                CheckSkillsValue(skills, -1);
     
            if (row[21] is String prefabpath)
                CheckPrefabpathValue(prefabpath, -1);
     
            if (row[22] is String headpath)
                CheckHeadpathValue(headpath, -1);
     
            if (row[23] is String lihuipath)
                CheckLihuipathValue(lihuipath, -1);
     
            if (row[24] is Int16 wuqi)
                CheckWuqiValue(wuqi, -1);
     
            if (row[25] is Int16 toukui)
                CheckToukuiValue(toukui, -1);
     
            if (row[26] is Int16 yifu)
                CheckYifuValue(yifu, -1);
     
            if (row[27] is Int16 xiezi)
                CheckXieziValue(xiezi, -1);
     
            if (row[28] is Int32 mapID)
                CheckMapIDValue(mapID, -1);
     
            if (row[29] is Int32 mapPosX)
                CheckMapPosXValue(mapPosX, -1);
     
            if (row[30] is Int32 mapPosY)
                CheckMapPosYValue(mapPosY, -1);
     
            if (row[31] is Int32 mapPosZ)
                CheckMapPosZValue(mapPosZ, -1);
     
            if (row[32] is Int64 uid)
                CheckUidValue(uid, -1);
     
            if (row[33] is DateTime lastDate)
                CheckLastDateValue(lastDate, -1);
     
            if (row[34] is Boolean delRole)
                CheckDelRoleValue(delRole, -1);
     
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
            string cmdText = $"DELETE FROM characters {CheckSqlKey(0, iD)}";
            sb.Append(cmdText);
            RowState = DataRowState.Deleted;
    #endif
        }

    #if SERVER
        public void BulkLoaderBuilder(StringBuilder sb)
        {
 
            for (int i = 0; i < 35; i++)
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
                        NDebug.LogError($"characters表{name}列长度溢出!");
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
            return $"ID:{ID} Name:{Name} Zhiye:{Zhiye} Level:{Level} Levelupid:{Levelupid} Exp:{Exp} Shengming:{Shengming} Fali:{Fali} Tizhi:{Tizhi} Liliang:{Liliang} Minjie:{Minjie} Moli:{Moli} Meili:{Meili} Xingyun:{Xingyun} Lianjin:{Lianjin} Duanzao:{Duanzao} Jinbi:{Jinbi} Zuanshi:{Zuanshi} Chenghao:{Chenghao} Friends:{Friends} Skills:{Skills} Prefabpath:{Prefabpath} Headpath:{Headpath} Lihuipath:{Lihuipath} Wuqi:{Wuqi} Toukui:{Toukui} Yifu:{Yifu} Xiezi:{Xiezi} MapID:{MapID} MapPosX:{MapPosX} MapPosY:{MapPosY} MapPosZ:{MapPosZ} Uid:{Uid} LastDate:{LastDate} DelRole:{DelRole} ";
        }
    }
}
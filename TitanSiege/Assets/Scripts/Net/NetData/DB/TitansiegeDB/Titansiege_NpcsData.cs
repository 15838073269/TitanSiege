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
    public partial class NpcsData : IDataRow
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

     //1
        private readonly SByteObs zhiye = new SByteObs("NpcsData_zhiye", true, null);

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
                CheckUpdate(1);
            else if (type == 1)
                ZhiyeCall(false);
            else if (type == 2)
                ZhiyeCall(true);
            OnValueChanged?.Invoke(TitansiegeHashProto.NPCS_ZHIYE, value);
        }

        /// <summary>职业 --同步当前值到服务器Player对象上，需要处理</summary>
        public void ZhiyeCall(bool syncId = false)
        {
            
            object[] objects;
            if (syncId) objects = new object[] { this[TitansiegeDBEvent.NpcsData_SyncID], zhiye.Value };
            else objects = new object[] { zhiye.Value };
#if SERVER
            CheckUpdate(1);
            TitansiegeDBEvent.OnSyncProperty?.Invoke(client, NetCmd.SyncPropertyData, (ushort)TitansiegeHashProto.NPCS_ZHIYE, objects);
#else
            TitansiegeDBEvent.Client.SendRT(NetCmd.SyncPropertyData, (ushort)TitansiegeHashProto.NPCS_ZHIYE, objects);
#endif
        }

        [Rpc(hash = (ushort)TitansiegeHashProto.NPCS_ZHIYE)]
        private void ZhiyeRpc(SByte value)//重写NetPlayer的OnStart方法来处理客户端自动同步到服务器数据库, 方法内部添加AddRpc(data(NpcsData));收集Rpc
        {
            Zhiye = value;
        }
     //1
        private readonly SByteObs level = new SByteObs("NpcsData_level", true, null);

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
                CheckUpdate(2);
            else if (type == 1)
                LevelCall(false);
            else if (type == 2)
                LevelCall(true);
            OnValueChanged?.Invoke(TitansiegeHashProto.NPCS_LEVEL, value);
        }

        /// <summary>等级 --同步当前值到服务器Player对象上，需要处理</summary>
        public void LevelCall(bool syncId = false)
        {
            
            object[] objects;
            if (syncId) objects = new object[] { this[TitansiegeDBEvent.NpcsData_SyncID], level.Value };
            else objects = new object[] { level.Value };
#if SERVER
            CheckUpdate(2);
            TitansiegeDBEvent.OnSyncProperty?.Invoke(client, NetCmd.SyncPropertyData, (ushort)TitansiegeHashProto.NPCS_LEVEL, objects);
#else
            TitansiegeDBEvent.Client.SendRT(NetCmd.SyncPropertyData, (ushort)TitansiegeHashProto.NPCS_LEVEL, objects);
#endif
        }

        [Rpc(hash = (ushort)TitansiegeHashProto.NPCS_LEVEL)]
        private void LevelRpc(SByte value)//重写NetPlayer的OnStart方法来处理客户端自动同步到服务器数据库, 方法内部添加AddRpc(data(NpcsData));收集Rpc
        {
            Level = value;
        }
     //1
        private readonly Int32Obs exp = new Int32Obs("NpcsData_exp", true, null);

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
                CheckUpdate(3);
            else if (type == 1)
                ExpCall(false);
            else if (type == 2)
                ExpCall(true);
            OnValueChanged?.Invoke(TitansiegeHashProto.NPCS_EXP, value);
        }

        /// <summary>经验 --同步当前值到服务器Player对象上，需要处理</summary>
        public void ExpCall(bool syncId = false)
        {
            
            object[] objects;
            if (syncId) objects = new object[] { this[TitansiegeDBEvent.NpcsData_SyncID], exp.Value };
            else objects = new object[] { exp.Value };
#if SERVER
            CheckUpdate(3);
            TitansiegeDBEvent.OnSyncProperty?.Invoke(client, NetCmd.SyncPropertyData, (ushort)TitansiegeHashProto.NPCS_EXP, objects);
#else
            TitansiegeDBEvent.Client.SendRT(NetCmd.SyncPropertyData, (ushort)TitansiegeHashProto.NPCS_EXP, objects);
#endif
        }

        [Rpc(hash = (ushort)TitansiegeHashProto.NPCS_EXP)]
        private void ExpRpc(Int32 value)//重写NetPlayer的OnStart方法来处理客户端自动同步到服务器数据库, 方法内部添加AddRpc(data(NpcsData));收集Rpc
        {
            Exp = value;
        }
     //1
        private readonly Int32Obs shengming = new Int32Obs("NpcsData_shengming", true, null);

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
                CheckUpdate(4);
            else if (type == 1)
                ShengmingCall(false);
            else if (type == 2)
                ShengmingCall(true);
            OnValueChanged?.Invoke(TitansiegeHashProto.NPCS_SHENGMING, value);
        }

        /// <summary>生命 --同步当前值到服务器Player对象上，需要处理</summary>
        public void ShengmingCall(bool syncId = false)
        {
            
            object[] objects;
            if (syncId) objects = new object[] { this[TitansiegeDBEvent.NpcsData_SyncID], shengming.Value };
            else objects = new object[] { shengming.Value };
#if SERVER
            CheckUpdate(4);
            TitansiegeDBEvent.OnSyncProperty?.Invoke(client, NetCmd.SyncPropertyData, (ushort)TitansiegeHashProto.NPCS_SHENGMING, objects);
#else
            TitansiegeDBEvent.Client.SendRT(NetCmd.SyncPropertyData, (ushort)TitansiegeHashProto.NPCS_SHENGMING, objects);
#endif
        }

        [Rpc(hash = (ushort)TitansiegeHashProto.NPCS_SHENGMING)]
        private void ShengmingRpc(Int32 value)//重写NetPlayer的OnStart方法来处理客户端自动同步到服务器数据库, 方法内部添加AddRpc(data(NpcsData));收集Rpc
        {
            Shengming = value;
        }
     //1
        private readonly Int32Obs fali = new Int32Obs("NpcsData_fali", true, null);

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
                CheckUpdate(5);
            else if (type == 1)
                FaliCall(false);
            else if (type == 2)
                FaliCall(true);
            OnValueChanged?.Invoke(TitansiegeHashProto.NPCS_FALI, value);
        }

        /// <summary>法力 --同步当前值到服务器Player对象上，需要处理</summary>
        public void FaliCall(bool syncId = false)
        {
            
            object[] objects;
            if (syncId) objects = new object[] { this[TitansiegeDBEvent.NpcsData_SyncID], fali.Value };
            else objects = new object[] { fali.Value };
#if SERVER
            CheckUpdate(5);
            TitansiegeDBEvent.OnSyncProperty?.Invoke(client, NetCmd.SyncPropertyData, (ushort)TitansiegeHashProto.NPCS_FALI, objects);
#else
            TitansiegeDBEvent.Client.SendRT(NetCmd.SyncPropertyData, (ushort)TitansiegeHashProto.NPCS_FALI, objects);
#endif
        }

        [Rpc(hash = (ushort)TitansiegeHashProto.NPCS_FALI)]
        private void FaliRpc(Int32 value)//重写NetPlayer的OnStart方法来处理客户端自动同步到服务器数据库, 方法内部添加AddRpc(data(NpcsData));收集Rpc
        {
            Fali = value;
        }
     //1
        private readonly Int16Obs tizhi = new Int16Obs("NpcsData_tizhi", true, null);

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
                CheckUpdate(6);
            else if (type == 1)
                TizhiCall(false);
            else if (type == 2)
                TizhiCall(true);
            OnValueChanged?.Invoke(TitansiegeHashProto.NPCS_TIZHI, value);
        }

        /// <summary>体质 --同步当前值到服务器Player对象上，需要处理</summary>
        public void TizhiCall(bool syncId = false)
        {
            
            object[] objects;
            if (syncId) objects = new object[] { this[TitansiegeDBEvent.NpcsData_SyncID], tizhi.Value };
            else objects = new object[] { tizhi.Value };
#if SERVER
            CheckUpdate(6);
            TitansiegeDBEvent.OnSyncProperty?.Invoke(client, NetCmd.SyncPropertyData, (ushort)TitansiegeHashProto.NPCS_TIZHI, objects);
#else
            TitansiegeDBEvent.Client.SendRT(NetCmd.SyncPropertyData, (ushort)TitansiegeHashProto.NPCS_TIZHI, objects);
#endif
        }

        [Rpc(hash = (ushort)TitansiegeHashProto.NPCS_TIZHI)]
        private void TizhiRpc(Int16 value)//重写NetPlayer的OnStart方法来处理客户端自动同步到服务器数据库, 方法内部添加AddRpc(data(NpcsData));收集Rpc
        {
            Tizhi = value;
        }
     //1
        private readonly Int16Obs liliang = new Int16Obs("NpcsData_liliang", true, null);

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
                CheckUpdate(7);
            else if (type == 1)
                LiliangCall(false);
            else if (type == 2)
                LiliangCall(true);
            OnValueChanged?.Invoke(TitansiegeHashProto.NPCS_LILIANG, value);
        }

        /// <summary>力量 --同步当前值到服务器Player对象上，需要处理</summary>
        public void LiliangCall(bool syncId = false)
        {
            
            object[] objects;
            if (syncId) objects = new object[] { this[TitansiegeDBEvent.NpcsData_SyncID], liliang.Value };
            else objects = new object[] { liliang.Value };
#if SERVER
            CheckUpdate(7);
            TitansiegeDBEvent.OnSyncProperty?.Invoke(client, NetCmd.SyncPropertyData, (ushort)TitansiegeHashProto.NPCS_LILIANG, objects);
#else
            TitansiegeDBEvent.Client.SendRT(NetCmd.SyncPropertyData, (ushort)TitansiegeHashProto.NPCS_LILIANG, objects);
#endif
        }

        [Rpc(hash = (ushort)TitansiegeHashProto.NPCS_LILIANG)]
        private void LiliangRpc(Int16 value)//重写NetPlayer的OnStart方法来处理客户端自动同步到服务器数据库, 方法内部添加AddRpc(data(NpcsData));收集Rpc
        {
            Liliang = value;
        }
     //1
        private readonly Int16Obs minjie = new Int16Obs("NpcsData_minjie", true, null);

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
                CheckUpdate(8);
            else if (type == 1)
                MinjieCall(false);
            else if (type == 2)
                MinjieCall(true);
            OnValueChanged?.Invoke(TitansiegeHashProto.NPCS_MINJIE, value);
        }

        /// <summary>敏捷 --同步当前值到服务器Player对象上，需要处理</summary>
        public void MinjieCall(bool syncId = false)
        {
            
            object[] objects;
            if (syncId) objects = new object[] { this[TitansiegeDBEvent.NpcsData_SyncID], minjie.Value };
            else objects = new object[] { minjie.Value };
#if SERVER
            CheckUpdate(8);
            TitansiegeDBEvent.OnSyncProperty?.Invoke(client, NetCmd.SyncPropertyData, (ushort)TitansiegeHashProto.NPCS_MINJIE, objects);
#else
            TitansiegeDBEvent.Client.SendRT(NetCmd.SyncPropertyData, (ushort)TitansiegeHashProto.NPCS_MINJIE, objects);
#endif
        }

        [Rpc(hash = (ushort)TitansiegeHashProto.NPCS_MINJIE)]
        private void MinjieRpc(Int16 value)//重写NetPlayer的OnStart方法来处理客户端自动同步到服务器数据库, 方法内部添加AddRpc(data(NpcsData));收集Rpc
        {
            Minjie = value;
        }
     //1
        private readonly Int16Obs moli = new Int16Obs("NpcsData_moli", true, null);

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
                CheckUpdate(9);
            else if (type == 1)
                MoliCall(false);
            else if (type == 2)
                MoliCall(true);
            OnValueChanged?.Invoke(TitansiegeHashProto.NPCS_MOLI, value);
        }

        /// <summary>魔力 --同步当前值到服务器Player对象上，需要处理</summary>
        public void MoliCall(bool syncId = false)
        {
            
            object[] objects;
            if (syncId) objects = new object[] { this[TitansiegeDBEvent.NpcsData_SyncID], moli.Value };
            else objects = new object[] { moli.Value };
#if SERVER
            CheckUpdate(9);
            TitansiegeDBEvent.OnSyncProperty?.Invoke(client, NetCmd.SyncPropertyData, (ushort)TitansiegeHashProto.NPCS_MOLI, objects);
#else
            TitansiegeDBEvent.Client.SendRT(NetCmd.SyncPropertyData, (ushort)TitansiegeHashProto.NPCS_MOLI, objects);
#endif
        }

        [Rpc(hash = (ushort)TitansiegeHashProto.NPCS_MOLI)]
        private void MoliRpc(Int16 value)//重写NetPlayer的OnStart方法来处理客户端自动同步到服务器数据库, 方法内部添加AddRpc(data(NpcsData));收集Rpc
        {
            Moli = value;
        }
     //1
        private readonly Int16Obs meili = new Int16Obs("NpcsData_meili", true, null);

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
                CheckUpdate(10);
            else if (type == 1)
                MeiliCall(false);
            else if (type == 2)
                MeiliCall(true);
            OnValueChanged?.Invoke(TitansiegeHashProto.NPCS_MEILI, value);
        }

        /// <summary>魅力 --同步当前值到服务器Player对象上，需要处理</summary>
        public void MeiliCall(bool syncId = false)
        {
            
            object[] objects;
            if (syncId) objects = new object[] { this[TitansiegeDBEvent.NpcsData_SyncID], meili.Value };
            else objects = new object[] { meili.Value };
#if SERVER
            CheckUpdate(10);
            TitansiegeDBEvent.OnSyncProperty?.Invoke(client, NetCmd.SyncPropertyData, (ushort)TitansiegeHashProto.NPCS_MEILI, objects);
#else
            TitansiegeDBEvent.Client.SendRT(NetCmd.SyncPropertyData, (ushort)TitansiegeHashProto.NPCS_MEILI, objects);
#endif
        }

        [Rpc(hash = (ushort)TitansiegeHashProto.NPCS_MEILI)]
        private void MeiliRpc(Int16 value)//重写NetPlayer的OnStart方法来处理客户端自动同步到服务器数据库, 方法内部添加AddRpc(data(NpcsData));收集Rpc
        {
            Meili = value;
        }
     //1
        private readonly Int16Obs xingyun = new Int16Obs("NpcsData_xingyun", true, null);

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
                CheckUpdate(11);
            else if (type == 1)
                XingyunCall(false);
            else if (type == 2)
                XingyunCall(true);
            OnValueChanged?.Invoke(TitansiegeHashProto.NPCS_XINGYUN, value);
        }

        /// <summary>幸运 --同步当前值到服务器Player对象上，需要处理</summary>
        public void XingyunCall(bool syncId = false)
        {
            
            object[] objects;
            if (syncId) objects = new object[] { this[TitansiegeDBEvent.NpcsData_SyncID], xingyun.Value };
            else objects = new object[] { xingyun.Value };
#if SERVER
            CheckUpdate(11);
            TitansiegeDBEvent.OnSyncProperty?.Invoke(client, NetCmd.SyncPropertyData, (ushort)TitansiegeHashProto.NPCS_XINGYUN, objects);
#else
            TitansiegeDBEvent.Client.SendRT(NetCmd.SyncPropertyData, (ushort)TitansiegeHashProto.NPCS_XINGYUN, objects);
#endif
        }

        [Rpc(hash = (ushort)TitansiegeHashProto.NPCS_XINGYUN)]
        private void XingyunRpc(Int16 value)//重写NetPlayer的OnStart方法来处理客户端自动同步到服务器数据库, 方法内部添加AddRpc(data(NpcsData));收集Rpc
        {
            Xingyun = value;
        }
     //1
        private readonly Int32Obs jinbi = new Int32Obs("NpcsData_jinbi", true, null);

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
                CheckUpdate(12);
            else if (type == 1)
                JinbiCall(false);
            else if (type == 2)
                JinbiCall(true);
            OnValueChanged?.Invoke(TitansiegeHashProto.NPCS_JINBI, value);
        }

        /// <summary>金币 --同步当前值到服务器Player对象上，需要处理</summary>
        public void JinbiCall(bool syncId = false)
        {
            
            object[] objects;
            if (syncId) objects = new object[] { this[TitansiegeDBEvent.NpcsData_SyncID], jinbi.Value };
            else objects = new object[] { jinbi.Value };
#if SERVER
            CheckUpdate(12);
            TitansiegeDBEvent.OnSyncProperty?.Invoke(client, NetCmd.SyncPropertyData, (ushort)TitansiegeHashProto.NPCS_JINBI, objects);
#else
            TitansiegeDBEvent.Client.SendRT(NetCmd.SyncPropertyData, (ushort)TitansiegeHashProto.NPCS_JINBI, objects);
#endif
        }

        [Rpc(hash = (ushort)TitansiegeHashProto.NPCS_JINBI)]
        private void JinbiRpc(Int32 value)//重写NetPlayer的OnStart方法来处理客户端自动同步到服务器数据库, 方法内部添加AddRpc(data(NpcsData));收集Rpc
        {
            Jinbi = value;
        }
     //1
        private readonly Int32Obs zuanshi = new Int32Obs("NpcsData_zuanshi", true, null);

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
                CheckUpdate(13);
            else if (type == 1)
                ZuanshiCall(false);
            else if (type == 2)
                ZuanshiCall(true);
            OnValueChanged?.Invoke(TitansiegeHashProto.NPCS_ZUANSHI, value);
        }

        /// <summary>钻石 --同步当前值到服务器Player对象上，需要处理</summary>
        public void ZuanshiCall(bool syncId = false)
        {
            
            object[] objects;
            if (syncId) objects = new object[] { this[TitansiegeDBEvent.NpcsData_SyncID], zuanshi.Value };
            else objects = new object[] { zuanshi.Value };
#if SERVER
            CheckUpdate(13);
            TitansiegeDBEvent.OnSyncProperty?.Invoke(client, NetCmd.SyncPropertyData, (ushort)TitansiegeHashProto.NPCS_ZUANSHI, objects);
#else
            TitansiegeDBEvent.Client.SendRT(NetCmd.SyncPropertyData, (ushort)TitansiegeHashProto.NPCS_ZUANSHI, objects);
#endif
        }

        [Rpc(hash = (ushort)TitansiegeHashProto.NPCS_ZUANSHI)]
        private void ZuanshiRpc(Int32 value)//重写NetPlayer的OnStart方法来处理客户端自动同步到服务器数据库, 方法内部添加AddRpc(data(NpcsData));收集Rpc
        {
            Zuanshi = value;
        }
     //1
        private readonly StringObs skills = new StringObs("NpcsData_skills", false, null);

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
                CheckUpdate(14);
            else if (type == 1)
                SkillsCall(false);
            else if (type == 2)
                SkillsCall(true);
            OnValueChanged?.Invoke(TitansiegeHashProto.NPCS_SKILLS, value);
        }

        /// <summary>技能 --同步当前值到服务器Player对象上，需要处理</summary>
        public void SkillsCall(bool syncId = false)
        {
            
            object[] objects;
            if (syncId) objects = new object[] { this[TitansiegeDBEvent.NpcsData_SyncID], skills.Value };
            else objects = new object[] { skills.Value };
#if SERVER
            CheckUpdate(14);
            TitansiegeDBEvent.OnSyncProperty?.Invoke(client, NetCmd.SyncPropertyData, (ushort)TitansiegeHashProto.NPCS_SKILLS, objects);
#else
            TitansiegeDBEvent.Client.SendRT(NetCmd.SyncPropertyData, (ushort)TitansiegeHashProto.NPCS_SKILLS, objects);
#endif
        }

        [Rpc(hash = (ushort)TitansiegeHashProto.NPCS_SKILLS)]
        private void SkillsRpc(String value)//重写NetPlayer的OnStart方法来处理客户端自动同步到服务器数据库, 方法内部添加AddRpc(data(NpcsData));收集Rpc
        {
            Skills = value;
        }
     //1
        private readonly StringObs prefabpath = new StringObs("NpcsData_prefabpath", false, null);

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
                CheckUpdate(15);
            else if (type == 1)
                PrefabpathCall(false);
            else if (type == 2)
                PrefabpathCall(true);
            OnValueChanged?.Invoke(TitansiegeHashProto.NPCS_PREFABPATH, value);
        }

        /// <summary>预制体路径 --同步当前值到服务器Player对象上，需要处理</summary>
        public void PrefabpathCall(bool syncId = false)
        {
            
            object[] objects;
            if (syncId) objects = new object[] { this[TitansiegeDBEvent.NpcsData_SyncID], prefabpath.Value };
            else objects = new object[] { prefabpath.Value };
#if SERVER
            CheckUpdate(15);
            TitansiegeDBEvent.OnSyncProperty?.Invoke(client, NetCmd.SyncPropertyData, (ushort)TitansiegeHashProto.NPCS_PREFABPATH, objects);
#else
            TitansiegeDBEvent.Client.SendRT(NetCmd.SyncPropertyData, (ushort)TitansiegeHashProto.NPCS_PREFABPATH, objects);
#endif
        }

        [Rpc(hash = (ushort)TitansiegeHashProto.NPCS_PREFABPATH)]
        private void PrefabpathRpc(String value)//重写NetPlayer的OnStart方法来处理客户端自动同步到服务器数据库, 方法内部添加AddRpc(data(NpcsData));收集Rpc
        {
            Prefabpath = value;
        }
     //1
        private readonly StringObs headpath = new StringObs("NpcsData_headpath", false, null);

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
                CheckUpdate(16);
            else if (type == 1)
                HeadpathCall(false);
            else if (type == 2)
                HeadpathCall(true);
            OnValueChanged?.Invoke(TitansiegeHashProto.NPCS_HEADPATH, value);
        }

        /// <summary>头像路径 --同步当前值到服务器Player对象上，需要处理</summary>
        public void HeadpathCall(bool syncId = false)
        {
            
            object[] objects;
            if (syncId) objects = new object[] { this[TitansiegeDBEvent.NpcsData_SyncID], headpath.Value };
            else objects = new object[] { headpath.Value };
#if SERVER
            CheckUpdate(16);
            TitansiegeDBEvent.OnSyncProperty?.Invoke(client, NetCmd.SyncPropertyData, (ushort)TitansiegeHashProto.NPCS_HEADPATH, objects);
#else
            TitansiegeDBEvent.Client.SendRT(NetCmd.SyncPropertyData, (ushort)TitansiegeHashProto.NPCS_HEADPATH, objects);
#endif
        }

        [Rpc(hash = (ushort)TitansiegeHashProto.NPCS_HEADPATH)]
        private void HeadpathRpc(String value)//重写NetPlayer的OnStart方法来处理客户端自动同步到服务器数据库, 方法内部添加AddRpc(data(NpcsData));收集Rpc
        {
            Headpath = value;
        }
     //1
        private readonly StringObs lihuipath = new StringObs("NpcsData_lihuipath", false, null);

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
                CheckUpdate(17);
            else if (type == 1)
                LihuipathCall(false);
            else if (type == 2)
                LihuipathCall(true);
            OnValueChanged?.Invoke(TitansiegeHashProto.NPCS_LIHUIPATH, value);
        }

        /// <summary>立绘路径 --同步当前值到服务器Player对象上，需要处理</summary>
        public void LihuipathCall(bool syncId = false)
        {
            
            object[] objects;
            if (syncId) objects = new object[] { this[TitansiegeDBEvent.NpcsData_SyncID], lihuipath.Value };
            else objects = new object[] { lihuipath.Value };
#if SERVER
            CheckUpdate(17);
            TitansiegeDBEvent.OnSyncProperty?.Invoke(client, NetCmd.SyncPropertyData, (ushort)TitansiegeHashProto.NPCS_LIHUIPATH, objects);
#else
            TitansiegeDBEvent.Client.SendRT(NetCmd.SyncPropertyData, (ushort)TitansiegeHashProto.NPCS_LIHUIPATH, objects);
#endif
        }

        [Rpc(hash = (ushort)TitansiegeHashProto.NPCS_LIHUIPATH)]
        private void LihuipathRpc(String value)//重写NetPlayer的OnStart方法来处理客户端自动同步到服务器数据库, 方法内部添加AddRpc(data(NpcsData));收集Rpc
        {
            Lihuipath = value;
        }
     //1
        private readonly DateTimeObs lastDate = new DateTimeObs("NpcsData_lastDate", false, null);

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
                CheckUpdate(18);
            else if (type == 1)
                LastDateCall(false);
            else if (type == 2)
                LastDateCall(true);
            OnValueChanged?.Invoke(TitansiegeHashProto.NPCS_LASTDATE, value);
        }

        /// <summary>最后登录时间 --同步当前值到服务器Player对象上，需要处理</summary>
        public void LastDateCall(bool syncId = false)
        {
            
            object[] objects;
            if (syncId) objects = new object[] { this[TitansiegeDBEvent.NpcsData_SyncID], lastDate.Value };
            else objects = new object[] { lastDate.Value };
#if SERVER
            CheckUpdate(18);
            TitansiegeDBEvent.OnSyncProperty?.Invoke(client, NetCmd.SyncPropertyData, (ushort)TitansiegeHashProto.NPCS_LASTDATE, objects);
#else
            TitansiegeDBEvent.Client.SendRT(NetCmd.SyncPropertyData, (ushort)TitansiegeHashProto.NPCS_LASTDATE, objects);
#endif
        }

        [Rpc(hash = (ushort)TitansiegeHashProto.NPCS_LASTDATE)]
        private void LastDateRpc(DateTime value)//重写NetPlayer的OnStart方法来处理客户端自动同步到服务器数据库, 方法内部添加AddRpc(data(NpcsData));收集Rpc
        {
            LastDate = value;
        }
     //2

        public NpcsData() { }

    #if SERVER
        public NpcsData(params object[] parms) : base()
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
                case 0: length = 0; return "iD";
     //3
                case 1: length = 0; return "zhiye";
     //3
                case 2: length = 0; return "level";
     //3
                case 3: length = 0; return "exp";
     //3
                case 4: length = 0; return "shengming";
     //3
                case 5: length = 0; return "fali";
     //3
                case 6: length = 0; return "tizhi";
     //3
                case 7: length = 0; return "liliang";
     //3
                case 8: length = 0; return "minjie";
     //3
                case 9: length = 0; return "moli";
     //3
                case 10: length = 0; return "meili";
     //3
                case 11: length = 0; return "xingyun";
     //3
                case 12: length = 0; return "jinbi";
     //3
                case 13: length = 0; return "zuanshi";
     //3
                case 14: length = 200; return "skills";
     //3
                case 15: length = 100; return "prefabpath";
     //3
                case 16: length = 100; return "headpath";
     //3
                case 17: length = 100; return "lihuipath";
     //3
                case 18: length = 0; return "lastDate";
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
                    case 0: return this.iD;
     //5
                    case 1: return this.zhiye.Value;
     //5
                    case 2: return this.level.Value;
     //5
                    case 3: return this.exp.Value;
     //5
                    case 4: return this.shengming.Value;
     //5
                    case 5: return this.fali.Value;
     //5
                    case 6: return this.tizhi.Value;
     //5
                    case 7: return this.liliang.Value;
     //5
                    case 8: return this.minjie.Value;
     //5
                    case 9: return this.moli.Value;
     //5
                    case 10: return this.meili.Value;
     //5
                    case 11: return this.xingyun.Value;
     //5
                    case 12: return this.jinbi.Value;
     //5
                    case 13: return this.zuanshi.Value;
     //5
                    case 14: return this.skills.Value;
     //5
                    case 15: return this.prefabpath.Value;
     //5
                    case 16: return this.headpath.Value;
     //5
                    case 17: return this.lihuipath.Value;
     //5
                    case 18: return this.lastDate.Value;
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
                        this.iD = (Int64)value;
                        break;
     //7
                    case 1:
                        CheckZhiyeValue((SByte)value, -1);
                        break;
     //7
                    case 2:
                        CheckLevelValue((SByte)value, -1);
                        break;
     //7
                    case 3:
                        CheckExpValue((Int32)value, -1);
                        break;
     //7
                    case 4:
                        CheckShengmingValue((Int32)value, -1);
                        break;
     //7
                    case 5:
                        CheckFaliValue((Int32)value, -1);
                        break;
     //7
                    case 6:
                        CheckTizhiValue((Int16)value, -1);
                        break;
     //7
                    case 7:
                        CheckLiliangValue((Int16)value, -1);
                        break;
     //7
                    case 8:
                        CheckMinjieValue((Int16)value, -1);
                        break;
     //7
                    case 9:
                        CheckMoliValue((Int16)value, -1);
                        break;
     //7
                    case 10:
                        CheckMeiliValue((Int16)value, -1);
                        break;
     //7
                    case 11:
                        CheckXingyunValue((Int16)value, -1);
                        break;
     //7
                    case 12:
                        CheckJinbiValue((Int32)value, -1);
                        break;
     //7
                    case 13:
                        CheckZuanshiValue((Int32)value, -1);
                        break;
     //7
                    case 14:
                        CheckSkillsValue((String)value, -1);
                        break;
     //7
                    case 15:
                        CheckPrefabpathValue((String)value, -1);
                        break;
     //7
                    case 16:
                        CheckHeadpathValue((String)value, -1);
                        break;
     //7
                    case 17:
                        CheckLihuipathValue((String)value, -1);
                        break;
     //7
                    case 18:
                        CheckLastDateValue((DateTime)value, -1);
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
                    case "iD": return this.iD;
     //9
                    case "zhiye": return this.zhiye.Value;
     //9
                    case "level": return this.level.Value;
     //9
                    case "exp": return this.exp.Value;
     //9
                    case "shengming": return this.shengming.Value;
     //9
                    case "fali": return this.fali.Value;
     //9
                    case "tizhi": return this.tizhi.Value;
     //9
                    case "liliang": return this.liliang.Value;
     //9
                    case "minjie": return this.minjie.Value;
     //9
                    case "moli": return this.moli.Value;
     //9
                    case "meili": return this.meili.Value;
     //9
                    case "xingyun": return this.xingyun.Value;
     //9
                    case "jinbi": return this.jinbi.Value;
     //9
                    case "zuanshi": return this.zuanshi.Value;
     //9
                    case "skills": return this.skills.Value;
     //9
                    case "prefabpath": return this.prefabpath.Value;
     //9
                    case "headpath": return this.headpath.Value;
     //9
                    case "lihuipath": return this.lihuipath.Value;
     //9
                    case "lastDate": return this.lastDate.Value;
     //10
                }
                throw new Exception("错误");
            }
            set
            {
                switch (name)
                {
     //11
                    case "iD":
                        this.iD = (Int64)value;
                        break;
     //11
                    case "zhiye":
                        CheckZhiyeValue((SByte)value, -1);
                        break;
     //11
                    case "level":
                        CheckLevelValue((SByte)value, -1);
                        break;
     //11
                    case "exp":
                        CheckExpValue((Int32)value, -1);
                        break;
     //11
                    case "shengming":
                        CheckShengmingValue((Int32)value, -1);
                        break;
     //11
                    case "fali":
                        CheckFaliValue((Int32)value, -1);
                        break;
     //11
                    case "tizhi":
                        CheckTizhiValue((Int16)value, -1);
                        break;
     //11
                    case "liliang":
                        CheckLiliangValue((Int16)value, -1);
                        break;
     //11
                    case "minjie":
                        CheckMinjieValue((Int16)value, -1);
                        break;
     //11
                    case "moli":
                        CheckMoliValue((Int16)value, -1);
                        break;
     //11
                    case "meili":
                        CheckMeiliValue((Int16)value, -1);
                        break;
     //11
                    case "xingyun":
                        CheckXingyunValue((Int16)value, -1);
                        break;
     //11
                    case "jinbi":
                        CheckJinbiValue((Int32)value, -1);
                        break;
     //11
                    case "zuanshi":
                        CheckZuanshiValue((Int32)value, -1);
                        break;
     //11
                    case "skills":
                        CheckSkillsValue((String)value, -1);
                        break;
     //11
                    case "prefabpath":
                        CheckPrefabpathValue((String)value, -1);
                        break;
     //11
                    case "headpath":
                        CheckHeadpathValue((String)value, -1);
                        break;
     //11
                    case "lihuipath":
                        CheckLihuipathValue((String)value, -1);
                        break;
     //11
                    case "lastDate":
                        CheckLastDateValue((DateTime)value, -1);
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
        /// 查询1: Query("`iD`=1");
        /// <para></para>
        /// 查询2: Query("`iD`=1 and `index`=1");
        /// <para></para>
        /// 查询3: Query("`iD`=1 or `index`=1");
        /// </summary>
        /// <param name="filterExpression"></param>
        /// <returns></returns>
        public static NpcsData Query(string filterExpression)
        {
            var cmdText = $"select * from npcs where {filterExpression}; ";
            var data = TitansiegeDB.I.ExecuteQuery<NpcsData>(cmdText);
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
        public static async UniTask<NpcsData> QueryAsync(string filterExpression)
        {
            var cmdText = $"select * from npcs where {filterExpression}; ";
            var data = await TitansiegeDB.I.ExecuteQueryAsync<NpcsData>(cmdText);
            return data;
        }
        public static NpcsData[] QueryList(string filterExpression)
        {
            var cmdText = $"select * from npcs where {filterExpression}; ";
            var datas = TitansiegeDB.I.ExecuteQueryList<NpcsData>(cmdText);
            return datas;
        }
        public static async UniTask<NpcsData[]> QueryListAsync(string filterExpression)
        {
            var cmdText = $"select * from npcs where {filterExpression}; ";
            var datas = await TitansiegeDB.I.ExecuteQueryListAsync<NpcsData>(cmdText);
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
            if (row[0] is Int64 iD)
                this.iD = iD;
     //15
            if (row[1] is SByte zhiye)
                CheckZhiyeValue(zhiye, -1);
     //15
            if (row[2] is SByte level)
                CheckLevelValue(level, -1);
     //15
            if (row[3] is Int32 exp)
                CheckExpValue(exp, -1);
     //15
            if (row[4] is Int32 shengming)
                CheckShengmingValue(shengming, -1);
     //15
            if (row[5] is Int32 fali)
                CheckFaliValue(fali, -1);
     //15
            if (row[6] is Int16 tizhi)
                CheckTizhiValue(tizhi, -1);
     //15
            if (row[7] is Int16 liliang)
                CheckLiliangValue(liliang, -1);
     //15
            if (row[8] is Int16 minjie)
                CheckMinjieValue(minjie, -1);
     //15
            if (row[9] is Int16 moli)
                CheckMoliValue(moli, -1);
     //15
            if (row[10] is Int16 meili)
                CheckMeiliValue(meili, -1);
     //15
            if (row[11] is Int16 xingyun)
                CheckXingyunValue(xingyun, -1);
     //15
            if (row[12] is Int32 jinbi)
                CheckJinbiValue(jinbi, -1);
     //15
            if (row[13] is Int32 zuanshi)
                CheckZuanshiValue(zuanshi, -1);
     //15
            if (row[14] is String skills)
                CheckSkillsValue(skills, -1);
     //15
            if (row[15] is String prefabpath)
                CheckPrefabpathValue(prefabpath, -1);
     //15
            if (row[16] is String headpath)
                CheckHeadpathValue(headpath, -1);
     //15
            if (row[17] is String lihuipath)
                CheckLihuipathValue(lihuipath, -1);
     //15
            if (row[18] is DateTime lastDate)
                CheckLastDateValue(lastDate, -1);
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
            string cmdText = $"DELETE FROM npcs {CheckSqlKey(0, iD)}";
            sb.Append(cmdText);
            RowState = DataRowState.Deleted;
    #endif
        }

    #if SERVER
        public void BulkLoaderBuilder(StringBuilder sb)
        {
 
            for (int i = 0; i < 19; i++)
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
                        NDebug.LogError($"npcs表{name}列长度溢出!");
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
            return $"ID:{ID} Zhiye:{Zhiye} Level:{Level} Exp:{Exp} Shengming:{Shengming} Fali:{Fali} Tizhi:{Tizhi} Liliang:{Liliang} Minjie:{Minjie} Moli:{Moli} Meili:{Meili} Xingyun:{Xingyun} Jinbi:{Jinbi} Zuanshi:{Zuanshi} Skills:{Skills} Prefabpath:{Prefabpath} Headpath:{Headpath} Lihuipath:{Lihuipath} LastDate:{LastDate} ";
        }
    }
}
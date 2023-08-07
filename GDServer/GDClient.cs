using GDServer.Services;
using MySqlX.XDevAPI.Common;
using Net.Component;
using Net.Server;
using Net.Share;
using System.Collections.Generic;
using System.Security.Cryptography;
using Titansiege;
using static Google.Protobuf.Reflection.FieldOptions.Types;

namespace GDServer {
    public class GDClient : NetPlayer//客户端类
    {
        /// <summary>
        /// 默认基础闪避和基础暴击，各职业怪物不同就单独修改
        /// </summary>
        public const float BaseDodge = 0.01f;
        public const float BaseCrit = 0.01f;

        internal NTransform transform = new NTransform();
        public List<CharactersData>? CharacterList = new List<CharactersData>();
        public CharactersData? current;//当前登录角色
        public UsersData? User =new UsersData();
        public  GDScene scene;
        public bool m_IsDie = false;
        public override void OnEnter() {
            m_IsDie = false;
            scene = Scene as GDScene;
        }

        public override void OnUpdate() {
            if (scene == null && current == null) {
                return;
            }
            scene.AddOperation(new Operation(Command.PlayerState, UserID) {
                index = FightHP,
                index1 = FightMagic,
            });
        }

        internal void BeAttacked(int damage) {
            if (m_IsDie)
                return;
            FightHP -= (short)damage;
            if (FightHP<=0&& !m_IsDie) {
                m_IsDie = true;
            }
        }

        public void Resurrection() {
            //数据库中不设置最大生命，而是根据等级+装备来计算
            FightHP = m_MaxFightHP;
            m_IsDie = false;
        }
        public override void Dispose() {
            base.Dispose();
        }

        public override void OnExit() {
            base.OnExit();
        }

        public override void OnRemoveClient() {
            base.OnRemoveClient();
            
        }

        public override void OnSignOut() {
            base.OnSignOut();
            RemoveRpc(User);
            RemoveRpc(current);
            User = null;
            current = null;
        }
        /// <summary>
        /// 登录成功后才会调用
        /// </summary>
        public override void OnStart() {
            base.OnStart();
            AddRpc(User);
            //AddRpc(current); //登陆成功后没有选择角色，此时为空，所以不能在这里加载，应该在选择角色后加载
        }
        public override void OnRpcExecute(RPCModel model) {
            switch (model.methodHash) {
                default:
                    base.OnRpcExecute(model);
                    break;
            }
        }

        /// <summary>
        /// 根据属性写入战斗属性
        /// 暂时还未加入道具影响，例如装备，需道具模块开发完成后，再完善
        /// </summary>
        public virtual void UpdateFightProps() {
            float jcDodge = BaseDodge;//基础闪避率，各职业角色和怪物不同
            float jcCrit = BaseCrit;//基础暴击率，各职业角色和怪物不同
            switch (current?.Zhiye) {
                case (int)Zhiye.剑士:
                    Attack = current.Liliang * 10;
                    Defense = current.Liliang * 3 + current.Tizhi * 7;
                    break;
                case (int)Zhiye.法师:
                    Attack = current.Moli * 10;
                    Defense = current.Moli * 4 + current.Tizhi * 6;
                    jcDodge = 0.02f;
                    break;
                case (int)Zhiye.游侠:
                    Attack = current.Minjie * 10;
                    Defense = current.Minjie * 4 + current.Tizhi * 6;
                    jcDodge = 0.03f;
                    break;
                default:
                    break;
            }
            //闪避,基础闪避率0.01f;
            Dodge = jcDodge + (float)current.Minjie / 1000f >= 0.3f ? 0.3f : (float)current.Minjie / 1000f;//属性加成的闪避
            Crit = jcCrit + (float)current.Xingyun * jcCrit >= 0.5f ? 0.5f : (float)current.Xingyun * jcCrit;//暴击率
            FightHP = current.Shengming + current.Tizhi * 10;
            Debuger.Log(FightHP);
            m_MaxFightHP = FightHP;
            FightMagic = current.Fali + current.Moli * 10;
            m_MaxFightMagic = FightMagic;
        }
        #region 战斗属性
        public int m_MaxFightHP =0;
        public int m_MaxFightMagic = 0;
        protected int m_Attack;
        public virtual int Attack {
            get {
                return m_Attack;
            }
            set {
                m_Attack = value;
                //通知ui层数据发生变化
            }
        }
        protected int m_Defense;
        public virtual int Defense {
            get {
                return m_Defense;
            }
            set {
                m_Defense = value;
                //通知ui层数据发生变化

            }
        }
        protected float m_Dodge;//闪避
        public virtual float Dodge {
            get {
                return m_Dodge;
            }
            set {
                m_Dodge = value;
                //通知ui层数据发生变化

            }
        }
        protected float m_Crit;//暴击
        public virtual float Crit {
            get {
                return m_Crit;
            }
            set {
                m_Crit = value;
                //通知ui层数据发生变化

            }
        }
        protected int m_FightHp;//战斗时生命
        public virtual int FightHP {
            get {

                return m_FightHp;
            }
            set {

                if (value <= 0) {
                    m_FightHp = 0;
                    m_IsDie = true;
                    //通知死亡 todo
                } else {
                    m_FightHp = value;
                }
                //通知ui层数据发生变化

            }
        }
        protected int m_FightMaxHp;//战斗时最大生命
        public virtual int FightMaxHp {
            get {

                return m_FightMaxHp;
            }
            set {
                m_FightMaxHp = value;
                //通知ui层数据发生变化

            }
        }
        protected int m_FightMagic;//战斗魔力
        public virtual int FightMagic {
            get {
                return m_FightMagic;
            }
            set {
                m_FightMagic = value;
                //通知ui层数据发生变化

            }
        }
        protected int m_FightMaxMagic;//战斗时最大魔力
        public virtual int FightMaxMagic {
            get {
                return m_FightMaxMagic;
            }
            set {
                m_FightMaxMagic = value;
                //通知ui层数据发生变化

            }
        }
        protected int m_FightLevel = 0;//等级
        public virtual int FightLevel {
            get {
                return m_FightLevel;
            }
            set {
                m_FightLevel = value - 1;
                //通知ui层数据发生变化

            }
        }
        #endregion
    }
    /// <summary>
    /// 职业枚举
    /// </summary>
    public enum Zhiye {
        剑士 = 0,
        法师 = 1,
        游侠 = 2,
    }
}

/****************************************************
    文件：NPCBase.cs
	作者：昔莘
    邮箱: 304183153@qq.com
    日期：2021/11/26 15:55:57
	功能：Nothing
*****************************************************/
using GF.ConfigTable;
using GF.Const;
using GF.MainGame.Data;
using GF.MainGame.Module.Fight;
using GF.Model;
using GF.Service;
using GF.Unity.AB;
using UnityEngine;

namespace GF.MainGame.Module.NPC {
    public class NPCBase : MonoBehaviour {
        public NpcType m_NpcType; 
        public int m_GDID;
        public NPCAnimatorBase m_Nab;
        public ushort m_Id;
        public GameObject m_Wpdorsum;//背部武器
        public GameObject m_WpHand;//手持武器
        protected NPCDataBase m_Data;//数据值
        protected LevelUpDataBase m_LevelData;//升级系数
        protected bool m_IsFight = false;
        public bool IsFight {
            get {
                return m_IsFight;
            }
            set { 
                m_IsFight = value;
                ChangeWp();
            }
        }
        public CharacterController m_Character;
        public AniState CurrentState = AniState.none;
        public bool isCanMove = true;//能否移动，用来判断是否npc
        /// <summary>
        /// 玩家不用这里的数值，这里是配置表的数值
        /// 只有怪物采用配置表的数值
        /// </summary>
        public NPCDataBase Data {
            get {
                if (m_Data == null) {
                    m_Data = ConfigerManager.m_NPCData.FindNPCByID(m_Id);
                }
                return m_Data;
            }
        }
        public LevelUpDataBase LevelData {
            get {
                if (m_LevelData == null) {
                    LevelUpData levelup= ConfigerManager.GetInstance.FindData<LevelUpData>(CT.TABLE_LEVEL);
                    if (levelup != null) {
                        m_LevelData = levelup.FindByID(Data.Levelupid);
                    }
                }
                return m_LevelData;
            }
        }
        public virtual void Awake() {
            m_Character = GetComponent<CharacterController>();
            InitNPCAnimaor();
            
        }
        public virtual void Start() {
            oldPosition = transform.position;
            Init();
            UpdateFightProps();
        }

        public virtual void Init(bool canmove = true) {
            isCanMove = canmove;
            
        }
        /// <summary>
        /// 根据属性写入战斗属性
        /// 暂时还未加入道具影响，例如装备，需道具模块开发完成后，再完善
        /// </summary>
        public virtual void UpdateFightProps() {
            float jcDodge = AppConfig.Dodge;//基础闪避率，各职业角色和怪物不同
            float jcCrit = AppConfig.Crit;//基础暴击率，各职业角色和怪物不同
            if (m_NpcType == NpcType.player) {//计算玩家的属性
                CharactersData cd = UserService.GetInstance.m_CurrentChar;
                switch (cd.Zhiye) {
                    case (int)Zhiye.剑士:
                        Attack = cd.Liliang * 10;
                        Defense = cd.Liliang * 3 + cd.Tizhi * 7;
                        break;
                    case (int)Zhiye.法师:
                        Attack = UserService.GetInstance.m_CurrentChar.Moli * 10;
                        Defense = cd.Moli * 4 + cd.Tizhi * 6;
                        jcDodge = 0.02f;
                        break;
                    case (int)Zhiye.游侠:
                        Attack = UserService.GetInstance.m_CurrentChar.Minjie * 10;
                        Defense = cd.Minjie * 4 + cd.Tizhi * 6;
                        jcDodge = 0.03f;
                        break;
                    default:
                        break;
                }
                //闪避,基础闪避率0.01f;
                Dodge = jcDodge + (float)cd.Minjie / 1000f >= 0.3f ? 0.3f : (float)cd.Minjie / 1000f;//属性加成的闪避
                Crit = jcCrit + (float)cd.Xingyun * jcCrit >= 0.5f ? 0.5f : (float)cd.Xingyun * jcCrit;//暴击率
                FightHP = cd.Shengming + cd.Tizhi * 10;
                FightMagic = cd.Fali + cd.Moli * 10;
            } else if(m_NpcType == NpcType.monster){//计算怪物的属性
                switch (Data.Zhiye) {
                    case (int)Zhiye.剑士:
                        Attack = (Data.Liliang+Data.Level*LevelData.Liliang) * 10;
                        Defense = (Data.Liliang + Data.Level * LevelData.Liliang) * 3 + (Data.Tizhi + Data.Level * LevelData.Tizhi) * 7;
                        break;
                    case (int)Zhiye.法师:
                        Attack = (Data.Moli + Data.Level * LevelData.Moli) * 10;
                        Defense = (Data.Moli + Data.Level * LevelData.Moli) * 4 + (Data.Tizhi + Data.Level * LevelData.Tizhi) * 6;
                        jcDodge = 0.02f;
                        break;
                    case (int)Zhiye.游侠:
                        Attack = (Data.Minjie + Data.Level * LevelData.Minjie) * 10;
                        Defense = (Data.Minjie + Data.Level * LevelData.Minjie) * 4 + (Data.Tizhi + Data.Level * LevelData.Tizhi) * 6;
                        jcDodge = 0f;
                        break;
                    default:
                        break;
                }
                //闪避,基础闪避率0.01f;
                Dodge = jcDodge + (float)(Data.Minjie + Data.Level * LevelData.Minjie) / 1000f >= 0.3f ? 0.3f : (float)(Data.Minjie + Data.Level * LevelData.Minjie ) / 1000f;//属性加成的闪避
                Crit = jcCrit + (float)(Data.Xingyun + Data.Level * LevelData.Xingyun) * jcCrit >= 0.5f ? 0.5f : (float)(Data.Xingyun + Data.Level * LevelData.Xingyun) * jcCrit;//暴击率
                FightHP = (Data.Shengming + Data.Level * LevelData.Shengming) + (Data.Tizhi + Data.Level * LevelData.Tizhi) * 10;//战斗生命
                FightMagic = (Data.Fali + Data.Level * LevelData.Fali) + (Data.Moli + Data.Level * LevelData.Moli) * 10;//战斗法力
            }
        }
        public virtual void InitNPCAnimaor() {
            m_Nab = transform.GetComponent<NPCAnimatorBase>();
            if (m_Nab == null) {
                m_Nab = transform.gameObject.AddComponent<NPCAnimatorBase>();
                m_Nab.Init();
            }
        }
       
        /// <summary>
        /// 正常情况下，进入战斗状态的函数也不在这里，应该是点击切换战斗，或者触发战斗时调用的
        /// </summary>
        /// <param name="fight"></param>
        public virtual void SetFight(bool fight) {
            IsFight = fight;
            m_Nab.SetFight(IsFight);
            if (IsFight) {
                AppTools.Send<NPCBase, AniState>((int)StateEvent.ChangeState, this, AniState.wait);
            } else {
                AppTools.Send<NPCBase, AniState>((int)StateEvent.ChangeState, this, AniState.idle);
            }
        }

        /// <summary>
        /// 如果是网络角色，movemodule脚本不会控制它，所以需要自己判断切换跑步和待机动画
        /// </summary>
        protected Vector3 oldPosition;//网络对象上一帧同步过来的位置
        protected Vector3 newPosition;//网络对象本帧同步过来的位置
        protected Vector3 oldoldPosition;//网络对象上上一帧同步过来的位置，用来减少移动中的频繁改变动画状态的情况，只有三帧都相同，才让切换,相当于延迟一帧
        protected float time;
        public bool m_IsNetPlayer = false;
        public bool isPlaySkill = false;
        public virtual void LateUpdate() {
            if (!isPlaySkill&&m_IsNetPlayer && (Time.time > time)) {
                newPosition = transform.position;
                if (oldPosition != newPosition) {
                    if (Vector3.Distance(oldPosition, newPosition) > 0.02f) {
                        if ((CurrentState != AniState.fightrun && CurrentState != AniState.run)) {
                            if (IsFight) {
                                AppTools.Send<NPCBase, AniState>((int)StateEvent.ChangeState, this, AniState.fightrun);
                            } else {
                                AppTools.Send<NPCBase, AniState>((int)StateEvent.ChangeState, this, AniState.run);
                            }
                        } 
                        
                    } else {
                        if (Vector3.Distance(oldPosition, oldoldPosition) <= 0.02f) {
                            if (CurrentState == AniState.run || CurrentState == AniState.fightrun) {
                                if (IsFight) {
                                    AppTools.Send<NPCBase, AniState>((int)StateEvent.ChangeState, this, AniState.wait);
                                } else {
                                    AppTools.Send<NPCBase, AniState>((int)StateEvent.ChangeState, this, AniState.idle);
                                }
                            }
                        }
                    }
                } else {
                    if (Vector3.Distance(oldPosition, oldoldPosition) <= 0.02f) {
                        if (CurrentState == AniState.run || CurrentState == AniState.fightrun) {
                            if (IsFight) {
                                AppTools.Send<NPCBase, AniState>((int)StateEvent.ChangeState, this, AniState.wait);
                            } else {
                                AppTools.Send<NPCBase, AniState>((int)StateEvent.ChangeState, this, AniState.idle);
                            }
                        }
                    } 
                }
                oldoldPosition = oldPosition;
                oldPosition = newPosition;
                time = Time.time + (1f / 50f);
            }
        }
        /// <summary>
        /// 根据战斗状态切换武器
        /// </summary>
        public void ChangeWp() {
            if (IsFight) {
                if (m_Wpdorsum!=null) {
                    m_Wpdorsum.SetActive(false);
                }
                if (m_WpHand!=null) {
                    m_WpHand.SetActive(true);
                }
            } else {
                if (m_Wpdorsum != null) {
                    m_Wpdorsum.SetActive(true);
                }
                if (m_WpHand != null) {
                    m_WpHand.SetActive(false);
                }
            }
        }
        #region 战斗属性
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
        protected float m_FightHp;//战斗时生命
        public virtual float FightHP {
            get {
                return m_FightHp;
            }
            set {
                m_FightHp = value;
                //通知ui层数据发生变化

            }
        }
        protected float m_FightMagic;//战斗魔力
        public virtual float FightMagic {
            get {
                return m_FightMagic;
            }
            set {
                m_FightMagic = value;
                //通知ui层数据发生变化

            }
        }
        #endregion
    }
}


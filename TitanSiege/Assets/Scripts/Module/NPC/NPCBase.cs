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
        public FightProp m_FightProp;//战斗属性
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
            InitFightProps();
        }

        public virtual void Init(bool canmove = true) {
            isCanMove = canmove;
            
        }
        /// <summary>
        /// 子类初始化战斗属性
        /// </summary>
        public virtual void InitFightProps() {
            //属性加成,魔法加成法术攻击,1点魔力10点攻击
            switch (UserService.GetInstance.m_CurrentChar.Zhiye) {
                case 0:
                    m_FightProp.Attack = UserService.GetInstance.m_CurrentChar.Liliang * 10;
                    break;
                case 1:
                    m_FightProp.Attack = UserService.GetInstance.m_CurrentChar.Moli * 10;
                    break;
                case 2:
                    break;
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
    }
    public class FightProp {
        protected int m_Attack;
        public virtual int Attack {
            get {
                return m_Attack;
            }
            set {
                if (value >= 0) {
                    m_Attack = value;
                } else {
                    m_Attack = 0;
                }
                //通知ui层数据发生变化

            }
        }

    }
}

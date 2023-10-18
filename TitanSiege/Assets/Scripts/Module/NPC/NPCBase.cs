/****************************************************
    文件：NPCBase.cs
	作者：昔莘
    邮箱: 304183153@qq.com
    日期：2021/11/26 15:55:57
	功能：Nothing
*****************************************************/
using Cysharp.Threading.Tasks;
using GameDesigner;
using GF.ConfigTable;
using GF.Const;
using GF.MainGame.Data;
using GF.MainGame.Module.Fight;
using GF.MainGame.UI;
using GF.NetWork;
using GF.Service;
using GF.Unity.UI;
using Net.Client;
using Net.Share;
using Net.System;
using System.Collections.Generic;
using Titansiege;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.EventSystems;

namespace GF.MainGame.Module.NPC {
    
    public class NPCBase : MonoBehaviour,IPointerDownHandler {
        public NpcType m_NpcType; //角色类型
        public int m_GDID;//网络id
        public NPCAnimatorBase m_Nab;
        public ushort m_Id;//数据的id
        public GameObject m_Wpdorsum;//背部武器
        public GameObject m_WpHand;//手持武器
        public Transform m_HPRoot;//血条位置
        public StateManager m_State;//GDnet的状态机
        public List<int> m_SkillId;//本角色的技能id
        public Dictionary<string, int> m_AllStateID;//GDNet状态机所有的状态标识
        protected NPCDataBase m_Data;//数据值
        protected LevelUpDataBase m_LevelData;//升级系数
        public int m_CurrentSkillId = -1;//当前正在施展的技能的id
        protected bool m_IsFight = false;
        public EffectBase m_LevelUp;
        public GameObject m_Selected;//角色脚下的选中目标
        private NPCBase m_AttackTarget = null;//攻击的目标

        public bool isPlaySkill = false;//是否正在播放技能
        public bool m_IsDie = false;
        //public Transform m_SkilleffectParent;//角色技能特效的父物体，所有技能特效都在这个物体下，需要丢出的技能特效重置回来时也需要用它
        public NPCBase AttackTarget {
            set {
                m_AttackTarget = value;
                //if (m_NpcType != NpcType.monster) {//怪物不能给别人加选中，只有玩家可以
                //    if (m_AttackTarget == null) {
                //        m_AttackTarget.m_Selected.SetActive(false);
                //    } else {
                //        m_AttackTarget.m_Selected.SetActive(true);
                //    }
                //}
            }
            get {
                return m_AttackTarget;
            }
        }
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
        public bool isCanMove = true;//能否移动，用来判断是否npc

        public FightProp FP = new FightProp();//战斗属性,区别于配置表的属性,战斗属性是运行时，通过基础属性计算出来的
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
            m_State = GetComponent<StateManager>();
            m_AllStateID = new Dictionary<string, int>();
            //if (m_SkilleffectParent==null) {
            //    m_SkilleffectParent = transform.Find("skilleffect");
            //}
            //获取所有状态标志，方便切换状态使用
            if (m_State!=null) {
                for (int i = 0; i < m_State.stateMachine.states.Count; i++) {
                    m_AllStateID.Add(m_State.stateMachine.states[i].name, m_State.stateMachine.states[i].ID);
                }
            }
            InitNPCAnimaor();
            //设置血条的根位置
            if (m_HPRoot == null) {
                m_HPRoot = transform.Find("HPRoot");
            }
            m_SkillId = new List<int>();
            //状态机初始化（start调用）需要使用这个数据，所以这个数据必须在Awake中获取
            switch (m_NpcType) {
                case NpcType.player:
                    //技能添加的顺序，决定角色技能在面板上的位置，第一个永远是普通攻击，第二个永远是第一个技能，以此类推
                    //if (UserService.GetInstance.m_CurrentChar != null && UserService.GetInstance.m_CurrentChar.Skills != "") {//这里不能直接使用玩家的数据，因为非本机玩家也会执行这个
                    if (ClientBase.Instance.UID == m_GDID) {
                        if (UserService.GetInstance.m_CurrentChar != null && UserService.GetInstance.m_CurrentChar.Skills != "") {
                            string[] strarr = UserService.GetInstance.m_CurrentChar.Skills.Split('|');
                            for (int i = 0; i < strarr.Length; i++) {
                                if (!string.IsNullOrEmpty(strarr[i])) { //数据库存储最后一个字符时“|”,所以会出现一个空白项
                                    m_SkillId.Add(int.Parse(strarr[i]));
                                }
                            }
                        }
                    } else { //网络玩家对象，先用本地数据，其实可以使用网络同步的数据，后面再改吧
                        if (Data != null && Data.Skills.Count!=0) {
                            m_SkillId = Data.Skills;
                        }
                    }
                    break;
                case NpcType.monster:
                    //技能添加的顺序，决定角色技能在面板上的位置，第一个永远是普通攻击，第二个永远是第一个技能，以此类推
                    if (Data != null && Data.Skills.Count != 0) {
                        m_SkillId = Data.Skills;
                    } else {
                        Debuger.LogError($"{name}{m_GDID}没有配置数据或者技能数据！");
                    }
                    break;
                case NpcType.npc:
                    break;
                default:
                    break;
            }
            if (m_Selected!=null) {
                m_Selected.SetActive(false);//默认隐藏显示选中特效
            }
            if (m_LevelUp != null) {
                m_LevelUp.gameObject.SetActive(false);
            }
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
        /// uphpui,false是创建血条，true是更新血条
        /// </summary>
        public virtual void UpdateFightProps(bool uphpui = false) {
            float jcDodge = FP.BaseDodge;//基础闪避率，各职业角色和怪物不同
            float jcCrit = FP.BaseCrit;//基础暴击率，各职业角色和怪物不同
            if (m_NpcType == NpcType.player) {//计算玩家的属性
                if (ClientBase.Instance.UID == m_GDID) {//本机玩家
                    CharactersData cd = UserService.GetInstance.m_CurrentChar;
                    switch (cd.Zhiye) {
                        case (int)Zhiye.剑士:
                            FP.Attack = cd.Liliang * 10;
                            FP.Defense = cd.Liliang * 3 + cd.Tizhi * 7;
                            break;
                        case (int)Zhiye.法师:
                            FP.Attack = cd.Moli * 10;
                            FP.Defense = cd.Moli * 4 + cd.Tizhi * 6;
                            jcDodge = 0.02f;
                            break;
                        case (int)Zhiye.游侠:
                            FP.Attack = cd.Minjie * 10;
                            FP.Defense = cd.Minjie * 4 + cd.Tizhi * 6;
                            jcDodge = 0.03f;
                            break;
                        default:
                            break;
                    }
                    //闪避,基础闪避率0.01f;
                    FP.Dodge = jcDodge + (float)cd.Minjie / 1000f >= 0.3f ? 0.3f : (float)cd.Minjie / 1000f + jcDodge;//属性加成的闪避
                    FP.Crit = jcCrit + (float)cd.Xingyun * jcCrit >= 0.5f ? 0.5f : (float)cd.Xingyun * jcCrit;//暴击率
                    FP.FightHP = cd.Shengming + cd.Tizhi * 10;
                    FP.FightMaxHp = FP.FightHP;//战斗时最大生命
                    FP.FightMagic = cd.Fali + cd.Moli * 10;
                    FP.FightMaxMagic = FP.FightMagic;//战斗最大法力
                } else { //非本机玩家，ClientSceneManager中创建对象时，或从服务器获取战斗属性，此处无需处理

                }
            } else if(m_NpcType == NpcType.monster){//计算怪物的属性
                switch (Data.Zhiye) {
                    case (int)Zhiye.剑士:
                        FP.Attack = (Data.Liliang) * 10;
                        FP.Defense = (Data.Liliang) * 3 + (Data.Tizhi ) * 7;
                        break;
                    case (int)Zhiye.法师:
                        FP.Attack = (Data.Moli) * 10;
                        FP.Defense = (Data.Moli) * 4 + (Data.Tizhi) * 6;
                        jcDodge = 0.02f;
                        break;
                    case (int)Zhiye.游侠:
                        FP.Attack = (Data.Minjie) * 10;
                        FP.Defense = (Data.Minjie ) * 4 + (Data.Tizhi) * 6;
                        jcDodge = 0f;
                        break;
                    default:
                        break;
                }
                //闪避,基础闪避率0.01f;
                FP.Dodge = jcDodge + (float)(Data.Minjie) / 1000f >= 0.3f ? 0.3f : (float)(Data.Minjie ) / 1000f + jcDodge;//属性加成的闪避
                FP.Crit = jcCrit + (float)(Data.Xingyun) * jcCrit >= 0.5f ? 0.5f : (float)(Data.Xingyun) * jcCrit;//暴击率
                FP.FightMaxHp = (Data.Shengming) + (Data.Tizhi) * 10;//战斗时最大生命
                FP.FightHP = FP.FightMaxHp;//战斗生命
                FP.FightMaxMagic = (Data.Fali) + (Data.Moli) * 10;//战斗最大法力
                FP.FightMagic = FP.FightMaxMagic;//战斗法力
            }
            //本机玩家和怪物创建显示血条和名称，网络玩家是在物体创建时创建,可能重复创建，所以需要再创建时排除一下重复
            if (uphpui) {
                AppTools.Send<NPCBase>((int)HPEvent.UpdateHp, this);
            } else {
                if ((NpcType.player == m_NpcType && m_GDID == ClientBase.Instance.UID) || m_NpcType == NpcType.monster) {
                    AppTools.Send<NPCBase>((int)HPEvent.CreateHPUI, this);
                }
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
        /// 玩家角色切换角色动画使用，需要判断一下是不是本机，如果不是，发通知通知服务器切换动画
        /// </summary>
        /// <param name="stateid"></param>
        public void ChangeState(int stateid,int skillid = -1) {
            bool sendto = true;//能否发送，有的时候是不用执行的，比如释放技能却魔力不足
            if (m_State.stateMachine.currState.ID!= stateid) {//判断当前状态，节省带宽
                if (m_GDID == ClientBase.Instance.UID) {
                    if (skillid == -1) {
                        ClientBase.Instance.AddOperation(new Operation(Command.SwitchState, m_GDID) { index1 = stateid});
                    } else if (skillid != -1) {
                        if (m_IsDie) {
                            return;
                        }
                        SkillDataBase sd = ConfigerManager.m_SkillData.FindSkillByID(skillid);
                        if (FP.FightMagic < sd.xiaohao) {//魔力不够，直接返回idle
                            UIManager.GetInstance.OpenUIWidget(AppConfig.UIMsgTips, "没有魔力了!");
                            sendto = false;
                        } else {
                            ClientBase.Instance.AddOperation(new Operation(Command.Skill, m_GDID) { index1 = stateid, index2 = skillid, index3 = sd.xiaohao });
                        }
                    }
                }
                if (sendto) {//能否发送，有的时候是不用执行的，比如释放技能却魔力不足
                    if (m_IsDie&& stateid!= m_AllStateID["die"]) {
                        return;
                    }
                    m_State.StatusEntry(stateid);
                }
            }
        }
        /// <summary>
        /// 正常情况下，进入战斗状态的函数也不在这里，应该是点击切换战斗，或者触发战斗时调用的
        /// </summary>
        /// <param name="fight"></param>
        public float m_Resetidletime;
        //private bool iswaitidle = false;
        public virtual void SetFight(bool fight) {
            IsFight = fight;
            if (m_GDID == ClientBase.Instance.UID) {
                int stateid = 0;
                if (IsFight) {
                    stateid = m_AllStateID["fightidle"];
                } else {
                    stateid = m_AllStateID["idle"];
                }
                ChangeState(stateid);
                //开一个计时器，如果一段时间之内没有发动任何技能或者收到任何伤害，就切换为非战斗状态。
                //任意的技能和伤害都会重置这个时间
                if (fight) {
                    m_Resetidletime = AppConfig.FightReset;
                    _ = SetIdleTimer();
                }
            }
        }
        private async UniTaskVoid SetIdleTimer() {
            await UniTask.WaitUntil(() => m_Resetidletime <= 0) ;
            SetFight(false);
        }
        /// <summary>
        /// 如果是网络角色，movemodule脚本不会控制它，所以需要自己判断切换跑步和待机动画
        /// </summary>
        protected Vector3 oldPosition;//网络对象上一帧同步过来的位置
        protected Vector3 newPosition;//网络对象本帧同步过来的位置
        protected Vector3 oldoldPosition;//网络对象上上一帧同步过来的位置，用来减少移动中的频繁改变动画状态的情况，只有三帧都相同，才让切换,相当于延迟一帧
        protected float time;
       
        public virtual void LateUpdate() {
            if (m_Resetidletime > 0 && !m_IsDie) {
                m_Resetidletime -= Time.deltaTime;
            }
            //if (!isPlaySkill&&m_GDID!=ClientBase.Instance.UID && (Time.time > time)) {
            //    newPosition = transform.position;
            //    if (oldPosition != newPosition) {
            //        if (Vector3.Distance(oldPosition, newPosition) > 0.02f) {
            //            if ((m_State.stateMachine.currState.ID != m_AllStateID["fightrun"] && m_State.stateMachine.currState.ID != m_AllStateID["run"])) {
            //                if (IsFight) {
            //                    m_State.StatusEntry(m_AllStateID["fightrun"]);
            //                } else {
            //                    m_State.StatusEntry(m_AllStateID["run"]);
            //                }
            //            } 

            //        } else {
            //            if (Vector3.Distance(oldPosition, oldoldPosition) <= 0.02f) {
            //                if (m_State.stateMachine.currState.ID == m_AllStateID["run"] || m_State.stateMachine.currState.ID == m_AllStateID["fightrun"]) {
            //                    if (IsFight) {
            //                        m_State.StatusEntry(m_AllStateID["fightidle"]);
            //                    } else {
            //                        m_State.StatusEntry(m_AllStateID["idle"]);
            //                    }
            //                }
            //            }
            //        }
            //    } else {
            //        if (Vector3.Distance(oldPosition, oldoldPosition) <= 0.02f) {
            //            if (m_State.stateMachine.currState.ID == m_AllStateID["run"] || m_State.stateMachine.currState.ID == m_AllStateID["fightrun"]) {
            //                if (IsFight) {
            //                    m_State.StatusEntry(m_AllStateID["fightidle"]);
            //                } else {
            //                    m_State.StatusEntry(m_AllStateID["idle"]);
            //                }
            //            }
            //        } 
            //    }
            //    oldoldPosition = oldPosition;
            //    oldPosition = newPosition;
            //    time = Time.time + (1f / 50f);
            //}
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
        /// <summary>
        /// 选中事件
        /// </summary>
        /// <param name="eventData"></param>
        public void OnPointerDown(PointerEventData eventData) {
            //只能选中m_Selected的不为空的物体
            if (m_Selected != null) {
                NPCModule npcmodule = AppTools.GetModule<NPCModule>(MDef.NPCModule);
                if (npcmodule != null && npcmodule.m_CurrentSelected != this) {
                    AppTools.Send<NPCBase>((int)NpcEvent.ChangeSelected, this);
                    UserService.GetInstance.m_CurrentPlayer.AttackTarget = this;
                }
            } 
        }

        /// <summary>
        /// 升级的方法
        /// </summary>
        /// <param name="levelupnum">升级多少次</param>
        public void UpProp(int levelupnum,int exp,int hp = -1) {
            if (NpcType.player == m_NpcType) {
                if (ClientBase.Instance.UID == m_GDID) {//本机玩家
                    CharactersData cd = UserService.GetInstance.m_CurrentChar;
                    if (levelupnum > 0) {
                        cd.Liliang += (short)(LevelData.Liliang * levelupnum);
                        cd.Tizhi += (short)(LevelData.Tizhi * levelupnum);
                        cd.Minjie += (short)(LevelData.Minjie * levelupnum);
                        cd.Moli += (short)(LevelData.Moli * levelupnum);
                        cd.Shengming += (short)(LevelData.Shengming * levelupnum);
                        cd.Fali += (short)(LevelData.Fali * levelupnum);
                        cd.Meili += (short)(LevelData.Meili * levelupnum);
                        cd.Xingyun += (short)(LevelData.Xingyun * levelupnum);
                        cd.Lianjin += (short)(LevelData.Lianjin * levelupnum);
                        cd.Duanzao += (short)(LevelData.Duanzao * levelupnum);
                        cd.Level += (sbyte)levelupnum;
                        UpdateFightProps(true);
                    } 
                    cd.Exp = exp;
                } else { //非本机玩家，无需处理，只需要更新血条即可
                    if (hp != -1) { 
                        FP.FightMaxHp = hp; 
                        FP.FightHP = hp;
                    } 
                }
                if (levelupnum>0) {//如果升级了，不论是本机玩家还是网络玩家，都播放升级特效
                    //播放升级特效
                    if (m_LevelUp.gameObject.activeSelf) {
                        m_LevelUp.gameObject.SetActive(false);
                    }
                    m_LevelUp.gameObject.SetActive(true);
                    ThreadManager.Event.AddEvent(3f, () => {
                        m_LevelUp.gameObject.SetActive(false);
                    });
                }
                
            }
            
        }

    }
}


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
using GF.MainGame.UI;
using GF.NetWork;
using GF.Service;
using Net.Client;
using Net.Share;
using System.Collections.Generic;
using Titansiege;
using UnityEngine;
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
        
        public GameObject m_Selected;//角色脚下的选中目标
        private NPCBase m_AttackTarget = null;//攻击的目标

        public bool m_IsNetPlayer = false;//是否是网络对象，也就是费本机玩家
        public bool isPlaySkill = false;//是否正在播放技能
        public NPCBase AttackTarget {
            set {
                m_AttackTarget = value;
                if (m_NpcType != NpcType.monster) {//怪物不能给别人加选中，只有玩家可以
                    if (m_AttackTarget == null) {
                        m_AttackTarget.m_Selected.SetActive(false);
                    } else {
                        m_AttackTarget.m_Selected.SetActive(true);
                    }
                }
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
        /// <summary>
        /// 玩家不用这里的数值，这里是配置表的数值
        /// 只有怪物采用配置表的数值
        /// </summary>
        public NPCDataBase Data {
            get {
                if (m_Data == null) {
                    m_Data = ConfigerManager.m_NPCData.FindNPCByID(m_Id);
                    FightLevel = m_Data.Level;
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
                    if (UserService.GetInstance.m_CurrentChar != null && UserService.GetInstance.m_CurrentChar.Skills != "") {
                        string[] strarr = UserService.GetInstance.m_CurrentChar.Skills.Split('|');
                        for (int i = 0; i < strarr.Length; i++) {
                            if (!string.IsNullOrEmpty(strarr[i])) { //数据库存储最后一个字符时“|”,所以会出现一个空白项
                                m_SkillId.Add(int.Parse(strarr[i]));
                            }
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
                        Attack = (Data.Liliang+FightLevel*LevelData.Liliang) * 10;
                        Defense = (Data.Liliang + FightLevel * LevelData.Liliang) * 3 + (Data.Tizhi + FightLevel * LevelData.Tizhi) * 7;
                        break;
                    case (int)Zhiye.法师:
                        Attack = (Data.Moli + FightLevel * LevelData.Moli) * 10;
                        Defense = (Data.Moli + FightLevel * LevelData.Moli) * 4 + (Data.Tizhi + FightLevel * LevelData.Tizhi) * 6;
                        jcDodge = 0.02f;
                        break;
                    case (int)Zhiye.游侠:
                        Attack = (Data.Minjie + FightLevel * LevelData.Minjie) * 10;
                        Defense = (Data.Minjie + FightLevel * LevelData.Minjie) * 4 + (Data.Tizhi + FightLevel * LevelData.Tizhi) * 6;
                        jcDodge = 0f;
                        break;
                    default:
                        break;
                }
                //闪避,基础闪避率0.01f;
                Dodge = jcDodge + (float)(Data.Minjie + FightLevel * LevelData.Minjie) / 1000f >= 0.3f ? 0.3f : (float)(Data.Minjie + FightLevel * LevelData.Minjie ) / 1000f;//属性加成的闪避
                Crit = jcCrit + (float)(Data.Xingyun + FightLevel * LevelData.Xingyun) * jcCrit >= 0.5f ? 0.5f : (float)(Data.Xingyun + FightLevel * LevelData.Xingyun) * jcCrit;//暴击率
                FightMaxHp = (Data.Shengming + FightLevel * LevelData.Shengming) + (Data.Tizhi + FightLevel * LevelData.Tizhi) * 10;//战斗时最大生命
                FightHP = FightMaxHp;//战斗生命
                FightMaxMagic = (Data.Fali + FightLevel * LevelData.Fali) + (Data.Moli + FightLevel * LevelData.Moli) * 10;//战斗最大法力
                FightMagic = FightMaxMagic;//战斗法力
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
        public float m_Resetidletime;
        private bool iswaitidle = false;
        public virtual void SetFight(bool fight) {
            IsFight = fight;
            int stateid = 0;
            if (IsFight) {
                stateid = m_AllStateID["fightidle"];
            } else {
                stateid = m_AllStateID["idle"];
            }
            m_State.StatusEntry(stateid);
            if (!m_IsNetPlayer) {//如果不是网络对象，就发送操作命令给服务器
                Operation cmd = new Operation(Command.SwitchState, ClientBase.Instance.UID);
                cmd.index1 = stateid;
                ClientBase.Instance.AddOperation(cmd);
            }
            //开一个计时器，如果一段时间之内没有发动任何技能或者收到任何伤害，就切换为非战斗状态。
            //任意的技能和伤害都会重置这个时间
            if (fight&& iswaitidle==false) {
                m_Resetidletime = AppConfig.FightReset;
                iswaitidle = true;
                _ = SetIdleTimer();
            }
           
        }
        private async UniTaskVoid SetIdleTimer() {
            await UniTask.WaitUntil(() => m_Resetidletime <= 0) ;
            SetFight(false);
            iswaitidle = false;
        }
        /// <summary>
        /// 如果是网络角色，movemodule脚本不会控制它，所以需要自己判断切换跑步和待机动画
        /// </summary>
        protected Vector3 oldPosition;//网络对象上一帧同步过来的位置
        protected Vector3 newPosition;//网络对象本帧同步过来的位置
        protected Vector3 oldoldPosition;//网络对象上上一帧同步过来的位置，用来减少移动中的频繁改变动画状态的情况，只有三帧都相同，才让切换,相当于延迟一帧
        protected float time;
       
        public virtual void LateUpdate() {
            if (m_Resetidletime > 0) {
                m_Resetidletime -= Time.deltaTime;
            }
            //if (!isPlaySkill&&m_IsNetPlayer && (Time.time > time)) {
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
            if (m_Selected!=null) {
                NPCModule npcmodule = AppTools.GetModule<NPCModule>(MDef.NPCModule);
                if (m_GDID != ClientBase.Instance.UID && npcmodule != null && npcmodule.m_CurrentSelected != this) {
                    AppTools.Send<NPCBase>((int)NpcEvent.ChangeSelected, this);
                    UserService.GetInstance.m_CurrentPlayer.AttackTarget = this;
                }
            }
            
        }
        #region 战斗属性
        public bool m_IsDie = false;
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
                    //m_IsDie = true;
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
                m_FightLevel = value-1;
                //通知ui层数据发生变化

            }
        }
        #endregion
    }
}


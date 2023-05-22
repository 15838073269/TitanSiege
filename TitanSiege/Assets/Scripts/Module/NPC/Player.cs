/****************************************************
    文件：Player.cs
	作者：昔莘
    邮箱: 304183153@qq.com
    日期：2021/11/29 15:36:19
	功能：Nothing
*****************************************************/


using GameDesigner;
using GF.ConfigTable;
using GF.MainGame.Data;
using Net.Client;
using Net.Share;
using System.Collections.Generic;
using UnityEngine;

namespace GF.MainGame.Module.NPC {
    public class Player : NPCBase {
        
        public string m_PlayerName;
        
        //public int m_Infoid;//数据库Characterdata表的id
        public override void Start() {
            base.Start();
            SetFight(false);
        }
       
        public override void InitNPCAnimaor() {
            m_Nab = transform.GetComponent<PlayerAnimator>();
            if (m_Nab == null) {
                m_Nab = transform.gameObject.AddComponent<PlayerAnimator>();
                m_Nab.Init();
            }
            ChangeWp();//切换以下武器
        }

        internal void Check() {//检查角色是否死亡并同步生命值
            if (FightHP<=0 &&!m_IsDie) { //服务器同步的血量已经小于等于0了。此时客户端还没死掉
                m_IsDie = true;
                FightHP = 0;
                m_State.StatusEntry(m_AllStateID["die"]);
            }
        }
        // <summary>
        /// 获得animator下某个动画片段的时长方法
        /// </summary>
        /// <param animator="animator">Animator组件</param> 
        /// <param name="name">要获得的动画片段名字</param>
        /// <returns></returns>
        public float GetAnimatorSeconds(Animator animator, string name) {
            //动画片段时间长度
            float length = 0;
            AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;
            foreach (AnimationClip clip in clips) {
                if (clip.name.Equals(name)) {
                    length = clip.length;
                    break;
                }
            }
            return length;
        }
        public int GetAnimatorMilliseconds(Animator animator, string name) {
            //动画片段时间长度
            int length = 0;
            AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;
            foreach (AnimationClip clip in clips) {
                if (clip.name.Equals(name)) {
                    length = (int)(clip.length * 1000f);
                    break;
                }
            }
            return length;
        }
    }
    #region GDNet的状态机
    public class PIdleState : StateBehaviour {
        //private Player m_Self;
        //public override void OnInit() {
        //    m_Self = transform.GetComponent<Player>();
        //}

        //public override void OnUpdate() {
        //    if (m_Self.m_IsNetPlayer) { 
        //        return;
        //    }
        //}
    }
    public class PRunState : StateBehaviour {

    }
    public class PFightIdleState : StateBehaviour {
    }
    public class PFightRunState : StateBehaviour {
    }
    public class PAttackState : ActionCore {
        private Player m_Self;
        private int m_SkillID;
        private SkillDataBase m_SData;
        public override void OnInit() {
            m_Self = transform.GetComponent<Player>();
            m_SkillID = m_Self.m_SkillId[0];
            m_SData = ConfigerManager.m_SkillData.FindNPCByID(m_SkillID);
            if (m_Self.IsFight == false) {
                m_Self.SetFight(true);
                m_Self.ChangeWp();//切换以下武器
            }
            m_Self.m_Resetidletime = AppConfig.FightReset;
        }
        /// <summary>
        /// 动画帧事件处理方法
        /// </summary>
        /// <param name="action"></param>
        public override void OnAnimationEvent(StateAction action) {
            if (!m_Self.m_IsDie) {
                m_Self.m_Nab.Attack(m_SData);
                _= AppTools.SendReturn<SkillDataBase, NPCBase, int>((int)SkillEvent.CountSkillHurt, m_SData, m_Self);
                
            }
        }
        public override void OnExit(StateAction action) {
            m_Self.isPlaySkill = false;
            m_Self.m_Nab.HiddenEffect();
        }
    }
    //public class PSkill1State : StateBehaviour {
    //    private Player m_Self;
    //    private int m_SkillID;
    //    SkillDataBase sd;
    //    public override void OnInit() {
    //        m_Self = transform.GetComponent<Player>();
    //        m_SkillID = m_Self.m_SkillId[1];
    //        sd = ConfigerManager.m_SkillData.FindNPCByID(m_SkillID);
    //    }
    //    public override void OnEnter() {
    //        if (m_Self.IsFight==false) {
    //            m_Self.SetFight(true);
    //            m_Self.ChangeWp();//切换以下武器
    //        }
    //        m_Self.m_Resetidletime = AppConfig.FightReset;
    //        m_Self.m_Nab.Attack(sd);
    //        AppTools.SendReturn<SkillDataBase, NPCBase, int>((int)SkillEvent.CountSkillHurt, sd, m_Self);//发送消息让技能模块计算伤害
    //    }

    //    public override void OnExit() {
    //        m_Self.isPlaySkill = false;
    //        m_Self.m_Nab.HiddenEffect();
    //    }
    //}
  
    public class PSkill1Action : MyAcitonCore {
        private Player m_Self;
        private int m_SkillID;
        private SkillDataBase m_SData;
       
        public override void OnInit() {
            m_Self = transform.GetComponent<Player>();
            m_SkillID = m_Self.m_SkillId[1];
            m_SData = ConfigerManager.m_SkillData.FindNPCByID(m_SkillID);
            //初始化技能配置表中的事件
            for (int i = 0; i < m_SData.skilleventlist.Count; i++) {
                EventArg e = new EventArg();
                e.eventType = (Const.EventType)m_SData.skilleventlist[i].eventtype;
                e.animEventTime = m_SData.skilleventlist[i].eventtime;
                e.eventEnter = false;
            }
        }
        
        /// <summary>
        /// 动画帧事件处理方法
        /// </summary>
        /// <param name="action"></param>
        public override void OnAnimationEvent(StateAction action,EventArg eventarg) {
            switch (eventarg.eventType) {
                case Const.EventType.showeff:
                    //播放特效
                    if (effectSpwan != null) {
                        if (activeMode == ActiveMode.Instantiate)
                            Object.Destroy(InstantiateSpwan(stateManager), spwanTime);
                        else if (activeMode == ActiveMode.ObjectPool) {
                            bool active = false;
                            foreach (GameObject go in activeObjs) {
                                if (go == null) {
                                    activeObjs.Remove(go);
                                    break;
                                }
                                if (!go.activeSelf) {
                                    go.SetActive(true);
                                    go.transform.SetParent(null);
                                    SetPosition(stateManager, go);
                                    active = true;
                                    GameDesigner.StateEvent.AddEvent(spwanTime, () =>
                                    {
                                        if (go != null) go.SetActive(false);
                                    });
                                    break;
                                }
                            }
                            if (!active) {
                                var go = InstantiateSpwan(stateManager);
                                activeObjs.Add(go);
                                GameDesigner.StateEvent.AddEvent(spwanTime, () =>
                                {
                                    if (go != null) go.SetActive(false);
                                });
                            }
                        } else {
                            effectSpwan.gameObject.SetActive(true);
                            SetPosition(stateManager, effectSpwan.gameObject);
                        }
                    }
                    break;
                case Const.EventType.hiddeneff:
                    //EffectArg arg = obj as EffectArg;
                    //if (arg != null) {
                    //    if (arg.CurrentEffect != null) {
                    //        if (!arg.CurrentEffect.m_IsFollow) {
                    //            arg.CurrentEffect.transform.parent = m_EffectFather;
                    //            arg.CurrentEffect.transform.localPosition = arg.LastEffectLocalPos;
                    //            arg.CurrentEffect.transform.localRotation = arg.LastEffectLocalRotate;
                    //        }
                    //        arg.CurrentEffect.gameObject.SetActive(false);
                    //    }
                    //}
                    break;
                case Const.EventType.attack:
                    if (m_SData.usecollider != 0) {//使用碰撞
                        RaycastHit[] hitarr = Physics.BoxCastAll(m_Self.transform.position, new Vector3(2f, 2f, 2f), m_Self.transform.forward, Quaternion.identity, 5f);
                        if (hitarr.Length > 0) {
                            List<NPCBase> mlist = new List<NPCBase>();
                            for (int j = 0; j < hitarr.Length; j++) {
                                mlist.Add(hitarr[j].transform.GetComponent<Monster>());
                            }
                            AppTools.SendReturn<SkillDataBase, NPCBase, List<NPCBase>, int>((int)SkillEvent.CountSkillHurt, m_SData, m_Self, mlist);//发送消息让技能模块计算伤害
                        }
                    } else { //不使用碰撞
                        AppTools.SendReturn<SkillDataBase, NPCBase, int>((int)SkillEvent.CountSkillHurt, m_SData, m_Self);//发送消息让技能模块计算伤害
                    }
                    break;
                case Const.EventType.weiyi:
                    float sec = m_Self.GetAnimatorSeconds(m_Self.m_Nab.m_ani, m_SData.texiao);
                    m_Self.isPlaySkill = true;
                    //这里目前只完成了位移技能的特效，但技能特效不只位置，需再开发
                    if (!m_Self.m_IsNetPlayer) {//如果不是网络对象，就自行控制特效移动
                        if (m_SData.weiyi > 0) {//速度只要不为0，就会移动
                            float t = m_SData.weiyi / sec;
                            AppTools.Send<float, bool>((int)MoveEvent.SetSkillMove, t, true);
                        } else { //速度如果为0，就是发送不能移动的命令
                            AppTools.Send<float, bool>((int)MoveEvent.SetSkillMove, 0f, true);
                        }
                    }
                    break;
                
                default:
                    break;
            }
        }
        public override void OnExit(StateAction action) {
            base.OnExit(action);
            effectSpwan.gameObject.SetActive(false);
            m_Self.isPlaySkill = false;
            m_Self.m_Nab.m_CurrentEffect = null;
            AppTools.Send<float, bool>((int)MoveEvent.SetSkillMove, 0f, false);
        }
    }
    public class PSkill2State : StateBehaviour {
        private Player m_Self;
        private int m_SkillID;
        SkillDataBase sd;
        public override void OnInit() {
            m_Self = transform.GetComponent<Player>();
            m_SkillID = m_Self.m_SkillId[2];
            sd = ConfigerManager.m_SkillData.FindNPCByID(m_SkillID);
        }
        public override void OnEnter() {
            if (m_Self.IsFight == false) {
                m_Self.SetFight(true);
                m_Self.ChangeWp();//切换以下武器
            }
            m_Self.m_Resetidletime = AppConfig.FightReset;
            m_Self.m_Nab.Attack(sd);
            AppTools.SendReturn<SkillDataBase, NPCBase,int>((int)SkillEvent.CountSkillHurt, sd, m_Self);//发送消息让技能模块计算伤害
        }
        public override void OnExit() {
            m_Self.isPlaySkill = false;
            m_Self.m_Nab.HiddenEffect();
        }
    }
    public class PSkill3State : StateBehaviour {
        private Player m_Self;
        private int m_SkillID;
        SkillDataBase sd;
        public override void OnInit() {
            m_Self = transform.GetComponent<Player>();
            m_SkillID = m_Self.m_SkillId[3];
            sd = ConfigerManager.m_SkillData.FindNPCByID(m_SkillID);
        }
        public override void OnEnter() {
            if (m_Self.IsFight == false) {
                m_Self.SetFight(true);
                m_Self.ChangeWp();//切换以下武器
            }
            m_Self.m_Resetidletime = AppConfig.FightReset;
            m_Self.m_Nab.Attack(sd);
            AppTools.SendReturn<SkillDataBase, NPCBase, int>((int)SkillEvent.CountSkillHurt, sd, m_Self);//发送消息让技能模块计算伤害
        }
        
        public override void OnExit() {
            m_Self.isPlaySkill = false;
            m_Self.m_Nab.HiddenEffect();
        }
    }
    public class PSkill4State : StateBehaviour {
        private Player m_Self;
        private int m_SkillID;
        SkillDataBase sd;
        public override void OnInit() {
            m_Self = transform.GetComponent<Player>();
            m_SkillID = m_Self.m_SkillId[4];
            sd = ConfigerManager.m_SkillData.FindNPCByID(m_SkillID);
        }
        public override void OnEnter() {
            if (m_Self.IsFight == false) {
                m_Self.SetFight(true);
                m_Self.ChangeWp();//切换以下武器
            }
            m_Self.m_Resetidletime = AppConfig.FightReset;
            m_Self.m_Nab.Attack(sd);
            AppTools.SendReturn<SkillDataBase, NPCBase, int>((int)SkillEvent.CountSkillHurt, sd, m_Self);//发送消息让技能模块计算伤害
        }
        public override void OnExit() {
            m_Self.isPlaySkill = false;
            m_Self.m_Nab.HiddenEffect();
        }
    }
    #endregion
}

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
using GF.MainGame.UI;
using GF.Service;
using Net.Client;
using Net.Share;
using System;

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
    public class PSkill1State : StateBehaviour {
        private Player m_Self;
        private int m_SkillID;
        SkillDataBase sd;
        public override void OnInit() {
            m_Self = transform.GetComponent<Player>();
            m_SkillID = m_Self.m_SkillId[1];
            sd = ConfigerManager.m_SkillData.FindNPCByID(m_SkillID);
        }
        public override void OnEnter() {
            if (m_Self.IsFight==false) {
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

/****************************************************
    文件：Player.cs
	作者：昔莘
    邮箱: 304183153@qq.com
    日期：2021/11/29 15:36:19
	功能：Nothing
*****************************************************/


using GameDesigner;
using GF.ConfigTable;
using GF.Const;
using GF.MainGame.Data;
using GF.NetWork;
using GF.Service;
using MoreMountains.Feedbacks;
using Net.Client;
using Net.Share;
using Net.System;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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
            
            if (FP.FightHP <=0) { //服务器同步的血量已经小于等于0了。此时客户端还没死掉
                m_IsDie = true;
                FP.FightHP = 0;
                ChangeState(m_AllStateID["die"]);
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
        /// <summary>
        /// 玩家复活
        /// </summary>
        public void Fuhuo() {
            Debuger.Log("准备复活");
            FP.FightHP = FP.FightMaxHp;
            FP.FightMagic = FP.FightMaxMagic;
            //还得加上是否显示血条判断
            if (AppTools.GetModule<HPModule>(MDef.HPModule).IsShowHp) {
                AppTools.Send<NPCBase>((int)HPEvent.ShowHP, this);
            }
            m_Resetidletime = 0;
            SetFight(false);
            m_IsDie = false;
            //更新血条
            AppTools.Send<NPCBase>((int)HPEvent.UpdateHp, this);
            //发送服务器玩家复活
            if (m_GDID==ClientBase.Instance.UID) {
                //隐藏死亡ui
                AppTools.Send((int)DieEvent.HideUI);
                ChangeState(m_AllStateID["idle"]);
            }
        }
    }
    #region GDNet的状态机
    public class PDieState : StateBehaviour {
        private Player m_Self;
        private ushort m_EnterStop = 0;//因为只要不脱离状态，死亡动画播放完成后，onstop会一直调用，需要加个判断，避免onstop中的代码重复调用
        public override void OnInit() {
            m_Self = transform.GetComponent<Player>();
        }
        public override void OnEnter() {
            //取消选中状态
            if (m_Self.m_GDID == ClientBase.Instance.UID) {//只有本机玩家才需要取消选中
                //死亡后直接黑屏，禁止任何ui操作
                AppTools.Send((int)DieEvent.ShowUI);
                AppTools.Send<NPCBase>((int)NpcEvent.CanelSelected, null);
            }
            m_Self.AttackTarget = null;
        }
        //注意onstop只要一直处于状态，就会不停调用执行，这里得处理一下，避免重复
        public override void OnStop() {
            if (m_EnterStop == 0) {
                if (m_Self.m_GDID == ClientBase.Instance.UID) {
                    //播放完死亡动画，隐藏血条
                    AppTools.Send<NPCBase>((int)HPEvent.HideHP, m_Self);
                }
                m_EnterStop++;
            }
        }
        public override void OnExit() {
            m_EnterStop = 0;
        }
    }
    public class PIdleState : StateBehaviour {

    }
    public class PRunState : StateBehaviour {

    }
    public class PFightIdleState : StateBehaviour {
    }
    public class PFightRunState : StateBehaviour {
    }
   //技能的处理单独成脚本了，请查看PSkillAction.cs
    #endregion
}

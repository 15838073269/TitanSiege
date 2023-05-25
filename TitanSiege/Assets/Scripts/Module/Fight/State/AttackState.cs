/****************************************************
    文件：AttackState.cs
	作者：昔莘
    邮箱: 304183153@qq.com
    日期：2022/11/19 0:1:34
	功能：战斗攻击
*****************************************************/
using DG.Tweening.Core;
using GF.ConfigTable;
using GF.Const;
using GF.MainGame.Data;
using GF.MainGame.Module.NPC;
using Net.Client;
using Net.Config;
using System.Collections;
using UnityEngine;
namespace GF.MainGame.Module.Fight {
    /// <summary>
    /// 攻击状态
    /// </summary>
    public class AttackState : StateBase {
        public AttackState() {
            Init("Attack");
        }
        public override void Enter(NPCBase npc, object args = null) {
            base.Enter(npc, args);
            //npc.CurrentState = Const.AniState.attack;
            
        }

        public override void Exit(NPCBase npc, object args = null) {
            base.Exit(npc,args);
            npc.m_Nab.HiddenEffect();
        }

        public override void Process(NPCBase npc, object args = null) {
            base.Process(npc,args);
            SkillDataBase sd = null;
            if (npc.m_GDID != ClientBase.Instance.UID) {
                sd = ConfigerManager.m_SkillData.FindNPCByID((int)args);
            } else {
                if (args != null) {
                    sd = (SkillDataBase)args;
                }
            }
            npc.m_Nab.Attack(sd);
            AppTools.Send<SkillDataBase, NPCBase>((int)SkillEvent.CountSkillHurt,sd,npc);//发送消息让技能模块计算伤害
        }
        
    }
}
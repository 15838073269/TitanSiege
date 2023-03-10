/****************************************************
    文件：PlayerAnimator.cs
	作者：昔莘
    邮箱: 304183153@qq.com
    日期：2022/11/20 0:21:28
	功能：Nothing
*****************************************************/

using GF.Const;
using Net.System;
using GF.MainGame.Data;
using UnityEngine;
using Net.Client;
using Net.Share;
using GF.NetWork;
using GF.ConfigTable;

namespace GF.MainGame.Module.NPC {
    public class PlayerAnimator : NPCAnimatorBase {
        
        /// <summary>
        /// 攻击动画
        /// </summary>
        public override void Attack(SkillDataBase sd = null) {
            
            Player npc = transform.GetComponent<Player>();
            if (!npc.m_IsNetPlayer) {//如果不是网络对象，就发送操作命令给服务器
                Operation cmd = new Operation(Command.Skill, ClientBase.Instance.UID);
                cmd.index1 = sd.id;
                ClientBase.Instance.AddOperation(cmd);
            }
            m_ani.SetInteger("action", sd.donghua);
            float sec = GetAnimatorSeconds(m_ani, sd.texiao);
            npc.isPlaySkill = true;
            if (!npc.m_IsNetPlayer) {//如果不是网络对象，就自行控制移动
                if (sd.skillefflist.Count > 0) {//速度只要不为0，就会移动
                    float t = float.Parse(sd.skillefflist[0].xiaoguo1) / sec;
                    AppTools.Send<float, bool>((int)MoveEvent.SetSkillMove, t, true);
                } else { //速度如果为0，就是发送不能移动的命令
                    AppTools.Send<float, bool>((int)MoveEvent.SetSkillMove, 0f, true);
                }
            }
            ///因为特效资源与动画资源不同步而多停顿0.2f
            ThreadManager.Event.AddEvent(sec+0.2f, ExitAttack, npc);
        }
        
        public void ExitAttack(object obj) {
            Player npc = obj as Player;
            npc.isPlaySkill = false;
            AppTools.Send<NPCBase, AniState>((int)StateEvent.ChangeState, npc, AniState.wait);
           
        }
        public override void HiddenEffect() {
            base.HiddenEffect();
            m_ani.SetInteger("action", -1);
            AppTools.Send<float, bool>((int)MoveEvent.SetSkillMove, 0f, false);
        }
        
    }
}
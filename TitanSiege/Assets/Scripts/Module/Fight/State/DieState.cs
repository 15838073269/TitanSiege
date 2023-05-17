/****************************************************
    文件：RunState.cs
	作者：昔莘
    邮箱: 304183153@qq.com
    日期：2022/11/19 0:1:34
	功能：死亡状态
*****************************************************/
using GF.MainGame.Module.NPC;
using UnityEngine;
namespace GF.MainGame.Module.Fight {
    /// <summary>
    /// 移动状态
    /// </summary>
    public class DieState : StateBase {
        public DieState() {
            Init("Run");
        }
        public override void Enter(NPCBase npc, object args = null) {
            base.Enter(npc, args);
            //npc.CurrentState = Const.AniState.die;
        }

        public override void Exit(NPCBase npc, object args = null) {
            base.Exit(npc, args);
        }

        public override void Process(NPCBase npc, object args = null) {
            base.Process(npc, args);
            npc.m_Nab.Die();
        }
    }
}
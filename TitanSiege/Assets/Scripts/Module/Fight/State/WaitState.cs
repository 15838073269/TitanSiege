/****************************************************
    文件：WaitState.cs
	作者：昔莘
    邮箱: 304183153@qq.com
    日期：2022/11/19 0:1:25
	功能： 战斗等待状态
*****************************************************/
using GF.MainGame.Module.NPC;
using UnityEngine;
namespace GF.MainGame.Module.Fight {
    /// <summary>
    /// 战斗等待状态
    /// </summary>
    public class WaitState : StateBase {
        public WaitState() {
            Init("wait");
        }
        public override void Enter(NPCBase npc, object args =null) {
            base.Enter(npc, args);
            //npc.CurrentState = Const.AniState.wait;
        }

        public override void Exit(NPCBase npc, object args = null) {
            base.Exit(npc, args);
        }

        public override void Process(NPCBase npc, object args = null) {
            base.Process(npc, args);
            npc.m_Nab.Idle();
        }
    }
}
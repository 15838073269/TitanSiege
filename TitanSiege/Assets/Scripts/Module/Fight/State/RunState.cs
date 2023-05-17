/****************************************************
    文件：RunState.cs
	作者：昔莘
    邮箱: 304183153@qq.com
    日期：2022/11/19 0:1:34
	功能：移动状态
*****************************************************/
using GF.MainGame.Module.NPC;
namespace GF.MainGame.Module.Fight {
    /// <summary>
    /// 移动状态
    /// </summary>
    public class RunState : StateBase {
        public RunState() {
            Init("Run");
        }
        public override void Enter(NPCBase npc, object args = null) {
            base.Enter(npc, args);
            //npc.CurrentState = Const.AniState.run;
        }

        public override void Exit(NPCBase npc, object args = null) {
            base.Exit(npc, args);
        }

        public override void Process(NPCBase npc, object args = null) {
            base.Process(npc, args);
            npc.m_Nab.Move();
        }
    }
}
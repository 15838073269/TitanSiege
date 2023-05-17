using GF.MainGame.Module.NPC;
namespace GF.MainGame.Module.Fight {
    public class HurtState : StateBase {
        public HurtState() {
            Init("Hurt");
        }
        public override void Enter(NPCBase npc, object args = null) {
            base.Enter(npc, args);
            //npc.CurrentState = Const.AniState.hurt;
        }

        public override void Exit(NPCBase npc, object args = null) {
            base.Exit(npc, args);
        }

        public override void Process(NPCBase npc, object args = null) {
            base.Process(npc, args);
            npc.m_Nab.Hurt();
        }
    }
}


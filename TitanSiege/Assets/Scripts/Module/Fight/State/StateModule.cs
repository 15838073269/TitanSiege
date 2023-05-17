/****************************************************
    文件：StateModule.cs
	作者：昔莘
    邮箱: 304183153@qq.com
    日期：2022/11/18 23:51:7
	功能：战斗状态管理
*****************************************************/
using GF.Const;
using GF.MainGame.Module.Fight;
using GF.MainGame.Module.NPC;
using GF.Module;
using System.Collections.Generic;
namespace GF.MainGame.Module {
    public class StateModule : GeneralModule {
        /// <summary>
        /// 状态机
        /// </summary>
        private Dictionary<AniState,IState> FSM;
        public override void Create() {
            base.Create();
            //添加状态机
            FSM = new Dictionary<AniState, IState>();
            FSM.Add(AniState.wait,new WaitState());
            FSM.Add(AniState.run, new RunState());
            FSM.Add(AniState.fightrun, new FightRunState());
            FSM.Add(AniState.idle, new IdleState());
            FSM.Add(AniState.attack, new AttackState());
            FSM.Add(AniState.die, new DieState());
           // AppTools.Regist<NPCBase, AniState>((int)StateEvent.ChangeState, ChangeState);
           // AppTools.Regist<NPCBase, AniState, object>((int)StateEvent.ChangeStateWithArgs, ChangeStateWithArgs);
        }
        /// <summary>
        /// 状态变更
        /// </summary>
        /// <param name="npc"></param>
        /// <param name="targetState"></param>
        public void ChangeState(NPCBase npc,AniState targetState) {
            //if (npc.CurrentState == targetState) {
            //    return;
            //}
            
            //if (FSM.ContainsKey(targetState)) {
            //    if (npc.CurrentState!=AniState.none) {
            //        FSM[npc.CurrentState].Exit(npc);
            //    }
            //    FSM[targetState].Enter(npc);
            //    FSM[targetState].Process(npc); 
            //}
        }
        public void ChangeStateWithArgs(NPCBase npc, AniState targetState,object args) {
            //if (npc.CurrentState == targetState) {
            //    return;
            //}

            //if (FSM.ContainsKey(targetState)) {
            //    if (npc.CurrentState != AniState.none) {
            //        FSM[npc.CurrentState].Exit(npc,args);
            //    }
            //    FSM[targetState].Enter(npc, args);
            //    FSM[targetState].Process(npc, args);
            //}
        }
        public override void Release() {
            base.Release();
        }
    }
}

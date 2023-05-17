/****************************************************
    文件：IState.cs
	作者：昔莘
    邮箱: 304183153@qq.com
    日期：2022/11/18 23:52:29
	功能：战斗状态接口
*****************************************************/

using GF.MainGame.Module.NPC;
namespace GF.MainGame.Module.Fight {
    interface IState {
        /// <summary>
        /// 进入状态
        /// </summary>
        void Enter(NPCBase npc,object args=null);
        /// <summary>
        /// 处理状态
        /// </summary>
        void Process(NPCBase npc,object args=null);
        /// <summary>
        /// 离开状态
        /// </summary>
        void Exit(NPCBase npc,object args = null);
    }
    public class StateBase : IState {
        public string StateName = "StateBase";
        public virtual void Init(string name) {
            StateName = name;
        }
        public virtual void Enter(NPCBase npc, object args = null) {
            //Debuger.Log("进入"+StateName);
        }

        public virtual void Exit(NPCBase npc, object args = null) {
            //Debuger.Log("离开" + StateName);
        }

        public virtual void Process(NPCBase npc, object args = null) {
            //Debuger.Log("处理" + StateName);
        }
    }

}

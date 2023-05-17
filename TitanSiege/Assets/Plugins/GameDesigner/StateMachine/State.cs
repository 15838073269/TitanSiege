using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GameDesigner
{
    /// <summary>
    /// 动画模式
    /// </summary>
    public enum AnimationMode
    {
        /// <summary>
        /// 旧版动画
        /// </summary>
        Animation,
        /// <summary>
        /// 新版动画
        /// </summary>
        Animator
    }

    /// <summary>
    /// 动画播放模式
    /// </summary>
	public enum AnimPlayMode
    {
        /// <summary>
        /// 随机播放动画
        /// </summary>
		Random,
        /// <summary>
        /// 顺序播放动画
        /// </summary>
		Sequence,
    }

    /// <summary>
    /// 状态 -- v2017/12/6
    /// </summary>
    [Serializable]
    public sealed class State : StateBase
    {
        /// <summary>
        /// 状态连接集合
        /// </summary>
		public List<Transition> transitions = new List<Transition>();
        /// <summary>
        /// 动作系统 使用为真 , 不使用为假
        /// </summary>
		public bool actionSystem = false;
        /// <summary>
        /// 动画循环?
        /// </summary>
        public bool animLoop = true;
        /// <summary>
        /// 动画模式
        /// </summary>
        public AnimPlayMode animPlayMode = AnimPlayMode.Random;
        /// <summary>
        /// 动作索引
        /// </summary>
		public int actionIndex = 0;
        /// <summary>
        /// 动画速度
        /// </summary>
		public float animSpeed = 1;
        /// <summary>
        /// 动画结束是否进入下一个状态
        /// </summary>
        public bool isExitState = false;
        /// <summary>
        /// 动画结束进入下一个状态的ID
        /// </summary>
        public int DstStateID = 0;
        /// <summary>
        /// 状态动作集合
        /// </summary>
		public List<StateAction> actions = new List<StateAction>();

        public State() { }

#if UNITY_EDITOR
        /// <summary>
        /// 创建状态
        /// </summary>
        public static State CreateStateInstance(StateMachine stateMachine, string stateName, Vector2 position)
        {
            var state = new State(stateMachine);
            state.name = stateName;
            state.rect.position = position;
            return state;
        }
#endif

        /// <summary>
        /// 构造函数
        /// </summary>
        public State(StateMachine _stateMachine)
        {
            stateMachine = _stateMachine;
            ID = stateMachine.states.Count;
            stateMachine.states.Add(this);
            actions.Add(new StateAction() { ID = ID, stateMachine = stateMachine });
            stateMachine.UpdateStates();
        }

        /// <summary>
        /// 当前状态动作
        /// </summary>
        public StateAction Action => actions[actionIndex % actions.Count];

        /// <summary>
        /// 进入状态
        /// </summary>
		public void OnEnterState()
        {
            var action = Action;
            if (animPlayMode == AnimPlayMode.Random)//选择要进入的动作索引
                actionIndex = Random.Range(0, actions.Count);
            else
                actionIndex++;
            foreach (ActionBehaviour behaviour in action.behaviours) //当子动作的动画开始进入时调用
                if (behaviour.Active)
                    behaviour.OnEnter(Action);
            switch (stateMachine.animMode)
            {
                case AnimationMode.Animation:
                    stateMachine.animation[action.clipName].speed = animSpeed;
                    if (action.rewind) stateMachine.animation.Rewind(action.clipName);
                    stateMachine.animation.Play(action.clipName);
                    break;
                case AnimationMode.Animator:
                    stateMachine.animator.speed = animSpeed;
                    if(action.rewind) stateMachine.animator.Rebind();
                    stateMachine.animator.Play(action.clipName);
                    break;
            }
        }

        /// <summary>
        /// 状态每一帧
        /// </summary>
		public void OnUpdateState()
        {
            bool isPlaying = true;
            var action = Action;
            switch (stateMachine.animMode)
            {
                case AnimationMode.Animation:
                    var animState = stateMachine.animation[action.clipName];
                    action.animTime = animState.time / animState.length * 100f;
                    isPlaying = stateMachine.animation.isPlaying;
                    break;
                case AnimationMode.Animator:
                    action.animTime = stateMachine.animator.GetCurrentAnimatorStateInfo(0).normalizedTime / 1f * 100f;
                    break;
            }
            if (action.animTime >= action.animTimeMax | !isPlaying)
            {
                if (isExitState & transitions.Count > 0)
                {
                    transitions[DstStateID].isEnterNextState = true;
                    return;
                }
                if (animLoop)
                {
                    OnExitState();//退出函数
                    OnActionExit();
                    if (stateMachine.stateID == ID)//如果在动作行为里面有切换状态代码, 则不需要重载函数了, 否则重载当前状态
                        OnEnterState();//重载进入函数
                    return;
                }
                else OnStop();
            }
            foreach (ActionBehaviour behaviour in action.behaviours)
                if (behaviour.Active)
                    behaviour.OnUpdate(action);
        }

        /// <summary>
        /// 当退出状态
        /// </summary>
		public void OnExitState()
        {
            foreach (ActionBehaviour behaviour in Action.behaviours) //当子动作结束
                if (behaviour.Active)
                    behaviour.OnExit(Action);
        }

        /// <summary>
        /// 当子动作处于循环播放模式时, 子动作每次播放完成动画都会调用一次
        /// </summary>
        private void OnActionExit()
        {
            foreach (StateBehaviour behaviour in behaviours) //当子动作停止
                if (behaviour.Active)
                    behaviour.OnActionExit();
        }

        /// <summary>
        /// 当动作停止
        /// </summary>
        public void OnStop()
        {
            foreach (StateBehaviour behaviour in behaviours) //当子动作停止
                if (behaviour.Active)
                    behaviour.OnStop();
            foreach (ActionBehaviour behaviour in Action.behaviours) //当子动作停止
                if (behaviour.Active)
                    behaviour.OnStop(Action);
        }
    }
}
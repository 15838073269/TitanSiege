namespace GameDesigner
{
    using Net.Helper;
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public enum RuntimeInitMode 
    {
        Awake, Start,
    }

    /// <summary>
    /// 状态执行管理类
    /// V2017.12.6
    /// 版本修改V2019.8.27
    /// </summary>
    public sealed class StateManager : MonoBehaviour
    {
        /// <summary>
        /// 状态机
        /// </summary>
		public StateMachine stateMachine = null;
        public RuntimeInitMode initMode = RuntimeInitMode.Awake;

        void Awake()
        {
            if (initMode == RuntimeInitMode.Awake)
                Init();
        }

        void Start()
        {
            if (initMode == RuntimeInitMode.Start)
                Init();
        }

        private void Init()
        {
            if (stateMachine == null)
            {
                enabled = false;
                return;
            }
            if (stateMachine.GetComponentInParent<StateManager>() == null)//当使用本地公用状态机时
            {
                var sm = Instantiate(stateMachine, transform);
                sm.name = stateMachine.name;
                sm.transform.localPosition = Vector3.zero;
                if (sm.animation == null)
                    sm.animation = GetComponentInChildren<Animation>();
                else if (!sm.animation.gameObject.scene.isLoaded)
                    sm.animation = GetComponentInChildren<Animation>();
                if (sm.animator == null)
                    sm.animator = GetComponentInChildren<Animator>();
                else if (!sm.animator.gameObject.scene.isLoaded)
                    sm.animator = GetComponentInChildren<Animator>();
                stateMachine = sm;
            }
            foreach (var state in stateMachine.states)
            {
                for (int i = 0; i < state.behaviours.Count; i++)
                {
                    var behaviour = (StateBehaviour)state.behaviours[i].InitBehaviour();
                    state.behaviours[i] = behaviour;
                    behaviour.OnInit();
                }
                foreach (var t in state.transitions)
                {
                    for (int i = 0; i < t.behaviours.Count; i++)
                    {
                        var behaviour = (TransitionBehaviour)t.behaviours[i].InitBehaviour();
                        behaviour.transitionID = i;
                        t.behaviours[i] = behaviour;
                        behaviour.OnInit();
                    }
                }
                if (state.actionSystem)
                {
                    foreach (var action in state.actions)
                    {
                        for (int i = 0; i < action.behaviours.Count; i++)
                        {
                            var behaviour = (ActionBehaviour)action.behaviours[i].InitBehaviour();
                            action.behaviours[i] = behaviour;
                            behaviour.OnInit();
                        }
                    }
                }
            }
            if (stateMachine.defaultState.actionSystem)
                stateMachine.defaultState.OnEnterState();
        }

        private void Update()
        {
            OnState(stateMachine.currState);
        }

        /// <summary>
        /// 处理状态各种行为与事件方法
        /// </summary>
        /// <param name="state">要执行的状态</param>
        public void OnState(State state)
        {
            if (state.actionSystem)
                state.OnUpdateState();
            foreach (StateBehaviour behaviour in state.behaviours)
                if (behaviour.Active)
                    behaviour.OnUpdate();
            for (int i = 0; i < state.transitions.Count; i++)
                OnTransition(state.transitions[i]);
        }

        /// <summary>
        /// 处理连接行为线条方法
        /// </summary>
        /// <param name="transition">要执行的连接线条</param>
        public void OnTransition(Transition transition)
        {
            foreach (TransitionBehaviour behaviour in transition.behaviours)
                if (behaviour.Active)
                    behaviour.OnUpdate(ref transition.isEnterNextState);
            if (transition.model == TransitionModel.ExitTime)
            {
                transition.time += Time.deltaTime;
                if (transition.time > transition.exitTime)
                    transition.isEnterNextState = true;
            }
            if (transition.isEnterNextState)
            {
                EnterNextState(stateMachine.currState, transition.nextState);
                transition.time = 0;
                transition.isEnterNextState = false;
            }
        }

        /// <summary>
        /// 当退出状态时处理连接事件
        /// </summary>
        /// <param name="state">要退出的状态</param>
        public void OnStateTransitionExit(State state)
        {
            foreach (var transition in state.transitions)
                if (transition.model == TransitionModel.ExitTime)
                    transition.time = 0;
        }

        /// <summary>
        /// 当进入下一个状态
        /// </summary>
        /// <param name="currState">当前状态</param>
        /// <param name="enterState">要进入的状态</param>
        public void EnterNextState(State currState, State enterState)
        {
            foreach (StateBehaviour behaviour in currState.behaviours)//先退出当前的所有行为状态OnExitState的方法
                if (behaviour.Active)
                    behaviour.OnExit();
            OnStateTransitionExit(currState);
            foreach (StateBehaviour behaviour in enterState.behaviours)//最后进入新的状态前调用这个新状态的所有行为类的OnEnterState方法
                if (behaviour.Active)
                    behaviour.OnEnter();
            if (currState.actionSystem)
                currState.OnExitState();
            if (enterState.actionSystem)
                enterState.OnEnterState();
            stateMachine.stateID = enterState.ID;
        }

        /// <summary>
        /// 当进入下一个状态, 你也可以立即进入当前播放的状态, 如果不想进入当前播放的状态, 使用StatusEntry方法
        /// </summary>
        /// <param name="nextStateIndex">下一个状态的ID</param>
		public void EnterNextState(int nextStateIndex)
        {
            EnterNextState(stateMachine.currState, stateMachine.states[nextStateIndex]);
        }

        /// <summary>
        /// 进入下一个状态, 如果状态正在播放就不做任何处理, 如果想让动作立即播放可以使用 OnEnterNextState 方法
        /// </summary>
        /// <param name="stateID"></param>
        public void StatusEntry(int stateID)
        {
            if (stateMachine.stateID == stateID)
                return;
            EnterNextState(stateID);
        }

        private void OnDestroy()
        {
            if (stateMachine != null)
            {
                foreach (var state in stateMachine.states)
                {
                    foreach (var behaviour in state.behaviours)
                    {
                        behaviour.OnDestroyComponent();
                    }
                    foreach (var transition in state.transitions)
                    {
                        foreach (var behaviour in transition.behaviours)
                        {
                            behaviour.OnDestroyComponent();
                        }
                    }
                    foreach (var action in state.actions)
                    {
                        foreach (var behaviour in action.behaviours)
                        {
                            behaviour.OnDestroyComponent();
                        }
                    }
                }
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            OnScriptReload();
        }

        public void OnScriptReload()
        {
            if (stateMachine == null)
                return;
            foreach (var s in stateMachine.states)
            {
                for (int i = 0; i < s.behaviours.Count; i++)
                {
                    var type = AssemblyHelper.GetType(s.behaviours[i].name);
                    if (type == null)
                    {
                        s.behaviours.RemoveAt(i);
                        if (i >= 0) i--;
                        continue;
                    }
                    var metadatas = new List<Metadata>(s.behaviours[i].metadatas);
                    var active = s.behaviours[i].Active;
                    var show = s.behaviours[i].show;
                    s.behaviours[i] = (StateBehaviour)Activator.CreateInstance(type);
                    s.behaviours[i].Reload(type, stateMachine, metadatas);
                    s.behaviours[i].ID = s.ID;
                    s.behaviours[i].Active = active;
                    s.behaviours[i].show = show;
                }
                foreach (var t in s.transitions)
                {
                    for (int i = 0; i < t.behaviours.Count; i++)
                    {
                        var type = AssemblyHelper.GetType(t.behaviours[i].name);
                        if (type == null)
                        {
                            t.behaviours.RemoveAt(i);
                            if (i >= 0) i--;
                            continue;
                        }
                        var metadatas = new List<Metadata>(t.behaviours[i].metadatas);
                        var active = t.behaviours[i].Active;
                        var show = t.behaviours[i].show;
                        t.behaviours[i] = (TransitionBehaviour)Activator.CreateInstance(type);
                        t.behaviours[i].Reload(type, stateMachine, metadatas);
                        t.behaviours[i].ID = s.ID;
                        t.behaviours[i].Active = active;
                        t.behaviours[i].show = show;
                    }
                }
                foreach (var a in s.actions)
                {
                    for (int i = 0; i < a.behaviours.Count; i++)
                    {
                        var type = AssemblyHelper.GetType(a.behaviours[i].name);
                        if (type == null)
                        {
                            a.behaviours.RemoveAt(i);
                            if (i >= 0) i--;
                            continue;
                        }
                        var metadatas = new List<Metadata>(a.behaviours[i].metadatas);
                        var active = a.behaviours[i].Active;
                        var show = a.behaviours[i].show;
                        a.behaviours[i] = (ActionBehaviour)Activator.CreateInstance(type);
                        a.behaviours[i].Reload(type, stateMachine, metadatas);
                        a.behaviours[i].ID = s.ID;
                        a.behaviours[i].Active = active;
                        a.behaviours[i].show = show;
                    }
                }
            }
        }
#endif
    }
}
using System.Collections.Generic;
using UnityEngine;

namespace GameDesigner 
{
    /// <summary>
    /// 音效模式
    /// </summary>
	public enum AudioMode
    {
        /// <summary>
        /// 进入状态播放音效
        /// </summary>
		EnterPlay,
        /// <summary>
        /// 动画事件播放音效
        /// </summary>
		AnimEvent,
        /// <summary>
        /// 退出状态播放音效
        /// </summary>
		ExitPlay
    }

    /// <summary>
    /// 技能生成模式
    /// </summary>
	public enum ActiveMode
    {
        /// <summary>
        /// 实例化
        /// </summary>
		Instantiate,
        /// <summary>
        /// 对象池
        /// </summary>
		ObjectPool,
        /// <summary>
        /// 激活模式
        /// </summary>
        Active,
    }

    /// <summary>
    /// 技能物体设置模式
    /// </summary>
	public enum SpwanMode
    {
        None,
        /// <summary>
        /// 设置技能物体在自身位置
        /// </summary>
		localPosition,
        /// <summary>
        /// 设置技能物体在父对象位置和成为父对象的子物体
        /// </summary>
		SetParent,
        /// <summary>
        /// 设置技能物体在父对象位置
        /// </summary>
		SetInTargetPosition,
        /// <summary>
        /// 激活模式的自身位置
        /// </summary>
        ActiveLocalPosition,
    }

    /// <summary>
    /// 内置的技能动作行为组件, 此组件包含播放音效, 处理技能特效
    /// </summary>
    public class ActionCore : ActionBehaviour
    {
        /// <summary>
        /// 动画事件时间
        /// </summary>
		public float animEventTime = 50f;
        /// <summary>
        /// 是否已到达事件时间或超过事件时间，到为true，没到为flase
        /// </summary>
		[HideField]
        public bool eventEnter = false;
        /// <summary>
        /// 技能粒子物体
        /// </summary>
		public GameObject effectSpwan = null;
        /// <summary>
        /// 粒子物体生成模式
        /// </summary>
		public ActiveMode activeMode = ActiveMode.Instantiate;
        /// <summary>
        /// 粒子物体销毁或关闭时间
        /// </summary>
		public float spwanTime = 1f;
        /// <summary>
        /// 粒子物体对象池
        /// </summary>
		public List<GameObject> activeObjs = new List<GameObject>();
        /// <summary>
        /// 粒子位置设置
        /// </summary>
		public SpwanMode spwanmode = SpwanMode.localPosition;
        /// <summary>
        /// 作为粒子挂载的父对象 或 作为粒子生成在此parent对象的位置
        /// </summary>
		public Transform parent = null;
        /// <summary>
        /// 粒子出生位置
        /// </summary>
		public Vector3 effectPostion = new Vector3(0, 1.5f, 2f);
        /// <summary>
        /// 粒子角度
        /// </summary>
        public Vector3 effectEulerAngles;
        /// <summary>
        /// 是否播放音效
        /// </summary>
        public bool isPlayAudio = false;
        /// <summary>
        /// 音效触发模式
        /// </summary>
		public AudioMode audioModel = AudioMode.AnimEvent;
        /// <summary>
        /// 音效剪辑
        /// </summary>
		public List<AudioClip> audioClips = new List<AudioClip>();
        /// <summary>
        /// 音效索引
        /// </summary>
		[HideField]
        public int audioIndex = 0;

        public override void OnEnter(StateAction action)
        {
            eventEnter = false;
            if (isPlayAudio & audioModel == AudioMode.EnterPlay & audioClips.Count > 0)
            {
                audioIndex = Random.Range(0, audioClips.Count);
                AudioManager.Play(audioClips[audioIndex]);
            }
        }

        public override void OnUpdate(StateAction action)
        {
            if (action.animTime >= animEventTime & !eventEnter)
            {
                if (effectSpwan != null)
                {
                    if (activeMode == ActiveMode.Instantiate)
                        Object.Destroy(InstantiateSpwan(stateManager), spwanTime);
                    else if (activeMode == ActiveMode.ObjectPool)
                    {
                        bool active = false;
                        foreach (GameObject go in activeObjs)
                        {
                            if (go == null)
                            {
                                activeObjs.Remove(go);
                                break;
                            }
                            if (!go.activeSelf)
                            {
                                go.SetActive(true);
                                go.transform.SetParent(null);
                                SetPosition(stateManager, go);
                                active = true;
                                StateEvent.AddEvent(spwanTime, () =>
                                {
                                    if (go != null) go.SetActive(false);
                                });
                                break;
                            }
                        }
                        if (!active)
                        {
                            var go = InstantiateSpwan(stateManager);
                            activeObjs.Add(go);
                            StateEvent.AddEvent(spwanTime, () =>
                            {
                                if(go != null) go.SetActive(false);
                            });
                        }
                    }
                    else
                    {
                        effectSpwan.SetActive(true);
                        SetPosition(stateManager, effectSpwan);
                    }
                }
                if (isPlayAudio & audioModel == AudioMode.AnimEvent & audioClips.Count > 0)
                {
                    audioIndex = Random.Range(0, audioClips.Count);
                    AudioManager.Play(audioClips[audioIndex]);
                }
                eventEnter = true;
                OnAnimationEvent(action);
            }
        }

        /// <summary>
        /// 当动画事件触发
        /// </summary>
        public virtual void OnAnimationEvent(StateAction action) { }

        /// <summary>
        /// 设置技能位置
        /// </summary>
		private void SetPosition(StateManager stateManager, GameObject go)
        {
            switch (spwanmode)
            {
                case SpwanMode.localPosition:
                    go.transform.localPosition = stateManager.transform.TransformPoint(effectPostion);
                    go.transform.eulerAngles = stateManager.transform.eulerAngles + effectEulerAngles;
                    break;
                case SpwanMode.SetParent:
                    parent = parent ? parent : stateManager.transform;
                    go.transform.SetParent(parent);
                    go.transform.position = parent.TransformPoint(effectPostion);
                    go.transform.eulerAngles = parent.eulerAngles + effectEulerAngles;
                    break;
                case SpwanMode.SetInTargetPosition:
                    parent = parent ? parent : stateManager.transform;
                    go.transform.SetParent(parent);
                    go.transform.position = parent.TransformPoint(effectPostion);
                    go.transform.eulerAngles = parent.eulerAngles + effectEulerAngles;
                    go.transform.SetParent(null);
                    break;
                case SpwanMode.ActiveLocalPosition:
                    go.transform.localPosition = effectPostion;
                    go.transform.localEulerAngles = effectEulerAngles;
                    break;
            }
        }

        /// <summary>
        /// 技能实例化
        /// </summary>
		private GameObject InstantiateSpwan(StateManager stateManager)
        {
            var go = Object.Instantiate(effectSpwan);
            OnSpwanEffect(go);
            SetPosition(stateManager, go);
            return go;
        }

        /// <summary>
        /// 当技能物体实例化
        /// </summary>
        /// <param name="effect"></param>
        public virtual void OnSpwanEffect(GameObject effect) { }

        public override void OnExit(StateAction action)
        {
            if (isPlayAudio & audioModel == AudioMode.ExitPlay & audioClips.Count > 0)
            {
                audioIndex = Random.Range(0, audioClips.Count);
                AudioManager.Play(audioClips[audioIndex]);
            }
            eventEnter = false;
        }

        public override void OnDestroyComponent()
        {
            foreach (var obj in activeObjs)
            {
                Object.Destroy(obj);
            }
        }
    }
}
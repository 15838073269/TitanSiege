using GameDesigner;
using GF;
using GF.MainGame;
using GF.MainGame.Module.Fight;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// GDNet的Actioncore不满足我的需求，自己写一个
/// </summary>
public class EventArg {
    public float animEventTime;  /// 动画事件时间
    public GF.Const.EventType eventType;//动画事件类型
    public bool eventEnter;// 是否已到达事件时间或超过事件时间，到为true，没到为flase
}
public class MyAcitonCore : ActionBehaviour {
    
    /// <summary>
    /// 动画事件list
    /// </summary>
    public List<EventArg> animEventList = new List<EventArg>();

    /// <summary>
    /// 技能粒子物体
    /// </summary>
    public EffectBase effectSpwan = null;
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

    public override void OnEnter(StateAction action) {
        if (effectSpwan!=null) {
            spwanTime = effectSpwan.m_Particle.main.duration;
        }
        for (int i = 0; i < animEventList.Count; i++) {
            animEventList[i].eventEnter = false;
        }
        if (isPlayAudio & audioModel == AudioMode.EnterPlay & audioClips.Count > 0) {
            audioIndex = Random.Range(0, audioClips.Count);
            AudioManager.Play(audioClips[audioIndex]);
        }
    }

    public override void OnUpdate(StateAction action) {
        if (animEventList.Count>0) {
            for (int i = 0; i < animEventList.Count; i++) {
                if (action.animTime >= animEventList[i].animEventTime & !animEventList[i].eventEnter) {
                    switch (animEventList[i].eventType) {
                        case GF.Const.EventType.playaudio:
                            if (isPlayAudio & audioModel == AudioMode.AnimEvent & audioClips.Count > 0) {
                                audioIndex = Random.Range(0, audioClips.Count);
                                AudioManager.Play(audioClips[audioIndex]);
                            }
                            break;
                        case GF.Const.EventType.none:

                            break;
                        case GF.Const.EventType.shake:
                            AppTools.Send((int)MoveEvent.ShakeCamera);
                            break;
                        default:
                            Debuger.Log("未知事件类型"+ animEventList[i].eventType.ToString());
                            break;
                    }
                    animEventList[i].eventEnter = true;
                    OnAnimationEvent(action, animEventList[i]);
                    break;
                }
            }
        }
    }

    /// <summary>
    /// 当动画事件触发
    /// </summary>
    public virtual void OnAnimationEvent(StateAction action,EventArg evntarg) { }

    /// <summary>
    /// 设置技能位置
    /// </summary>
    public void SetPosition(StateManager stateManager, GameObject go) {
        switch (spwanmode) {
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
    public GameObject InstantiateSpwan(StateManager stateManager) {
        var go = Object.Instantiate(effectSpwan.gameObject);
        OnSpwanEffect(go);
        SetPosition(stateManager, go);
        return go;
    }

    /// <summary>
    /// 当技能物体实例化
    /// </summary>
    /// <param name="effect"></param>
    public virtual void OnSpwanEffect(GameObject effect) { }

    public override void OnExit(StateAction action) {
        if (isPlayAudio & audioModel == AudioMode.ExitPlay & audioClips.Count > 0) {
            audioIndex = Random.Range(0, audioClips.Count);
            AudioManager.Play(audioClips[audioIndex]);
        }
        for (int i = 0; i < animEventTimeList.Count; i++) {
            animEventTimeList[i].eventEnter = false;
        }
    }

    public override void OnDestroyComponent() {
        foreach (var obj in activeObjs) {
            Object.Destroy(obj);
        }
    }
}


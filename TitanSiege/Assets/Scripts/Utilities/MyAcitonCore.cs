using GameDesigner;
using GF;
using GF.Const;
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
    public SkillEventType eventType;//动画事件类型
    public bool eventEnter;// 是否已到达事件时间或超过事件时间，到为true，没到为flase
    public float eventeff;//事件的效果，例如位移多少米，屏幕抖动幅度，特效参数等，可以放任意值，在代码中拓展识别即可
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
    /// 作为粒子挂载的父对象 或 作为粒子生成在此parent对象的位置,这里的父对象是默认的粒子的父对象，在本游戏就是角色下skilleffect物体
    /// </summary>
    public Transform effectparent = null;
    /// <summary>
    /// 粒子出生位置
    /// </summary>
    public Vector3 effectPostion = new Vector3(0, 1.5f, 2f);
    /// <summary>
    /// 粒子角度
    /// </summary>
    public Quaternion effectRotation;
    /// <summary>
    /// 是否播放音效
    /// </summary>
    public bool isPlayAudio = false;
    /// <summary>
    /// 音效触发模式
    /// </summary>
    public AudioMode audioModel = AudioMode.EnterPlay;
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
        if (effectparent == null) {
            effectparent = transform.Find("skilleffect");    
        }
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
                        case SkillEventType.playaudio:
                            if (isPlayAudio & audioModel == AudioMode.AnimEvent & audioClips.Count > 0) {
                                audioIndex = Random.Range(0, audioClips.Count);
                                AudioManager.Play(audioClips[audioIndex]);
                            }
                            break;
                        case SkillEventType.none:

                            break;
                        case SkillEventType.shake:
                            AppTools.Send<float>((int)MoveEvent.ShakeCamera, animEventList[i].eventeff);
                            break;
                        case SkillEventType.showeff:
                            //播放特效
                            if (effectSpwan != null) {
                                if (activeMode == ActiveMode.Instantiate)
                                    Object.Destroy(InstantiateSpwan(stateManager), spwanTime);
                                else if (activeMode == ActiveMode.ObjectPool) {
                                    bool active = false;
                                    foreach (GameObject go in activeObjs) {
                                        if (go == null) {
                                            activeObjs.Remove(go);
                                            break;
                                        }
                                        if (!go.activeSelf) {
                                            go.SetActive(true);
                                            go.transform.SetParent(null);
                                            SetPosition(stateManager, go);
                                            active = true;
                                            GameDesigner.StateEvent.AddEvent(spwanTime, () =>
                                            {
                                                if (go != null) go.SetActive(false);
                                            });
                                            break;
                                        }
                                    }
                                    if (!active) {
                                        var go = InstantiateSpwan(stateManager);
                                        activeObjs.Add(go);
                                        GameDesigner.StateEvent.AddEvent(spwanTime, () =>
                                        {
                                            if (go != null) go.SetActive(false);
                                        });
                                    }
                                } else if (activeMode == ActiveMode.Active) {
                                    effectSpwan.gameObject.SetActive(true);
                                    if (spwanmode == SpwanMode.SetParent) {//先把位置记录一下
                                        effectPostion = effectSpwan.transform.localPosition;
                                        effectRotation = effectSpwan.transform.localRotation;
                                    }
                                    SetPosition(stateManager, effectSpwan.gameObject);
                                    if (spwanmode == SpwanMode.SetParent) {
                                        GameDesigner.StateEvent.AddEvent(spwanTime, () =>
                                        {
                                            if (effectSpwan != null) effectSpwan.gameObject.SetActive(false);
                                            effectSpwan.transform.SetParent(effectparent);
                                            effectSpwan.transform.localPosition = effectPostion;
                                            effectSpwan.transform.localRotation = effectRotation;
                                        });
                                    }
                                }
                            }
                            break;
                        default:
                           
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
    public virtual void OnAnimationEvent(StateAction action,EventArg eventarg) { }

    /// <summary>
    /// 设置技能位置
    /// </summary>
    public void SetPosition(StateManager stateManager, GameObject go) {
        switch (spwanmode) {
            case SpwanMode.localPosition:
                go.transform.localPosition = stateManager.transform.TransformPoint(effectPostion);
                //go.transform.eulerAngles = stateManager.transform.eulerAngles + effectRotation;
                break;
            case SpwanMode.SetParent:
                //parent = parent ? parent : stateManager.transform;
                go.transform.SetParent(AppMain.GetInstance.SceneTransform);
                //go.transform.position = parent.TransformPoint(effectPostion);
                //go.transform.eulerAngles = parent.eulerAngles + effectEulerAngles;
                break;
            case SpwanMode.SetInTargetPosition:
                //effectSpwan_parent = effectSpwan_parent ? effectSpwan_parent : stateManager.transform;
                go.transform.SetParent(effectparent);
                go.transform.position = effectparent.TransformPoint(effectPostion);
                //go.transform.eulerAngles = effectparent.eulerAngles + effectRotation;
                go.transform.SetParent(null);
                break;
            case SpwanMode.ActiveLocalPosition:
                //go.transform.localPosition = effectPostion;
                //go.transform.localEulerAngles = effectEulerAngles;
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
        for (int i = 0; i < animEventList.Count; i++) {
            animEventList[i].eventEnter = false;
        }
    }

    public override void OnDestroyComponent() {
        foreach (var obj in activeObjs) {
            Object.Destroy(obj);
        }
    }
}


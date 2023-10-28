using GameDesigner;
using GF;
using GF.Const;
using GF.MainGame;
using GF.MainGame.Module.Fight;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// GDNet��Actioncore�������ҵ������Լ�дһ��
/// </summary>
public class EventArg {
    public float animEventTime;  /// �����¼�ʱ��
    public SkillEventType eventType;//�����¼�����
    public bool eventEnter;// �Ƿ��ѵ����¼�ʱ��򳬹��¼�ʱ�䣬��Ϊtrue��û��Ϊflase
    public float eventeff;//�¼���Ч��������λ�ƶ����ף���Ļ�������ȣ���Ч�����ȣ����Է�����ֵ���ڴ�������չʶ�𼴿�
}
public class MyAcitonCore : ActionBehaviour {
    
    /// <summary>
    /// �����¼�list
    /// </summary>
    public List<EventArg> animEventList = new List<EventArg>();

    /// <summary>
    /// ������������
    /// </summary>
    public EffectBase effectSpwan = null;
    /// <summary>
    /// ������������ģʽ
    /// </summary>
    public ActiveMode activeMode = ActiveMode.Instantiate;
    /// <summary>
    /// �����������ٻ�ر�ʱ��
    /// </summary>
    public float spwanTime = 1f;
    /// <summary>
    /// ������������
    /// </summary>
    public List<GameObject> activeObjs = new List<GameObject>();
    /// <summary>
    /// ����λ������
    /// </summary>
    public SpwanMode spwanmode = SpwanMode.localPosition;
    /// <summary>
    /// ��Ϊ���ӹ��صĸ����� �� ��Ϊ���������ڴ�parent�����λ��,����ĸ�������Ĭ�ϵ����ӵĸ������ڱ���Ϸ���ǽ�ɫ��skilleffect����
    /// </summary>
    public Transform effectparent = null;
    /// <summary>
    /// ���ӳ���λ��
    /// </summary>
    public Vector3 effectPostion = new Vector3(0, 1.5f, 2f);
    /// <summary>
    /// ���ӽǶ�
    /// </summary>
    public Quaternion effectRotation;
    /// <summary>
    /// �Ƿ񲥷���Ч
    /// </summary>
    public bool isPlayAudio = false;
    /// <summary>
    /// ��Ч����ģʽ
    /// </summary>
    public AudioMode audioModel = AudioMode.EnterPlay;
    /// <summary>
    /// ��Ч����
    /// </summary>
    public List<AudioClip> audioClips = new List<AudioClip>();
    /// <summary>
    /// ��Ч����
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
                            //������Ч
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
                                    if (spwanmode == SpwanMode.SetParent) {//�Ȱ�λ�ü�¼һ��
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
    /// �������¼�����
    /// </summary>
    public virtual void OnAnimationEvent(StateAction action,EventArg eventarg) { }

    /// <summary>
    /// ���ü���λ��
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
    /// ����ʵ����
    /// </summary>
    public GameObject InstantiateSpwan(StateManager stateManager) {
        var go = Object.Instantiate(effectSpwan.gameObject);
        OnSpwanEffect(go);
        SetPosition(stateManager, go);
        return go;
    }

    /// <summary>
    /// ����������ʵ����
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


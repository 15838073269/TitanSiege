using System.Collections.Generic;
using UnityEngine;

namespace GameDesigner 
{
    /// <summary>
    /// ��Чģʽ
    /// </summary>
	public enum AudioMode
    {
        /// <summary>
        /// ����״̬������Ч
        /// </summary>
		EnterPlay,
        /// <summary>
        /// �����¼�������Ч
        /// </summary>
		AnimEvent,
        /// <summary>
        /// �˳�״̬������Ч
        /// </summary>
		ExitPlay
    }

    /// <summary>
    /// ��������ģʽ
    /// </summary>
	public enum ActiveMode
    {
        /// <summary>
        /// ʵ����
        /// </summary>
		Instantiate,
        /// <summary>
        /// �����
        /// </summary>
		ObjectPool,
        /// <summary>
        /// ����ģʽ
        /// </summary>
        Active,
    }

    /// <summary>
    /// ������������ģʽ
    /// </summary>
	public enum SpwanMode
    {
        None,
        /// <summary>
        /// ���ü�������������λ��
        /// </summary>
		localPosition,
        /// <summary>
        /// ���ü��������ڸ�����λ�úͳ�Ϊ�������������
        /// </summary>
		SetParent,
        /// <summary>
        /// ���ü��������ڸ�����λ��
        /// </summary>
		SetInTargetPosition,
        /// <summary>
        /// ����ģʽ������λ��
        /// </summary>
        ActiveLocalPosition,
    }

    /// <summary>
    /// ���õļ��ܶ�����Ϊ���, ���������������Ч, ��������Ч
    /// </summary>
    public class ActionCore : ActionBehaviour
    {
        /// <summary>
        /// �����¼�ʱ��
        /// </summary>
		public float animEventTime = 50f;
        /// <summary>
        /// �Ƿ��ѵ����¼�ʱ��򳬹��¼�ʱ�䣬��Ϊtrue��û��Ϊflase
        /// </summary>
		[HideField]
        public bool eventEnter = false;
        /// <summary>
        /// ������������
        /// </summary>
		public GameObject effectSpwan = null;
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
        /// ��Ϊ���ӹ��صĸ����� �� ��Ϊ���������ڴ�parent�����λ��
        /// </summary>
		public Transform parent = null;
        /// <summary>
        /// ���ӳ���λ��
        /// </summary>
		public Vector3 effectPostion = new Vector3(0, 1.5f, 2f);
        /// <summary>
        /// ���ӽǶ�
        /// </summary>
        public Vector3 effectEulerAngles;
        /// <summary>
        /// �Ƿ񲥷���Ч
        /// </summary>
        public bool isPlayAudio = false;
        /// <summary>
        /// ��Ч����ģʽ
        /// </summary>
		public AudioMode audioModel = AudioMode.AnimEvent;
        /// <summary>
        /// ��Ч����
        /// </summary>
		public List<AudioClip> audioClips = new List<AudioClip>();
        /// <summary>
        /// ��Ч����
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
        /// �������¼�����
        /// </summary>
        public virtual void OnAnimationEvent(StateAction action) { }

        /// <summary>
        /// ���ü���λ��
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
        /// ����ʵ����
        /// </summary>
		private GameObject InstantiateSpwan(StateManager stateManager)
        {
            var go = Object.Instantiate(effectSpwan);
            OnSpwanEffect(go);
            SetPosition(stateManager, go);
            return go;
        }

        /// <summary>
        /// ����������ʵ����
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
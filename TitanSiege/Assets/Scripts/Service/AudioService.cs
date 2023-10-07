using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GF.Module;
using GF.Utils;
using GF.MainGame;
using GF.Unity.AB;

namespace GF.Unity.Audio {
    public class AudioItem {
        public string path;
        public AudioClip clip;
        public bool iscache;
    }
    public class AudioService:Singleton<AudioService> {
        protected override void InitSingleton() {
            base.InitSingleton();
            GameObject audiolist = new GameObject("AudioList");
            audiolist.transform.SetParent(AppMain.GetInstance.SceneTransform);
            audiolist.transform.position = Vector3.zero;
            PublicAudio = new GameObject("PublicAudio").AddComponent<AudioSource>();
            PublicAudio.transform.SetParent(audiolist.transform);
            PublicAudio.playOnAwake = false;
            BGAudio = new GameObject("BGAudio").AddComponent<AudioSource>();
            BGAudio.transform.SetParent(audiolist.transform);
            BGAudio.playOnAwake = false;
            UIAudio = new GameObject("UIAudio").AddComponent<AudioSource>();
            UIAudio.transform.SetParent(audiolist.transform);
            UIAudio.playOnAwake = false;
            FTAudio = new GameObject("FTAudio").AddComponent<AudioSource>();
            FTAudio.transform.SetParent(audiolist.transform);
            FTAudio.playOnAwake = false;
            JQAudio = new GameObject("JQAudio").AddComponent<AudioSource>();
            JQAudio.transform.SetParent(audiolist.transform);
            JQAudio.playOnAwake = false;
        }
        private AudioSource PublicAudio;//���ò�����
        private AudioSource BGAudio;//��������
        private AudioSource UIAudio;//UI����
        private AudioSource FTAudio;//ս������
        private AudioSource JQAudio;//����Ի�
        
        /// <summary>
        /// ����������Ч
        /// </summary>
        private Dictionary<string,AudioItem> m_AudioClipDic = new Dictionary<string,AudioItem>();
        public void ClearMusicCache() {//�ѻ���ȫ��������������ڴ治���ʱ��
            foreach (KeyValuePair<string, AudioItem> a in m_AudioClipDic) {
                ResourceManager.GetInstance.DestoryResource(a.Value.clip, true);
            }
            m_AudioClipDic.Clear();
        }
        public AudioClip GetAudioClip(string path) {
            AudioItem clipitem = null;
            if (m_AudioClipDic.TryGetValue(path,out clipitem) && clipitem != null) {
                return clipitem.clip;
            }
            return null;
        }
        /// <summary>
        /// ���ű������֣��������֣�һ�㲻��Ҫ���棬������Ҳ����Ҫ����Ϊ��ೡ���ı������ֶ���һ��
        /// </summary>
        ///  <param name="iscache">Ĭ�ϲ�����</param>
        /// <param name="path"></param>
        /// <param name="isloop">Ĭ��ѭ��</param>
        public void PlayBGMusic(string path,bool iscache = false, bool isloop = true) {
            AudioClip clip = LoadAudio(path,iscache);
            PlayBGMusic(clip, isloop);
            clip = null;
        }
        public void PlayBGMusic(AudioClip clip,bool isloop = true) {
            if (clip == null) {
                BGAudio.clip = null;
                return;
            }
            if (clip != BGAudio.clip) {
                BGAudio.clip = clip;
            }
            BGAudio.loop = isloop;
            BGAudio.Play();
        }
        /// <summary>
        /// ����ս������,�������ýϸߣ�һ�㶼����һ��
        /// </summary>
        ///  <param name="iscache">Ĭ�ϻ���</param>
        /// <param name="path"></param>
        /// <param name="isloop">Ĭ�ϲ�ѭ��</param>
        public void PlayFTMusic(string path, bool iscache = true, bool isloop = false) {
            AudioClip clip = LoadAudio(path, iscache);
            PlayFTMusic(clip, isloop);
            clip = null;
        }
        public void PlayFTMusic(AudioClip clip,bool isloop = false) {
            if (clip == null) {
                FTAudio.clip = null;
                return;
            }
            if (clip != BGAudio.clip) {
                FTAudio.clip = clip;
            }
            FTAudio.loop = isloop;
            FTAudio.Play();
        }
        /// <summary>
        /// ���ž���Ի�,�������ýϵͣ�Ĭ�ϲ�����
        /// </summary>
        ///  <param name="iscache">Ĭ�ϲ�����</param>
        /// <param name="path"></param>
        /// <param name="isloop">Ĭ�ϲ�ѭ��</param>
        public void PlayJQMusic(string path, bool iscache = false, bool isloop = false) {
            AudioClip clip = LoadAudio(path, iscache);
            PlayJQMusic(clip, isloop);
            clip = null;
            
        }
        public void PlayJQMusic(AudioClip clip, bool isloop = false) {
            if (clip == null) {
                JQAudio.clip = null;
                return;
            }
            if (clip != BGAudio.clip) {
                JQAudio.clip = clip;
            }
            JQAudio.loop = isloop;
            JQAudio.Play();
        }
        /// <summary>
        /// ����UI����,UI�������ýϸߣ�һ�㶼����һ��
        /// </summary>
        ///  <param name="iscache">Ĭ�ϻ���</param>
        /// <param name="path"></param>
        /// <param name="isloop">Ĭ�ϲ�ѭ��</param>
        public void PlayUIMusic(string path, bool iscache = true, bool isloop = false) {
            AudioClip clip = LoadAudio(path, iscache);
            PlayUIMusic(clip, isloop);
            clip = null;
            
        }
        public void PlayUIMusic(AudioClip clip, bool isloop = false) {
            if (clip == null) {
                UIAudio.clip = null;
                return;
            }
            if (clip != BGAudio.clip) {
                UIAudio.clip = clip;
            }
            UIAudio.loop = isloop;
            UIAudio.Play();
        }
        /// <summary>
        /// ����������Դ�������Ƿ񻺴�������Դѡ���������Ƿ�ɾ��
        /// Ĭ��������Դ��������ɾ��
        /// </summary>
        /// <param name="path"></param>
        /// <param name="cache">�Ƿ񻺴�</param>
        /// <returns></returns>
        private AudioClip LoadAudio(string path,bool cache = true) {
            AudioItem clipitem = null;
            if (m_AudioClipDic.TryGetValue(path, out clipitem) && clipitem != null) {
                clipitem.iscache = cache;//ÿ�μ���ʱ����ȡ������Ҫ���»����״̬
                return clipitem.clip;
            } else {
                bool bClear = false;
                if (cache) {//�����Ҫ�������֣�����������ɾ����Դ
                    bClear = false;
                } else {//�����������Ͱ���Դ�����
                    bClear = true;
                }
                clipitem = new AudioItem();
                clipitem.clip = ResourceManager.GetInstance.LoadResource<AudioClip>(path, bClear);
                if (clipitem.clip != null) {
                    clipitem.iscache = cache;
                    clipitem.path = path;
                    m_AudioClipDic.Add(path, clipitem);
                    return clipitem.clip;
                } else {
                    Debuger.LogWarning($"����������Ϊ{path}�����֣����飡");
                    return null;
                }
            }
        }
        private AudioItem HasClip(AudioClip clip) {
            foreach (AudioItem c in m_AudioClipDic.Values) {
                if (c.clip == clip) {
                    return c;
                }
            }
            return null;
        }
        /// <summary>
        /// ֹͣ����
        /// </summary>
        /// <param name="isclearcache">�Ƿ�ɾ���ײ�Ļ��棬ͬʱҲ����������ֵ�Ļ���</param>
        public void StopBGAudio() {
            if (BGAudio!=null) {
                BGAudio.Stop();
                if (BGAudio.clip!=null) {
                    AudioItem c = HasClip(BGAudio.clip);
                    if (c != null ) {//��Դ״̬Ϊ������ʱ��ֱ�Ӵӻ����������������ײ���Դ
                        if (c.iscache == false) {
                            m_AudioClipDic.Remove(c.path);
                            ResourceManager.GetInstance.DestoryResource(BGAudio.clip, !c.iscache);
                        }
                    }
                    
                }
                BGAudio.clip = null;
            }
        }
        public void StopUIAudio() {
            if (UIAudio != null) {
                UIAudio.Stop();
                if (UIAudio.clip != null) {
                    AudioItem c = HasClip(UIAudio.clip);
                    if (c != null) {
                        if (c.iscache == false) {//��Դ״̬Ϊ������ʱ��ֱ�Ӵӻ����������������ײ���Դ
                            m_AudioClipDic.Remove(c.path);
                            ResourceManager.GetInstance.DestoryResource(UIAudio.clip, !c.iscache);
                        }
                    }
                }
                UIAudio.clip = null;
            }
        }
        public void StopFTAudio(bool isclearcache = false) {
            if (FTAudio != null) {
                FTAudio.Stop();
                if (FTAudio.clip != null) {
                    AudioItem c = HasClip(FTAudio.clip);
                    if (c != null) {
                        if (c.iscache == false) {//��Դ״̬Ϊ������ʱ��ֱ�Ӵӻ����������������ײ���Դ
                            m_AudioClipDic.Remove(c.path);
                            ResourceManager.GetInstance.DestoryResource(FTAudio.clip, !c.iscache);
                        }
                    }
                }
                FTAudio.clip = null;
            }
        }
        public void StopJQAudio(bool isclearcache = false) {
            if (JQAudio != null) {
                JQAudio.Stop();
                if (JQAudio.clip != null) {
                    AudioItem c = HasClip(JQAudio.clip);
                    if (c != null) {
                        if (c.iscache == false) {//��Դ״̬Ϊ������ʱ��ֱ�Ӵӻ����������������ײ���Դ
                            m_AudioClipDic.Remove(c.path);
                            ResourceManager.GetInstance.DestoryResource(JQAudio.clip, !c.iscache);
                        }
                    }
                }
                JQAudio.clip = null;
            }
        }

        
    }
}


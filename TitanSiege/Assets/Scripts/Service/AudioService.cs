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
        private AudioSource PublicAudio;//公用播放器
        private AudioSource BGAudio;//背景音乐
        private AudioSource UIAudio;//UI音乐
        private AudioSource FTAudio;//战斗音乐
        private AudioSource JQAudio;//剧情对话
        
        /// <summary>
        /// 缓存所有音效
        /// </summary>
        private Dictionary<string,AudioItem> m_AudioClipDic = new Dictionary<string,AudioItem>();
        public void ClearMusicCache() {//把缓存全部清理掉，用于内存不足的时候
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
        /// 播放背景音乐，背景音乐，一般不需要缓存，跳场景也不需要，因为大多场景的背景音乐都不一样
        /// </summary>
        ///  <param name="iscache">默认不缓存</param>
        /// <param name="path"></param>
        /// <param name="isloop">默认循环</param>
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
        /// 播放战斗音乐,音乐重用较高，一般都缓存一下
        /// </summary>
        ///  <param name="iscache">默认缓存</param>
        /// <param name="path"></param>
        /// <param name="isloop">默认不循环</param>
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
        /// 播放剧情对话,音乐重用较低，默认不缓存
        /// </summary>
        ///  <param name="iscache">默认不缓存</param>
        /// <param name="path"></param>
        /// <param name="isloop">默认不循环</param>
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
        /// 播放UI音乐,UI音乐重用较高，一般都缓存一下
        /// </summary>
        ///  <param name="iscache">默认缓存</param>
        /// <param name="path"></param>
        /// <param name="isloop">默认不循环</param>
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
        /// 加载音乐资源，根据是否缓存音乐资源选择跳场景是否删除
        /// 默认音乐资源跳场景不删除
        /// </summary>
        /// <param name="path"></param>
        /// <param name="cache">是否缓存</param>
        /// <returns></returns>
        private AudioClip LoadAudio(string path,bool cache = true) {
            AudioItem clipitem = null;
            if (m_AudioClipDic.TryGetValue(path, out clipitem) && clipitem != null) {
                clipitem.iscache = cache;//每次加载时，读取缓存需要更新缓存的状态
                return clipitem.clip;
            } else {
                bool bClear = false;
                if (cache) {//如果需要缓存音乐，就跳场景不删除资源
                    bClear = false;
                } else {//否则跳场景就把资源清理掉
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
                    Debuger.LogWarning($"不存在名称为{path}的音乐，请检查！");
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
        /// 停止播放
        /// </summary>
        /// <param name="isclearcache">是否删除底层的缓存，同时也会清楚本身字典的缓存</param>
        public void StopBGAudio() {
            if (BGAudio!=null) {
                BGAudio.Stop();
                if (BGAudio.clip!=null) {
                    AudioItem c = HasClip(BGAudio.clip);
                    if (c != null ) {//资源状态为不缓存时，直接从缓存中清除，并清除底层资源
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
                        if (c.iscache == false) {//资源状态为不缓存时，直接从缓存中清除，并清除底层资源
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
                        if (c.iscache == false) {//资源状态为不缓存时，直接从缓存中清除，并清除底层资源
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
                        if (c.iscache == false) {//资源状态为不缓存时，直接从缓存中清除，并清除底层资源
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


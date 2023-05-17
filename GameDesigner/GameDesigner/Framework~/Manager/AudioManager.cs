using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    /// <summary>
    /// ��Ч����
    /// </summary>
    public class AudioManager : MonoBehaviour
    {
        public GameObject audioObject;
        private readonly List<AudioSource> sources = new List<AudioSource>();
        private readonly List<AudioSource> destroyPlayingSources = new List<AudioSource>();

        public virtual void Update()
        {
            for (int i = 0; i < destroyPlayingSources.Count; ++i)
            {
                if (!destroyPlayingSources[i].isPlaying)
                {
                    Destroy(destroyPlayingSources[i]);
                    destroyPlayingSources.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// ������Ч����
        /// ����clip : ������Ҫ���ŵ���Դ
        /// </summary>
        public void Play(AudioClip clip)
        {
            Play(clip, 1f);
        }

        /// <summary>
        /// ������Ч����
        /// ����clip : ������Ҫ���ŵ���Դ
        /// ����volume : ������С����
        /// </summary>
        public AudioSource Play(AudioClip clip, float volume)
        {
            return Play(clip, volume, false);
        }

        /// <summary>
        /// ������Ч����
        /// ����clip : ������Ҫ���ŵ���Դ
        /// ����volume : ������С����
        /// </summary>
        public AudioSource Play(AudioClip clip, float volume, bool loop)
        {
            if (clip == null)
                return null;
            for (int i = 0; i < sources.Count; ++i)
            {
                if (!sources[i].isPlaying)//�����Ч�������� ���� ��Чû�б����� �����ִ�в�����Ч
                {
                    sources[i].clip = clip;
                    sources[i].volume = volume;
                    sources[i].loop = loop;
                    if (loop)
                        sources[i].Play();
                    else
                        sources[i].PlayOneShot(clip, volume);
                    return sources[i];
                }
            }
            var source = audioObject.AddComponent<AudioSource>();
            sources.Add(source);
            source.clip = clip;
            source.volume = volume;
            source.loop = loop;
            if (loop)
                source.Play();
            else
                source.PlayOneShot(clip, volume);
            return source;
        }

        /// <summary>
        /// ����Ч�����������AudioSource���
        /// ����clip : ������Ҫ���ŵ���Դ
        /// </summary>
        public AudioSource OnPlayingDestroy(AudioClip clip)
        {
            return OnPlayingDestroy(clip, 1f); ;
        }

        /// <summary>
        /// ����Ч�����������AudioSource���
        /// ����clip : ������Ҫ���ŵ���Դ
        /// ����volume : ������С����
        /// </summary>
        public AudioSource OnPlayingDestroy(AudioClip clip, float volume)
        {
            var source = audioObject.AddComponent<AudioSource>();
            destroyPlayingSources.Add(source);
            source.volume = volume;
            source.clip = clip;
            source.PlayOneShot(clip);
            return source;
        }

        /// <summary>
        /// ����Ч�����������AudioSource���
        /// ����clip : ������Ҫ���ŵ���Դ
        /// ����source : ��ƵԴ���
        /// </summary>
        public void OnPlayingDestroy(AudioSource source, AudioClip clip)
        {
            destroyPlayingSources.Add(source);
            source.clip = clip;
            source.PlayOneShot(clip);
        }

        public AudioSource Stop(AudioClip clip)
        {
            if (clip == null)
                return null;
            for (int i = 0; i < sources.Count; ++i)
            {
                if (sources[i].clip == clip)
                {
                    sources[i].Stop();
                    return sources[i];
                }
            }
            return null;
        }

        public AudioSource GetAudioSource(AudioClip clip)
        {
            if (clip == null)
                return null;
            for (int i = 0; i < sources.Count; ++i)
            {
                if (sources[i].clip == clip)
                    return sources[i];
            }
            return null;
        }
    }
}
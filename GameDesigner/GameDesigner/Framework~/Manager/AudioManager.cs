using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public class AudioDesc
    {
        public int Id;
        public AudioSource source;
    }

    /// <summary>
    /// ��Ч����
    /// </summary>
    public class AudioManager : MonoBehaviour
    {
        public GameObject audioObject;
        private int Id;
        private readonly List<AudioDesc> sources = new List<AudioDesc>();

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
        public int Play(AudioClip clip, float volume)
        {
            return Play(clip, volume, false);
        }

        /// <summary>
        /// ������Ч����
        /// ����clip : ������Ҫ���ŵ���Դ
        /// ����volume : ������С����
        /// </summary>
        public int Play(AudioClip clip, float volume, bool loop)
        {
            if (clip == null)
                return -1;
            foreach (var desc in sources)
            {
                if (!desc.source.isPlaying)//�����Ч�������� ���� ��Чû�б����� �����ִ�в�����Ч
                {
                    SetAudioDesc(desc, clip, volume, loop);
                    return desc.Id;
                }
            }
            var source = audioObject.AddComponent<AudioSource>();
            var id = Id++;
            var desc1 = new AudioDesc() { Id = id, source = source };
            sources.Add(desc1);
            SetAudioDesc(desc1, clip, volume, loop);
            return id;
        }

        private void SetAudioDesc(AudioDesc desc, AudioClip clip, float volume, bool loop)
        {
            desc.source.clip = clip;
            desc.source.volume = volume;
            desc.source.loop = loop;
            if (loop)
                desc.source.Play();
            else
                desc.source.PlayOneShot(clip, volume);
        }

        public void Stop(int Id)
        {
            foreach (var desc in sources)
            {
                if (desc.Id == Id)
                {
                    desc.source.Stop();
                    return;
                }
            }
        }

        public int PlaySound(string clipRes, float volume, bool isLoop)
        {
            var audioClip = Global.Resources.LoadAsset<AudioClip>(AssetBundleType.All, clipRes);
            return Play(audioClip, volume, isLoop);
        }
    }
}
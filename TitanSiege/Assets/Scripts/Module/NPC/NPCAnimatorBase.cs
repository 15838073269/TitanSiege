/****************************************************
    文件：NPCAnimator.cs
	作者：昔莘
    邮箱: 304183153@qq.com
    日期：2022/11/6 0:0:52
	功能：Nothing
*****************************************************/


using GF.MainGame.Module.Fight;
using GF.Unity.Audio;
using System.Collections.Generic;
using UnityEngine;
using Net.System;
using GF.MainGame.Data;


namespace GF.MainGame.Module.NPC {
  
    public class NPCAnimatorBase : MonoBehaviour {
        public EffectBase m_CurrentEffect;
        public Transform m_EffectFather;
        //private Vector3 m_LastEffectLocalPos;
        //private Quaternion m_LastEffectLocalRotate;
        //可以自行在面板拖入
        public Dictionary<string, EffectBase> SkillEffect;
        public Animator m_ani;
 
        public virtual void Init() {
            m_ani = GetComponent<Animator>();
            SkillEffect = new Dictionary<string, EffectBase>();
            m_EffectFather = transform.Find("skilleffect");
            //读取职业技能配置表配置的技能特效 todo
            if (m_EffectFather!=null) {
                EffectBase[] effectarr = m_EffectFather.GetComponentsInChildren<EffectBase>();
                for (int i = 0; i < effectarr.Length; i++) {
                    effectarr[i].Init();
                    SkillEffect.Add(effectarr[i].name, effectarr[i]);
                    effectarr[i].gameObject.SetActive(false);
                }
            }
        }
        
        public virtual void SetFight(bool fight) {
            m_ani.SetBool("fightidle", fight);
        }
        /// <summary>
        /// 攻击动画
        /// </summary>
        public virtual void Attack(SkillDataBase sd = null) {
           
        }
        /// <summary>
        /// 死亡动画
        /// </summary>
        public virtual void Die() {
            //m_ani.SetInteger("die", -1);
        }
        /// <summary>
        /// 受击动画
        /// </summary>
        public virtual void Hurt() {
           // m_ani.SetTrigger("hurt");
        }
        /// 移动动画
        /// </summary>
        public virtual void Move() {
            //if (m_ani.GetBool("fightidle")) {
            //    m_ani.SetFloat("fightrun", 1f);
            //} else {
            //    m_ani.SetFloat("run", 1f);
            //}
        }
       
        /// <summary>
        /// 等待处理
        /// </summary>
        public virtual void Idle() {
            //if (m_ani.GetBool("fightidle")) {
            //    m_ani.SetFloat("fightrun", 0f);
            //} else {
            //    m_ani.SetFloat("run", 0f);
            //}
        }
        public virtual void HiddenEffect() {
            m_CurrentEffect = null;
        }
        /// <summary>
        /// 播放特效
        /// </summary>
        /// <param name="effname"></param>
        public virtual void PlayEffect(string effname) {
            EffectBase temp = null;
            SkillEffect.TryGetValue(effname, out temp);
            if (temp != null) {
                EffectArg arg = new EffectArg(temp, temp.transform.localPosition, temp.transform.localRotation);
                ThreadManager.Event.AddEvent(temp.m_Particle.main.duration, PlayEffectOver, arg);
                if (!temp.m_IsFollow) {
                    temp.transform.parent = AppMain.GetInstance.SceneTransform;
                }
                m_CurrentEffect = temp;
                //这里改成通过对象池创建技能效果
                temp.gameObject.SetActive(true);
            }
        }
        
        private void PlayEffectOver(object obj) {
            EffectArg arg = obj as EffectArg;
            if (arg != null) {
                if (arg.CurrentEffect != null) {
                    if (!arg.CurrentEffect.m_IsFollow) {
                        arg.CurrentEffect.transform.parent = m_EffectFather;
                        arg.CurrentEffect.transform.localPosition = arg.LastEffectLocalPos;
                        arg.CurrentEffect.transform.localRotation = arg.LastEffectLocalRotate;
                    }
                    arg.CurrentEffect.gameObject.SetActive(false);
                }
            }
        }
       
        public void StopEffect() {
            if (m_CurrentEffect!=null) {
                m_CurrentEffect.gameObject.SetActive(false);
            }
        }
        /// <summary>
        /// 播放音效
        /// </summary>
        /// <param name="audio"></param>
        public void PlayAudio(Object audio) {
            AudioClip clip = audio as AudioClip;
            if (clip!=null) {
                AudioService.GetInstance.PlayFTMusic(clip,false);
            }
        }
        public void StopAudio() {
            AudioService.GetInstance.StopFTAudio();
        }
        /// <summary>
        /// 抖动屏幕
        /// </summary>
        public void Shake(float t) {
            AppTools.Send((int)MoveEvent.ShakeCamera);
        }
        /// <summary>
        /// 动画时，停止技能移动的方法
        /// </summary>
        public void StopMove() {
            AppTools.Send<float, bool>((int)MoveEvent.SetSkillMove, 0f, true);
        }
        /// <summary>
        /// 获得animator下某个动画片段的时长方法
        /// </summary>
        /// <param animator="animator">Animator组件</param> 
        /// <param name="name">要获得的动画片段名字</param>
        /// <returns></returns>
        public float GetAnimatorSeconds(Animator animator, string name) {
            //动画片段时间长度
            float length = 0;
            AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;
            foreach (AnimationClip clip in clips) {
                if (clip.name.Equals(name)) {
                    length = clip.length;
                    break;
                }
            }
            return length;
        }
        public int GetAnimatorMilliseconds(Animator animator, string name) {
            //动画片段时间长度
            int length = 0;
            AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;
            foreach (AnimationClip clip in clips) {
                if (clip.name.Equals(name)) {
                    length = (int)(clip.length*1000f);
                    break;
                }
            }
            return length;
        }
      
        
    }
    public class EffectArg {
        public EffectBase CurrentEffect;
        public Vector3 LastEffectLocalPos;
        public Quaternion LastEffectLocalRotate;
        public EffectArg(EffectBase eb, Vector3 lastpos, Quaternion lastrotate) {
            CurrentEffect = eb;
            LastEffectLocalPos = lastpos;
            LastEffectLocalRotate = lastrotate;
        }
    }
}
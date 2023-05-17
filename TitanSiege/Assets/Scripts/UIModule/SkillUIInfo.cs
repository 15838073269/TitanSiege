/****************************************************
    文件：SkillUIInfo.cs
	作者：昔莘
    邮箱: 304183153@qq.com
    日期：2022/11/14 1:48:20
	功能：Nothing
*****************************************************/

using GF.MainGame.Data;
using GF.Service;
using GF.Unity.AB;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace GF.MainGame.UI {
    public class SkillUIInfo : MonoBehaviour {
        private Image m_SkillImg;
        private Button m_Btn;//当前按钮
        public Image m_Heidi;//技能cd或者未解锁的黑底
        public RawImage m_Suo;//技能未解锁的锁样
        public Text m_Cdtime;//技能cd
        public bool IsJiesuo;//是否解锁技能
        /// <summary>
        /// 技能配置表的数据
        /// </summary>
        public SkillDataBase m_Data;
        public void Init(SkillDataBase sb) {
            m_Btn = transform.GetComponent<Button>();
            m_SkillImg = transform.GetComponent<Image>();
            m_Data = sb;
            m_SkillImg.sprite = ResourceManager.GetInstance.LoadResource<Sprite>(m_Data.pic,bClear:false);
            if ((ushort)(UserService.GetInstance.m_CurrentChar.Level) >= m_Data.xuqiudengji) {
                m_Heidi.gameObject.SetActive(false);
                m_Suo.gameObject.SetActive(false);
                m_Btn.interactable = true;
                IsJiesuo = true;
            } else {
                m_Heidi.gameObject.SetActive(true);
                m_Suo.gameObject.SetActive(true);
                m_Btn.interactable = false;
                IsJiesuo = false;
            }
            m_Cdtime.gameObject.SetActive(false);
        }
        /// <summary>
        /// 点击触发技能后，播放cd
        /// </summary>
        public void SetCD() {
            m_Cdtime.text = m_Data.cd+"";
            m_Cdtime.gameObject.SetActive(true);
            m_Heidi.gameObject.SetActive(true);
            m_Btn.interactable=false;
            
            StartCoroutine("JsCd");
        }
        float cd = 0f;//开一个协程,未执行完时，把脚本挂载的物体隐藏，就会出现bug,所以每次脚本启动都要检查一下是否有未执行完的技能cd
        public void OnEnable() {
            if (cd>0) {
                StartCoroutine("JsCd");
            }
        }
        IEnumerator JsCd() {
            if (cd == 0f) {
                cd = m_Data.cd;
            }
            while (cd>0) {
                cd -= Time.deltaTime;
                string s = cd.ToString("#0.0");
                if (s!= m_Cdtime.text) {
                    m_Cdtime.text = s;
                }
                m_Heidi.fillAmount = cd / m_Data.cd;
                yield return null;
            }
            m_Heidi.fillAmount = 1f;
            cd = 0f;
            m_Cdtime.gameObject.SetActive(false);
            m_Heidi.gameObject.SetActive(false);
            m_Btn.interactable = true;
        }
        /// <summary>
        /// 升级时判断一下是否解锁技能
        /// </summary>
        /// <param name="level"></param>
        public void XuqiuDengji(ushort level) {
            if (!IsJiesuo) {
                if (level > m_Data.xuqiudengji) {
                    m_Heidi.gameObject.SetActive(false);
                    m_Suo.gameObject.SetActive(false);
                    m_SkillImg.GetComponent<Button>().interactable = true;
                    IsJiesuo = true;
                }
            }
        }
        
    }
} 

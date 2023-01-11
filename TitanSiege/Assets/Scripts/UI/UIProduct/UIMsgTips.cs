
using GF.Unity.EXtension;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

namespace GF.Unity.UI {
    public class UIMsgTips:UIWidget
    {
        public Text ctlTextBar;
        public Image img;
        private const float MaxYOffset = 180f;
        private float m_alpha = 0;
        private float m_yOffset = MaxYOffset;
        private Queue<string> m_TipsQue = new Queue<string>();//tips队列
        private bool m_ShowTips = true;
        /// <summary>
        /// 添加消息到队列
        /// </summary>
        /// <param name="tips"></param>
        public void AddTipsQue(string tips) {
            //多线程问题，加个锁
            lock (m_TipsQue) {
                m_TipsQue.Enqueue(tips);
            }
        }
        private void SetTips() {
            if (m_ShowTips == false) {
                string tips = m_TipsQue.Dequeue();
                ctlTextBar.text = tips;
                m_yOffset = MaxYOffset;
                m_alpha = 1;
                m_ShowTips = true;
            }
        }
        public void OnEnable() {
            m_alpha = 0;
            m_ShowTips = true;
            ctlTextBar.color = new UnityEngine.Color(1, 1, 1, 0);
            img.color = new UnityEngine.Color(1, 1, 1, 0);
        }
        protected override void OnOpen(object arg = null)
        {
            base.OnOpen(arg);
            AddTipsQue(arg as string);
        }

        void Update()
        {
            if (m_ShowTips) {
                m_alpha -= 0.5f*Time.deltaTime;
                ctlTextBar.color = new UnityEngine.Color(1,1,1, m_alpha);
                img.color = new UnityEngine.Color(1, 1, 1, m_alpha);
                if (m_alpha < 0) {
                    m_alpha = 0;
                    m_ShowTips = false;
                    if (m_TipsQue.Count > 0) {
                        SetTips();
                    } else {
                        this.Close();
                    }
                }
                m_yOffset -= 1f;
                img.transform.SetLocalY(MaxYOffset - m_yOffset);
                if (m_yOffset < 0) {
                    m_yOffset = 0;
                }
            }
        }
    }

}
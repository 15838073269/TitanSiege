using GF.Const;
using GF.MainGame.Data;
using GF.MainGame.Module.NPC;
using GF.Service;
using GF.Unity.UI;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GF.MainGame.UI {
    public class RoleInfo : MonoBehaviour {

        public Image m_JianshiHead;
        public Image m_jinglingHead;
        public Image m_longnvHead;
        public Text m_PlayerName;
        public Text m_Zhiye;
        public Text m_Lv;
        public Image m_InfoImg;
        public Sprite m_Jianshiimg;
        public Sprite m_Jinglingimg;
        public Sprite m_Longnvimg;
        public long m_CharacterId;
        private Zhiye rolezhiye;
        public void InitInfo(string name,int zhiye,short lv,long id) {
            m_CharacterId = id;
            Zhiye zhiyedata = (Zhiye)zhiye;
            m_PlayerName.text = name;
            m_Zhiye.text= zhiyedata.ToString();
            rolezhiye = zhiyedata;
            m_Lv.text= lv+"级";
            ShowImageHead(zhiyedata);
            //职业的枚举对应的id设置和角色表的id一直
            AddUIClickListener(m_JianshiHead, () => MainSelected(m_CharacterId));
            AddUIClickListener(m_jinglingHead, () => MainSelected(m_CharacterId));
            AddUIClickListener(m_longnvHead, () => MainSelected(m_CharacterId));
            AddUIClickListener(m_InfoImg, () => MainSelected(m_CharacterId));
        }
        public void ShowImageHead(Zhiye zhiyedata) {
            switch (zhiyedata) {
                case Zhiye.剑士:
                    m_JianshiHead.gameObject.SetActive(true);
                    m_jinglingHead.gameObject.SetActive(false);
                    m_longnvHead.gameObject.SetActive(false);
                    m_InfoImg.sprite = m_Jianshiimg;
                    break;
                case Zhiye.法师:
                    m_JianshiHead.gameObject.SetActive(false);
                    m_jinglingHead.gameObject.SetActive(true);
                    m_longnvHead.gameObject.SetActive(false);
                    m_InfoImg.sprite = m_Jinglingimg;
                    break;
                case Zhiye.游侠:
                    m_JianshiHead.gameObject.SetActive(false);
                    m_jinglingHead.gameObject.SetActive(false);
                    m_longnvHead.gameObject.SetActive(true);
                    m_InfoImg.sprite = m_Longnvimg;
                    break;
                default:
                    break;
            }
        }
        public void MainSelected(long roleid) {
            AppTools.Send<long>((int)CreateAndSelectEvent.Selected, roleid);
        }

        private Color c1 = new Color(1f, 1f, 1f, 1f);
        private Color c2 = new Color(82f / 255f, 82f / 255f, 82f / 255f, 1f);
        private Color c3 = new Color(1f, 1f, 1f, 0.2f);
        /// <summary>
        /// 显示隐藏角色
        /// </summary>
        /// <param name="isshow">显示还是隐藏</param>
        public void ShowOrHiden(bool isshow) {
            if (isshow) {//显示
                switch (rolezhiye) {
                    case Zhiye.剑士:
                        m_JianshiHead.color = c1;
                        break;
                    case Zhiye.游侠:
                        m_longnvHead.color = c1;
                        break;
                    case Zhiye.法师:
                        m_jinglingHead.color = c1;
                        break;
                    default:
                        break;
                }
                m_InfoImg.color = c1;
                m_PlayerName.color = c1;
                m_Zhiye.color = c1;
                m_Lv.color = c1;
                //显示模型 职业的枚举对应的id设置和角色表的id一致
                UserService.GetInstance.m_CurrentID = (ushort)rolezhiye;
                AppTools.Send<ushort>((int)CreateAndSelectEvent.ShowModel, (ushort)rolezhiye);
            } else {//隐藏
                switch (rolezhiye) {
                    case Zhiye.剑士:
                        m_JianshiHead.color = c2;
                        break;
                    case Zhiye.游侠:
                        m_longnvHead.color = c2;
                        break;
                    case Zhiye.法师:
                        m_jinglingHead.color = c2;
                        break;
                    default:
                        break;
                }
                m_InfoImg.color = c2;
                m_PlayerName.color = c3;
                m_Zhiye.color = c3;
                m_Lv.color = c3;
            }
        }

        /// <summary>
        /// 为UIPanel内的脚本提供便捷的UI事件监听接口
        /// </summary>
        /// <param name="target">目标UI</param>
        /// <param name="listener"></param>
        public void AddUIClickListener(UIBehaviour target, Action listener) {
            if (target != null) {
                UIEventTrigger.Get(target).onClick -= listener;
                UIEventTrigger.Get(target).onClick += listener;
            }
        }

    }
}

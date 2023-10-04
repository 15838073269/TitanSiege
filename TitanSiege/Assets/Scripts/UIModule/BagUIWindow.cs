/****************************************************
    文件：BagUIWindow.cs
	作者：昔莘
    邮箱: 304183153@qq.com
    日期：2023/9/30 8:41:9
	功能：Nothing
*****************************************************/

using GF.MainGame.Data;
using GF.Service;
using GF.Unity.UI;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GF.MainGame.UI {
    public enum ToggleBtnType { 
        quanbu,
        yaopin,
        zhuangbei,
        renwu,
        zawu,
    }
    public class BagUIWindow : UIWindow {
        public Button m_Zhengli;
        public Button m_JinbiAdd;
        public Button m_ZhuanshiAdd;
        public ToggleGroup m_GroupToggle;
        public Toggle m_Quanbu;
        public Toggle m_Yaopin;
        public Toggle m_Zhuangbei;
        public Toggle m_Renwu;
        public Toggle m_Zawu;
        public List<Image> m_Gezi;
        public Text m_Jinbi;
        public Text m_Zhuanshi;

        private bool m_IsInit = false;
        private bool m_IsFull = false;
        private List<ItemBaseUI> m_AllItem;
        private void Start() {
            m_Quanbu.onValueChanged.AddListener((bool ison) => {
                ToggleValueChange(ison, ToggleBtnType.quanbu);
            });
            m_Yaopin.onValueChanged.AddListener((bool ison) => {
                ToggleValueChange(ison, ToggleBtnType.yaopin);
            });
            m_Zhuangbei.onValueChanged.AddListener((bool ison) => {
                ToggleValueChange(ison, ToggleBtnType.zhuangbei);
            });
            m_Renwu.onValueChanged.AddListener((bool ison) => {
                ToggleValueChange(ison, ToggleBtnType.renwu);
            });
            m_Zawu.onValueChanged.AddListener((bool ison) => {
                ToggleValueChange(ison, ToggleBtnType.zawu);
            });
        }
        /// <summary>
        /// toggle变化时监听的方法
        /// </summary>
        /// <param name="ison"></param>
        /// <param name="btntype"></param>
        public void ToggleValueChange(bool ison, ToggleBtnType btntype) {
            switch (btntype) {
                case ToggleBtnType.quanbu:
                    for (int i = 0; i < m_AllItem.Count; i++) {
                        m_AllItem[i].gameObject.SetActive(true);
                    }
                    InitBag(m_AllItem);
                    break;
                case ToggleBtnType.yaopin:
                    for (int i = 0; i < m_AllItem.Count; i++) {
                        if (m_AllItem[i].m_Data.itemtype == (int)ItemType.yaopin) {
                            m_AllItem[i].gameObject.SetActive(true);
                        } else {
                            m_AllItem[i].gameObject.SetActive(false);
                        }
                    }
                    InitBag(m_AllItem);
                    break;
                case ToggleBtnType.zhuangbei:
                    for (int i = 0; i < m_AllItem.Count; i++) {
                        if ((m_AllItem[i].m_Data.itemtype == (int)ItemType.yifu) || (m_AllItem[i].m_Data.itemtype == (int)ItemType.kuzi)|| (m_AllItem[i].m_Data.itemtype == (int)ItemType.wuqi)|| (m_AllItem[i].m_Data.itemtype == (int)ItemType.xianglian)|| (m_AllItem[i].m_Data.itemtype == (int)ItemType.jiezi)|| (m_AllItem[i].m_Data.itemtype == (int)ItemType.xiezi)) {
                            m_AllItem[i].gameObject.SetActive(true);
                        } else {
                            m_AllItem[i].gameObject.SetActive(false);
                        }
                    }
                    InitBag(m_AllItem);
                    break;
                case ToggleBtnType.renwu:
                    for (int i = 0; i < m_AllItem.Count; i++) {
                        if (m_AllItem[i].m_Data.itemtype == (int)ItemType.renwu) {
                            m_AllItem[i].gameObject.SetActive(true);
                        } else {
                            m_AllItem[i].gameObject.SetActive(false);
                        }
                    }
                    InitBag(m_AllItem);
                    break;
                case ToggleBtnType.zawu:
                    for (int i = 0; i < m_AllItem.Count; i++) {
                        if (m_AllItem[i].m_Data.itemtype == (int)ItemType.zawu) {
                            m_AllItem[i].gameObject.SetActive(true);
                        } else {
                            m_AllItem[i].gameObject.SetActive(false);
                        }
                    }
                    InitBag(m_AllItem);
                    break;
                default:
                    Debuger.LogError($"未知类型{btntype},请检查背包Toggle类型");
                    break;
            }
        }
        protected override void OnOpen(object args = null) {
            base.OnOpen(args);
            List<ItemBaseUI> list = args as List<ItemBaseUI>;
            if (!m_IsInit) {
                InitBag(list);
                m_IsInit = true; 
            }
            m_Jinbi.text = UserService.GetInstance.m_CurrentChar.Jinbi.ToString();
            m_Zhuanshi.text = UserService.GetInstance.m_CurrentChar.Zuanshi.ToString();
        }

        public void InitBag(List<ItemBaseUI> list) {
            if (list.Count > 0) {
                m_AllItem = list;
                if (list.Count > m_Gezi.Count) { //物品超了，这种情况应该禁止继续添加物品
                    m_IsFull = true;
                }
                for (int i = 0; i < list.Count; i++) {
                    if (list[i]!=null && list[i].gameObject.activeSelf) {
                        list[i].transform.parent = m_Gezi[i].transform;
                        list[i].transform.localPosition = Vector3.zero;
                    }
                }
            }
        }
       
        protected override void OnClose(bool bClear = false, object arg = null) {
            base.OnClose(bClear, arg);
        }
        public override void Close(bool bClear = false, object arg = null) {
            base.Close(bClear, arg);
        }
    }
}

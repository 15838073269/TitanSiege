/****************************************************
    文件：EquSelectUI.cs
	作者：昔莘
    邮箱: 304183153@qq.com
    日期：2023/10/20 10:36:49
	功能：Nothing
*****************************************************/

using GF.MainGame.Data;
using GF.MainGame.Module;
using GF.Unity.UI;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace GF.MainGame.UI {
    public class EquSelectUI : MonoBehaviour {
        public List<BagBoxUI> m_SelectBoxs;
        public Button m_Close;
        public void Start() {
            m_Close.onClick.AddListener(Close);
        }
        /// <summary>
        /// 初始化获取所有同类装备
        /// </summary>
        /// <param name="itemtype">装备类型</param>
        public void InitEqu(int itemtype) {
            List<ItemBaseUI> tempui = AppTools.GetModule<ItemModule>(MDef.ItemModule).m_CurItem;
            //只有48个格子，最多显示48个
            int listid = 0; 
            if (tempui.Count > 48) {
                listid = 48;
                UIManager.GetInstance.OpenUIWidget(AppConfig.UIMsgTips,"同类装备数量已经达到可展示上线，系统默认只展示前48个，请清理背包后，再选择装备！");
            } else {
                listid = tempui.Count;
            }
            int boxid = 0;//格子的id从0开始
            //这里不能和背包混用一套，单独创建一套
            for (int i = 0; i < listid; i++) {
                if (tempui[i].m_Data.itemtype == itemtype) {
                    ItemBaseUI temp = AppTools.SendReturn<ItemBaseUI>((int)ItemEvent.GetItemUI);
                    temp.Init(tempui[i].m_Data.id, ItemPos.inSelect, tempui[i].Num);
                    temp.transform.SetParent(m_SelectBoxs[boxid].transform);
                    temp.transform.localPosition = Vector3.zero;
                    m_SelectBoxs[boxid].Item = temp;
                    boxid++;
                }
            }
            if (boxid == 0) { //说明一个装备都没有
                UIManager.GetInstance.OpenUIWidget(AppConfig.UIMsgTips,"你没有此类装备！");
            }
            gameObject.SetActive(true);
        }
        public void Close() {
            for (int i = 0; i<m_SelectBoxs.Count; i++) {
                if (m_SelectBoxs[i].Item!=null) {
                    AppTools.Send<ItemBaseUI>((int)ItemEvent.RecycleItemUINoServer, m_SelectBoxs[i].Item);
                    m_SelectBoxs[i].Item = null; 
                }
            }
            gameObject.SetActive(false);
        }
    }
}

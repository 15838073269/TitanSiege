/****************************************************
    文件：MapUIPage.cs
	作者：昔莘
    邮箱: 304183153@qq.com
    日期：2022/12/23 16:18:30
	功能：Nothing
*****************************************************/

using GF.MainGame.Module;
using GF.Unity.UI;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GF.MainGame.UI {
    public class MapUIPage : UIPage {
        public Button m_Clostbtn;
        /// <summary>
        /// 所有地图按钮，先在地图上手动拖进去，回头再写自动加载
        /// </summary>
        public List<MapBtn> m_MapBtnList = new List<MapBtn>();
        public void Start() {
            m_Clostbtn.onClick.AddListener(() => { 
                Close();
            });
        }

        protected override void OnOpen(object args = null) {
            base.OnOpen(args);
            string current = args.ToString();//当前所在场景，如果在这个场景，就把按钮禁用
            for (int i = 0; i < m_MapBtnList.Count; i++) {
                if (current == m_MapBtnList[i].m_ToSceneName) {
                    m_MapBtnList[i].SetBtnInterface(false);
                } else {
                    m_MapBtnList[i].SetBtnInterface(true);
                }
            }
        }
        public override void Close(bool bClear = false, object arg = null) {
            base.Close(bClear, arg);
        }

        protected override void OnClose(bool bClear = false, object arg = null) {
            base.OnClose(bClear, arg);
        }
    }
}
/****************************************************
    文件：BagUIWindow.cs
	作者：昔莘
    邮箱: 304183153@qq.com
    日期：2023/9/30 8:41:9
	功能：Nothing
*****************************************************/

using GF.Service;
using GF.Unity.UI;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GF.MainGame.UI {
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
        protected override void OnOpen(object args = null) {
            base.OnOpen(args);
            List<ItemBaseUI> list = args as List<ItemBaseUI>;
            if (!m_IsInit) {
                InitBag(list);
            }
            m_Jinbi.text = UserService.GetInstance.m_CurrentChar.Jinbi.ToString();
            m_Zhuanshi.text = UserService.GetInstance.m_CurrentChar.Zuanshi.ToString();
        }

        public void InitBag(List<ItemBaseUI> list) {
            if (list.Count > 0) {
                if (list.Count > m_Gezi.Count) { //物品超了，这种情况应该禁止继续添加物品

                }
                for (int i = 0; i < m_Gezi.Count; i++) {

                }
            }
        }
        /// <summary>
        /// 使用道具
        /// </summary>
        /// <param name="item"></param>
        public void UserItem(ItemBaseUI item) { 
            
        }
        /// <summary>
        /// 删除道具
        /// </summary>
        /// <param name="item"></param>
        private void DeleItem(ItemBaseUI item) { 
            
        }
        protected override void OnClose(bool bClear = false, object arg = null) {
            base.OnClose(bClear, arg);
        }
        public override void Close(bool bClear = false, object arg = null) {
            base.Close(bClear, arg);
        }
    }
}

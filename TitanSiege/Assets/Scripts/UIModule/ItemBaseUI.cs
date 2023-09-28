/****************************************************
    文件：EquItemUI.cs
	作者：昔莘
    邮箱: 304183153@qq.com
    日期：2023/9/28 11:36:50
	功能：Nothing
*****************************************************/

using GF.ConfigTable;
using GF.MainGame.Data;
using GF.MainGame.Module;
using GF.Unity.AB;
using UnityEngine;
using UnityEngine.UI;
namespace GF.MainGame.UI {
    public class ItemBaseUI : MonoBehaviour {
        public Image m_ItemColor;
        public Button m_ItemBtn;
        public Text m_ItemLevel;
        public ItemDataBase m_Data;
        /// <summary>
        /// 是否在背包内，因为装备界面用的也是这个，所以判断一下，如果不在背包内，就是装备栏上的
        /// </summary>
        private bool m_IsInBag = true;
        public void Init(int id,bool inbag) {
            ItemModule mod = AppTools.GetModule<ItemModule>(MDef.ItemModule);
            m_Data = mod.m_Data.FindItemByID(id);
            if (m_Data == null) {
                return;
            }
            m_ItemColor.sprite = mod.m_PinzhiDic[m_Data.pinzhi];
            //打开背包一瞬间可能同一时间加载大量图片，所以选择异步加载
            //这里没必要缓存，因为内部加载已经做了缓存了
            ResourceManager.GetInstance.AsyncLoadResource(m_Data.pic, OnLoadSpriteOver,LoadResPriority.RES_HIGHT,bclear:false);
            m_ItemLevel.text = m_Data.level+"";
            m_IsInBag = inbag;

        }
        /// <summary>
        /// 异步资源加载完成后的回调
        /// </summary>
        /// <param name="path"></param>
        /// <param name="obj"></param>
        /// <param name="objarr"></param>
        public void OnLoadSpriteOver(string path, Object obj, params object[] objarr) {
            m_ItemBtn.image.sprite = obj as Sprite;
        }
        private void Start() {
            m_ItemBtn.onClick.AddListener(ClickBtn);
        }
        private void ClickBtn() {
            if (m_IsInBag) { //在背包内的点击，就显示装备介绍的UI
                //todo
            } else { //在装备栏上，显示卸下按钮
                AppTools.Send<ItemType>((int)MainUIEvent.ShowXiexia,(ItemType)m_Data.itemtype);
            }
        }
    }
}

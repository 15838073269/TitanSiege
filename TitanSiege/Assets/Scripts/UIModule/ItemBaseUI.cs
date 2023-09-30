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
        public Text m_ItemNum;
        public ItemDataBase m_Data;
        private int m_Num;//道具数量
        /// <summary>
        /// 是否在背包内，因为装备界面用的也是这个，所以判断一下，如果不在背包内，就是装备栏上的，或者在选择装备界面
        /// </summary>
        private ItemPos m_Pos;
        /// <summary>
        /// 初始化方法
        /// </summary>
        /// <param name="id">物品数据配置表id</param>
        /// <param name="inbag">是否在背包，不在背包</param>
        /// <param name="num"></param>
        public void Init(int id,ItemPos pos,int num = 1) {
            ItemModule mod = AppTools.GetModule<ItemModule>(MDef.ItemModule);
            m_Data = mod.m_Data.FindItemByID(id);
            if (m_Data == null) {
                return;
            }
            m_ItemColor.sprite = mod.m_PinzhiDic[m_Data.pinzhi];
            //打开背包一瞬间可能同一时间加载大量图片，所以选择异步加载
            //这里没必要缓存，因为内部加载已经做了缓存了
            ResourceManager.GetInstance.AsyncLoadResource(m_Data.pic, OnLoadSpriteOver,LoadResPriority.RES_HIGHT,bclear:false);
            if (num == 1) {
                m_ItemNum.text = num.ToString();
                m_ItemNum.gameObject.SetActive(false);
            } else if (num < 1) {
                Debuger.LogError($"道具{id}数量小于0");
            } else if (num>999) {
                num = 999;//强制数量堆叠
                m_ItemNum.text = num.ToString();
                m_ItemNum.gameObject.SetActive(true);
            } else {
                m_ItemNum.text = num.ToString();
                m_ItemNum.gameObject.SetActive(true);
            }
            m_Num = num;
            m_Pos = pos;
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
            switch (m_Pos) {
                case ItemPos.inBag: //在背包内的点击，就显示装备介绍的UI

                    break;
                case ItemPos.inEqu:
                    //在装备栏上，显示卸下按钮
                    AppTools.Send<ItemType>((int)MainUIEvent.ShowXiexia, (ItemType)m_Data.itemtype);
                    break;
                case ItemPos.inSelect:

                    break;
                default:
                    Debuger.Log("未知道具位置，请检查参数！");
                    break;
            }
        }
    }
    
}

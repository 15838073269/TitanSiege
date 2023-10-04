/****************************************************
    文件：ItemModule.cs
	作者：昔莘
    邮箱: 304183153@qq.com
    日期：2023/9/28 12:30:43
	功能：Nothing
*****************************************************/

using UnityEngine;
using GF.Module;
using System.Resources;
using GF.Unity.AB;
using GF.MainGame.Data;
using System.Collections.Generic;
using GF.ConfigTable;
using GF.MainGame.UI;
using GF.Pool;
using UnityEngine.Analytics;
using MoreMountains.Feedbacks;
using GF.Unity.UI;
using System.Text;
using cmd;
using GF.Service;

namespace GF.MainGame.Module {
    public class ItemModule : GeneralModule {
        /// <summary>
        /// 所有道具数据
        /// </summary>
        public ItemData m_Data;
        /// <summary>
        /// 所有品质的图片，目前品质为：白、绿、蓝、紫、橙，五种
        /// </summary>
        public Dictionary<int,Sprite> m_PinzhiDic;
        /// <summary>
        /// 当前玩家的道具
        /// </summary>
        private List<ItemBaseUI> m_CurItem;
        /// <summary>
        /// 对象池
        /// objectmanger的加载中自带对象池，没必要再用对象池管理一次了
        /// </summary>
        //private ClassObjectPool<ItemBaseUI> m_Pool;
        public override void Create() {
            base.Create();
            m_Data = ConfigerManager.GetInstance.FindData<ItemData>(CT.TABLE_ITEM);
            m_PinzhiDic = new Dictionary<int, Sprite>();
            m_CurItem = new List<ItemBaseUI>();
            m_PinzhiDic[(int)Pinzhi.bai] = Unity.AB.ResourceManager.GetInstance.LoadResource<Sprite>("UIRes/item/bai.PNG", bClear:false);
            m_PinzhiDic[(int)Pinzhi.lv] = Unity.AB.ResourceManager.GetInstance.LoadResource<Sprite>("UIRes/item/lv.PNG", bClear: false);
            m_PinzhiDic[(int)Pinzhi.lan] = Unity.AB.ResourceManager.GetInstance.LoadResource<Sprite>("UIRes/item/lan.PNG", bClear: false);
            m_PinzhiDic[(int)Pinzhi.zi] = Unity.AB.ResourceManager.GetInstance.LoadResource<Sprite>("UIRes/item/zi.PNG", bClear: false);
            m_PinzhiDic[(int)Pinzhi.cheng] = Unity.AB.ResourceManager.GetInstance.LoadResource<Sprite>("UIRes/item/cheng.PNG", bClear: false);
            AppTools.Regist((int)ItemEvent.ShowBag, ShowBag);
            AppTools.Regist<ItemBaseUI>((int)ItemEvent.ShowItemDesc, ShowItemDesc);
            AppTools.Regist<ItemDataBase,int>((int)ItemEvent.AddItemUIIfNoneCreateInBag, AddItemUIIfNoneCreateInBag);
        }
       
        /// <summary>
        /// 显示背包ui界面
        /// </summary>
        public void ShowBag() {
            if (m_CurItem.Count > 0) {

            } else {
                //重新生成
                if (UserService.GetInstance.m_UserItem.Count > 0) {//这里默认没有处理已装备的装备，所以需要装备时，将数据排除掉
                    foreach (KeyValuePair<int, int> tmp in UserService.GetInstance.m_UserItem) {
                        //注意通过objectmager加载的ui路径和UImanager加载的不同
                        CreateItemUI(tmp.Key, ItemPos.inBag, tmp.Value);
                    }
                } else { //空背包，无物品

                }
            }
            UIManager.GetInstance.OpenWindow(AppConfig.BagUIWindow, m_CurItem);
        }
        
        /// <summary>
        /// 增加或者减少道具的数量
        /// </summary>
        /// <param name="id"></param>
        /// <param name="num"></param>
        public bool AddItemNum(int id,int num) {
            if (UserService.GetInstance.m_UserItem.TryGetValue(id, out int ordernum)) {
                if (num < 0) {//减少数量，首先先判断一下现有的,数量不足就返回false
                    if (ordernum < num) {
                        return false;
                    } else {
                        ordernum -= num;
                    }
                } else {
                    ordernum += num;
                }
            }
            return true;
        }

        /// <summary>
        /// 显示物品详情
        /// </summary>
        /// <param name="data"></param>
        private void ShowItemDesc(ItemBaseUI itemui) {
            if (itemui == null) {
                Debuger.LogError("该道具无数据显示，请检查道具id是否正确！");
                return;
            }
            UIManager.GetInstance.OpenUIWidget(AppConfig.ItemDescUIWidget,itemui);
        }
        /// <summary>
        /// 查询背包里是否有这个物品，如果没有，就创建一个
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public void AddItemUIIfNoneCreateInBag(ItemDataBase data,int num = 1) {
            ItemBaseUI itemui = null;
            for (int i = 0; i < m_CurItem.Count; i++) {
                if (m_CurItem[i].m_Data.id == data.id) {
                    itemui = m_CurItem[i];
                    itemui.Num += num;
                    break;
                }
            }
            if (itemui == null) {//如果没有就创建一个
                itemui = CreateItemUI(data.id,ItemPos.inBag, num);
                //创建完还要通知一下背包新增了一个物品
                //todo
            }
            //发送服务器通知背包的数据变化
            //todo
        }
        /// <summary>
        /// 往背包中创建物品的ui
        /// </summary>
        /// <param name="id"></param>
        /// <param name="itempos"></param>
        /// <param name="num"></param>
        /// <returns></returns>
        private ItemBaseUI CreateItemUI(int id,ItemPos itempos,int num = 1) {
            //注意通过objectmager加载的ui路径和UImanager加载的不同
            ItemBaseUI itemui = ObjectManager.GetInstance.InstanceObject(AppConfig.ItemBaseUI, father: AppMain.GetInstance.uiroot.transform, bClear: false).GetComponent<ItemBaseUI>();
            itemui.Init(id, itempos, num);
            m_CurItem.Add(itemui);
            return itemui;
        }
        public override void Release() {
            base.Release();
            m_PinzhiDic.Clear();
        }

    }
}

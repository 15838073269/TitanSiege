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
using UnityEditor;

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
        /// ui的对象池需要单独管理，objectmanger中的对象池是给gameobject物体用的，并没有针对ui，所以ui的对象池需要单独处理一下
        /// </summary>
        private ClassObjectPool<ItemBaseUI> m_Pool;
        
        /// <summary>
        /// 背包的ui，存着备用
        /// </summary>
        private BagUIWindow m_BagUI;
        public override void Create() {
            base.Create();
            m_Pool = new ClassObjectPool<ItemBaseUI>(50,true);
            m_Pool.CretateObj = CreateItemUIToPool;
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
            AppTools.Regist<ItemDataBase,int,bool>((int)ItemEvent.AddItemUIIfNoneCreateInBag, AddItemUIIfNoneCreateInBag);
            AppTools.Regist<ItemBaseUI>((int)ItemEvent.RecycleItemUI, RecycleItemUI);
            AppTools.Regist<int,ItemPos,int,ItemBaseUI>((int)ItemEvent.CreateItemUI, CreateItemUI);
        }
        /// <summary>
        /// 给对象池使用的创建物品ui
        /// </summary>
        /// <returns></returns>
        private ItemBaseUI CreateItemUIToPool() {
            //注意通过objectmager加载的ui路径和UImanager加载的不同
            ItemBaseUI itemui = ObjectManager.GetInstance.InstanceObject(AppConfig.ItemBaseUI, father: AppMain.GetInstance.uiroot.transform, bClear: false).GetComponent<ItemBaseUI>();
            return itemui;
        }
        /// <summary>
        /// 回收物品对象池
        /// </summary>
        /// <param name="itemui"></param>
        public void RecycleItemUI(ItemBaseUI itemui) {
            m_CurItem.Remove(itemui);//从背包存储中先删除
            for (int i = 0; i < m_BagUI.m_Boxs.Count; i++) {//再把格子里的清理掉
                if (m_BagUI.m_Boxs[i].Item == itemui) {
                    m_BagUI.m_Boxs[i].Item = null; 
                    break;
                }
            }
            if (m_Pool.Recycle(itemui)) {
                itemui.transform.SetParent(AppMain.GetInstance.m_UIPoolRoot.transform);
                itemui.gameObject.SetActive(false);
            } else {
                Object.Destroy(itemui.gameObject);
            }
        }
        public ItemBaseUI GetItemUI() {
            ItemBaseUI itemui = m_Pool.GetObj(true);
            return itemui;
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
            if (m_BagUI == null) {
                m_BagUI = UIManager.GetInstance.OpenWindow(AppConfig.BagUIWindow, m_CurItem) as BagUIWindow;
            } else {
                UIManager.GetInstance.OpenWindow(AppConfig.BagUIWindow, m_CurItem);
            }
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
        public bool AddItemUIIfNoneCreateInBag(ItemDataBase data,int num = 1) {
            ItemBaseUI itemui = null;
            for (int i = 0; i < m_CurItem.Count; i++) {
                if (m_CurItem[i].m_Data.id == data.id) {
                    itemui = m_CurItem[i];
                    itemui.Num += num;
                    break;
                }
            }
            if (itemui == null) {//如果没有就创建一个
                if (!m_BagUI.m_IsFull) {
                    itemui = CreateItemUI(data.id, ItemPos.inBag, num);
                } else {
                   
                    return false;
                }
            }
            return true;
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
            ItemBaseUI itemui = GetItemUI();
            itemui.Init(id, itempos, num);
            if (!itemui.gameObject.activeSelf) {
                itemui.gameObject.SetActive(true);
            }
            m_CurItem.Add(itemui);
            //处理背包
            if (m_BagUI!=null && m_BagUI.m_IsInit) {//只有初始化之后的背包，出现变化才需要手动添加
                BagBoxUI box = m_BagUI.ReturnEmptyBox;
                box.Item = itemui;
                itemui.transform.parent = box.transform;
                itemui.transform.localPosition = Vector3.zero;
            }
            return itemui;
        }
        public override void Release() {
            base.Release();
            m_PinzhiDic.Clear();
        }

    }
}

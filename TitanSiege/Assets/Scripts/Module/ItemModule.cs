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
        /// 道具ui的对象池
        /// </summary>
        private ClassObjectPool<ItemBaseUI> m_UIPool;

        private Dictionary<int, int> m_UserItem;

        private List<ItemBaseUI> m_CurItem;
        public override void Create() {
            base.Create();
            m_Data = ConfigerManager.GetInstance.FindData<ItemData>(CT.TABLE_ITEM);
            m_PinzhiDic = new Dictionary<int, Sprite>();
            m_UIPool = new ClassObjectPool<ItemBaseUI>(80);//这里的数量可能不够
            m_UserItem = new Dictionary<int, int>();
            m_CurItem = new List<ItemBaseUI>();
            m_PinzhiDic[(int)Pinzhi.bai] = Unity.AB.ResourceManager.GetInstance.LoadResource<Sprite>("UIRes/item/bai.PNG", bClear:false);
            m_PinzhiDic[(int)Pinzhi.lv] = Unity.AB.ResourceManager.GetInstance.LoadResource<Sprite>("UIRes/item/lv.PNG", bClear: false);
            m_PinzhiDic[(int)Pinzhi.lan] = Unity.AB.ResourceManager.GetInstance.LoadResource<Sprite>("UIRes/item/lan.PNG", bClear: false);
            m_PinzhiDic[(int)Pinzhi.zi] = Unity.AB.ResourceManager.GetInstance.LoadResource<Sprite>("UIRes/item/zi.PNG", bClear: false);
            m_PinzhiDic[(int)Pinzhi.cheng] = Unity.AB.ResourceManager.GetInstance.LoadResource<Sprite>("UIRes/item/cheng.PNG", bClear: false);
            AppTools.Regist((int)ItemEvent.GetItemUI, GetItemUI);
            AppTools.Regist((int)ItemEvent.ShowBag, ShowBag);
            AppTools.Regist<ItemBaseUI>((int)ItemEvent.RecycleItemUI, RecycleItemUI);
        }
        public ItemBaseUI GetItemUI() {
            ItemBaseUI item = m_UIPool.GetObj(true);
            return item;
        }
        public void RecycleItemUI(ItemBaseUI item) {
            m_UIPool.Recycle(item);
        }
        /// <summary>
        /// 显示背包ui界面
        /// </summary>
        public void ShowBag() {
            //先清理一下，回收itemui
            if (m_CurItem.Count>0) { 
                for (int i = 0; i < m_CurItem.Count; i++) {
                    RecycleItemUI(m_CurItem[i]);
                }
                m_CurItem.Clear();
            }
            //重新生成
            if (m_UserItem.Count > 0) {//这里默认没有处理已装备的装备，所以需要装备时，将数据排除掉
                foreach (KeyValuePair<int, int> tmp in m_UserItem) {
                    ItemBaseUI itemui = GetItemUI();
                    itemui.Init(tmp.Key,ItemPos.inBag,tmp.Value);
                    m_CurItem.Add(itemui);
                }
            } else { //空背包，无物品

            }
            UIManager.GetInstance.OpenWindow(AppConfig.BagUIWindow, m_CurItem);
        }
        /// <summary>
        /// 将数据库中的字符串道具数据初始化成字典
        /// 字符串的结构为：1|1,2|1,234|10,12|1,
        /// </summary>
        /// <param name="str"></param>
        public void InitUserItem(string str) {
            string[] strarr = str.Split(',');
            for (int i = 0; i < strarr.Length; i++) {
                if (!string.IsNullOrEmpty(strarr[i])) {//保险起见，加一层判断
                    string[] strarr1 = strarr[i].Split("|");
                    int itemid = int.Parse(strarr1[0]);
                    int itemnum = int.Parse(strarr1[1]);
                    if (itemid != 0&& itemnum>=1) {
                        m_UserItem.Add(itemid, itemnum);
                    } else {
                        Debuger.LogError("道具数据解析错误，请检查");
                    }
                }
            }
        }
        /// <summary>
        /// 增加或者减少道具的数量
        /// </summary>
        /// <param name="id"></param>
        /// <param name="num"></param>
        public bool AddItemNum(int id,int num) {
            if (m_UserItem.TryGetValue(id, out int ordernum)) {
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
        /// 将现有道具转换成字符串，数据库写入使用
        /// </summary>
        /// <returns></returns>
        public string UserItemToStr() {
            StringBuilder sb = new StringBuilder();
            foreach (KeyValuePair<int, int> tmp in m_UserItem) {
                if (tmp.Key!=0&&tmp.Value>0) {
                    sb.Append($"{tmp.Key}|{tmp.Value},");
                }
            }
            return sb.ToString();
        }
        public override void Release() {
            base.Release();
            m_PinzhiDic.Clear();
            m_UIPool.ClearPool();
        }
    }
}

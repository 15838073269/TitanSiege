/****************************************************
    文件：ItemModule.cs
	作者：昔莘
    邮箱: 304183153@qq.com
    日期：2023/9/28 12:30:43
	功能：Nothing
*****************************************************/

using UnityEngine;
using GF.Module;
using GF.Unity.AB;
using GF.MainGame.Data;
using System.Collections.Generic;
using GF.ConfigTable;
using GF.MainGame.UI;
using GF.Pool;
using GF.Unity.UI;
using cmd;
using GF.Service;
using Net.Client;
using Cysharp.Threading.Tasks;
using Titansiege;

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
        public List<ItemBaseUI> m_CurItem;
        /// <summary>
        /// 对象池
        /// ui的对象池需要单独管理，objectmanger中的对象池是给gameobject物体用的，并没有针对ui，所以ui的对象池需要单独处理一下
        /// </summary>
        private ClassObjectPool<ItemBaseUI> m_Pool;
        /// <summary>
        /// 所有道具的属性值，备用
        /// </summary>
        public Dictionary<int, ItemBase> m_AllItem = new Dictionary<int, ItemBase>();
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
            AppTools.Regist<ItemDataBase,int, UniTask<bool>>((int)ItemEvent.AddItemUIIfNoneCreateInBag, AddItemUIIfNoneCreateInBag);
            AppTools.Regist<ItemBaseUI, UniTask<bool>>((int)ItemEvent.RecycleItemUI, RecycleItemUI);
            AppTools.Regist<ItemBaseUI>((int)ItemEvent.RecycleItemUINoServer, RecycleItemUINoServer);
            AppTools.Regist<ItemBaseUI>((int)ItemEvent.GetItemUI, GetItemUI);
            AppTools.Regist<int,ItemPos,int, UniTask<ItemBaseUI>>((int)ItemEvent.CreateItemUI, CreateItemUI);
            AppTools.Regist<ItemBaseUI,int, UniTask<bool>>((int)ItemEvent.ToDoItemNum, ToDoItemNum);
            AppTools.Regist<int, ushort>((int)ItemEvent.AddJinbiOrZuanshi, AddJinbiOrZuanshi);
            AppTools.Regist<int, ItemBase>((int)ItemEvent.GetItemProp, GetItemProp);
            AppTools.Regist<bool>((int)ItemEvent.BagIsFull, BagIsFull);
            //初始化所有装备道具的效果值备用
            InitAllItemProp();
        }
        /// <summary>
        /// 其他模块用来判断背包是否满了
        /// </summary>
        /// <returns></returns>
        public bool BagIsFull() {
            return m_BagUI.m_IsFull;
        }
        /// <summary>
        /// //初始化所有装备道具的效果值备用
        /// </summary>
        public void InitAllItemProp() {
            foreach (KeyValuePair<int, ItemDataBase> itemdata in m_Data.m_AllItemBaseDic) {
                ItemBase temp = new ItemBase(itemdata.Key);
                m_AllItem.Add(itemdata.Key, temp);
            }
        }
        /// <summary>
        /// 通过id获取道具的属性，目前用来获取效果值，其他需求什么的都可以放这里面，但我懒得改了
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ItemBase GetItemProp(int id) {
            ItemBase item = null;
            if (m_AllItem.TryGetValue(id, out item)) {
                return item;
            } else {
                return null;
            }
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
        public async UniTask<bool> RecycleItemUI(ItemBaseUI itemui) {
            //发送服务器删除背包物品
            var task = await ClientBase.Instance.Call((ushort)ProtoType.DestroyItem,pars:itemui.m_Data.id);
            if (task.IsCompleted) {
                int code = task.model.AsInt;
                if (code == 0||code == -1) {//删除背包内ui，code==-1是服务器中没有这个道具，一般是因为处理数据为0时删除了，或者玩家作弊，不过删除道具对游戏没影响，这里先不单独处理了，留个接口拓展吧。
                    UserService.GetInstance.m_UserItem.Remove(itemui.m_Data.id);//将数据中的删除
                    m_CurItem.Remove(itemui);//从背包存储中先删除
                    for (int i = 0; i < m_BagUI.m_Boxs.Count; i++) {//再把格子里的清理掉
                        if (m_BagUI.m_Boxs[i].Item == itemui) {
                            m_BagUI.m_Boxs[i].Item = null;
                            break;
                        }
                    }
                    //处理对象池
                    RecycleItemUINoServer(itemui);
                    return true;
                }
                return false;
            } else {
                UIManager.GetInstance.OpenUIWidget(AppConfig.UIMsgTips, $"请求超时，请检查网络链接");
                return false;
            }
        }
        /// <summary>
        /// 不需要传输服务器的回收物品对象池
        /// 无需背包处理，主要是选择栏用的，只要把对象回收销毁即可
        /// </summary>
        /// <param name="itemui"></param>
        public void RecycleItemUINoServer(ItemBaseUI itemui) {
            //处理对象池
            if (m_Pool.Recycle(itemui)) {
                itemui.transform.SetParent(AppMain.GetInstance.m_UIPoolRoot.transform);
                itemui.gameObject.SetActive(false);
            } else {
                Object.Destroy(itemui.gameObject);
            }
        }
        public ItemBaseUI GetItemUI() {
            ItemBaseUI itemui = m_Pool.GetObj(true);
            if (!itemui.gameObject.activeSelf) {
                itemui.gameObject.SetActive(true);
            }
            return itemui;
        }
        /// <summary>
        /// 显示背包ui界面
        /// </summary>
        public void ShowBag() {
            if (m_CurItem.Count > 0) {//已经初始化了

            } else {
                //重新生成
                if (UserService.GetInstance.m_UserItem.Count > 0) {//这里默认没有处理已装备的装备，所以需要装备时，将数据排除掉
                    foreach (KeyValuePair<int, int> tmp in UserService.GetInstance.m_UserItem) {
                        //注意通过objectmager加载的ui路径和UImanager加载的不同
                        CreateItemUINoServer(tmp.Key, ItemPos.inBag, tmp.Value);
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
        public async UniTask<bool> ToDoItemNum(ItemBaseUI itemui,int num) {
            if (!UserService.GetInstance.m_UserItem.ContainsKey(itemui.m_Data.id)) {
                if (num > 0) {//添加道具的情况，不存在，就要添加一下
                    UserService.GetInstance.m_UserItem.Add(itemui.m_Data.id,0);
                } 
            }
            if (UserService.GetInstance.m_UserItem.TryGetValue(itemui.m_Data.id, out int ordernum)) {
                if (num < 0) {//减少数量，首先先判断一下现有的,数量不足就返回false
                    if (ordernum < Mathf.Abs(num)) {
                        return false;//数量不足
                    }
                }
                //发送服务器处理
                var task = await ClientBase.Instance.Call((ushort)ProtoType.TodoItemNum, timeoutMilliseconds: 0, itemui.m_Data.id, num);
                if (task.IsCompleted) {
                    int code = task.model.AsInt;
                    switch (code) {
                        case -2:
                            //服务器存储没有这个物品，客户端作弊非法操作，直接关闭客户端
                            UIMsgBoxArg arg = new UIMsgBoxArg();
                            arg.title = "警告！";
                            arg.content = "发现数据作弊，系统发出警告，将强行退出游戏！";
                            arg.btnname = "关闭游戏";
                            UIMsgBox box = UIManager.GetInstance.OpenWindow(AppConfig.UIMsgBox, arg) as UIMsgBox;
                            box.oncloseevent += AppTools.OnFailed;
                            return false;
                        case -1:
                            //服务器存储数量不足，客户端作弊非法操作，直接关闭客户端
                            UIMsgBoxArg arg1 = new UIMsgBoxArg();
                            arg1.title = "警告！";
                            arg1.content = "发现数据作弊，系统发出警告，将强行退出游戏！";
                            arg1.btnname = "关闭游戏";
                            UIMsgBox box1 = UIManager.GetInstance.OpenWindow(AppConfig.UIMsgBox, arg1) as UIMsgBox;
                            box1.oncloseevent += AppTools.OnFailed;
                            return false;
                        case 0:
                            UserService.GetInstance.m_UserItem[itemui.m_Data.id] += num;
                            //这里不用处理，应为num等于0时，会自动销毁处理
                            //if (UserService.GetInstance.m_UserItem[itemui.m_Data.id] <= 0) {
                            //    UserService.GetInstance.m_UserItem.Remove(itemui.m_Data.id);
                            //}
                            itemui.Num += num;
                            break;
                    }
                    return true;
                } else {
                    UIManager.GetInstance.OpenUIWidget(AppConfig.UIMsgTips, $"请求超时，请检查网络链接");
                    return false;
                }
            } else {
                Debuger.Log("背包内没有这个道具");
                return false;
            }
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
        public async UniTask<bool> AddItemUIIfNoneCreateInBag(ItemDataBase data,int num = 1) {
            ItemBaseUI itemui = null;
            for (int i = 0; i < m_CurItem.Count; i++) {
                if (m_CurItem[i].m_Data.id == data.id) {
                    itemui = m_CurItem[i];
                    _ = ToDoItemNum(itemui,num);
                    break;
                }
            }
            if (itemui == null) {//如果没有就创建一个
                if (!m_BagUI.m_IsFull) {
                    itemui = await CreateItemUI(data.id, ItemPos.inBag, num);
                    if (itemui == null) {
                        //网络问题创建失败，或者非法
                        return false;
                    }
                } else {
                    return false;
                }
            }
            return true;
        }
        /// <summary>
        /// 往背包中创建物品的ui，一般用于往背包添加物品，初始化背包不用这个
        /// </summary>
        /// <param name="id"></param>
        /// <param name="itempos"></param>
        /// <param name="num"></param>
        /// <returns></returns>
        private async UniTask<ItemBaseUI> CreateItemUI(int id,ItemPos itempos,int num = 1) {
            //发送服务器创建道具
            var task = await ClientBase.Instance.Call((ushort)ProtoType.CreateItemToBag,timeoutMilliseconds:0,id,num);
            if (task.IsCompleted) {
                int code = task.model.AsInt;
                if (code == 0) {
                    //服务端创建成功后，就往数据中添加一下
                    UserService.GetInstance.m_UserItem.Add(id,num);
                    //处理客户端ui，注意通过objectmager加载的ui路径和UImanager加载的不同
                    ItemBaseUI itemui = GetItemUI();
                    itemui.Init(id, itempos, num);
                    if (!itemui.gameObject.activeSelf) {
                        itemui.gameObject.SetActive(true);
                    }
                    m_CurItem.Add(itemui);
                    //处理背包
                    if (m_BagUI != null && m_BagUI.m_IsInit) {//只有初始化之后的背包，出现变化才需要手动添加
                        BagBoxUI box = m_BagUI.ReturnEmptyBox;
                        box.Item = itemui;
                        itemui.transform.SetParent(box.transform);
                        itemui.transform.localPosition = Vector3.zero;
                    }
                    return itemui;
                } else {
                    return null;
                }
            } else {
                //网络错误，直接返回
                UIManager.GetInstance.OpenUIWidget(AppConfig.UIMsgTips, "网络错误，请检查网络连接！");
                return null;
            }
        }
        /// <summary>
        /// 初始化背包使用的，与上面区分
        /// </summary>
        /// <param name="id"></param>
        /// <param name="itempos"></param>
        /// <param name="num"></param>
        /// <returns></returns>
        private  ItemBaseUI CreateItemUINoServer(int id, ItemPos itempos, int num = 1) {
            //处理客户端ui，注意通过objectmager加载的ui路径和UImanager加载的不同
            ItemBaseUI itemui = GetItemUI();
            itemui.Init(id, itempos, num);
            if (!itemui.gameObject.activeSelf) {
                itemui.gameObject.SetActive(true);
            }
            m_CurItem.Add(itemui);
            //处理背包
            if (m_BagUI != null && m_BagUI.m_IsInit) {//只有初始化之后的背包，出现变化才需要手动添加
                BagBoxUI box = m_BagUI.ReturnEmptyBox;
                box.Item = itemui;
                itemui.transform.SetParent(box.transform);
                itemui.transform.localPosition = Vector3.zero;
            }
            return itemui;
        }
        /// <summary>
        /// 增加金币或者钻石
        /// </summary>
        /// <param name="num"></param>
        /// <param name="typenum"></param>
        public void AddJinbiOrZuanshi(int num, ushort typenum) {
            CharactersData cd = UserService.GetInstance.m_CurrentChar;
            switch (typenum) {
                case 0:
                    cd.Jinbi += num;
                    m_BagUI.m_Jinbi.text = cd.Jinbi.ToString();
                    break;
                case 1:
                    cd.Zuanshi += num;
                    m_BagUI.m_Zhuanshi.text = cd.Zuanshi.ToString();
                    break;
                default:
                    break;
            }
        }
        public override void Release() {
            base.Release();
            m_PinzhiDic.Clear();
        }

    }
}

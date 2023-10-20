/****************************************************
    文件：InfoUIWindow.cs
	作者：昔莘
    邮箱: 304183153@qq.com
    日期：2023/9/11 21:33:13
	功能：角色信息窗口
*****************************************************/

using GF.MainGame.Module;
using GF.Unity.UI;
using GF.Unity.AB;
using UnityEngine;
using Unity.VisualScripting;
using UnityEngine.UI;
using GF.MainGame.Module.NPC;
using GF.Service;
using Titansiege;
using GF.MainGame.Data;
using DG.Tweening;
using System.Collections.Generic;
using GF.Pool;
using static UnityEditor.PlayerSettings;
using System.Security.Cryptography;
using Cysharp.Threading.Tasks;
using Net.Client;
using System.Net.Sockets;
using cmd;

namespace GF.MainGame.UI {
    public class InfoUIWindow : UIWindow {
        public Text m_NameTxt;
        public Text m_GdidTxt;
        public Text m_ChenghaoTxt;
        public Text m_ZhiyeTxt;
        public Text m_LvTxt;
        public Text m_ShengmingTxt;
        public Image m_ShengmingImg;
        public Text m_MofaTxt;
        public Image m_MofaImg;
        public Text m_ExpTxt;
        public Image m_ExpImg;
        public Text m_GongjiTxt;
        public Text m_FangyuTxt;
        public Text m_ShanbiTxt;
        public Text m_BaojiTxt;
        public Text m_TizhiTxt;
        public Text m_LiliangTxt;
        public Text m_MinjieTxt;
        public Text m_MoliTxt;
        public Text m_MeiliTxt;
        public Text m_XingyunTxt;
        public Text m_ZhandouliTxt;
        /// <summary>
        /// 装备栏
        /// </summary>
        public Button yifubtn;
        public ItemBaseUI yifuitem;//这里直接固定在ui面板上了，省得再来回加载了
        public Button kuzibtn;
        public ItemBaseUI kuziitem;
        public Button wuqibtn;
        public ItemBaseUI wuqiitem;
        public Button xianglianbtn;
        public ItemBaseUI xianglianitem;
        public Button jiezibtn;
        public ItemBaseUI jieziitem;
        public Button xiezibtn;
        public ItemBaseUI xieziitem;
        public Text yitxt;
        public Text kutxt;
        public Text wutxt;
        public Text xiangtxt;
        public Text jietxt;
        public Text xietxt;
        /// <summary>
        /// 装备选择的界面，会自动筛选
        /// </summary>
        public EquSelectUI m_EquSelect;
        /// <summary>
        /// 显示的模型
        /// </summary>
        private ModelShow m_ModelShow;
        /// <summary>
        /// 当前装备栏对应的装备id
        /// </summary>
        private Dictionary<ItemType, int> m_EquPosDic = new Dictionary<ItemType, int>();
        /// <summary>
        /// 当前选择中的装备栏
        /// 目前用来显示装备信息和卸下装备
        /// </summary>
        private ItemBaseUI m_CurrentItem = null;
        
        public void Awake() {
            if (m_ModelShow == null) {
                GameObject go = ObjectManager.GetInstance.InstanceObject("NPCPrefab/modelcamera.prefab",bClear:false);
                if (go!=null) {
                    m_ModelShow = go.GetComponent<ModelShow>();
                }
            }
            yifubtn.onClick.AddListener(() => {
                ShowEquSelect(ItemType.yifu);
            });
            kuzibtn.onClick.AddListener(() => {
                ShowEquSelect(ItemType.kuzi);
            });
            wuqibtn.onClick.AddListener(() => {
                ShowEquSelect(ItemType.wuqi);
            });
            xianglianbtn.onClick.AddListener(() => {
                ShowEquSelect(ItemType.xianglian);
            });
            jiezibtn.onClick.AddListener(() => {
                ShowEquSelect(ItemType.jiezi);
            });
            xiezibtn.onClick.AddListener(() => {
                ShowEquSelect(ItemType.xiezi);
            });
            //初始化装备栏
            m_EquPosDic[ItemType.wuqi] = -1;
            m_EquPosDic[ItemType.yifu] = -1;
            m_EquPosDic[ItemType.kuzi] = -1;
            m_EquPosDic[ItemType.xianglian] = -1;
            m_EquPosDic[ItemType.jiezi] = -1;
            m_EquPosDic[ItemType.xiezi] = -1;
           
        }
        /// <summary>
        /// 显示卸下装备按钮，或者显示选择装备面板
        /// </summary>
        public void ShowEquSelect(ItemType itemtype) {
            m_EquSelect.InitEqu((int)itemtype);
            
        }
        protected override void OnOpen(object args = null) {
            base.OnOpen(args);
            if (m_ModelShow == null) {
                GameObject go = ObjectManager.GetInstance.InstanceObject("NPCPrefab/modelcamera.prefab", bClear: false);
                if (go != null) {
                    m_ModelShow = go.GetComponent<ModelShow>();
                }
            }
            m_ModelShow.gameObject.SetActive(true);
            CharactersData cd = UserService.GetInstance.m_CurrentChar;
            Player p = UserService.GetInstance.m_CurrentPlayer;
            DataToUI(cd,p);//更新属性到ui面板
            #region 显示装备并存储
            //衣服装备栏
            if (cd.Yifu > 0) {
                if (m_EquPosDic[ItemType.yifu] != cd.Yifu) {//避免重复初始化,浪费性能
                    m_EquPosDic[ItemType.yifu] = cd.Yifu;
                    yifuitem.Init(cd.Yifu, ItemPos.inEqu);
                }
                yifuitem.gameObject.SetActive(true);
            } else {//有可能会等于0
                yifuitem.gameObject.SetActive(false);
            }
            //裤子装备栏
            if (cd.Kuzi > 0) {
                if (m_EquPosDic[ItemType.kuzi] != cd.Kuzi) {//避免重复初始化
                    m_EquPosDic[ItemType.kuzi] = cd.Kuzi;
                    kuziitem.Init(cd.Kuzi, ItemPos.inEqu);
                }
                kuziitem.gameObject.SetActive(true);
            } else {//有可能会等于0
                kuziitem.gameObject.SetActive(false);
            }
            //武器装备栏
            if (cd.Wuqi > 0) {
                if (m_EquPosDic[ItemType.wuqi] != cd.Wuqi) {//避免重复初始化
                    m_EquPosDic[ItemType.wuqi] = cd.Wuqi;
                    wuqiitem.Init(cd.Wuqi, ItemPos.inEqu);
                }
                wuqiitem.gameObject.SetActive(true);
            } else {//有可能会等于0
                wuqiitem.gameObject.SetActive(false);
            }
            //项链装备栏
            if (cd.Xianglian > 0) {
                if (m_EquPosDic[ItemType.xianglian] != cd.Xianglian) {//避免重复初始化
                    m_EquPosDic[ItemType.xianglian] = cd.Xianglian;
                    xianglianitem.Init(cd.Xianglian, ItemPos.inEqu);
                }
                xianglianitem.gameObject.SetActive(true);
            } else {//有可能会等于0
                xianglianitem.gameObject.SetActive(false);
            }
            //戒子装备栏
            if (cd.Jiezi > 0) {
                if (m_EquPosDic[ItemType.jiezi] != cd.Jiezi) {//避免重复初始化
                    m_EquPosDic[ItemType.jiezi] = cd.Jiezi;
                    jieziitem.Init(cd.Jiezi, ItemPos.inEqu); 
                }
                jieziitem.gameObject.SetActive(true);
            } else {//有可能会等于0
                jieziitem.gameObject.SetActive(false);
            } 
            //鞋子装备栏
            if (cd.Xiezi > 0) {
                if (m_EquPosDic[ItemType.xiezi] != cd.Xiezi) {//避免重复初始化
                    m_EquPosDic[ItemType.xiezi] = cd.Xiezi;
                    xieziitem.Init(cd.Xiezi, ItemPos.inEqu);
                }
                xieziitem.gameObject.SetActive(true);
            } else {//有可能会等于0
                xieziitem.gameObject.SetActive(false);
            }
            #endregion
            //隐藏非必要内容
            m_EquSelect.gameObject.SetActive(false);
        }
        private void DataToUI(CharactersData cd=null, Player p = null) {
            if (cd == null) {
                cd = UserService.GetInstance.m_CurrentChar;
                p = UserService.GetInstance.m_CurrentPlayer;
            }
            m_NameTxt.text = p.m_PlayerName;
            m_GdidTxt.text = $"ID:{p.m_GDID.ToString()}";
            m_ChenghaoTxt.text = cd.Chenghao;
            m_ZhiyeTxt.text = ((Zhiye)cd.Zhiye).ToString();
            m_LvTxt.text = $"{cd.Level}级";
            m_ShengmingTxt.text = $"{p.FP.FightHP}/{p.FP.FightMaxHp}";
            m_ShengmingImg.fillAmount = (float)p.FP.FightHP / (float)p.FP.FightMaxHp;
            m_MofaTxt.text = $"{p.FP.FightMagic}/{p.FP.FightMaxMagic}";
            m_MofaImg.fillAmount = (float)p.FP.FightMagic / (float)p.FP.FightMaxMagic;
            int upexp = cd.Level * cd.Level * p.LevelData.UpExp;//升级经验算法
            m_ExpTxt.text = $"{cd.Exp}/{upexp}";
            m_ExpImg.fillAmount = (float)cd.Exp / (float)upexp;
            m_GongjiTxt.text = p.FP.Attack.ToString();
            m_FangyuTxt.text = p.FP.Defense.ToString();
            m_ShanbiTxt.text = p.FP.Dodge.ToString("P1");
            m_BaojiTxt.text = p.FP.Crit.ToString("P1");
            m_TizhiTxt.text = cd.Tizhi.ToString();
            m_LiliangTxt.text = cd.Liliang.ToString();
            m_MinjieTxt.text = cd.Minjie.ToString();
            m_MoliTxt.text = cd.Moli.ToString();
            m_MeiliTxt.text = cd.Meili.ToString();
            m_XingyunTxt.text = cd.Xingyun.ToString();
            //战斗力计算：攻击/2+防御+闪避*5000+暴击*6000+生命/4+魔力/5
            m_ZhandouliTxt.text = (p.FP.Attack / 2 + p.FP.Defense + p.FP.Dodge * 5000 + p.FP.Crit * 2000 + p.FP.FightMaxHp / 4 + p.FP.FightMagic / 6).ToString();
        }
       
        /// <summary>
        /// 说是交换，其实就是装备，装备有两种情况，一种在背包内直接点击装备，另一种在选择栏上，在背包中，装备栏上不一定有装备，但在选择栏上，装备栏上一定没装备，因为按照设计逻辑，只有装备栏为空才能打开选择栏
        /// </summary>
        /// <param name="equ"></param>
        public async void ChangeEquItem(ItemBaseUI equ) {
            if (equ != null) {
                //如果是在选择栏上，得找到背包中对应的道具，用背包中的道具装备，否则会出现问题
                if (equ.m_Pos == ItemPos.inSelect) {
                    m_EquSelect.Close();
                    List<ItemBaseUI> itemlist = AppTools.GetModule<ItemModule>(MDef.ItemModule).m_CurItem;
                    for (int i = 0; i < itemlist.Count; i++) {
                        if (equ.m_Data.id == itemlist[i].m_Data.id) {
                            equ = itemlist[i];
                            break;
                        }
                    }
                } 
                CharactersData cd = UserService.GetInstance.m_CurrentChar;
                FightProp fp = UserService.GetInstance.m_CurrentPlayer.FP;
                //bool bagisfull = AppTools.SendReturn<bool>((int)ItemEvent.BagIsFull);

                var equtempdic = equ.m_Itembase.m_PropXiaoguoDic;//先缓存起来
                Dictionary<XiaoGuo,Dictionary<string,EffArgs>> changtempdic = null;
                int tempid = equ.m_Data.id;
                int itemtype = equ.m_Data.itemtype;
                //数量-1,如果为0 此时equ会被直接删除，所以需要先记录一下数据
                bool issuccess = await AppTools.SendReturn<ItemBaseUI, int, UniTask<bool>>((int)ItemEvent.ToDoItemNum, equ, -1);

                if (issuccess) {//执行成功才会开始计算装备,失败了不用管，ToDoItemNum执行过程中自己会报错
                    switch ((ItemType)itemtype) {
                        case ItemType.yifu:
                            if (cd.Yifu > 0) { //这种情况下，必定是在背包中，需要两个物品交换，一个显示在背包，一个显示在装备栏
                                //先查找背包内有没有物品，没有的话新建一个物品到背包
                                //这个方法里已经加过数量了,而且只有背包满了或者网络错误或者非法数据的时候，才会返回false
                                bool state = await AppTools.SendReturn<ItemDataBase, int, UniTask<bool>>((int)ItemEvent.AddItemUIIfNoneCreateInBag, yifuitem.m_Data, 1);
                                if (state == false) { //背包已满，添加失败
                                    UIManager.GetInstance.OpenUIWidget(AppConfig.UIMsgTips, "背包已满，无法装备,请先清理背包！");
                                    return;
                                }
                                changtempdic = yifuitem.m_Itembase.m_PropXiaoguoDic;
                            }
                            yifuitem.Init(tempid, ItemPos.inEqu);
                            cd.Yifu = (short)tempid;
                            m_EquPosDic[ItemType.yifu] = cd.Yifu;
                            if (!yifuitem.gameObject.activeSelf) { 
                                yifuitem.gameObject.SetActive(true);
                            }
                            break;
                        case ItemType.kuzi:
                            if (cd.Kuzi > 0) { //这种情况下，必定是在背包中，需要两个物品交换，一个显示在背包，一个显示在装备栏
                                //先查找背包内有没有物品，没有的话新建一个物品到背包
                                //这个方法里已经加过数量了,而且只有背包满了或者网络错误或者非法数据的时候，才会返回false
                                bool state = await AppTools.SendReturn<ItemDataBase, int, UniTask<bool>>((int)ItemEvent.AddItemUIIfNoneCreateInBag, kuziitem.m_Data, 1);
                                if (state == false) { //背包已满，添加失败
                                    UIManager.GetInstance.OpenUIWidget(AppConfig.UIMsgTips, "背包已满，无法装备,请先清理背包！");
                                    return;
                                }
                                changtempdic = kuziitem.m_Itembase.m_PropXiaoguoDic;
                            }
                            kuziitem.Init(tempid, ItemPos.inEqu);
                            cd.Kuzi = (short)tempid;
                            m_EquPosDic[ItemType.kuzi] = cd.Kuzi;
                            if (!kuziitem.gameObject.activeSelf) {
                                kuziitem.gameObject.SetActive(true);
                            }
                            break;
                        case ItemType.wuqi:
                            if (cd.Wuqi > 0) { //这种情况下，必定是在背包中，需要两个物品交换，一个显示在背包，一个显示在装备栏
                                //先查找背包内有没有物品，没有的话新建一个物品到背包
                                //这个方法里已经加过数量了,而且只有背包满了或者网络错误或者非法数据的时候，才会返回false
                                bool state = await AppTools.SendReturn<ItemDataBase, int, UniTask<bool>>((int)ItemEvent.AddItemUIIfNoneCreateInBag, wuqiitem.m_Data, 1);
                                if (state == false) { //背包已满，添加失败
                                    UIManager.GetInstance.OpenUIWidget(AppConfig.UIMsgTips, "背包已满，无法装备,请先清理背包！");
                                    return;
                                }
                                changtempdic = wuqiitem.m_Itembase.m_PropXiaoguoDic;
                            } 
                            wuqiitem.Init(tempid, ItemPos.inEqu);
                            cd.Wuqi = (short)tempid;
                            m_EquPosDic[ItemType.wuqi] = cd.Wuqi;
                            if (!wuqiitem.gameObject.activeSelf) {
                                wuqiitem.gameObject.SetActive(true);
                            }
                            break;
                        case ItemType.xianglian:
                            if (cd.Xianglian > 0) { //这种情况下，必定是在背包中，需要两个物品交换，一个显示在背包，一个显示在装备栏
                                //先查找背包内有没有物品，没有的话新建一个物品到背包
                                //这个方法里已经加过数量了,而且只有背包满了或者网络错误或者非法数据的时候，才会返回false
                                bool state = await AppTools.SendReturn<ItemDataBase, int, UniTask<bool>>((int)ItemEvent.AddItemUIIfNoneCreateInBag, xianglianitem.m_Data, 1);
                                if (state == false) { //背包已满，添加失败
                                    UIManager.GetInstance.OpenUIWidget(AppConfig.UIMsgTips, "背包已满，无法装备,请先清理背包！");
                                    return;
                                }
                                changtempdic = xianglianitem.m_Itembase.m_PropXiaoguoDic;
                            }
                            xianglianitem.Init(tempid, ItemPos.inEqu);
                            cd.Xianglian = (short)tempid;
                            m_EquPosDic[ItemType.xianglian] = cd.Xianglian;
                            if (!xianglianitem.gameObject.activeSelf) {
                                xianglianitem.gameObject.SetActive(true);
                            }
                            break;
                        case ItemType.jiezi:
                            if (cd.Jiezi > 0) { //这种情况下，必定是在背包中，需要两个物品交换，一个显示在背包，一个显示在装备栏
                                //先查找背包内有没有物品，没有的话新建一个物品到背包
                                //这个方法里已经加过数量了,而且只有背包满了或者网络错误或者非法数据的时候，才会返回false
                                bool state = await AppTools.SendReturn<ItemDataBase, int, UniTask<bool>>((int)ItemEvent.AddItemUIIfNoneCreateInBag, jieziitem.m_Data, 1);
                                if (state == false) { //背包已满，添加失败
                                    UIManager.GetInstance.OpenUIWidget(AppConfig.UIMsgTips, "背包已满，无法装备,请先清理背包！");
                                    return;
                                }
                                changtempdic = jieziitem.m_Itembase.m_PropXiaoguoDic;
                            }
                            jieziitem.Init(tempid, ItemPos.inEqu);
                            cd.Jiezi = (short)tempid;
                            m_EquPosDic[ItemType.jiezi] = cd.Jiezi;
                            if (!jieziitem.gameObject.activeSelf) {
                                jieziitem.gameObject.SetActive(true);
                            }
                            break;
                        case ItemType.xiezi:
                            if (cd.Xiezi > 0) { //这种情况下，必定是在背包中，需要两个物品交换，一个显示在背包，一个显示在装备栏
                                //先查找背包内有没有物品，没有的话新建一个物品到背包
                                //这个方法里已经加过数量了,而且只有背包满了或者网络错误或者非法数据的时候，才会返回false
                                bool state = await AppTools.SendReturn<ItemDataBase, int, UniTask<bool>>((int)ItemEvent.AddItemUIIfNoneCreateInBag, xieziitem.m_Data, 1);
                                if (state == false) { //背包已满，添加失败
                                    UIManager.GetInstance.OpenUIWidget(AppConfig.UIMsgTips, "背包已满，无法装备,请先清理背包！");
                                    return;
                                }
                                changtempdic = xieziitem.m_Itembase.m_PropXiaoguoDic;
                            }
                            xieziitem.Init(tempid, ItemPos.inEqu);
                            cd.Xiezi = (short)tempid;
                            m_EquPosDic[ItemType.xiezi] = cd.Xiezi;
                            if (!xieziitem.gameObject.activeSelf) {
                                xieziitem.gameObject.SetActive(true);
                            }
                            break;
                        default:
                            Debuger.LogError("卸下的装备类型错误，请检查");
                            break;
                    }
                    //发送服务器计算玩家数据，这里只发送玩家数据，背包变化由另一个模块发送，这里是客户端自己计算完后发送服务端，其实安全考虑，可以改成服务端计算完成后，发给客户端同步，但同步费流量，如果网络差的话，客户体验不太好。
                    //先用两套计算吧，最后加个关键数据核对
                    //其实这里的复杂操作，根本原因是因为角色属性设计问题，这个游戏中，我设计数据库为最终数据存储，所以所有装备导致的属性变化，都会存入数据库，这样取用的时候是方便了，但读取或者更换装备的时候尤其的麻烦，
                    //正常情况应该是数据库只记录玩家基本属性，装备的附加属性，每次读取时，实时计算附加到玩家身上，这样取用虽然麻烦点，但无论是装备数据读取还是变更都简单了不少，而且还减少了数据库读写以及带宽
                    //不想改了，先按照麻烦的来吧，以后开了新坑再改
                    var task = await ClientBase.Instance.Call((ushort)ProtoType.ChangeEqu, timeoutMilliseconds: 0, tempid, itemtype);
                    if (task.IsCompleted) {
                        int code = task.model.AsInt;
                        if (code == 0) {
                            if (changtempdic == null) {//原本没装备
                                UpdateCharacterData(equtempdic, null, cd, fp);
                            } else {//原本有装备
                                UpdateCharacterData(equtempdic, changtempdic, cd, fp);
                            }
                        }
                    } else {
                        UIManager.GetInstance.OpenUIWidget(AppConfig.UIMsgTips, "网络错误，请检查网络连接！");
                        //需要将背包数据恢复回来
                        //这里也是设计问题，应该全部判定结束后，统一发送服务器修改，而不是一步一步来
                        //也能用，先不改了
                        //todo
                    }
                }
            }
        }
        /// <summary>
        /// 卸下某个装备，这里itemui传递的就是卸下的装备
        /// </summary>
        /// <param name="itemui"></param>
        public async UniTask<bool> XiexiaItem(ItemBaseUI itemui) {
            //先查找背包内有没有物品，没有的话新建一个物品到背包
            //这个方法里已经加过数量了,而且只有背包满了的时候，才会返回false
            bool state = await AppTools.SendReturn<ItemDataBase, int, UniTask<bool>>((int)ItemEvent.AddItemUIIfNoneCreateInBag, itemui.m_Data, 1);
            if (state == false) { //背包已满，添加失败
                UIManager.GetInstance.OpenUIWidget(AppConfig.UIMsgTips, "背包已满，无法装备,请先清理背包！");
                return false;
            } else {
                CharactersData cd = UserService.GetInstance.m_CurrentChar;
                switch ((ItemType)itemui.m_Data.itemtype) {
                    case ItemType.yifu:
                        cd.Yifu = -1;
                        break;
                    case ItemType.kuzi:
                        cd.Kuzi = -1;
                        break;
                    case ItemType.wuqi:
                        cd.Wuqi = -1;
                        break;
                    case ItemType.xianglian:
                        cd.Xianglian = -1;
                        break;
                    case ItemType.jiezi:
                        cd.Jiezi = -1;
                        break;
                    case ItemType.xiezi:
                        cd.Xiezi = -1;
                        break;
                    default:
                        Debuger.LogError("装备类型错误，请检查数据表数据！");
                        break;
                }
                UpdateCharacterData(null, itemui.m_Itembase.m_PropXiaoguoDic);
                m_EquPosDic[(ItemType)itemui.m_Data.itemtype] = -1;
                return true;
            }
        }
        
        /// <summary>
        /// 处理玩家数据，m1永远为需要装备的，m2永远为需要卸下的
        /// </summary>
        /// <param name="m1">需要装备的道具的效果字典</param>
        /// <param name="m2">需要卸下的道具效果字典</param>
        private void UpdateCharacterData(Dictionary<XiaoGuo, Dictionary<string, EffArgs>> m1 = null, Dictionary<XiaoGuo, Dictionary<string, EffArgs>> m2 = null, CharactersData cd = null, FightProp fp = null) {
            if (cd == null) {
                cd = UserService.GetInstance.m_CurrentChar;
                fp = UserService.GetInstance.m_CurrentPlayer.FP;
            }
            //减去旧的数据
            if (m2 != null) {
                foreach (KeyValuePair<XiaoGuo, Dictionary<string, EffArgs>> xiaoguo in m2) {
                    if (xiaoguo.Value.Count > 0) {
                        foreach (KeyValuePair<string, EffArgs> xg in xiaoguo.Value) {
                            switch (xg.Key) {
                                case "liliang":
                                    cd.Liliang -= (short)xg.Value.ivalue;
                                    break;
                                case "tizhi":
                                    cd.Tizhi -= (short)xg.Value.ivalue;
                                    break;
                                case "minjie":
                                    cd.Minjie -= (short)xg.Value.ivalue;
                                    break;
                                case "moli":
                                    cd.Moli -= (short)xg.Value.ivalue;
                                    break;
                                case "xingyun":
                                    cd.Xingyun -= (short)xg.Value.ivalue;
                                    break;
                                case "meili":
                                    cd.Meili -= (short)xg.Value.ivalue;
                                    break;
                                case "lianjin":
                                    cd.Lianjin -= (short)xg.Value.ivalue;
                                    break;
                                case "duanzao":
                                    cd.Duanzao -= (short)xg.Value.ivalue;
                                    break;
                            }
                        }
                    }
                }
            }
            //加上新的数据
            if (m1 != null) {
                foreach (KeyValuePair<XiaoGuo, Dictionary<string, EffArgs>> xiaoguo in m1) {
                    if (xiaoguo.Value.Count > 0) {
                        foreach (KeyValuePair<string, EffArgs> xg in xiaoguo.Value) {
                            switch (xg.Key) {
                                case "liliang":
                                    cd.Liliang += (short)xg.Value.ivalue;
                                    break;
                                case "tizhi":
                                    cd.Tizhi += (short)xg.Value.ivalue;
                                    break;
                                case "minjie":
                                    cd.Minjie += (short)xg.Value.ivalue;
                                    break;
                                case "moli":
                                    cd.Moli += (short)xg.Value.ivalue;
                                    break;
                                case "xingyun":
                                    cd.Xingyun += (short)xg.Value.ivalue;
                                    break;
                                case "meili":
                                    cd.Meili += (short)xg.Value.ivalue;
                                    break;
                                
                                case "lianjin":
                                    cd.Lianjin += (short)xg.Value.ivalue;
                                    break;
                                case "duanzao":
                                    cd.Duanzao += (short)xg.Value.ivalue;
                                    break;
                                
                            }
                        }
                    }
                }
            }
            //添加完属性，更新一下运行时属性,装备附加的运行时属性也会在此处添加上
            UserService.GetInstance.m_CurrentPlayer.UpdateFightProps(true);
            //更新数据到ui
            DataToUI();
        }
        
        public override void Close(bool bClear = false, object arg = null) {
             base.Close(bClear, arg);
             m_ModelShow.gameObject.SetActive(false);
             m_CurrentItem = null;
        }
        protected override void OnClose(bool bClear = false, object arg = null) {
            base.OnClose(bClear, arg);
        }
        public void OnDestroy() {
           
        }
    }
}
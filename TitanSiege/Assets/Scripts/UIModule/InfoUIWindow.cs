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
        public Image xuanze;
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
                ShowBtnOrPanel(ItemType.yifu);
            });
            
            kuzibtn.onClick.AddListener(() => {
                ShowBtnOrPanel(ItemType.kuzi);
            });
            wuqibtn.onClick.AddListener(() => {
                ShowBtnOrPanel(ItemType.wuqi);
            });
            xianglianbtn.onClick.AddListener(() => {
                ShowBtnOrPanel(ItemType.xianglian);
            });
            jiezibtn.onClick.AddListener(() => {
                ShowBtnOrPanel(ItemType.jiezi);
            });
            xiezibtn.onClick.AddListener(() => {
                ShowBtnOrPanel(ItemType.xiezi);
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
        public void ShowBtnOrPanel(ItemType itemtype) {
            CharactersData cd = UserService.GetInstance.m_CurrentChar;
            switch (itemtype) {
                case ItemType.yifu:
                    if (cd.Yifu > 0) {//如果装备的id不等于0,那就说明有装备的，此时不应该响应此点击
                        cd.Yifu = -1;
                        //发送服务器，卸下装备，让服务器计算数值后，传递给服务端
                    } else { //显示选择装备界面
                        xuanze.gameObject.SetActive(true);
                        //显示对应类型装备
                        //todo
                    }
                    break;
                case ItemType.kuzi:
                    if (cd.Kuzi > 0) {//如果装备的id不等于0,那就说明有装备的，此时不应该响应此点击
                        return;
                    } else { //显示选择装备界面
                        xuanze.gameObject.SetActive(true);
                        //显示对应类型装备
                        //todo
                    }
                    break;
                case ItemType.wuqi:
                    if (cd.Wuqi > 0) {//如果装备的id不等于0,那就说明有装备的，此时不应该响应此点击
                        return;
                    } else { //显示选择装备界面
                        xuanze.gameObject.SetActive(true);
                        //显示对应类型装备
                        //todo
                    }
                    break;
                case ItemType.xianglian:
                    if (cd.Xianglian > 0) {//如果装备的id不等于0,那就说明有装备的，此时不应该响应此点击
                        return;
                    } else { //显示选择装备界面
                        xuanze.gameObject.SetActive(true);
                        //显示对应类型装备
                        //todo
                    }
                    break;
                case ItemType.jiezi:
                    if (cd.Jiezi > 0) {//如果装备的id不等于0,那就说明有装备的，此时不应该响应此点击
                        return;
                    } else { //显示选择装备界面
                        xuanze.gameObject.SetActive(true);
                        //显示对应类型装备
                        //todo
                    }
                    break;
                case ItemType.xiezi:
                    if (cd.Xiezi > 0) {//如果装备的id不等于0,那就说明有装备的，此时不应该响应此点击
                        return;
                    } else { //显示选择装备界面
                        xuanze.gameObject.SetActive(true);
                        //显示对应类型装备
                        //todo
                    }
                    break;
                default: 
                    break;
            }
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
            xuanze.gameObject.SetActive(false);
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
        public void ChangeEquItem(ItemBaseUI equ) {
            if (equ!=null) {
                CharactersData cd = UserService.GetInstance.m_CurrentChar;
                FightProp fp = UserService.GetInstance.m_CurrentPlayer.FP;
                bool bagisfull = AppTools.SendReturn<bool>((int)ItemEvent.BagIsFull);
                switch ((ItemType)equ.m_Data.itemtype ) {
                    case ItemType.yifu:
                        if (cd.Yifu > 0) { //这种情况下，必定是在背包中，需要两个物品交换，一个显示在背包，一个显示在装备栏
                            //数据处理完毕，开始处理ui显示
                            int tempid = equ.m_Data.id;//先缓存起来，方便交换
                            if (equ.Num > 1) {
                                equ.Num -= 1;
                                //先查找背包内有没有物品，没有的话新建一个物品到背包
                                //这个方法里已经加过数量了,而且只有背包满了的时候，才会返回false
                                bool state = AppTools.SendReturn<ItemDataBase,int,bool>((int)ItemEvent.AddItemUIIfNoneCreateInBag, yifuitem.m_Data,1);
                                if (state == false) { //背包已满，添加失败
                                    UIManager.GetInstance.OpenUIWidget(AppConfig.UIMsgTips, "背包已满，无法装备,请先清理背包！");
                                    return;
                                }
                                UpdateCharacterData(equ, yifuitem, cd, fp);
                            } else { //只有一个,就直接把两个道具交换
                                UpdateCharacterData(equ, yifuitem, cd, fp);
                                equ.Init(yifuitem.m_Data.id, ItemPos.inBag);
                            }
                            cd.Yifu = (short)tempid;
                            m_EquPosDic[ItemType.yifu] = cd.Yifu;
                            yifuitem.Init(tempid, ItemPos.inEqu);
                        } else { //在选择栏或者在背包内装备栏无对应装备
                            UpdateCharacterData(equ, null, cd, fp);
                            yifuitem.Init(equ.m_Data.id, ItemPos.inEqu);
                            cd.Yifu = (short)equ.m_Data.id;
                            m_EquPosDic[ItemType.yifu] = cd.Yifu;
                            if (equ.Num > 1) {
                                equ.Num -= 1;
                            } else { //只有一个,就直接把背包内的道具删除
                                AppTools.Send<ItemBaseUI>((int)ItemEvent.RecycleItemUI, equ);
                            }
                        }
                        //发送服务器计算玩家数据，这里只发送玩家数据，背包变化由另一个模块发送，这里是客户端自己计算完后发送服务端，其实安全考虑，可以改成服务端计算完成后，发给客户端同步，但同步费流量，如果网络差的话，客户体验不太好。
                        //先用两套计算吧，最后加个关键数据核对
                        //todo
                        break;
                    case ItemType.kuzi:
                        if (cd.Kuzi > 0) { //这种情况下，必定是在背包中，需要两个物品交换，一个显示在背包，一个显示在装备栏
                            //数据处理完毕，开始处理ui显示
                            int tempid = equ.m_Data.id;//先缓存起来，方便交换
                            if (equ.Num > 1) {
                                equ.Num -= 1;
                                //先查找背包内有没有物品，没有的话新建一个物品到背包
                                //这个方法里已经加过数量了,而且只有背包满了的时候，才会返回false
                                bool state = AppTools.SendReturn<ItemDataBase, int, bool>((int)ItemEvent.AddItemUIIfNoneCreateInBag, kuziitem.m_Data, 1);
                                if (state == false) { //背包已满，添加失败
                                    UIManager.GetInstance.OpenUIWidget(AppConfig.UIMsgTips, "背包已满，无法装备,请先清理背包！");
                                    return;
                                }
                                UpdateCharacterData(equ, kuziitem, cd, fp);
                            } else { //只有一个,就直接把两个道具交换
                                UpdateCharacterData(equ, kuziitem, cd, fp);
                                equ.Init(kuziitem.m_Data.id, ItemPos.inBag);
                            }
                            cd.Kuzi = (short)tempid;
                            m_EquPosDic[ItemType.kuzi] = cd.Kuzi;
                            kuziitem.Init(tempid, ItemPos.inEqu);
                        } else { //在选择栏或者在背包内装备栏无对应装备
                            UpdateCharacterData(equ, null, cd, fp);
                            kuziitem.Init(equ.m_Data.id, ItemPos.inEqu);
                            cd.Kuzi = (short)equ.m_Data.id;
                            m_EquPosDic[ItemType.kuzi] = cd.Kuzi;
                            if (equ.Num > 1) {
                                equ.Num -= 1;
                            } else { //只有一个,就直接把背包内的道具删除
                                AppTools.Send<ItemBaseUI>((int)ItemEvent.RecycleItemUI, equ);
                            }
                        }
                        //发送服务器计算玩家数据，这里只发送玩家数据，背包变化由另一个模块发送，这里是客户端自己计算完后发送服务端，其实安全考虑，可以改成服务端计算完成后，发给客户端同步，但同步费流量，如果网络差的话，客户体验不太好。
                        //先用两套计算吧，最后加个关键数据核对
                        //todo
                        break;
                    case ItemType.wuqi:
                        if (cd.Wuqi > 0) { //这种情况下，必定是在背包中，需要两个物品交换，一个显示在背包，一个显示在装备栏
                            //数据处理完毕，开始处理ui显示
                            int tempid = equ.m_Data.id;//先缓存起来，方便交换
                            if (equ.Num > 1) {
                                equ.Num -= 1;
                                //先查找背包内有没有物品，没有的话新建一个物品到背包
                                //这个方法里已经加过数量了,而且只有背包满了的时候，才会返回false
                                bool state = AppTools.SendReturn<ItemDataBase, int, bool>((int)ItemEvent.AddItemUIIfNoneCreateInBag, wuqiitem.m_Data, 1);
                                if (state == false) { //背包已满，添加失败
                                    UIManager.GetInstance.OpenUIWidget(AppConfig.UIMsgTips, "背包已满，无法装备,请先清理背包！");
                                    return;
                                }
                                UpdateCharacterData(equ, wuqiitem, cd, fp);
                            } else { //只有一个,就直接把两个道具交换
                                UpdateCharacterData(equ, wuqiitem, cd, fp);
                                equ.Init(wuqiitem.m_Data.id, ItemPos.inBag);
                            }
                            cd.Wuqi = (short)tempid;
                            m_EquPosDic[ItemType.wuqi] = cd.Wuqi;
                            wuqiitem.Init(tempid, ItemPos.inEqu);
                        } else { //在选择栏或者在背包内装备栏无对应装备
                            UpdateCharacterData(equ, null, cd, fp);
                            wuqiitem.Init(equ.m_Data.id, ItemPos.inEqu);
                            cd.Wuqi = (short)equ.m_Data.id;
                            m_EquPosDic[ItemType.wuqi] = cd.Wuqi;
                            if (equ.Num > 1) {
                                equ.Num -= 1;
                            } else { //只有一个,就直接把背包内的道具删除
                                AppTools.Send<ItemBaseUI>((int)ItemEvent.RecycleItemUI, equ);
                            }
                        }
                        //发送服务器计算玩家数据，这里只发送玩家数据，背包变化由另一个模块发送，这里是客户端自己计算完后发送服务端，其实安全考虑，可以改成服务端计算完成后，发给客户端同步，但同步费流量，如果网络差的话，客户体验不太好。
                        //先用两套计算吧，最后加个关键数据核对
                        //todo
                        break;
                    case ItemType.xianglian:
                        if (cd.Xianglian > 0) { //这种情况下，必定是在背包中，需要两个物品交换，一个显示在背包，一个显示在装备栏
                            //数据处理完毕，开始处理ui显示
                            int tempid = equ.m_Data.id;//先缓存起来，方便交换
                            if (equ.Num > 1) {
                                equ.Num -= 1;
                                //先查找背包内有没有物品，没有的话新建一个物品到背包
                                //这个方法里已经加过数量了,而且只有背包满了的时候，才会返回false
                                bool state = AppTools.SendReturn<ItemDataBase, int, bool>((int)ItemEvent.AddItemUIIfNoneCreateInBag, xianglianitem.m_Data, 1);
                                if (state == false) { //背包已满，添加失败
                                    UIManager.GetInstance.OpenUIWidget(AppConfig.UIMsgTips, "背包已满，无法装备,请先清理背包！");
                                    return;
                                }
                                UpdateCharacterData(equ, xianglianitem, cd, fp);
                            } else { //只有一个,就直接把两个道具交换
                                UpdateCharacterData(equ, xianglianitem, cd, fp);
                                equ.Init(xianglianitem.m_Data.id, ItemPos.inBag);
                            }
                            cd.Xianglian = (short)tempid;
                            m_EquPosDic[ItemType.xianglian] = cd.Xianglian;
                            xianglianitem.Init(tempid, ItemPos.inEqu);
                        } else { //在选择栏或者在背包内装备栏无对应装备
                            UpdateCharacterData(equ, null, cd, fp);
                            xianglianitem.Init(equ.m_Data.id, ItemPos.inEqu);
                            cd.Xianglian = (short)equ.m_Data.id;
                            m_EquPosDic[ItemType.xianglian] = cd.Xianglian;
                            if (equ.Num > 1) {
                                equ.Num -= 1;
                            } else { //只有一个,就直接把背包内的道具删除
                                AppTools.Send<ItemBaseUI>((int)ItemEvent.RecycleItemUI, equ);
                            }
                        }
                        //发送服务器计算玩家数据，这里只发送玩家数据，背包变化由另一个模块发送，这里是客户端自己计算完后发送服务端，其实安全考虑，可以改成服务端计算完成后，发给客户端同步，但同步费流量，如果网络差的话，客户体验不太好。
                        //先用两套计算吧，最后加个关键数据核对
                        //todo
                        break;
                    case ItemType.jiezi:
                        if (cd.Jiezi > 0) { //这种情况下，必定是在背包中，需要两个物品交换，一个显示在背包，一个显示在装备栏
                            //数据处理完毕，开始处理ui显示
                            int tempid = equ.m_Data.id;//先缓存起来，方便交换
                            if (equ.Num > 1) {
                                equ.Num -= 1;
                                //先查找背包内有没有物品，没有的话新建一个物品到背包
                                //这个方法里已经加过数量了,而且只有背包满了的时候，才会返回false
                                bool state = AppTools.SendReturn<ItemDataBase, int, bool>((int)ItemEvent.AddItemUIIfNoneCreateInBag, jieziitem.m_Data, 1);
                                if (state == false) { //背包已满，添加失败
                                    UIManager.GetInstance.OpenUIWidget(AppConfig.UIMsgTips, "背包已满，无法装备,请先清理背包！");
                                    return;
                                }
                                UpdateCharacterData(equ, jieziitem, cd, fp);
                            } else { //只有一个,就直接把两个道具交换
                                UpdateCharacterData(equ, jieziitem, cd, fp);
                                equ.Init(jieziitem.m_Data.id, ItemPos.inBag);
                            }
                            cd.Jiezi = (short)tempid;
                            m_EquPosDic[ItemType.jiezi] = cd.Jiezi;
                            jieziitem.Init(tempid, ItemPos.inEqu);
                        } else { //在选择栏或者在背包内装备栏无对应装备
                            UpdateCharacterData(equ, null, cd, fp);
                            jieziitem.Init(equ.m_Data.id, ItemPos.inEqu);
                            cd.Jiezi = (short)equ.m_Data.id;
                            m_EquPosDic[ItemType.jiezi] = cd.Jiezi;
                            if (equ.Num > 1) {
                                equ.Num -= 1;
                            } else { //只有一个,就直接把背包内的道具删除
                                AppTools.Send<ItemBaseUI>((int)ItemEvent.RecycleItemUI, equ);
                            }
                        }
                        //发送服务器计算玩家数据，这里只发送玩家数据，背包变化由另一个模块发送，这里是客户端自己计算完后发送服务端，其实安全考虑，可以改成服务端计算完成后，发给客户端同步，但同步费流量，如果网络差的话，客户体验不太好。
                        //先用两套计算吧，最后加个关键数据核对
                        //todo
                        break;
                    case ItemType.xiezi:
                        if (cd.Xiezi > 0) { //这种情况下，必定是在背包中，需要两个物品交换，一个显示在背包，一个显示在装备栏
                            //数据处理完毕，开始处理ui显示
                            int tempid = equ.m_Data.id;//先缓存起来，方便交换
                            if (equ.Num > 1) {
                                equ.Num -= 1;
                                //先查找背包内有没有物品，没有的话新建一个物品到背包
                                //这个方法里已经加过数量了,而且只有背包满了的时候，才会返回false
                                bool state = AppTools.SendReturn<ItemDataBase, int, bool>((int)ItemEvent.AddItemUIIfNoneCreateInBag, xieziitem.m_Data, 1);
                                if (state == false) { //背包已满，添加失败
                                    UIManager.GetInstance.OpenUIWidget(AppConfig.UIMsgTips, "背包已满，无法装备,请先清理背包！");
                                    return;
                                }
                                UpdateCharacterData(equ, xieziitem, cd, fp);
                            } else { //只有一个,就直接把两个道具交换
                                UpdateCharacterData(equ, xieziitem, cd, fp);
                                equ.Init(xieziitem.m_Data.id, ItemPos.inBag);
                            }
                            cd.Xiezi = (short)tempid;
                            m_EquPosDic[ItemType.xiezi] = cd.Xiezi;
                            xieziitem.Init(tempid, ItemPos.inEqu);
                        } else { //在选择栏或者在背包内装备栏无对应装备
                            UpdateCharacterData(equ, null, cd, fp);
                            xieziitem.Init(equ.m_Data.id, ItemPos.inEqu);
                            cd.Xiezi = (short)equ.m_Data.id;
                            m_EquPosDic[ItemType.xiezi] = cd.Xiezi;
                            if (equ.Num > 1) {
                                equ.Num -= 1;
                            } else { //只有一个,就直接把背包内的道具删除
                                AppTools.Send<ItemBaseUI>((int)ItemEvent.RecycleItemUI, equ);
                            }
                        }
                        //发送服务器计算玩家数据，这里只发送玩家数据，背包变化由另一个模块发送，这里是客户端自己计算完后发送服务端，其实安全考虑，可以改成服务端计算完成后，发给客户端同步，但同步费流量，如果网络差的话，客户体验不太好。
                        //先用两套计算吧，最后加个关键数据核对
                        //todo
                        break;
                    default:
                        Debuger.LogError("卸下的装备类型错误，请检查");
                        break;
                }
            }
        }
        /// <summary>
        /// 卸下某个装备，这里itemui传递的就是卸下的装备
        /// </summary>
        /// <param name="itemui"></param>
        public bool XiexiaItem(ItemBaseUI itemui) {
            //先查找背包内有没有物品，没有的话新建一个物品到背包
            //这个方法里已经加过数量了,而且只有背包满了的时候，才会返回false
            bool state = AppTools.SendReturn<ItemDataBase, int, bool>((int)ItemEvent.AddItemUIIfNoneCreateInBag, itemui.m_Data, 1);
            if (state == false) { //背包已满，添加失败
                UIManager.GetInstance.OpenUIWidget(AppConfig.UIMsgTips, "背包已满，无法装备,请先清理背包！");
                return false;
            } else {
                UpdateCharacterData(null, itemui);
                m_EquPosDic[(ItemType)itemui.m_Data.itemtype] = -1;
                return true;
            }
        }
        /// <summary>
        /// 处理玩家数据，m1永远为需要装备的，m2永远为需要卸下的
        /// </summary>
        /// <param name="m1">需要装备的道具</param>
        /// <param name="m2">需要卸下的道具</param>
        private void UpdateCharacterData(ItemBaseUI m1=null, ItemBaseUI m2 = null, CharactersData cd = null, FightProp fp = null) {
            if (cd == null) {
                cd = UserService.GetInstance.m_CurrentChar;
                fp = UserService.GetInstance.m_CurrentPlayer.FP;
            }
            //减去旧的数据
            if (m2 != null) {
                if (m2.m_PropXiaoguoDic.Count > 0) {//有属性类的需求
                    foreach (KeyValuePair<XiaoGuo, Dictionary<string, EffArgs>> xiaoguo in m2.m_PropXiaoguoDic) {
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
                                    case "maxhp":
                                        fp.FightMaxHp -= (short)xg.Value.ivalue;
                                        fp.FightHP -= (short)xg.Value.ivalue;
                                        if (fp.FightHP<=0) {
                                            fp.FightHP = 1;
                                        }
                                        break;
                                    case "maxmagic":
                                        fp.FightMaxMagic -= (short)xg.Value.ivalue;
                                        fp.FightMagic -= (short)xg.Value.ivalue;
                                        if (fp.FightMagic < 0) {
                                            fp.FightMagic = 0;
                                        }
                                        break;
                                    case "gongji":
                                        fp.Attack -= (short)xg.Value.ivalue;
                                        break;
                                    case "fangyu":
                                        fp.Defense -= (short)xg.Value.ivalue;
                                        break;
                                    case "shanbi":
                                        fp.Dodge -= xg.Value.fvalue;
                                        break;
                                    case "baoji":
                                        fp.Crit -= xg.Value.fvalue;
                                        break;
                                    case "lianjin":
                                        cd.Lianjin -= (short)xg.Value.ivalue;
                                        break;
                                    case "duanzao":
                                        cd.Duanzao -= (short)xg.Value.ivalue;
                                        break;
                                    default:
                                        Debuger.LogError($"未知属性{xg.Key}，无法匹配计算，请检查数据表");
                                        break;
                                }
                            }
                        }
                    }
                }
            }
            //加上新的数据
            if (m1 != null) {
                Debuger.Log(m1.m_PropXiaoguoDic.Count);
                if (m1.m_PropXiaoguoDic.Count > 0) {//有属性类的需求
                    foreach (KeyValuePair<XiaoGuo, Dictionary<string, EffArgs>> xiaoguo in m1.m_PropXiaoguoDic) {
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
                                    case "maxhp":
                                        fp.FightMaxHp += (short)xg.Value.ivalue;
                                        fp.FightHP += (short)xg.Value.ivalue;
                                        break;
                                    case "maxmagic":
                                        fp.FightMaxMagic += (short)xg.Value.ivalue;
                                        fp.FightMagic += (short)xg.Value.ivalue;
                                        break;
                                    case "gongji":
                                        fp.Attack += (short)xg.Value.ivalue;
                                        break;
                                    case "fangyu":
                                        fp.Defense += (short)xg.Value.ivalue;
                                        break;
                                    case "shanbi":
                                        fp.Dodge += xg.Value.fvalue;
                                        break;
                                    case "baoji":
                                        fp.Crit += xg.Value.fvalue;
                                        break;
                                    case "lianjin":
                                        cd.Lianjin += (short)xg.Value.ivalue;
                                        break;
                                    case "duanzao":
                                        cd.Duanzao += (short)xg.Value.ivalue;
                                        break;
                                    default:
                                        Debuger.LogError($"未知属性{xg.Key}，无法匹配计算，请检查数据表");
                                        break;
                                }
                            }
                        }
                    }
                }
            }
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
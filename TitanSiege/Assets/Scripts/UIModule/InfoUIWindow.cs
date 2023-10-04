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
        public Button xiexiabtn;
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
        
        public void Start() {
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
            xiexiabtn.onClick.AddListener(() => {
                ClickXiexia();
            });
            AppTools.Regist<ItemType>((int)MainUIEvent.ShowXiexia, ShowXiexia);
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
            m_ZhandouliTxt.text = (p.FP.Attack/2+ p.FP.Defense+ p.FP.Dodge*5000+ p.FP.Crit*2000 + p.FP.FightMaxHp/4 + p.FP.FightMagic/6).ToString();
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
                    jieziitem.Init(cd.Kuzi, ItemPos.inEqu);
                }
                jieziitem.gameObject.SetActive(true);
            } else {//有可能会等于0
                jieziitem.gameObject.SetActive(false);
            } 
            //鞋子装备栏
            if (cd.Xiezi > 0) {
                if (m_EquPosDic[ItemType.xiezi] != cd.Xiezi) {//避免重复初始化
                    m_EquPosDic[ItemType.xiezi] = cd.Xiezi;
                    xieziitem.Init(cd.Kuzi, ItemPos.inEqu);
                }
                xieziitem.gameObject.SetActive(true);
            } else {//有可能会等于0
                xieziitem.gameObject.SetActive(false);
            }
            #endregion
            //隐藏非必要内容
            xuanze.gameObject.SetActive(false);
            xiexiabtn.gameObject.SetActive(false);
        }
        /// <summary>
        /// 点击装备栏装备的处理
        /// </summary>
        /// <param name="pos">点击装备所在的位置</param>
        public void ShowXiexia(ItemType tp) {
            Vector3 pos = Vector3.one;
            switch (tp) {
                case ItemType.yifu:
                    pos = yifuitem.transform.position;
                    m_CurrentItem = yifuitem;
                    break;
                case ItemType.kuzi:
                    pos = kuziitem.transform.position;
                    m_CurrentItem = kuziitem;
                    break;
                case ItemType.wuqi:
                    pos = wuqiitem.transform.position;
                    m_CurrentItem = wuqiitem;
                    break;
                case ItemType.xianglian:
                    pos = xianglianitem.transform.position;
                    m_CurrentItem = xianglianitem;
                    break;
                case ItemType.jiezi:
                    pos = jieziitem.transform.position;
                    m_CurrentItem = jieziitem;
                    break;
                case ItemType.xiezi:
                    pos = xieziitem.transform.position;
                    m_CurrentItem = xieziitem;
                    break;
                default:
                    m_CurrentItem = null;
                    Debuger.LogError("装备类型错误，请检查");
                    break;
            }
            xiexiabtn.transform.position = pos;
            xiexiabtn.transform.DOMoveX(97.4f,2f);
            xiexiabtn.gameObject.SetActive(true);
        }
        /// <summary>
        /// 装备卸下按钮的处理
        /// </summary>
        public void ClickXiexia() {
            if (m_CurrentItem != null) {
                m_CurrentItem.gameObject.SetActive(false);
            } else {
                Debuger.LogError("显示错误，m_CurrentItem为空，请检查代码");
            }
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
            AppTools.Remove<ItemType>((int)MainUIEvent.ShowXiexia, ShowXiexia);
        }
    }
}
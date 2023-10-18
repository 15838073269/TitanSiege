/****************************************************
    文件：ItemDescUIWidget.cs
	作者：昔莘
    邮箱: 304183153@qq.com
    日期：2023/10/2 10:56:53
	功能：Nothing
*****************************************************/

using cmd;
using Cysharp.Threading.Tasks;
using GF.MainGame.Data;
using GF.MainGame.Module;
using GF.Service;
using GF.Unity.AB;
using GF.Unity.UI;
using Net.Client;
using Net.Component;
using Net.Share;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Titansiege;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.PlayerSettings;
using static UnityEditor.Progress;

namespace GF.MainGame.UI {
    public class ItemDescUIWidget : UIWidget {
        public Transform m_BtnFather;
        /// <summary>
        /// 主道具父节点
        /// </summary>
        public RawImage m_Desc;
        public Image m_ItemColor;
        public Image m_ItemImg;
        public Text m_ItemName;
        public Text m_ItemType;
        public Text m_Pinzhi;
        public Text m_ItemLevel;
        public Text m_Miaoshu;
        public Text m_Shuxing;
        public Text m_Xuqiu;
        /// <summary>
        /// 对比的道具父节点
        /// </summary>
        public RawImage m_CompareDesc;
        public Image m_CItemColor;
        public Image m_CItemImg;
        public Text m_CItemName;
        public Text m_CItemType;
        public Text m_CPinzhi;
        public Text m_CItemLevel;
        public Text m_CMiaoshu;
        public Text m_CShuxing;
        public Text m_CXuqiu;

        public Button m_UseBtn;//使用按钮
        public Button m_EquBtn;//装备按钮
        public Button m_DeleBtn;//删除按钮
        public Button m_XiexiaBtn;//卸下按钮
        public Button m_CloseBtn;//卸下按钮

        private Vector3 m_OldPos;//默认的详情栏位置，默认时，是详情和对比栏都存在的 
        private Vector3 m_OldBtnPos;
        private ItemBaseUI m_CurrenItem;
        /// <summary>
        /// 定义部分颜色，不同品级的物品，名称颜色跟着变
        /// </summary>
        public Color m_Bai;
        public Color m_Lv;
        public Color m_lan;
        public Color m_Zi;
        public Color m_Cheng;

        CharactersData cd;//玩家数据，方便使用
        private void Awake() {
            m_OldPos = m_Desc.transform.localPosition;
            m_OldBtnPos = m_BtnFather.transform.localPosition;
            m_CloseBtn.onClick.AddListener(()=>{
                Close();
            });
            m_EquBtn.onClick.AddListener(OnZhuangbei);
            m_XiexiaBtn.onClick.AddListener(OnXiexia);
            m_DeleBtn.onClick.AddListener(OnDiuqi);
            m_UseBtn.onClick.AddListener(OnUse);
            m_Bai = new Color(0.79f, 0.77f, 0.77f);
            m_Lv = new Color(0.22f, 0.83f, 0.16f);
            m_lan = new Color(0f, 0.44f, 1f);
            m_Zi = new Color(0.6f, 0.13f, 1f);
            m_Cheng = new Color(1f, 0.41f, 0f);
            cd = UserService.GetInstance.m_CurrentChar;
        }

        protected override void OnOpen(object args = null) {
            base.OnOpen(args);
            ItemBaseUI itemui = args as ItemBaseUI;
            if (itemui != null) {
                m_CurrenItem = itemui;
                bool isequ = false;
                //处理对比装备详情
                if (itemui.m_Pos == ItemPos.inBag) {//在背包中
                    //如果是装备，如果有已装备，就需要显示已装备项，
                    switch (itemui.m_Data.itemtype) {
                        case (int)ItemType.yifu:
                            InitCompareDesc(cd.Yifu);
                            isequ = true;
                            break;
                        case (int)ItemType.kuzi:
                            InitCompareDesc(cd.Kuzi);
                            isequ = true;
                            break;
                        case (int)ItemType.xianglian:
                            InitCompareDesc(cd.Xianglian);
                            isequ = true;
                            break;
                        case (int)ItemType.wuqi:
                            InitCompareDesc(cd.Wuqi);
                            isequ = true;
                            break;
                        case (int)ItemType.jiezi:
                            InitCompareDesc(cd.Jiezi);
                            isequ = true;
                            break;
                        case (int)ItemType.xiezi:
                            InitCompareDesc(cd.Xiezi);
                            isequ = true;
                            break;
                        default:
                            InitCompareDesc(-1);
                            isequ = false;
                            break;
                    }
                } else if (itemui.m_Pos == ItemPos.inSelect) { //在选择栏中，只有没装备时才会打开选择栏，所以这里必定不会出现对比栏
                    InitCompareDesc(-1);
                    isequ = true;//选择栏上只有装备

                } else if (itemui.m_Pos == ItemPos.inEqu) {
                    InitCompareDesc(-1);
                    isequ = true;//装备栏上只有装备
                }
                //加载道具界面
                InitDesc(itemui,isequ);
            }  
        }
        /// <summary>
        /// 将数据写入对比详情栏
        /// </summary>
        /// <param name="itemid">参数为-1，意味着不需要对比栏</param>
        private void InitCompareDesc(int itemid,ItemPos itemPos = ItemPos.inBag) {
            if (itemid == -1) { //说明不是装备，或者装备栏为空，不需要显示对比栏,恢复原状
                if (m_Desc.transform.localPosition == m_OldPos) { //如果在默认位置（默认有对比栏）,就需要移动隐藏对比栏
                    m_Desc.transform.localPosition = new Vector3(m_OldPos.x-300f, m_OldPos.y, m_OldPos.z);
                    m_BtnFather.transform.localPosition = new Vector3(m_OldBtnPos.x-300f,m_OldBtnPos.y,m_OldBtnPos.z);
                    m_CompareDesc.gameObject.SetActive(false);
                }
                return;
            } else {
                if (m_Desc.transform.localPosition != m_OldPos) { //如果在默认位置（默认有对比栏）,就需要移动隐藏对比栏
                    m_Desc.transform.localPosition = m_OldPos;
                    m_BtnFather.transform.localPosition = m_OldBtnPos;
                    m_CompareDesc.gameObject.SetActive(true);
                }
                if (itemPos == ItemPos.inBag) {
                    m_UseBtn.gameObject.SetActive(false);
                    m_EquBtn.gameObject.SetActive(true);
                    m_DeleBtn.gameObject.SetActive(true);
                    m_XiexiaBtn.gameObject.SetActive(false);
                } else if (itemPos == ItemPos.inSelect) {//这里不会进入
                    //m_UseBtn.gameObject.SetActive(false);
                    //m_EquBtn.gameObject.SetActive(true);
                    //m_DeleBtn.gameObject.SetActive(false);
                    //m_XiexiaBtn.gameObject.SetActive(false);
                }
                ItemModule mod = AppTools.GetModule<ItemModule>(MDef.ItemModule);
                ItemDataBase data = mod.m_Data.FindItemByID(itemid);
                //开始写入数据到对比栏
                m_CItemColor.sprite = mod.m_PinzhiDic[data.pinzhi];
                //打开背包一瞬间可能同一时间加载大量图片，所以选择异步加载
                //这里没必要缓存，因为内部加载已经做了缓存了
                ResourceManager.GetInstance.AsyncLoadResource(data.pic, OnLoadSpriteOver, LoadResPriority.RES_HIGHT, bclear: false);
                m_CItemLevel.text = $"{data.level}级";
                m_CItemName.text = data.name;
                m_CItemType.text = GetItemTypeName((ItemType)data.itemtype);
                m_CMiaoshu.text = data.desc;
                m_CShuxing.text = PropToStr(data);
                m_CPinzhi.text = GetPinzhiText((Pinzhi)data.pinzhi,true);
                m_CXuqiu.text = XuqiuToStr(data);
            }
        }
        /// <summary>
        /// 异步资源加载完成后的回调
        /// </summary>
        /// <param name="path"></param>
        /// <param name="obj"></param>
        /// <param name="objarr"></param>
        public void OnLoadSpriteOver(string path, UnityEngine.Object obj, params object[] objarr) {
            if (obj != null) {
                Texture2D t2d = (obj as Texture2D);
                m_CItemImg.sprite = Sprite.Create(t2d, new Rect(0, 0, t2d.width, t2d.height), Vector2.zero);
            }
        }
        /// <summary>
        /// 将数据写入详情栏
        /// </summary>
        /// <param name="ui"></param>
        private void InitDesc(ItemBaseUI ui,bool isequ = false) {
            if (ui.m_Pos == ItemPos.inBag) {//在背包中
                if (isequ) {//如果是装备
                    m_UseBtn.gameObject.SetActive(false);
                    m_EquBtn.gameObject.SetActive(true);
                } else {
                    m_UseBtn.gameObject.SetActive(true);
                    m_EquBtn.gameObject.SetActive(false);
                }
                m_DeleBtn.gameObject.SetActive(true);
                m_XiexiaBtn.gameObject.SetActive(false);
            } else if (ui.m_Pos == ItemPos.inSelect) {//选择栏是给选择装备使用的，所以非装备，不会进入
                m_UseBtn.gameObject.SetActive(true);
                m_EquBtn.gameObject.SetActive(false);
                m_DeleBtn.gameObject.SetActive(false);
                m_XiexiaBtn.gameObject.SetActive(false);
            }else if (ui.m_Pos == ItemPos.inEqu) { //在装备栏上,只显示卸下和关闭
                m_UseBtn.gameObject.SetActive(false);
                m_EquBtn.gameObject.SetActive(false);
                m_DeleBtn.gameObject.SetActive(false);
                m_XiexiaBtn.gameObject.SetActive(true);
                //m_CloseBtn.gameObject.SetActive(true);  //关闭按钮永远不会隐藏
            }
            ItemDataBase data = ui.m_Data;
            //开始写入数据到对比栏
            m_ItemColor.sprite = ui.m_ItemColor.sprite;
            //打开背包一瞬间可能同一时间加载大量图片，所以选择异步加载
            //这里没必要缓存，因为内部加载已经做了缓存了
            m_ItemImg.sprite = ui.m_ItemBtn.image.sprite;
            m_ItemLevel.text = $"{data.level}级";
            m_ItemName.text = data.name;
            m_ItemType.text = GetItemTypeName((ItemType)data.itemtype);
            m_Miaoshu.text = data.desc;
            m_Shuxing.text = PropToStr(data);
            m_Pinzhi.text = GetPinzhiText((Pinzhi)data.pinzhi);
            m_Xuqiu.text = XuqiuToStr(data);
        }
        #region 道具效果转字符串
        /// <summary>
        /// 道具效果转成字符串显示
        /// </summary>
        /// <returns></returns>
        private string PropToStr(ItemDataBase data) {
            StringBuilder str = new StringBuilder();
            if (data.xiaoguo1 > 0) { //无效果时，默认为0
                str.Append(XiaoguoTostr(data.xiaoguo1, data.xiaoguo1zhi));
            }
            if (data.xiaoguo2 > 0) {
                str.Append(XiaoguoTostr(data.xiaoguo2, data.xiaoguo2zhi));
            }
            if (data.xiaoguo3 > 0) {
                str.Append(XiaoguoTostr(data.xiaoguo3, data.xiaoguo3zhi));
            }
            //最后处理一下逗号
            string finstr = str.ToString();
            if (string.IsNullOrEmpty(finstr)) {
                finstr = "无";
            } else {
                if (finstr.EndsWith("，")) { //判断一下最后是否为逗号
                    finstr = finstr.Substring(0, finstr.Length - 1);
                }
            }
            return finstr;
        }
        /// <summary>
        /// 根据效果的类型，用不同的方式处理效果值
        /// </summary>
        /// <param name="xiaoguo"></param>
        /// <param name="xiaoguozhi"></param>
        /// <returns></returns>
        private string XiaoguoTostr(int xiaoguo,string xiaoguozhi) {
            StringBuilder str = new StringBuilder();
            switch ((XiaoGuo)xiaoguo) {
                case XiaoGuo.addhp:
                    int num = int.Parse(xiaoguozhi);
                    if (num > 0) {
                        str.Append($"回复生命+{num}");
                    } else {
                        str.Append($"减少生命{num}");
                    }
                    break;
                case XiaoGuo.addmp:
                    int num1 = int.Parse(xiaoguozhi);
                    if (num1 > 0) {
                        str.Append($"回复法力+{num1}");
                    } else {
                        str.Append($"减少法力{num1}");
                    }
                    break;
                case XiaoGuo.addfightprop:
                case XiaoGuo.addprop:
                    str.Append(GetPropStr(xiaoguozhi));
                    break;
                case XiaoGuo.addskill:
                    break;
                case XiaoGuo.addjinbi:
                    int num2 = int.Parse(xiaoguozhi);
                    if (num2 > 0) {
                        str.Append($"金币+{num2}");
                    } else {
                        str.Append($"金币{num2}");
                    }
                    break;
                case XiaoGuo.addzhuanshi:
                    int num3 = int.Parse(xiaoguozhi);
                    if (num3 > 0) {
                        str.Append($"钻石+{num3}");
                    } else {
                        str.Append($"钻石{num3}");
                    }
                    break;
                case XiaoGuo.additem:
                    break;
                case XiaoGuo.addexp:
                    int num4 = int.Parse(xiaoguozhi);
                    if (num4 > 0) {
                        str.Append($"经验+{num4}");
                    } else {
                        str.Append($"经验{num4}");
                    }
                    break;
            }
            return str.ToString();
        }
        /// <summary>
        /// 将效果值中的addfightprop和addprop转化为字符串
        /// </summary>
        /// <param name="propstr"></param>
        /// <returns></returns>
        private string GetPropStr(string propstr,bool isxuqiu = false) {
            if (propstr =="0") { //效果值为空时默认为“0”
                return "";
            }
            StringBuilder str = new StringBuilder();
            if (propstr.Contains(",")) {
                string[] strarr = propstr.Split(',');
                if (strarr.Length > 0) {
                    for (int i = 0; i < strarr.Length; i++) {
                        if (!string.IsNullOrEmpty(strarr[i])) {//判断一下空白，防止配表的多写
                            string[] strarr1 = strarr[i].Split('|');
                            if (isxuqiu) {
                                str.Append($"{GetPropName(strarr1[0])}要求");
                            } else {
                                str.Append(GetPropName(strarr1[0]));
                            }
                            if (strarr1[0] == "baoji" || strarr1[0] == "shanbi") {
                                float num = float.Parse(strarr1[1]);
                                if (isxuqiu) {
                                    str.Append($"{num.ToString("P1")}，");
                                } else {
                                    if (num > 0) {
                                        str.Append($"+{num.ToString("P1")}，");//用中文逗号，好看些
                                    } else {
                                        str.Append($"{num.ToString("P1")}，");
                                    }
                                }
                            } else {
                                int num = int.Parse(strarr1[1]);
                                if (isxuqiu) {
                                    str.Append($"{num}，");
                                } else {
                                    if (num > 0) {
                                        str.Append($"+{num}，");//用中文逗号，好看些
                                    } else {
                                        str.Append($"{num}，");
                                    }
                                }
                              
                            }
                        }
                    }
                }
            } else { //前面已经判断过空了，所以这里只可能时只有一个元素的情况
                string[] strarr = propstr.Split('|');
                if (isxuqiu) {
                    str.Append($"{GetPropName(strarr[0])}要求");
                } else {
                    str.Append(GetPropName(strarr[0]));
                }
                if (strarr[0] == "baoji" || strarr[0] == "shanbi") {
                    float num = float.Parse(strarr[1]);
                    if (isxuqiu) {
                        str.Append($"{num.ToString("P1")}，");
                    } else {
                        if (num > 0) {
                            str.Append($"+{num.ToString("P1")}，");//用中文逗号，好看些
                        } else {
                            str.Append($"{num.ToString("P1")}，");
                        }
                    }
                } else {
                    int num = int.Parse(strarr[1]);
                    if (isxuqiu) {
                        str.Append($"{num}，");
                    } else {
                        if (num > 0) {
                            str.Append($"+{num}，");//用中文逗号，好看些
                        } else {
                            str.Append($"{num}，");
                        }
                    }
                }
            }
            return str.ToString();
        }
        #endregion
        #region 道具需求转字符串
        /// <summary>
        /// 道具效果转成字符串显示
        /// </summary>
        /// <returns></returns>
        private string XuqiuToStr(ItemDataBase data) {
            StringBuilder str = new StringBuilder();
            if (data.xuqiu1 > 0) { //无效果时，默认为0
                str.Append(GetXuqiuToStr(data.xuqiu1, data.xuqiu1zhi));
            }
            if (data.xuqiu2 > 0) {
                str.Append(GetXuqiuToStr(data.xuqiu2, data.xuqiu2zhi));
            }
            if (data.xuqiu3 > 0) {
                str.Append(GetXuqiuToStr(data.xuqiu3, data.xuqiu3zhi));
            }
            //最后处理一下逗号
            string finstr = str.ToString();
            if (string.IsNullOrEmpty(finstr)) {
                finstr = "无";
            } else {
                if (finstr.EndsWith("，")) { //判断一下最后是否为逗号
                    finstr = finstr.Substring(0, finstr.Length - 1);
                }
            }
            return finstr;
        }
        /// <summary>
        /// 根据效果的类型，用不同的方式处理效果值
        /// </summary>
        /// <param name="xiaoguo"></param>
        /// <param name="xiaoguozhi"></param>
        /// <returns></returns>
        private string GetXuqiuToStr(int xuqiu, string xuqiuzhi) {
            StringBuilder str = new StringBuilder();
            switch ((XuQiu)xuqiu) {
                case XuQiu.level:
                    int num = int.Parse(xuqiuzhi);
                    str.Append($"等级要求{num}级，");
                    break;
                case XuQiu.zhiye:
                    Zhiye zy = (Zhiye)int.Parse(xuqiuzhi);
                    str.Append($"职业要求{zy.ToString()}，");
                    break;
                case XuQiu.fightprop:
                case XuQiu.prop:
                    str.Append(GetPropStr(xuqiuzhi, true));
                    break;
            }
            return str.ToString();
        }
        #endregion
        /// <summary>
        /// 匹配战斗数据和基础属性的名称
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        private string GetPropName(string s) {
            string str = "";
            switch (s) {
                case "liliang":
                    str = "力量";
                    break;
                case "tizhi":
                    str = "体质";
                    break;
                case "minjie":
                    str = "敏捷";
                    break;
                case "moli":
                    str = "魔力";
                    break;
                case "xingyun":
                    str = "幸运";
                    break;
                case "meili":
                    str = "魅力";
                    break;
                case "maxhp":
                    str = "最大生命";
                    break;
                case "maxmagic":
                    str = "最大法力";
                    break;
                case "gongji":
                    str = "攻击";
                    break;
                case "fangyu":
                    str = "防御";
                    break;
                case "shanbi":
                    str = "闪避";
                    break;
                case "baoji":
                    str = "暴击";
                    break;
                case "lianjin":
                    str = "炼金";
                    break;
                case "duanzao":
                    str = "锻造";
                    break;
                default:
                    str = "未知属性";
                    break;
            }
            return str;
        }
        /// <summary>
        /// 通过类型获取名称
        /// </summary>
        /// <param name="itemtype"></param>
        /// <returns></returns>
        private string GetItemTypeName(ItemType itemtype) {
            string str = "";
            switch (itemtype) {
                case ItemType.yaopin:
                    str = "药品";
                    break;
                case ItemType.yifu:
                    str = "衣服";
                    break;
                case ItemType.kuzi:
                    str = "下装";
                    break;
                case ItemType.wuqi:
                    str = "武器";
                    break;
                case ItemType.xianglian:
                    str = "项链";
                    break;
                case ItemType.jiezi:
                    str = "戒指";
                    break;
                case ItemType.xiezi:
                    str = "鞋子";
                    break;
                case ItemType.jinengshu:
                    str = "技能书";
                    break;
                case ItemType.renwu:
                    str = "任务物品";
                    break;
                case ItemType.zawu:
                    str = "杂物";
                    break;
                default:
                    str = "未知";
                    break;
            }
            return str;
        }
        private string GetPinzhiText(Pinzhi p, bool iscompare = false) {
            string s = "";
            switch (p) {
                case Pinzhi.bai:
                    s = "普通物品";
                    if (iscompare) {
                        if (m_CItemName.color != m_Bai) {
                            m_CItemName.color = m_Bai;
                            m_CPinzhi.color = m_Bai;
                        }
                    } else {
                        if (m_ItemName.color != m_Bai) {
                            m_ItemName.color = m_Bai;
                            m_Pinzhi.color = m_Bai;
                        }
                    }
                    break;
                case Pinzhi.lv:
                    s = "精良物品";
                    if (iscompare) {
                        if (m_CItemName.color != m_Lv) {
                            m_CItemName.color = m_Lv;
                            m_CPinzhi.color = m_Lv;
                        }
                    } else {
                        if (m_ItemName.color != m_Lv) {
                            m_ItemName.color = m_Lv;
                            m_Pinzhi.color = m_Lv;
                        }
                    }
                    break;
                case Pinzhi.lan:
                    s = "卓越物品";
                    if (iscompare) {
                        if (m_CItemName.color != m_lan) {
                            m_CItemName.color = m_lan;
                            m_CPinzhi.color = m_lan;
                        }
                    } else {
                        if (m_ItemName.color != m_lan) {
                            m_ItemName.color = m_lan;
                            m_Pinzhi.color = m_lan;
                        }
                    }
                    break;
                case Pinzhi.zi:
                    s = "传奇物品";
                    if (iscompare) {
                        if (m_CItemName.color != m_Zi) {
                            m_CItemName.color = m_Zi;
                            m_CPinzhi.color = m_Zi;
                        }
                    } else {
                        if (m_ItemName.color != m_Zi) {
                            m_ItemName.color = m_Zi;
                            m_Pinzhi.color = m_Zi;
                        }
                    }
                    break;
                case Pinzhi.cheng:
                    s = "史诗物品";
                    if (iscompare) {
                        if (m_CItemName.color != m_Cheng) {
                            m_CItemName.color = m_Cheng;
                            m_CPinzhi.color = m_Cheng;
                        }
                    } else {
                        if (m_ItemName.color != m_Cheng) {
                            m_ItemName.color = m_Cheng;
                            m_Pinzhi.color = m_Cheng;
                        }
                    }
                    break;
                default:
                    s = "未知品级";
                    if (iscompare) {
                        if (m_CItemName.color != m_Bai) {
                            m_CItemName.color = m_Bai;
                            m_CPinzhi.color = m_Bai;
                        }
                    } else {
                        if (m_ItemName.color != m_Bai) {
                            m_ItemName.color = m_Bai;
                            m_Pinzhi.color = m_Bai;
                        }
                    }
                    break;
            }
            return s;
        }
        /// <summary>
        /// 点击卸载按钮
        /// </summary>
        public void OnXiexia() {
            if (m_CurrenItem!=null) {
                switch ((ItemType)m_CurrenItem.m_Data.itemtype) {
                    case ItemType.yifu:
                        cd.Yifu = -1;
                        break;
                    case ItemType.kuzi:
                        cd.Kuzi = -1;
                        break;
                    case ItemType.wuqi:
                        cd .Wuqi = -1;
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
                bool isxiexia = AppTools.SendReturn<ItemBaseUI,bool>((int)MainUIEvent.XiexiaItem,m_CurrenItem);
                if (isxiexia && m_CurrenItem.m_Pos == ItemPos.inEqu) {//如果是在装备栏上，要实时显示装备卸下的效果，其实只有装备栏上才会显示卸下，也不会有其他情况
                    //这里隐藏就好，新装备时，会把原本ui数据替换掉
                    m_CurrenItem.gameObject.SetActive(false);
                }
                Close();
                //发送服务器，让服务器计算卸下后的玩家数据
                //todo
            }
        }
        /// <summary>
        /// 点击装备按钮
        /// </summary>
        public void OnZhuangbei() {
            if (m_CurrenItem != null) {
                //判断一下是否符合装备需求
                if (m_CurrenItem.IsCanEqu()) {
                    AppTools.Send<ItemBaseUI>((int)MainUIEvent.ChangeEquItem, m_CurrenItem);
                    Close();
                } else { //提示不满足装备
                    UIManager.GetInstance.OpenUIWidget(AppConfig.UIMsgTips,"角色不满足装备需求，无法装备！");
                }
            }
        }
        /// <summary>
        /// 丢弃道具
        /// </summary>
        private ItemBaseUI m_DiuqiItem = null;
        public void OnDiuqi() {
            if (m_CurrenItem!=null) {
                UIMsgBoxArg arg = new UIMsgBoxArg();
                arg.title = "警告";
                arg.content = $"{m_CurrenItem.m_Data.name}丢弃无法找回，请确认是否要进行丢弃，确认后系统将永久销毁此道具！";
                arg.btnname = "确认|取消";
                UIMsgBox temp = UIManager.GetInstance.OpenWindow(AppConfig.UIMsgBox, arg) as UIMsgBox;
                temp.oncloseevent += IsDiuqi;
                m_DiuqiItem = m_CurrenItem;
                Close();//close会清除m_CurrenItem，所以一定要放到后面
            }
        }
        private void IsDiuqi(object args = null) {
            ushort btnindex = ushort.Parse(args.ToString());
            if (btnindex == 0) {//点击第一个按钮，也就是确认
                AppTools.Send<ItemBaseUI>((int)ItemEvent.RecycleItemUI, m_DiuqiItem);
            } else { //点击取消，什么都不用做，基类已经处理了
            }
            m_DiuqiItem = null;
        }
        /// <summary>
        /// 使用某个道具
        /// </summary>
        public async void OnUse() {
            //通知背包，使用后减少物品
            var isusetask = await AppTools.SendReturn<ItemBaseUI, int, UniTask<bool>>((int)ItemEvent.ToDoItemNum, m_CurrenItem,-1);
            if (!isusetask) {
                UIManager.GetInstance.OpenUIWidget(AppConfig.UIMsgTips, $"使用失败，该道具数量不足！");
                return;
            }
            if (m_CurrenItem.m_IntXiaoguoDic.Count>0) {
                if (m_CurrenItem.IsCanEqu()) {
                    foreach (KeyValuePair<XiaoGuo,int> xiaoguo in m_CurrenItem.m_IntXiaoguoDic) {
                        switch (xiaoguo.Key) {
                            case XiaoGuo.addhp:
                                int huifu = UserService.GetInstance.m_CurrentPlayer.FP.FightMaxHp - UserService.GetInstance.m_CurrentPlayer.FP.FightHP;
                                if (huifu > 0) {//需要回血了再发送数据
                                    ClientBase.Instance.AddOperation(new Operation(Command.AddHpOrMp, ClientBase.Instance.UID) {
                                        index = xiaoguo.Value,//回血
                                        index1 = 0,//回蓝
                                    });
                                    UIManager.GetInstance.OpenUIWidget(AppConfig.UIMsgTips, $"使用成功，玩家回复血量{huifu}点");
                                } else { //==0
                                    UIManager.GetInstance.OpenUIWidget(AppConfig.UIMsgTips, $"使用成功，玩家生命已满！");
                                }
                                break;
                            case XiaoGuo.addmp:
                                int huilan = UserService.GetInstance.m_CurrentPlayer.FP.FightMaxMagic - UserService.GetInstance.m_CurrentPlayer.FP.FightMagic;
                                if (huilan > 0) {//需要回血了再发送数据
                                    ClientBase.Instance.AddOperation(new Operation(Command.AddHpOrMp, ClientBase.Instance.UID) {
                                        index = 0,//回血
                                        index1 = xiaoguo.Value,//回蓝
                                    });
                                    UIManager.GetInstance.OpenUIWidget(AppConfig.UIMsgTips, $"使用成功，玩家回复魔力{huilan}点");
                                } else {
                                    UIManager.GetInstance.OpenUIWidget(AppConfig.UIMsgTips, $"使用成功，玩家法力已满！");
                                }
                                break;
                            case XiaoGuo.addjinbi:
                                var task = await ClientBase.Instance.Call((ushort)ProtoType.addjinbi, pars:xiaoguo.Value);
                                if (!task.IsCompleted) {
                                    UIManager.GetInstance.OpenUIWidget(AppConfig.UIMsgTips, "请求超时，请检查网络链接");
                                    return;
                                } else {
                                    int code = task.model.AsInt;
                                    if (code == -1) {
                                        //有可能是减去
                                        UIManager.GetInstance.OpenUIWidget(AppConfig.UIMsgTips, "金币数量不足！");
                                    } else {
                                        //通知ui
                                        UIManager.GetInstance.OpenUIWidget(AppConfig.UIMsgTips, $"使用成功，玩家增加{xiaoguo.Value}金币");
                                        AppTools.Send<int, ushort>((int)ItemEvent.AddJinbiOrZuanshi, xiaoguo.Value,0);
                                    }
                                   
                                }
                                break;
                            case XiaoGuo.addzhuanshi:
                                //设计的第二个参数是钻石
                                var task1 = await ClientBase.Instance.Call((ushort)ProtoType.addzuanshi,pars:xiaoguo.Value);
                                if (!task1.IsCompleted) {
                                    UIManager.GetInstance.OpenUIWidget(AppConfig.UIMsgTips, "请求超时，请检查网络链接");
                                    return;
                                } else {
                                    int code = task1.model.AsInt;
                                    if (code == -1) {
                                        //addzuansh有可能是减去
                                        UIManager.GetInstance.OpenUIWidget(AppConfig.UIMsgTips, "钻石数量不足！");
                                    } else {
                                        //通知ui
                                        UIManager.GetInstance.OpenUIWidget(AppConfig.UIMsgTips, $"使用成功，玩家增加{xiaoguo.Value}钻石");
                                        AppTools.Send<int, ushort>((int)ItemEvent.AddJinbiOrZuanshi, xiaoguo.Value, 1);
                                    }
                                }
                                break;
                            case XiaoGuo.addexp:
                                //发送服务器即可，如果升级，服务器会计算
                                ClientBase.Instance.AddOperation(new Operation(Command.addrexp,ClientBase.Instance.UID) {
                                    index = xiaoguo.Value,
                                });
                                UIManager.GetInstance.OpenUIWidget(AppConfig.UIMsgTips, $"使用成功，玩家增加经验{xiaoguo.Value}点");
                                break; 
                            case XiaoGuo.addskill:

                                break;
                            case XiaoGuo.additem:

                                break;
                        }
                    }
                    
                }
            }
            Close();
        }
        protected override void OnClose(bool bClear = false, object arg = null) {
            base.OnClose(bClear, arg);
        }
        public override void Close(bool bClear = false, object arg = null) {
            base.Close(bClear, arg);
            m_CurrenItem = null;
        }

    }
}

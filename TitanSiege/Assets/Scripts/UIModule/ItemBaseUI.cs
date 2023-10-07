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
using GF.MainGame.Module.NPC;
using GF.Service;
using GF.Unity.AB;
using System.Collections.Generic;
using System.Text;
using Titansiege;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
namespace GF.MainGame.UI {
    public class EffArgs {
        public string effname;//效果或者需求名称,用来匹配
        public float fvalue;//flaot的效果值
        public int ivalue;//int类型的效果至
    }
    public class ItemBaseUI : MonoBehaviour {
        public Image m_ItemColor;
        public Button m_ItemBtn;
        public Text m_ItemNum;
        public ItemDataBase m_Data;
        private int m_Num;//道具数量
        public int Num { 
            get { 
                return m_Num;
            }
            set {
                m_Num = value;
                if (m_Num == 1) {
                    m_ItemNum.gameObject.SetActive(false);
                } else {
                    if (!m_ItemNum.gameObject.activeSelf) {
                        m_ItemNum.gameObject.SetActive(true);
                    }
                }
                m_ItemNum.text = m_Num.ToString();

            } 
        }
        /// <summary>
        /// 是否在背包内，因为装备界面用的也是这个，所以判断一下，如果不在背包内，就是装备栏上的，或者在选择装备界面
        /// </summary>
        public ItemPos m_Pos;
        /// <summary>
        /// 效果值的存储，属性类效果值，一般是装备上使用，需要大量字符串匹配
        /// </summary>
        public Dictionary<XiaoGuo, Dictionary<string, EffArgs>> m_PropXiaoguoDic = new Dictionary<XiaoGuo, Dictionary<string, EffArgs>>();
        /// <summary>
        /// 效果值的存储，使用类效果，一般是添加某个数值，例如药品、金币、钻石
        /// </summary>
        public Dictionary<XiaoGuo, int> m_IntXiaoguoDic = new Dictionary<XiaoGuo,int>();
        /// <summary>
        /// 需求值的存储，属性类效果值，一般是装备上使用，需要大量字符串匹配
        /// </summary>
        public Dictionary<XuQiu, Dictionary<string, EffArgs>> m_XuqiuDic = new Dictionary<XuQiu, Dictionary<string, EffArgs>>();
        /// <summary>
        /// 需求值的存储，使用类效果，一般是添加某个数值，例如等级
        /// </summary>
        public Dictionary<XuQiu, int> m_IntXuqiuDic = new Dictionary<XuQiu, int>();

        

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
            Num = num;
            m_Pos = pos;
            m_PropXiaoguoDic.Clear();
            m_IntXiaoguoDic.Clear();
            m_XuqiuDic.Clear();
            m_IntXuqiuDic.Clear();
            XiaoguoToDic();
            XuqiuToDic();
        }
        /// <summary>
        /// 异步资源加载完成后的回调
        /// </summary>
        /// <param name="path"></param>
        /// <param name="obj"></param>
        /// <param name="objarr"></param>
        public void OnLoadSpriteOver(string path, Object obj, params object[] objarr) {
            if (obj!=null) {
                Texture2D t2d=(obj as Texture2D);
                m_ItemBtn.image.sprite = Sprite.Create(t2d, new Rect(0, 0, t2d.width, t2d.height), Vector2.zero);
            }
        }
        private void Start() {
            m_ItemBtn.onClick.AddListener(ClickBtn);
        }
        private void ClickBtn() {
            AppTools.Send<ItemBaseUI>((int)ItemEvent.ShowItemDesc, this);
            //switch (m_Pos) {
            //    case ItemPos.inBag: //在背包内的点击，就显示装备介绍的UI
            //        AppTools.Send<ItemBaseUI>((int)ItemEvent.ShowItemDesc,this);
            //        break;
            //    case ItemPos.inEqu:
            //        //在装备栏上，显示卸下按钮
            //        AppTools.Send<ItemBaseUI>((int)ItemEvent.ShowItemDesc, this);
            //        break;
            //    case ItemPos.inSelect:
            //        AppTools.Send<ItemBaseUI>((int)ItemEvent.ShowItemDesc, this);
            //        break;
            //    default:
            //        Debuger.Log("未知道具位置，请检查参数！");
            //        break;
            //}
        }
        
        /// <summary>
        /// 道具效果转成字典
        /// </summary>
        /// <returns></returns>
        private void XiaoguoToDic() {
            if (m_Data.xiaoguo1 > 0) { //无效果时，默认为0
                XiaoguoTo(m_Data.xiaoguo1, m_Data.xiaoguo1zhi);
            }
            if (m_Data.xiaoguo2 > 0) {
                XiaoguoTo(m_Data.xiaoguo2, m_Data.xiaoguo2zhi);
            }
            if (m_Data.xiaoguo3 > 0) {
                XiaoguoTo(m_Data.xiaoguo3, m_Data.xiaoguo3zhi);
            }
        }
        /// <summary>
        /// 根据效果的类型，用不同的方式处理效果值
        /// </summary>
        /// <param name="xiaoguo"></param>
        /// <param name="xiaoguozhi"></param>
        /// <returns></returns>
        private void XiaoguoTo(int xiaoguo, string xiaoguozhi) {
            switch ((XiaoGuo)xiaoguo) {
                case XiaoGuo.addhp:
                    int num1 = int.Parse(xiaoguozhi);
                    m_IntXiaoguoDic.Add(XiaoGuo.addhp, num1);
                    break;
                case XiaoGuo.addmp:
                    int num2 = int.Parse(xiaoguozhi);
                    m_IntXiaoguoDic.Add(XiaoGuo.addmp, num2);
                    break;
                case XiaoGuo.addjinbi:
                    int num3 = int.Parse(xiaoguozhi);
                    m_IntXiaoguoDic.Add(XiaoGuo.addjinbi, num3);
                    break;
                case XiaoGuo.addzhuanshi:
                    int num4 = int.Parse(xiaoguozhi);
                    m_IntXiaoguoDic.Add(XiaoGuo.addzhuanshi, num4);
                    break;
                case XiaoGuo.addexp:
                    int num5 = int.Parse(xiaoguozhi);
                    m_IntXiaoguoDic.Add(XiaoGuo.addexp, num5);
                    break;
                case XiaoGuo.addfightprop:
                    PropToDic(XiaoGuo.addfightprop, xiaoguozhi);
                    break;
                case XiaoGuo.addprop:
                    PropToDic(XiaoGuo.addprop,xiaoguozhi);
                    break;
                case XiaoGuo.addskill:
                    break;
                case XiaoGuo.additem:
                    break;
            }
        }
        /// <summary>
        /// 将效果值中的addfightprop和addprop转化为字典存储
        /// </summary>
        /// <param name="propstr"></param>
        /// <returns></returns>
        private void PropToDic(XiaoGuo xiaoguo,string propstr) {
            if (propstr == "0") { //效果值为空时默认为“0”
                return;
            }
            if (propstr.Contains(",")) {
                string[] strarr = propstr.Split(',');
                if (strarr.Length > 0) {
                    Dictionary<string, EffArgs> tempdic = new Dictionary<string, EffArgs>();
                    for (int i = 0; i < strarr.Length; i++) {
                        if (!string.IsNullOrEmpty(strarr[i])) {//判断一下空白，防止配表的多写
                            string[] strarr1 = strarr[i].Split('|');
                            EffArgs e = new EffArgs();
                            e.effname = strarr1[0];
                            if (strarr1[0] == "baoji" || strarr1[0] == "shanbi") {
                                e.fvalue = float.Parse(strarr1[1]);
                                e.ivalue = 0;
                            } else {
                                e.ivalue = int.Parse(strarr1[1]);
                                e.fvalue = 0f;
                            }
                            tempdic.Add(strarr1[0], e);
                        }
                    }
                    m_PropXiaoguoDic.Add(xiaoguo, tempdic);
                }
            } else { //前面已经判断过空了，所以这里只可能时只有一个元素的情况
                Dictionary<string, EffArgs> tempdic = new Dictionary<string, EffArgs>();
                string[] strarr = propstr.Split('|');
                EffArgs e = new EffArgs();
                e.effname = strarr[0];
                if (strarr[0] == "baoji" || strarr[0] == "shanbi") {
                    e.fvalue = float.Parse(strarr[1]);
                    e.ivalue = 0;
                } else {
                    e.ivalue = int.Parse(strarr[1]);
                    e.fvalue = 0f;
                }
                tempdic.Add(strarr[0], e);
                m_PropXiaoguoDic.Add(xiaoguo, tempdic);
            }
        }
        #region 需求存字典
        private void XuqiuToDic() {
            if (m_Data.xuqiu1 > 0) { //无效果时，默认为0
                XuqiuTo(m_Data.xuqiu1, m_Data.xuqiu1zhi);
            }
            if (m_Data.xuqiu2 > 0) {
                XuqiuTo(m_Data.xuqiu2, m_Data.xuqiu2zhi);
            }
            if (m_Data.xuqiu3 > 0) {
                XuqiuTo(m_Data.xuqiu3, m_Data.xuqiu3zhi);
            }
        }

        private void XuqiuTo(int xuqiu,string xuqiuzhi) {
            switch ((XuQiu)xuqiu) {
                case XuQiu.level:
                    int num1 = int.Parse(xuqiuzhi);
                    m_IntXuqiuDic.Add(XuQiu.level, num1);
                    break;
                case XuQiu.zhiye:
                    int num2 = int.Parse(xuqiuzhi);
                    m_IntXuqiuDic.Add(XuQiu.zhiye, num2);
                    break;
                case XuQiu.fightprop:
                    XuqiuPropToDic(XuQiu.fightprop, xuqiuzhi);
                    break;
                case XuQiu.prop:
                    XuqiuPropToDic(XuQiu.prop, xuqiuzhi);
                    break;

            }
        }
        /// <summary>
        /// 将效果值中的addfightprop和addprop转化为字典存储
        /// </summary>
        /// <param name="propstr"></param>
        /// <returns></returns>
        private void XuqiuPropToDic(XuQiu xuqiu, string propstr) {
            if (propstr == "0") { //效果值为空时默认为“0”
                return;
            }
            if (propstr.Contains(",")) {
                string[] strarr = propstr.Split(',');
                if (strarr.Length > 0) {
                    Dictionary<string, EffArgs> tempdic = new Dictionary<string, EffArgs>();
                    for (int i = 0; i < strarr.Length; i++) {
                        if (!string.IsNullOrEmpty(strarr[i])) {//判断一下空白，防止配表的多写
                            string[] strarr1 = strarr[i].Split('|');
                            EffArgs e = new EffArgs();
                            e.effname = strarr1[0];
                            if (strarr1[0] == "baoji" || strarr1[0] == "shanbi") {
                                e.fvalue = float.Parse(strarr1[1]);
                                e.ivalue = 0;
                            } else {
                                e.ivalue = int.Parse(strarr1[1]);
                                e.fvalue = 0f;
                            }
                            tempdic.Add(strarr1[0], e);
                        }
                    }
                    m_XuqiuDic.Add(xuqiu, tempdic);
                }
            } else { //前面已经判断过空了，所以这里只可能时只有一个元素的情况
                Dictionary<string, EffArgs> tempdic = new Dictionary<string, EffArgs>();
                string[] strarr = propstr.Split('|');
                EffArgs e = new EffArgs();
                e.effname = strarr[0];
                if (strarr[0] == "baoji" || strarr[0] == "shanbi") {
                    e.fvalue = float.Parse(strarr[1]);
                    e.ivalue = 0;
                } else {
                    e.ivalue = int.Parse(strarr[1]);
                    e.fvalue = 0f;
                }
                tempdic.Add(strarr[0], e);
                m_XuqiuDic.Add(xuqiu, tempdic);
            }
        }
        /// <summary>
        /// 能否装备
        /// </summary>
        /// <returns></returns>
        public bool IsCanEqu(CharactersData cd = null,FightProp fp = null) {
            if (cd == null) {
                cd = UserService.GetInstance.m_CurrentChar;
                fp = UserService.GetInstance.m_CurrentPlayer.FP;
            }
            if (m_XuqiuDic.Count>0) {//有属性类的需求
                foreach (KeyValuePair<XuQiu,Dictionary<string,EffArgs>> xuqiu in m_XuqiuDic) {
                    if (xuqiu.Value.Count>0) {
                        foreach (KeyValuePair<string, EffArgs> xq in xuqiu.Value) {
                            switch (xq.Key) {
                                case "liliang":
                                    if (cd.Liliang <xq.Value.ivalue) {
                                        return false;
                                    }
                                    break;
                                case "tizhi":
                                    if (cd.Tizhi < xq.Value.ivalue) {
                                        return false;
                                    }
                                    break;
                                case "minjie":
                                    if (cd.Minjie < xq.Value.ivalue) {
                                        return false;
                                    }
                                    break;
                                case "moli":
                                    if (cd.Moli < xq.Value.ivalue) {
                                        return false;
                                    }
                                    break;
                                case "xingyun":
                                    if (cd.Xingyun < xq.Value.ivalue) {
                                        return false;
                                    }
                                    break;
                                case "meili":
                                    if (cd.Meili < xq.Value.ivalue) {
                                        return false;
                                    }
                                    break;
                                case "maxhp":
                                    if (fp.FightMaxHp < xq.Value.ivalue) {
                                        return false;
                                    }
                                    break;
                                case "maxmagic":
                                    if (fp.FightMaxMagic < xq.Value.ivalue) {
                                        return false;
                                    }
                                    break;
                                case "gongji":
                                    if (fp.Attack < xq.Value.ivalue) {
                                        return false;
                                    }
                                    break;
                                case "fangyu":
                                    if (fp.Defense < xq.Value.ivalue) {
                                        return false;
                                    }
                                    break;
                                case "shanbi":
                                    if (fp.Dodge < xq.Value.fvalue) {
                                        return false;
                                    }
                                    break;
                                case "baoji":
                                    if (fp.Crit < xq.Value.fvalue) {
                                        return false;
                                    }
                                    break;
                                case "lianjin":
                                    if (cd.Lianjin < xq.Value.ivalue) {
                                        return false;
                                    }
                                    break;
                                case "duanzao":
                                    if (cd.Duanzao < xq.Value.ivalue) {
                                        return false;
                                    }
                                    break;
                                default:
                                    Debuger.LogError($"位置属性{xq.Key}，无法匹配计算，请检查数据表");
                                    return false;
                            }
                        }
                    }
                }
            }
            if (m_IntXuqiuDic.Count > 0) {//有数值类的需求
                foreach (KeyValuePair<XuQiu,int> xuqiu in m_IntXuqiuDic) {
                    switch (xuqiu.Key) {
                        case XuQiu.level:
                            if (cd.Level < xuqiu.Value) {
                                return false;
                            }
                            break;
                        case XuQiu.zhiye:
                            if (cd.Zhiye != xuqiu.Value) {
                                return false;
                            }
                            break;
                        default:
                            Debuger.LogError($"位置属性{xuqiu.Key}，无法匹配计算，请检查数据表");
                            return false;
                    }
                }
            }
            //以上情况都满足，就是可以佩戴
            return true;
        }
        #endregion
    }

}

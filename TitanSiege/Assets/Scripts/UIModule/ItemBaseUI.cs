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
using System.Collections.Generic;
using System.Text;
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
            if (obj!=null) {
                Texture2D t2d=(obj as Texture2D);
                m_ItemBtn.image.sprite = Sprite.Create(t2d, new Rect(0, 0, t2d.width, t2d.height), Vector2.zero);
            }
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
        /// <summary>
        /// 道具效果转成字符串显示
        /// </summary>
        /// <returns></returns>
        private void XiaoguoToDic(ItemDataBase data) {
            if (data.xiaoguo1 > 0) { //无效果时，默认为0
                XiaoguoTo(data.xiaoguo1, data.xiaoguo1zhi);
            }
            if (data.xiaoguo2 > 0) {
                XiaoguoTo(data.xiaoguo2, data.xiaoguo2zhi);
            }
            if (data.xiaoguo3 > 0) {
                XiaoguoTo(data.xiaoguo3, data.xiaoguo3zhi);
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
                            } else {
                                e.ivalue = int.Parse(strarr1[1]);
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
        
    }
    
}

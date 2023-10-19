/****************************************************
    文件：ItemBase.cs
	作者：昔莘
    邮箱: 304183153@qq.com
    日期：2023/10/19 21:37:37
	功能：Nothing
*****************************************************/

using GF.ConfigTable;
using GF.MainGame;
using GF.MainGame.Data;
using GF.MainGame.Module;
using GF.MainGame.UI;
using System.Collections.Generic;
using UnityEngine;
namespace GF.MainGame.Module {
    public class ItemBase : MonoBehaviour {
        public ItemDataBase m_Data;
        /// <summary>
        /// 效果值的存储，属性类效果值，一般是装备上使用，需要大量字符串匹配
        /// </summary>
        public Dictionary<XiaoGuo, Dictionary<string, EffArgs>> m_PropXiaoguoDic = new Dictionary<XiaoGuo, Dictionary<string, EffArgs>>();
        /// <summary>
        /// 效果值的存储，使用类效果，一般是添加某个数值，例如药品、金币、钻石
        /// </summary>
        public Dictionary<XiaoGuo, int> m_IntXiaoguoDic = new Dictionary<XiaoGuo, int>();
        public ItemBase(int id) {
            m_Data = AppTools.GetModule<ItemModule>(MDef.ItemModule).m_Data.FindItemByID(id);
            XiaoguoToDic();
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
                    PropToDic(XiaoGuo.addprop, xiaoguozhi);
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
        private void PropToDic(XiaoGuo xiaoguo, string propstr) {
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
    }
}
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

namespace GF.MainGame.UI {
    public class InfoUIWindow : UIWindow {
        public Text m_NameTxt;
        public Text m_GdidTxt;
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

        private ModelShow m_ModelShow;
        public void Start() {
            if (m_ModelShow == null) {
                GameObject go = ObjectManager.GetInstance.InstanceObject("NPCPrefab/modelcamera.prefab",bClear:false);
                if (go!=null) {
                    m_ModelShow = go.GetComponent<ModelShow>();
                }
            }
        }
        protected override void OnOpen(object args = null) {
            base.OnOpen(args);

        }
        public override void Close(bool bClear = false, object arg = null) {
            base.Close(bClear, arg);
        }

        protected override void OnClose(bool bClear = false, object arg = null) {
            base.OnClose(bClear, arg);
        }

    }
}
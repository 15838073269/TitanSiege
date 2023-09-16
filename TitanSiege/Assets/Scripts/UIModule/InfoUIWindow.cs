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

    }
    public override void Close(bool bClear = false, object arg = null) {
            base.Close(bClear, arg);
            m_ModelShow.gameObject.SetActive(false);
        }

        protected override void OnClose(bool bClear = false, object arg = null) {
            base.OnClose(bClear, arg);
        }

    }
}
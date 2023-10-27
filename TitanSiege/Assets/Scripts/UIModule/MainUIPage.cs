/****************************************************
    文件：MainUIPage.cs
	作者：昔莘
    邮箱: 304183153@qq.com
    日期：2022/3/1 14:32:16
	功能：Nothing
*****************************************************/

using GF.MainGame.Module;
using GF.MainGame.Module.NPC;
using GF.Service;
using GF.Unity.AB;
using GF.Unity.UI;
using Net.Client;
using Titansiege;
using UnityEngine;
using UnityEngine.UI;
namespace GF.MainGame.UI {
    public class MainUIPage : UIPage {
        #region UI变量
        public Animation listani;
        public Button zhankaibtn;
        public Button bagbtn;
        public Button renwubtn;
        public Button shezhibtn;
        public Button renwuhiden;
        public Button headbtn;
        public Button infobtn;
        public Button pengyoubtn;
        public Text playername;
        public Text level;
        public Text hp;
        public Image hpimg;
        public Text mp;
        public Image mpimg;
        #endregion
        private bool isbtnopen;//按钮列表是否展开，默认展开
        private bool isrenwuhiden;//任务列表是否展开，默认展开
        private MainUIModule mu;
        public void Start() {
            isbtnopen = true;//按钮列表是否展开，默认展开
            isrenwuhiden = true;//任务列表是否展开，默认展开
            renwuhiden.onClick.AddListener(TaskOpenOrHiden);
            zhankaibtn.onClick.AddListener(BtnOpenOrHiden);
            infobtn.onClick.AddListener(() => {
                BtnClick("info");
            });
            headbtn.onClick.AddListener(() => {
                BtnClick("info");
            });
            bagbtn.onClick.AddListener(() => {
                BtnClick("bag");
            });
            shezhibtn.onClick.AddListener(() => {
                BtnClick("shezhi");
            });
            renwubtn.onClick.AddListener(() => {
                BtnClick("renwu");
            });
            pengyoubtn.onClick.AddListener(() => {
                BtnClick("pengyou");
            });
        }
        public  void Init(Player p=null) {
            if (p == null) {
                p = UserService.GetInstance.m_CurrentPlayer;
            }
            playername.text = UserService.GetInstance.m_CurrentChar.Name;
            level.text = UserService.GetInstance.m_CurrentChar.Level.ToString();
            hp.text = $"{p.FP.FightHP}/{p.FP.FightMaxHp}";
            hpimg.fillAmount = (float)p.FP.FightHP / (float)p.FP.FightMaxHp;
            mp.text = $"{p.FP.FightMagic}/{p.FP.FightMaxMagic}";
            mpimg.fillAmount = (float)p.FP.FightMagic / (float)p.FP.FightMaxMagic;
            headbtn.image.sprite = ResourceManager.GetInstance.LoadResource<Sprite>(UserService.GetInstance.m_CurrentChar.Headpath, bClear: false);
        }
        public override void Close(bool bClear = false, object arg = null) {
            base.Close(bClear, arg);
        }

        protected override void OnClose(bool bClear = false, object arg = null) {
            base.OnClose(bClear, arg);
        }
        public void BtnOpenOrHiden() {
            if (isbtnopen) {
                listani.Play("btnlisthidden");
                isbtnopen = false;
            } else {
                listani.Play("btnlistshow");
                isbtnopen = true;
            }
        }
        public void TaskOpenOrHiden() {
            if (isrenwuhiden) {
                listani.Play("taskhidden");
                isrenwuhiden = false;
            } else {
                listani.Play("taskshow");
                isrenwuhiden = true;
            }
        }
        public void BtnClick(string name) {
            AppTools.PlayBtnClick();
            AppTools.GetModule<MainUIModule>(MDef.MainUIModule).BtnClick(name);
        }
        /// <summary>
        /// 更新ui面板上的血条和蓝条
        /// </summary>
        public void UpdateHpMp() {
            //面板只属于本机玩家
            Player p = UserService.GetInstance.m_CurrentPlayer;
            hp.text = $"{p.FP.FightHP}/{p.FP.FightMaxHp}";
            hpimg.fillAmount = (float)p.FP.FightHP / (float)p.FP.FightMaxHp;
            mp.text = $"{p.FP.FightMagic}/{p.FP.FightMaxMagic}";
            mpimg.fillAmount = (float)p.FP.FightMagic / (float)p.FP.FightMaxMagic;
        }
    }
}

/****************************************************
    文件：MainUIPage.cs
	作者：昔莘
    邮箱: 304183153@qq.com
    日期：2022/3/1 14:32:16
	功能：Nothing
*****************************************************/

using GF.MainGame.Module;
using GF.Unity.AB;
using GF.Unity.UI;
using Titansiege;
using UnityEngine;
using UnityEngine.UI;
namespace GF.MainGame.UI {
    public class MainUIPage : UIPage {
        #region UI变量
        public Animation listani;
        public Button zhankaibtn;
        public Button bagbtn;
        public Button skillbtn;
        public Button tianfubtn;
        public Button renwubtn;
        public Button shezhibtn;
        public Button renwuhiden;
        public Button headbtn;

        public Text playername;
        public Text level;
        public Text hp;
        public Text mp;
        #endregion
        private bool isbtnopen;//按钮列表是否展开，默认展开
        private bool isrenwuhiden;//任务列表是否展开，默认展开
        private MainUIModule mu;
        public void Start() {
            isbtnopen = true;//按钮列表是否展开，默认展开
            isrenwuhiden = true;//任务列表是否展开，默认展开
            renwuhiden.onClick.AddListener(TaskOpenOrHiden);
            zhankaibtn.onClick.AddListener(BtnOpenOrHiden);
            //headbtn.onClick.AddListener(() => {
            //    BtnClick("info");
            //});
            //bagbtn.onClick.AddListener(() => {
            //    BtnClick("bag");
            //});
            ////skillbtn.onClick.AddListener(() => {
            ////    BtnClick("skill");
            ////});
            //tianfubtn.onClick.AddListener(() => {
            //    BtnClick("tianfu");
            //});
            //shezhibtn.onClick.AddListener(() => {
            //    BtnClick("shezhi");
            //});
            //renwubtn.onClick.AddListener(() => {
            //    BtnClick("renwu");
            //});
        }
        protected override void OnOpen(object args = null) {
            base.OnOpen(args);
            if (args!=null) {
                var cd = args as CharactersData;
                playername.text = cd.Name;
                level.text = cd.Level.ToString();
                hp.text = cd.Shengming.ToString();
                mp.text = cd.Fali.ToString();
                headbtn.image.sprite = ResourceManager.GetInstance.LoadResource<Sprite>(cd.Headpath,bClear:false);
            }
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
        
    }
}

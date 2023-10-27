/****************************************************
    文件：MianUIModule.cs
	作者：昔莘
    邮箱: 304183153@qq.com
    日期：2022/3/9 15:1:7
	功能：主界面UI的管理模块
*****************************************************/

using GF.Module;
using GF.Unity.UI;
using GF.Service;
using GF.MainGame.UI;
using Titansiege;
using Cysharp.Threading.Tasks;
using GF.MainGame.Data;
using Unity.VisualScripting;

namespace GF.MainGame.Module {
    public class MainUIModule : GeneralModule {
        MainUIPage m_Ui;
        InfoUIWindow m_Info;
        public override void Create() {
            base.Create();
            ///测试使用，生产可以注释
            if (UserService.GetInstance.m_CurrentChar == null) {
                TestScene t1 = new TestScene();
            }
            Show();
            AppTools.Regist((int)MainUIEvent.UpdateHpMp, UpdateHpMp);
            AppTools.Regist<ItemBaseUI>((int)MainUIEvent.ChangeEquItem, ChangeEquItem);
            AppTools.Regist<ItemBaseUI, UniTask<bool>>((int)MainUIEvent.XiexiaItem, XiexiaItem);
        } 
        public override void Release() {
            base.Release();
        }

        public override void Show() {
            base.Show();
            m_Ui = UIManager.GetInstance.OpenPage(AppConfig.MainUIPage) as MainUIPage;
            if (AppTools.GetModule<MoveModule>(MDef.MoveModule) == null) {
                AppTools.CreateModule<MoveModule>(MDef.MoveModule);
            }
            if (AppTools.GetModule<FightModule>(MDef.FightModule) == null) {
                AppTools.CreateModule<FightModule>(MDef.FightModule);
            }
            m_Ui.Init();
            ///创建血条模块
            if (!AppTools.HasModule(MDef.HPModule)) {
                AppTools.CreateModule<HPModule>(MDef.HPModule);
            }
            //创建聊天模块
            TalkModule t = AppTools.CreateModule<TalkModule>(MDef.TalkModule);
            t.m_TalkUI = m_Ui.transform.Find("TalkUI").GetComponent<TalkUI>();//初始化聊天模块ui
            InitUI();
        }
        public void InitUI() {
            //这里需要先加载一次，不然背包模块没启动，数据过不来
            AppTools.Send((int)ItemEvent.ShowBag);
            UIManager.GetInstance.CloseWindow(AppConfig.BagUIWindow);
            //这里需要先加载一次，不然角色信息模块没启动，装备数据过不来
            m_Info = UIManager.GetInstance.OpenWindow(AppConfig.InfoUIWindow) as InfoUIWindow;
            UIManager.GetInstance.CloseWindow(AppConfig.InfoUIWindow);
        }
        /// <summary>
        /// 更新ui面板上的血条和蓝条
        /// </summary>
        public void UpdateHpMp() {
            m_Ui.UpdateHpMp();
        }
        public void BtnClick(string name) {
            switch (name) {
                case "info":
                    UIManager.GetInstance.OpenWindow(AppConfig.InfoUIWindow);
                    break;
                case "bag":
                    AppTools.Send((int)ItemEvent.ShowBag);
                    break;
                case "skill":
                    UIManager.GetInstance.OpenUIWidget(AppConfig.UIMsgTips, "功能正在开发中");
                    break;
                case "ronglu":
                    UIManager.GetInstance.OpenUIWidget(AppConfig.UIMsgTips, "功能正在开发中");
                    break;
                case "renwu":
                    UIManager.GetInstance.OpenUIWidget(AppConfig.UIMsgTips, "功能正在开发中");
                    break;
                case "shezhi":
                    UIManager.GetInstance.OpenUIWidget(AppConfig.UIMsgTips, "功能正在开发中");
                    break;
                case "pengyou":
                    UIManager.GetInstance.OpenUIWidget(AppConfig.UIMsgTips, "功能正在开发中");
                    break;
                default:
                    break;
            }
        }
        private void ChangeEquItem(ItemBaseUI itemui) {
            m_Info.ChangeEquItem(itemui);
        }
        private async UniTask<bool> XiexiaItem(ItemBaseUI itemui) {
           return await m_Info.XiexiaItem(itemui);
        }
    }
}

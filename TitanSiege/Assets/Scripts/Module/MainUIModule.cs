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

namespace GF.MainGame.Module {
    public class MainUIModule : GeneralModule {
        MainUIPage m_Ui;
        public override void Create() {
            base.Create();
            ///测试使用，生产可以注释
            if (UserService.GetInstance.m_CurrentChar == null) {
                TestScene t1 = new TestScene();
            }
            Show();
            ///创建血条模块
            if (!AppTools.HasModule(MDef.HPModule)) {
                AppTools.CreateModule<HPModule>(MDef.HPModule);
            }
            AppTools.Regist((int)MainUIEvent.UpdateHpMp, UpdateHpMp);
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
                    break;
                case "skill":
                    break;
                case "tianfu":
                    break;
                case "renwu":
                    break;
                case "sehzhi":
                    break;
                default:
                    break;
            }
        }
    }
}

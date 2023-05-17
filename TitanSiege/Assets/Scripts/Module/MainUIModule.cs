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
        private CharactersData nb=null;
        private MainUIPage m_Ui;
        public override void Create() {
            base.Create();
            ///测试使用，生产可以注释
            if (UserService.GetInstance.m_CurrentChar == null) {
                TestScene t1 = new TestScene();
            }
            nb = UserService.GetInstance.m_CurrentChar;
            Show();
            ///创建血条模块
            if (!AppTools.HasModule(MDef.HPModule)) {
                AppTools.CreateModule<HPModule>(MDef.HPModule);
            }
        } 
        public override void Release() {
            base.Release();
        }

        public override void Show() {
            base.Show();
            m_Ui = UIManager.GetInstance.OpenPage(AppConfig.MainUIPage, nb) as MainUIPage;
            if (AppTools.GetModule<MoveModule>(MDef.MoveModule) == null) {
                AppTools.CreateModule<MoveModule>(MDef.MoveModule);
            }
            if (AppTools.GetModule<FightModule>(MDef.FightModule) == null) {
                AppTools.CreateModule<FightModule>(MDef.FightModule);
            }
        }
        public void BtnClick(string name) {
            switch (name) {
                case "info":
                    //info.Show<NPCDataBase>(nb);
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

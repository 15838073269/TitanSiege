/****************************************************
    文件：DieUIModule.cs
	作者：昔莘
    邮箱: 304183153@qq.com
    日期：2023/4/12 20:35:23
	功能：玩家死亡模块
*****************************************************/
using GF;
using GF.MainGame;
using GF.Module;
using GF.Unity.UI;
namespace GF.MainGame.Module {
    public class DieUIModule : GeneralModule {
        public override void Create() {
            base.Create();
            AppTools.Regist((int)DieEvent.ShowUI, ShowUI);
            Debuger.Log("死亡模块启动");
        }

        public override void Release() {
            base.Release();
        }

        public void ShowUI() {
            Debuger.Log("打开死亡ui");
            UIManager.GetInstance.OpenWindow(AppConfig.DieUIWindow);
        }
    }
}

/****************************************************
    �ļ���DieUIModule.cs
	���ߣ���ݷ
    ����: 304183153@qq.com
    ���ڣ�2023/4/12 20:35:23
	���ܣ��������ģ��
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
            AppTools.Regist((int)DieEvent.HideUI, HideUI);
        }

        public override void Release() {
            base.Release();
        }

        public void ShowUI() {
            UIManager.GetInstance.OpenWindow(AppConfig.DieUIWindow);
        }
        public void HideUI() {
            UIManager.GetInstance.CloseWindow(AppConfig.DieUIWindow);
        }
    }
}

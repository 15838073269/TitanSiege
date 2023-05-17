/****************************************************
    文件：FightModule.cs
	作者：昔莘
    邮箱: 304183153@qq.com
    日期：2022/11/18 23:40:1
	功能：Nothing
*****************************************************/
using GF.MainGame.Module;
using GF.Module;

namespace GF.MainGame.Module {
    public class FightModule : GeneralModule {
        public override void Create() {
            base.Create();
            AppTools.CreateModule<SkillModule>(MDef.SkillModule);
            AppTools.CreateModule<StateModule>(MDef.StateModule);
        }

        public override void Release() {
            base.Release();
        }

        public override void Show() {
            base.Show();
        }
    }
}

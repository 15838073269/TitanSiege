using GF.Service;
using GF.Unity.Audio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace GF.MainGame.Module {
    public class CreateRoleScene : SceneBase {
        public override void Start() {
            base.Start();
            //创建角色穿件或选择模块
            AppTools.CreateModule<CreateAndSelectModule>(MDef.CreateAndSelectModule);
            if (UserService.GetInstance.m_UserModel.m_CharacterList.Count == 0) {
                AppTools.Send<bool>((int)CreateAndSelectEvent.ShowWind, false);
            } else {
                AppTools.Send<bool>((int)CreateAndSelectEvent.ShowWind, true);
            }
        }
    }
}


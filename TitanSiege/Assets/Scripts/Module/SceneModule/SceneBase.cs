/****************************************************
    文件：SceneBase.cs
	作者：昔莘
    邮箱: 304183153@qq.com
    日期：2021/12/6 8:46:40
	功能：单个场景的基类
*****************************************************/

using UnityEngine;
using GF.MainGame;
using GF.Module;
using GF.Msg;
using DG.Tweening.Core.Easing;
using GF.MainGame.Module.NPC;
using Net.Client;
using Net.Component;
using Net.UnityComponent;
using GF.Service;
using GameDesigner;
using GF.Unity.Audio;

namespace GF.MainGame.Module {
    public class SceneBase:MonoBehaviour {
        public string SceneName;
        public string m_CharName;
        public AudioClip m_BgMusic = null;//当前场景的背景音乐，可以为空
        /// <summary>
        /// 玩家进入场景后的默认位置信息
        /// </summary>
        public Transform m_Default;//需要自行配置
        protected SceneModule m_Sm;
        public virtual void Start() {
            if (m_Sm == null) {
                m_Sm = AppTools.GetModule<SceneModule>(MDef.SceneModule);
                
            }
            if (string.IsNullOrEmpty(m_Sm.m_Current)) {
                m_Sm.m_Current = SceneName;
            }
            SceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            AppTools.Send<SceneBase>((int)SceneEvent.AddSceneBase,this);
            if (SceneName != AppConfig.RoleScene) {
                if (m_Default == null) {
                    Debuger.LogError($"{m_CharName}场景没有设置默认点！！！");
                    return;
                }
                if (AppTools.GetModule<MainUIModule>(MDef.MainUIModule) == null) {
                    ///主界面模块为空，说明是第一次进入游戏
                    AppTools.CreateModule<MainUIModule>(MDef.MainUIModule);
                }
                AppTools.Send<Transform>((int)MoveEvent.SetMoveObjToScene, m_Default);
            }
            if (m_BgMusic != null) {
                AudioService.GetInstance.PlayBGMusic(m_BgMusic);
            } else {
                AudioService.GetInstance.StopBGAudio();
            }
            //发送服务器切换场景角色,这里改成先切场景，场景切换成功后，再发送
            if (SceneName!= "CreateRole") {//排除一下创建角色的场景
                ClientManager.Instance.SendRT("SwitchScene", SceneName);
            }
        }
        public virtual void OnDestroy() {
            AudioService.GetInstance.StopBGAudio();
        }
    }
    
}
 
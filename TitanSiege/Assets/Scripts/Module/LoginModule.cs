/****************************************************
    文件：LoginModule.cs
	作者：昔莘
    邮箱: 304183153@qq.com
    日期：2021/6/12 22:20:50
	功能：登录模块
*****************************************************/
using GF.MainGame.UI;
using GF.Module;
using GF.Service;
using GF.Unity.Audio;
using GF.Unity.UI;
using UnityEngine;

namespace GF.MainGame.Module {
    public class LoginModule : GeneralModule {
        LoginUIPage loginpage;
        /// <summary>
        /// 创建模块
        /// </summary>
        /// <param name="args">参数</param>
        public override void Create() {
            Debuger.Log("Login模块被创建了");
            AppTools.Regist<string, string, string>((int)LoginEvent.Register, SendRegisterMessage);
            AppTools.Regist<string, string>((int)LoginEvent.Login, SendLoginMessage);
            Show();
            //播放音乐
            AudioService.GetInstance.PlayBGMusic("audio/bgMainCity.mp3",true);
            loginpage = UIManager.GetInstance.GetUI(AppConfig.LoginUIPage) as LoginUIPage;
            UserService.GetInstance.OnRegister += RegisterSuccess;
            UserService.GetInstance.OnLogin += LoginSuccess;
        }
        /// <summary>
        /// 显示UI
        /// </summary>
        /// <param name="arg">参数</param>
        public override void Show() {
            UIManager.GetInstance.OpenPage(AppConfig.LoginUIPage);
        }
        
        public override void Release() {
            AudioService.GetInstance.StopBGAudio();
            base.Release();
           
        }
        private string m_Account;
        public void SendLoginMessage(string username,string password) {
            UserService.GetInstance.SendLogin(username, password);
            m_Account = username;
        }
        public void SendRegisterMessage(string username, string password,string eamil) {
            UserService.GetInstance.SendRegister(username, password,eamil);
        }
        public void RegisterSuccess(bool state,string errormsg) {

            if (state) {
                //处理UI，打开登录界面
                UIManager.GetInstance.OpenUIWidget(AppConfig.UIMsgTips, "恭喜你，注册成功！");
                loginpage.ResetAndCloseRegister();

            } else {
                loginpage.ShowRegisterNotice(errormsg);
            }
            
        }
        public void LoginSuccess(bool state, string errormsg) {

            if (state) {
                //处理UI，登录场景加载
                PlayerPrefs.SetString("account", m_Account);//存储本地
                loginpage.ShowLoading();
            } else {
                loginpage.ShowNotice(errormsg);
            }

        }
    }
}

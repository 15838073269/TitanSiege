/****************************************************
    �ļ���LoginModule.cs
	���ߣ���ݷ
    ����: 304183153@qq.com
    ���ڣ�2021/6/12 22:20:50
	���ܣ���¼ģ��
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
        /// ����ģ��
        /// </summary>
        /// <param name="args">����</param>
        public override void Create() {
            Debuger.Log("Loginģ�鱻������");
            AppTools.Regist<string, string, string>((int)LoginEvent.Register, SendRegisterMessage);
            AppTools.Regist<string, string>((int)LoginEvent.Login, SendLoginMessage);
            Show();
            //��������
            AudioService.GetInstance.PlayBGMusic("audio/bgMainCity.mp3",true);
            loginpage = UIManager.GetInstance.GetUI(AppConfig.LoginUIPage) as LoginUIPage;
            UserService.GetInstance.OnRegister += RegisterSuccess;
            UserService.GetInstance.OnLogin += LoginSuccess;
        }
        /// <summary>
        /// ��ʾUI
        /// </summary>
        /// <param name="arg">����</param>
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
                //����UI���򿪵�¼����
                UIManager.GetInstance.OpenUIWidget(AppConfig.UIMsgTips, "��ϲ�㣬ע��ɹ���");
                loginpage.ResetAndCloseRegister();

            } else {
                loginpage.ShowRegisterNotice(errormsg);
            }
            
        }
        public void LoginSuccess(bool state, string errormsg) {

            if (state) {
                //����UI����¼��������
                PlayerPrefs.SetString("account", m_Account);//�洢����
                loginpage.ShowLoading();
            } else {
                loginpage.ShowNotice(errormsg);
            }

        }
    }
}

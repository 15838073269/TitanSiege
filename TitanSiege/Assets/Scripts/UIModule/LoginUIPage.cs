/****************************************************
    文件：LoginUI.cs
	作者：昔莘
    邮箱: 304183153@qq.com
    日期：2021/6/7 22:20:50
	功能：登录模块的主UI
*****************************************************/
using GF.Unity.UI;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using GF.Service;
using GF.Unity.Audio;
using GF.MainGame.Module;

namespace GF.MainGame.UI {
    public class LoginUIPage : UIPage {
        //loginpage
        public Button m_LoginBtn;
        public InputField m_account;
        public InputField m_password;
        public GameObject m_LoginPage;
        public Text loginnotice;
        //Registerpage
        public Button m_RegisterBtn;
        public InputField m_RegisterAccount;
        public InputField m_RegisterPassword;
        public InputField m_Repassword;
        public InputField m_Eamil;
        public GameObject m_RegisterPage;
        public Button m_CanelRegisterBtn;
        public Button m_RegisterSendBtn;
        public Text Registernotice;
        //loading
        public Slider m_Slider;
        public GameObject m_Loadpage;
        private bool m_IsLoading = false;
        protected override void OnOpen(object args = null) {
            base.OnOpen(args);
            m_LoginBtn.onClick.AddListener(LoginGame);
            m_LoginPage.transform.DOLocalMoveY(-270f,1f);
            m_RegisterBtn.onClick.AddListener(OpenReGister);
            m_CanelRegisterBtn.onClick.AddListener(CanelRegister);
            m_RegisterSendBtn.onClick.AddListener(RegisterSend);
            loginnotice.gameObject.SetActive(false);
            Registernotice.gameObject.SetActive(false);
            if (PlayerPrefs.HasKey("account")) {
                m_account.text = PlayerPrefs.GetString("account");
            }
        }
        
        private void LoginGame() {
             AppTools.PlayBtnClick();
            //判断账号密码是否合法
            if (CheckAccountInfo(m_account.text, m_password.text)) {
                //禁止按键，防止连点
                m_LoginBtn.interactable = false;
                //检查通过，连接服务器，发送登录
                AppTools.Send<string, string>((int)LoginEvent.Login, m_account.text, m_password.text);
                //延迟处理，防止连点
                Invoke("InvokeLoginBtn", 1.5f);
            }
        }
        private void InvokeLoginBtn() {
            m_LoginBtn.interactable = true;
        }
        private void RegisterSend() {
            AppTools.PlayBtnClick();
            //判断账号密码是否合法
            if(CheckAccountInfo(m_RegisterAccount.text, m_RegisterPassword.text, m_Repassword.text, m_Eamil.text)){
                //禁止按键，防止连点
                m_RegisterBtn.interactable = false;
                //检查通过，连接服务器，发送注册
                AppTools.Send<string, string,string>((int)LoginEvent.Register, m_RegisterAccount.text, m_RegisterPassword.text, m_Eamil.text);
                //延迟处理，防止连点
                Invoke("InvokeRegisterSendBtn", 1.5f);
            }
        }
        private void InvokeRegisterSendBtn() {
            m_RegisterSendBtn.interactable = true;
        }
        private void OpenReGister() {
            AppTools.PlayBtnClick();
            m_LoginPage.transform.DOLocalMoveY(979f, 1f);
            m_RegisterPage.transform.DOLocalMoveX(0f, 1f);
            loginnotice.gameObject.SetActive(false);
            Registernotice.gameObject.SetActive(false);
        }
        private void CanelRegister() {
            AppTools.PlayBtnClick();
            m_LoginPage.transform.DOLocalMoveY(-270f, 1f);
            m_RegisterPage.transform.DOLocalMoveX(Screen.width, 1f);
        }
        public void ResetAndCloseRegister() {
            m_RegisterAccount.text = "";
            m_RegisterPassword.text = "";
            m_Repassword.text = "";
            m_Eamil.text = "";
            CanelRegister();
        }
        public void ShowNotice(string msg) {
            loginnotice.gameObject.SetActive(true);
            loginnotice.text = msg;
            loginnotice.transform.DOShakePosition(1, new Vector3(6, 6, 0), 8, 45, true);
        }
        public void ShowRegisterNotice(string msg) {
            Registernotice.gameObject.SetActive(true);
            Registernotice.text = msg;
            Registernotice.transform.DOShakePosition(1, new Vector3(6, 6, 0), 8, 45, true);
        }
        /// <summary>
        /// 检查账号信息是否合法
        /// </summary>
        /// <returns></returns>
        public bool CheckAccountInfo(string _account, string _password) {
            bool IsNull = CheckIsNull(_account, _password);
            if (IsNull == false) {//检查不通过，显示错误提示
                ShowNotice("账号或者密码不能为空！");
                return false;
            } else {
                return true;

            }
        }
        /// <summary>
        /// 检查注册信息是否合法
        /// </summary>
        /// <returns></returns>
        public bool CheckAccountInfo(string _account, string _password,string _repassword,string _email) {
            bool IsNull = CheckIsNull(_account, _password, _repassword, _email);
            if (IsNull == false) {//检查不通过，显示错误提示
                ShowRegisterNotice("所有填写均不允许为空！");
                return false;
            } else {
                if (!_password.Equals(_repassword) ) {
                    ShowRegisterNotice("两次密码输入不相同！");
                    return false;
                }
                //发送邮件验证码
                //todo
                
                return true;
            }
        }
        private bool CheckIsNull(string arg1, string arg2) {
            if (string.IsNullOrEmpty(arg1) || string.IsNullOrEmpty(arg2)) {
                return false;
            }
            return true;
        }
        private bool CheckIsNull(string arg1, string arg2, string arg3, string arg4) {
            if (string.IsNullOrEmpty(arg1) || string.IsNullOrEmpty(arg2)|| string.IsNullOrEmpty(arg3)|| string.IsNullOrEmpty(arg4)) {
                return false;
            }
            return true;
        }
        public void OnDestroy() {
            OnClose();
        }
        protected override void OnClose(bool bClear = false, object arg = null) {
            base.OnClose(bClear, arg);
        }
        /// <summary>
        /// 显示进度条
        /// </summary>
        public void ShowLoading() {
            m_LoginPage.transform.DOLocalMoveY(979f, 1f);
            m_Loadpage.transform.DOLocalMoveY(0f, 1f);
            Invoke("SetProgress", 1f);
        }
        public void SetProgress() {
            GameSceneService.GetInstance.SceneLoadedOver = OnLoadOver;
            GameSceneService.GetInstance.AsyncLoadScene(AppConfig.RoleScene,false);
            m_IsLoading = true;
        }
        protected override void OnUpdate() {
            base.OnUpdate();
            if (m_IsLoading) {
                m_Slider.value = (float)GameSceneService.LoadingProgress / 100f;
            }

        }
        public void OnLoadOver(object arg) {
            m_IsLoading = false;
            m_LoginPage.transform.DOLocalMoveY(-270f, 1f);
            m_Loadpage.transform.DOLocalMoveY(-1166f, 1f);
            m_Slider.value = 0f;
            GameSceneService.GetInstance.SceneLoadedBegain = null;
            Close();
        }
    }
}

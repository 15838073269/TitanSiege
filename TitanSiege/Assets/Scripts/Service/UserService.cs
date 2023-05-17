/****************************************************
    文件：UserService.cs
	作者：昔莘
    邮箱: 304183153@qq.com
    日期：2022/10/4 0:20:12
	功能：Nothing
*****************************************************/
using UnityEngine;
using Net.Component;
using Net.Share;
using GF.Model;
using System.Collections.Generic;
using GF.MainGame;
using GF.Unity.UI;
using System.Threading.Tasks;
using GF.MainGame.Module.NPC;
using Titansiege;

namespace GF.Service {
    public class UserService : Utils.Singleton<UserService> {
        public UserModel m_UserModel;//账户数据
        public CharactersData m_CurrentChar = null;//当前使用的角色数据
        public int m_CurrentID = -1;//当前使用角色的本地配置表id,用来加载模型和头像
        public Player m_CurrentPlayer=null;//当前控制的角色
        public UnityEngine.Events.UnityAction<bool, string> OnLogin;
        public UnityEngine.Events.UnityAction<bool, string> OnRegister;
        public UnityEngine.Events.UnityAction<bool, string> OnCharacterCreate;
        bool connected = false;
        bool isQuitGame = false;

        public UserService() {
            Init();
        }

        public void Dispose() {
          
        }

        public void Init() {
            m_UserModel = new UserModel();
            ClientManager.Instance.client.OnConnectedHandle += OnGameServerConnect;
        }
        public async Task<bool> Reconnect() {
            if (connected == false) {
               bool isconnect = await ClientManager.Instance.Connect();
                if (isconnect== false) {
                    UIMsgBoxArg arg = new UIMsgBoxArg();
                    arg.title = "重要提示";
                    arg.content = "网络连接错误，可能是服务器未启动，请稍后重试，或者联系管理员！";
                    arg.btnname = "关闭游戏";
                    UIMsgBox box = UIManager.GetInstance.OpenWindow(AppConfig.UIMsgBox, arg) as UIMsgBox;
                    box.oncloseevent += OnFailed;
                }
                return isconnect;
            }
            return true;
        }
         private void OnFailed(object args = null) {
            Application.Quit();
        }
        void OnGameServerConnect() {//连接成功才会调用，不成功不调用
            this.connected = true;
            ClientManager.Instance.client.AddRpc(this);
        }

        public void OnGameServerDisconnect(int result, string reason) {
            //this.DisconnectNotify(result, reason);
            return;
        }
        
        
        public async void SendLogin(string user, string psw) {
            if (connected == false) {
               await Reconnect();
            }
            ClientManager.Instance.client.SendRT("Login", user, psw);
        }
        [Rpc]
        void LoginCallback(bool state, string msg,List<CharactersData> cdatalist = null,UsersData udata = null) {
            Debug.LogFormat("OnLogin:{0} [{1}]", state, msg);
            if (state&& cdatalist != null) {//登陆成功逻辑
                m_UserModel.m_CharacterList= cdatalist;
                m_UserModel.m_Users = udata;
            };
            if (this.OnLogin != null) {
                this.OnLogin(state, msg);

            }
        }
        [Rpc]
        void LoginFailCallback(bool state, string msg) {
            Debug.LogFormat("OnLogin:{0} [{1}]", state, msg);
            if (this.OnLogin != null) {
                this.OnLogin(state, msg);

            }
        }
        public async void SendRegister(string user, string psw,string email) {
            if (connected == false) {
                await Reconnect();
            }
            ClientManager.Instance.client.SendRT("Register", user, psw, email);
        }
        [Rpc]
        void RegisterCallback(bool state, string msg) {
            Debug.LogFormat("OnUserRegister:{0} [{1}]", state, msg);
            if (this.OnRegister != null) {
                this.OnRegister(state, msg);

            }
        }


    }
   
}


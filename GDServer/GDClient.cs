using GameDesigner;
using GDServer.Services;
using Net.Component;
using Net.Server;
using Net.Share;
using System.Security.Cryptography;

namespace GDServer {
    public class GDClient : NetPlayer//客户端类
    {
        internal NTransform transform = new NTransform();
        internal bool isDead;
        internal GDScene scene;
        public List<CharactersData>? CharacterList = new List<CharactersData>();
        public CharactersData current;//当前登录角色
        public UsersData? User = new UsersData();

        public override void OnEnter() {
            isDead = false;
            scene = Scene as GDScene;
        }

        public override void OnUpdate() {
            //scene.AddOperation(new Operation(GDCommand.PlayerState, UserID) {
            //    index1 = (int)current.Shengming
            //});
        }

        internal void BeAttacked(int damage) {
            if (isDead)
                return;
            current.Shengming -= (short)damage;
            if (current.Shengming <= 0) {
                isDead = true;
                current.Shengming = 0;
            }
        }

        public void Resurrection() {
            //数据库中不设置最大生命，而是根据等级+装备来计算
            //current.Shengming = data.HealthMax;
            isDead = false;
        }
        public override void Dispose() {
            base.Dispose();
        }

        public override void OnExit() {
            base.OnExit();
        }

        public override void OnRemove() {
            base.OnRemove();
        }

        public override void OnRemoveClient() {
            base.OnRemoveClient();
            
        }

        public override void OnSignOut() {
            base.OnSignOut();
        }
        /// <summary>
        /// 登录成功后才会调用
        /// </summary>
        public override void OnStart() {
            base.OnStart();
            
        }
        
        #region  拓展网络请求发送方法
        public void Send(byte[] buffer) {
            TServer.Instance.Send(this, buffer);
        }

        public void Send(byte cmd, byte[] buffer) {
            TServer.Instance.Send(this, cmd, buffer);
        }

        public void Send(string func, params object[] pars) {
            TServer.Instance.Send(this, func, pars);
        }

        public void Send(byte cmd, string func, params object[] pars) {
            TServer.Instance.Send(this, cmd, func, pars);
        }

        public void CallRpc(string func, params object[] pars) {
            TServer.Instance.Send(this, func, pars);
        }

        public void CallRpc(byte cmd, string func, params object[] pars) {
            TServer.Instance.Send(this, cmd, func, pars);
        }

        public void Request(string func, params object[] pars) {
            TServer.Instance.Send(this, func, pars);
        }

        public void Request(byte cmd, string func, params object[] pars) {
            TServer.Instance.Send(this, cmd, func, pars);
        }

        public void SendRT(string func, params object[] pars) {
            TServer.Instance.SendRT(this, func, pars);
        }

        public void SendRT(byte cmd, string func, params object[] pars) {
            TServer.Instance.SendRT(this, cmd, func, pars);
        }

        public void SendRT(byte[] buffer) {
            TServer.Instance.SendRT(this, buffer);
        }

        public void SendRT(byte cmd, byte[] buffer) {
            TServer.Instance.SendRT(this, cmd, buffer);
        }
        #endregion
    }
}

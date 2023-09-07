/****************************************************
    文件：DieUIWindow.cs
	作者：昔莘
    邮箱: 304183153@qq.com
    日期：2023/9/5 11:43:51
	功能：Nothing
*****************************************************/

using GF.Unity.UI;
using Net.Client;
using UnityEngine;
using UnityEngine.UI;

namespace GF.MainGame.UI {
    public class DieUIWindow : UIWindow {
        public Button m_FuhuoBtn;
        public void Start() {
            m_FuhuoBtn.onClick.AddListener(()=> { Fuhuo(0); });
        }
        protected override void OnOpen(object args = null) {
            base.OnOpen(args);
        }
        public override void Close(bool bClear = false, object arg = null) {
            base.Close(bClear, arg);
        }

        protected override void OnClose(bool bClear = false, object arg = null) {
            base.OnClose(bClear, arg);
        }
        /// <summary>
        /// 玩家复活
        /// 这里给多种复活方式预留了功能
        /// 目前只有复活0，也就是在传送点复活
        /// 后面可以加上原地复活等
        /// </summary>
        public void Fuhuo(int i) {
            
            //发送复活消息给NPCModlue,这里的按钮本机只能复活本机玩家
            AppTools.Send<int,int>((int)NpcEvent.FuhuoPlayer, ClientBase.Instance.UID,i);
        }

    }
}
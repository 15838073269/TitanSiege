/****************************************************
    文件：DieUIWindow.cs
	作者：昔莘
    邮箱: 304183153@qq.com
    日期：2023/9/5 11:43:51
	功能：Nothing
*****************************************************/

using GF.Unity.UI;
using UnityEngine;
namespace GF.MainGame.UI {
    public class DieUIWindow : UIWindow {
        protected override void OnOpen(object args = null) {
            base.OnOpen(args);
        }
        public override void Close(bool bClear = false, object arg = null) {
            base.Close(bClear, arg);
        }

        protected override void OnClose(bool bClear = false, object arg = null) {
            base.OnClose(bClear, arg);
        }


    }
}
using GF.Unity.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace GF.MainGame.UI {
    public class HpUIPage : UIPage {
        public override void Close(bool bClear = false, object arg = null) {
            base.Close(bClear, arg);
        }

        protected override void OnClose(bool bClear = false, object arg = null) {
            base.OnClose(bClear, arg);
        }

        protected override void OnOpen(object args = null) {
            base.OnOpen(args);
        }
    }
}


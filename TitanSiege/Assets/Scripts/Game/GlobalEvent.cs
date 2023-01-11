using System;

namespace GF.MainGame {
    public static class GlobalEvent {
        public static Action<bool> OnLogin;//登陆的事件
        public static Action OnUpdate;//update事件，可以让没有继承mono的脚本使用update
        public static Action OnFixedUpdate;//FixedUpdate事件，可以让没有继承mono的脚本使用FixedUpdata
    }
}

namespace GF.Unity.UI {
    /// <summary>
    /// 定义UI的类型
    /// </summary>
    public enum UITypeDef {
        /// <summary>
        /// 未知类型
        /// </summary>
        Unkown = 0,
        /// <summary>
        /// 页面类型
        /// </summary>
        Page = 1,
        /// <summary>
        /// 窗口类型
        /// </summary>
        Window = 2,
        /// <summary>
        /// 挂件类型
        /// </summary>
        Widget = 3,
        /// <summary>
        /// 加载页面类型
        /// </summary>
        Loading = 4
    }
    /// <summary>
    /// 定义UI的layer层级，层级从小到大排列，越大越在上面
    /// </summary>
    public class UILayerDef {
        /// <summary>
        ///背景放在最底层
        /// </summary>
        public const int BackGround = 0;
        /// <summary>
        ///-1999 页面层
        /// </summary>
        public const int Page = 1000;
        /// <summary>
        ///正常的窗口
        /// </summary>
        public const int NormalWindow = 2000;
        /// <summary>
        ///顶部的窗口，主要用在MsgBox
        /// </summary>
        public const int TopWindow = 3000;
        /// <summary>
        /// 挂件，一般用于轮播消息
        /// </summary>
        public const int Widget =4000;
        /// <summary>
        /// 加载页面
        /// </summary>
        public const int Loading = 5000;
        /// <summary>
        /// 未知UI
        /// </summary>
        public const int Unkown = 9999;
        /// <summary>
        /// 获取UI类型的默认层级
        /// </summary>
        /// <param name="type">ui类型</param>
        /// <returns></returns>
        public static int GetDefaultLayer(UITypeDef type) {
            switch (type) {
                case UITypeDef.Page:
                    return Page;
                case UITypeDef.Widget:
                    return Widget;
                case UITypeDef.Window:
                    return NormalWindow;
                case UITypeDef.Loading:
                    return Loading;
                case UITypeDef.Unkown:
                    return Unkown;
                default:
                    return Unkown;
            }
        }
    }
}

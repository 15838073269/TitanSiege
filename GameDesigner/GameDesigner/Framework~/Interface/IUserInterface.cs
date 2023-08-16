using System;

namespace Framework 
{
    /// <summary>
    /// 界面接口
    /// </summary>
    public interface IUserInterface
    {
        /// <summary>
        /// 显示一个ui界面
        /// </summary>
        /// <param name="onBack"></param>
        void ShowUI(Delegate onBack = null, params object[] pars);
        /// <summary>
        /// 屏幕中间提示
        /// </summary>
        /// <param name="info"></param>
        void ShowUI(string info);
        /// <summary>
        /// 显示一个ui界面, 带消息提示信息参数
        /// </summary>
        /// <param name="title"></param>
        /// <param name="info"></param>
        /// <param name="action"></param>
        /// <param name="onBack"></param>
        void ShowUI(string title, string info, Action<bool> action = null, Delegate onBack = null);
        /// <summary>
        /// 显示一个界面, 带进度加载信息 或者 屏幕中间提示延迟显示
        /// </summary>
        /// <param name="title"></param>
        /// <param name="value">如果是加载界面则当作进度值, 如果是信息提示可当作延迟显示</param>
        /// <param name="onBack"></param>
        void ShowUI(string title, float value, Delegate onBack = null);
        /// <summary>
        /// 关闭一个界面, 也就是隐藏界面
        /// </summary>
        /// <param name="isBack"></param>
        void HideUI(bool isBack = false, params object[] pars);
    }
}
using System;

namespace Framework 
{
    /// <summary>
    /// 界面接口
    /// </summary>
    public interface IForm
    {
        /// <summary>
        /// 显示一个ui界面
        /// </summary>
        /// <param name="onBack"></param>
        void ShowUI(Action onBack = null);
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
        void ShowUI(string title, string info, Action<bool> action = null, Action onBack = null);
        /// <summary>
        /// 显示一个界面, 带进度加载信息
        /// </summary>
        /// <param name="title"></param>
        /// <param name="progress"></param>
        /// <param name="onBack"></param>
        void ShowUI(string title, float progress, Action onBack = null);
        /// <summary>
        /// 关闭一个界面, 也就是隐藏界面
        /// </summary>
        /// <param name="isBack"></param>
        void HideUI(bool isBack = false);
    }
}
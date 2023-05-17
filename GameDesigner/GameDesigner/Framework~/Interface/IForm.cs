using System;

namespace Framework 
{
    /// <summary>
    /// ����ӿ�
    /// </summary>
    public interface IForm
    {
        /// <summary>
        /// ��ʾһ��ui����
        /// </summary>
        /// <param name="onBack"></param>
        void ShowUI(Action onBack = null);
        /// <summary>
        /// ��Ļ�м���ʾ
        /// </summary>
        /// <param name="info"></param>
        void ShowUI(string info);
        /// <summary>
        /// ��ʾһ��ui����, ����Ϣ��ʾ��Ϣ����
        /// </summary>
        /// <param name="title"></param>
        /// <param name="info"></param>
        /// <param name="action"></param>
        /// <param name="onBack"></param>
        void ShowUI(string title, string info, Action<bool> action = null, Action onBack = null);
        /// <summary>
        /// ��ʾһ������, �����ȼ�����Ϣ
        /// </summary>
        /// <param name="title"></param>
        /// <param name="progress"></param>
        /// <param name="onBack"></param>
        void ShowUI(string title, float progress, Action onBack = null);
        /// <summary>
        /// �ر�һ������, Ҳ�������ؽ���
        /// </summary>
        /// <param name="isBack"></param>
        void HideUI(bool isBack = false);
    }
}
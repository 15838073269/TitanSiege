using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    /// <summary>
    /// 界面打开操作模式
    /// </summary>
    public enum UIFormMode 
    {
        /// <summary>
        /// 不做任何响应
        /// </summary>
        None,
        /// <summary>
        /// 关闭当前界面, 并打开新的界面
        /// </summary>
        CloseCurrForm,
        /// <summary>
        /// 只隐藏当前界面, 然后打开新的界面
        /// </summary>
        HideCurrForm,
    }


    /// <summary>
    /// UI界面基类
    /// </summary>
    public class UIFormBase : MonoBehaviour, IForm
    {
        public Action onBack;

        public void ShowUI(Action onBack = null)
        {
            gameObject.SetActive(true);
            transform.SetAsLastSibling();
            if (onBack != null)
                this.onBack = onBack;
        }

        public void ShowUI(string info)
        {
            ShowUI(string.Empty, info, null, null);
        }

        public void ShowUI(string title, string info, Action<bool> action, Action onBack = null)
        {
            ShowUI(onBack);
            OnShowUI(title, info, action);
        }

        public void ShowUI(string title, float progress, Action onBack = null)
        {
            ShowUI(onBack);
            OnShowUI(title, progress);
        }

        public void HideUI(bool isBack = true)
        {
            gameObject.SetActive(false);
            if (isBack & onBack != null)
            {
                onBack();
                onBack = null;
            }
        }

        public virtual void OnShowUI(string title, string info, Action<bool> action)
        {
            throw new Exception($"请重写OnShowUI方法处理你的消息提示");
        }

        public virtual void OnShowUI(string title, float progress)
        {
            throw new Exception($"请重写OnShowUI方法处理你的进度加载界面");
        }
    }

    /// <summary>
    /// UI界面基类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class UIFormBase<T> : UIFormBase where T : UIFormBase<T>
    {
        public static T I => Global.UI.GetFormOrCreate<T>();

        public static T Show(Action onBack = null, UIFormMode formMode = UIFormMode.CloseCurrForm)
        {
            var form = Global.UI.OpenForm<T>(onBack, formMode);
            return form;
        }

        public static void Show(string title, string info, Action<bool> result = null)
        {
            var i = Show(null, UIFormMode.None);
            i.OnShowUI(title, info, result);
        }

        public static void Hide(bool isBack = true)
        {
            Global.UI.CloseForm<T>(isBack);
        }
    }

    /// <summary>
    /// UI界面基类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class UIFormBase<T, Item> : UIFormBase<T> where T : UIFormBase<T, Item>
    {
        public Item item;
        public Transform itemRoot;
        public List<Item> items = new List<Item>();
    }

    /// <summary>
    /// UI界面基类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class UIFormBase1<T, Item> : UIFormBase<T> where T : UIFormBase1<T, Item>
    {
        public Item[] item;
        public Transform[] itemRoot;
        public List<Item>[] items = new List<Item>[0];
    }

    /// <summary>
    /// UI界面基类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class NetworkOneUIFormBase<T> : UIFormBase<T> where T : NetworkOneUIFormBase<T>
    {
        public virtual void Awake()
        {
            Global.Network.AddRpcOne(this);
        }

        public virtual void OnDestroy()
        {
            Global.Network.RemoveRpcOne(this);
        }
    }

    /// <summary>
    /// UI界面基类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class NetworkTwoUIFormBase<T> : UIFormBase<T> where T : NetworkTwoUIFormBase<T>
    {
        public virtual void Awake()
        {
            Global.Network.AddRpcTwo(this);
        }

        public virtual void OnDestroy()
        {
            Global.Network.RemoveRpcTwo(this);
        }
    }

    /// <summary>
    /// UI界面基类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class NetworkAllUIFormBase<T> : UIFormBase<T> where T : NetworkAllUIFormBase<T>
    {
        public virtual void Awake()
        {
            Global.Network.AddRpc(-1, this);
        }

        public virtual void OnDestroy()
        {
            Global.Network.RemoveRpc(-1, this);
        }
    }
}
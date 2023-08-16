using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    /// <summary>
    /// 界面打开操作模式
    /// </summary>
    public enum UIMode
    {
        /// <summary>
        /// 不做任何响应
        /// </summary>
        None,
        /// <summary>
        /// 关闭当前界面, 并打开新的界面
        /// </summary>
        CloseCurrUI,
        /// <summary>
        /// 只隐藏当前界面, 然后打开新的界面
        /// </summary>
        HideCurrUI,
    }

    /// <summary>
    /// UI界面基类
    /// </summary>
    public class UIBase : MonoBehaviour, IUserInterface
    {
        public Delegate onBack;

        public void ShowUI(Delegate onBack = null, params object[] pars)
        {
            gameObject.SetActive(true);
            transform.SetAsLastSibling();
            if (onBack != null)
                this.onBack = onBack;
            OnShowUI(pars);
        }

        public void ShowUI(string info)
        {
            ShowUI(string.Empty, info, null, null);
        }

        public void ShowUI(string title, string info, Action<bool> action, Delegate onBack = null)
        {
            ShowUI(onBack);
            OnShowUI(title, info, action);
        }

        public void ShowUI(string title, float progress, Delegate onBack = null)
        {
            ShowUI(onBack);
            OnShowUI(title, progress);
        }

        public void HideUI(bool isBack = true, params object[] pars)
        {
            gameObject.SetActive(false);
            if (isBack & onBack != null)
            {
                onBack?.DynamicInvoke(pars);
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

        public virtual void OnShowUI(params object[] pars) { }
    }

    /// <summary>
    /// UI界面基类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class UIBase<T> : UIBase where T : UIBase<T>
    {
        public static T I => Global.UI.GetFormOrCreate<T>();

        public static T Show(Delegate onBack = null, UIMode formMode = UIMode.CloseCurrUI, params object[] pars)
        {
            var form = Global.UI.OpenForm<T>(onBack, formMode, pars);
            return form;
        }

        public static void Show(string title, string info, Action<bool> result = null)
        {
            var i = Show(null, UIMode.None);
            i.OnShowUI(title, info, result);
        }

        public static void Hide(bool isBack = true, params object[] pars)
        {
            Global.UI.CloseForm<T>(isBack, pars);
        }
    }

    /// <summary>
    /// UI界面基类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class UIBase<T, Item> : UIBase<T> where T : UIBase<T, Item>
    {
        public Item item;
        public Transform itemRoot;
        public List<Item> items = new List<Item>();
    }

    /// <summary>
    /// UI界面基类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class UIBase<T, Item1, Item2> : UIBase<T> where T : UIBase<T, Item1, Item2>
    {
        public Item1 item_1;
        public Item2 item_2;
        public Transform[] itemRoots;
        public List<Item1> item1List = new List<Item1>();
        public List<Item2> item2List = new List<Item2>();
    }

    /// <summary>
    /// UI界面基类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class UIBase1<T, Item> : UIBase<T> where T : UIBase1<T, Item>
    {
        public Item[] item;
        public Transform[] itemRoot;
        public List<Item>[] items = new List<Item>[0];
    }

    /// <summary>
    /// UI界面基类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class NetworkOneUIBase<T> : UIBase<T> where T : NetworkOneUIBase<T>
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
    public class NetworkTwoUIBase<T> : UIBase<T> where T : NetworkTwoUIBase<T>
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
    public class NetworkAllUIBase<T> : UIBase<T> where T : NetworkAllUIBase<T>
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

    public class UIFormBase : UIBase { }
    public class UIFormBase<T> : UIBase<T> where T : UIFormBase<T> { }
    public class UIFormBase<T, Item> : UIBase<T, Item> where T : UIFormBase<T, Item> { }
    public class UIFormBase1<T, Item> : UIBase1<T, Item> where T : UIFormBase1<T, Item> { }
}
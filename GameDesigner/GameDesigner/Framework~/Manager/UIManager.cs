using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    /// <summary>
    /// ui管理器, 内置两个基础界面 Message和Loading, 请分写此组件定义你的其他界面
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        public Transform UIRoot;
        public Transform[] Levels;
        public string sheetName = "UI";
        public Dictionary<string, UIFormBase> formDict = new Dictionary<string, UIFormBase>();
        public Stack<UIFormBase> formStack = new Stack<UIFormBase>();
        public IForm Loading, Message, Tips;
        [SerializeField] private UIFormBase _Loading;
        [SerializeField] private UIFormBase _Message;
        [SerializeField] private UIFormBase _Tips;

        public virtual void Awake()
        {
            Loading = _Loading;
            Message = _Message;
            Tips = _Tips;
            AddForm(_Loading); 
            AddForm(_Message);
            AddForm(_Tips);
        }

        /// <summary>
        /// 将form界面添加到所有界面字典里面
        /// </summary>
        /// <param name="form"></param>
        public void AddForm(UIFormBase form)
        {
            formDict[form.GetType().Name] = form;
        }

        /// <summary>
        /// 打开一个界面, 如果不存在则从ab包加载, 如果存在直接显示并返回
        /// </summary>
        /// <typeparam name="T">ui表列的名称</typeparam>
        /// <param name="onBack">当关闭界面后回调</param>
        /// <param name="formMode">当前界面响应模式</param>
        /// <returns></returns>
        public T OpenForm<T>(Delegate onBack = null, UIFormMode formMode = UIFormMode.CloseCurrForm) where T : UIFormBase<T>
        {
            var formName = typeof(T).Name;
            return OpenForm(formName, onBack, formMode) as T;
        }

        /// <summary>
        /// 打开一个界面, 如果不存在则从ab包加载, 如果存在直接显示并返回
        /// </summary>
        /// <typeparam name="T">ui表列的名称</typeparam>
        /// <param name="onBack">当关闭界面后回调</param>
        /// <param name="formMode">当前界面响应模式</param>
        /// <returns></returns>
        public void OpenForm(UIFormBase uiForm, Delegate onBack = null, UIFormMode formMode = UIFormMode.CloseCurrForm)
        {
            var formName = uiForm.GetType().Name;
            OpenForm(formName, onBack, formMode);
        }

        /// <summary>
        /// 打开一个界面, 如果不存在则从ab包加载, 如果存在直接显示并返回
        /// </summary>
        /// <param name="formName">ui表列的名称</param>
        /// <param name="onBack">当关闭界面后回调</param>
        /// <param name="formMode">当前界面响应模式</param>
        /// <returns></returns>
        public UIFormBase OpenForm(string formName, Delegate onBack = null, UIFormMode formMode = UIFormMode.CloseCurrForm)
        {
            if (formDict.TryGetValue(formName, out var form))
                if (form != null)
                    goto J;
            form = InstantiateForm(formName);
            formDict[formName] = form;
        J: if (formStack.Count > 0)
            {
                UIFormBase form1;
                switch (formMode)
                {
                    case UIFormMode.HideCurrForm:
                        form1 = formStack.Peek();//只是隐藏当前界面不能弹出
                        form1.HideUI(false);
                        break;
                    case UIFormMode.CloseCurrForm:
                        form1 = formStack.Pop();//关闭上一个界面需要弹出
                        form1.HideUI(false);
                        break;
                    case UIFormMode.None://不做任何动作, Message消息框
                        break;
                }
            }
            form.ShowUI(onBack);
            form.transform.SetAsLastSibling();
            formStack.Push(form);//如果是消息框, 一定会关闭了才能再次打开, 不存在多次压入
            return form;
        }

        private UIFormBase InstantiateForm(string formName) 
        {
            var dataTable = Global.Table.GetTable(sheetName);
            var dataRows = dataTable.Select($"Name = '{formName}'");
            if (dataRows.Length == 0)
                throw new Exception($"找不到界面:{formName}, 请配置!");
            var path = ObjectConverter.AsString(dataRows[0]["Path"]);
            var level = ObjectConverter.AsInt(dataRows[0]["Level"]);
            var form = Global.Resources.Instantiate<UIFormBase>(path, Levels[level]);
            return form;
        }

        public void CloseForm<T>(bool isBack, params object[] pars)
        {
            var formName = typeof(T).Name;
            CloseForm(formName, isBack, pars);
        }

        public void CloseForm(string formName, bool isBack = true, params object[] pars)
        {
            if (formDict.TryGetValue(formName, out var form))
            {
                CloseForm(form, isBack, pars);
            }
        }

        public void CloseForm(UIFormBase form, bool isBack = true, params object[] pars)
        {
            if (formStack.Count > 0)
            {
                if (form != formStack.Peek())
                {
                    form.HideUI(isBack, pars);
                    return;
                }
                form = formStack.Pop();//弹出自己的面板
                form.HideUI(isBack, pars);
                if (formStack.Count > 0)
                {
                    form = formStack.Peek();//弹出上一个界面进行显示, 但不会移除
                    form.ShowUI();
                    form.transform.SetAsLastSibling();
                }
            }
            else 
            {
                form.HideUI(isBack, pars);
            }
        }

        public T GetForm<T>() where T : UIFormBase
        {
            var formName = typeof(T).Name;
            return GetForm(formName) as T;
        }

        public UIFormBase GetForm(string formName) 
        {
            formDict.TryGetValue(formName, out var form);
            return form;
        }

        public T GetFormOrCreate<T>(bool isShow = false) where T : UIFormBase
        {
            var formName = typeof(T).Name;
            return GetFormOrCreate(formName, isShow) as T;
        }

        public UIFormBase GetFormOrCreate(string formName, bool isShow = false)
        {
            var form = GetForm(formName);
            if (form != null)
                return form;
            form = InstantiateForm(formName);
            form.gameObject.SetActive(isShow);
            return form;
        }
    }
}
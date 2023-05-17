using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    /// <summary>
    /// ui������, ���������������� Message��Loading, ���д��������������������
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
        /// ��form������ӵ����н����ֵ�����
        /// </summary>
        /// <param name="form"></param>
        public void AddForm(UIFormBase form)
        {
            formDict[form.GetType().Name] = form;
        }

        /// <summary>
        /// ��һ������, ������������ab������, �������ֱ����ʾ������
        /// </summary>
        /// <typeparam name="T">ui���е�����</typeparam>
        /// <param name="onBack">���رս����ص�</param>
        /// <param name="formMode">��ǰ������Ӧģʽ</param>
        /// <returns></returns>
        public T OpenForm<T>(Action onBack = null, UIFormMode formMode = UIFormMode.CloseCurrForm) where T : UIFormBase<T>
        {
            var formName = typeof(T).Name;
            return OpenForm(formName, onBack, formMode) as T;
        }

        /// <summary>
        /// ��һ������, ������������ab������, �������ֱ����ʾ������
        /// </summary>
        /// <param name="formName">ui���е�����</param>
        /// <param name="onBack">���رս����ص�</param>
        /// <param name="formMode">��ǰ������Ӧģʽ</param>
        /// <returns></returns>
        public UIFormBase OpenForm(string formName, Action onBack = null, UIFormMode formMode = UIFormMode.CloseCurrForm)
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
                        form1 = formStack.Peek();//ֻ�����ص�ǰ���治�ܵ���
                        form1.HideUI(false);
                        break;
                    case UIFormMode.CloseCurrForm:
                        form1 = formStack.Pop();//�ر���һ��������Ҫ����
                        form1.HideUI(false);
                        break;
                    case UIFormMode.None://�����κζ���, Message��Ϣ��
                        break;
                }
            }
            form.ShowUI(onBack);
            form.transform.SetAsLastSibling();
            formStack.Push(form);//�������Ϣ��, һ����ر��˲����ٴδ�, �����ڶ��ѹ��
            return form;
        }

        private UIFormBase InstantiateForm(string formName) 
        {
            var dataTable = Global.Table.GetTable(sheetName);
            var dataRows = dataTable.Select($"Name = '{formName}'");
            if (dataRows.Length == 0)
                throw new Exception($"�Ҳ�������:{formName}, ������!");
            var path = ObjectConverter.AsString(dataRows[0]["Path"]);
            var level = ObjectConverter.AsInt(dataRows[0]["Level"]);
            var form = Global.Resources.Instantiate<UIFormBase>(path, Levels[level]);
            return form;
        }

        public void CloseForm<T>(bool isBack)
        {
            var formName = typeof(T).Name;
            CloseForm(formName, isBack);
        }

        public void CloseForm(string formName, bool isBack = true)
        {
            if (formDict.TryGetValue(formName, out var form))
            {
                CloseForm(form, isBack);
            }
        }

        public void CloseForm(UIFormBase form, bool isBack = true)
        {
            if (formStack.Count > 0)
            {
                if (form != formStack.Peek())
                {
                    form.HideUI(isBack);
                    return;
                }
                form = formStack.Pop();//�����Լ������
                form.HideUI(isBack);
                if (formStack.Count > 0)
                {
                    form = formStack.Peek();//������һ�����������ʾ, �������Ƴ�
                    form.ShowUI();
                    form.transform.SetAsLastSibling();
                }
            }
            else 
            {
                form.HideUI(isBack);
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
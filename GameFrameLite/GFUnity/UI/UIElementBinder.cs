using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace GF.Unity.UI { 
    /// <summary>
    /// UI元素绑定脚本，通过反射给某个字段赋值
    /// 用来处理字段拖拽不严谨的问题，不过反射效率低，不建议使用，小的项目上，不如拖拽方便效率高。
    /// </summary>
    public class UIElementAttribute : Attribute
    {
        
    }

    public class UIElementBinder
    {
        /// <summary>
        /// 绑定UI元素
        /// </summary>
        /// <param name="parent">继承mono的实例</param>
        public static void BindAllUIElement(MonoBehaviour parent)
        {
            //通过反射获取mono所有的实例
            var fis = parent.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            //遍历实例
            for (int i = 0; i < fis.Length; i++)
            {
                var fi = fis[i];
                //在派生类中重写时，返回应用于此成员并由 Type 标识的自定义属性的数组。
                //此处是用来判断是否为parent的子物体
                var tokens = fi.GetCustomAttributes(typeof(UIElementAttribute), true);
                if (tokens.Length > 0)
                {
                    BindUIElement(parent, fi);
                }
            }
        }
        /// <summary>
        /// 将子物体中的某个物体实例的类绑定到父类上
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="fi">FieldInfo是反射应用中的类，它表示某个对象公共字段的信息。</param>
        private static void BindUIElement(MonoBehaviour parent, FieldInfo fi)
        {
            Transform element = parent.transform.Find(fi.Name);
            string uiName = fi.Name;
            if (element == null)
            {
                if (uiName.StartsWith("m_"))
                {
                    uiName = uiName.Substring(2);
                    element = parent.transform.Find(uiName);
                }
                else if (uiName.StartsWith("_"))
                {
                    uiName = uiName.Substring(1);
                    element = parent.transform.Find(uiName);
                }
            }

            if (element == null)
            {
                var c = uiName[0];
                c = Char.IsLower(c) ? Char.ToUpper(c) : Char.ToLower(c);
                uiName = c + uiName.Substring(1);
                element = parent.transform.Find(uiName);
            }


            if (element != null)
            {
                var value = element.GetComponent(fi.FieldType);
                //通过FieldInfo.SetValue设置任何字段的值
                fi.SetValue(parent, value);
            }
            else
            {
                Debuger.LogError("Canot Find UIElement:{0}", fi.Name);
            }
        }

        public static void BindUIElement(MonoBehaviour parent, string uiName)
        {
            var fis = parent.GetType().GetFields();
            for (int i = 0; i < fis.Length; i++)
            {
                var fi = fis[i];
                if (fi.Name == uiName)
                {
                    BindUIElement(parent, fi);    
                }
            }
        }
    }
}
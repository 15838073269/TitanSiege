using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GF.Unity.UI {
    /// <summary>
    /// UI的根节点类，一般挂在Canvas上的，用来寻找各个UI
    /// </summary>
    public class UIRoot:MonoBehaviour {
        public Canvas m_Canvas;
        public void Awake() {
            m_Canvas = transform.GetComponent<Canvas>();
            DontDestroyOnLoad(this);
            //UIManager.GetInstance.m_UIRoot = this;
            //DontDestroyOnLoad(UIManager.GetInstance.m_EventSystem);
        }
        /// <summary>
        /// 方便寻找某个物体下子物体上的UI控件，一般用来寻找本物体下的子物体，不常用
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="UIName">UI名称</param>
        /// <returns></returns>
        public T FindUIInChild<T>(Transform trans,string UIName) where T : MonoBehaviour {
            Transform target = trans.transform.Find(UIName);
            if (target != null) {
                return target.GetComponent<T>();
            } else {
                Debuger.LogError("未找到UI控件：{0}", UIName);
                return default(T);
            }
        }
        /// <summary>
        /// 从uiroot下根据名称和类型寻找一个组件
        /// </summary>
        /// <typeparam name="T">组件类型，例如:button</typeparam>
        /// <param name="name">组件名称</param>
        /// <returns></returns>
        public T FindUI<T>(string name) where T:MonoBehaviour{
            if (string.IsNullOrEmpty(name)) {
                return null;
            }
            GameObject obj = FindUI(name);
            if (obj != null) {
                return obj.GetComponent<T>();
            }
            return null;
        }
        /// <summary>
        /// 根据名称寻找UI
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public GameObject FindUI(string name) {
            if (string.IsNullOrEmpty(name)) {
                return null;
            }
            Transform trans = null;
            //GameObject root = FindUIRoot();
            Transform t = this.gameObject.transform;
            for (int i = 0; i < t.childCount; i++) {//通过子类循环寻找对应UI
                Transform c = t.GetChild(i);
                if (c.name == name) {
                    trans = c;
                    break;
                }
            }
            if (trans != null) {
                return trans.gameObject;
            }
            //ui不存在
            return null;
        }
        /// <summary>
        /// 寻找当前场景中的UIRoot
        /// </summary>
        /// <returns></returns>
        public GameObject FindUIRoot() {
            /////GameObject root = GameObject.Find("UIRoot"); //性能太低
            //if (root!=null) {
            //    return root;
            //}
            //root = GameObject.FindGameObjectWithTag("UIRoot"); //换成tag的方式，但需要在面板中加上tag值
            //UIRoot  temproot = root.GetComponent<UIRoot>() ;
            //if (root!=null  && temproot!= null) {//这个通过tag找到的UIroot必须包含UIRoot脚本
            //    return root;
            //}
            //Debuger.LogError("场景内没有UIRoot");
            //return null;
            return this.gameObject;
        }
        /// <summary>
        /// 把一个UIpage/UIWidget/UIWindow添加到UIroot下
        /// </summary>
        /// <param name="child">需要添加的UIpage/UIWidget/UIWindow</param>
        public void AddChild(UIPanel child) {
            //GameObject root = FindUIRoot();
            //if (root==null || child == null) {
            //    return;
            //}
            //worldPositionStays设置为false将会将GameObject移动到它的新parent对象同样位置。worldpositionstay参数的默认值为true。
            child.transform.SetParent(this.gameObject.transform,false);
        }
        /// <summary>
        /// UI排序，按照ui的Layer进行排序，方便整理UI界面
        /// 注意调用的地方，正确layer是在open时才赋值的。
        /// </summary>
        public void Sort() {
            //GameObject root = FindUIRoot();
            //if (root == null) {
            //    return;
            //}

            List<UIPanel> list = new List<UIPanel>();
            this.gameObject.GetComponentsInChildren<UIPanel>(true, list);
            list.Sort((a, b) => {
                return a.Layer - b.Layer;
            });

            for (int i = 0; i < list.Count; i++) {
                list[i].transform.SetSiblingIndex(i);
            }
        }

        //==============================================================================
        //UIRoot所在的Canvas的常用逻辑
        //==============================================================================
        //private static Camera m_uiCamera;
        //private static CanvasScaler m_canvasScaler;

        //public Camera UICamera;

        //void Awake() {
        //    //让UIRoot一直存在于所有场景中
        //    DontDestroyOnLoad(gameObject);
        //    m_uiCamera = UICamera;
        //    m_canvasScaler = GetComponent<CanvasScaler>();
        //}

        //public CanvasScaler GetUIScaler() {
        //    return m_canvasScaler;
        //}

        //public Camera GetUICamera() {
        //    return m_uiCamera;
        //}
    }
}

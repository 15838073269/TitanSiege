using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using GF.Utils;
using GF.Unity.AB;
using UnityEngine.EventSystems;
using System.Linq;

//计划改造UI模块，首先改造UIRoot的获取模式，transform.find的性能不是太好，改成加载ui预制体后存在字典中获取的形式
//2、UIRoot的获取方式目前通过tag，而且每次都get一次，这个应该存储起来，然后做成不销毁的
namespace GF.Unity.UI {
    /// <summary>
    /// UI的管理类
    /// </summary>
    public class UIManager : Singleton<UIManager> {
        public const string LOG_TAG = "UIManager";

        public static string MainScene = "MainScene";//主城场景的名称，放到congfig中统一处理
        public static string MainPage = "MainPage";//主城主要UI页面的名称，放到congfig中统一处理
        /// <summary>
        /// 记录UI轨迹的栈，栈的特点，后进先出，用于每个页面的返回上一层
        /// </summary>
        private Stack<UIPageTrack> m_pageTrackStack;
        /// <summary>
        /// 当前轨迹页面
        /// </summary>
        private UIPageTrack m_currenPage;
       
        /// <summary>
        /// 缓存已经加载的UI
        /// </summary>
        private Dictionary<string,UIPanel> m_DicLoadedPanel;
        /// <summary>
        /// UIroot的引用
        /// </summary>
        public UIRoot m_UIRoot;
        public Camera m_UICamera;
        public GameObject m_EventSystem;
        /// <summary>
        /// 无参构造
        /// </summary>
        public UIManager(){
            m_pageTrackStack = new Stack<UIPageTrack>();
            m_DicLoadedPanel = new Dictionary<string, UIPanel>();
        }
        /// <summary>
        /// 初始化方法
        /// </summary>
        public void Init(UIRoot root,Camera uicamera,GameObject eventsystem) {
            CheckSingleton();//检查单例
            m_pageTrackStack.Clear();
            m_DicLoadedPanel.Clear();
            
            //if (m_UIRoot == null) {
            //    m_UIRoot = GameObject.FindGameObjectWithTag("UIRoot").GetComponent<UIRoot>();
            //}
            m_UIRoot = root;
            m_UICamera = uicamera;
            m_EventSystem = eventsystem;
        }
        /// <summary>
        ///清理所有UI
        /// </summary>
        public void ReleaseAll() {
            CloseAllLoadedUI();
            m_pageTrackStack.Clear();
            m_DicLoadedPanel.Clear();
        }
        /// <summary>
        /// 把所有打开过的UI全部关闭
        /// </summary>
        private void CloseAllLoadedUI() {//为了避免某些UI被意外释放，需要先判断一下再close
            foreach (string uiname in m_DicLoadedPanel.Keys) {
                if (m_DicLoadedPanel[uiname] == null) {
                    m_DicLoadedPanel.Remove(uiname);
                } else if (m_DicLoadedPanel[uiname].IsOpen&& (m_DicLoadedPanel[uiname].UIType != UITypeDef.Widget)) {//关闭时，排除一下挂件，挂件特殊，需要手动自行关闭，有的挂件也会自己关。
                    m_DicLoadedPanel[uiname].Close();
                }
            }
        }
        /// <summary>
        /// 加载一个UI页面/窗口/控件
        /// </summary>
        /// <typeparam name="T">ui的类型（页面/窗口/控件）</typeparam>
        /// <param name="name">加载的UI名称</param>
        /// <returns>返回这个ui</returns>
        private T Load<T>(string name, Type implType) where T : UIPanel {
            string scenename = SceneManager.GetActiveScene().name;
            T ui = default(T);
            string loadpath = $"UIPrefab/{name}.prefab";
            GameObject go = ObjectManager.GetInstance.InstanceObject(loadpath,setTranform:true,bClear:false,isfullpath:false,father:m_UIRoot.transform);
            if (go != null) {
                ui = go.gameObject.GetComponent<T>();
                if (ui == null) {
                    try {
                        ui = go.AddComponent(implType) as T;
                    } catch (Exception e) {
                        Debuger.LogError($"无法自动添加抽象的UIPanel,{e.Message}");
                    }
                }
                if (ui != null) {
                    go.name = name;
                } else {
                    Debuger.LogError("UI预制体" + name + "没有对应的ui组件！");
                }
            } else {
                Debuger.LogError("没有名字叫" + name + "的UI预制体！");
            }
            return ui;
        }
        
        /// <summary>
        /// 打开一个ui
        /// </summary>
        /// <typeparam name="T">ui的类型</typeparam>
        /// <param name="name">需要打开的ui名称</param>
        /// <param name="arg">打开ui时传递的参数</param>
        /// <returns>返回打开的ui</returns>
        private T Open<T>(string name, Type implType = null, object arg=null) where T : UIPanel {
            T ui = GetUI<T>(GetShortUIName(name));//去掉名称中的“/”和路径
            if (ui==null) {
                ui = Load<T>(name, implType);//加载时应通过路径加载
            }
            if (ui != null) {
                //加载过ui组件后，判断这个ui是否在缓存list中，不在就添加进去。
                if (!m_DicLoadedPanel.ContainsKey(ui.gameObject.name)) {
                    m_DicLoadedPanel.Add(ui.gameObject.name,ui);
                }
                ui.Open(arg);
                m_UIRoot.Sort();  //按照自定义layer值排序，高的显示在最上面
            } else {
                Debuger.LogError($"没有名字叫{name}的UI，无法打开！");
            }
            return ui;
        }
        /// <summary>
        /// 获取UI的短名称，这个是因为在加载UI时，名称可能会带有路径，需要去除路径
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private string GetShortUIName(string name) {
            int i = name.LastIndexOf("/");
            if (i < 0)
                i = name.LastIndexOf("\\");
            if (i < 0)
                return name;
            return name.Substring(i + 1);
        }
        /// <summary>
        /// 根据名称获取UI,注意这里只能获取加载进来的UI预制体，细节的UI无法获取，非UI预制体需要使用uiroot中的findui来获取
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public UIPanel GetUI(string name) {
            string shortname = GetShortUIName(name);
            //for (int i = 0; i < m_DicLoadedPanel.Count; i++) {
            //    if (m_DicLoadedPanel[i].name == shortname || m_listLoadedPanel[i].name == name) {
            //        return m_DicLoadedPanel[i];
            //    }
            //}
            UIPanel ui = null;
            if (m_DicLoadedPanel.TryGetValue(shortname,out ui)&&ui!=null) {
                return ui;
            }
            return null;
        }
        public T GetUI<T>(string name) where T:UIPanel {
            string shortname = GetShortUIName(name);
            //for (int i = 0; i < m_DicLoadedPanel.Count; i++) {
            //    if (m_DicLoadedPanel[i].name == shortname || m_listLoadedPanel[i].name == name) {
            //        return m_DicLoadedPanel[i];
            //    }
            //}
            UIPanel ui = null;
            if (m_DicLoadedPanel.TryGetValue(shortname, out ui) && ui != null) {
                return ui as T;
            }
            return null;
        }


        #region UILoading管理
        public UILoading OpenLoading(string name, object arg = null) {
            Debuger.Log(name);
            UILoading ui = Open<UILoading>(name, arg: arg);
            return ui;
        }
        public void CloseLoading(string name, bool bClear = false, object arg = null) {
            Debuger.Log(name);
            UILoading ui = GetUI(name) as UILoading;
            if (ui != null) {
                ui.Close(bClear, arg);
            }
        }
        #endregion
        //==========================================================================================
        #region UIpage的管理
        /// <summary>
        /// 打开一个ui页面
        /// </summary>
        /// <param name="page">ui页面名称</param>
        /// <param name="arg">打开时传递的参数</param>
        public UIPage OpenPage(string page,object arg=null,bool ishidden = true) {
            //Debuger.Log(LOG_TAG, "OpenPage(),scene:{0},page:{1},arg:{2}", scene, page, arg);
            if (m_currenPage!=null) {//先把当前页面添加到栈中
                if (m_currenPage.UIName!= page) {//判断一下，当前页面和要打开的一致，不再重复入栈，避免返回时重复返回体验不好，但还是得重复打开，因为参数可能不一样
                    m_pageTrackStack.Push(m_currenPage);
                }
            }
            return OpenPageWorker(page,null, arg, ishidden);
        }
        
        /// <summary>
        /// 返回上一个页面，从栈中获取上一个
        /// </summary>
        public void GoBackPage() {
            //Debuger.Log(LOG_TAG, "GoBackPage()");
            //通过栈判断是否有上一个页面
            if (m_pageTrackStack.Count > 0) {
               UIPageTrack temp = m_pageTrackStack.Pop();
               OpenPageWorker(temp.UIName,temp.type, temp.arg);
            } else if(m_pageTrackStack.Count == 0) {//栈中没有任何缓存，证明没有上一页，可以直接返回主页
               EnterMainPage();
            }
        }
        /// <summary>
        /// 打开某一个UI页面的实际处理方法
        /// </summary>
        /// <param name="scenename">UI所在场景名称</param>
        /// <param name="page">ui页面对的名称</param>
        /// <param name="arg">打开UI时传递的参数</param>
        private UIPage OpenPageWorker(string pagename,Type type,object arg=null,bool ishidden = true) {
            //Debuger.Log(LOG_TAG, "OpenPageWorker(),scene:{0},page:{1},arg:{2}", scenename, pagename, arg);
            //把将要打开的UI的信息赋值给m_currenPage。
            m_currenPage = new UIPageTrack();
            m_currenPage.UIName = pagename;
            m_currenPage.arg = arg;
            m_currenPage.type = type;
            //关闭当前页面的打开的所有UI(包含页面、窗口),但不包括控件，因为控件要么会自消失，要么像摇杆一样需要长存
            if (ishidden) {
                CloseAllLoadedUI();
            } 
           return Open<UIPage>(pagename, type, arg);
        }
        public void ClosePage(string name, bool bClear = false, object arg = null) {
            UIPage ui = GetUI(name) as UIPage;
            if (ui != null) {
                ui.Close(bClear,arg);
            }
        }
        /// <summary>
        /// 进入到主Ui界面，这个情况其实是打开多层Ui后，直接返回主界面，不再通过返回一层一层的返回
        /// </summary>
        public void EnterMainPage() {
            m_pageTrackStack.Clear();//清除所有栈中记录的界面层级数据
            OpenPageWorker(MainPage, null, null);
        }
        #endregion

        #region UIWindow的管理
        /// <summary>
        /// 打开一个ui窗口
        /// </summary>
        /// <param name="windowname">UI窗口的名称</param>
        /// <param name="arg">打开传递的参数</param>
        /// <returns>UIWindow</returns>
        public UIWindow OpenWindow(string windowname, object arg=null) {
            UIWindow tempwindow = Open<UIWindow>(windowname, arg:arg);
            return tempwindow;
        }

        public void CloseWindow(string name, bool bClear = false, object arg = null) {
            UIWindow ui = GetUI(name) as UIWindow;
            if (ui != null) {
                ui.Close(bClear,arg);
            }
        }
        #endregion

        #region UIWidget的管理
        /// <summary>
        /// 打开一个ui控件
        /// </summary>
        /// <param name="Widgetname">UI控件的名称</param>
        /// <param name="arg">打开传递的参数</param>
        /// <returns>UIWidget</returns>
        public UIWidget OpenUIWidget(string widgetname, object arg = null) {
            UIWidget tempwidget = Open<UIWidget>(widgetname, arg:arg);
            return tempwidget;
        }
        
        public void CloseWidget(string name, bool bClear = false, object arg = null) {
            UIWidget ui = GetUI(name) as UIWidget;
            if (ui != null) {
                ui.Close(bClear,arg);
            }
        }
        #endregion
        /// <summary>
        /// ui默认是不清理资源的，如果需要清理的话，就调用这个方法
        /// </summary>
        /// <param name="go">需要清理的ui对象</param>
        /// <param name="bClear">是否清理</param>
        public void ReleaseUI(GameObject go,bool bClear = false) {
            //UI永远都不进对象池
            ObjectManager.GetInstance.ReleaseObject(go, 0, bClear, false);
        }

        /// <summary>
        /// 把鼠标点击事件传递到下层 UI 或者 GameObject
        /// </summary>
        public void PassEvent<T>(PointerEventData data, ExecuteEvents.EventFunction<T> function)
            where T : IEventSystemHandler {
            var results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(data, results);
            var current = data.pointerCurrentRaycast.gameObject;
            //遍历 RayCastResult   
            for (int i = 0; i < results.Count; i++) {
                //剔除穿透脚本所在对象
                if (current != results[i].gameObject) {
                    //执行多层点击穿透
                    // ExecuteEvents.Execute(results[i].gameObject, data, function);
                    data.pointerPress = results[i].gameObject;
                    //只执行单层层穿透  点击事件传递成功break
                    if (ExecuteEvents.Execute(results[i].gameObject, data, function)) {
                        break;
                    }
                }
            }
        }
        /*  示例监听按下
        public void OnPointerDown(PointerEventData eventData) {
            PassEvent(eventData, ExecuteEvents.pointerDownHandler);
        }*/
    }
    /// <summary>
    /// 记录UI轨迹的一个节点
    /// </summary>
    class UIPageTrack {
        /// <summary>
        /// 页面名称
        /// </summary>
        public string UIName;
        /// <summary>
        /// 页面的类型
        /// </summary>
        public Type type;
        /// <summary>
        /// 页面的参数 
        /// </summary>
        public object arg;
    }
}

using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GF.Unity.UI {
    /// <summary>
    /// UIPanel是UIPage(页面),UIWindow(窗口)，UIWidget(挂件)的基类。
    /// </summary>
    public abstract class UIPanel:MonoBehaviour, ILogTag {
        public string LOG_TAG { get; protected set; }
        public virtual  UITypeDef UIType { get { return UITypeDef.Unkown; } }

        private int m_layer = UILayerDef.Unkown;
       
        public int Layer {
            get {
                return m_layer;
            }
            set {
                m_layer = value;
            }
        }
        /// <summary>
        /// 关闭事件的委托，一般情况下，窗口被关闭了需要通过委托事件告知打开这个窗口的模块，方便其他模块 做业务处理
        /// </summary>
        public delegate void CloseEvent(object arg = null);
        /// <summary>
        /// CloseEvent委托的事件
        /// </summary>
        public event CloseEvent oncloseevent;
        //Awake，用来绑定
        //void Awake() {
        //    LOG_TAG = this.GetType().Name;
        //    if (AutoBindUIElement) {
        //        UIElementBinder.BindAllUIElement(this);
        //    }
        //}
        /// <summary>
        /// UI关闭和打开的动画片段
        /// </summary>
        [SerializeField]
        private AnimationClip m_openAniClip;
        [SerializeField]
        private AnimationClip m_closeAniClip;
        private float m_closeAniClipTime;//关闭动画的时长
        private object m_closeArg;//关闭的参数，因为关闭需要等关闭动画执行完后再执行，所以需要先暂存一下参数

        private void Update() {
            if (m_closeAniClipTime > 0) {//通过动画时间来判断是否执行完毕，不是很精准，这里不使用协程是因为UIPanel是所有UI模块的基类，大量Ui调用动画时，大量的协程容易产生GC
                m_closeAniClipTime -= UnityEngine.Time.deltaTime;
                if (m_closeAniClipTime <= 0) {
                    CloseWorker();
                }
            }
            OnUpdate();//同样的道理，为了避免子类重写时忘记调用父类导致问题，所以子类都是使用OnUpdate处理Update方法
        }
        
        /// <summary>
        /// ui的打开方法,
        /// </summary>
        /// <param name="arg"></param>
        public void Open(object arg = null) {
            LOG_TAG = this.GetType().Name;
            if (!this.gameObject.activeSelf) {
                this.gameObject.SetActive(true);
            }
            OnOpen(arg);
            //播放UI动画
            if (m_openAniClip != null) {
                Animation ani = gameObject.GetComponent<Animation>();
                if (ani != null) {
                    ani.Play(m_openAniClip.name);
                } else {
                    Debuger.LogError("设置了动画,但是没有找到Animation组件");
                }
            }
        }

        /// <summary>
        /// ui的关闭方法
        /// </summary>
        /// <param name="bClear">是否清除资源，true的话会删除ui,并把内存中的资源一并清理，false就是把ui直接关闭，不清理内存，一般ui都不需要清理内存的</param>
        /// <param name="arg"></param>
        public virtual void Close(bool bClear =false,object arg = null) {
            m_closeArg = arg;
            m_closeAniClipTime = 0;
            //播放UI动画
            if (m_closeAniClip != null) {
                Animation ani = gameObject.GetComponent<Animation>();
                if (ani != null) {
                    m_closeAniClipTime = m_closeAniClip.length;
                    ani.Play(m_closeAniClip.name);
                } else {
                    Debuger.LogError("设置了动画,但是没有找到Animation组件");
                    CloseWorker(bClear);
                }
            } else {
                CloseWorker(bClear);
            } 
        }
        
        /// <summary>
        /// close真正逻辑执行的方法
        /// </summary>
        private void CloseWorker(bool bClear = false) {
            if (this.gameObject.activeSelf) {
                this.gameObject.SetActive(false);
            }
            if (oncloseevent != null) {//判断事件是否为空，如果不为空，就回调
                oncloseevent(m_closeArg);
                oncloseevent = null;
            }
            OnClose(bClear,m_closeArg);
        }


        /// <summary>
        /// 当UI被打开时调用的方法，原设计将open设计成virtual，让子类去实现，但是如果派生类实现没有调用父类的实现，会导致打开失败，所以就将open分成open（基础实现）和可重写的OnOpen两部分,不在open里写，是为了ui框架逻辑和业务逻辑区分开来。OnOpen用来处理业务逻辑。open方法只处理ui逻辑。
        /// </summary>
        /// <param name="args"></param>
        protected virtual void OnOpen(object args=null) {
            Layer = UILayerDef.GetDefaultLayer(UIType);
        }
        
        /// <summary>
        /// 当ui被关闭时，调用的方法。之所以要单独写一个方法，不在close里写，是为了ui框架逻辑和业务逻辑区分开来。OnClose用来处理业务逻辑。close方法只处理ui逻辑。
        /// 该模块重写时需要支持可重复调用
        /// </summary>
        protected virtual void OnClose(bool bClear = false,object arg = null) {
            if (bClear) {
                UIManager.GetInstance.ReleaseUI(this.gameObject,true);
            }
        }
       
        /// <summary>
        /// 子类用来处理需要Update的方法
        /// </summary>
        protected virtual void OnUpdate() {

        }
        /// <summary>
        /// 判断自己是否被激活
        /// </summary>
        public bool IsOpen { get { return this.gameObject.activeSelf; } }

        //========================================工具方法=============================================
        #region UI事件监听辅助函数
        /// <summary> 
        /// 为UIPanel内的脚本提供便捷的UI事件监听接口
        /// </summary>
        /// <param name="UIName">UI物体的名称</param>
        /// <param name="listener"></param>
        public void AddUIClickListener(string UIName, Action<string> listener) {
            Transform target = this.transform.Find(UIName);
            if (target != null) {
                UIEventTrigger.Get(target).onClickWithName -= listener;
                UIEventTrigger.Get(target).onClickWithName += listener;
            } else {
                Debuger.LogError("未找到UI控件：{0}", UIName);
            }
        }
        /// <summary>
        /// 为UIPanel内的脚本提供便捷的UI事件监听接口
        /// </summary>
        /// <param name="UIName">UI物体的名称</param>
        /// <param name="listener"></param>
        public void AddUIClickListener(string UIName, Action listener) {
            Transform target = this.transform.Find(UIName);
            if (target != null) {
                UIEventTrigger.Get(target).onClick -= listener;
                UIEventTrigger.Get(target).onClick += listener;
            } else {
                Debuger.LogError("未找到UI控件：{0}", UIName);
            }
        }
        /// <summary>
        /// 为UIPanel内的脚本提供便捷的UI事件监听接口
        /// </summary>
        /// <param name="target">目标UI</param>
        /// <param name="listener"></param>
        public void AddUIClickListener(UIBehaviour target, Action listener) {
            if (target != null) {
                UIEventTrigger.Get(target).onClick -= listener;
                UIEventTrigger.Get(target).onClick += listener;
            }
        }
        public void AddUIClickListener(UIBehaviour target, Action<string> listener) {
            if (target != null) {
                UIEventTrigger.Get(target).onClickWithName -= listener;
                UIEventTrigger.Get(target).onClickWithName += listener;
            }
        }
        /// <summary>
        /// 移除UI控件的监听器
        /// </summary>
        /// <param name="UIName"></param>
        /// <param name="listener"></param>
        public void RemoveUIClickListener(string UIName, Action<string> listener) {
            Transform target = this.transform.Find(UIName);
            if (target != null) {
                if (UIEventTrigger.HasExistTrigger(target)) {
                    UIEventTrigger.Get(target).onClickWithName -= listener;
                }
            } else {
                Debuger.LogError("未找到UI控件：{0}", UIName);
            }
        }
        /// <summary>
        /// 移除UI控件的监听器
        /// </summary>
        /// <param name="UIName"></param>
        /// <param name="listener"></param>
        public void RemoveUIClickListener(string UIName, Action listener) {
            Transform target = this.transform.Find(UIName);
            if (target != null) {
                if (UIEventTrigger.HasExistTrigger(target)) {
                    UIEventTrigger.Get(target).onClick -= listener;
                }
            } else {
                Debuger.LogError("未找到UI控件：{0}", UIName);
            }
        }
        /// <summary>
        /// 移除UI控件的监听器
        /// </summary>
        /// <param name="target"></param>
        /// <param name="listener"></param>
        public void RemoveUIClickListener(UIBehaviour target, Action listener) {
            if (target != null) {
                if (UIEventTrigger.HasExistTrigger(target.transform)) {
                    UIEventTrigger.Get(target).onClick -= listener;
                }
            }
        }
        /// <summary>
        /// 移除UI控件的所有监听器
        /// </summary>
        /// <param name="UIName"></param>
        public void RemoveUIClickListeners(string UIName) {
            Transform target = this.transform.Find(UIName);
            if (target != null) {
                if (UIEventTrigger.HasExistTrigger(target)) {
                    UIEventTrigger.Get(target).onClick = null;
                }
            } else {
                Debuger.LogError("未找到UI控件：{0}", UIName);
            }
        }
        /// <summary>
        /// 移除UI控件的所有监听器
        /// </summary>
        /// <param name="target"></param>
        public void RemoveUIClickListeners(UIBehaviour target) {
            if (target != null) {
                if (UIEventTrigger.HasExistTrigger(target.transform)) {
                    UIEventTrigger.Get(target).onClick = null;
                }
            }
        }
        #endregion 
    }
}

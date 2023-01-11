
using GF.Msg;
using System.Collections.Generic;
using System.Reflection;

namespace GF.Module {
    /// <summary>
    /// 常规模块,继承ILogTag的类，使用日志时可以直接应用this.log,否则只能使用Debuger.log
    /// </summary>
    public class GeneralModule : ModuleBase, ILogTag {
        /// <summary>
        /// 模块的名称
        /// </summary>
        private string m_name = "";
        /// <summary>
        /// 不允许模块名称被中途修改，所以定义属性
        /// </summary>
        public string Name { 
            get {
                if (string.IsNullOrEmpty(m_name)) {
                    m_name = this.GetType().Name;
                }
                return m_name; 
            } 
        }
        /// <summary>
        /// 继承ILogTag后，需要有一个LOG_TAG变量，用来输出时显示哪个模块的输出
        /// </summary>
        public string LOG_TAG{ get; protected set; }//之所以在这里不直接赋值类名，而是在构造中赋值，只为了在后面子类中重新覆盖，这样可以实现一个模块多个实例时输出不同的名称
        public int m_ID { get; set; }

        /// <summary>
        /// 在模块加载中时，在进度中显示该模块的名称
        /// </summary>
        public string Title;
        /// <summary>
        /// 构造
        /// </summary>
        public GeneralModule() {
            //模块名称默认为类名
            m_name = this.GetType().Name;
            LOG_TAG = m_name;
        }
        /// <summary>
        /// 模块创建方法，具体业务逻辑需要子类重写
        /// </summary>
        /// <param name="args"></param>
        public virtual void Create<T>(T args) {
            this.Log("args:{0}", args);
           
        }
        /// <summary>
        /// 模块创建方法，具体业务逻辑需要子类重写
        /// </summary>
        /// <param name="args"></param>
        public virtual void Create() {

        }
        /// <summary>
        /// 重写Release
        /// </summary>
        public override void Release() {
            base.Release();
            this.Log();//通过日志输出判断模块是否被释放
            MsgCenter.ClearModuleMsg(m_ID);//清楚模块所有的消息委托
            
        }
        /// <summary>
        /// 很多模块都是可见的，例如背包之类的，这个函数用来让子类显示自身模块，正常也是通过消息调用这个函数
        /// </summary>
        public virtual void Show<T>(T arg) {
            Debuger.Log("显示模块 ");
        }
        /// <summary>
        /// 很多模块都是可见的，例如背包之类的，这个函数用来让子类显示自身模块，正常也是通过消息调用这个函数
        /// </summary>
        public virtual void Show() {
            Debuger.Log("显示模块 ");
        }
        #region 原有的反射调用方法，已废弃，性能还不如unity自带的SendMessage
        /// <summary>
        /// //消息处理,使用internal定义，只能被moduleManager调用，不允许其他程序集外访问
        /// </summary>
        /// <param name="msg">需要调用的方法名称</param>
        /// <param name="args">传递的参数</param>
        //internal void HandleMsg(string msg, object[] args) {
        //    //this.Log("HandleMessage(),msg:{0},args:{1}", msg, args);//输出参数，方便问题定
        //    MethodInfo mi;
        //    if (!ModuleManager.GetInstance.m_AllMethodDic.TryGetValue($"{this.Name}.{msg}", out mi) || mi == null) {
        //        //使用C#反射来调用其他方法，必须是实例方法，不能是静态的，可以是非公，也可以是公共的，建议非公，因为一般消息处理方法，作为内部通讯，public容易被其他模块误调用，也不会作为静态方法
        //        mi = this.GetType().GetMethod(msg, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
        //        ModuleManager.GetInstance.m_AllMethodDic.Add($"{this.Name}.{msg}", mi);
        //    }
        //    if (mi != null) {//如果有这个方法，就调用处理
        //        mi.Invoke(this, BindingFlags.NonPublic, null, args, null);
        //    } else { //如果没有这个方法，就调用公共的处理消息的方法
        //        OnModuleMessage(msg, args);
        //    }
        //}
        #endregion

    }


}

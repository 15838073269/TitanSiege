using System;
namespace GDServer {
    //服务层的泛型基类，ServiceModule是服务层，完全独立，不需要事件机制和消息机制
    public abstract class Singleton<T> where T : Singleton<T>, new() {
        //泛型单例
        private static T _instance = default(T);
        public static T GetInstance {
            get {
                if (_instance == null) {
                    _instance = new T();
                    _instance.InitSingleton();
                }
                
                return _instance;
            }
        }
        //避免有人不知道继承servicemodule的类是单例,加上一个检查
        protected void CheckSingleton() {
            if (_instance == null) { //如果单例变量为空，证明不是通过正常单例调用的，而是自己new的，就 抛出异常
                var exp = new Exception("ServiceModule<" + typeof(T).Name + ">是单例，无法被直接实例化");
                throw exp;
            }
        }
        /// <summary>
        /// 方便初始化单例，扩展使用，一般用不上
        /// </summary>
        protected virtual void InitSingleton() {

        }
    }
}

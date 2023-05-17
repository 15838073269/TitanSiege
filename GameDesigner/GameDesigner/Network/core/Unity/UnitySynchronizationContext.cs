using System;
using System.Threading;

namespace Net.Unity
{
    /// <summary>
    /// unity线程同步中心管理类
    /// </summary>
#if UNITY_EDITOR
    [UnityEditor.InitializeOnLoad]
#endif
    public class UnitySynchronizationContext
    {
        /// <summary>
        /// 线程同步中心核心对象
        /// </summary>
        public static SynchronizationContext Context { get; private set; }

        private static int mainThreadId;
        /// <summary>
        /// unity主线程id
        /// </summary>
        public static int MainThreadId => mainThreadId;

        static UnitySynchronizationContext() 
        {
            InitializeSynchronizationContext();
        }

#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_WSA || UNITY_WEBGL
        [UnityEngine.RuntimeInitializeOnLoadMethod]
#endif
        private static void InitializeSynchronizationContext()
        {
            Context = SynchronizationContext.Current;
            if (Context == null)
                Context = new SynchronizationContext();
            mainThreadId = Thread.CurrentThread.ManagedThreadId;
        }

        public static void Send(SendOrPostCallback d, object state) 
        {
            Context.Send(d, state);
        }

        public static void Post(SendOrPostCallback d, object state) 
        {
            Context.Post(d, state);
        }

        /// <summary>
        /// 在主线程调用, 并返回结果到此线程, 如果此线程是主线程, 则直接调用并返回
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="d"></param>
        /// <returns></returns>
        public static T Get<T>(Func<T> d) 
        {
            if (Thread.CurrentThread.ManagedThreadId == mainThreadId)
                return d();
            bool complete = false;
            T result = default;
            Context.Post(new SendOrPostCallback((o)=> 
            {
                try { result = d(); }
                finally { complete = true; }
            }), null);
            while (!complete)
                Thread.Sleep(1);
            return result;
        }
    }
}
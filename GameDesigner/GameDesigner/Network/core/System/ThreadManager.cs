using System;
using System.Threading;
using Net.Event;
using UnityEngine.LowLevel;
using System.Collections.Generic;

namespace Net.System
{
    /// <summary>
    /// 事件线程管理
    /// </summary>
    public static class ThreadManager
    {
#if !UNITY_WEBGL
        private static Thread MainThread;
#endif
        /// <summary>
        /// 计时器对象
        /// </summary>
        public static TimerEvent Event { get; private set; } = new TimerEvent();
        /// <summary>
        /// 时间计数间隔
        /// </summary>
        public static uint Interval { get; set; } = 1;
        /// <summary>
        /// 运行中?
        /// </summary>
        public static bool IsRuning { get; set; }

        static ThreadManager()
        {
            Init();
            Start();
        }

        private static void Init()
        {
            IsRuning = true;
        }

        [UnityEngine.RuntimeInitializeOnLoadMethod(UnityEngine.RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Initialize()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.playModeStateChanged += (state) =>
            {
                switch (state)
                {
                    case UnityEditor.PlayModeStateChange.EnteredPlayMode:
                        IsRuning = true;
                        break;
                    case UnityEditor.PlayModeStateChange.ExitingPlayMode:
                        IsRuning = false;
                        break;
                }
            };
#endif
#if !UNITY_WEBGL //在webgl平台下 必须是主线程
            if (Config.Config.MainThreadTick)
#endif
            {
                var playerLoop = PlayerLoop.GetCurrentPlayerLoop();
                var runner = new PlayerLoopRunner();
                var runnerLoop = new PlayerLoopSystem
                {
                    type = typeof(PlayerLoopRunner),
                    updateDelegate = runner.Run
                };
                var copyList = new List<PlayerLoopSystem>(playerLoop.subSystemList)
                {
                    runnerLoop
                };
                playerLoop.subSystemList = copyList.ToArray();
                PlayerLoop.SetPlayerLoop(playerLoop);
            }
        }

        private static void Start()
        {
#if !UNITY_WEBGL
            if (!Config.Config.MainThreadTick)
            {
                MainThread = new Thread(Execute)
                {
                    Name = "事件线程",
                    IsBackground = true
                };
                MainThread.Start();
            }
#endif
        }

        /// <summary>
        /// 控制台死循环线程
        /// </summary>
        public static void Run()
        {
            Stop();
            Execute();
        }

        /// <summary>
        /// unity update每帧调用
        /// </summary>
        public static void Run(uint interval)
        {
            Stop();
            Event.UpdateEventFixed(interval, false);
        }

        public static void Stop()
        {
#if !UNITY_WEBGL
            if (MainThread != null)
            {
                MainThread.Abort();
                MainThread = null;
                Event.ResetEvents();//当线程直接结束, 也会中断当前异步执行的代码, 导致没有设置完成字段为true问题
            }
#endif
        }

        public static void Execute()
        {
            while (IsRuning)
            {
                try
                {
                    Event.UpdateEventFixed(Interval, true);
                }
                catch (ThreadAbortException ex)
                {
                    NDebug.LogWarning("事件线程:" + ex.Message);
                }
                catch (Exception ex)
                {
                    NDebug.LogError("事件线程异常:" + ex);
                }
            }
        }

        public static int Invoke(Func<bool> ptr, bool isAsync = false) 
        {
            return Event.AddEvent(0, ptr, isAsync);
        }

        public static int Invoke(Action ptr, bool isAsync = false)
        {
            return Event.AddEvent(0, ptr, isAsync);
        }

        public static int Invoke(string name, Func<bool> ptr, bool isAsync = false)
        {
            return Event.AddEvent(name, 0, ptr, isAsync);
        }

        public static int Invoke(float time, Func<bool> ptr, bool isAsync = false)
        {
            return Event.AddEvent(time, ptr, isAsync);
        }

        public static int Invoke(string name, float time, Func<bool> ptr, bool isAsync = false)
        {
            return Event.AddEvent(name, time, ptr, isAsync);
        }

        public static int Invoke(string name, int time, Func<bool> ptr, bool isAsync = false)
        {
            return Event.AddEvent(name, time, ptr, isAsync);
        }
    }
}

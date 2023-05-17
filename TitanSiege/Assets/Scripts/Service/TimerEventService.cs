/****************************************************
    文件：TimerEventService.cs
	作者：昔莘
    邮箱: 304183153@qq.com
    日期：2022/11/29 11:12:50
	功能：时间事件的管理类
*****************************************************/

using Net.Event;
using System.Threading;
using System;
using UnityEngine;
namespace GF.MainGame {
    public class TimerEventService : MonoBehaviour {
        public static TimerEvent Event { get; private set; } = new TimerEvent();
        /// <summary>
        /// 时间计数间隔
        /// </summary>
        public static uint Interval { get; set; } = 1;
        /// <summary>
        /// 运行中?
        /// </summary>
        public static bool IsRuning { get; set; }


        void Start() {
            Init();
        }

        private static void Init() {
            IsRuning = true;
        }

#if UNITY_EDITOR
        [UnityEngine.RuntimeInitializeOnLoadMethod]
        private static async void OnInUnityEditor() {
            IsRuning = true;
            while (UnityEditor.EditorApplication.isPlaying)
                await global::System.Threading.Tasks.Task.Yield();
            IsRuning = false;
        }
#endif

        /// <summary>
        /// unity update每帧调用
        /// </summary>
        public static void Run(uint interval) {
            Event.UpdateEventFixed(interval, false);
        }

     

        public static int Invoke(Func<bool> ptr, bool isAsync = false) {
            return Event.AddEvent(0, ptr, isAsync);
        }

        public static int Invoke(Action ptr, bool isAsync = false) {
            return Event.AddEvent(0, ptr, isAsync);
        }

        public static int Invoke(string name, Func<bool> ptr, bool isAsync = false) {
            return Event.AddEvent(name, 0, ptr, isAsync);
        }

        public static int Invoke(float time, Func<bool> ptr, bool isAsync = false) {
            return Event.AddEvent(time, ptr, isAsync);
        }

        public static int Invoke(string name, float time, Func<bool> ptr, bool isAsync = false) {
            return Event.AddEvent(name, time, ptr, isAsync);
        }

        public static int Invoke(string name, int time, Func<bool> ptr, bool isAsync = false) {
            return Event.AddEvent(name, time, ptr, isAsync);
        }
    }
}

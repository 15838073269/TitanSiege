using Cysharp.Threading.Tasks;
using Net.System;
using System;
using System.Threading;
#if !UNITY_WEBGL
using System.Threading.Tasks;
#endif

namespace Net.Event
{
    /// <summary>
    /// 时间计时器类
    /// </summary>
    public class TimerEvent
    {
        public class Event
        {
            public string name;
            public ulong time;
            public Action ptr1;
            public Action<object> ptr2;
            public Func<bool> ptr3;
            public Func<object, bool> ptr4;
            public object obj;
            public int invokeNum;
            internal ulong timeMax;
            internal int eventId;
            internal bool async;
            internal bool complete = true;
            internal bool isRemove;
            internal void SetIntervalTime(uint value)
            {
                timeMax = value;
            }
            public override string ToString()
            {
                return $"{name}";
            }
        }
        public FastListSafe<Event> events = new FastListSafe<Event>();
        private int eId = 1000;
        private ulong time;
        private uint frame;
        private uint startTick;
        private uint nextTick;

        /// <summary>
        /// 添加计时器事件, time时间后调用ptr
        /// </summary>
        /// <param name="time"></param>
        /// <param name="ptr"></param>
        /// <param name="isAsync">如果是耗时任务, 需要设置true</param>
        /// <returns>可用于结束事件的id</returns>
        public int AddEvent(float time, Action ptr, bool isAsync = false)
        {
            var enentID = Interlocked.Increment(ref eId);
            events.Add(new Event()
            {
                time = (ulong)(this.time + (time * 1000)),
                ptr1 = ptr,
                eventId = enentID,
                async = isAsync,
            });
            return enentID;
        }

        /// <summary>
        /// 添加计时器事件, time时间后调用ptr
        /// </summary>
        /// <param name="time"></param>
        /// <param name="ptr"></param>
        /// <param name="obj"></param>
        /// <param name="isAsync">如果是耗时任务, 需要设置true</param>
        /// <returns>可用于结束事件的id</returns>
        public int AddEvent(float time, Action<object> ptr, object obj, bool isAsync = false)
        {
            var enentID = Interlocked.Increment(ref eId);
            events.Add(new Event()
            {
                time = (ulong)(this.time + (time * 1000)),
                ptr2 = ptr,
                obj = obj,
                eventId = enentID,
                async = isAsync,
            });
            return enentID;
        }

        /// <summary>
        /// 添加计时器事件, 当time时间到调用ptr, 总共调用invokeNum次数
        /// </summary>
        /// <param name="time"></param>
        /// <param name="invokeNum">调用次数, -1则是无限循环</param>
        /// <param name="ptr"></param>
        /// <param name="obj"></param>
        /// <param name="isAsync">如果是耗时任务, 需要设置true</param>
        /// <returns>可用于结束事件的id</returns>
        public int AddEvent(float time, int invokeNum, Action<object> ptr, object obj, bool isAsync = false)
        {
            var enentID = Interlocked.Increment(ref eId);
            events.Add(new Event()
            {
                time = (ulong)(this.time + (time * 1000)),
                ptr2 = ptr,
                obj = obj,
                invokeNum = invokeNum,
                timeMax = (ulong)(time * 1000),
                eventId = enentID,
                async = isAsync,
            });
            return enentID;
        }

        /// <summary>
        /// 添加计时器事件, 当time时间到调用ptr, 当ptr返回true则time时间后再次调用ptr, 直到ptr返回false为止
        /// </summary>
        /// <param name="time"></param>
        /// <param name="ptr"></param>
        /// <param name="isAsync">如果是耗时任务, 需要设置true</param>
        /// <returns>可用于结束事件的id</returns>
        public int AddEvent(float time, Func<bool> ptr, bool isAsync = false)
        {
            return AddEvent("", time, ptr, isAsync);
        }

        /// <summary>
        /// 添加计时器事件, 当time时间到调用ptr, 当ptr返回true则time时间后再次调用ptr, 直到ptr返回false为止
        /// </summary>
        /// <param name="name"></param>
        /// <param name="time"></param>
        /// <param name="ptr"></param>
        /// <param name="isAsync">如果是耗时任务, 需要设置true</param>
        /// <returns>可用于结束事件的id</returns>
        public int AddEvent(string name, float time, Func<bool> ptr, bool isAsync = false)
        {
            var enentID = Interlocked.Increment(ref eId);
            events.Add(new Event()
            {
                name = name,
                time = (ulong)(this.time + (time * 1000)),
                ptr3 = ptr,
                eventId = enentID,
                timeMax = (ulong)(time * 1000),
                async = isAsync,
            });
            return enentID;
        }

        /// <summary>
        /// 添加计时器事件, 当time时间到调用ptr, 当ptr返回true则time时间后再次调用ptr, 直到ptr返回false为止
        /// </summary>
        /// <param name="name"></param>
        /// <param name="time"></param>
        /// <param name="ptr"></param>
        /// <param name="isAsync">如果是耗时任务, 需要设置true</param>
        /// <returns>可用于结束事件的id</returns>
        public int AddEvent(string name, int time, Func<bool> ptr, bool isAsync = false)
        {
            var enentID = Interlocked.Increment(ref eId);
            events.Add(new Event()
            {
                name = name,
                time = this.time + (ulong)time,
                ptr3 = ptr,
                eventId = enentID,
                timeMax = (ulong)time,
                async = isAsync,
            });
            return enentID;
        }

        /// <summary>
        /// 添加计时事件, 当time时间到调用ptr, 当ptr返回true则time时间后再次调用ptr, 直到ptr返回false为止
        /// </summary>
        /// <param name="time"></param>
        /// <param name="ptr"></param>
        /// <param name="obj"></param>
        /// <param name="isAsync">如果是耗时任务, 需要设置true</param>
        /// <returns>可用于结束事件的id</returns>
        public int AddEvent(float time, Func<object, bool> ptr, object obj, bool isAsync = false)
        {
            var enentID = Interlocked.Increment(ref eId);
            events.Add(new Event()
            {
                time = (ulong)(this.time + (time * 1000)),
                ptr4 = ptr,
                obj = obj,
                invokeNum = 1,
                timeMax = (ulong)(time * 1000),
                eventId = enentID,
                async = isAsync,
            });
            return enentID;
        }

        public TimerEvent() 
        {
            frame = 0u;//一秒60次
            startTick = (uint)Environment.TickCount;
            nextTick = startTick + 1000u;
        }

        public void UpdateEventFixed(uint interval = 17, bool sleep = false)//60帧
        {
            UpdateEventFixed((uint)Environment.TickCount, interval, sleep);
        }

        public void UpdateEventFixed(uint tick, uint interval, bool sleep = false)//60帧
        {
            var frameRate = 1000u / interval;
            if (tick >= nextTick)
            {
                if (frame < frameRate)
                {
                    var step = (frameRate - frame) * interval;
                    UpdateEvent(step);
                }
                frame = 0u;
                startTick = tick;
                nextTick = tick + 1000u;
                if (startTick >= nextTick)
                    nextTick = uint.MaxValue;
            }
            else if (frame < frameRate & (tick - startTick) >= frame * interval)
            {
                UpdateEvent(interval);
                frame++;
            }
            else if (sleep)
            {
                Thread.Sleep(1);
                var total = nextTick - tick;
                if (total >= 2000u) //当uint.MaxValue和1差距很大时, 会出现计数不命中的问题, 导致bug
                    nextTick = tick;
            }
            else
            {
                var total = nextTick - tick;
                if (total >= 2000u) //当uint.MaxValue和1差距很大时, 会出现计数不命中的问题, 导致bug
                    nextTick = tick;
            }
        }

        public void UpdateEvent(uint interval = 17)//60帧
        {
            time += interval;
            var count = events.Count;
            for (int i = 0; i < count; i++)
            {
                try
                {
                    var evt = events._items[i];
                    if (evt.isRemove)
                    {
                        events.RemoveAt(i);
                        count = events.Count;
                        if (i >= 0) i--;
                        continue;
                    }
                    if (time >= evt.time)
                    {
                        if (evt.ptr1 != null)
                        {
#if !UNITY_WEBGL
                            if (evt.async)
                            {
                                if (!evt.complete)
                                    continue;
                                evt.complete = false;
#if SERVICE
                                UniTask.Run(WorkExecute1, evt, false);
#else
                                _ = UniTask.RunOnThreadPool(WorkExecute1, evt, false);
#endif
                            }
                            else
#endif
                                evt.ptr1();
                        }
                        else if (evt.ptr2 != null)
                        {
#if !UNITY_WEBGL
                            if (evt.async)
                            {
                                if (!evt.complete)
                                    continue;
                                evt.complete = false;
#if SERVICE
                                UniTask.Run(WorkExecute2, evt, false);
#else
                                _ = UniTask.RunOnThreadPool(WorkExecute2, evt, false);
#endif
                            }
                            else
#endif
                            evt.ptr2(evt.obj);
                        }
                        else if (evt.ptr3 != null)
                        {
#if !UNITY_WEBGL
                            if (evt.async)
                            {
                                if (!evt.complete)
                                    continue;
                                evt.complete = false;
#if SERVICE
                                UniTask.Run(WorkExecute3, evt, false);
#else
                                _ = UniTask.RunOnThreadPool(WorkExecute3, evt, false);
#endif
                                continue;
                            }
#endif
                            if (evt.ptr3())
                                goto J;
                        }
                        else if (evt.ptr4 != null)
                        {
#if !UNITY_WEBGL
                            if (evt.async)
                            {
                                if (!evt.complete)
                                    continue;
                                evt.complete = false;
#if SERVICE
                                UniTask.Run(WorkExecute4, evt, false);
#else
                                _ = UniTask.RunOnThreadPool(WorkExecute4, evt, false);
#endif
                                continue;
                            }
#endif
                            if (evt.ptr4(evt.obj))
                                goto J;
                        }
                        if (evt.invokeNum == -1)
                            goto J;
                        if (--evt.invokeNum <= 0)
                        {
                            events.RemoveAt(i);
                            count = events.Count;
                            if (i >= 0) i--;
                            continue;//解决J:执行后索引超出异常
                        }
                    J: if (i >= 0 & i < count)
                            evt.time = time + evt.timeMax;
                    }
                }
                catch (Exception ex) 
                {
                    NDebug.LogError("计时器异常:" + ex);
                }
            }
        }

#if !UNITY_WEBGL
        private void WorkExecute1(object state)
        {
            var evt = state as Event;
            evt.ptr1();
            if (--evt.invokeNum <= 0)
                evt.isRemove = true;
            else
                evt.time = time + evt.timeMax;
            evt.complete = true;
        }

        private void WorkExecute2(object state)
        {
            var evt = state as Event;
            evt.ptr2(evt.obj);
            if (--evt.invokeNum <= 0)
                evt.isRemove = true;
            else
                evt.time = time + evt.timeMax;
            evt.complete = true;
        }

        private void WorkExecute3(object state)
        {
            var evt = state as Event;
            if (evt.ptr3())
                evt.time = time + evt.timeMax;
            else
                evt.isRemove = true;
            evt.complete = true;
        }

        private void WorkExecute4(object state)
        {
            var evt = state as Event;
            if (evt.ptr4(evt.obj))
                evt.time = time + evt.timeMax;
            else
                evt.isRemove = true;
            evt.complete = true;
        }
#endif

        /// <summary>
        /// 获取计时事件
        /// </summary>
        /// <param name="eventId"></param>
        public Event GetEvent(int eventId)
        {
            for (int i = 0; i < events.Count; i++)
            {
                if (events[i].eventId == eventId)
                {
                    return events[i];
                }
            }
            return null;
        }

        /// <summary>
        /// 移除事件
        /// </summary>
        /// <param name="eventId"></param>
        public void RemoveEvent(int eventId)
        {
            for (int i = 0; i < events.Count; i++)
            {
                if (events[i].eventId == eventId)
                {
                    events[i].isRemove = true;
                    return;
                }
            }
        }

        /// <summary>
        /// 重置计时器事件间隔
        /// </summary>
        /// <param name="eventID"></param>
        /// <param name="interval"></param>
        public void ResetTimeInterval(int eventID, ulong interval)
        {
            var evt = GetEvent(eventID);
            if (evt != null)
            {
                evt.time = time + interval;
                evt.timeMax = interval;
            }
        }

        /// <summary>
        /// 重置事件
        /// </summary>
        public void ResetEvents()
        {
            for (int i = 0; i < events.Count; i++)
            {
                events[i].complete = true;
            }
        }
    }

    public class TimerTick 
    {
        private uint frame;
        private uint startTick;
        private uint nextTick;

        public TimerTick()
        {
            frame = 0;//一秒60次
            startTick = (uint)Environment.TickCount;
            nextTick = startTick + 1000u;
        }

        public bool CheckTimeout(uint tick, uint interval, bool sleep = false)//60帧
        {
            var frameRate = 1000u / interval;
            if (tick >= nextTick)
            {
                frame = 0u;
                startTick = tick;
                nextTick = tick + 1000u;
                if (startTick >= nextTick)
                    nextTick = uint.MaxValue;
                return true;
            }
            else if (frame < frameRate & (tick - startTick) >= frame * interval)
            {
                frame++;
                return true;
            }
            else if (sleep)
            {
                Thread.Sleep(1);
                var total = nextTick - tick;
                if (total >= 2000u) //当uint.MaxValue和1差距很大时, 会出现计数不命中的问题, 导致bug
                    nextTick = tick;
            }
            else
            {
                var total = nextTick - tick;
                if (total >= 2000u) //当uint.MaxValue和1差距很大时, 会出现计数不命中的问题, 导致bug
                    nextTick = tick;
            }
            return false;
        }
    }
}
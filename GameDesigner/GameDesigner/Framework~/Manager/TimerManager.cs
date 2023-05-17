using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public class TimerManager : MonoBehaviour
    {
        private readonly Dictionary<string, float> timerDict = new Dictionary<string, float>();

        /// <summary>
        /// 检查时间是否到达, 如果时间到达,则重新计算下一次的时间并返回true
        /// </summary>
        /// <param name="name"></param>
        /// <param name="time"></param>
        /// <param name="firstValue">如果此时间名是第一次调用, 需要返回的是true或者false</param>
        /// <returns></returns>
        public bool IsTimeOut(string name, float time, bool firstValue = false)
        {
            if (!timerDict.TryGetValue(name, out var tick))
            {
                timerDict.Add(name, Time.time + time);
                return firstValue;
            }
            if (Time.time >= tick)
            {
                timerDict[name] = Time.time + time;
                return true;
            }
            return false;
        }

        public void RemoveTime(string name)
        {
            timerDict.Remove(name);
        }
    }
}
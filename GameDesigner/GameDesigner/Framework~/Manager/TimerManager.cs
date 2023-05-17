using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public class TimerManager : MonoBehaviour
    {
        private readonly Dictionary<string, float> timerDict = new Dictionary<string, float>();

        /// <summary>
        /// ���ʱ���Ƿ񵽴�, ���ʱ�䵽��,�����¼�����һ�ε�ʱ�䲢����true
        /// </summary>
        /// <param name="name"></param>
        /// <param name="time"></param>
        /// <param name="firstValue">�����ʱ�����ǵ�һ�ε���, ��Ҫ���ص���true����false</param>
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
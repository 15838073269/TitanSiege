using System;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    private readonly Dictionary<string, List<Action<object[]>>> events = new Dictionary<string, List<Action<object[]>>>();

    /// <summary>
    /// 添加事件, 以方法名为事件名称
    /// </summary>
    /// <param name="eventDelegate"></param>
    public void AddEvent(Action<object[]> eventDelegate)
    {
        var eventName = eventDelegate.Method.Name;
        AddEvent(eventName, eventDelegate);
    }

    /// <summary>
    /// 添加事件
    /// </summary>
    /// <param name="eventName"></param>
    /// <param name="eventDelegate"></param>
    public void AddEvent(string eventName, Action<object[]> eventDelegate)
    {
        if (!events.TryGetValue(eventName, out var delegates))
            events.Add(eventName, delegates = new List<Action<object[]>>());
        delegates.Add(eventDelegate);
    }

    /// <summary>
    /// 派发事件
    /// </summary>
    /// <param name="eventName"></param>
    /// <param name="pars"></param>
    public void Dispatch(string eventName, params object[] pars)
    {
        if (events.TryGetValue(eventName, out var delegates))
        {
            foreach (var item in delegates)
                item.Invoke(pars);
        }
    }

    /// <summary>
    /// 移除事件, 以方法名为事件名查找并移除
    /// </summary>
    /// <param name="eventDelegate"></param>
    public void Remove(Action<object[]> eventDelegate)
    {
        var eventName = eventDelegate.Method.Name;
        Remove(eventName, eventDelegate);
    }

    /// <summary>
    /// 移除事件
    /// </summary>
    /// <param name="eventName"></param>
    /// <param name="eventDelegate"></param>
    public void Remove(string eventName, Action<object[]> eventDelegate)
    {
        if (events.TryGetValue(eventName, out var delegates))
        {
            foreach (var item in delegates)
            {
                if (item.Equals(eventDelegate)) 
                {
                    delegates.Remove(item);
                    break;
                }
            }
        }
    }
}

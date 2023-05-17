using System;
using System.Collections.Generic;
using Object = UnityEngine.Object;
using UnityEngine;

namespace Framework 
{
    public class ObjectPool : MonoBehaviour
    {
        private readonly Dictionary<Type, Queue<Object>> pool = new Dictionary<Type, Queue<Object>>();

        public T GetObject<T>(AssetBundleType abType, string resPath) where T : Object
        {
            var type = typeof(T);
            if (!pool.TryGetValue(type, out var queue))
                pool.Add(type, queue = new Queue<Object>());
            if (queue.Count > 0)
                return queue.Dequeue() as T;
            var resObj = Global.Resources.LoadAsset<T>(abType, resPath);
            return Instantiate(resObj);
        }

        public T GetObject<T>(string resPath) where T : Object
        {
            var type = typeof(T);
            if (!pool.TryGetValue(type, out var queue))
                pool.Add(type, queue = new Queue<Object>());
            if (queue.Count > 0)
                return queue.Dequeue() as T;
            var resObj = Global.Resources.LoadAssetWithAll<T>(resPath);
            return Instantiate(resObj);
        }

        public T GetObject<T>(Object ifCreatObject, Transform ifCreatObjectParent = null) where T : Object
        {
            var type = typeof(T);
            if (!pool.TryGetValue(type, out var queue))
                pool.Add(type, queue = new Queue<Object>());
            T poolObj;
            while (queue.Count > 0) //如果池内的物体被意外删除了, 就会被清除忽略掉
            {
                poolObj = queue.Dequeue() as T;
                if (poolObj != null)
                    return poolObj;
            }
            poolObj = Instantiate(ifCreatObject as T, ifCreatObjectParent);
            return poolObj;
        }

        public void Recycling(Object obj)
        {
            var type = obj.GetType();
            if (!pool.TryGetValue(type, out var queue))
                pool.Add(type, queue = new Queue<Object>());
            queue.Enqueue(obj);
        }
    }
}
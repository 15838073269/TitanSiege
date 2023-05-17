#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_WSA || UNITY_WEBGL
namespace Net.Component
{
    using global::System;
    using global::System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// MonoBehaviour 单例基类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class SingleCase<T> : MonoBehaviour where T : SingleCase<T>
    {
        /// <summary>
        /// 由于第一次判断实例是否为空的时候, 如果直接使用Instance会进行查找类型, 单例被被赋值, 所以有必要的时候这个静态字段要用到
        /// </summary>
        protected static T instance;
        /// <summary>
        /// 单例实例
        /// </summary>
        public static T Instance
        {
            get
            {
                if (instance == null)
                {
#if UNITY_2020_1_OR_NEWER
                    instance = FindObjectOfType<T>(true);
#else
                    var ts = Resources.FindObjectsOfTypeAll<T>();
                    foreach (var t in ts)
                    {
                        if (t.gameObject.scene.isLoaded)
                        {
                            instance = t;
                            break;
                        }
                    }
#endif
                }
                return instance;
            }
        }
        /// <summary>
        /// 单例实例
        /// </summary>
        public static T I
        {
            get { return Instance; }
        }

        protected virtual void Awake()
        {
            if (instance != null & instance != this)
            {
                Destroy(gameObject);
                return;
            }
            instance = this as T;
        }

        public Stack<Action> OnBack = new Stack<Action>();

        public static T Show(Action onBack = null)
        {
            var i = I;
            if (i == null)
                return null;
            i.gameObject.SetActive(true);
            //i.transform.SetAsLastSibling();
            if (onBack != null)
                i.OnBack.Push(onBack);
            return i;
        }

        public static void Hide(bool isBack = true)
        {
            var i = I;
            if (i == null)
                return;
            i.gameObject.SetActive(false);
            if (isBack)
            {
                if (i.OnBack.Count == 0)
                    return;
                i.OnBack.Pop()();
            }
        }
    }
}
#endif
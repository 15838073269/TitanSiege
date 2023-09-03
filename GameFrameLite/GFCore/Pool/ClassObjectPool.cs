/****************************************************
    文件：ClassObjectPool.cs
	作者：昔莘
    邮箱: 304183153@qq.com
    日期：2021/8/18 8:52:21
	功能：类的对象池
*****************************************************/

using System;
using System.Collections.Generic;
namespace GF.Pool {
    public class ClassObjectPool<T> where T : class, new() {
        public Func<T> CretateObj = null; 
        //池，此处使用栈，因为是较底层的代码，所以尽量不要用list，字典等。
        protected Stack<T> m_Pool = new Stack<T>();
        //池子的最大对象个数，<=0 则表示不限个数
        protected int m_MaxCount = 0;
        //没有回收的对象个数
        protected int m_NoRecycleCount = 0;
        //对象池管理是否是mono的
        private bool m_IsMono;
        //构造
        public ClassObjectPool(int maxcount, bool isMono=false) {
            m_IsMono = isMono;
            m_MaxCount = maxcount;
            if (!isMono) {
                for (int i = 0; i < maxcount; i++) {//将所有的对象都添加到栈中
                    m_Pool.Push(new T());
                }
            }
            
        }
        /// <summary>
        /// 从对象池中取用对象
        /// </summary>
        /// <param name="CreateIfPoolEmpty">如果对象池空了，是否自动创建</param>
        /// <returns></returns>
        public T GetObj(bool CreateIfPoolEmpty) {
            T rtn = null;
            if (m_Pool.Count > 0) {
                rtn = m_Pool.Pop();
                if (rtn == null) { //对象可能意外情况被销毁了
                    if (CreateIfPoolEmpty) { //如果选择自动创建
                        if (!m_IsMono) {
                            rtn = new T();
                        } else {
                            rtn = CretateObj.Invoke();
                        }
                        m_NoRecycleCount++;
                    }
                }
            } else { //池子里没有了，例如最大个数5个，结果实际用到了第六个时
                if (CreateIfPoolEmpty) { //如果选择自动创建
                    if (!m_IsMono) {
                        rtn = new T();
                    } else {
                        rtn = CretateObj.Invoke();
                    }
                    m_NoRecycleCount++;
                }
            }
            return rtn;//没有选择自动创建，遇到上述情况就返回null
        }
        /// <summary>
        /// 回收类对象
        /// </summary>
        /// <param name="obj">对象</param>
        /// <returns></returns>
        public bool Recycle(T obj) {
            if (obj == null) {
                return false;
            }
            m_NoRecycleCount--;
            if (m_Pool.Count >= m_MaxCount && m_MaxCount > 0) {
                obj = null;
                return false;
            }
            m_Pool.Push(obj);
            return true;
        }
        /// <summary>
        /// 清空对象池，一般只有内存不足主动清理了才会使用
        /// 否则直接销毁对象池即可
        /// </summary>
        public void ClearPool() {
            m_Pool.Clear();
        }
        /// <summary>
        /// 销毁对象池
        /// </summary>
        ~ClassObjectPool() {
            //while (m_Pool.Count>0) {
            //    T tmp = m_Pool.Pop();
            //    tmp = null;
            //}
            CretateObj = null;
            m_Pool.Clear();
        }
    }
}

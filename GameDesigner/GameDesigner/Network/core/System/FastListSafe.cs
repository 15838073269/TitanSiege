using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading;

namespace Net.System
{
    /// <summary>
    /// 线程安全的List类, 无序的, 极速的
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [DebuggerTypeProxy(typeof(Mscorlib_CollectionDebugView<>))]
    [DebuggerDisplay("Count = {Count}")]
    [Serializable]
    public class FastListSafe<T> : FastList<T>
    {
        public void Add(T item)
        {
            lock (SyncRoot)
            {
                base.Add(item);
            }
        }

        public void Add(T item, out int index)
        {
            lock (SyncRoot)
            {
                base.Add(item, out index);
            }
        }

        public void AddRange(IEnumerable<T> collection)
        {
            lock (SyncRoot)
            {
                base.AddRange(collection);
            }
        }

        public int BinarySearch(int index, int count, T item, IComparer<T> comparer)
        {
            lock (SyncRoot)
            {
                return base.BinarySearch(index, count, item, comparer);
            }
        }

        public void Clear()
        {
            lock (SyncRoot) 
            {
                base.Clear();
            }
        }

        public void CopyTo(T[] array)
        {
            CopyTo(array, 0);
        }

        public void CopyTo(int index, T[] array, int arrayIndex, int count)
        {
            lock (SyncRoot)
            {
                base.CopyTo(index, array, arrayIndex, count);
            }
        }


        public void CopyTo(T[] array, int arrayIndex)
        {
            lock (SyncRoot)
            {
                base.CopyTo(array, arrayIndex);
            }
        }

        public void ForEach(Action<T> action)
        {
            lock (SyncRoot) 
            {
                base.ForEach(action);
            }
        }

        public T[] GetRange(int index, int count)
        {
            lock (SyncRoot)
            {
                return base.GetRange(index, count);
            }
        }

        /// <summary>
        /// 获取列表对象, 并移除列表, 如果在多线程下, 多线程并行下, 是可以获取到对象, 但是会出现长度不是所指定的长度, 所以获取后要判断一下长度
        /// </summary>
        /// <param name="index"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public T[] GetRemoveRange(int index, int count)
        {
            lock (SyncRoot)
            {
                return base.GetRemoveRange(index, count);
            }
        }

        public void Insert(int index, T item)
        {
            lock (SyncRoot)
            {
                base.Insert(index, item);
            }
        }

        public void InsertRange(int index, IEnumerable<T> collection)
        {
            lock (SyncRoot)
            {
                base.InsertRange(index, collection);
            }
        }

        public bool Remove(T item)
        {
            lock (SyncRoot)
            {
                return base.Remove(item);
            }
        }

        public int RemoveAll(Predicate<T> match)
        {
            lock (SyncRoot)
            {
                return base.RemoveAll(match);
            }
        }


        public void RemoveAt(int index)
        {
            lock (SyncRoot)
            {
                base.RemoveAt(index);
            }
        }

        public void RemoveRange(int index, int count)
        {
            lock (SyncRoot)
            {
                base.RemoveRange(index, count);
            }
        }

        public void Reverse()
        {
            Reverse(0, Count);
        }

        public void Reverse(int index, int count)
        {
            lock (SyncRoot)
            {
                base.Reverse(index, count);
            }
        }

        public void Sort()
        {
            Sort(0, Count, null);
        }

        public void Sort(IComparer<T> comparer)
        {
            Sort(0, Count, comparer);
        }

        public void Sort(int index, int count, IComparer<T> comparer)
        {
            lock (SyncRoot)
            {
                base.Sort(index, count, comparer);
            }
        }

        public void Sort(Comparison<T> comparison)
        {
            lock (SyncRoot)
            {
                base.Sort(comparison);
            }
        }

        public T[] ToArray()
        {
            lock (SyncRoot)
            {
                return base.ToArray();
            }
        }
    }
}
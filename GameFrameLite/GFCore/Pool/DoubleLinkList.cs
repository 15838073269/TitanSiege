/****************************************************
    文件：ClassObjectPool.cs
	作者：昔莘
    邮箱: 304183153@qq.com
    日期：2021/8/18 8:52:21
	功能：双向链表类
*****************************************************/

using System.Collections.Generic;

namespace GF.Pool {
    /// <summary>
    /// 双向链表节点
    /// </summary>
    public class DoubleLinkListNode<T> where T : class, new() {
        /// <summary>
        /// 节点前驱
        /// </summary>
        public DoubleLinkListNode<T> prev = null;
        /// <summary>
        /// 节点后驱
        /// </summary>
        public DoubleLinkListNode<T> next = null;
        /// <summary>
        /// 当前节点
        /// </summary>
        public T t = null;

    }
    /// <summary>
    /// 双向链表
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DoubleLinkList<T> where T : class, new() {
        /// <summary>
        /// 表头
        /// </summary>
        public DoubleLinkListNode<T> Head = null;
        /// <summary>
        /// 表尾
        /// </summary>
        public DoubleLinkListNode<T> Tail = null;
        /// <summary>
        /// 双向链表的节点对象池
        /// </summary>
        protected ClassObjectPool<DoubleLinkListNode<T>> m_DoubleLinkNodePool = new ClassObjectPool<DoubleLinkListNode<T>>(500);
        /// <summary>
        /// 节点数量
        /// </summary>
        protected int m_Count = 0;
        public int Count {
            get {
                return m_Count;
            }
            set {
                if (value < 0) {
                    Count = 0;
                }
            }
        }
        /// <summary>
        /// 添加节点到链表头部
        /// </summary>
        /// <param name="t">节点内容对象</param>
        /// <returns>节点对象</returns>
        public DoubleLinkListNode<T> AddHeader(T t) {
            DoubleLinkListNode<T> pnote = m_DoubleLinkNodePool.GetObj(true);
            pnote.next = null;
            pnote.prev = null;
            pnote.t = t;
            return AddHeader(pnote);
        }
        public DoubleLinkListNode<T> AddHeader(DoubleLinkListNode<T> pnote) {
            if (pnote == null) {
                return null;
            }
            pnote.prev = null;//作为头部，就没有前驱
            if (Head == null) {
                Head = Tail = pnote;//如果是空链表，就作为第一个元素
            } else {
                pnote.next = Head;//原head作为现节点的后驱
                Head.prev = pnote;//原head前驱为现节点
                Head = pnote;//将现节点作为新的head赋值
            }
            m_Count++;
            return pnote;
        }
        /// <summary>
        /// 添加节点到链表尾部
        /// </summary>
        /// <param name="t">节点内容</param>
        /// <returns></returns>
        public DoubleLinkListNode<T> AddTail(T t) {
            DoubleLinkListNode<T> pnote = m_DoubleLinkNodePool.GetObj(true);
            pnote.next = null;
            pnote.prev = null;
            pnote.t = t;
            return AddTail(pnote);
        }
        public DoubleLinkListNode<T> AddTail(DoubleLinkListNode<T> pnote) {
            if (pnote == null) {
                return null;
            }
            pnote.next = null;
            if (Head == null) {
                Head = Tail = pnote;//如果是空链表，就作为第一个元素
            } else {
                pnote.prev = Tail;
                Tail.next = pnote;
                Tail = pnote;
            }
            return pnote;
        }
        /// <summary>
        /// 移除某个节点
        /// </summary>
        /// <param name="pnode"></param>
        public void RemoveNode(DoubleLinkListNode<T> pnode) {
            if (pnode == null) {
                return;
            }
            if (pnode == Head) {
                Head = pnode.next;
            }
            if (pnode == Tail) {
                Tail = pnode.prev;
            }
            if (pnode.prev != null) {
                pnode.prev.next = pnode.next;
            }
            if (pnode.next != null) {
                pnode.next.prev = pnode.prev;
            }
            pnode.prev = pnode.next = null;
            pnode.t = null;
            m_Count--;
        }

        public void MoveToHead(DoubleLinkListNode<T> pnode) {
            if (pnode == null || pnode == Head) {
                return;
            }
            if (pnode.prev == null && pnode.next == null) {
                return;
            }
            if (pnode == Tail) {
                Tail = pnode.prev;
            }
            if (pnode.prev != null) {
                pnode.prev.next = pnode.next;
            }
            if (pnode.next != null) {
                pnode.next.prev = pnode.prev;
            }
            pnode.prev = null;
            pnode.next = Head;
            Head.prev = pnode;
            Head = pnode;
            if (Tail == null) {
                Tail = Head;
            }
        }
    }
    /// <summary>
    /// 双向链表的封装操作类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CMapList<T> where T : class, new() {
        DoubleLinkList<T> m_DLink = new DoubleLinkList<T>();
        /// <summary>
        /// 存储所有链表节点额字典，用来快速查找链表节点，key为链表节点内容T,value为链表节点
        /// 这么做有些争议，本来使用链表就是想让资源存储结构更简单，在加一个字典辅助，使用是简单了，明显浪费了资源
        /// </summary>
        Dictionary<T, DoubleLinkListNode<T>> m_FindNodeDic = new Dictionary<T, DoubleLinkListNode<T>>();
        /// <summary>
        /// 插入一个内容到表头
        /// </summary>
        /// <param name="t"></param>
        public void InsertToHead(T t) {
            DoubleLinkListNode<T> node = null;
            if (m_FindNodeDic.TryGetValue(t, out node) && node != null) {
                m_DLink.MoveToHead(node);
                return;
            } else {
                node = m_DLink.AddHeader(t);
                m_FindNodeDic.Add(t, node);
            }
        }
        /// <summary>
        /// 从尾部弹出一个节点，其实就是删除了
        /// </summary>
        public void PopTail() {
            //首先要有尾部
            if (m_DLink.Tail != null) {
                Remove(m_DLink.Tail.t);
            }
        }
        /// <summary>
        /// 删除一个节点
        /// </summary>
        /// <param name="t"></param>
        public void Remove(T t) {
            DoubleLinkListNode<T> node = null;
            if (!m_FindNodeDic.TryGetValue(t, out node) || node == null) {
                return;
            }
            m_DLink.RemoveNode(node);
            m_FindNodeDic.Remove(t);
        }
        /// <summary>
        /// 获取尾部节点
        /// </summary>
        /// <returns></returns>
        public T BackTail() {
            return (m_DLink.Tail == null) ? null : m_DLink.Tail.t;
        }
        /// <summary>
        /// 返回字典长度
        /// </summary>
        /// <returns></returns>
        public int Size() {
            return m_FindNodeDic.Count;
        }
        /// <summary>
        /// 是否包含这个节点
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public bool Find(T t) {
            DoubleLinkListNode<T> node = null;
            if (!m_FindNodeDic.TryGetValue(t, out node) || node == null) {
                return false;
            }
            return true;
        }
        
        /// <summary>
        /// 把某个节点移动到头部
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public bool MoveToHead(T t) {
            DoubleLinkListNode<T> node = null;
            if (!m_FindNodeDic.TryGetValue(t, out node) || node == null) {
                return false;
            }
            m_DLink.MoveToHead(node);
            return true;
        }

        public void Clear() {
            while (m_DLink.Tail!=null) {
                PopTail();
            }
        }
        ~CMapList(){
            Clear();
        }
    }
}

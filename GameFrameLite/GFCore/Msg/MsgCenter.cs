

using GF.Module;
using GF.Utils;
using System;
using System.Collections.Generic;

namespace GF.Msg {
    /// <summary>
    /// 每个消息节点的类
    /// </summary>
    public class EventNote {//每个消息（脚本）对应的节点，使用单向链表的方式存储
        public ICacheMsg curdata;//该节点当前缓存消息
        public EventNote next;//该节点的下一个缓存节点
        public EventNote(ICacheMsg data) {//构造函数给节点赋值
            this.curdata = data;
            this.next = null;
        }
    }
    /// <summary>
    /// 缓存消息的接口,具备返回值的消息不支持缓存
    /// </summary>
    public interface ICacheMsg {
        int Key { get; set; }//事件id
        int Module { get; set; }//事件所属模块
    }
    /// <summary>
    /// 缓存消息的实现
    /// </summary>
    public class CacheMsg : ICacheMsg {
        public int Key { get; set; }//事件id
        public int Module { get; set; }//事件所属模块
       
    }
    public class CacheMsg<T1> : ICacheMsg {
        public int Key { get; set; }//事件id
        public int Module { get; set; }//事件所属模块
        public bool Isreturn { get; set; }//是否需要返回值


        public T1 Arg1;
    }
    public class CacheMsg<T1,T2> : ICacheMsg {
        public int Key { get; set; }//事件id
        public int Module { get; set; }//事件所属模块
        public bool Isreturn { get; set; }//是否需要返回值
        public T1 Arg1;
        public T2 Arg2;
    }
    public class CacheMsg<T1, T2,T3> : ICacheMsg {
        public int Key { get; set; }//事件id
        public int Module { get; set; }//事件所属模块
        public bool Isreturn { get; set; }//是否需要返回值
        public T1 Arg1;
        public T2 Arg2;
        public T3 Arg3;
    }
    public class CacheMsg<T1, T2, T3,T4> : ICacheMsg {
        public int Key { get; set; }//事件id
        public int Module { get; set; }//事件所属模块
        public bool Isreturn { get; set; }//是否需要返回值
        public T1 Arg1;
        public T2 Arg2;
        public T3 Arg3;
        public T4 Arg4;
    }
    public static class MsgCenter {
        #region 消息处理代码
        /// <summary>
        /// 外层string是模块名称,之所以套上一层模块名，是为了后面按模块区分消息，避免多人协作时，消息名称重复，如果单人开发，这里没必要嵌套
        /// 内层string是注册的事件名称
        /// IMessageData本身也包含模块名，为了方便后面使用
        /// </summary>
        private static Dictionary<int, Dictionary<int, IMessageAction>> m_ActionDic;
        private static Dictionary<int, EventNote> m_CacheDic;//缓存因为模块未启动导致的消息,key是事件的id,消息缓存不能是字段，此处应该考虑使用链表，因为一个消息可能发送多次，缓存时，需要按顺序挨个执行
        private static Dictionary<int, string> m_ModuleNameDic;

        public static void Init(Dictionary<int, string> modulenamedic) {
            m_ActionDic = new Dictionary<int, Dictionary<int, IMessageAction>>();
            m_CacheDic = new Dictionary<int, EventNote>();
            m_ModuleNameDic = modulenamedic;
        }
        /// <summary>
        /// 通过名称获取模块id
        /// </summary>
        /// <param name="modulename"></param>
        /// <returns></returns>
        public static int GetModuleId(string modulename) {
            int id = -1;
            foreach (KeyValuePair<int ,string> item in m_ModuleNameDic) {
                if (item.Value == modulename) {
                    id = item.Key;
                    break;
                }
            }
            return id;
        }
        private static void ModuleCreate(int module) {
            string modulename;
            if (m_ModuleNameDic.TryGetValue(module, out modulename)) {
                if (!ModuleManager.GetInstance.HasModule(modulename)) {
                    ModuleManager.GetInstance.CreateModule(modulename);
                }
            }
        }
        #region Action Register
        public static void Register(int ownermodule, int key, Action action) {
            Dictionary<int, IMessageAction> msgdatadic;
            if (m_ActionDic.TryGetValue(ownermodule, out msgdatadic)) {
                if (msgdatadic.TryGetValue(key, out var previousAction)) {
                    if (previousAction is MessageAction messageData) {
                        messageData.MessageEvents += action;
                    }
                } else {
                    MessageAction msgdata = new MessageAction(action);
                    msgdatadic.Add(key, msgdata);
                }
            } else {
                msgdatadic = new Dictionary<int, IMessageAction>();
                MessageAction msgdata = new MessageAction(action);
                msgdatadic.Add(key, msgdata);
                m_ActionDic.Add(ownermodule, msgdatadic);
            }
            //判断一下是否存在未执行的操作
            EventNote note = null;
            if (m_CacheDic.TryGetValue(key,out note) && note != null) {
                do {//调用每个节点的对应的脚本的消息处理函数，此处使用的策略模式，分别调用的是每个脚本的消息处理函数
                    CacheMsg temp = note.curdata as CacheMsg;
                    //存在就调用一下执行方法
                    SendMessage(ownermodule, key);
                    note = note.next;
                } while (note != null);
                m_CacheDic.Remove(key);//移除
            }
        }
        public static void Register<T1>(int ownermodule, int key, Action<T1> action) {
            Dictionary<int, IMessageAction> msgdatadic;
            if (m_ActionDic.TryGetValue(ownermodule, out msgdatadic)) {
                if (msgdatadic.TryGetValue(key, out var previousAction)) {
                    if (previousAction is MessageAction<T1> messageData) {
                        messageData.MessageEvents += action;
                    }
                } else {
                    MessageAction<T1> msgdata = new MessageAction<T1>(action);
                    msgdatadic.Add(key, msgdata);
                }
            } else {
                msgdatadic = new Dictionary<int, IMessageAction>();
                MessageAction<T1> msgdata = new MessageAction<T1>(action);
                msgdatadic.Add(key, msgdata);
                m_ActionDic.Add(ownermodule, msgdatadic);
            }
            //判断一下是否存在未执行的操作
            EventNote note = null;
            if (m_CacheDic.TryGetValue(key, out note) && note != null) {
                do {//调用每个节点的对应的脚本的消息处理函数，此处使用的策略模式，分别调用的是每个脚本的消息处理函数
                    CacheMsg<T1> temp = note.curdata as CacheMsg<T1>;
                    if (temp.Isreturn) {
                        //存在就调用一下执行方法
                        SendMessageWithReturn<T1>(ownermodule, key);
                    } else {
                        //存在就调用一下执行方法
                        SendMessage<T1>(ownermodule, key, temp.Arg1);
                    }
                    note = note.next;
                } while (note != null);
                m_CacheDic.Remove(key);//移除
            }
        }
        public static void Register<T1, T2>(int ownermodule, int key, Action<T1, T2> action) {
            Dictionary<int, IMessageAction> msgdatadic;
            if (m_ActionDic.TryGetValue(ownermodule, out msgdatadic)) {
                if (msgdatadic.TryGetValue(key, out var previousAction)) {
                    if (previousAction is MessageAction<T1, T2> messageData) {
                        messageData.MessageEvents += action;
                    }
                } else {
                    MessageAction<T1, T2> msgdata = new MessageAction<T1, T2>(action);

                    msgdatadic.Add(key, msgdata);
                }
            } else {
                msgdatadic = new Dictionary<int, IMessageAction>();
                MessageAction<T1, T2> msgdata = new MessageAction<T1, T2>(action);

                msgdatadic.Add(key, msgdata);
                m_ActionDic.Add(ownermodule, msgdatadic);
            }
            //判断一下是否存在未执行的操作
            EventNote note = null;
            if (m_CacheDic.TryGetValue(key, out note) && note != null) {
                do {//调用每个节点的对应的脚本的消息处理函数，此处使用的策略模式，分别调用的是每个脚本的消息处理函数
                    CacheMsg<T1, T2> temp = note.curdata as CacheMsg<T1, T2>;
                    if (temp.Isreturn) {
                        //存在就调用一下执行方法
                        SendMessageWithReturn<T1, T2>(ownermodule, key, temp.Arg1);
                    } else {
                        //存在就调用一下执行方法
                        SendMessage<T1, T2>(ownermodule, key, temp.Arg1, temp.Arg2);
                    }
                    note = note.next;
                } while (note != null);
                m_CacheDic.Remove(key);//移除
            }
        }
        public static void Register<T1, T2, T3, T4>(int ownermodule, int key, Action<T1, T2, T3, T4> action) {
            Dictionary<int, IMessageAction> msgdatadic;
            if (m_ActionDic.TryGetValue(ownermodule, out msgdatadic)) {
                if (msgdatadic.TryGetValue(key, out var previousAction)) {
                    if (previousAction is MessageAction<T1, T2, T3, T4> messageData) {
                        messageData.MessageEvents += action;
                    }
                } else {
                    MessageAction<T1, T2, T3, T4> msgdata = new MessageAction<T1, T2, T3, T4>(action);

                    msgdatadic.Add(key, msgdata);
                }
            } else {
                msgdatadic = new Dictionary<int, IMessageAction>();
                MessageAction<T1, T2, T3, T4> msgdata = new MessageAction<T1, T2, T3, T4>(action);

                msgdatadic.Add(key, msgdata);
                m_ActionDic.Add(ownermodule, msgdatadic);
            }
            //判断一下是否存在未执行的操作
            EventNote note = null;
            if (m_CacheDic.TryGetValue(key, out note) && note != null) {
                do {//调用每个节点的对应的脚本的消息处理函数，此处使用的策略模式，分别调用的是每个脚本的消息处理函数
                    CacheMsg<T1, T2, T3, T4> temp = note.curdata as CacheMsg<T1, T2, T3, T4>;
                    if (temp.Isreturn) {
                        //存在就调用一下执行方法
                        SendMessageWithReturn<T1, T2, T3, T4>(ownermodule, key, temp.Arg1, temp.Arg2, temp.Arg3);
                    } else {
                        //存在就调用一下执行方法
                        SendMessage<T1, T2, T3, T4>(ownermodule, key, temp.Arg1, temp.Arg2, temp.Arg3, temp.Arg4);
                    }
                    note = note.next;
                } while (note != null);
                m_CacheDic.Remove(key);//移除
            }
        }
        public static void Register<T1, T2, T3>(int ownermodule, int key, Action<T1, T2, T3> action) {
            Dictionary<int, IMessageAction> msgdatadic;
            if (m_ActionDic.TryGetValue(ownermodule, out msgdatadic)) {
                if (msgdatadic.TryGetValue(key, out var previousAction)) {
                    if (previousAction is MessageAction<T1, T2, T3> messageData) {
                        messageData.MessageEvents += action;
                    }
                } else {
                    MessageAction<T1, T2, T3> msgdata = new MessageAction<T1, T2, T3>(action);

                    msgdatadic.Add(key, msgdata);
                }
            } else {

                msgdatadic = new Dictionary<int, IMessageAction>();
                MessageAction<T1, T2, T3> msgdata = new MessageAction<T1, T2, T3>(action);

                msgdatadic.Add(key, msgdata);
                m_ActionDic.Add(ownermodule, msgdatadic);
            }
            //判断一下是否存在未执行的操作
            EventNote note = null;
            if (m_CacheDic.TryGetValue(key, out note) && note != null) {
                do {//调用每个节点的对应的脚本的消息处理函数，此处使用的策略模式，分别调用的是每个脚本的消息处理函数
                    CacheMsg<T1, T2, T3> temp = note.curdata as CacheMsg<T1, T2, T3>;
                    if (temp.Isreturn) {
                        //存在就调用一下执行方法
                        SendMessageWithReturn<T1, T2, T3>(ownermodule, key, temp.Arg1, temp.Arg2);
                    } else {
                        //存在就调用一下执行方法
                        SendMessage<T1, T2, T3>(ownermodule, key, temp.Arg1, temp.Arg2, temp.Arg3);
                    }
                    note = note.next;
                } while (note != null);
                m_CacheDic.Remove(key);//移除
            }
           
        }
        #endregion
        #region Func Register
        public static void Register<T1>(int ownermodule, int key, Func<T1> func) {
            Dictionary<int, IMessageAction> msgdatadic;
            if (m_ActionDic.TryGetValue(ownermodule, out msgdatadic)) {
                if (msgdatadic.TryGetValue(key, out var previousAction)) {
                    if (previousAction is MessageFun<T1> messageData) {
                        messageData.MessageEvents += func;
                    }
                } else {
                    MessageFun<T1> msgdata = new MessageFun<T1>(func);
                    msgdatadic.Add(key, msgdata);
                }
            } else {
                msgdatadic = new Dictionary<int, IMessageAction>();
                MessageFun<T1> msgdata = new MessageFun<T1>(func);
                msgdatadic.Add(key, msgdata);
                m_ActionDic.Add(ownermodule, msgdatadic);
            }
        }
        public static void Register<T1,T2>(int ownermodule, int key, Func<T1, T2> func) {
            Dictionary<int, IMessageAction> msgdatadic;
            if (m_ActionDic.TryGetValue(ownermodule, out msgdatadic)) {
                if (msgdatadic.TryGetValue(key, out var previousAction)) {
                    if (previousAction is MessageFun<T1, T2> messageData) {
                        messageData.MessageEvents += func;
                    }
                } else {
                    MessageFun<T1, T2> msgdata = new MessageFun<T1, T2>(func);
                    msgdatadic.Add(key, msgdata);
                }
            } else {
                msgdatadic = new Dictionary<int, IMessageAction>();
                MessageFun<T1, T2> msgdata = new MessageFun<T1, T2>(func);
                msgdatadic.Add(key, msgdata);
                m_ActionDic.Add(ownermodule, msgdatadic);
            }
        }
        public static void Register<T1, T2,T3>(int ownermodule, int key, Func<T1, T2, T3> func) {
            Dictionary<int, IMessageAction> msgdatadic;
            if (m_ActionDic.TryGetValue(ownermodule, out msgdatadic)) {
                if (msgdatadic.TryGetValue(key, out var previousAction)) {
                    if (previousAction is MessageFun<T1, T2, T3> messageData) {
                        messageData.MessageEvents += func;
                    }
                } else {
                    MessageFun<T1, T2, T3> msgdata = new MessageFun<T1, T2, T3>(func);
                    msgdatadic.Add(key, msgdata);
                }
            } else {
                msgdatadic = new Dictionary<int, IMessageAction>();
                MessageFun<T1, T2, T3> msgdata = new MessageFun<T1, T2, T3>(func);
                msgdatadic.Add(key, msgdata);
                m_ActionDic.Add(ownermodule, msgdatadic);
            }
        }
        public static void Register<T1, T2, T3, T4>(int ownermodule, int key, Func<T1, T2, T3, T4> func) {
            Dictionary<int, IMessageAction> msgdatadic;
            if (m_ActionDic.TryGetValue(ownermodule, out msgdatadic)) {
                if (msgdatadic.TryGetValue(key, out var previousAction)) {
                    if (previousAction is MessageFun<T1, T2, T3, T4> messageData) {
                        messageData.MessageEvents += func;
                    }
                } else {
                    MessageFun<T1, T2, T3, T4> msgdata = new MessageFun<T1, T2, T3, T4>(func);
                    msgdatadic.Add(key, msgdata);
                }
            } else {
                msgdatadic = new Dictionary<int, IMessageAction>();
                MessageFun<T1, T2, T3, T4> msgdata = new MessageFun<T1, T2, T3, T4>(func);
                msgdatadic.Add(key, msgdata);
                m_ActionDic.Add(ownermodule, msgdatadic);
            }
        }
        #endregion
        #region Action Remove
        public static void Remove(int ownermodule, int key, Action action) {
            Dictionary<int, IMessageAction> msgdatadic;
            if (m_ActionDic.TryGetValue(ownermodule, out msgdatadic)) {
                if (msgdatadic.TryGetValue(key, out var previousAction)) {
                    if (previousAction is MessageAction messageData) {
                        messageData.MessageEvents -= action;
                    }
                } else {
                    Debuger.LogWarning($"{ownermodule}模块不存在{key}方法，请检查");
                }
            } else {
                Debuger.LogWarning($"{ownermodule}模块还未启动或者不存在，请检查");
            }
        }

        public static void Remove<T1>(int ownermodule, int key, Action<T1> action) {
            Dictionary<int, IMessageAction> msgdatadic;
            if (m_ActionDic.TryGetValue(ownermodule, out msgdatadic)) {
                if (msgdatadic.TryGetValue(key, out var previousAction)) {
                    if (previousAction is MessageAction<T1> messageData) {
                        messageData.MessageEvents -= action;
                    }
                } else {
                    Debuger.LogWarning($"{ownermodule}模块不存在{key}方法，请检查");
                }
            } else {
                Debuger.LogWarning($"{ownermodule}模块还未启动或者不存在，请检查");
            }
        }
        public static void Remove<T1, T2>(int ownermodule, int key, Action<T1, T2> action) {
            Dictionary<int, IMessageAction> msgdatadic;
            if (m_ActionDic.TryGetValue(ownermodule, out msgdatadic)) {
                if (msgdatadic.TryGetValue(key, out var previousAction)) {
                    if (previousAction is MessageAction<T1, T2> messageData) {
                        messageData.MessageEvents -= action;
                    }
                } else {
                    Debuger.LogWarning($"{ownermodule}模块不存在{key}方法，请检查");
                }
            } else {
                Debuger.LogWarning($"{ownermodule}模块还未启动或者不存在，请检查");
            }
        }
        public static void Remove<T1, T2, T3>(int ownermodule, int key, Action<T1, T2, T3> action) {
            Dictionary<int, IMessageAction> msgdatadic;
            if (m_ActionDic.TryGetValue(ownermodule, out msgdatadic)) {
                if (msgdatadic.TryGetValue(key, out var previousAction)) {
                    if (previousAction is MessageAction<T1, T2, T3> messageData) {
                        messageData.MessageEvents -= action;
                    }
                } else {
                    Debuger.LogWarning($"{ownermodule}模块不存在{key}方法，请检查");
                }
            } else {
                Debuger.LogWarning($"{ownermodule}模块还未启动或者不存在，请检查");
            }
        }
        public static void Remove<T1, T2, T3, T4>(int ownermodule, int key, Action<T1, T2, T3, T4> action) {
            Dictionary<int, IMessageAction> msgdatadic;
            if (m_ActionDic.TryGetValue(ownermodule, out msgdatadic)) {
                if (msgdatadic.TryGetValue(key, out var previousAction)) {
                    if (previousAction is MessageAction<T1, T2, T3, T4> messageData) {
                        messageData.MessageEvents -= action;
                    }
                } else {
                    Debuger.LogWarning($"{ownermodule}模块不存在{key}方法，请检查");
                }
            } else {
                Debuger.LogWarning($"{ownermodule}模块还未启动或者不存在，请检查");
            }
        }
        #endregion
        /// <summary>
        /// 判断模块是否创建
        /// </summary>
        /// <param name="moduleid"></param>
        /// <returns></returns>
        public static bool IsHasModule(int moduleid) { 
            string modulename;
            if (m_ModuleNameDic.TryGetValue(moduleid, out modulename)) {
                if (ModuleManager.GetInstance.HasModule(modulename)) {
                    return true;
                } else {
                    return false;
                }
            } else {
                Debuger.Log($"没有id为{moduleid}的模块");
                return false;
            }
        }
       
        //一个msgid注册一个节点 方法的重载
        public static void RegistCache(int msgid, EventNote note) {
            if (m_CacheDic.ContainsKey(msgid)) {
                EventNote tmpnote = m_CacheDic[msgid];
                while (tmpnote.next != null) {
                    tmpnote = tmpnote.next;
                }
                tmpnote.next = note;
            } else {
                m_CacheDic.Add(msgid, note);
            }
        }
        //释放节点,1、节点在尾部时 2、节点在中间时  3、节点在头部时
        public static void UnRegisCache(int msgid, ICacheMsg data) {
            if (m_CacheDic.ContainsKey(msgid)) {
                EventNote temp = m_CacheDic[msgid];
                if (temp.curdata == data) { //3,节点在头部s
                    EventNote header = temp;
                    if (temp.next != null) {//有后继节点
                        header.curdata = temp.next.curdata;
                        header.next = temp.next.next;
                    } else {//没有后继节点
                        header = null;
                        temp = null;
                        m_CacheDic.Remove(msgid);
                    }
                } else {//1，2的情况
                    while (temp.next != null && temp.next.curdata != data) {
                        temp = temp.next;
                    }
                    if (temp.next.next != null) {//2的情况
                        temp.next = temp.next.next;
                    } else {//1的情况
                        temp.next = null;
                    }
                }
            } else {
                Debuger.Log("节点列表中不存在" + msgid);
            }
        }
        public static void SendMessage(int ownermodule, int key) {
            if (IsHasModule(ownermodule)) {
                if (m_ActionDic[ownermodule].TryGetValue(key, out var previousAction)) {
                    (previousAction as MessageAction)?.MessageEvents?.Invoke();
                } else {
                    CacheMsg temp = new CacheMsg();
                    temp.Key = key;
                    temp.Module = ownermodule;

                    RegistCache(key, new EventNote(temp));
                    Debuger.Log($"模块{ownermodule}未启动，消息{key}已经缓存");
                }
            } else {
                CacheMsg temp = new CacheMsg();
                temp.Key = key;
                temp.Module = ownermodule;
                RegistCache(key, new EventNote(temp));
                Debuger.Log($"模块{ownermodule}未启动，消息{key}已经缓存");
            }

        }
        public static void SendMessage<T1>(int ownermodule, int key, T1 data1) {
            if (IsHasModule(ownermodule)) {
                if (m_ActionDic[ownermodule].TryGetValue(key, out var previousAction)) {
                    (previousAction as MessageAction<T1>)?.MessageEvents?.Invoke(data1);
                } else {
                    CacheMsg<T1> temp = new CacheMsg<T1>();
                    temp.Key = key;
                    temp.Module = ownermodule;
                    temp.Arg1 = data1;
                    temp.Isreturn = false;
                    RegistCache(key, new EventNote(temp));
                    Debuger.Log($"模块{ownermodule}未启动，消息{key}已经缓存");
                }
            } else {
                CacheMsg<T1> temp = new CacheMsg<T1>();
                temp.Key = key;
                temp.Module = ownermodule;
                temp.Arg1 = data1;
                temp.Isreturn = false;
                RegistCache(key, new EventNote(temp));
                Debuger.Log($"模块{ownermodule}未启动，消息{key}已经缓存");
            }
        }
        public static void SendMessage<T1, T2>(int ownermodule, int key, T1 data1, T2 data2) {
            if (IsHasModule(ownermodule)) {
                if (m_ActionDic[ownermodule].TryGetValue(key, out var previousAction)) {
                    (previousAction as MessageAction<T1, T2>)?.MessageEvents?.Invoke(data1, data2);
                } else {
                    CacheMsg<T1,T2> temp = new CacheMsg<T1,T2>();
                    temp.Key = key;
                    temp.Module = ownermodule;
                    temp.Arg1 = data1;
                    temp.Arg2 = data2;
                    temp.Isreturn = false;
                    RegistCache(key, new EventNote(temp));
                    Debuger.Log($"模块{ownermodule}未启动，消息{key}已经缓存");
                }
            } else {
                CacheMsg<T1, T2> temp = new CacheMsg<T1, T2>();
                temp.Key = key;
                temp.Module = ownermodule;
                temp.Arg1 = data1;
                temp.Arg2 = data2;
                temp.Isreturn = false;
                RegistCache(key, new EventNote(temp));
                Debuger.Log($"模块{ownermodule}未启动，消息{key}已经缓存");
            }
        }
        public static void SendMessage<T1, T2, T3>(int ownermodule, int key, T1 data1, T2 data2, T3 data3) {
            if (IsHasModule(ownermodule)) {
                if (m_ActionDic[ownermodule].TryGetValue(key, out var previousAction)) {
                    (previousAction as MessageAction<T1, T2, T3>)?.MessageEvents?.Invoke(data1, data2, data3);
                } else {
                    CacheMsg<T1, T2,T3> temp = new CacheMsg<T1, T2,T3>();
                    temp.Key = key;
                    temp.Module = ownermodule;
                    temp.Arg1 = data1;
                    temp.Arg2 = data2;
                    temp.Arg3 = data3;
                    temp.Isreturn = false;
                    RegistCache(key, new EventNote(temp));
                    Debuger.Log($"模块{ownermodule}未启动，消息{key}已经缓存");
                }
            } else {
                CacheMsg<T1, T2, T3> temp = new CacheMsg<T1, T2, T3>();
                temp.Key = key;
                temp.Module = ownermodule;
                temp.Arg1 = data1;
                temp.Arg2 = data2;
                temp.Arg3 = data3;
                temp.Isreturn = false;
                RegistCache(key, new EventNote(temp));
                Debuger.Log($"模块{ownermodule}未启动，消息{key}已经缓存");
            }
            
        }
        public static void SendMessage<T1, T2, T3, T4>(int ownermodule, int key, T1 data1, T2 data2, T3 data3, T4 data4) {
            if (IsHasModule(ownermodule)) {
                if (m_ActionDic[ownermodule].TryGetValue(key, out var previousAction)) {
                    (previousAction as MessageAction<T1, T2, T3, T4>)?.MessageEvents?.Invoke(data1, data2, data3, data4);
                } else {
                    CacheMsg<T1, T2, T3,T4> temp = new CacheMsg<T1, T2, T3,T4>();
                    temp.Key = key;
                    temp.Module = ownermodule;
                    temp.Arg1 = data1;
                    temp.Arg2 = data2;
                    temp.Arg3 = data3;
                    temp.Arg4 = data4;
                    temp.Isreturn = false;
                    RegistCache(key, new EventNote(temp));
                    Debuger.Log($"模块{ownermodule}未启动，消息{key}已经缓存");
                }
            } else {
                CacheMsg<T1, T2, T3, T4> temp = new CacheMsg<T1, T2, T3, T4>();
                temp.Key = key;
                temp.Module = ownermodule;
                temp.Arg1 = data1;
                temp.Arg2 = data2;
                temp.Arg3 = data3;
                temp.Arg4 = data4;
                temp.Isreturn = false;
                RegistCache(key, new EventNote(temp));
                Debuger.Log($"模块{ownermodule}未启动，消息{key}已经缓存");
            }
        }

        /// <summary>
        /// 带返回值的发送消息，
        /// 内部使用Func<T>事件，而不是Action<T>了
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="ownermodule"></param>
        /// <param name="key"></param>
        /// <param name="data1"></param>
        /// <returns></returns>
        public static T1 SendMessageWithReturn<T1>(int ownermodule, int key) {
            if (IsHasModule(ownermodule)) {
                if (m_ActionDic[ownermodule].TryGetValue(key, out var previousAction)) {
                    return (previousAction as MessageFun<T1>).MessageEvents();
                } else {
                    Debuger.LogError($"注册事件{key}不存在，可能还未注册，请检查脚本启动顺序");
                    return default;
                }
            } else {
                Debuger.LogError($"注册事件{key}模块还未创建");
                return default;
            }
        }
        //Func后面可以跟很多类型，最后一个类型则是返回值类型
        //前面的类型为参数类型
        //参数类型必须跟指向的方法的参数类型按照顺序一一对应
        public static T2 SendMessageWithReturn<T1, T2>(int ownermodule, int key, T1 data1) {
            if (IsHasModule(ownermodule)) {
                if (m_ActionDic[ownermodule].TryGetValue(key, out var previousAction)) {
                    return (previousAction as MessageFun<T1, T2>).MessageEvents(data1);
                } else {
                    Debuger.LogError($"注册事件{key}不存在");
                    return default;
                }
            } else {
                Debuger.LogError($"注册事件{key}模块还未创建");
                return default;
            }
           
        }
        public static T3 SendMessageWithReturn<T1, T2,T3>(int ownermodule, int key, T1 data1, T2 data2) {
            if (IsHasModule(ownermodule)) {
                if (m_ActionDic[ownermodule].TryGetValue(key, out var previousAction)) {
                    return (previousAction as MessageFun<T1, T2, T3>).MessageEvents(data1, data2);
                } else {
                    Debuger.LogError($"注册事件{key}不存在");
                    return default;
                }
            } else {
                Debuger.LogError($"注册事件{key}模块还未创建");
                return default;
            }
           
        }
        public static T4 SendMessageWithReturn<T1, T2, T3,T4>(int ownermodule, int key, T1 data1, T2 data2,T3 data3) {
            if (IsHasModule(ownermodule)) {
                if (m_ActionDic[ownermodule].TryGetValue(key, out var previousAction)) {
                    return (previousAction as MessageFun<T1, T2, T3, T4>).MessageEvents(data1, data2, data3);
                } else {
                    Debuger.LogError($"注册事件{key}不存在");
                    return default;
                }
            } else {
                Debuger.LogError($"注册事件{key}模块还未创建");
                return default;
            }
        }
        public static void ClearMsgCeneter() {
            m_ActionDic.Clear();
        }
        /// <summary>
        /// 清除一个模块所有的消息委托，一般用于某个模块被销毁时
        /// 消息除非必要一般不用自己销毁，只需要注册就行，每次都由所属的模块管理器统一清除
        /// </summary>
        /// <param name="moduleid"></param>
        public static void ClearModuleMsg(int moduleid) {
            if (m_ActionDic.ContainsKey(moduleid)) {
                foreach (IMessageAction msg in m_ActionDic[moduleid].Values) {
                    msg.ClearAciton();
                }
                m_ActionDic[moduleid].Clear();
            }
            //该模块清理缓存中的消息   有必要清理吗？这种情况太极端了，先不做吧
        }
        #endregion
    }

}



using GF.Module;
using GF.Utils;
using System;
using System.Collections.Generic;

namespace GF.Msg {
    /// <summary>
    /// ÿ����Ϣ�ڵ����
    /// </summary>
    public class EventNote {//ÿ����Ϣ���ű�����Ӧ�Ľڵ㣬ʹ�õ�������ķ�ʽ�洢
        public ICacheMsg curdata;//�ýڵ㵱ǰ������Ϣ
        public EventNote next;//�ýڵ����һ������ڵ�
        public EventNote(ICacheMsg data) {//���캯�����ڵ㸳ֵ
            this.curdata = data;
            this.next = null;
        }
    }
    /// <summary>
    /// ������Ϣ�Ľӿ�,�߱�����ֵ����Ϣ��֧�ֻ���
    /// </summary>
    public interface ICacheMsg {
        int Key { get; set; }//�¼�id
        int Module { get; set; }//�¼�����ģ��
    }
    /// <summary>
    /// ������Ϣ��ʵ��
    /// </summary>
    public class CacheMsg : ICacheMsg {
        public int Key { get; set; }//�¼�id
        public int Module { get; set; }//�¼�����ģ��
       
    }
    public class CacheMsg<T1> : ICacheMsg {
        public int Key { get; set; }//�¼�id
        public int Module { get; set; }//�¼�����ģ��
        public bool Isreturn { get; set; }//�Ƿ���Ҫ����ֵ


        public T1 Arg1;
    }
    public class CacheMsg<T1,T2> : ICacheMsg {
        public int Key { get; set; }//�¼�id
        public int Module { get; set; }//�¼�����ģ��
        public bool Isreturn { get; set; }//�Ƿ���Ҫ����ֵ
        public T1 Arg1;
        public T2 Arg2;
    }
    public class CacheMsg<T1, T2,T3> : ICacheMsg {
        public int Key { get; set; }//�¼�id
        public int Module { get; set; }//�¼�����ģ��
        public bool Isreturn { get; set; }//�Ƿ���Ҫ����ֵ
        public T1 Arg1;
        public T2 Arg2;
        public T3 Arg3;
    }
    public class CacheMsg<T1, T2, T3,T4> : ICacheMsg {
        public int Key { get; set; }//�¼�id
        public int Module { get; set; }//�¼�����ģ��
        public bool Isreturn { get; set; }//�Ƿ���Ҫ����ֵ
        public T1 Arg1;
        public T2 Arg2;
        public T3 Arg3;
        public T4 Arg4;
    }
    public static class MsgCenter {
        #region ��Ϣ�������
        /// <summary>
        /// ���string��ģ������,֮��������һ��ģ��������Ϊ�˺��水ģ��������Ϣ���������Э��ʱ����Ϣ�����ظ���������˿���������û��ҪǶ��
        /// �ڲ�string��ע����¼�����
        /// IMessageData����Ҳ����ģ������Ϊ�˷������ʹ��
        /// </summary>
        private static Dictionary<int, Dictionary<int, IMessageAction>> m_ActionDic;
        private static Dictionary<int, EventNote> m_CacheDic;//������Ϊģ��δ�������µ���Ϣ,key���¼���id,��Ϣ���治�����ֶΣ��˴�Ӧ�ÿ���ʹ��������Ϊһ����Ϣ���ܷ��Ͷ�Σ�����ʱ����Ҫ��˳�򰤸�ִ��
        private static Dictionary<int, string> m_ModuleNameDic;

        public static void Init(Dictionary<int, string> modulenamedic) {
            m_ActionDic = new Dictionary<int, Dictionary<int, IMessageAction>>();
            m_CacheDic = new Dictionary<int, EventNote>();
            m_ModuleNameDic = modulenamedic;
        }
        /// <summary>
        /// ͨ�����ƻ�ȡģ��id
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
            //�ж�һ���Ƿ����δִ�еĲ���
            EventNote note = null;
            if (m_CacheDic.TryGetValue(key,out note) && note != null) {
                do {//����ÿ���ڵ�Ķ�Ӧ�Ľű�����Ϣ���������˴�ʹ�õĲ���ģʽ���ֱ���õ���ÿ���ű�����Ϣ������
                    CacheMsg temp = note.curdata as CacheMsg;
                    //���ھ͵���һ��ִ�з���
                    SendMessage(ownermodule, key);
                    note = note.next;
                } while (note != null);
                m_CacheDic.Remove(key);//�Ƴ�
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
            //�ж�һ���Ƿ����δִ�еĲ���
            EventNote note = null;
            if (m_CacheDic.TryGetValue(key, out note) && note != null) {
                do {//����ÿ���ڵ�Ķ�Ӧ�Ľű�����Ϣ���������˴�ʹ�õĲ���ģʽ���ֱ���õ���ÿ���ű�����Ϣ������
                    CacheMsg<T1> temp = note.curdata as CacheMsg<T1>;
                    if (temp.Isreturn) {
                        //���ھ͵���һ��ִ�з���
                        SendMessageWithReturn<T1>(ownermodule, key);
                    } else {
                        //���ھ͵���һ��ִ�з���
                        SendMessage<T1>(ownermodule, key, temp.Arg1);
                    }
                    note = note.next;
                } while (note != null);
                m_CacheDic.Remove(key);//�Ƴ�
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
            //�ж�һ���Ƿ����δִ�еĲ���
            EventNote note = null;
            if (m_CacheDic.TryGetValue(key, out note) && note != null) {
                do {//����ÿ���ڵ�Ķ�Ӧ�Ľű�����Ϣ���������˴�ʹ�õĲ���ģʽ���ֱ���õ���ÿ���ű�����Ϣ������
                    CacheMsg<T1, T2> temp = note.curdata as CacheMsg<T1, T2>;
                    if (temp.Isreturn) {
                        //���ھ͵���һ��ִ�з���
                        SendMessageWithReturn<T1, T2>(ownermodule, key, temp.Arg1);
                    } else {
                        //���ھ͵���һ��ִ�з���
                        SendMessage<T1, T2>(ownermodule, key, temp.Arg1, temp.Arg2);
                    }
                    note = note.next;
                } while (note != null);
                m_CacheDic.Remove(key);//�Ƴ�
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
            //�ж�һ���Ƿ����δִ�еĲ���
            EventNote note = null;
            if (m_CacheDic.TryGetValue(key, out note) && note != null) {
                do {//����ÿ���ڵ�Ķ�Ӧ�Ľű�����Ϣ���������˴�ʹ�õĲ���ģʽ���ֱ���õ���ÿ���ű�����Ϣ������
                    CacheMsg<T1, T2, T3, T4> temp = note.curdata as CacheMsg<T1, T2, T3, T4>;
                    if (temp.Isreturn) {
                        //���ھ͵���һ��ִ�з���
                        SendMessageWithReturn<T1, T2, T3, T4>(ownermodule, key, temp.Arg1, temp.Arg2, temp.Arg3);
                    } else {
                        //���ھ͵���һ��ִ�з���
                        SendMessage<T1, T2, T3, T4>(ownermodule, key, temp.Arg1, temp.Arg2, temp.Arg3, temp.Arg4);
                    }
                    note = note.next;
                } while (note != null);
                m_CacheDic.Remove(key);//�Ƴ�
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
            //�ж�һ���Ƿ����δִ�еĲ���
            EventNote note = null;
            if (m_CacheDic.TryGetValue(key, out note) && note != null) {
                do {//����ÿ���ڵ�Ķ�Ӧ�Ľű�����Ϣ���������˴�ʹ�õĲ���ģʽ���ֱ���õ���ÿ���ű�����Ϣ������
                    CacheMsg<T1, T2, T3> temp = note.curdata as CacheMsg<T1, T2, T3>;
                    if (temp.Isreturn) {
                        //���ھ͵���һ��ִ�з���
                        SendMessageWithReturn<T1, T2, T3>(ownermodule, key, temp.Arg1, temp.Arg2);
                    } else {
                        //���ھ͵���һ��ִ�з���
                        SendMessage<T1, T2, T3>(ownermodule, key, temp.Arg1, temp.Arg2, temp.Arg3);
                    }
                    note = note.next;
                } while (note != null);
                m_CacheDic.Remove(key);//�Ƴ�
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
                    Debuger.LogWarning($"{ownermodule}ģ�鲻����{key}����������");
                }
            } else {
                Debuger.LogWarning($"{ownermodule}ģ�黹δ�������߲����ڣ�����");
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
                    Debuger.LogWarning($"{ownermodule}ģ�鲻����{key}����������");
                }
            } else {
                Debuger.LogWarning($"{ownermodule}ģ�黹δ�������߲����ڣ�����");
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
                    Debuger.LogWarning($"{ownermodule}ģ�鲻����{key}����������");
                }
            } else {
                Debuger.LogWarning($"{ownermodule}ģ�黹δ�������߲����ڣ�����");
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
                    Debuger.LogWarning($"{ownermodule}ģ�鲻����{key}����������");
                }
            } else {
                Debuger.LogWarning($"{ownermodule}ģ�黹δ�������߲����ڣ�����");
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
                    Debuger.LogWarning($"{ownermodule}ģ�鲻����{key}����������");
                }
            } else {
                Debuger.LogWarning($"{ownermodule}ģ�黹δ�������߲����ڣ�����");
            }
        }
        #endregion
        /// <summary>
        /// �ж�ģ���Ƿ񴴽�
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
                Debuger.Log($"û��idΪ{moduleid}��ģ��");
                return false;
            }
        }
       
        //һ��msgidע��һ���ڵ� ����������
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
        //�ͷŽڵ�,1���ڵ���β��ʱ 2���ڵ����м�ʱ  3���ڵ���ͷ��ʱ
        public static void UnRegisCache(int msgid, ICacheMsg data) {
            if (m_CacheDic.ContainsKey(msgid)) {
                EventNote temp = m_CacheDic[msgid];
                if (temp.curdata == data) { //3,�ڵ���ͷ��s
                    EventNote header = temp;
                    if (temp.next != null) {//�к�̽ڵ�
                        header.curdata = temp.next.curdata;
                        header.next = temp.next.next;
                    } else {//û�к�̽ڵ�
                        header = null;
                        temp = null;
                        m_CacheDic.Remove(msgid);
                    }
                } else {//1��2�����
                    while (temp.next != null && temp.next.curdata != data) {
                        temp = temp.next;
                    }
                    if (temp.next.next != null) {//2�����
                        temp.next = temp.next.next;
                    } else {//1�����
                        temp.next = null;
                    }
                }
            } else {
                Debuger.Log("�ڵ��б��в�����" + msgid);
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
                    Debuger.Log($"ģ��{ownermodule}δ��������Ϣ{key}�Ѿ�����");
                }
            } else {
                CacheMsg temp = new CacheMsg();
                temp.Key = key;
                temp.Module = ownermodule;
                RegistCache(key, new EventNote(temp));
                Debuger.Log($"ģ��{ownermodule}δ��������Ϣ{key}�Ѿ�����");
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
                    Debuger.Log($"ģ��{ownermodule}δ��������Ϣ{key}�Ѿ�����");
                }
            } else {
                CacheMsg<T1> temp = new CacheMsg<T1>();
                temp.Key = key;
                temp.Module = ownermodule;
                temp.Arg1 = data1;
                temp.Isreturn = false;
                RegistCache(key, new EventNote(temp));
                Debuger.Log($"ģ��{ownermodule}δ��������Ϣ{key}�Ѿ�����");
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
                    Debuger.Log($"ģ��{ownermodule}δ��������Ϣ{key}�Ѿ�����");
                }
            } else {
                CacheMsg<T1, T2> temp = new CacheMsg<T1, T2>();
                temp.Key = key;
                temp.Module = ownermodule;
                temp.Arg1 = data1;
                temp.Arg2 = data2;
                temp.Isreturn = false;
                RegistCache(key, new EventNote(temp));
                Debuger.Log($"ģ��{ownermodule}δ��������Ϣ{key}�Ѿ�����");
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
                    Debuger.Log($"ģ��{ownermodule}δ��������Ϣ{key}�Ѿ�����");
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
                Debuger.Log($"ģ��{ownermodule}δ��������Ϣ{key}�Ѿ�����");
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
                    Debuger.Log($"ģ��{ownermodule}δ��������Ϣ{key}�Ѿ�����");
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
                Debuger.Log($"ģ��{ownermodule}δ��������Ϣ{key}�Ѿ�����");
            }
        }

        /// <summary>
        /// ������ֵ�ķ�����Ϣ��
        /// �ڲ�ʹ��Func<T>�¼���������Action<T>��
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
                    Debuger.LogError($"ע���¼�{key}�����ڣ����ܻ�δע�ᣬ����ű�����˳��");
                    return default;
                }
            } else {
                Debuger.LogError($"ע���¼�{key}ģ�黹δ����");
                return default;
            }
        }
        //Func������Ը��ܶ����ͣ����һ���������Ƿ���ֵ����
        //ǰ�������Ϊ��������
        //�������ͱ����ָ��ķ����Ĳ������Ͱ���˳��һһ��Ӧ
        public static T2 SendMessageWithReturn<T1, T2>(int ownermodule, int key, T1 data1) {
            if (IsHasModule(ownermodule)) {
                if (m_ActionDic[ownermodule].TryGetValue(key, out var previousAction)) {
                    return (previousAction as MessageFun<T1, T2>).MessageEvents(data1);
                } else {
                    Debuger.LogError($"ע���¼�{key}������");
                    return default;
                }
            } else {
                Debuger.LogError($"ע���¼�{key}ģ�黹δ����");
                return default;
            }
           
        }
        public static T3 SendMessageWithReturn<T1, T2,T3>(int ownermodule, int key, T1 data1, T2 data2) {
            if (IsHasModule(ownermodule)) {
                if (m_ActionDic[ownermodule].TryGetValue(key, out var previousAction)) {
                    return (previousAction as MessageFun<T1, T2, T3>).MessageEvents(data1, data2);
                } else {
                    Debuger.LogError($"ע���¼�{key}������");
                    return default;
                }
            } else {
                Debuger.LogError($"ע���¼�{key}ģ�黹δ����");
                return default;
            }
           
        }
        public static T4 SendMessageWithReturn<T1, T2, T3,T4>(int ownermodule, int key, T1 data1, T2 data2,T3 data3) {
            if (IsHasModule(ownermodule)) {
                if (m_ActionDic[ownermodule].TryGetValue(key, out var previousAction)) {
                    return (previousAction as MessageFun<T1, T2, T3, T4>).MessageEvents(data1, data2, data3);
                } else {
                    Debuger.LogError($"ע���¼�{key}������");
                    return default;
                }
            } else {
                Debuger.LogError($"ע���¼�{key}ģ�黹δ����");
                return default;
            }
        }
        public static void ClearMsgCeneter() {
            m_ActionDic.Clear();
        }
        /// <summary>
        /// ���һ��ģ�����е���Ϣί�У�һ������ĳ��ģ�鱻����ʱ
        /// ��Ϣ���Ǳ�Ҫһ�㲻���Լ����٣�ֻ��Ҫע����У�ÿ�ζ���������ģ�������ͳһ���
        /// </summary>
        /// <param name="moduleid"></param>
        public static void ClearModuleMsg(int moduleid) {
            if (m_ActionDic.ContainsKey(moduleid)) {
                foreach (IMessageAction msg in m_ActionDic[moduleid].Values) {
                    msg.ClearAciton();
                }
                m_ActionDic[moduleid].Clear();
            }
            //��ģ���������е���Ϣ   �б�Ҫ�������������̫�����ˣ��Ȳ�����
        }
        #endregion
    }

}

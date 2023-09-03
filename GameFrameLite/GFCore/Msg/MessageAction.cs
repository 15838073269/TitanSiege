using System;
namespace GF.Msg {
    public interface IMessageAction {
        void ClearAciton();
    }

    public class MessageAction<T1> : IMessageAction {
        
        public Action<T1> MessageEvents;

        public MessageAction(Action<T1> action) {
            MessageEvents += action;
        }

        public void ClearAciton() {
            MessageEvents = null;
        }
    }
    public class MessageAction : IMessageAction {
        
        public Action MessageEvents;

        public MessageAction(Action action) {
            MessageEvents += action;
        }
        public void ClearAciton() {
            MessageEvents = null;
        }
    }
    public class MessageAction<T1,T2> : IMessageAction {
        
        public Action<T1, T2> MessageEvents;

        public MessageAction(Action<T1, T2> action) {
            MessageEvents += action;
        }
        public void ClearAciton() {
            MessageEvents = null;
        }
    }
    public class MessageAction<T1,T2,T3> : IMessageAction {
        
        public Action<T1, T2, T3> MessageEvents;

        public MessageAction(Action<T1, T2, T3> action) {
            MessageEvents += action;
        }
        public void ClearAciton() {
            MessageEvents = null;
        }
    }
    public class MessageAction<T1, T2, T3,T4> : IMessageAction {
        public Action<T1, T2, T3,T4> MessageEvents;

        public MessageAction(Action<T1, T2, T3,T4> action) {
            MessageEvents += action;
        }
        public void ClearAciton() {
            MessageEvents = null;
        }
    }
    /// <summary>
    /// ����ֵ��ʱ�����ͣ�Ŀǰֻд��һ��������ֵ�ģ��������Ҫ��������ֵ�ģ��ٷ�װ
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    public class MessageFun<T1> : IMessageAction {

        public Func<T1> MessageEvents;

        public MessageFun(Func<T1> fun) {
            MessageEvents += fun;
        }
        public void ClearAciton() {
            MessageEvents = null;
        }
    }
    //Func������Ը��ܶ����ͣ����һ���������Ƿ���ֵ����
    //ǰ�������Ϊ��������
    //�������ͱ����ָ��ķ����Ĳ������Ͱ���˳��һһ��Ӧ
    public class MessageFun<T1,T2> : IMessageAction {

        public Func<T1,T2> MessageEvents;

        public MessageFun(Func<T1,T2> fun) {
            MessageEvents += fun;
        }
        public void ClearAciton() {
            MessageEvents = null;
        }
    }
    public class MessageFun<T1, T2,T3> : IMessageAction {

        public Func<T1, T2,T3> MessageEvents;

        public MessageFun(Func<T1, T2,T3> fun) {
            MessageEvents += fun;
        }
        public void ClearAciton() {
            MessageEvents = null;
        }
    }
    public class MessageFun<T1, T2, T3,T4> : IMessageAction {

        public Func<T1, T2, T3,T4> MessageEvents;

        public MessageFun(Func<T1, T2, T3,T4> fun) {
            MessageEvents += fun;
        }
        public void ClearAciton() {
            MessageEvents = null;
        }
    }
}


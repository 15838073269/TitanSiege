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
    /// 返回值的时间类型，目前只写了一个带返回值的，如果有需要两个返回值的，再封装
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
    //Func后面可以跟很多类型，最后一个类型则是返回值类型
    //前面的类型为参数类型
    //参数类型必须跟指向的方法的参数类型按照顺序一一对应
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


using UnityEngine.EventSystems;
using UnityEngine;
using System;

namespace GF.Unity.UI {
    /// <summary>
    /// UI的事件，封装于UnityEngine.EventSystems，主要是给UI添加需要的事件的，例如image添加点击事件等。
    /// </summary>
    public class UIEventTrigger:EventTrigger {
        ///应用案例
        //public void Foo() {
        //    GameObject go = new GameObject();
        //    UIEventTrigger.Get(go).OnDrag();
        //}
        public Action onClick;//点击事件
        public Action<GameObject> onClickWithObject;//点击事件传参游戏物体
        public Action<string> onClickWithName;//点击事件传参字符串
        public Action<PointerEventData> onClickWithEvent;//点击事件传参event参数
        public Action<PointerEventData> onDown;//按下
        public Action<PointerEventData> onEnter;//进入
        public Action<PointerEventData> onExit;//离开
        public Action<PointerEventData> onUp;//抬起
        public Action<PointerEventData> onBeginDrag;//开始拖动
        public Action<PointerEventData> onDrag;//拖动
        public Action<PointerEventData> onEndDrag;//结束拖动
        public Action<BaseEventData> onSelect;//选中
        public Action<BaseEventData> onUpdateSelect;//更新选择


        /// <summary>
        /// 获取游戏物体上是否有事件模块，没有的话，就给添加返回
        /// </summary>
        /// <param name="go">游戏物体</param>
        /// <returns></returns>
        public static UIEventTrigger Get(GameObject go) {
            UIEventTrigger listener = go.GetComponent<UIEventTrigger>();
            if (listener == null) {
                listener = go.AddComponent<UIEventTrigger>();
            }
            return listener;
        }

        public static  UIEventTrigger Get(UIBehaviour control) {
            UIEventTrigger listener = control.gameObject.GetComponent<UIEventTrigger>();
            if (listener == null)
                listener = control.gameObject.AddComponent<UIEventTrigger>();
            return listener;
        }

        public static  UIEventTrigger Get(Transform transform) {
            UIEventTrigger listener = transform.gameObject.GetComponent<UIEventTrigger>();
            if (listener == null)
                listener = transform.gameObject.AddComponent<UIEventTrigger>();
            return listener;
        }
        /// <summary>
        /// 是否存在UIEventTrigger
        /// </summary>
        /// <param name="transform"></param>
        /// <returns></returns>
        public static bool HasExistTrigger(Transform transform) {
            return transform.gameObject.GetComponent<UIEventTrigger>() != null;
        }
        //重写click事件的处理方式
        public override void OnPointerClick(PointerEventData eventData) {
            if (onClickWithObject != null)
                onClickWithObject(gameObject);
            if (onClick != null)
                onClick();
            if (onClickWithEvent != null)
                onClickWithEvent(eventData);
            if (onClickWithName != null)
                onClickWithName(gameObject.name);
        }
        public override void OnPointerDown(PointerEventData eventData) {
            if (onDown != null)
                onDown(eventData);
        }
        public override void OnPointerEnter(PointerEventData eventData) {
            if (onEnter != null)
                onEnter(eventData);
        }
        public override void OnPointerExit(PointerEventData eventData) {
            if (onExit != null)
                onExit(eventData);
        }
        public override void OnPointerUp(PointerEventData eventData) {
            if (onUp != null)
                onUp(eventData);
        }
        public override void OnSelect(BaseEventData eventData) {
            if (onSelect != null)
                onSelect(eventData);
        }
        public override void OnUpdateSelected(BaseEventData eventData) {
            if (onUpdateSelect != null)
                onUpdateSelect(eventData);
        }
        public override void OnBeginDrag(PointerEventData eventData) {
            if (onBeginDrag != null) { onBeginDrag(eventData); }
        }
        public override void OnDrag(PointerEventData eventData) {
            if (onDrag != null) { onDrag(eventData); }
        }
        public override void OnEndDrag(PointerEventData eventData) {
            if (onEndDrag != null) { onEndDrag(eventData); }
        }
    }
}

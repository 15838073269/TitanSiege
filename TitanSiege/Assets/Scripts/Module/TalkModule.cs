/****************************************************
    文件：TalkModule.cs
	作者：昔莘
    邮箱: 304183153@qq.com
    日期：2023/10/27 10:52:45
	功能：Nothing
*****************************************************/

using UnityEngine;
using GF.Module;
using GF.MainGame.UI;
using GF.Pool;
using GF.Unity.AB;
using GF.Unity.UI;
using System.Collections.Generic;

namespace GF.MainGame.Module {
    public class TalkModule : GeneralModule {
        public TalkUI m_TalkUI;

        /// <summary>
        /// 对象池
        /// ui的对象池需要单独管理，objectmanger中的对象池是给gameobject物体用的，并没有针对ui，所以ui的对象池需要单独处理一下
        /// </summary>
        private ClassObjectPool<OneTalkUI> m_Pool;
        /// <summary>
        /// 聊天队列，记录最近的100条聊天信息
        /// </summary>
        private Queue<OneTalkUI> TalkQue = new Queue<OneTalkUI>();
        public override void Create() {
            base.Create();
            m_Pool = new ClassObjectPool<OneTalkUI>(100, true);
            m_Pool.CretateObj = CreateOneTalkUIToPool;
            //m_TalkUI = AppMain.GetInstance.uiroot.FindUIInChild<TalkUI>(UIManager.GetInstance.GetUI<MainUIPage>(AppConfig.MainUIPage).transform, "TalkUI");
            AppTools.Regist<string>((int)TalkEvent.AddEmoji, AddEmoji);
            AppTools.Regist<OneTalkUI>((int)TalkEvent.RecycleTalkUI, RecycleTalkUI);
            AppTools.Regist<OneTalkUI>((int)TalkEvent.GetTalkUI, GetTalkUI);
            AppTools.Regist<string>((int)TalkEvent.AddOneTalk, AddOneTalk);
        }
        /// <summary>
        /// 给对象池使用的创建物品ui
        /// </summary>
        /// <returns></returns>
        private OneTalkUI CreateOneTalkUIToPool() {
            //注意通过objectmager加载的ui路径和UImanager加载的不同
            OneTalkUI onetalk = ObjectManager.GetInstance.InstanceObject(AppConfig.OneTalkUI, father: AppMain.GetInstance.uiroot.transform, bClear: false).GetComponent<OneTalkUI>();
            return onetalk;
        }

        /// <summary>
        /// 不需要传输服务器的回收物品对象池
        /// </summary>
        /// <param name="onetalk"></param>
        public void RecycleTalkUI(OneTalkUI onetalk) {
            //处理对象池
            if (m_Pool.Recycle(onetalk)) {
                onetalk.transform.SetParent(AppMain.GetInstance.m_UIPoolRoot.transform);
                onetalk.gameObject.SetActive(false);
            } else {
                Object.Destroy(onetalk.gameObject);
            }
        }
        public OneTalkUI GetTalkUI() {
            OneTalkUI onetalk = m_Pool.GetObj(true);
            if (!onetalk.gameObject.activeSelf) {
                onetalk.gameObject.SetActive(true);
            }
            return onetalk;
        }
        private void AddEmoji(string str) { 
            m_TalkUI.AddEmojiStrInContent(str);
        }
        /// <summary>
        /// 添加对象到内容框，
        /// </summary>
        private void AddOneTalk(string str) {
            OneTalkUI one = GetTalkUI();
            one.m_EmojiText.text = str;
            m_TalkUI.AddOneTalkTo(one);
            TalkQue.Enqueue(one);
            //判断一下队列是否超过100个
            while (TalkQue.Count >= 100) {
               RecycleTalkUI(TalkQue.Dequeue());
            }
        }
        public override void Release() {
            base.Release();
        }
    }
}

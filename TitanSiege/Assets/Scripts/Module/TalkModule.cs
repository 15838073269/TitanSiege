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
using cmd;
using Net.Client;

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
        private Queue<OneTalkUI> m_TalkQue = new Queue<OneTalkUI>();
        //世界发言的单独颜色
        private Color shijie = new Color(0.87f, 0.53f, 0.22f);
        public override void Create() {
            base.Create();
            m_Pool = new ClassObjectPool<OneTalkUI>(100, true);
            m_Pool.CretateObj = CreateOneTalkUIToPool;
            //m_TalkUI = AppMain.GetInstance.uiroot.FindUIInChild<TalkUI>(UIManager.GetInstance.GetUI<MainUIPage>(AppConfig.MainUIPage).transform, "TalkUI");
            AppTools.Regist<string>((int)TalkEvent.AddEmoji, AddEmoji);
            AppTools.Regist<OneTalkUI>((int)TalkEvent.RecycleTalkUI, RecycleTalkUI);
            AppTools.Regist<OneTalkUI>((int)TalkEvent.GetTalkUI, GetTalkUI);
            AppTools.Regist<string, TalkType>((int)TalkEvent.AddOneTalk, AddOneTalk);
            AppTools.Regist<TalkType>((int)TalkEvent.TalkToggleChange, TalkToggleChange);
            //告诉一下系统聊天模块已经加载完了
            ClientBase.Instance.SendRT((ushort)ProtoType.SendTalk, "over", (int)TalkType.系统);
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
        /// <summary>
        /// 从对象池中获取对象
        /// </summary>
        /// <returns></returns>
        public OneTalkUI GetTalkUI() {
            OneTalkUI onetalk = m_Pool.GetObj(true);
            if (!onetalk.gameObject.activeSelf) {
                onetalk.gameObject.SetActive(true);
            }
            return onetalk;
        }
        /// <summary>
        /// 添加表情
        /// </summary>
        /// <param name="str"></param>
        private void AddEmoji(string str) { 
            m_TalkUI.AddEmojiStrInContent(str);
        }
        /// <summary>
        /// 添加对象到内容框
        /// </summary>
        
        private void AddOneTalk(string str,TalkType talktype) {
            OneTalkUI one = GetTalkUI();
            Color c = Color.white;
            if (TalkType.当前 == talktype) {
                c = Color.white;
            } else if (TalkType.世界 == talktype) {
                c = shijie;
            } else if (TalkType.队伍 == talktype) {
                c = Color.blue;
            } else if (TalkType.系统 == talktype) {
                c = Color.red;
            }
            one.Init(talktype,str,c);
            m_TalkUI.AddOneTalkTo(one);
            m_TalkQue.Enqueue(one);
            //判断一下队列是否超过100个
            while (m_TalkQue.Count >= 100) {
               RecycleTalkUI(m_TalkQue.Dequeue());
            }
        }
        private void TalkToggleChange(TalkType talktype) {
            if (TalkType.综合 == talktype) {
                foreach (OneTalkUI talk in m_TalkQue) {
                    talk.gameObject.SetActive(true);
                }
            } else {
                foreach (OneTalkUI talk in m_TalkQue) {
                    if (talk.m_TalkType != talktype) {
                        talk.gameObject.SetActive(false);
                    } else {
                        if (!talk.gameObject.activeSelf) {
                            talk.gameObject.SetActive(true);
                        }
                    }
                }
            }
        }
        public override void Release() {
            base.Release();
        }
    }
}

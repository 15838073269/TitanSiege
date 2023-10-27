/****************************************************
    文件：TalkUI.cs
	作者：昔莘
    邮箱: 304183153@qq.com
    日期：2023/10/27 10:25:37
	功能：Nothing
*****************************************************/

using GF.Service;
using GF.Unity.UI;
using Net.Client;
using Net.Share;
using System;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace GF.MainGame.UI {
    
    public class TalkUI : MonoBehaviour {
        public Button m_Zhankai;//展开按钮
        public Button m_Yincang;//隐藏按钮
        public Button m_Fasong;//发送按钮
        public RectTransform m_Ditu;//展开和隐藏按钮控制的底图 
        public RectTransform m_Fayan;//发言组件的父节点，用来隐藏
        public ScrollRect m_ContentScrollRect;//聊天内容的组件  在540和247之间移动
        public RectTransform m_ContentScrollRectTransform;
        public Scrollbar m_ContentBar;//移动条
        public InputField m_InputField;//输入
        public Dropdown m_Dropdown;//发言类型
        public Button m_Biaoqing;//表情按钮
        public Transform m_BiaoqingPanel;//表情面板

        private short m_Stepi = 0;//三段式隐藏展开标志，>0  展开 <=0 隐藏  0是默认状态
        
        private void Start() {
            m_BiaoqingPanel.gameObject.SetActive(false);
            m_Zhankai.onClick.AddListener(OnZhankai);
            m_Yincang.onClick.AddListener(OnYincang);
            m_Biaoqing.onClick.AddListener(OnBiaoqing);
            m_Fasong.onClick.AddListener(OnFasong);
        }
        /// <summary>
        /// 发送输入内容到服务器分发
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        private void OnFasong() {
            if (m_InputField.text.Length > 0) {
                string typestr = m_Dropdown.captionText.text;
                StringBuilder sb = new StringBuilder();
                TalkType temptype = (TalkType)Enum.Parse(typeof(TalkType), typestr);
                if (TalkType.当前 == temptype) { //发送给服务端场景管理执行
                    sb.Append("【当前】");
                    sb.Append(UserService.GetInstance.m_CurrentChar.Name);
                    sb.Append("：");
                    sb.Append(m_InputField.text);
                    ClientBase.Instance.AddOperation(new Operation(Command.CurrentTalk, ClientBase.Instance.UID) { name = sb.ToString()});
                } else if (TalkType.世界 == temptype) {
                    sb.Append("【世界】");
                    sb.Append(UserService.GetInstance.m_CurrentChar.Name);
                    sb.Append("：");
                    sb.Append(m_InputField.text);
                    ClientBase.Instance.AddOperation(new Operation(Command.CurrentTalk, ClientBase.Instance.UID) { name = sb.ToString() });
                } else if (TalkType.队伍 == temptype) {
                    sb.Append("【队伍】");
                    sb.Append(UserService.GetInstance.m_CurrentChar.Name);
                    sb.Append("：");
                    sb.Append(m_InputField.text);
                    ClientBase.Instance.AddOperation(new Operation(Command.CurrentTalk, ClientBase.Instance.UID) { name = sb.ToString() });
                }
            } else {
                UIManager.GetInstance.OpenUIWidget(AppConfig.UIMsgTips,"你没有输入任何内容，无法发送！");
            }
            
        }

        //点击表情按钮
        private void OnBiaoqing() {
            if (m_BiaoqingPanel.gameObject.activeSelf) {
                m_BiaoqingPanel.gameObject.SetActive(false);
            } else {
                m_BiaoqingPanel.gameObject.SetActive(true);
            }
        }
        /// <summary>
        /// 二段隐藏按钮
        /// </summary>
        private void OnYincang() {
            switch (m_Stepi) {
                case 0://默认状态，此时应该变为-1全隐藏
                    m_Stepi = -1;
                    m_Ditu.transform.localPosition = new Vector3(m_Ditu.transform.localPosition.x, m_Ditu.transform.localPosition.y-305f, m_Ditu.transform.localPosition.z);
                    m_Fayan.gameObject.SetActive(false);
                    m_Yincang.gameObject.SetActive(false);
                    m_Zhankai.gameObject.SetActive(true);
                    break;
                case 1://全展开状态，转变为默认状态
                    m_Stepi = 0;
                    m_Ditu.transform.localPosition = new Vector3(m_Ditu.transform.localPosition.x, m_Ditu.transform.localPosition.y - 237f, m_Ditu.transform.localPosition.z);
                    m_ContentScrollRectTransform.sizeDelta = new Vector2(m_ContentScrollRectTransform.sizeDelta.x, m_ContentScrollRectTransform.sizeDelta.y - 253f);
                    m_Yincang.gameObject.SetActive(true);
                    break;
                case -1://全隐藏状态，不用处理
                    //m_Yincang.gameObject.SetActive(false);
                    break;
                default:
                    m_Stepi = 0;
                    m_Yincang.gameObject.SetActive(true);
                    m_Zhankai.gameObject.SetActive(true);
                    break;
            }
        }
        /// <summary>
        /// 二段展开按钮
        /// </summary>
        private void OnZhankai() {
            switch (m_Stepi) {
                case 0://默认状态，此时应该变为1
                    m_Stepi = 1;
                    m_Ditu.transform.localPosition = new Vector3(m_Ditu.transform.localPosition.x, m_Ditu.transform.localPosition.y + 237f, m_Ditu.transform.localPosition.z);
                    m_ContentScrollRectTransform.sizeDelta = new Vector2(m_ContentScrollRectTransform.sizeDelta.x, m_ContentScrollRectTransform.sizeDelta.y + 253f);
                    m_Yincang.gameObject.SetActive(true);
                    m_Zhankai.gameObject.SetActive(false);
                    break;
                case 1://全展开状态，不用处理
                    //m_Zhankai.gameObject.SetActive(false);
                    break;
                case -1://全隐藏状态，展开成默认
                    m_Stepi = 0;
                    m_Ditu.transform.localPosition = new Vector3(m_Ditu.transform.localPosition.x, m_Ditu.transform.localPosition.y + 305f, m_Ditu.transform.localPosition.z);
                    m_Yincang.gameObject.SetActive(true);
                    m_Zhankai.gameObject.SetActive(true);
                    m_Fayan.gameObject.SetActive(true);
                    break;
                default:
                    m_Stepi = 0;
                    m_Yincang.gameObject.SetActive(true);
                    m_Zhankai.gameObject.SetActive(true);
                    break;
            }
        }
        /// <summary>
        /// 添加emoji表情到输入框
        /// </summary>
        /// <param name="str"></param>
        public void AddEmojiStrInContent(string str) { 
            m_InputField.text += str;
            m_BiaoqingPanel.gameObject.SetActive(false);
            m_InputField.Select();
        }
        /// <summary>
        /// 添加对象到输入框
        /// </summary>
        public void AddOneTalkTo(OneTalkUI one) {
            one.transform.SetParent(m_ContentScrollRect.content);
            m_ContentBar.value = 0;
        }

    }
}

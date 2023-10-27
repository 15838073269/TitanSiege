/****************************************************
    文件：OneTalkUI.cs
	作者：昔莘
    邮箱: 304183153@qq.com
    日期：2023/10/27 10:38:21
	功能：Nothing
*****************************************************/

using UnityEngine;
using UnityEngine.UI;

namespace GF.MainGame.UI {
    public class OneTalkUI : MonoBehaviour {
        public TalkType m_TalkType;
        public EmojiText m_EmojiText;
        public Outline m_Outline;
        public Color m_TextColor;
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="talktype"></param>
        /// <param name="content"></param>
        /// <param name="color"></param>
        public void Init(TalkType talktype,string content,Color color) {
            m_TalkType = talktype;
            m_EmojiText.text = content;
            m_TextColor = color;
            m_Outline.effectColor = m_TextColor;
        }
    }
}

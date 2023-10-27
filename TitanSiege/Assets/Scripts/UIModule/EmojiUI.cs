/****************************************************
    文件：Emojibase.cs
	作者：昔莘
    邮箱: 304183153@qq.com
    日期：2023/10/27 14:57:4
	功能：Nothing
*****************************************************/

using UnityEngine;
using UnityEngine.EventSystems;

namespace GF.MainGame.UI {
    public class EmojiUI : MonoBehaviour, IPointerDownHandler {
        public string m_EmojiKey = "";
        public void OnPointerDown(PointerEventData eventData) {
            if (!string.IsNullOrEmpty(m_EmojiKey)) {
                AppTools.Send<string>((int)TalkEvent.AddEmoji, m_EmojiKey);
            } else {
                Debuger.Log("表情包数据没有配置！");
            }
            
        }
    }
}

/****************************************************
    文件：TouchArea.cs
	作者：昔莘
    邮箱: 304183153@qq.com
    日期：2022/3/2 16:34:46
	功能：Nothing
*****************************************************/

using GF.MainGame.Module;
using GF.Unity.UI;
using UnityEngine;
using UnityEngine.EventSystems;
namespace GF.MainGame.UI {
    public class TouchArea : MonoBehaviour,IDragHandler,IBeginDragHandler {
        private Vector2 startPos;
        public void OnDrag(PointerEventData eventData) {
            float x = -(eventData.position.x - startPos.x);
            AppTools.Send<float>((int)NpcEvent.RotateTo,x);
        }

        public void OnBeginDrag(PointerEventData eventData) {
            startPos = eventData.position;
        }
        

    }
}

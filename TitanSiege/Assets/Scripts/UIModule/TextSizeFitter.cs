/****************************************************
    文件：TextSizeFitter.cs
	作者：昔莘
    邮箱: 304183153@qq.com
    日期：2023/10/26 9:7:39
	功能：Nothing
*****************************************************/

using UnityEngine;
using UnityEngine.UI;
namespace GF.MainGame.UI {
    public class TextSizeFitter : MonoBehaviour {
        //Text最小/最大宽度
        public int textSizeMinHeight = 65;
        public int textSizeMaxHeight = 178;

        int index = 0;
        private void SetTextSize(Text targetText) {
            //设置最优高度
            int textSizewidth = Mathf.CeilToInt(targetText.preferredWidth);
            targetText.rectTransform.sizeDelta = new Vector2(textSizewidth, textSizeMaxHeight);
        }
    }
}

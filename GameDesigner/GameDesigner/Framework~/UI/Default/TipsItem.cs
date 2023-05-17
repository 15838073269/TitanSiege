using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace Framework
{
    public class TipsItem : MonoBehaviour
    {
        public Text info;
        internal float time;

        private async void OnEnable()
        {
            transform.localScale = Vector3.zero;
            transform.DOScale(1f, 0.2f);
            await UniTask.Delay((int)(time * 1000f));
            transform.DOScale(0f, 0.2f);
            await UniTask.Delay(300);
            gameObject.SetActive(false);
            Global.Pool.Recycling(this);
        }
    }
}
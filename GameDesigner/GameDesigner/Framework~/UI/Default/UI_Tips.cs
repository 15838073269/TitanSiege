using Cysharp.Threading.Tasks;

namespace Framework
{
    public class UI_Tips : UIBase<UI_Tips, TipsItem>
    {
        public float hideTime = 1.5f;

        private void Awake()
        {
            item.gameObject.SetActive(false);
        }

        public override void OnShowUI(string title, float progress)
        {
            _ = OnShowTips(title, progress);
        }

        private async UniTaskVoid OnShowTips(string info, float delay)
        {
            await UniTask.Delay((int)(delay * 1000f));
            var item1 = Global.Pool.GetObject<TipsItem>(item, itemRoot);
            item1.info.text = info;
            item1.time = hideTime;
            item1.gameObject.SetActive(true);
            item1.transform.SetAsLastSibling();
        }
    }
}
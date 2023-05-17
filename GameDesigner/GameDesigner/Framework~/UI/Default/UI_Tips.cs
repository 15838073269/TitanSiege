using System;

namespace Framework
{
    public class UI_Tips : UIFormBase<UI_Tips, TipsItem>
    {
        public float hideTime = 1.5f;

        private void Awake()
        {
            item.gameObject.SetActive(false);
        }

        public override void OnShowUI(string title, string info, Action<bool> action)
        {
            var item1 = Global.Pool.GetObject<TipsItem>(item, itemRoot);
            item1.info.text = info;
            item1.time = hideTime;
            item1.gameObject.SetActive(true);
            item1.transform.SetAsLastSibling();
        }
    }
}

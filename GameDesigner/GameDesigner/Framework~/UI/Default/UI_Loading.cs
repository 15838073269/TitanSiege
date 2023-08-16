using UnityEngine.UI;

namespace Framework
{
    public class UI_Loading : UIBase<UI_Loading>
    {
        public Text title;
        public Slider progress;

        public override void OnShowUI(string title, float progress)
        {
            this.title.text = title;
            this.progress.value = progress;
        }
    }
}
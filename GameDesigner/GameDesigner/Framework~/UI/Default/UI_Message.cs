using System;
using UnityEngine.UI;

namespace Framework
{
    public class UI_Message : UIBase<UI_Message>
    {
        public Text title, info;
        public Button confirm, cancel;
        private Action<bool> action;

        // Start is called before the first frame update
        void Start()
        {
            confirm.onClick.AddListener(() => {
                action?.Invoke(true);
                Hide();
            });
            cancel.onClick.AddListener(() => {
                action?.Invoke(false);
                Hide();
            });
        }

        public override void OnShowUI(string title, string info, Action<bool> action)
        {
            this.title.text = title;
            this.info.text = info;
            this.action = action;
        }
    }
}
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace GF.Unity.UI {
    public class UIMsgBox : UIWindow {
        /// <summary>
        /// 定义消息窗口的参数
        /// </summary>
        private UIMsgBoxArg m_arg;
        /// <summary>
        /// 消息窗口的各个UI变量
        /// </summary>
        public Text txtContent;
        public Text ctlTitle;
        public Button[] buttons;//定义多个按钮
        /// <summary>
        /// 在打开消息窗口时，处理消息窗口的参数
        /// </summary>
        /// <param name="args"></param>
        protected override void OnOpen(object args = null) {
            base.OnOpen(args);
            m_arg = args as UIMsgBoxArg;
            txtContent.text = m_arg.content;
            string[] btntxtarr;
            if (string.IsNullOrEmpty(m_arg.btnname)) {
                btntxtarr = new string[0];
            } else {
                btntxtarr = m_arg.btnname.Split('|');
            }
            
            ctlTitle.text = m_arg.title;
            //根据是否有标题内容，判断是否需要隐藏标题
            if (!string.IsNullOrEmpty(m_arg.title)) {
                ctlTitle.gameObject.SetActive(true);
            } else {
                ctlTitle.gameObject.SetActive(false);
            }
            if (!string.IsNullOrEmpty(m_arg.btnname)) {
                //根据按钮的数量，设置按钮的分布方式，例如1个按钮，居中。
                float btnWidth = 350.0f;
                float btnStarX = (1 - btntxtarr.Length) * btnWidth / 2f;
                for (int i = 0; i < buttons.Length; i++)
                {
                    if (i < btntxtarr.Length)
                    {
                        buttons[i].GetComponentInChildren<Text>().text = btntxtarr[i];
                        SetActiveChild(buttons[i], true);
                        Vector3 pos = buttons[i].transform.localPosition;
                        pos.x = btnStarX + i * btnWidth;
                        buttons[i].transform.localPosition = pos;
                        ushort js = (ushort)i;
                        buttons[i].onClick.AddListener(delegate () { this.OnBtnClick(js); });
                    }
                    else
                    {
                        SetActiveChild(buttons[i], false);
                    }
                }
            } else {
                for (int i = 0; i < buttons.Length; i++)
                {
                     SetActiveChild(buttons[i], false);
                }
            }
        }
        public void SetActiveChild(Button btn, bool b) {
            btn.gameObject.SetActive(b);
        }
        public void OnBtnClick(int btnindex) {
            //AppTools.PlayBtnClick();
            this.Close(arg:btnindex);//调用close方法，传递点击的哪个btn作为参数。这样在UIWindowde CloseEvent事件中，调用方就知道了点击了哪个按钮。从而做后续处理
        }
    }
    /// <summary>
    /// MsgBox的参数结构体
    /// </summary>
    public class UIMsgBoxArg {
        public string title = "提示";
        public string content="";
        public string btnname;//这里需要传入多个按钮的文本，通过|分割，例如：确认|取消|关闭,空白就默认没有按钮
    }
}


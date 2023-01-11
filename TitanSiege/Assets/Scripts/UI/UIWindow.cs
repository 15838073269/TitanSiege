using UnityEngine;
using UnityEngine.UI;
namespace GF.Unity.UI {
    /// <summary>
    /// 窗口UI的基类
    /// </summary>
    public class UIWindow : UIPanel {
        public override UITypeDef UIType { get { return UITypeDef.Window; } }
        /// <summary>
        /// 关闭按钮，一般窗口都有，可以为空
        /// </summary>
        [SerializeField]
        private Button m_btnClose;
        /// <summary>
        /// 打开UI窗口的参数，可以为空
        /// </summary>
        protected object m_openArg;
        /// <summary>
        /// UI窗口激活时调用
        /// </summary>
        protected void OnEnable() {
            //this.Log("OnEnable()");
            if (m_btnClose != null) {
                m_btnClose.onClick.AddListener(OnBtnClose);
            }
        }
        /// <summary>
        /// 返回按钮的事件
        /// </summary>
        private void OnBtnClose() {
            //this.Log("OnBtnClose()");
            Close();
        }

        /// <summary>
        /// UI不可用时调用
        /// </summary>
        protected void OnDisable() {
            //this.Log("OnDisable()");
            if (m_btnClose != null) {
                m_btnClose.onClick.RemoveAllListeners();
            }
        }
        
    }
}

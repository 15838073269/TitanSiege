using UnityEngine;
using UnityEngine.UI;

namespace GF.Unity.UI {
    /// <summary>
    /// 页面UI的基类
    /// </summary>
    public class UIPage : UIPanel {
        public override UITypeDef UIType { get { return UITypeDef.Page; } }
        /// <summary>
        /// 返回按钮，按照常规页面设计，大部分页面应该都有一个返回按钮，可以为空
        /// </summary>
        [SerializeField]//强制将成员变量在Inspector中显示
        private Button m_btnGoBack;
        /// <summary>
        /// 打开UIpage的参数，可以为空
        /// </summary>
        protected object m_openArg;
        /// <summary>
        /// UI页面激活时调用
        /// </summary>
        protected virtual void OnEnable() {
            //this.Log("OnEnable()");
            if (m_btnGoBack!=null) {
                m_btnGoBack.onClick.AddListener(OnBtnGoBack);
            }
        }
        /// <summary>
        /// 返回按钮的事件
        /// </summary>
        private void OnBtnGoBack() {
            //this.Log("OnBtnGoBack()");
            UIManager.GetInstance.GoBackPage();
        }

        /// <summary>
        /// UI不可用时调用
        /// </summary>
        protected void OnDisable() {
            //this.Log("OnDisable()");
            if (m_btnGoBack!=null) {
                m_btnGoBack.onClick.RemoveAllListeners();
            }
        }
        
    }
}

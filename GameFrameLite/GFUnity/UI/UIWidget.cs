using UnityEngine;

namespace GF.Unity.UI {
    /// <summary>
    /// UI挂件的基类
    /// </summary>
    public class UIWidget:UIPanel {
        /// <summary>
        /// 打开UI挂件的参数，可以为空
        /// </summary>
        protected object m_openArg;
        public override UITypeDef UIType { get { return UITypeDef.Widget; } }
    }
}

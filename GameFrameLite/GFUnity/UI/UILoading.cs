
using GF.Service;
using UnityEngine.UI;

namespace GF.Unity.UI {
    /// <summary>
    /// loading的参数类
    /// </summary>
    public class UILoadingArg {
        public string title = "";
        public string tips = "";
        public int progress = 0;//0~100
        public string bgname = "";
        public override string ToString() {
            return string.Format("title:{0}, tips:{1}, progress:{2}", title, tips, progress);
        }
    }
    /// <summary>
    /// UILoading基础类，如果有其他需求就继承实现
    /// </summary>
    public class UILoading : UIPanel {
        public override UITypeDef UIType { get { return UITypeDef.Loading; } }

        public Text txtTitle;
        public Text txtTips;
        public Slider slider;
        public Image bg;//更换背景，待开发
        protected UILoadingArg m_arg;
        public UILoadingArg arg { get { return m_arg; } }

        protected override void OnOpen(object arg = null) {
            base.OnOpen(arg);
            m_arg = arg as UILoadingArg;
            if (m_arg == null) {
                m_arg = new UILoadingArg();
            }
            GameSceneService.GetInstance.SceneLoadedBegain += SceneLoadedBegain;
            GameSceneService.GetInstance.SceneLoadedOver += SceneLoadedOver;
            UpdateText();
        }
        /// <summary>
        /// 加载场景打开事件
        /// </summary>
        public virtual void SceneLoadedBegain(object arg = null) { 
        
        }
        /// <summary>
        /// 加载场景关闭事件
        /// </summary>
        public virtual void SceneLoadedOver(object arg = null) {
            Close(false,arg);
            slider.value = 0;
            m_arg = null;
            GameSceneService.GetInstance.SceneLoadedBegain -= SceneLoadedBegain;
            GameSceneService.GetInstance.SceneLoadedOver -= SceneLoadedOver;
        }
        /// <summary>
        /// 处理进度条内容
        /// </summary>
        /// <param name="title"></param>
        /// <param name="progress"></param>
        public void ShowProgress(string title, int progress) {
            m_arg.title = title;
            m_arg.progress = progress;
        }
        public void ShowProgress(int progress) {
            m_arg.progress = progress;
        }
        public void ShowProgress(string tips) {
            m_arg.tips = tips;
        }
        /// <summary>
        /// 重写uipanel类的update
        /// </summary>
        protected override void OnUpdate() {
            base.OnUpdate();
            if (m_arg != null) {
                UpdateText();
                UpdateProgress();
            }
        }
        /// <summary>
        /// 更新进度条
        /// </summary>
        protected virtual void UpdateProgress() {
            ShowProgress(GameSceneService.LoadingProgress);
            slider.value =(float) m_arg.progress / 100f;
        }
        /// <summary>
        /// 更新文本
        /// </summary>
        private void UpdateText() {
            if (txtTitle != null) {
                txtTitle.text = m_arg.title + "(" + m_arg.progress + "%)";
            }
            if (txtTips != null&& txtTips.text!= m_arg.tips) {
                txtTips.text = m_arg.tips;
            }
        }
    }
}

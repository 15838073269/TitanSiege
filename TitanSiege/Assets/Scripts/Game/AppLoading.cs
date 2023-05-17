using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
namespace GF.MainGame {
    public class AppLoading:MonoBehaviour {
        /// <summary>
        /// 单例
        /// </summary>
        private static AppLoading _Instance;
        public static AppLoading GetInstance {
            get {
                return _Instance;
            }
        }
        private void Awake() {
            _Instance = this;
            //EventTrigger e;
        }
        /// <summary>
        /// 定义Ui控件
        /// </summary>
        public Text txtTitle;
        public Text txtProgress;
        //public CtlProgressbar progressBar;//进度条控件 
        /// <summary>
        /// 显示加载资源进度
        /// </summary>
        /// <param name="title">标题</param>
        /// <param name="progress">进度</param>
        public static void Show(string title, float progress) {//静态函数通过单例调用非静态，先判断单例是否存在
            //_Instance = UIRoot.FindUI<AppLoading>("AppLoading"); //这里之所以加载以下，是因为单例可能被销毁了
            if (_Instance != null) {
                if (!_Instance.gameObject.activeSelf) {
                    _Instance.gameObject.SetActive(true);
                }
                _Instance.ShowProgress(title, progress);
            }
        }
        //隐藏资源加载进度
        public static void Hide() {
            if (_Instance != null) {
                if (_Instance.gameObject.activeSelf) {
                    _Instance.gameObject.SetActive(false);
                }
            }
        }

        private void ShowProgress(string title, float progress) {
            if (txtTitle != null) {
                txtTitle.text = title + "(" + (int)progress * 100 + "%)";
            }
            //if (progressBar!=null) {
            //    progressBar.SetData(progress);
            //}
        }

        private void OnDestroy() {
            _Instance = null;
        }
    }
}

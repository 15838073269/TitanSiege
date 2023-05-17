/****************************************************
    文件：EnterWidget.cs
	作者：昔莘
    邮箱: 304183153@qq.com
    日期：2021/12/6 11:17:30
	功能：进入按钮的挂件
*****************************************************/
using GF.Unity.UI;
using UnityEngine;
using UnityEngine.UI;
using GF.Msg;

namespace GF.MainGame.UI {
    public class UIEnterArg {
       public string scenename;
       public Vector3 pos;
    }
    public class EnterWidget : UIWidget {
        private Button m_EnterBtn;
        private string m_EnterScene;
        private Camera m_UICamera;
        private RectTransform m_Rect;
        private RectTransform m_RootRect;
        public override void Close(bool bClear = false, object arg = null) {
            base.Close(bClear, arg);
            m_EnterScene = null;
        }
        protected override void OnClose(bool bClear = false, object arg = null) {
            base.OnClose(bClear, arg);
        }

        protected override void OnOpen(object arg = null) {
            base.OnOpen(arg);
            UIEnterArg enterarg = arg as UIEnterArg;
            m_Rect = transform as RectTransform;
            m_EnterScene = enterarg.scenename;
            m_UICamera = UIManager.GetInstance.m_UICamera;
            m_RootRect = AppMain.GetInstance.uiroot.transform as RectTransform;
            Vector2 retpos;
            //两个摄像机的坐标系转换神坑
            //先将物体获取渲染摄像机中的屏幕坐标，然后再转换到ui摄像机中的坐标
            RectTransformUtility.ScreenPointToLocalPointInRectangle(m_RootRect, Camera.main.WorldToScreenPoint(enterarg.pos), m_UICamera, out retpos);
            m_Rect.anchoredPosition = retpos;
        }

        private void Start() {
            m_EnterBtn = transform.GetComponent<Button>();
            Debuger.Log(m_EnterBtn);
            m_EnterBtn.onClick.AddListener(EnterScene);
        }
        /// <summary>
        /// 进入单地图场景
        /// </summary>
        public void EnterScene() {
            if (!string.IsNullOrEmpty(m_EnterScene)) {
                AppTools.Send<string>((int)SceneEvent.OpenScene, m_EnterScene);
            } else{
                Debuger.Log("正在建设中");
            }
            Close();
        }
    }
}

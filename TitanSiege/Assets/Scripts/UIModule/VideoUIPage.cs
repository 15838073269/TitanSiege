/****************************************************
    文件：VideoUIPage.cs
	作者：昔莘
    邮箱: 304183153@qq.com
    日期：2021/12/2 16:5:19
	功能：Nothing
*****************************************************/

using GF.Service;
using GF.Unity.AB;
using GF.Unity.UI;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace GF.MainGame.UI {
    public class UIVideoArg {
        public string m_LoadSceneName = "";
        public string m_VideoClipName = "";
    }
    public class VideoUIPage : UIPage {
        public Button m_tiaoguo;
        public VideoPlayer m_video;
        private VideoClip m_CurrentClip;
        private UIVideoArg m_Arg;
        public override void Close(bool bClear = false, object arg = null) {
            m_video.Pause();
            m_tiaoguo.gameObject.SetActive(false);
            m_video.clip = null;
            //后面再考虑一下，目前情况可以清理
            if (m_CurrentClip != null) {
                ResourceManager.GetInstance.DestoryResource(m_CurrentClip);
                m_CurrentClip = null;
            }
            base.Close(bClear, arg);
        }
        protected override void OnOpen(object arg = null) {
            base.OnOpen(arg);
            if (m_tiaoguo == null) {
                m_tiaoguo = UIManager.GetInstance.m_UIRoot.FindUI<Button>("tiaoguo");
            }
            if (m_video == null) {
                m_video = UIManager.GetInstance.m_UIRoot.FindUI("VideoMovie").GetComponent<VideoPlayer>();
            }
            if (arg!=null) {
                m_Arg = arg as UIVideoArg;
                LoadVideoClip(m_Arg.m_VideoClipName);
            }
            m_tiaoguo.onClick.AddListener(Tiaoguo);
        }
        
        public void Tiaoguo() {
            Close();
            UILoadingArg arg = new UILoadingArg();
            arg.tips = "仙路世界没有主线剧情，所有的支线任务共同推进天下大势的发展。";
            arg.title = "正在加载仙路世界，请稍等！";
            GameSceneService.GetInstance.AsyncLoadScene(AppConfig.MainScene,uiarg:arg);
        }
        public void LoadVideoClip(string videoname) {
            if (string.IsNullOrEmpty(videoname)) {
                Debuger.Log($"视频资源{videoname}不存在，请检查！将直接跳过动画");
                Tiaoguo();
            }
            string path = "movies/" + videoname;
            m_CurrentClip = ResourceManager.GetInstance.LoadResource<VideoClip>(path);
            if (m_CurrentClip == null) {
                Debuger.Log($"视频资源{videoname}不存在，请检查！将直接跳过动画");
                Tiaoguo();
            } else {
                m_video.clip = m_CurrentClip;
                m_video.Play();
                m_tiaoguo.gameObject.SetActive(true);
                StartCoroutine("VideoOver");
            }
        }
        protected IEnumerator VideoOver() {
            yield return new WaitForSeconds((float)m_CurrentClip.length);
            while (m_video.isPlaying) {
                yield return null;
            }
            Close();
            if (m_Arg != null && !string.IsNullOrEmpty(m_Arg.m_LoadSceneName)) {
                UILoadingArg arg = new UILoadingArg();
                arg.tips = "仙路世界没有主线剧情，所有的支线任务共同推进天下大势的发展。";
                arg.title = "正在加载仙路世界，请稍等！";
                GameSceneService.GetInstance.AsyncLoadScene(AppConfig.MainScene, uiarg: arg);
            }
        }
        public double GetVideoTime() {
            return m_CurrentClip.frameRate;
        }
    }
}

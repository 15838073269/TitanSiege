/****************************************************
    文件：MapBtn.cs
	作者：昔莘
    邮箱: 304183153@qq.com
    日期：2022/12/23 16:30:39
	功能：Nothing
*****************************************************/

using GF.Unity.UI;
using UnityEngine;
using UnityEngine.UI;

namespace GF.MainGame.UI {
    public class MapBtn : MonoBehaviour {
        public string m_ToSceneName;
        
        private Transform m_Notice;
        private Button m_Btn;
        public void Awake() {
            m_Btn = GetComponent<Button>();
            m_Notice = transform.Find("tishi");
            m_Btn.onClick.AddListener(() => {
                MapUIPage map = UIManager.GetInstance.GetUI<MapUIPage>(AppConfig.MapUIPage);
                map.Close();
                UIManager.GetInstance.EnterMainPage();
                AppTools.Send<string>((int)SceneEvent.OpenScene, m_ToSceneName);
            });
        }
        public void SetBtnInterface(bool enable) {
            if (enable == false) {
                m_Btn.interactable = false;
                m_Notice.gameObject.SetActive(true);
                m_Btn.Select();
            } else {
                m_Btn.interactable = true;
                m_Notice.gameObject.SetActive(false);
            }
        }
    }
}

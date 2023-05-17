/****************************************************
    文件：ChuanSong.cs
	作者：昔莘
    邮箱: 304183153@qq.com
    日期：2021/12/6 8:46:40
	功能：传送物体的类
*****************************************************/
using GF.MainGame.Module.NPC;
using GF.Module;
using GF.Unity.UI;
using UnityEngine;
namespace GF.MainGame.Module {
    public class ChuanSong:MonoBehaviour {
        /// <summary>
        /// 传送的目标场景名称，如果m_ShowMap == true，这里就是当前场景名称
        /// </summary>
        public string m_ToScenename = "tiankongcheng";
        public string m_Charname;
        public bool m_ShowMap;
        public SceneModule m_Sm;
        public void Start() {
            if (m_Sm == null) {
                m_Sm = AppTools.GetModule<SceneModule>(MDef.SceneModule);
            }
        }
        public void OnTriggerEnter(Collider other) {
            if (!other.GetComponent<Player>().m_IsNetPlayer) {
                m_Sm.OpenChuanSongUI(m_ToScenename, m_ShowMap, m_Charname);
            }
        }

        public void OnTriggerExit(Collider other) {
            UIManager.GetInstance.CloseWindow("UIMsgBox");
        }
    }
}

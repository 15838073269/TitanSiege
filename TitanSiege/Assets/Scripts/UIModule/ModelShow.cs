/****************************************************
    文件：ModelShow.cs
	作者：昔莘
    邮箱: 304183153@qq.com
    日期：2023/9/11 21:36:40
	功能：Nothing
*****************************************************/

using GF.Service;
using UnityEngine;
namespace GF.MainGame.Module {
    public class ModelShow : MonoBehaviour {
        public GameObject m_Zhanshi;
        public GameObject m_Fashi;
        public GameObject m_Youxia;
        public void OnEnable() {
            switch (UserService.GetInstance.m_CurrentChar.Zhiye) {
                case 0:
                    m_Zhanshi.SetActive(true);
                    m_Fashi.SetActive(false);
                    m_Youxia.SetActive(false);
                    break; 
                case 1:
                    m_Zhanshi.SetActive(false);
                    m_Fashi.SetActive(true);
                    m_Youxia.SetActive(false);
                    break;
                case 2:
                    m_Zhanshi.SetActive(false);
                    m_Fashi.SetActive(false);
                    m_Youxia.SetActive(true);
                    break;
                default:
                    Debuger.LogError("未知职业！");
                    break;
            }
        }
    }
}

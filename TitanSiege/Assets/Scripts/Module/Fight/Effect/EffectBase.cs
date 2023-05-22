/****************************************************
    文件：EffectBase.cs
	作者：昔莘
    邮箱: 304183153@qq.com
    日期：2022/11/23 16:42:25
	功能：Nothing
*****************************************************/

using UnityEngine;
namespace GF.MainGame.Module.Fight {
    public class EffectBase : MonoBehaviour {
        public ParticleSystem m_Particle;
        public bool m_IsFollow;

        public void Init() {
            if (m_Particle == null) {
                m_Particle = transform.GetComponent<ParticleSystem>();
            }
        }
      
    }
}
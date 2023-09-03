/****************************************************
    文件：EffectOfflineData.cs
	作者：昔莘
    邮箱: 304183153@qq.com
    日期：2021/9/15 11:29:51
	功能：特效的离线数据
离线数据的扩展用法，可以将常用组件放在离线数据中，例如刚体，碰撞器，这样在游戏运行时，需要使用碰撞器就可以通过离线数据直接获取，而不用再次GetComponent,每次GetComponent都会有性能消耗
*****************************************************/

using UnityEngine;
namespace GF.Unity.AB {
    public class EffectOfflineData : OfflineDatabase {
        public ParticleSystem[] m_ParticleSystems;
        public TrailRenderer[] m_TrailRenders;//拖尾
        /// <summary>
        /// 绑定数据
        /// </summary>
        public override void BindData() {
            base.BindData();
            m_ParticleSystems = gameObject.GetComponentsInChildren<ParticleSystem>(true);
            m_TrailRenders = gameObject.GetComponentsInChildren<TrailRenderer>(true);
        }

        public override void ResetProp() {
            base.ResetProp();
            for (int i = 0; i < m_ParticleSystems.Length; i++) {
                m_ParticleSystems[i].Clear();
                m_ParticleSystems[i].Play();
            }
            for (int i = 0; i < m_TrailRenders.Length; i++) {
                m_TrailRenders[i].Clear();
            }
        }
    }
}

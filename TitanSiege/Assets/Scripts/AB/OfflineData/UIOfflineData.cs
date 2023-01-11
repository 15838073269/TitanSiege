/****************************************************
    文件：UIOfflineData.cs
	作者：昔莘
    邮箱: 304183153@qq.com
    日期：2021/9/15 11:30:18
	功能：UI物体的离线数据
          离线数据的扩展用法，可以将常用组件放在离线数据中，例如刚体，碰撞器，这样在游戏运行时，需要使用碰撞器就可以通过离线数据直接获取，而不用再次GetComponent,每次GetComponent都会有性能消耗
*****************************************************/
//离线数据会占用一些内存，但数据本身就不大，影响不大
using UnityEngine;
namespace GF.Unity.AB {
    public class UIOfflineData : OfflineDatabase {
        public Vector2[] m_AnchorsMax;
        public Vector2[] m_AnchorsMin;
        public Vector2[] m_Pivot;
        public Vector2[] m_SizeDelta;
        public Vector2[] m_AnchordPos;
        public ParticleSystem[] m_Particle;

        public override void ResetProp() {
            //RectTransform还原
            for (int i = 0; i < m_AllPoint.Length; i++) {
                RectTransform rect = m_AllPoint[i] as RectTransform;
                if (rect != null) {
                    rect.localPosition = m_Pos[i];
                    rect.localRotation = m_Rot[i];
                    rect.localScale = m_Scale[i];
                    rect.anchorMax = m_AnchorsMax[i];
                    rect.anchorMin = m_AnchorsMin[i];
                    rect.pivot = m_Pivot[i];
                    rect.sizeDelta = m_SizeDelta[i];
                    rect.anchoredPosition3D = m_AnchordPos[i];
                    if (rect.gameObject.activeSelf != m_AllPointActive[i]) {
                        rect.gameObject.SetActive(m_AllPointActive[i]);
                    }
                    //运行中不会删除他的子物体，所以只会多，不会少
                    if (rect.childCount > m_AllPointChildCount[i]) {
                        for (int j = 0; j < rect.childCount; j++) {
                            GameObject tmp = rect.GetChild(j).gameObject;
                            //如果不是对象管理器创建的就销毁，对象管理器创建的，对象管理器会自行销毁
                            if (!ObjectManager.GetInstance.IsObjectManagerCreate(tmp)) {
                                GameObject.Destroy(tmp);
                            }
                        }
                    }
                }
            }
            //particle还原，粒子特效还原
            for (int i = 0; i < m_Particle.Length; i++) {
                m_Particle[i].Clear(true);
                m_Particle[i].Play();
            }
        }

        public override void BindData() {
            //很多prefaab外层都是一个父类空物体，插组件都在里层，所以需要从GetComponentInChildren中寻找一个
            //GetComponentInChildren(true),true是代表不显示的也寻找
            m_AllPoint = gameObject.GetComponentsInChildren<RectTransform>(true);
            m_AllPointChildCount = new int[m_AllPoint.Length];
            m_AllPointActive = new bool[m_AllPoint.Length];
            m_Scale = new Vector3[m_AllPoint.Length];
            m_Rot = new Quaternion[m_AllPoint.Length];
            m_Pos = new Vector3[m_AllPoint.Length];
            m_AnchorsMax = new Vector2[m_AllPoint.Length];
            m_AnchorsMin = new Vector2[m_AllPoint.Length];
            m_Pivot = new Vector2[m_AllPoint.Length];
            m_SizeDelta = new Vector2[m_AllPoint.Length];
            m_AnchordPos = new Vector2[m_AllPoint.Length];
            for (int i = 0; i < m_AllPoint.Length; i++) {
                if (m_AllPoint[i] is RectTransform) {
                    RectTransform rect = m_AllPoint[i] as RectTransform;
                    m_AllPointChildCount[i] = rect.childCount;
                    m_AllPointActive[i] = rect.gameObject.activeSelf;
                    m_Pos[i] = rect.localPosition;
                    m_Rot[i] = rect.localRotation;
                    m_Scale[i] = rect.localScale;
                    m_AnchorsMax[i] = rect.anchorMax;
                    m_AnchorsMin[i] = rect.anchorMin;
                    m_Pivot[i] = rect.pivot;
                    m_SizeDelta[i] = rect.sizeDelta;
                    m_AnchordPos[i] = rect.anchoredPosition3D;
                }
            }
            m_Particle = gameObject.GetComponentsInChildren<ParticleSystem>(true);
        }
    }
}

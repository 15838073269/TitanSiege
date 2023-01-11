/****************************************************
    文件：OfflineDatabase.cs
	作者：昔莘
    邮箱: 304183153@qq.com
    日期：2021/9/15 9:35:51
	功能：1、离线数据的处理基类，适用于预制体。离线数据的作用主要是用来恢复已使用的游戏对象的默认状态和数值，因为使用了对象池，所以放回对象池时需要把对象还原默认状态（大小位置），每次手动处理太复杂，就通过记录的离线数据获取
    2、离线数据的扩展用法，可以将常用组件放在离线数据中，例如刚体，碰撞器，这样在游戏运行时，需要使用碰撞器就可以通过离线数据直接获取，而不用再次GetComponent,每次GetComponent都会有性能消耗
*****************************************************/
using UnityEngine;
namespace GF.Unity.AB {
    public class OfflineDatabase : MonoBehaviour {
        /// <summary>
        /// 刚体，一般一个预制体只有一个刚体
        /// </summary>
        public Rigidbody m_Rigidbody;
        /// <summary>
        /// 碰撞器，一般一个预制体只有一个碰撞器
        /// </summary>
        public Collider m_Collider;
        /// <summary>
        /// 预制体的所有节点
        /// </summary>
        public Transform[] m_AllPoint;
        /// <summary>
        /// 每个节点有多少个子节点，节点与节点的对应关系
        /// </summary>
        public int[] m_AllPointChildCount;
        /// <summary>
        /// 每个节点是否显示
        /// </summary>
        public bool[] m_AllPointActive;
        /// <summary>
        /// 每个节点的位置
        /// </summary>
        public Vector3[] m_Pos;
        /// <summary>
        /// 每个节点的缩放
        /// </summary>
        public Vector3[] m_Scale;
        /// <summary>
        /// 每个节点的旋转
        /// </summary>
        public Quaternion[] m_Rot;
        /// <summary>
        /// 还原方法
        /// </summary>
        public virtual void ResetProp() {
            for (int i = 0; i < m_AllPoint.Length; i++) {
                if (m_AllPoint[i] != null) {
                    m_AllPoint[i].localPosition = m_Pos[i];
                    m_AllPoint[i].localRotation = m_Rot[i];
                    m_AllPoint[i].localScale = m_Scale[i];
                    if (m_AllPoint[i].gameObject.activeSelf != m_AllPointActive[i]) {
                        m_AllPoint[i].gameObject.SetActive(m_AllPointActive[i]);
                    }
                    //运行中不会删除他的子物体，所以只会多，不会少
                    if (m_AllPoint[i].childCount > m_AllPointChildCount[i]) {
                        for (int j = 0; j < m_AllPoint[i].childCount; j++) {
                            GameObject tmp = m_AllPoint[i].GetChild(j).gameObject;
                            //如果不是对象管理器创建的就销毁，对象管理器创建的，对象管理器会自行销毁
                            if (!ObjectManager.GetInstance.IsObjectManagerCreate(tmp)) {
                                GameObject.Destroy(tmp);
                            }
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 给上述字段赋值，不在运行时调用，编辑器下使用
        /// </summary>
        public virtual void BindData() {
            //很多prefaab外层都是一个父类空物体，插组件都在里层，所以需要从GetComponentInChildren中寻找一个
            //GetComponentInChildren(true),true是代表不显示的也寻找
            m_Collider = gameObject.GetComponentInChildren<Collider>(true);
            m_Rigidbody = gameObject.GetComponentInChildren<Rigidbody>(true);
            m_AllPoint = gameObject.GetComponentsInChildren<Transform>(true);
            m_AllPointChildCount = new int[m_AllPoint.Length];
            m_AllPointActive = new bool[m_AllPoint.Length];
            m_Scale = new Vector3[m_AllPoint.Length];
            m_Rot = new Quaternion[m_AllPoint.Length];
            m_Pos = new Vector3[m_AllPoint.Length];
            for (int i = 0; i < m_AllPoint.Length; i++) {
                m_AllPointChildCount[i] = m_AllPoint[i].childCount;
                m_AllPointActive[i] = m_AllPoint[i].gameObject.activeSelf;
                m_Pos[i] = m_AllPoint[i].localPosition;
                m_Rot[i] = m_AllPoint[i].localRotation;
                m_Scale[i] = m_AllPoint[i].localScale;
            }
        }
    }
}
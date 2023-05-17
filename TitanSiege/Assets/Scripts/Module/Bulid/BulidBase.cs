/****************************************************
    文件：BulidBase.cs
	作者：昔莘
    邮箱: 304183153@qq.com
    日期：2021/12/6 8:46:40
	功能：大地图上所有建筑物的基类
*****************************************************/
using GF.MainGame.Module;
using GF.Module;
using UnityEngine;
namespace GF.MainGame.Bulid {
    public class BulidBase :MonoBehaviour{
        private ushort m_ID = 0;
        public string m_Charname;
        public string m_Scenename;
        public string m_PrefabName;
        private BulidModule m_Bm;
        /// <summary>
        ///这里的是玩家从场景传送到大地图时的位置信息
        /// </summary>
        public Transform m_Default;
        public virtual void Start() {
            if (m_Bm == null) {
                m_Bm = AppTools.GetModule<BulidModule>(MDef.BulidModule);
            }
        }
        public virtual void OnTriggerEnter(Collider other) {
            // AppTools.Send(ModuleDef.BulidModule, "BulidTriggerEnter", this,other.gameObject);
            //消息机制使用了反射，反射的效率并不高，所以在跨模块使用消息，本模块的，就直接调用
            m_Bm.BulidTriggerEnter(this, other.gameObject);
        }
        public virtual void OnTriggerExit(Collider other) {
            // AppTools.Send(ModuleDef.BulidModule, "BulidTriggerExit");
            //消息机制使用了反射，反射的效率并不高，所以在跨模块使用消息，本模块的，就直接调用
            m_Bm.BulidTriggerExit();
        }
      
    }
}

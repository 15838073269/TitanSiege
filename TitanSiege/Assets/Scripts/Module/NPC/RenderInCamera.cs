/****************************************************
    文件：RenderInCamera.cs
	作者：昔莘
    邮箱: 304183153@qq.com
    日期：2023/3/16 0:30:50
	功能：Nothing
*****************************************************/

using UnityEngine;
namespace GF.MainGame.Module.NPC {
    public class RenderInCamera : MonoBehaviour {
        public NPCBase NPC;
        public void Start() {
            NPC = transform.parent.GetComponent<NPCBase>();
        }
        
        void OnBecameVisible() {
            //Debuger.Log(NPC.m_GDID);
            AppTools.Send<NPCBase>((int)HPEvent.ShowHP, NPC);
        }
        /// 物体不在屏幕内    需要注意的是如果在编辑器模式下运行 即使Game看不到物体，Scene仍然看到物体的话  不会执行这个方法，只有当Game和Scnen两个视图都看不到物体 才能执行这个方法
        void OnBecameInvisible() {
            //Debuger.Log(NPC.m_GDID);
            AppTools.Send<NPCBase>((int)HPEvent.HideHP, NPC);
        }

    }
}
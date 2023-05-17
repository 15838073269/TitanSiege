/****************************************************
    文件：NTransform.cs
	作者：昔莘
    邮箱: 304183153@qq.com
    日期：2022/11/5 15:57:31
	功能：Nothing
*****************************************************/
#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_WSA

using GF.MainGame.Module.NPC;
using Net.UnityComponent;
using UnityEngine;
namespace GF.NetWork {
    [DefaultExecutionOrder(1000)]
    public class NTransform : NetworkTransformBase {
       // private NPCBase npc;
        public override void Start() {
            base.Start();
            //npc = transform.GetComponent<NPCBase>();
            //if (npc == null) {
            //    npc = transform.GetComponent<Player>();
            //}
        }
    }
}

#endif

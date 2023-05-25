/****************************************************
    文件：MonsterAnimatorBase.cs
	作者：昔莘
    邮箱: 304183153@qq.com
    日期：2023/1/6 15:39:14
	功能：Nothing
*****************************************************/

using GF.MainGame.Data;
using Net.Client;
using Net.Share;
using Net.System;
namespace GF.MainGame.Module.NPC {
    public class MonsterAnimatorBase : NPCAnimatorBase {

        public override void Init() {
            base.Init();
        }

        public override void Die() {
           // m_ani.SetInteger("die", -1);
            //设定一个计时，死亡2秒后，溶解尸体
            ThreadManager.Event.AddEvent(1.618f, DieToDestroy);
        }
        /// <summary>
        /// 死亡销毁
        /// </summary>
        private async void DieToDestroy() {
            Monster m = transform.GetComponent<Monster>();
            if (m != null) { 
               await m.HideModel();
               m.gameObject.SetActive(false);
            }
        }
    }
}

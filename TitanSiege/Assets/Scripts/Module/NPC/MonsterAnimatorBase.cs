/****************************************************
    文件：MonsterAnimatorBase.cs
	作者：昔莘
    邮箱: 304183153@qq.com
    日期：2023/1/6 15:39:14
	功能：Nothing
*****************************************************/

using GF.MainGame.Data;
using UnityEngine;
namespace GF.MainGame.Module.NPC {
    public class MonsterAnimatorBase : NPCAnimatorBase {
        //public override void Attack(SkillDataBase sd = null) {

        //}

        //public override void Attack1(int skillid) {

        //}

        public override void Idle() {
            m_ani.SetInteger("run", 0);
        }

        public override void Init() {
            base.Init();
        }

        public override void Move() {
            m_ani.SetInteger("run", 1);
        }
    }
}

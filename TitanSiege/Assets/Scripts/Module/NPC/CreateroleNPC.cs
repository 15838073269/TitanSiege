
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace GF.MainGame.Module.NPC {
    public class CreateroleNPC : NPCBase {
        public override void Start() {
            Init(false);
            InitNPCAnimaor();
            AppTools.Regist<float>((int)NpcEvent.RotateTo, RotateTo);
            AppTools.Regist((int)NpcEvent.SetRotationZero, SetRotationZero);
        }
        public void PlayPose() {
            m_Nab.m_ani.SetTrigger("pose");
        }
        /// <summary>
        /// ×ÔÉíÐý×ª
        /// </summary>
        public void RotateTo( float x) {
            transform.Rotate(Vector3.up,x*Time.deltaTime);
        }

        public void SetRotationZero() { 
            transform.rotation = Quaternion.identity;
        }
        public void OnDestroy() {
            AppTools.Remove<float>((int)NpcEvent.RotateTo, RotateTo);
            AppTools.Remove((int)NpcEvent.SetRotationZero, SetRotationZero);
        }
    }
}


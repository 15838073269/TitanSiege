/****************************************************
    文件：Player.cs
	作者：昔莘
    邮箱: 304183153@qq.com
    日期：2021/11/29 15:36:19
	功能：Nothing
*****************************************************/

using GF.Const;
using GF.MainGame.Module;
using GF.MainGame.Module.Fight;
using GF.Module;
using GF.Service;
using Net.Component;
using Net.UnityComponent;
using System.Collections.Generic;
using UnityEngine;

namespace GF.MainGame.Module.NPC {
    public class Player : NPCBase {
        
        public string m_PlayerName;
        
        //public int m_Infoid;//数据库Characterdata表的id
        public override void Start() {
            base.Start();
            SetFight(true);
            
        }
       
        public override void InitNPCAnimaor() {
            m_Nab = transform.GetComponent<PlayerAnimator>();
            if (m_Nab == null) {
                m_Nab = transform.gameObject.AddComponent<PlayerAnimator>();
                m_Nab.Init();
            }
            ChangeWp();//切换以下武器
        }
        


    }
}

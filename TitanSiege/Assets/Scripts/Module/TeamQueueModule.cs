/****************************************************
    文件：NPCQueue.cs
	作者：昔莘
    邮箱: 304183153@qq.com
    日期：2021/12/1 8:57:39
	功能：玩家的队伍
*****************************************************/

using System.Collections.Generic;
using UnityEngine;
using GF.Module;
using GF.MainGame.Module.NPC;

namespace GF.MainGame.Module {
    public class TeamQueueModule : GeneralModule {
        public List<NPCBase> m_ListQueue= new List<NPCBase>();
        
        public override void Create() {
            base.Create();
            //for (ushort i = 0; i <= AppConfig.TeamNum; i++) {
            //    m_ListQueue.Add(i, null);
            //}
        }

        /// <summary>
        /// 是否包含这个npc
        /// </summary>
        /// <param name="npc"></param>
        /// <returns></returns>
        public bool IsContainsNPC(NPCBase npc) {
            for (int i = 0; i < m_ListQueue.Count; i++) {
                if (npc == m_ListQueue[i]) {
                    return true;
                }
            }
            
            return false;
        }

        public override void Release() {
            base.Release();
        }

        public override void Show() {
            base.Show();
        }
        /// <summary>
        /// 交换位置
        /// </summary>
        public void ChangeSet() { 
            
        }
    }
   
}

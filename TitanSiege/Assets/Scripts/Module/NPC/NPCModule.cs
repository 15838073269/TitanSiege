/****************************************************
    文件：NPCModlue.cs
	作者：昔莘
    邮箱: 304183153@qq.com
    日期：2021/11/27 11:20:36
	功能：Nothing
*****************************************************/

using UnityEngine;
using GF.Module;
using System.Collections.Generic;
using GF.MainGame.Module.NPC;
using Net.System;
using GF.Service;
using Net.Client;
using Net.Share;

namespace GF.MainGame.Module {
    public class NPCModule : GeneralModule {
        public Dictionary<int, Player> AllPlayers;
        public NPCBase m_CurrentSelected;
        //public Dictionary<string, ListSafe<Monster>> AllMonsters;
        public override void Create() {
            AllPlayers = new Dictionary<int, Player>();
            //AllMonsters = new Dictionary<string, ListSafe<Monster>>();
            //AppTools.Regist<string, Monster>((int)NpcEvent.AddMonster, AddMonster);
            //AppTools.Regist<string>((int)NpcEvent.RemoveMonsterByScene, RemoveMonsterInScene);
            //AppTools.Regist<string, Monster>((int)NpcEvent.RemoveMonster, RemoveMonster);
            AppTools.Regist<int>((int)NpcEvent.Removeplayer, Removeplayer);
            AppTools.Regist<Player>((int)NpcEvent.AddPlayer, AddPlayer);
            AppTools.Regist<NPCBase>((int)NpcEvent.ChangeSelected, ChangeSelected);
            AppTools.Regist<NPCBase>((int)NpcEvent.CanelSelected, CanelSelected);
            AppTools.Regist<int,int>((int)NpcEvent.FuhuoPlayer, FuhuoPlayer);
            //AppTools.Regist<string, ListSafe<Monster>>((int)NpcEvent.GetMonstersbyScene, GetMonstersbyScene);
        }
        //public void AddMonster(string scenename, Monster m) {
        //    if (!AllMonsters.ContainsKey(scenename)) {
        //        AllMonsters.Add(scenename, new ListSafe<Monster>());
        //    }
        //    if (!AllMonsters[scenename].Contains(m)) {
        //        AllMonsters[scenename].Add(m);
        //        Debuger.Log(AllMonsters[scenename].Count);
        //    }
        //}
        ///// <summary>
        ///// 清除一整个场景的怪物数据，一般在场景卸载后执行
        ///// </summary>
        ///// <param name="scenename"></param>
        //public void RemoveMonsterInScene(string scenename) {
        //    if (AllMonsters.ContainsKey(scenename)) {
        //        AllMonsters[scenename].Clear();
        //    }
        //}
        //public void RemoveMonster(string scenename, Monster m) {
        //    if (AllMonsters.ContainsKey(scenename)) {
        //        if (AllMonsters[scenename].Contains(m)) {
        //            AllMonsters[scenename].Remove(m);
        //        }
        //    }
        //}
        public void AddPlayer(Player p) {
            if (!AllPlayers.ContainsKey(p.m_GDID)) {
                AllPlayers.Add(p.m_GDID, p);
            }
        }
        public void Removeplayer(int id) {
            if (AllPlayers.ContainsKey(id)) {
                AllPlayers.Remove(id);
            }
        }
        /// <summary>
        /// 这里给多种复活方式预留了功能
        /// 目前只有i=0，也就是在传送点复活
        /// 后面可以加上原地复活等
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="i"></param>
        public void FuhuoPlayer(int uid,int i) {
            Player p;
            Transform startpos=null;
            if (!AllPlayers.TryGetValue(uid, out p)) {
                Debuger.LogError($"复活玩家错误，玩家{uid}不存在，请检查！");
                return;
            }
            switch (i) {
                case 0://复活点复活
                    //获取场景的出生点
                    //传空参数是默认获取当前活动的场景名称，也可以获取指定名称的场景起始点
                    startpos = AppTools.SendReturn<string,Transform>((int)SceneEvent.GetSceneDefaultPos,"");
                    break;
                case 1://原地复活，有精力再做

                    break;
                default:
                    break;
            }
            if (startpos!=null) {
                AppTools.Send<Transform>((int)MoveEvent.SetMoveObjToScene, startpos);
                ClientBase.Instance.AddOperation(new Operation(Command.Resurrection, p.m_GDID));//移动完成后，发送服务器通知玩家复活
            }
        }
        /// <summary>
        /// 控制选中特效的显示
        /// </summary>
        /// <param name="npc">npc为null就是取消所有选中</param>
        public void ChangeSelected(NPCBase npc) {
            if (m_CurrentSelected != null) {
                m_CurrentSelected.m_Selected.SetActive(false);
            }
            if (m_CurrentSelected!=npc) {
                m_CurrentSelected = npc;
                m_CurrentSelected.m_Selected.SetActive(true);
            }
        }
        /// <summary>
        /// 取消npc选中，一般失去目标才会这么做
        /// </summary>
        public void CanelSelected(NPCBase npc = null) {
            if (npc == null) {
                if (m_CurrentSelected!=null) {
                    m_CurrentSelected.m_Selected.SetActive(false);
                    m_CurrentSelected = null;
                }
                UserService.GetInstance.m_CurrentPlayer.AttackTarget = null;
            } else {//这种情况是选中物体消失或者死亡，取消选中
                if (m_CurrentSelected == npc) {
                    m_CurrentSelected.m_Selected.SetActive(false);
                    m_CurrentSelected = null;
                    UserService.GetInstance.m_CurrentPlayer.AttackTarget = null;
                }
            }
        }
        public override void Release() {
            base.Release();
        }
        ///// <summary>
        ///// 通过场景名获取场景内所有怪物
        ///// </summary>
        ///// <param name="scenename"></param>
        ///// <returns></returns>
        //public ListSafe<Monster> GetMonstersbyScene(string scenename) {
        //    ListSafe<Monster> temp = null;
        //    AllMonsters.TryGetValue(scenename, out temp);//无论有没有都返回，方便外部判断，例如主城没有任何怪物，但可以pk
        //    return temp;
        //}
    }
}

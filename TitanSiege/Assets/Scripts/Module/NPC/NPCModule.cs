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

namespace GF.MainGame.Module {
    public class NPCModule : GeneralModule {
        public Dictionary<int, Player> AllPlayers;
        public Dictionary<string, ListSafe<Monster>> AllMonsters;
        public override void Create() {
            AllPlayers = new Dictionary<int, Player>();
            AllMonsters = new Dictionary<string, ListSafe<Monster>>();
            AppTools.Regist<string, Monster>((int)NpcEvent.AddMonster, AddMonster);
            AppTools.Regist<string>((int)NpcEvent.RemoveMonsterByScene, RemoveMonsterInScene);
            AppTools.Regist<string, Monster>((int)NpcEvent.RemoveMonster, RemoveMonster);
            AppTools.Regist<int>((int)NpcEvent.Removeplayer, Removeplayer);
            AppTools.Regist<Player>((int)NpcEvent.AddPlayer, AddPlayer);
            AppTools.Regist<string, ListSafe<Monster>>((int)NpcEvent.GetMonstersbyScene, GetMonstersbyScene);
        }
        public void AddMonster(string scenename, Monster m) {
            if (!AllMonsters.ContainsKey(scenename)) {
                AllMonsters.Add(scenename, new ListSafe<Monster>());
            }
            if (!AllMonsters[scenename].Contains(m)) {
                AllMonsters[scenename].Add(m);
                Debuger.Log(AllMonsters[scenename].Count);
            }
        }
        /// <summary>
        /// 清除一整个场景的怪物数据，一般在场景卸载后执行
        /// </summary>
        /// <param name="scenename"></param>
        public void RemoveMonsterInScene(string scenename) {
            if (AllMonsters.ContainsKey(scenename)) {
                AllMonsters[scenename].Clear();
            }
        }
        public void RemoveMonster(string scenename, Monster m) {
            if (AllMonsters.ContainsKey(scenename)) {
                if (AllMonsters[scenename].Contains(m)) {
                    AllMonsters[scenename].Remove(m);
                }
            }
        }
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
        public override void Release() {
            base.Release();
        }
        /// <summary>
        /// 通过场景名获取场景内所有怪物
        /// </summary>
        /// <param name="scenename"></param>
        /// <returns></returns>
        public ListSafe<Monster> GetMonstersbyScene(string scenename) {
            ListSafe<Monster> temp = null;
            AllMonsters.TryGetValue(scenename, out temp);//无论有没有都返回，方便外部判断，例如主城没有任何怪物，但可以pk
            return temp;
        }
    }
}

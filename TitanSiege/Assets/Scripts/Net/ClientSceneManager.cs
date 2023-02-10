
using GF.Const;
using GF.MainGame;
using GF.MainGame.Module;
using GF.MainGame.Module.NPC;
using GF.Unity.AB;
using Net.Client;
using Net.Share;
using Net.System;
using Net.UnityComponent;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GF.NetWork {
    public class ClientSceneManager: NetworkSceneManager {
        private Transform MonsterFather;
        private ListSafe<string> monsters = new ListSafe<string>();
        public MyDictionary<int, Monster> monsterDics = new MyDictionary<int, Monster>();

        private void InitMonsters() {
            //读取配置表  todo
            //先临时添加几个怪物进来，回头开发场景配置表
            string s = "NPCPrefab/lang2.prefab";
            if (!monsters.Contains(s)) {
                monsters.Add(s);
            }
            if (MonsterFather == null) {
                MonsterFather = (new GameObject("Monsters")).transform;
            }
        }
        public override void OnNetworkObjectCreate(Operation opt, NetworkObject identity) {
            var p = identity.GetComponent<Player>();
            if (p != null) {
                p.m_GDID = opt.identity;
                p.m_IsNetPlayer = true;
                p.m_NpcType = NpcType.player;
                AppTools.Send<Player>((int)NpcEvent.AddPlayer, p);
            }
        }

        public override void OnOtherDestroy(NetworkObject identity) {
            var p = identity.GetComponent<Player>();
            if (p != null) {
                Destroy(p.gameObject);
                var players = AppTools.GetModule<NPCModule>(MDef.NPCModule).AllPlayers;
                players.Remove(p.m_GDID);
            }
        }
        
        public override void OnOtherOperator(Operation opt) {
            switch (opt.cmd) {
                case Command.Skill:
                    if (identitys.TryGetValue(opt.identity, out var t)) {
                        var p = t.GetComponent<Player>();
                        if (p.m_IsNetPlayer) {//不是本地的
                            Debuger.Log($"{opt.identity}释放技能{opt.index1}");
                            AppTools.Send<NPCBase, AniState, object>((int)StateEvent.ChangeStateWithArgs, p, AniState.attack, opt.index1);
                        }
                    }
                    break;
                case Command.AIMonster://未发生攻击时，服务端同步场景怪物行为
                    var monster = CheckMonster(opt);
                    if (monster.m_SynchSign == false ) {
                        monster.transform.position = opt.position;
                        monster.transform.rotation = opt.rotation;
                    }
                    break;
                case Command.EnemySync://前往或者攻击玩家时，客户端同步怪物位置的命令
                    var monster1 = CheckMonster(opt);
                    if (monster1.m_TargetID != ClientBase.Instance.UID) {//怪物不是本机控制的就直接同步位置
                        monster1.transform.position = opt.position;
                        monster1.transform.rotation = opt.rotation;
                        monster1.FightHP = opt.index1;
                    }
                    break;
                case Command.EnemySwitchState:
                    //客户端切换怪物状态的命令，客户端警戒方法发送的命令，收到命令后，处理服务端怪物状态，并告知其他客户端
                    //警戒方法，失去目标后也发送此命令
                    //var monster2 = CheckMonster(opt);
                    //monster2.state = opt.cmd1;
                    //monster2.state1 = opt.cmd2;
                    //monster2.StatusEntry();//切换状态后，客户端自行播放状态动画
                    //var monster2 = CheckMonster(opt);
                    //if (opt.identity != ClientBase.Instance.UID) { //如果不是目前操纵怪物的本机就切换状态，例如从移动切换为攻击
                       
                    //}
                    break;
                case Command.PlayerState: 
                    break;
                case Command.Resurrection: {
                        //if (identitys.TryGetValue(opt.identity, out var t)) {
                        //    var p = t.GetComponent<Player>();
                        //    p.Resurrection();
                        //}
                    }
                    break;
            }
        }
     
        ///<summary>
        /// 这个函数用来同步怪物位置和数据，现在有个bug，服务器同步过来时，可能是脚本启动顺序问题，接受服务端消息时，monster列表中还是空的，导致monster创建失败报错，
        /// 这里可以修改成，代码加载本场景的怪物预制体，检查脚本如果发现为空，就先加载预制体
        /// 这样也正好能通过配置表的方式配置monseter
        /// todo
        /// </summary>
        /// <param name="opt"></param>
        /// <returns></returns>
        public Monster CheckMonster(Operation opt) {//这个函数启动早。。。。
            if (!monsterDics.TryGetValue(opt.identity, out Monster monster)) {
                var mid = opt.buffer[0];
                if (monsters.Count == 0) {
                    InitMonsters();
                }
                string monsterpath = monsters[mid];
                monster = ObjectManager.GetInstance.InstanceObject(monsterpath, father: MonsterFather).GetComponent<Monster>();
                monster.transform.position = opt.position;
                monster.transform.rotation = opt.rotation;
                monster.m_GDID = opt.identity;
                monster.m_NpcType = NpcType.monster;
                monsterDics.Add(opt.identity, monster);
                AppTools.Send<string, Monster>((int)NpcEvent.AddMonster,SceneManager.GetActiveScene().name, monster);
            }
            return monster;
        }
    }
}



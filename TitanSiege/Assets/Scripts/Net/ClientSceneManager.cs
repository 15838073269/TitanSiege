
using GF.Const;
using GF.MainGame;
using GF.MainGame.Module;
using GF.MainGame.Module.NPC;
using GF.Unity.AB;
using GF.Utils;
using Net.Client;
using Net.Share;
using Net.System;
using Net.UnityComponent;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GF.NetWork {
    public class ClientSceneManager: NetworkSceneManager {
        private Transform MonsterFather;
        public ListSafe<string> m_MonstersPathList = new ListSafe<string>();
        public Monster[] m_MonsterPrefabList;
        public Dictionary<int, Monster> m_MonsterDics = new Dictionary<int, Monster>();
        private void InitMonsters() {
            //读取配置表  todo
            //先临时添加几个怪物进来，回头开发场景配置表
            string s = "NPCPrefab/lang2.prefab";
            if (!m_MonstersPathList.Contains(s)) {
                m_MonstersPathList.Add(s);
            }
            if (MonsterFather == null) {
                MonsterFather = (new GameObject("Monsters")).transform;
            }
        }
        public override void OnNetworkObjectCreate(Operation opt, NetworkObject identity) {
            var p = identity.GetComponent<Player>();
            if (p != null) {
                p.m_GDID = opt.identity;
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
                        if (p.m_GDID!=ClientBase.Instance.UID) {//不是本地的
                            Debuger.Log($"{opt.identity}释放技能{opt.index2}");
                            p.m_CurrentSkillId = opt.index2;
                            p.m_State.StatusEntry(opt.index1);
                        }
                    }
                    break;
                case Command.SwitchState://发送消息切换玩家状态
                    if (identitys.TryGetValue(opt.identity, out var t1)) {
                        var p = t1.GetComponent<Player>();
                        if (p.m_GDID != ClientBase.Instance.UID) {//不是本地的
                            Debuger.Log($"{opt.identity}切换状态为{opt.index1}");
                            p.m_State.StatusEntry(opt.index1);
                        }
                    }
                    break;
                case Command.PlayerState:
                    if (identitys.TryGetValue(opt.identity, out var t2)) {
                        var p = t2.GetComponent<Player>();
                        p.FightHP = opt.index;
                        p.FightMagic = opt.index1;
                        p.Check();//检查角色是否死亡并同步生命值
                    }
                    break;
                case Command.EnemyPatrol://未发生攻击时，服务端同步场景怪物行为
                    var monster = CheckMonster(opt);
                    monster.m_NetState = opt.cmd1;
                    monster.m_PatrolState = opt.cmd2;
                    monster.FightHP = opt.index1;
                    monster.StatusEntry();
                    monster.transform.position = opt.position;
                    monster.transform.rotation = opt.rotation;
                    break;
                case Command.EnemySwitchState://两种情况使用这个命令，1、主控客户端告知服务端不再同步怪物数据，由服务的自己控制，此时命令发送cmd1和cmd2都是0
                                              //2、主控客户端同步怪物状态给服务器，服务器广播给其他客户端
                                              //客户端这里接收到的，只能是主控客户端同步怪物状态给服务器，服务器广播给其他客户端
                    if (m_MonsterDics.TryGetValue(opt.identity, out var monster2)) {
                        monster2.m_NetState = (int)opt.cmd1;//服务器回传后，要把本地怪物的状态设置为目标值
                        monster2.m_State.StatusEntry((int)opt.cmd2);//切换状态后，客户端自行播放状态动画
                    }
                    break;
                case Command.EnemySync://发生攻击时，客户端怪物状态同步
                    var monster3 = CheckMonster(opt);
                    monster3.m_NetState = opt.cmd1;
                    monster3.m_PatrolState = opt.cmd2;
                    monster3.FightHP = opt.index1;
                    monster3.m_targetID = opt.index2;
                    if (monster3.m_targetID!=ClientBase.Instance.UID) {//如果怪物目标不是本机,说明怪物不是本机在控制同步的，就需要从服务器同步怪物位置信息。 
                        monster3.transform.position = opt.position;
                        monster3.transform.rotation = opt.rotation;
                    }
                    break;
            }
        }
     
        ///<summary>
        /// 这个函数用来同步怪物位置和数据，现在有个bug，服务器同步过来时，可能是脚本启动顺序问题，接受服务端消息时，monster列表中还是空的，导致monster创建失败报错，（bug已解决）
        /// 这里可以修改成，代码加载本场景的怪物预制体，检查脚本如果发现为空，就先加载预制体
        /// 这样也正好能通过配置表的方式配置monseter
        /// todo
        /// </summary>
        /// <param name="opt"></param>
        /// <returns></returns>
        public Monster CheckMonster(Operation opt) {//这个函数启动早。。。。
            if (!m_MonsterDics.TryGetValue(opt.identity, out Monster monster)) {
                var mid = opt.index;
                if (m_MonstersPathList.Count == 0) {
                    InitMonsters();
                }
                string monsterpath = m_MonstersPathList[mid];
                monster = ObjectManager.GetInstance.InstanceObject(monsterpath, father: MonsterFather).GetComponent<Monster>();
                monster.transform.position = opt.position;
                monster.transform.rotation = opt.rotation;
                monster.m_GDID = opt.identity;
                monster.m_NpcType = NpcType.monster;
                m_MonsterDics.Add(opt.identity, monster);
                //AppTools.Send<string, Monster>((int)NpcEvent.AddMonster,SceneManager.GetActiveScene().name, monster);
            }
            return monster;
        }
    }
}



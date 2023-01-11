using ECS;
using GDServer.AI;
using GDServer.Services;
using Net;
using Net.Component;
using Net.Server;
using Net.Share;
using Net.System;
using System.Linq;


namespace GDServer {
    /// <summary>
    /// 场景管理器, 状态同步, 帧同步 固定帧发送当前场景的所有玩家操作
    /// </summary>
    public class GDScene : NetScene<GDClient> {
        internal SceneData sceneData = new SceneData();
        internal readonly MyDictionary<int, AIMonster> monsters = new MyDictionary<int, AIMonster>();
        internal GSystem ecsSystem = new GSystem();

        public void Init() {
            int id = 1;
            foreach (var item in sceneData.monsterPoints) {
                
                if (item.monsters == null)
                    continue;
               
                RoamingPath1 roamingPath = item.roamingPath;
                for (int i = 0; i < item.monsters.Length; i++) {
                    int js = 0;
                    if (roamingPath.waypointsList.Count > i) {
                        js = i;
                    } else {
                        js = i-roamingPath.waypointsList.Count;
                    }
                    var point = roamingPath.waypointsList[js];
                    var monster1 = ecsSystem.Create<Entity>().AddComponent<AIMonster>();
                    monster1.transform = new NTransform();
                    monster1.transform.position = point;
                    monster1.transform.rotation = Quaternion.identity;
                    monster1.roamingPath = roamingPath;
                    monster1.scene = this;
                    monster1.id = id++;
                    monster1.lastpointindex = js;
                    monster1.mid = item.monsters[i].id;
                    monster1.health = item.monsters[i].health;
                    monsters.Add(monster1.id, monster1);
                }
            }
        }

        public override void OnEnter(GDClient client) {
            client.Scene = this;
        }

        /// <summary>
        /// 网络帧同步, 状态同步更新
        /// </summary>
        public override void Update(IServerSendHandle<GDClient> handle, byte cmd = 19) {
            var players = Clients;
            int playerCount = players.Count;
            if (playerCount <= 0)
                return;
            for (int i = 0; i < players.Count; i++)
                players[i].OnUpdate();
            ecsSystem.Update();
            frame++;
            int count = operations.Count;
            if (count > 0) {
                while (count > Split) {
                    OnPacket(handle, cmd, Split);
                    count -= Split;
                }
                if (count > 0)
                    OnPacket(handle, cmd, count);
            }
            Event.UpdateEventFixed();
        }

        public override void OnOperationSync(GDClient client, OperationList list) {
            
            for (int i = 0; i < list.operations.Length; i++) {
                var opt = list.operations[i];
                switch (opt.cmd) {
                    case Command.Skill:
                        for (int n = 0; n < Clients.Count; n++) {
                            if (Clients[n].UserID == opt.identity) {
                                Debuger.Log($"{Clients[n].PlayerID},{Clients[n].UserID}使用了技能{opt.index1}");
                                AddOperation(opt);
                                break;
                            }
                        }
                        break;
                    case Command.AttackPlayer:
                        
                        break;
                    case Command.EnemySync://客户端同步怪物位置的命令
                        if (monsters.TryGetValue(opt.identity, out AIMonster monster1)) {
                            monster1.transform.position = opt.position;
                            monster1.transform.rotation = opt.rotation;
                        }
                        break;
                    case Command.EnemySwitchState://客户端切换怪物状态的命令
                        if (monsters.TryGetValue(opt.identity, out AIMonster monster3)) {
                            if (!monster3.isDeath) {
                                monster3.state = opt.cmd1;
                                monster3.state1 = opt.cmd2;
                                if (monster3.state == 0)
                                    monster3.targetID = 0;
                                AddOperation(opt);
                            } else monster3.PatrolCall();
                        }
                        break;
                    case Command.AIAttack:
                        client.BeAttacked(opt.identity);
                        break;
                    case Command.Resurrection:
                        client.Resurrection();
                        AddOperation(opt);
                        break;
                    default:
                        AddOperation(opt);
                        break;
                }
            }
        }

        public override void OnExit(GDClient client) {
            foreach (var monster in monsters.Values) {
                if (monster.targetID == client.UserID) {
                    monster.targetID = 0;
                    monster.state = 0;
                    monster.state1 = 0;
                }
            }
            if (CurrNum <= 0) //如果没人时要清除操作数据，不然下次进来会直接发送Command.OnPlayerExit指令给客户端，导致客户端的对象被销毁
                operations.Clear();
            else
                base.AddOperation(new Operation(Net.Component.Command.OnPlayerExit, client.UserID));
        }
    }
}

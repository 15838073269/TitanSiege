using ECS;
using GDServer.AI;
using Net;
using Net.Component;
using Net.MMORPG;
using Net.Server;
using Net.Share;
using Net.System;
using Titansiege;

namespace GDServer {
    /// <summary>
    /// 场景管理器, 状态同步, 帧同步 固定帧发送当前场景的所有玩家操作
    /// </summary>
    public class GDScene : NetScene<GDClient> {
        internal MapData mapData = new MapData();
        internal readonly Dictionary<int, AIMonster> monsters = new Dictionary<int, AIMonster>();
        internal GSystem ecsSystem = new GSystem();
        
        public void Init() {
            int identity = 1;
            foreach (var item in mapData.monsterPoints) {
                
                if (item.monsters == null)
                    continue;
               
                PatrolPath partrolpath = item.patrolPath;
                for (int i = 0; i < item.monsters.Length; i++) {
                    var point = partrolpath.waypoints[RandomHelper.Range(0, partrolpath.waypoints.Count)];//获取随机的点
                    var monster1 = ecsSystem.Create<Entity>().AddComponent<AIMonster>();//创建实体和怪物
                    monster1.identity = identity++;//添加唯一id
                    monster1.transform = new NTransform(point, Quaternion.identity);//创建transform，对标unity的transform
                    monster1.patrolPath = partrolpath;
                    monster1.scene = this;
                    monster1.enemyindex = item.monsters[i].id;//客户端怪物的模型索引id，clientManager中的那个
                    //各种属性额赋值从数据表计算
                    if (TitansiegeDB.I.m_Npcs.TryGetValue(item.monsters[i].mysqlid, out monster1.current)) {
                        monster1.UpdateFightProps();
                    }
                    if (monster1.current ==null) {
                        Debuger.Log(monster1.identity + "没有从数据库获取到任何数据，请检查mysqlid配置是否正确");
                    }
                    monsters.Add(monster1.identity, monster1);
                }
            }
        }

        public override void OnEnter(GDClient client) {
            client.Scene = this;
        }

        /// <summary>
        /// 网络帧同步, 状态同步更新
        /// </summary>
        public override void Update(IServerSendHandle<GDClient> handle, byte cmd) {
            var players = Clients;
            int playerCount = players.Count;
            if (playerCount <= 0)
                return;
            for (int i = 0; i < playerCount; i++)
                players[i].OnUpdate();
            ecsSystem.Update();//更新ecs
            int count = operations.Count;
            if (count > 0) {
                frame++;
                while (count > Split) {
                    OnPacket(handle, cmd, Split);
                    count -= Split;
                }
                if (count > 0)
                    OnPacket(handle, cmd, count);
            }
        }

        public override void OnOperationSync(GDClient client, OperationList list) {
            
            for (int i = 0; i < list.operations.Length; i++) {
                var opt = list.operations[i];
                switch (opt.cmd) {
                    case Command.Skill:
                        Debuger.Log($"{client.PlayerID},{client.UserID}使用了技能{opt.index2}");
                        client.CostMp(opt.index3);
                        AddOperation(opt);
                        break;
                    case Command.Attack:
                        if (monsters.TryGetValue(opt.index1,out var monster)) { //尝试获取怪物对象
                            monster.targetID = opt.identity;
                            monster.OnDamage(opt.index);
                            if (monster.isDeath && monster.targetID != 0) {//给玩家加经验
                                client.AddExp(monster.current.Exp);
                            }
                        }
                        break;
                    case Command.EnemyAttack://怪物攻击玩家时
                        for (int n = 0; n < Clients.Count; n++) {
                            if (Clients[n].UserID == opt.identity) {
                                Clients[n].BeAttacked(opt.index);
                                break;
                            }
                        }
                        break;
                    case Command.EnemySync://客户端同步怪物位置的命令，同步完后，服务端monster中的update会按帧将状态同步给所有客户端， 不需要再单独处理
                        if (monsters.TryGetValue(opt.identity, out AIMonster monster1)) {
                            monster1.transform.position = opt.position;
                            monster1.transform.rotation = opt.rotation;
                        }
                        break;
                    case Command.EnemySwitchState://两种情况使用这个命令，1、主控客户端告知服务端不再同步怪物数据，由服务的自己控制，此时命令发送cmd1和cmd2都是0
                        //2、主控客户端同步怪物状态给服务器，服务器广播给其他客户端
                        if (monsters.TryGetValue(opt.identity, out AIMonster monster3)) {
                            if (!monster3.isDeath) {//服务端判断怪物死没死，没死就切换状态，以后服务端自己同步
                                monster3.state = opt.cmd1;
                                monster3.patrolstate = opt.cmd2;
                                if (opt.cmd1 == 0) {//说明是主控客户端告知服务端不再同步怪物
                                    monster3.FP.FightHP = opt.index;
                                }
                                AddOperation(opt);
                            } else {//如果怪已经死亡，却还收到切换状态命令，说明客户端与服务端信息不一致，就把服务端数据同步给客户端
                                monster3.PatrolCall();
                            }
                        }
                        break;
                    case Command.AIBeAttack://技能伤害计算完毕后，发给服务端
                        //if (monsters.TryGetValue(opt.identity, out AIMonster monster4)) {
                        //    if (!monster4.isDeath) {//服务端判断怪物死没死，没死就传递伤害数值
                        //        monster4.OnDamage(opt.index1);
                        //        if (!monster4.isDeath) {
                        //            AddOperation(opt);
                        //        } else {
                        //            monster4.PatrolCall();
                        //        }
                        //    } else {//如果怪已经死亡，却还收到切换状态命令，说明客户端与服务端信息不一致，就把服务端数据同步给客户端
                        //        monster4.PatrolCall();
                        //    }
                        //}
                        break;
                    case Command.Resurrection://玩家复活
                        client.Resurrection();
                        AddOperation(opt);
                        break;
                    default://这里默认，要原路返回客户端
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
                    monster.patrolstate = 0;
                    monster.FP.FightHP = monster.FP.FightMaxHp;//脱离战斗就回满血
                }
            }
            if (Count <= 0) //如果没人时要清除操作数据，不然下次进来会直接发送Command.OnPlayerExit指令给客户端，导致客户端的对象被销毁
                operations.Clear();
            else
                AddOperation(new Operation(Command.OnPlayerExit, client.UserID));
        }
    }
}


using cmd;
using GF.Const;
using GF.MainGame;
using GF.MainGame.Module;
using GF.MainGame.Module.NPC;
using GF.Service;
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
        public override async void OnNetworkObjectCreate(Operation opt, NetworkObject identity) {
            var p = identity.GetComponent<Player>();
            if (p != null) {
                p.m_GDID = opt.identity;
                p.m_NpcType = NpcType.player;
                AppTools.Send<Player>((int)NpcEvent.AddPlayer, p);
                //请求服务器，更新血条和名称
                var task = await ClientBase.Instance.Call((ushort)ProtoType.playerupdateprop,pars:opt.identity);
                if (!task.IsCompleted) {
                    Debuger.Log("请求超时，请检查网络链接");
                    return;
                }
                var code = task.model.AsInt;
                switch (code) {
                    case 0:
                        Debuger.Log("服务器不存在角色" + p.m_GDID);
                        break;
                    case 1:
                        FightProp fp = task.model.As<FightProp>();
                        p.m_PlayerName = fp.PlayerName;
                        p.FP = fp;
                        //创建血条
                        AppTools.Send<NPCBase>((int)HPEvent.CreateHPUI, p);
                        break;
                    default:
                        Debuger.Log("请求更新玩家数据遇到未知命令：" + code);
                        break;
                }
            }
        }
      
        public override void OnOtherDestroy(NetworkObject identity) {
            var p = identity.GetComponent<Player>();
            if (p != null) {
                Destroy(p.gameObject);
                AppTools.Send<int>((int)NpcEvent.Removeplayer,p.m_GDID);
            }
        }
        
        public override void OnOtherOperator(Operation opt) {
            switch (opt.cmd) {
                case Command.Skill:
                    if (identitys.TryGetValue(opt.identity, out var t)) {
                        var p = t.GetComponent<Player>();
                        if (p.m_GDID!=ClientBase.Instance.UID) {//不是本地的
                            //攻击，重置战斗姿态切换时间
                            p.m_Resetidletime = AppConfig.FightReset;
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
                            if (p.m_AllStateID["idle"]== opt.index1 && p.IsFight) { //如果要切换idle,且当前为战斗状态
                                p.SetFight(false);
                            }
                            if (p.m_AllStateID["fightidle"] == opt.index1 && !p.IsFight) { //如果要切换fightidle,且当前为非战斗状态
                                p.SetFight(true);
                            }
                        }
                    }
                    break;
                case Command.PlayerState:
                    if (identitys.TryGetValue(opt.identity, out var t2)) {
                        var p = t2.GetComponent<Player>();
                        if (opt.index!= p.FP.FightHP) {
                            //被攻击了，重置战斗姿态切换时间
                            p.m_Resetidletime = AppConfig.FightReset;
                            p.FP.FightHP = opt.index;
                        }
                        p.FP.FightMagic = opt.index1;
                        p.FP.FightMaxHp = opt.index2;
                        p.FP.FightMaxMagic = opt.index3;
                        if (p.FP.FightHP> p.FP.FightMaxHp) {
                            p.FP.FightHP = p.FP.FightMaxHp;
                        }
                        if (p.FP.FightMagic > p.FP.FightMaxMagic) {
                            p.FP.FightMagic = p.FP.FightMaxMagic;
                        }
                        if ((!string.IsNullOrEmpty(opt.name)) ) {//网络玩家赋值名称，本地的不用
                            p.m_PlayerName = opt.name;
                            //创建显示血条和名称
                            AppTools.Send<NPCBase>((int)HPEvent.CreateHPUI, p);
                        }
                        AppTools.Send<NPCBase>((int)HPEvent.UpdateHp,p);
                        //if (opt.identity== ClientBase.Instance.UID) {//本地玩家才需要更新蓝条//更新血条时直接更新蓝条了，没必要单独更新了
                        //    AppTools.Send<NPCBase>((int)HPEvent.UpdateMp, p);
                        //}
                        p.Check();//检查角色是否死亡并同步生命值
                    }
                    break;
                //玩家属性变化后更新的命令
                case Command.PlayerUpdateProp:
                    if (identitys.TryGetValue(opt.identity, out var t3)) {
                        var p = t3.GetComponent<Player>();
                        p.UpProp(opt.index, opt.index1,opt.index2);
                    }
                    break;
                case Command.EnemyPatrol://1、未发生攻击时，服务端同步场景怪物行为
                    //2、死亡时也是发送这个命令，不同的是，死亡只发一次，客户端处理死亡后，直到收到怪物复活命令之前，不会再进行任何同步和命令
                    var monster = CheckMonster(opt);
                    if (monster == null) {
                        return;
                    }
                    ////如果原本是死亡的状态，证明这里需要复活了，就需要加载复活特效
                    //if ((monster.m_PatrolState == monster.m_AllStateID["die"])&& opt.cmd2 != monster.m_AllStateID["die"]) {
                    //    monster.Fuhuo();
                    //}
                    monster.m_NetState = opt.cmd1;
                    monster.m_PatrolState = opt.cmd2;
                    monster.FP.FightHP = opt.index1;
                    
                    monster.StatusEntry();
                    monster.transform.position = opt.position;
                    monster.transform.rotation = opt.rotation;
                    
                    break;
                case Command.EnemySwitchState://两种情况使用这个命令，1、主控客户端告知服务端不再同步怪物数据，由服务的自己控制，此时命令发送cmd1和cmd2都是0
                                              //2、主控客户端同步怪物状态给服务器，服务器广播给其他客户端
                    if (m_MonsterDics.TryGetValue(opt.identity, out var monster2)) {
                        monster2.m_NetState = (int)opt.cmd1;//服务器回传后，要把本地怪物的状态设置为目标值
                        monster2.m_State.StatusEntry((int)opt.cmd2);//切换状态后，客户端自行播放状态动画
                        if (opt.cmd1 == 0) {//说明是主控客户端告知服务端不再同步怪物
                            monster2.FP.FightHP = opt.index;
                            AppTools.Send<NPCBase>((int)HPEvent.UpdateHp, monster2);
                        }
                    }
                    break;
                case Command.EnemySync://怪物同步，客户端怪物状态同步
                    var monster3 = CheckMonster(opt);
                    if (monster3 == null) {
                        return;
                    }
                    monster3.m_NetState = opt.cmd1;
                    monster3.m_PatrolState = opt.cmd2;
                    monster3.FP.FightHP = opt.index1;
                    monster3.m_targetID = opt.index2;
                    AppTools.Send<NPCBase>((int)HPEvent.UpdateHp, monster3);
                    if (monster3.m_targetID!=ClientBase.Instance.UID) {//如果怪物目标不是本机,说明怪物不是本机在控制同步的，就需要从服务器同步怪物位置信息。 
                        monster3.transform.position = opt.position;
                        monster3.transform.rotation = opt.rotation;
                    }
                    break;
                case Command.EnemyUpdateProp://怪物属性同步给客户端，一般是第一次创建怪物或者怪物属性发生变化时使用
                    var monster4 = CheckMonster(opt);
                    if (monster4 == null) {
                        return;
                    }
                    monster4.FP.FightHP = opt.index1;
                    monster4.FP.FightMaxHp = opt.index2;
                    //更新血条
                    AppTools.Send<NPCBase>((int)HPEvent.UpdateHp, monster4);
                    break;
                case Command.Resurrection://某个玩家复活
                    if (identitys.TryGetValue(opt.identity, out var t4)) {
                        var p = t4.GetComponent<Player>();
                        p.Fuhuo();
                    }
                    break;
                case Command.CurrentTalk://当前频道的聊天
                    //当前场景的玩家都能收到这个消息
                    AppTools.Send<string, TalkType>((int)TalkEvent.AddOneTalk, opt.name, (TalkType)opt.index);
                    break;
            }
        }
     
        ///<summary>
        /// 这个函数用来同步怪物位置和数据，现在有个bug，服务器同步过来时，可能是脚本启动顺序问题，接受服务端消息时，monster列表中还是空的，导致monster创建失败报错，（bug已解决）
        /// 这里可以修改成，代码加载本场景的怪物预制体，检查脚本如果发现为空，就先加载预制体
        /// 这样也正好能通过配置表的方式配置monseter
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



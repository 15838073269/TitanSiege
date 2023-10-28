
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
            //��ȡ���ñ�  todo
            //����ʱ��Ӽ��������������ͷ�����������ñ�
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
                //���������������Ѫ��������
                var task = await ClientBase.Instance.Call((ushort)ProtoType.playerupdateprop,pars:opt.identity);
                if (!task.IsCompleted) {
                    Debuger.Log("����ʱ��������������");
                    return;
                }
                var code = task.model.AsInt;
                switch (code) {
                    case 0:
                        Debuger.Log("�����������ڽ�ɫ" + p.m_GDID);
                        break;
                    case 1:
                        FightProp fp = task.model.As<FightProp>();
                        p.m_PlayerName = fp.PlayerName;
                        p.FP = fp;
                        //����Ѫ��
                        AppTools.Send<NPCBase>((int)HPEvent.CreateHPUI, p);
                        break;
                    default:
                        Debuger.Log("������������������δ֪���" + code);
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
                        if (p.m_GDID!=ClientBase.Instance.UID) {//���Ǳ��ص�
                            //����������ս����̬�л�ʱ��
                            p.m_Resetidletime = AppConfig.FightReset;
                            Debuger.Log($"{opt.identity}�ͷż���{opt.index2}");
                            p.m_CurrentSkillId = opt.index2;
                            p.m_State.StatusEntry(opt.index1);
                        }
                    }
                    break;
                case Command.SwitchState://������Ϣ�л����״̬
                    if (identitys.TryGetValue(opt.identity, out var t1)) {
                        var p = t1.GetComponent<Player>();
                        if (p.m_GDID != ClientBase.Instance.UID) {//���Ǳ��ص�
                            Debuger.Log($"{opt.identity}�л�״̬Ϊ{opt.index1}");
                            p.m_State.StatusEntry(opt.index1);
                            if (p.m_AllStateID["idle"]== opt.index1 && p.IsFight) { //���Ҫ�л�idle,�ҵ�ǰΪս��״̬
                                p.SetFight(false);
                            }
                            if (p.m_AllStateID["fightidle"] == opt.index1 && !p.IsFight) { //���Ҫ�л�fightidle,�ҵ�ǰΪ��ս��״̬
                                p.SetFight(true);
                            }
                        }
                    }
                    break;
                case Command.PlayerState:
                    if (identitys.TryGetValue(opt.identity, out var t2)) {
                        var p = t2.GetComponent<Player>();
                        if (opt.index!= p.FP.FightHP) {
                            //�������ˣ�����ս����̬�л�ʱ��
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
                        if ((!string.IsNullOrEmpty(opt.name)) ) {//������Ҹ�ֵ���ƣ����صĲ���
                            p.m_PlayerName = opt.name;
                            //������ʾѪ��������
                            AppTools.Send<NPCBase>((int)HPEvent.CreateHPUI, p);
                        }
                        AppTools.Send<NPCBase>((int)HPEvent.UpdateHp,p);
                        //if (opt.identity== ClientBase.Instance.UID) {//������Ҳ���Ҫ��������//����Ѫ��ʱֱ�Ӹ��������ˣ�û��Ҫ����������
                        //    AppTools.Send<NPCBase>((int)HPEvent.UpdateMp, p);
                        //}
                        p.Check();//����ɫ�Ƿ�������ͬ������ֵ
                    }
                    break;
                //������Ա仯����µ�����
                case Command.PlayerUpdateProp:
                    if (identitys.TryGetValue(opt.identity, out var t3)) {
                        var p = t3.GetComponent<Player>();
                        p.UpProp(opt.index, opt.index1,opt.index2);
                    }
                    break;
                case Command.EnemyPatrol://1��δ��������ʱ�������ͬ������������Ϊ
                    //2������ʱҲ�Ƿ�����������ͬ���ǣ�����ֻ��һ�Σ��ͻ��˴���������ֱ���յ����︴������֮ǰ�������ٽ����κ�ͬ��������
                    var monster = CheckMonster(opt);
                    if (monster == null) {
                        return;
                    }
                    ////���ԭ����������״̬��֤��������Ҫ�����ˣ�����Ҫ���ظ�����Ч
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
                case Command.EnemySwitchState://�������ʹ��������1�����ؿͻ��˸�֪����˲���ͬ���������ݣ��ɷ�����Լ����ƣ���ʱ�����cmd1��cmd2����0
                                              //2�����ؿͻ���ͬ������״̬�����������������㲥�������ͻ���
                    if (m_MonsterDics.TryGetValue(opt.identity, out var monster2)) {
                        monster2.m_NetState = (int)opt.cmd1;//�������ش���Ҫ�ѱ��ع����״̬����ΪĿ��ֵ
                        monster2.m_State.StatusEntry((int)opt.cmd2);//�л�״̬�󣬿ͻ������в���״̬����
                        if (opt.cmd1 == 0) {//˵�������ؿͻ��˸�֪����˲���ͬ������
                            monster2.FP.FightHP = opt.index;
                            AppTools.Send<NPCBase>((int)HPEvent.UpdateHp, monster2);
                        }
                    }
                    break;
                case Command.EnemySync://����ͬ�����ͻ��˹���״̬ͬ��
                    var monster3 = CheckMonster(opt);
                    if (monster3 == null) {
                        return;
                    }
                    monster3.m_NetState = opt.cmd1;
                    monster3.m_PatrolState = opt.cmd2;
                    monster3.FP.FightHP = opt.index1;
                    monster3.m_targetID = opt.index2;
                    AppTools.Send<NPCBase>((int)HPEvent.UpdateHp, monster3);
                    if (monster3.m_targetID!=ClientBase.Instance.UID) {//�������Ŀ�겻�Ǳ���,˵�����ﲻ�Ǳ����ڿ���ͬ���ģ�����Ҫ�ӷ�����ͬ������λ����Ϣ�� 
                        monster3.transform.position = opt.position;
                        monster3.transform.rotation = opt.rotation;
                    }
                    break;
                case Command.EnemyUpdateProp://��������ͬ�����ͻ��ˣ�һ���ǵ�һ�δ���������߹������Է����仯ʱʹ��
                    var monster4 = CheckMonster(opt);
                    if (monster4 == null) {
                        return;
                    }
                    monster4.FP.FightHP = opt.index1;
                    monster4.FP.FightMaxHp = opt.index2;
                    //����Ѫ��
                    AppTools.Send<NPCBase>((int)HPEvent.UpdateHp, monster4);
                    break;
                case Command.Resurrection://ĳ����Ҹ���
                    if (identitys.TryGetValue(opt.identity, out var t4)) {
                        var p = t4.GetComponent<Player>();
                        p.Fuhuo();
                    }
                    break;
                case Command.CurrentTalk://��ǰƵ��������
                    //��ǰ��������Ҷ����յ������Ϣ
                    AppTools.Send<string, TalkType>((int)TalkEvent.AddOneTalk, opt.name, (TalkType)opt.index);
                    break;
            }
        }
     
        ///<summary>
        /// �����������ͬ������λ�ú����ݣ������и�bug��������ͬ������ʱ�������ǽű�����˳�����⣬���ܷ������Ϣʱ��monster�б��л��ǿյģ�����monster����ʧ�ܱ�����bug�ѽ����
        /// ��������޸ĳɣ�������ر������Ĺ���Ԥ���壬���ű��������Ϊ�գ����ȼ���Ԥ����
        /// ����Ҳ������ͨ�����ñ�ķ�ʽ����monseter
        /// </summary>
        /// <param name="opt"></param>
        /// <returns></returns>
        public Monster CheckMonster(Operation opt) {//������������硣������
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



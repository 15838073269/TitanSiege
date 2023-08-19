
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
                        if (p.m_GDID!=ClientBase.Instance.UID) {//���Ǳ��ص�
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
                        }
                    }
                    break;
                case Command.PlayerState:
                    if (identitys.TryGetValue(opt.identity, out var t2)) {
                        var p = t2.GetComponent<Player>();
                        p.FightHP = opt.index;
                        p.FightMagic = opt.index1;
                        p.Check();//����ɫ�Ƿ�������ͬ������ֵ
                    }
                    break;
                case Command.EnemyPatrol://δ��������ʱ�������ͬ������������Ϊ
                    var monster = CheckMonster(opt);
                    monster.m_NetState = opt.cmd1;
                    monster.m_PatrolState = opt.cmd2;
                    monster.FightHP = opt.index1;
                    monster.StatusEntry();
                    monster.transform.position = opt.position;
                    monster.transform.rotation = opt.rotation;
                    break;
                case Command.EnemySwitchState://�������ʹ��������1�����ؿͻ��˸�֪����˲���ͬ���������ݣ��ɷ�����Լ����ƣ���ʱ�����cmd1��cmd2����0
                                              //2�����ؿͻ���ͬ������״̬�����������������㲥�������ͻ���
                                              //�ͻ���������յ��ģ�ֻ�������ؿͻ���ͬ������״̬�����������������㲥�������ͻ���
                    if (m_MonsterDics.TryGetValue(opt.identity, out var monster2)) {
                        monster2.m_NetState = (int)opt.cmd1;//�������ش���Ҫ�ѱ��ع����״̬����ΪĿ��ֵ
                        monster2.m_State.StatusEntry((int)opt.cmd2);//�л�״̬�󣬿ͻ������в���״̬����
                    }
                    break;
                case Command.EnemySync://��������ʱ���ͻ��˹���״̬ͬ��
                    var monster3 = CheckMonster(opt);
                    monster3.m_NetState = opt.cmd1;
                    monster3.m_PatrolState = opt.cmd2;
                    monster3.FightHP = opt.index1;
                    monster3.m_targetID = opt.index2;
                    if (monster3.m_targetID!=ClientBase.Instance.UID) {//�������Ŀ�겻�Ǳ���,˵�����ﲻ�Ǳ����ڿ���ͬ���ģ�����Ҫ�ӷ�����ͬ������λ����Ϣ�� 
                        monster3.transform.position = opt.position;
                        monster3.transform.rotation = opt.rotation;
                    }
                    break;
            }
        }
     
        ///<summary>
        /// �����������ͬ������λ�ú����ݣ������и�bug��������ͬ������ʱ�������ǽű�����˳�����⣬���ܷ������Ϣʱ��monster�б��л��ǿյģ�����monster����ʧ�ܱ�����bug�ѽ����
        /// ��������޸ĳɣ�������ر������Ĺ���Ԥ���壬���ű��������Ϊ�գ����ȼ���Ԥ����
        /// ����Ҳ������ͨ�����ñ�ķ�ʽ����monseter
        /// todo
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



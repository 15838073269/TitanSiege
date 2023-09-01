#if SERVER

using Net.Event;
using Net.System;
using System.Collections.Generic;

namespace Titansiege {
    public partial class TitansiegeDB {
        /// <summary>
        /// ���߳���
        /// </summary>
        private readonly object m_syncroot = new object();
        public Dictionary<long, CharactersData> m_CharactersData = new Dictionary<long, CharactersData>();
        /// <summary>
        /// keyΪ�˻����ƣ�valueΪ����
        /// </summary>
        public Dictionary<string, UsersData> m_Users = new Dictionary<string, UsersData>(); 
        public Dictionary<ConfigType, ConfigData> m_Configs = new Dictionary<ConfigType, ConfigData>();
        public Dictionary<long, NpcsData> m_Npcs = new Dictionary<long, NpcsData>();
        public void InitData() {
            TitansiegeDB.I.Init((list) => {
                foreach (var item in list) {
                    if (item is CharactersData data1) {
                        m_CharactersData.Add(data1.ID, data1);
                    }
                    if (item is UsersData data2) {
                        m_Users.Add(data2.Username, data2);
                    }
                    if (item is ConfigData data3) {
                        m_Configs.Add((ConfigType)data3.Id, data3);
                    }
                    if (item is NpcsData data4) {
                        m_Npcs.Add(data4.ID, data4);
                    }
                }
            });
            NDebug.Log($"���ݼ������,����������{m_CharactersData.Count + m_Users.Count}��");
            //mysql���ݼ�����
            ThreadManager.Invoke("MySqlExecuted", 1f, TitansiegeDB.I.Executed, true);
            //var student = new CharactersData();//��  �������ֱ����д����ҪNewTableRow
            //student.ID = 1;//�ֶ�̫�������������
            //student.Name = "fasdsad";

            //student.NewTableRow();//Ҫ�����������ͬ����mysql

            //CharactersData.Add(student.Name, student);

            ////var student1 = studentDatas["����"];//��

            ////student1.Exp = 4565;//��

            //student1.Delete();//ɾ
        }

        /// <summary>
        /// ��ȡÿ���������id�����̰߳�ȫ
        /// </summary>
        /// <param name="type">����</param>
        /// <returns></returns>
        public int GetConfigID(ConfigType type) {
            lock (m_syncroot) {
                if (!m_Configs.TryGetValue(type, out var data)) {
                    m_Configs.Add(type, data = new ConfigData((int)type, type.ToString(), 1, $"��¼{type}������id����"));
                }
                return ++data.Count;
            }
        }
        /// <summary>
        /// ��ȡÿ�����data�����̰߳�ȫ
        /// </summary>
        /// <param name="type">����</param>
        /// <returns></returns>
        public ConfigData GetConfigData(ConfigType type) {
            lock (m_syncroot) {
                if (!m_Configs.TryGetValue(type, out var data)) {
                    m_Configs.Add(type, data = new ConfigData((int)type, type.ToString(), 1, $"��¼{type}������id����"));
                }
                return data;
            }
        }
    }
    public enum ConfigType{
        Users = 1,
        CharacterData = 2,
    }
}

#endif

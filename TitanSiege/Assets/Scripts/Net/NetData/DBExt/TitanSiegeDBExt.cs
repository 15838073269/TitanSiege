#if SERVER

using Net.Event;
using Net.System;
using System.Collections.Generic;

namespace Titansiege {
    public partial class TitansiegeDB {
        /// <summary>
        /// 多线程锁
        /// </summary>
        private readonly object m_syncroot = new object();
        public Dictionary<long, CharactersData> m_CharactersData = new Dictionary<long, CharactersData>();
        /// <summary>
        /// key为账户名称，value为数据
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
            NDebug.Log($"数据加载完毕,共加载数据{m_CharactersData.Count + m_Users.Count}条");
            //mysql数据检查更新
            ThreadManager.Invoke("MySqlExecuted", 1f, TitansiegeDB.I.Executed, true);
            //var student = new CharactersData();//增  如果参数直接填写则不需要NewTableRow
            //student.ID = 1;//字段太多可以这样设置
            //student.Name = "fasdsad";

            //student.NewTableRow();//要调用这个才能同步到mysql

            //CharactersData.Add(student.Name, student);

            ////var student1 = studentDatas["张三"];//查

            ////student1.Exp = 4565;//改

            //student1.Delete();//删
        }

        /// <summary>
        /// 获取每个表的自增id，多线程安全
        /// </summary>
        /// <param name="type">表类</param>
        /// <returns></returns>
        public int GetConfigID(ConfigType type) {
            lock (m_syncroot) {
                if (!m_Configs.TryGetValue(type, out var data)) {
                    m_Configs.Add(type, data = new ConfigData((int)type, type.ToString(), 1, $"记录{type}表自增id计数"));
                }
                return ++data.Count;
            }
        }
        /// <summary>
        /// 获取每个表的data，多线程安全
        /// </summary>
        /// <param name="type">表类</param>
        /// <returns></returns>
        public ConfigData GetConfigData(ConfigType type) {
            lock (m_syncroot) {
                if (!m_Configs.TryGetValue(type, out var data)) {
                    m_Configs.Add(type, data = new ConfigData((int)type, type.ToString(), 1, $"记录{type}表自增id计数"));
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

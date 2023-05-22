using System;
using System.Collections.Generic;
using System.IO;

namespace MySqlDataBuild
{
    public class Persist
    {
        private static Persist i;
        public static Persist I 
        { 
            get
            {
                if (i == null)
                {
                    i = new Persist();
                    LoadData(ref i);
                }
                return i;
            }
        }

        public string version;
        public string mysqlDBName, ip, port, pwd, user, mysqlPath;
        public string sqliteDBPath, sqliteGeneratePath;
        public List<string> mysqlDBNames = new List<string>();
        public List<string> mysqlPaths = new List<string>();
        public int namespaceIndex, namespaceIndex1;
        public bool clearOldFiles, compatible, creatDbPath;

        public static void LoadData(ref Persist persist)
        {
            if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + "persist.data"))
                return;
            var jsonText = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "persist.data");
            persist = Newtonsoft.Json.JsonConvert.DeserializeObject<Persist>(jsonText);
        }

        public static void SaveData()
        {
            var jsonText = Newtonsoft.Json.JsonConvert.SerializeObject(I);
            File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + "persist.data", jsonText);
        }
    }
}

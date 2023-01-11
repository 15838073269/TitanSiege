using Net;
using System;
using System.Collections.Generic;
using System.IO;

namespace GDServer
{
    class Time {
        internal static float time;
        internal static float deltaTime = 0.033f;
    }
    [Serializable]
    public class RoamingPath1
    {
        public List<Vector3> waypointsList = new List<Vector3>();
    }

    [Serializable]
    public class MonsterData
    {
        public int id;
        public int health;
    }

    [Serializable]
    public class MonsterPoint1
    {
        public MonsterData[] monsters;
        public RoamingPath1 roamingPath;
    }

    [Serializable]
    public class SceneData
    {
        public string name;
        public List<MonsterPoint1> monsterPoints = new List<MonsterPoint1>();

        public static SceneData ReadData(string path)
        {
            var jsonStr = File.ReadAllText(path);
            return Newtonsoft_X.Json.JsonConvert.DeserializeObject<SceneData>(jsonStr);
        }
    }
}
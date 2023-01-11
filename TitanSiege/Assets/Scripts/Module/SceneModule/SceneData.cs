/****************************************************
    文件：SceneData.cs
	作者：昔莘
    邮箱: 304183153@qq.com
    日期：2022/12/28 23:15:24
	功能：Nothing
*****************************************************/

using System.Collections.Generic;
using System;
using UnityEngine;
using System.IO;
using Net;
namespace GF.MainGame.Module {
    [Serializable]
    public class RoamingPath1 {
        public List<Net.Vector3> waypointsList = new List<Net.Vector3>();

    }

    [Serializable]
    public class MonsterData {
        public int id;
        public int health;
    }

    [Serializable]
    public class MonsterPoint1 {
        public MonsterData[] monsters;
        public RoamingPath1 roamingPath;
    }

    [Serializable]
    public class SceneData {
        public string name;
        public List<MonsterPoint1> monsterPoints = new List<MonsterPoint1>();

        public static SceneData ReadData(string path) {
            var jsonStr = File.ReadAllText(path);
            return Newtonsoft_X.Json.JsonConvert.DeserializeObject<SceneData>(jsonStr);
        }
    }
}
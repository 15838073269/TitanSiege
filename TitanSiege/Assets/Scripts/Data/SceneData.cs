/****************************************************
    文件：SceneData.cs
	作者：昔莘
    邮箱: 304183153@qq.com
    日期：2022/12/8 11:55:52
	功能：Nothing
*****************************************************/
using GF.ConfigTable;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace GF.MainGame.Data {

    [System.Serializable]
    public class SceneData : ExcelBase {
#if UNITY_EDITOR
        /// <summary>
        /// 编辑器调用
        /// </summary>
        public override void Construction() {
            //base.Construction();
            AllSceneDatas = new List<SceneDataBase>();
            //初始化几个测试用的数据
            SceneDataBase t = new SceneDataBase();
            t.id = 1;
            t.name = "圣城废墟";
            t.desc = "天空圣城纪元前的废墟。";
            t.audiopath = "audio/night.mp4";
            t.monsterlist = new List<NData>();
            NData md = new NData();
            md.ndid = 1;
            md.prepath = "NPCPrefab/cc.prefab";
            md.posx = 0.2f;
            md.posy = 0.2f;
            md.posz = 0.2f;
            md.rotax = 0.2f;
            md.rotay = 0.2f;
            md.rotaz = 0.2f;
            md.scalx = 0.2f;
            md.scaly = 0.2f;
            md.scalz = 0.2f;
            t.monsterlist.Add(md);
            t.npclist = new List<NData>();
            NData nd = new NData();
            nd.ndid = 1;
            nd.prepath = "NPCPrefab/cc.prefab";
            nd.posx = 0.2f;
            nd.posy = 0.2f;
            nd.posz = 0.2f;
            nd.rotax = 0.2f;
            nd.rotay = 0.2f;
            nd.rotaz = 0.2f;
            nd.scalx = 0.2f;
            nd.scaly = 0.2f;
            nd.scalz = 0.2f;
            nd.renwulist = new List<int>();
            nd.renwulist.Add(1);
            nd.renwulist.Add(2);
            nd.renwulist.Add(3);
            t.npclist.Add(nd);
            if (!AllSceneDatas.Contains(t)) {
                AllSceneDatas.Add(t);
            } else {
                Debuger.Log("数据已存在");
            }
        }
#endif
        /// <summary>
        /// 初始化
        /// </summary>
        public override void Init() {
            base.Init();
            m_AllSceneDataDic.Clear();
            for (int i = 0; i < AllSceneDatas.Count; i++) {
                if (m_AllSceneDataDic.ContainsKey(AllSceneDatas[i].id)) {
                    Debuger.LogError($"id{AllSceneDatas[i].id}已存在");
                } else {
                    m_AllSceneDataDic.Add(AllSceneDatas[i].id, AllSceneDatas[i]);
                }
            }
        }
        /// <summary>
        /// 根据id获取npc数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public SceneDataBase FindNPCByID(int id) {
            SceneDataBase scenedata;
            m_AllSceneDataDic.TryGetValue(id, out scenedata);
            return scenedata;
        }

        public override void Clear() {
            throw new System.NotImplementedException();
        }

        [XmlIgnore]//这个标签代表不需要序列化，dic序列化会报错
        public Dictionary<int, SceneDataBase> m_AllSceneDataDic = new Dictionary<int, SceneDataBase>();
        /// <summary>
        /// 这里不要new，否则反序列化时，可能会导致数据叠加
        /// AllNPC只会用在反序列化赋值
        /// </summary>
        [XmlElement("AllSceneDatas")]
        public List<SceneDataBase> AllSceneDatas { get; set; }
    }
    /// <summary>
    /// NPC的数据结构类
    /// </summary>
    [System.Serializable]
    public class SceneDataBase {
        //ID
        [XmlAttribute("ID")]
        public int id { get; set; }

        [XmlAttribute("场景名称")]
        public string name { get; set; }

        [XmlAttribute("场景描述")]
        public string desc { get; set; }
        [XmlAttribute("场景音乐")]
        public string audiopath { get; set; }

        [XmlElement("怪物list")]
        public List<NData> monsterlist { get; set; }
        [XmlElement("NPClist")]
        public List<NData> npclist { get; set; }
    }
    
   
}
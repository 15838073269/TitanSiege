/****************************************************
    文件：SceneData.cs
	作者：昔莘
    邮箱: 304183153@qq.com
    日期：2022/12/8 11:55:52
	功能：Nothing
*****************************************************/
using System.Xml.Serialization;
using System.Collections.Generic;

namespace GDServer {

    [System.Serializable]
    public class SceneData : ExcelBase {
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
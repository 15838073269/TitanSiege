using GF.ConfigTable;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
namespace GF.MainGame.Data {
    //数据表不能写命名空间内，否则，AppDomain.CurrentDomain.GetAssemblies()找不到它，因为它只能获取到运行时加载的程序集，MainGame.Data 运行时并不需要加载
    //目前通过写死命名空间的方式来实现了，可以写在命名空间内
    [System.Serializable]
    public class LevelUpData : ExcelBase {
#if UNITY_EDITOR
        /// <summary>
        /// 编辑器调用
        /// </summary>
        public override void Construction() {
            //base.Construction();
            AllLevelData = new List<LevelUpDataBase>();
            //初始化几个测试用的数据
            LevelUpDataBase nb1 = new LevelUpDataBase();
            nb1.ID = 1;
            nb1.LevelName = "剑士升级";
            nb1.UpExp = 0;
            nb1.Shengming = 500;
            nb1.Fali = 100;
            nb1.Tizhi = 1;
            nb1.Liliang = 8;
            nb1.Minjie = 8;
            nb1.Moli = 8;
            nb1.Meili = 99;
            nb1.Xingyun = 8;
            nb1.Lianjin = 50;
            nb1.Duanzao = 50;
            if (!AllLevelData.Contains(nb1)) {
                AllLevelData.Add(nb1);
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
            if (AllLevelData == null) {
                AllLevelData = new List<LevelUpDataBase>();
            }
            m_AllLevelDataDic.Clear();
            for (ushort i = 0; i < AllLevelData.Count; i++) {
                if (m_AllLevelDataDic.ContainsKey(AllLevelData[i].ID)) {
                    Debuger.LogError($"id{AllLevelData[i].ID}已存在");
                } else {
                    m_AllLevelDataDic.Add(AllLevelData[i].ID, AllLevelData[i]);
                }
            }
        }
        /// <summary>
        /// 根据id获取npc数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public LevelUpDataBase FindByID(ushort id) {
            LevelUpDataBase npc;
            m_AllLevelDataDic.TryGetValue(id, out npc);
            return npc;
        }

        public override void Clear() {
            throw new System.NotImplementedException();
        }

        [XmlIgnore]//这个标签代表不需要序列化，dic序列化会报错
        public Dictionary<ushort, LevelUpDataBase> m_AllLevelDataDic = new Dictionary<ushort, LevelUpDataBase>();
        /// <summary>
        /// 这里不要new，否则反序列化时，可能会导致数据叠加
        /// AllNPC只会用在反序列化赋值
        /// </summary>
        [XmlElement("AllLevelData")]
        public List<LevelUpDataBase> AllLevelData { get; set; }
    }
    /// <summary>
    /// NPC的数据结构类
    /// </summary>
    [System.Serializable]
    public class LevelUpDataBase {
        //iD
        [XmlAttribute("ID")]
        public ushort ID { get; set; }
        [XmlAttribute("数值名称")]
        public string LevelName { get; set; }

        [XmlAttribute("生命")]
        public ushort Shengming { get; set; }
        [XmlAttribute("法力")]
        public ushort Fali { get; set; }
       
        [XmlAttribute("升级经验")]
        public int UpExp { get; set; }

        [XmlAttribute("体质")]//加层防御和生命值
        public ushort Tizhi { get; set; }
        [XmlAttribute("力量")]//加层外功伤害，外功减伤
        public ushort Liliang { get; set; }
        [XmlAttribute("敏捷")]//加强暴击几率，加强功法特殊效果的出现几率，例如功法加力，起死回生概率提升，致盲，暴击，连击等
        public ushort Minjie { get; set; }
        [XmlAttribute("魔力")]//加层闪避和命中，战斗移动范围，加层战斗条速度
        public ushort Moli { get; set; }

        [XmlAttribute("魅力")]//加层法术类伤害，法术减伤和灵力值
        public ushort Meili { get; set; }
        [XmlAttribute("幸运")]
        public ushort Xingyun { get; set; }

        [XmlAttribute("炼金")]
        public ushort Lianjin { get; set; }
        [XmlAttribute("锻造")]
        public ushort Duanzao { get; set; }
        
    }
}

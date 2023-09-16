/****************************************************
    文件：SkillData.cs
	作者：昔莘
    邮箱: 304183153@qq.com
    日期：2022/7/11 11:43:3
	功能：技能配置表
*****************************************************/
using System.Xml.Serialization;

namespace GDServer {
    
    [System.Serializable]
    public class SkillData : ExcelBase {
        /// <summary>
        /// 初始化
        /// </summary>
        public override void Init() {
            base.Init();
            m_AllSkillDic.Clear();
            for (int i = 0; i < AllSkills.Count; i++) {
                if (m_AllSkillDic.ContainsKey(AllSkills[i].id)) {
                    Debuger.LogError($"id{AllSkills[i].id}已存在");
                } else {
                    m_AllSkillDic.Add(AllSkills[i].id, AllSkills[i]);
                }
            }
        }
        /// <summary>
        /// 根据id获取npc数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public SkillDataBase FindSkillByID(int id) {
            SkillDataBase skill;
            m_AllSkillDic.TryGetValue(id, out skill);
            return skill;
        }

        public override void Clear() {
            throw new System.NotImplementedException();
        }

        [XmlIgnore]//这个标签代表不需要序列化，dic序列化会报错
        public Dictionary<int, SkillDataBase> m_AllSkillDic = new Dictionary<int, SkillDataBase>();
        /// <summary>
        /// 这里不要new，否则反序列化时，可能会导致数据叠加
        /// AllNPC只会用在反序列化赋值
        /// </summary>
        [XmlElement("AllSkills")]
        public List<SkillDataBase> AllSkills { get; set; }
    }
    /// <summary>
    /// NPC的数据结构类
    /// </summary>
    [System.Serializable]
    public class SkillDataBase {
        //ID
        [XmlAttribute("ID")]
        public int id { get; set; }

        [XmlAttribute("技能名称")]
        public string name { get; set; }

        [XmlAttribute("技能描述")]
        public string desc { get; set; }

        [XmlAttribute("技能图标")]
        public string pic { get; set; }

        [XmlAttribute("技能类型")]
        public int skilltype { get; set; }//战士、法师、弓手、通用

        [XmlAttribute("伤害")]
        public int shanghai { get; set; }

        [XmlAttribute("技能升级值参数")]
        public ushort levelarg { get; set; }//技能升级参数，同时用于计算等级，其实就是n次方的底数，例如levelarg = 2,升级经验就是2的level次方

        [XmlAttribute("冷却")]
        public float cd { get; set; }
        [XmlAttribute("使用碰撞")]//对应enum SkillColliderType
        public int usecollider { get; set; }
        [XmlAttribute("攻击范围")]
        public float range { get; set; }
        [XmlAttribute("攻击角度")]
        public float angle { get; set; }
        [XmlAttribute("特效")]
        public string texiao { get; set; }
        [XmlAttribute("需求等级")]
        public ushort xuqiudengji { get; set; }
        [XmlAttribute("技能消耗")]
        public ushort xiaohao { get; set; }
        [XmlElement("事件list")]
        public List<skillevent> skilleventlist { get; set; }
    }
    
    [System.Serializable]
    public class skillevent {
        [XmlAttribute("事件ID")]
        public int id { get; set; }
        [XmlAttribute("事件类型")]
        public int eventtype { get; set; }

        [XmlAttribute("事件时间")]//动画触发时间的百分比，例如动画20%时触发事件
        public float eventtime { get; set; }
        [XmlAttribute("效果值")]//事件的效果，例如位移多少米，屏幕抖动幅度，特效参数等，可以放任意值，在代码中拓展识别即可
        public float eventeff { get; set; }
    }
}

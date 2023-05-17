/****************************************************
    文件：SkillData.cs
	作者：昔莘
    邮箱: 304183153@qq.com
    日期：2022/7/11 11:43:3
	功能：技能配置表
*****************************************************/
using GF.ConfigTable;
using System.Xml.Serialization;
using System.Collections.Generic;
using GF.Const;

namespace GF.MainGame.Data {
    
    [System.Serializable]
    public class SkillData : ExcelBase {
#if UNITY_EDITOR
        /// <summary>
        /// 编辑器调用
        /// </summary>
        public override void Construction() {
            //base.Construction();
            AllSkills = new List<SkillDataBase>();
            //初始化几个测试用的数据
            SkillDataBase t = new SkillDataBase();
            t.id = 1;
            t.name = "野蛮冲撞";
            t.desc = "战士冲锋技能，快速位移";
            t.pic = "UIRes/skill/chongzhuang.png";
            t.levelarg = 2;
            t.cd = 1.5f;
            t.skilltype = 0;
            t.shanghai = 100;
            t.xuqiudengji = 1;
            t.range = 80f;
            t.angle = 30f;
            t.skillefflist = new List<skilleff>();
            skilleff se = new skilleff();
            se.id = 1;
            se.yanchi = 0.2f;
            se.xiaoguo1 = "5";
            se.xiaoguo2 = "";
            se.efftype = (int)SkillEffType.weiyi;
            t.skillefflist.Add(se);
            t.texiao = "jianshi_chongzhuang";
            if (!AllSkills.Contains(t)) {
                AllSkills.Add(t);
            } else {
                Debuger.Log("数据已存在");
            }
            SkillDataBase t1 = new SkillDataBase();
            t1.id = 2;
            t1.name = "野蛮冲撞";
            t1.desc = "战士冲锋技能，快速位移";
            t1.pic = "UIRes/skill/chongzhuang.png";
            t1.levelarg = 2;
            t1.cd = 1.5f;
            t1.skilltype = 0;
            t1.shanghai = 100;
            t1.xuqiudengji = 1;
            t1.range  = 80f;
            t1.angle = 30f;
            t1.skillefflist = new List<skilleff>();
            skilleff se1 = new skilleff();
            se1.id = 2;
            se1.yanchi = 0.2f;
            se1.efftype =(int)SkillEffType.weiyi;
            se1.xiaoguo1 = "5";
            se1.xiaoguo2 = "";
            t1.skillefflist.Add(se1);
            t1.texiao = "jianshi_chongzhuang";
            t1.skillattlist = new List<skillatt>();
            skillatt satt = new skillatt();
            satt.id = 1;
            
            satt.xiaoguo3 = "";
            t1.skillattlist.Add(satt);
            if (!AllSkills.Contains(t1)) {
                AllSkills.Add(t1);
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
        public SkillDataBase FindNPCByID(int id) {
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
        [XmlAttribute("攻击范围")]
        public float range { get; set; }
        [XmlAttribute("攻击角度")]
        public float angle { get; set; }
        [XmlAttribute("特效")]
        public string texiao { get; set; }
        [XmlAttribute("需求等级")]
        public ushort xuqiudengji { get; set; }
        [XmlElement("效果list")]
        public List<skilleff> skillefflist { get; set; }//技能的特效，例如位移技能的特效就是位移，一个技能可以有多个特效，例如二段冲锋，就是两个位移特效，需要客户端根据数据处理
        [XmlElement("攻击点list")]
        public List<skillatt> skillattlist { get; set; }
    }
    [System.Serializable]
    public class skilleff {
        [XmlAttribute("效果ID")]
        public int id { get; set; }
        [XmlAttribute("延迟时间")]
        public float yanchi { get; set; }
        [XmlAttribute("效果类型")]
        public int efftype { get; set; }
        [XmlAttribute("效果值1")]//如果是冲锋技能，就是移动距离
        public string xiaoguo1 { get; set; }
        [XmlAttribute("效果值2")]
        public string xiaoguo2 { get; set; }
    }
    [System.Serializable]
    public class skillatt {
        [XmlAttribute("攻击ID")]
        public int id { get; set; }

       
        [XmlAttribute("效果值3")]//拓展备用，暂无用途图
        public string xiaoguo3 { get; set; }
    }
}

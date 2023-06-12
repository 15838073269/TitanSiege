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
            t.usecollider =(int)SkillColliderType.box;
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
            t1.usecollider = (int)SkillColliderType.box;
            t1.texiao = "jianshi_chongzhuang";
            t1.skilleventlist = new List<skillevent>();
            skillevent satt = new skillevent();
            satt.id = 1;
            satt.eventtype = (int)SkillEventType.attack;
            satt.eventtime = 0.1f;
            satt.eventeff = 5.0f;
            t1.skilleventlist.Add(satt);
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

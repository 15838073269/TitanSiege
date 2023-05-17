/****************************************************
    文件：NPCData.cs
	作者：昔莘
    邮箱: 304183153@qq.com
    日期：2021/10/28 21:22:50
	功能：Nothing
*****************************************************/
using GF.ConfigTable;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace GF.MainGame.Data {
        //数据表不能写命名空间内，否则，AppDomain.CurrentDomain.GetAssemblies()找不到它，因为它只能获取到运行时加载的程序集，MainGame.Data 运行时并不需要加载
        //目前通过写死命名空间的方式来实现了，可以写在命名空间内
        [System.Serializable]
    public class NPCData : ExcelBase {
#if UNITY_EDITOR
        /// <summary>
        /// 编辑器调用
        /// </summary>
        public override void Construction() {
            //base.Construction();
            AllNPC = new List<NPCDataBase>();
            //初始化几个测试用的数据
            NPCDataBase nb1 = new NPCDataBase();
            nb1.ID = 1;
            nb1.Name = "谢子良";
            nb1.Level = 1;
            nb1.Levelupid = 1;
            nb1.Zhiye = 0;
            nb1.Exp = 0;
            nb1.Shengming = 500;
            nb1.Fali = 100;
            nb1.Tizhi = 1;
            nb1.Liliang  = 8;
            nb1.Minjie = 8;
            nb1.Moli = 8;
            nb1.Meili = 99;
            nb1.Xingyun = 8;
            nb1.Lianjin = 50;
            nb1.Duanzao  = 50;
            nb1.Jinbi = 500;
            nb1.Zuanshi = 200;
            nb1.Chenghao = "无名菜鸟";
            nb1.Prefabpath = "NPCPrefab/Player.prefab";
            nb1.Headpath = "UIRes/head/12.png";
            nb1.Lihuipath = "UIRes/fight/fightlihui/1.png";
            nb1.Friends = new List<string>();
            nb1.Friends.Add("1");
            nb1.Friends.Add("2");
            nb1.Friends.Add("3");
            nb1.Skills = new List<int>();//技能添加的顺序，决定角色技能在
            //面板上的位置，第一个永远是普通攻击，第二个永远是第一个技能
            nb1.Skills.Add(1);
            nb1.Skills.Add(2);
            if (!AllNPC.Contains(nb1)) {
                AllNPC.Add(nb1);
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
            if (AllNPC == null) {
                AllNPC = new List<NPCDataBase>();
            }
            m_AllNPCBaseDic.Clear();
            for (ushort i = 0; i < AllNPC.Count; i++) {
                if (m_AllNPCBaseDic.ContainsKey(AllNPC[i].ID)) {
                    Debuger.LogError($"id{AllNPC[i].ID}已存在");
                } else {
                    m_AllNPCBaseDic.Add(AllNPC[i].ID, AllNPC[i]);
                }
            }
        }
        /// <summary>
        /// 根据id获取npc数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public NPCDataBase FindNPCByID(ushort id) {
            NPCDataBase npc;
            m_AllNPCBaseDic.TryGetValue(id, out npc);
            return npc;
        }

        public override void Clear() {
            throw new System.NotImplementedException();
        }

        [XmlIgnore]//这个标签代表不需要序列化，dic序列化会报错
        public Dictionary<ushort, NPCDataBase> m_AllNPCBaseDic = new Dictionary<ushort, NPCDataBase>();
        /// <summary>
        /// 这里不要new，否则反序列化时，可能会导致数据叠加
        /// AllNPC只会用在反序列化赋值
        /// </summary>
        [XmlElement("AllNPC")]
        public List<NPCDataBase> AllNPC { get; set; }
    }
    /// <summary>
    /// NPC的数据结构类
    /// </summary>
    [System.Serializable]
    public class NPCDataBase { 
        //iD
        [XmlAttribute("ID")]
        public ushort ID { get; set; }
        [XmlAttribute("名称")]
        public string Name {get;set;}
        [XmlAttribute("职业")]
        public ushort Zhiye { get; set; }
        [XmlAttribute("生命")]
        public ushort Shengming { get; set; }
        [XmlAttribute("法力")]
        public ushort Fali { get; set; }
        [XmlAttribute("等级")]
        public ushort Level { get; set; }
        [XmlAttribute("升级系数")]
        public ushort Levelupid { get; set; }
        [XmlAttribute("经验")]//当前经验，如果是怪物，就是怪物的所属经验
        public int Exp { get; set; }
        //攻击和防御是运行时数值，运行数值还有精力，闪避，闪避率，暴击，暴击率，抗暴率，连击率，命中，法术减伤，外功减伤，冰冻，灼烧，
        //命中大于闪避，闪避率随机数，攻击生效，伤害是：攻击（已包含属性加成）-防御（已包含属性加成）-法术减伤/外功减伤（很多武学兼备法术伤害和外功伤害，要分别减，减完之后再相加，就是伤害总值），小于<0为固定-1，（根据暴击率触发暴击，暴击数值/100,换算百分比，翻倍伤害-抗暴率*翻倍伤害）， = 最终伤害
        //所有角色运行时数值都一样，根据不同属性改变数值
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
        [XmlAttribute("钻石")]
        public int Zuanshi { get; set; }
        [XmlAttribute("金币")]
        public int Jinbi { get; set; }
        [XmlAttribute("称号")]
        public string Chenghao { get; set; }
        [XmlElement("亲朋")]
        public List<string> Friends { get; set; }//数据形式：1（角色id）|夫妻（关系）|20（好感）
        [XmlElement("技能")]//技能添加的顺序，决定角色技能在
                          //面板上的位置，第一个永远是普通攻击，第二个永远是第一个技能，以此类推
        public List<int> Skills { get; set; }//数据形式：1（id）

        [XmlAttribute("预制体")]
        public string Prefabpath { get; set; }
        [XmlAttribute("头像")]
        public string Headpath { get; set; }
        [XmlAttribute("立绘")]
        public string Lihuipath { get; set; }
    }
    

}

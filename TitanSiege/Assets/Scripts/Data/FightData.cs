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
    public class FightData : ExcelBase {
#if UNITY_EDITOR
        public override void Construction() {
            AllFight = new List<FightDataBase>();
            FightDataBase fight = new FightDataBase();
            fight.id = 1;
            fight.enemys = new List<ushort>();
            fight.enemys.Add(3);
            fight.enemys.Add(3);
            fight.enemys.Add(2);
            fight.npcs = new List<ushort>();
            fight.npcs.Add(3);
            AllFight.Add(fight);
        }
#endif

        /// <summary>
        /// 初始化
        /// </summary>
        public override void Init() {
            base.Init();
            if (AllFight == null) {
                AllFight = new List<FightDataBase>();
            }
            AllFightDataDic.Clear();
            for (int i = 0; i < AllFight.Count; i++) {
                AllFightDataDic.Add(AllFight[i].id, AllFight[i]);
            }
        }
        /// <summary>
        /// 根据ID查找buff
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public FightDataBase FinBuffById(ushort id) {
            FightDataBase fd;
            AllFightDataDic.TryGetValue(id, out fd);
            return fd;
        }

        public override void Clear() {
            throw new System.NotImplementedException();
        }

        [XmlIgnore]
        public Dictionary<ushort, FightDataBase> AllFightDataDic = new Dictionary<ushort, FightDataBase>();

        [XmlElement("AllFight")]
        public List<FightDataBase> AllFight { get; set; }
    }
    /// <summary>
    /// NPC的数据结构类
    /// </summary>
    [System.Serializable]
    public class FightDataBase { 
        [XmlAttribute("ID")]
        public ushort id { get; set; }
        [XmlElement("敌人")]
        public List<ushort> enemys { get; set; }
        [XmlElement("非玩家npc")]
        public List<ushort> npcs { get; set; }
    }
    
}

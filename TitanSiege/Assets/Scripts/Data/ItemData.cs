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
    public class ItemData : ExcelBase {
#if UNITY_EDITOR
        /// <summary>
        /// 编辑器调用
        /// </summary>
        public override void Construction() {
            //base.Construction();
            AllItems = new List<ItemBase>();
            //初始化几个测试用的数据
            ItemBase t = new ItemBase();
            t.id = 1;
            t.itemtype = 1;
            t.level = 1;
            t.name = "止血草";
            t.pic = "33/33/33";
            t.price = 200;
            t.cd = 0;
            t.desc = "止血药品！";
            t.talent = "";
            t.drop = true;
            t.xiaoguo1 = 0;
            t.xiaoguo1zhi = "hp,100";
            t.xiaoguo2 = 0;
            t.xiaoguo2zhi = "";
            t.xiaoguo3 = 0;
            t.xiaoguo3zhi = "";
            t.xuqiu1 = "";
            t.xuqiu1zhi = 0;
            t.xuqiu2 = "";
            t.xuqiu2zhi = 0;
            t.xuqiu3 = "";
            t.xuqiu3zhi = 0;
            if (!AllItems.Contains(t)) {
                AllItems.Add(t);
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
            m_AllItemBaseDic.Clear();
            for (int i = 0; i < AllItems.Count; i++) {
                if (m_AllItemBaseDic.ContainsKey(AllItems[i].id)) {
                    Debuger.LogError($"id{AllItems[i].id}已存在");
                } else {
                    m_AllItemBaseDic.Add(AllItems[i].id, AllItems[i]);
                }
            }
        }
        /// <summary>
        /// 根据id获取npc数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ItemBase FindNPCByID(int id) {
            ItemBase item;
            m_AllItemBaseDic.TryGetValue(id, out item);
            return item;
        }

        public override void Clear() {
            throw new System.NotImplementedException();
        }

        [XmlIgnore]//这个标签代表不需要序列化，dic序列化会报错
        public Dictionary<int, ItemBase> m_AllItemBaseDic = new Dictionary<int, ItemBase>();
        /// <summary>
        /// 这里不要new，否则反序列化时，可能会导致数据叠加
        /// AllNPC只会用在反序列化赋值
        /// </summary>
        [XmlElement("AllItems")]
        public List<ItemBase> AllItems { get; set; }
    }
    /// <summary>
    /// NPC的数据结构类
    /// </summary>
    [System.Serializable]
    public class ItemBase {
        //ID
        [XmlAttribute("ID")]
        public int id { get; set; }

        [XmlAttribute("物品名")]
        public string name { get; set; }

        [XmlAttribute("描述")]
        public string desc { get; set; }

        [XmlAttribute("图片")]
        public string pic { get; set; }

        [XmlAttribute("类型")]
        public ushort itemtype { get; set; }

        [XmlAttribute("物品等级")]
        public ushort level { get; set; }

        [XmlAttribute("贩卖价格")]
        public int price { get; set; }

        [XmlAttribute("是否掉落")]
        public bool drop { get; set; }

        [XmlAttribute("冷却")]
        public ushort cd { get; set; }

        [XmlAttribute("天赋")]
        public string talent { get; set; }

        [XmlAttribute("效果1")]
        public ushort xiaoguo1 { get; set; }

        [XmlAttribute("效果值1")]
        public string xiaoguo1zhi { get; set; }

        [XmlAttribute("效果2")]
        public ushort xiaoguo2 { get; set; }

        [XmlAttribute("效果值2")]
        public string xiaoguo2zhi { get; set; }

        [XmlAttribute("效果3")]
        public ushort xiaoguo3 { get; set; }

        [XmlAttribute("效果值3")]
        public string xiaoguo3zhi { get; set; }

        [XmlAttribute("需求1")]
        public string xuqiu1 { get; set; }

        [XmlAttribute("需求值1")]
        public ushort xuqiu1zhi { get; set; }

        [XmlAttribute("需求2")]
         public string xuqiu2 { get; set; }

        [XmlAttribute("需求值2")]
        public ushort xuqiu2zhi { get; set; }

        [XmlAttribute("需求3")]
        public string xuqiu3 { get; set; }

        [XmlAttribute("需求值3")]
        public ushort xuqiu3zhi { get; set; }

    }
    
}

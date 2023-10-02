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
            AllItems = new List<ItemDataBase>();
            //初始化几个测试用的数据
            ItemDataBase t = new ItemDataBase();
            t.id = 1;
            t.itemtype = 1;
            t.level = 1;
            t.pinzhi = (int)Pinzhi.bai;
            t.name = "止血草";
            t.pic = "33/33/33";
            t.price = 200;
            t.cd = 0;
            t.desc = "止血药品！";
            t.drop = true;
            t.xiaoguo1 = 1;
            t.xiaoguo1zhi = "100";
            t.xiaoguo2 = (int)XiaoGuo.addfightprop;
            t.xiaoguo2zhi = "maxhp|50,gongji|50,fangyu|10,baoji|0.01";
            t.xiaoguo3 = (int)XiaoGuo.addprop;
            t.xiaoguo3zhi = "lliang|5,tizhi|5,minjie|5";
            t.xuqiu1 = (int)XuQiu.fightprop;
            t.xuqiu1zhi = "gongji|50,fangyu|10";
            t.xuqiu2 = (int)XuQiu.prop; ;
            t.xuqiu2zhi = "lliang|5,tizhi|5,minjie|5";
            t.xuqiu3 = 0;
            t.xuqiu3zhi = "";
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
        public ItemDataBase FindItemByID(int id) {
            ItemDataBase item;
            m_AllItemBaseDic.TryGetValue(id, out item);
            return item;
        }

        public override void Clear() {
            throw new System.NotImplementedException();
        }

        [XmlIgnore]//这个标签代表不需要序列化，dic序列化会报错
        public Dictionary<int, ItemDataBase> m_AllItemBaseDic = new Dictionary<int, ItemDataBase>();
        /// <summary>
        /// 这里不要new，否则反序列化时，可能会导致数据叠加
        /// AllNPC只会用在反序列化赋值
        /// </summary>
        [XmlElement("AllItems")]
        public List<ItemDataBase> AllItems { get; set; }
    }
    /// <summary>
    /// NPC的数据结构类
    /// </summary>
    [System.Serializable]
    public class ItemDataBase {
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
        public int itemtype { get; set; }

        [XmlAttribute("物品等级")]
        public ushort level { get; set; }
        [XmlAttribute("品质")]
        public int pinzhi { get; set; }

        [XmlAttribute("贩卖价格")]
        public int price { get; set; }

        [XmlAttribute("是否掉落")]
        public bool drop { get; set; }

        [XmlAttribute("冷却")]
        public ushort cd { get; set; }

        [XmlAttribute("效果1")]
        public int xiaoguo1 { get; set; }

        [XmlAttribute("效果值1")]
        public string xiaoguo1zhi { get; set; }

        [XmlAttribute("效果2")]
        public int xiaoguo2 { get; set; }

        [XmlAttribute("效果值2")]
        public string xiaoguo2zhi { get; set; }

        [XmlAttribute("效果3")]
        public int xiaoguo3 { get; set; }

        [XmlAttribute("效果值3")]
        public string xiaoguo3zhi { get; set; }

        [XmlAttribute("需求1")]
        public int xuqiu1 { get; set; }

        [XmlAttribute("需求值1")]
        public string xuqiu1zhi { get; set; }

        [XmlAttribute("需求2")]
         public int xuqiu2 { get; set; }

        [XmlAttribute("需求值2")]
        public string xuqiu2zhi { get; set; }

        [XmlAttribute("需求3")]
        public int xuqiu3 { get; set; }

        [XmlAttribute("需求值3")]
        public string xuqiu3zhi { get; set; }

    }
    public enum XiaoGuo { 
        addhp=1,//回血
        addmp,//回蓝
        addfightprop,//增加战斗属性
        addprop,//增加常规属性
        addskill,//新增技能
        addjinbi,//添加金币
        addzhuanshi,//添加钻石
        additem,//添加物品，例如宝箱
        addexp,//增加经验
    }
    public enum ItemType { 
        yaopin=1,//药品
        yifu,//衣服装备
        kuzi,//裤子装备
        wuqi,//武器装备
        xianglian,//项链装备
        jiezi,//戒指装备
        xiezi,//鞋子装备
        jinengshu,//技能书
        renwu,//任务物品
        zawu,//杂物
    }
    public enum XuQiu {
        fightprop=1,//需要战斗属性
        prop,//需要基本属性
        level,//需要等级
        item,//需要某个物品
        zhiye,//需求某个职业
    }
    public enum Pinzhi { 
        bai = 1,//白,普通
        lv,//绿，精良
        lan,//蓝，卓越
        zi,//紫，传奇
        cheng,//橙，史诗
    }
    public enum ItemPos {
        inBag = 0,//在背包内
        inEqu,//在装备栏
        inSelect,//在装备选择栏上
    }
}

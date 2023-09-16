/****************************************************
    文件：NPCData.cs
	作者：昔莘
    邮箱: 304183153@qq.com
    日期：2021/10/28 21:22:50
	功能：Nothing
*****************************************************/
using System.Xml.Serialization;
using System.Collections.Generic;

namespace GDServer {
    //数据表不能写命名空间内，否则，AppDomain.CurrentDomain.GetAssemblies()找不到它，因为它只能获取到运行时加载的程序集，MainGame.Data 运行时并不需要加载
    //目前通过写死命名空间的方式来实现了，可以写在命名空间内
    [System.Serializable]
    public class TaskData : ExcelBase {

        /// <summary>
        /// 初始化
        /// </summary>
        public override void Init() {
            base.Init();
            m_AllTaskDataBaseDic.Clear();
            for (int i = 0; i < AllTasks.Count; i++) {
                if (m_AllTaskDataBaseDic.ContainsKey(AllTasks[i].id)) {
                    Debuger.LogError($"id{AllTasks[i].id}已存在");
                } else {
                    m_AllTaskDataBaseDic.Add(AllTasks[i].id, AllTasks[i]);
                }
            }
        }
        /// <summary>
        /// 根据id获取任务数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public TaskDataBase FindNPCByID(int id) {
            TaskDataBase item;
            m_AllTaskDataBaseDic.TryGetValue(id, out item);
            return item;
        }

        public override void Clear() {
            throw new System.NotImplementedException();
        }

        [XmlIgnore]//这个标签代表不需要序列化，dic序列化会报错
        public Dictionary<int, TaskDataBase> m_AllTaskDataBaseDic = new Dictionary<int, TaskDataBase>();
        /// <summary>
        /// 这里不要new，否则反序列化时，可能会导致数据叠加
        /// AllTasks只会用在反序列化赋值
        /// </summary>
        [XmlElement("AllTasks")]
        public List<TaskDataBase> AllTasks { get; set; }
    }
    /// <summary>
    /// 任务的数据结构类
    /// 怎么监控任务目标完成进度，例如杀掉5只狼
    /// 每次战斗结算时，记录一下敌方信息，再调用一下现有任务的完成条件，核对一下id，
    /// 如果有敌方信息中有目标id，就处理计算数量处理完成度
    /// </summary>
    [System.Serializable]
    public class TaskDataBase {
        //ID
        [XmlAttribute("ID")]
        public int id { get; set; }

        [XmlAttribute("剧本名")]
        public string sname { get; set; }
        [XmlAttribute("子任务名称")]//-为没有子名称
        public string zname { get; set; }
        [XmlAttribute("简述")]
        public string jianshu { get; set; }
      
        [XmlAttribute("详细描述")]
        public string miaoshu { get; set; }
        [XmlAttribute("任务角色id")]//任务触发的角色或者触发器id，设定如下相同//0代表没有触发npc，系统根据上一环任务自动触发
        public ushort fromid { get; set; }
        [XmlAttribute("任务类型")]
        public int tasktype { get; set; }
        [XmlAttribute("任务触发条件类型")]//任务触发的条件的条件类型
        public int tiaojiantype { get; set; }
        [XmlAttribute("任务触发条件内容")]//任务触发的条件的条件内容,内容形式见条件类型定义
        public string tiaojian { get; set; }
        [XmlAttribute("任务触发条件值")]//任务触发的条件的条件的值,内容形式见条件类型定义
        public string tiaojianzhi { get; set; }
        [XmlAttribute("任务完成条件类型")]//任务触发的条件的条件类型
        public int wanchengtype { get; set; }
        [XmlAttribute("任务完成条件内容")]//任务触发的条件的条件内容,内容形式见条件类型定义
        public string wancheng { get; set; }
        [XmlAttribute("任务完成条件值")]//任务触发的条件的条件的值,内容形式见条件类型定义
        public string wanchengzhi { get; set; }
        [XmlAttribute("目标角色id")]//目标可能是个位置，也可能是一个npc,npc好处理，这里就是角色id，发生对话时，
        //目标角色直接判断是否有任务存在且任务目标id为自己即可，如果是位置，就需要自定义触发器，并且规定一套和npcid不重复的触发器id，
        //这里规定，触发器的id从10000开始，如果id大于10000，就证明目标是一个任务触发器（位置）
        //因为不具备寻路功能，所以，无论是角色还是位置，都不需要记录具体的所在场景和坐标，只需要在描述中说明位置，让玩家自行寻找，找到后，由角色和触发器自行判断是否触发自己
        public ushort toid { get; set; }
        [XmlAttribute("目标角色数量")]//如杀3头狼
        public ushort tonum { get; set; }
        [XmlAttribute("后续剧情")]//单个直接写id，多种分支以#分割，例如1#2#34#224，按照顺序对应选择跳转的分支
        public string nextid { get; set; }
        [XmlAttribute("开始时间")]//格式：182#1#13  年#月#日，-代表永久存在
        public string begintime { get; set; }
        [XmlAttribute("结束时间")]//格式：182#1#13  年#月#日，-代表永久存在
        public string overtime { get; set; }
        [XmlAttribute("是否循环")]//是否循环，0代表不循环，1，2，3代表次数，99代表一直重复
        public ushort isloop { get; set; }
        [XmlAttribute("对话id")]//触发任务时对话内容和到达目标时的对话内容,按照#分割
        public string talkid { get; set; }
        [XmlAttribute("背景音乐")]//背景音乐名称，默认为-，就是没有音乐
        public string bgmusic { get; set; }

        [XmlElement("奖励列表")]
        public List<TaskJl> jianglilst { get; set; }
        
        [XmlAttribute("战斗id")]//预设，0为无战斗，其他为战斗数据的id，单独开一个战斗配置表，配置每场战斗的基本数据，例如敌方信息，人员，回合限制等
        public ushort zhandouid { get; set; }
    }
    [System.Serializable]
    public class TaskJl {
        [XmlAttribute("任务奖励类型")]
        public int jianglitype { get; set; }
        [XmlAttribute("任务奖励内容")]//任务触发的条件的条件内容,内容形式见条件类型定义
        public string jiangli { get; set; }
        [XmlAttribute("任务奖励值")]//奖励值，可以为负数
        public string jianglizhi { get; set; }
    }
   

}

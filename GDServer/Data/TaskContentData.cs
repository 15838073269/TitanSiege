/****************************************************
    文件：NPCData.cs
	作者：昔莘
    邮箱: 304183153@qq.com
    日期：2021/10/28 21:22:50
	功能：Nothing
*****************************************************/
using System.Xml.Serialization;

namespace GDServer {
    //数据表不能写命名空间内，否则，AppDomain.CurrentDomain.GetAssemblies()找不到它，因为它只能获取到运行时加载的程序集，MainGame.Data 运行时并不需要加载
    //目前通过写死命名空间的方式来实现了，可以写在命名空间内
    [System.Serializable]
    public class TaskContentData : ExcelBase {
        /// <summary>
        /// 初始化
        /// </summary>
        public override void Init() {
            base.Init();
            m_AllTaskContentDataBaseDic.Clear();
            for (int i = 0; i < AllTaskContents.Count; i++) {
                if (m_AllTaskContentDataBaseDic.ContainsKey(AllTaskContents[i].id)) {
                    Debuger.LogError($"id{AllTaskContents[i].id}已存在");
                } else {
                    m_AllTaskContentDataBaseDic.Add(AllTaskContents[i].id, AllTaskContents[i]);
                }
            }
        }
        /// <summary>
        /// 根据id获取任务数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public TaskContentDataBase FindNPCByID(int id) {
            TaskContentDataBase item;
            m_AllTaskContentDataBaseDic.TryGetValue(id, out item);
            return item;
        }

        public override void Clear() {
            throw new System.NotImplementedException();
        }

        [XmlIgnore]//这个标签代表不需要序列化，dic序列化会报错
        public Dictionary<int, TaskContentDataBase> m_AllTaskContentDataBaseDic = new Dictionary<int, TaskContentDataBase>();
        /// <summary>
        /// 这里不要new，否则反序列化时，可能会导致数据叠加
        /// AllTasks只会用在反序列化赋值
        /// </summary>
        [XmlElement("AllTasks")]
        public List<TaskContentDataBase> AllTaskContents { get; set; }
    }
    /// <summary>
    /// NPC的数据结构类
    /// </summary>
    [System.Serializable]
    public class TaskContentDataBase {

        [XmlAttribute("ID")]//对话id
        public int id { get; set; }

        [XmlElement("对话内容")]
        public List<TaskLine> linelist { get; set; }

    }
    public class TaskLine {
        [XmlAttribute("对话类型")]//内容的类型，枚举
        public int linetype { get; set; }
        [XmlAttribute("对话角色id")]//说话角色id
        public ushort uid { get; set; }
        
        [XmlAttribute("对话内容")]//一句话的内容
        public string line { get; set; }
    }
    
}

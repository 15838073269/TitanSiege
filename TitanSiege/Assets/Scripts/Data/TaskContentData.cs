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
using GF.Const;

namespace GF.MainGame.Data {
    //数据表不能写命名空间内，否则，AppDomain.CurrentDomain.GetAssemblies()找不到它，因为它只能获取到运行时加载的程序集，MainGame.Data 运行时并不需要加载
    //目前通过写死命名空间的方式来实现了，可以写在命名空间内
    [System.Serializable]
    public class TaskContentData : ExcelBase {
#if UNITY_EDITOR
        /// <summary>
        /// 编辑器调用
        /// </summary>
        public override void Construction() {
            //base.Construction();
            AllTaskContents = new List<TaskContentDataBase>();
            //初始化几个测试用的数据
            TaskContentDataBase t = new TaskContentDataBase();
            t.id = 1;
            t.linelist = new List<TaskLine>();
            TaskLine tl = new TaskLine();
            tl.uid = 1;
            tl.linetype = 0;
            tl.line = "算了，这些事情我也管不了，还是回去找小蝶吧！";
            t.linelist.Add(tl);
            TaskLine tl1 = new TaskLine();
            tl1.uid = 0;
            tl1.linetype = (int)LineType.Notice;
            tl1.line = "回到小蝶的家，发现小蝶不知去了哪里，你只能干巴巴的等待，然而直到深夜，小蝶也没有回来，你这才意识到，小蝶可能出事了。";
            t.linelist.Add(tl1);
            TaskLine tl2 = new TaskLine();
            tl2.uid = 1;
            tl2.linetype = (int)LineType.Log;
            tl2.line = "肯定出事，不行，我得赶快去找小蝶！";
            t.linelist.Add(tl2);
            if (!AllTaskContents.Contains(t)) {
                AllTaskContents.Add(t);
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

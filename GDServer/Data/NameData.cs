/****************************************************
    文件：CreateName.cs
	作者：昔莘
    邮箱: 304183153@qq.com
    日期：2022/2/11 15:31:42
	功能：名称的配置表
*****************************************************/
using GDServer;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace GDServer {
    //数据表不能写命名空间内，否则，AppDomain.CurrentDomain.GetAssemblies()找不到它，因为它只能获取到运行时加载的程序集，MainGame.Data 运行时并不需要加载
    //目前通过写死命名空间的方式来实现了，可以写在命名空间内
    [System.Serializable]
    public class NameData : ExcelBase {
        /// <summary>
        /// 初始化
        /// </summary>
        public override void Init() {
            base.Init();
            m_AllNameBaseDic.Clear();
            for (int i = 0; i < AllNames.Count; i++) {
                if (m_AllNameBaseDic.ContainsKey(AllNames[i].id)) {
                    Debuger.LogError($"id{AllNames[i].id}已存在");
                } else {
                    m_AllNameBaseDic.Add(AllNames[i].id, AllNames[i]);
                }
            }
        }
        /// <summary>
        /// 根据id获取npc数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public NameDataBase FindNameByID(ushort id) {
            NameDataBase name;
            m_AllNameBaseDic.TryGetValue(id, out name);
            return name;
        }

        public override void Clear() {
            m_AllNameBaseDic.Clear();
            AllNames.Clear();
        }

        [XmlIgnore]//这个标签代表不需要序列化，dic序列化会报错
        public Dictionary<ushort, NameDataBase> m_AllNameBaseDic = new Dictionary<ushort, NameDataBase>();
        /// <summary>
        /// 这里不要new，否则反序列化时，可能会导致数据叠加
        /// AllNPC只会用在反序列化赋值
        /// </summary>
        [XmlElement("AllNames")]
        public List<NameDataBase> AllNames { get; set; }
    }
    /// <summary>
    /// NPC的数据结构类
    /// </summary>
    [System.Serializable]
    public class NameDataBase {
        //ID
        [XmlAttribute("ID")]
        public ushort id { get; set; }

        [XmlAttribute("中文名称")]
        public string Cname { get; set; }

        [XmlAttribute("英文名称")]
        public string Ename { get; set; }


    }

}

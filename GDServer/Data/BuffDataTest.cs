
using System.Collections.Generic;
using System.Xml.Serialization;

namespace GDServer {
    //数据表不能写命名空间内，否则，AppDomain.CurrentDomain.GetAssemblies()找不到它，因为它只能获取到运行时加载的程序集，MainGame.Data 运行时并不需要加载
    //目前通过写死命名空间的方式来实现了，可以写在命名空间内
    [System.Serializable]
    public class BuffDataTest : ExcelBase {
        public override void Init() {
            AllBuffDic.Clear();
            for (int i = 0; i < AllBuffList.Count; i++) {
                AllBuffDic.Add(AllBuffList[i].Id, AllBuffList[i]);
            }
        }

        /// <summary>
        /// 根据ID查找buff
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public BuffBase FinBuffById(int id) {
            return AllBuffDic[id];
        }

        public override void Clear() {
            throw new System.NotImplementedException();
        }

        [XmlIgnore]
        public Dictionary<int, BuffBase> AllBuffDic = new Dictionary<int, BuffBase>();

        [XmlElement("AllBuffList")]
        public List<BuffBase> AllBuffList { get; set; }

        [XmlElement("MonsterBuffList")]
        public List<BuffBase> MonsterBuffList { get; set; }
    }

    public enum BuffEnum {
        None = 0,
        Ranshao = 1,
        Bingdong = 2,
        Du = 3,
    }

    [System.Serializable]
    public class BuffBase {
        [XmlAttribute("Id")]
        public int Id { get; set; }

        [XmlAttribute("Name")]
        public string Name { get; set; }

        [XmlAttribute("OutLook")]
        public string OutLook { get; set; }

        [XmlAttribute("Time")]
        public float Time { get; set; }

        [XmlAttribute("BuffType")]
        public BuffEnum BuffType { get; set; }

        [XmlElement("AllString")]
        public List<string> AllString { get; set; }

        [XmlElement("AllBuffList")]
        public List<BuffTest> AllBuffList { get; set; }
    }

    [System.Serializable]
    public class BuffTest {
        [XmlAttribute("Id")]
        public int Id { get; set; }

        [XmlAttribute("Name")]
        public string Name { get; set; }
    }
}


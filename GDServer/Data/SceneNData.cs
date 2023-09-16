/****************************************************
    文件：SceneNData.cs
	作者：昔莘
    邮箱: 304183153@qq.com
    日期：2022/12/8 11:55:52
	功能：这个脚本仅用来导出数据，不是实际
*****************************************************/
using System.Xml.Serialization;
using System.Collections.Generic;

namespace GDServer {

    [System.Serializable]
    public class SceneNData : ExcelBase {

        /// <summary>
        /// 初始化
        /// </summary>
        public override void Init() {
            base.Init();
            
        }
        /// <summary>
        /// 根据id获取npc数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public SceneDataBase FindNPCByID(int id) {
            return null;
        }

        public override void Clear() {
            
        }

        
        /// <summary>
        /// 这里不要new，否则反序列化时，可能会导致数据叠加
        /// AllNPC只会用在反序列化赋值
        /// </summary>
        [XmlElement("AllNDatas")]
        public List<NData> AllNDatas { get; set; }
    }
    /// <summary>
    /// NPC的数据结构类
    /// </summary>
    
    [System.Serializable]
    public class NData {
        [XmlAttribute("NDID")]
        public int ndid { get; set; }
        [XmlAttribute("预制体路径")]
        public string prepath { get; set; }
        [XmlAttribute("PosX")]
        public float posx { get; set; }
        [XmlAttribute("PosY")]
        public float posy { get; set; }
        [XmlAttribute("PosZ")]
        public float posz { get; set; }
        [XmlAttribute("RotaX")]
        public float rotax { get; set; }
        [XmlAttribute("RotaY")]
        public float rotay { get; set; }
        [XmlAttribute("RotaZ")]
        public float rotaz { get; set; }
        [XmlAttribute("ScalX")]
        public float scalx { get; set; }
        [XmlAttribute("ScalY")]
        public float scaly { get; set; }
        [XmlAttribute("ScalZ")]
        public float scalz { get; set; }
        [XmlElement("绑定任务")]
        public List<int> renwulist { get; set; }
    }
}
/****************************************************
    文件：AssetBundleConfig.cs
	作者：昔莘
    邮箱: 304183153@qq.com
    日期：2021/8/12 10:54:3
	功能：ab包配置表的数据结构，需要序列化
*****************************************************/

using System.Xml.Serialization;
using System.Collections.Generic;
namespace GF.Unity.AB {
    /// <summary>
    /// AB包配置表的基础类
    /// </summary>
    [System.Serializable]
    public class AssetBundleConfig {
        //存储所有Ab包的list
        [XmlElement("ABList")]
        public List<ABBase> ABList { get; set; }
    }
    /// <summary>
    /// 配置表数据元类
    /// </summary>
    [System.Serializable]
    public class ABBase {
        //ab包名称
        [XmlAttribute("ABName")]
        public string ABname { get; set; }
        //Crc是用来代表path路径的唯一值，path路径是string类型，对性能有影响，少用
        [XmlAttribute("Crc")]
        public uint Crc { set; get; }
        //ab包的路径
        [XmlAttribute("Path")]
        public string Path { set; get; }
        //ab包包含的资源
        [XmlAttribute("AssetName")]
        public string AssetName { set; get; }
        //当前ab包依赖的ab包数组
        [XmlElement("ABDepends")]
        public List<string> ABDepends { set; get; }
    }
}

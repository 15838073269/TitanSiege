/****************************************************
    文件：ExcelBase.cs
	作者：昔莘
    邮箱: 304183153@qq.com
    日期：2021/10/27 17:11:37
	功能：配置表的基类,需要注意，配置表类并不是存储数据的，而是对数据的操作,具体的数据结构由其他类
*****************************************************/
namespace GDServer {
    
    [System.Serializable]
    public abstract class ExcelBase {
        public string m_Name;
        /// <summary>
        /// 初始化
        /// </summary>
        public virtual void Init() {
            m_Name = this.GetType().Name;
        }
        public abstract void Clear();
    }
}


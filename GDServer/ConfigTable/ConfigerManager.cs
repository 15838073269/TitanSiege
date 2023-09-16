/****************************************************
    文件：BinarySerialize.cs
	作者：昔莘
    邮箱: 304183153@qq.com
    日期：2021/10/27 17:11:37
	功能：配置表管理类
*****************************************************/

namespace GDServer {
    public class ConfigerManager : Singleton<ConfigerManager> {
        public static string BinaryPath = AppDomain.CurrentDomain.BaseDirectory +"\\Data\\binary\\";
        public static string XmlPath = AppDomain.CurrentDomain.BaseDirectory + "\\Data\\xml\\";
        /// <summary>
        /// 储存所有已经加载的配置表
        /// 配置表一般都不会释放
        /// path --  配置表
        /// </summary>
        public Dictionary<string, ExcelBase> m_AllExcelData = new Dictionary<string, ExcelBase>();

        /// <summary>
        /// 加载数据表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path">二进制文件路径，后面如果变更probuffer形式，就换成probuffer文件路径</param>
        /// <returns></returns>
        public T LoadData<T>(string path) where T : ExcelBase {
            if (string.IsNullOrEmpty(path)) {
                return null;
            }
            string newpath = BinaryPath + path;
            if (!File.Exists(newpath)) {//没有二进制文件，就先转一下
                BinarySerializeOpt.XmlToBinary(newpath);
            }
            if (m_AllExcelData.ContainsKey(newpath)) {
                Debuger.LogError("重复加载相同配置文件" + newpath);
                return m_AllExcelData[newpath] as T;
            }

            T data = BinarySerializeOpt.BinaryDeserilize<T>(newpath);
            if (data != null) {
                data.Init();
                m_AllExcelData.Add(newpath, data);
                Debuger.Log($"配置表{data.m_Name}加载成功！");
            }
            return data;
        }
        /// <summary>
        /// 加载完后，按照数据类型定义一些常用的数据变量
        /// 方便使用
        /// </summary>
        //public static NPCData m_NPCData;
        //public static SkillData m_SkillData;
        public void InitData() {
            //获取角色数据
            //m_NPCData = ConfigerManager.GetInstance.FindData<NPCData>(CT.TABLE_NPC);
            //if (m_NPCData == null || m_NPCData.m_AllNPCBaseDic.Count == 0) {
            //    Debuger.LogError("加载角色数据为空，请检查！");
            //}
            //m_SkillData = ConfigerManager.GetInstance.FindData<SkillData>(CT.TABLE_SKILL);
            //if (m_SkillData == null || m_SkillData.m_AllSkillDic.Count == 0) {
            //    Debuger.LogError("加载技能数据为空，请检查！");
            //}
        }

        /// <summary>
        /// 根据路径查找数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path">二进制文件路径</param>
        /// <returns></returns>
        public T FindData<T>(string path) where T : ExcelBase {
            if (string.IsNullOrEmpty(path))
                return null;

            ExcelBase excelBase = null;
            string newpath = BinaryPath + path;
            if (m_AllExcelData.TryGetValue(newpath, out excelBase)) {
                return excelBase as T;
            } else {
                //没有数据就去加载
                excelBase = LoadData<T>(newpath);
            }

            return (T)excelBase;
        }
        /// <summary>
        /// 根据路径删除配置表
        /// </summary>
        /// <param name="path"></param>
        public void DelData(string path) {
            if (string.IsNullOrEmpty(path))
                return;
            string newpath = BinaryPath + path;
            if (m_AllExcelData.ContainsKey(newpath)) {
                m_AllExcelData[newpath].Clear();
                m_AllExcelData[newpath] = null;
                m_AllExcelData.Remove(newpath);
            } 
        }
        
    }
    /// <summary>
    /// 所有配置表路径
    /// </summary>
    public class CT {
        //配置表路径
        public const string TABLE_NPC ="NPCData.bytes";
        public const string TABLE_NAME = "NameData.bytes";
        public const string TABLE_SKILL = "SkillData.bytes";
        public const string TABLE_LEVEL = "LevelUpData.bytes";
        // public const string TABLE_BUFF = "data/binary/BuffData.bytes";
    }

}


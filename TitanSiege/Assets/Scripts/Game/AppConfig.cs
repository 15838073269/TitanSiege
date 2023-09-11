/****************************************************
    文件：AssetBundleConfig.cs
	作者：昔莘
    邮箱: 304183153@qq.com
    日期：2021/8/12 10:54:3
	功能：项目的配置表，所有常量配置都在这里
*****************************************************/
using UnityEngine;
using GF.Utils;
using GF.Codec;
namespace GF.MainGame {
    /// <summary>
    /// 整个游戏的配置文件
    /// </summary>
   
    public class AppConfig {
        public static bool IsOnline = true;//是否联网，本游戏可以单机玩，也可以联网玩，联网后会先根据日期同步本地数据信息
        public const ushort TeamNum = 5;//玩家队伍的最大人数
        public const ushort ScriptSpan = 1000;//每个manager管理的消息数量（脚本数量），数值根据情况来定，小型项目1000足够 
        /// <summary>
        /// 是否从ab包加载资源，false是从编辑器直接加载，用于开发时期。true是从Ab包加载，用于开发后期，测试及打包。
        /// </summary>
        public const bool LoadFormAssetBundle = false;

        /// <summary>
        /// 是否是编辑器下运行，这里主要是给dll中的代码使用的，因为代码打包成dll后，调用无法识别是否在编辑器环境#if UNITY_EDITOR 的方式失效了
        /// </summary>
        public const bool IsEditor = true;
        /// <summary>
        /// 异步加载时最大卡着连续异步加载的时间，单位是微秒，默认20万微妙
        /// </summary>
        public const long MAXMAXLOADINGTIME = 200000;
        /// <summary>
        /// 自定义的高中低配置制定m_NoRefrenceMapList的大小
        /// </summary>
        public const ushort MAXMAPLISTSIZE = 300;
        /// <summary>
        /// AssetBundleConfig包所在的路径,目前用于开发时AB包打包路径，生产时，需要替换成对应路径
        /// </summary>
        public readonly  static string ABConfigPath = Application.streamingAssetsPath+"/data";
        /// <summary>
        /// AB包所在路径，默认streamingAsset,实际打包需要修改成可读写路径
        /// </summary>
        public readonly static string ABPathDic = Application.streamingAssetsPath;

        /// <summary>
        /// 资源前置路径，这里是为了简化开发使用的。
        /// 资源完整的路径是：Assets/Art/audio/skill/fight/Fashu_1.wav 每次加载都需要写全路径，很烦，所以将Assets/Art/提取出来
        /// </summary>
        public const string ResPrePath = "Assets/Art/";
        /// <summary>
        /// 场景加载UI预制体的名称
        /// </summary>
        public const string SceneLoading = "LoadingUIPage";
        public const string LoginUIPage = "LoginUIPage";
        public const string CreatePlayerPage = "CreatePlayerPage";
        public const string SelectPlayerPage = "SelectPlayerPage";
        public const string UpdateResource = "UpdateResource";
        public const string UIMsgBox = "UIMsgBox";
        public const string UIMsgTips = "UIMsgTips";
        public const string MainUIPage = "MainUIPage";
        public const string JoyUIWidget = "JoyUIWidget";
        public const string SkillUIWidget = "SkillUIWidget";
        public const string SkillUIInfo = "SkillUIInfo";
        public const string MapUIPage = "MapUIPage";
        public const string HPUIBG = "HPUIBG";
        public const string DamageUIPath = "UIPrefab/DamgeUIWidget.prefab";
        public const string DieUIWindow = "DieUIWindow";
        public const string InfoUIWindow = "InfoUIWindow";
        /// <summary>
        /// 各个场景的名称
        /// </summary>
        public const string MainScene = "tiankongcheng";
        public const string EmptyScene = "empty";
        public const string MovieScene = "movie";
        public const string LoginScene = "Login";
        public const string RoleScene = "CreateRole";
        /// <summary>
        /// 玩家和怪物的基础移动速度
        /// </summary>
        public const float PlayerSpeed = 4f;
        public const float MonsterSpeed = 4f;
        
        /// <summary>
        /// 战斗后脱离战斗状态的时间
        /// </summary>
        public const float FightReset = 10f;
        /// <summary>
        /// 怪物放弃追击，警戒脱离的范围
        /// </summary>
        public const float WarnRange = 10f;
        /// <summary>
        /// 怪物的攻击范围，正常应该在技能里判断，先简单这么处理吧
        /// </summary>
        public const float AttackRange = 1.5f;
        /// <summary>
        /// xml配置表存储路径
        /// </summary>
        public const string XmlPath = "Assets/Art/data/xml/";
        /// <summary>
        /// 二进制配置表存储路径
        /// </summary>
        public const string BinaryPath = "Assets/Art/data/binary/";
        /// <summary>
        /// 脚本配置表存储路径
        /// </summary>
        public const string ScriptsPath = "Assets/Scripts/Data/";
        /// <summary>
        /// 脚本配置表存储路径
        /// </summary>
        public readonly static string ExcelPath = $"{Application.dataPath}/../Data/Excel/";
        /// <summary>
        ///reg配置表存储路径
        /// </summary>
        public readonly static string RegPath = $"{Application.dataPath}/../Data/Reg/";
        /// <summary>
        /// 用户数据，通过ProtoBuf序列后发送到服务器，和服务器的对比
        /// </summary>
#if UNITY_EDITOR
        public readonly static string Path = Application.persistentDataPath + "/AppConfig_Editor.data";
#else
        public readonly static string Path = Application.persistentDataPath + "/AppConfig.data";
#endif
        private static Config m_Value;
        public static Config Value {
            get {
                if (m_Value == null) {
                    m_Value = new Config();
                }
                return m_Value; 
            }
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public static void Init() {
            Debuger.Log($"AppConfig", "Init() Path= {Path}");
            //byte[] data = FileUtils.ReadFile(Path);//从配置文件中读取
            //if (data != null && data.Length > 0) {
            //    //通过protobuf反序列化data的数据，
            //    Config cfg = (Config)PBSerializer.NDeserialize(data, typeof(Config));
            //    if (cfg != null) {
            //        m_Value = cfg;
            //    }
            //}
        }
        //public static void Save() {
        //    if (m_Value != null) {
        //        byte[] data = PBSerializer.NSerialize(m_Value);
        //        FileUtils.SaveFile(Path, data);
        //    }
        //}
    }
 
    public class Config {
       // [ProtoMember(1)] public UserData mainUserData = new UserData();
        public bool enableBgMusic = true;//是否打开背景音乐
        public bool enableSoundEffect = true;//是否打开音效
    }
    /// <summary>
    /// 所有配置表路径
    /// </summary>
    public class CT {
        //配置表路径
        public const string TABLE_NPC = "data/binary/NPCData.bytes";
        public const string TABLE_NAME = "data/binary/NameData.bytes";
        public const string TABLE_SKILL = "data/binary/SkillData.bytes";
        public const string TABLE_LEVEL = "data/binary/LevelUpData.bytes";
        // public const string TABLE_BUFF = "data/binary/BuffData.bytes";
    }
}

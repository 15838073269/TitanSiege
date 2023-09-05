namespace GF.MainGame {
    public class ModuleDef {
        public const string NameSpace = "GF.MainGame.Module";
        public const string NativeAssemblyName = "Assembly-CSharp";//不需要热更新的模块
        public const string ScriptAssemblyName = "ILRScript";//需要热更新的模块

    }
    //每个manager的id，需要手动配置。msgbase中通过managerid来调用对应的manager
    public enum MDef {
        LoginModule = 0,
        CreateAndSelectModule = AppConfig.ScriptSpan * 1,
        NPCModule = AppConfig.ScriptSpan * 2,
        SkillModule = AppConfig.ScriptSpan * 3,
        FightModule = AppConfig.ScriptSpan * 4,
        AudiosModule = AppConfig.ScriptSpan * 5,
        VersionModule = AppConfig.ScriptSpan * 6,
        MoveModule = AppConfig.ScriptSpan * 7,
        BulidModule = AppConfig.ScriptSpan * 8,
        SceneModule = AppConfig.ScriptSpan * 9,
        TeamQueueModule = AppConfig.ScriptSpan *10,
        MainUIModule = AppConfig.ScriptSpan * 11,
        StateModule = AppConfig.ScriptSpan * 12,//调度全部角色 包括玩家的动画
        HPModule = AppConfig.ScriptSpan * 13,
        DieUIModule = AppConfig.ScriptSpan * 14,
    }
}
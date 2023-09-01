/****************************************************
    文件：Enums.cs
	作者：昔莘
    邮箱: 304183153@qq.com
    日期：2022/11/4 14:15:55
	功能：项目中所有的枚举，方便管理  理论上，这里的数据应该都放到服务器公用类中，以后再优化吧
*****************************************************/

namespace GF.Const {
    public enum SkillColliderType { 
        none = 0,
        box = 1,
        line,
        circle,
    }
    /// <summary>
    /// 攻击技能的事件类型
    /// </summary>
    public enum SkillEventType { 
        none=0,
        playaudio=1,
        showeff=2,
        hiddeneff=3,
        attack=4,
        weiyi=5,//位移为0就是施放技能时不能移动，为数值就是可以冲锋移动多少米，如果允许技能施法时移动，可以不设置weiyi，系统没有检测到位移就会允许释放技能的同时移动，一般适用于法师
        shake=6,//屏幕震动
        stopweiyi=7,//停止技能移动，一般用于提前停止
        addheight = 8,//位置加高，用于跳跃技能
    }
    /// <summary>
    /// 关系枚举
    /// </summary>
    public enum guanxi {
        父亲 = 0,
        母亲 = 1,
        夫妻 = 2,
        子女 = 3,
        朋友 = 4,
        师父 = 5,
        徒弟 = 6,
        仇敌 = 7,
    }
    ///// <summary>
    ///// 职业枚举  放到服务器公用类中了，因为服务器也要用
    ///// </summary>
    //public enum Zhiye {
    //    剑士 = 0,
    //    法师 = 1,
    //    游侠 = 2,
    //}
    /// <summary>
    /// 技能种类枚举
    /// </summary>
    public enum SkillType {
        none = 0,
        wuli = 1,
        fashu = 2,
        gongjian = 3,
    }
    public enum SkillEffType { 
        none = 0,
        weiyi = 1,

    }
    /// <summary>
    /// 技能范围枚举
    /// </summary>
    public enum SkillRange {
        dian = 0,
        mian,
        xian,
        mi,
        jing,
        hui,
    }
    /// <summary>
    /// 技能品级枚举
    /// </summary>
    public enum SkillPinji {
        shen = 0,
        tian,
        di,
        xuan,
        huang,
    }
   
    /// <summary>
    /// 任务对话枚举
    /// </summary>
    public enum LineType {
        Log = 0,//正常对话
        Img = 1,//显示图片，对应line是图片名称，根据名称从固定路径中寻找
        Notice = 2,//提示，或者旁白
        Select = 3,//单选，对应line以#分割对应的选项，最多四个，超过四个，只显示前四个
    }
    /// <summary>
    /// 任务参数枚举
    /// </summary>
    public enum TaskArg { //这个枚举是任务触发条件和奖励内容共用的
        none = 0,//无条件
        item = 1,//道具，对应道具id及数量
        shuxing = 2,//属性，对应要求的属性名称的字符串和增加数值，可正可负
        wuxue = 3,//武学,对应武学id，
        menpai = 4,//门派，对应的门派id
        qianzhi = 5,//完成前置任务，对应前置任务id，从角色信息中查询
        money = 6,
        exp = 7,
    }
    /// <summary>
    /// 任务完成条件枚举
    /// </summary>
    public enum Wancheng {
        none = 0,//无条件，就是普通对话
        fight = 1,//战斗，无论成败,对应战斗id
        win = 2,//战斗，且战斗胜利，对应战斗id
        shaguai = 3,//杀怪，根据目标id确定怪物类型，数量值，每次战斗结算时，统计数量，可以在战斗控制器中添加一个或者多个委托，如果委托不为空，就调用，由任务模块注册
        caiji = 4,//采集，根据目标id确定怪物类型，数量值，每次采集结算时，统计数量，可以在采集控制器中添加一个或者多个委托，如果委托不为空，就调用，由任务模块注册
    }
    /// <summary>
    /// 任务类型枚举
    /// </summary>
    public enum TaskType {
        zhuxian = 0,//主线
        zhixian = 1,//支线
        xunhuan = 2,//循环
        qiyu = 3,//奇遇任务
        menpai = 4,//门派任务

    }
    /// <summary>
    /// 玩家使用的动画状态
    /// </summary>
    public enum AniState { 
        none,
        idle,
        wait,
        run,
        fightrun,
        attack,
        die,
        hurt,
    }
    public enum DamageType {
        none =0,//正常伤害
        shangbi,
        baoji,
    }
    public enum NpcType {
        player,
        monster,
        npc,
    }
}
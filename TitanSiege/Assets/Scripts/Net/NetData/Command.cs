
public class Command : Net.Component.Command {
    public const byte Skill = 150;
    public const byte EnemyPatrol = 151;
    public const byte AIBeAttack = 152;
    public const byte Resurrection = 153;//复活
    public const byte PlayerState = 154;
    public const byte EnemyAttack = 155;
    public const byte EnemyUpdateProp = 156;//怪物属性同步给客户端，一般是第一次创建怪物或者怪物属性发生变化时使用
    public const byte PlayerUpdateProp = 157;//玩家重新计算属性的命令，为了节省带宽，不传递具体属性，只传递升级更新命令
    public const byte addrexp = 158;//吃经验书
    public const byte AddHpOrMp = 159;//回血或者回蓝
    public const byte CurrentTalk = 160;//当前场景的聊天
}

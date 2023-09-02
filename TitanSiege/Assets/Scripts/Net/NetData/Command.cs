
public class Command : Net.Component.Command {
    public const byte Skill = 150;
    public const byte EnemyPatrol = 151;
    public const byte AIBeAttack = 152;
    public const byte Resurrection = 153;//复活
    public const byte PlayerState = 154;
    public const byte EnemyAttack = 155;
    public const byte EnemyUpdateProp = 156;//怪物属性同步给客户端，一般是第一次创建怪物或者怪物属性发生变化时使用
    //public const byte PlayerUpdateProp = 157;//玩家属性同步给客户端，一般是第一次创建怪物或者怪物属性发生变化时使用
}

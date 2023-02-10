using Net.Component;
//GDCommand.cs文件最好与客户端Unity项目的GDCommand.cs同名且代码一样
namespace GDServer {
    public class Command: Net.Component.Command {
        public const byte Skill = 150;
        public const byte AIMonster = 151;
        public const byte AIBeAttack = 152;//应该不需要，因为技能会处理怪物被攻击
        public const byte Resurrection = 153;//复活
        public const byte PlayerState = 154;
        public const byte AttackPlayer = 155;
    }
}

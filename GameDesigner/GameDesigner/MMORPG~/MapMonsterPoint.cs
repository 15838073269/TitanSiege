using System;

namespace Net.MMORPG
{
    /// <summary>
    /// 地图怪物点
    /// </summary>
    [Serializable]
    public class MapMonsterPoint
    {
        /// <summary>
        /// 所有怪物数据
        /// </summary>
        public MonsterData[] monsters;
        /// <summary>
        /// 怪物巡逻点
        /// </summary>
        public PatrolPath patrolPath;
    }
}

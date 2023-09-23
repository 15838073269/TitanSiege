using System;

namespace Net.MMORPG
{
    /// <summary>
    /// 怪物数据
    /// </summary>
    [Serializable]
    public class MonsterData
    {
        /// <summary>
        /// 怪物索引
        /// </summary>
        public int id;
        /// <summary>
        /// 怪物生命值
        /// </summary>
        public int health;
        /// <summary>
        /// 可以赋值更多信息
        /// </summary>
        public string json;
    }
}
using System;

namespace Net.MMORPG
{
    /// <summary>
    /// 怪物数据，这里只需要把数据库表中怪物的id传递过去即可
    /// </summary>
    [Serializable]
    public class MonsterData
    {
        /// <summary>
        /// 怪物的索引id
        /// </summary>
        public int id;
        /// <summary>
        /// 怪物在数据库中npcs表的id
        /// </summary>
        public int mysqlid;
        /// <summary>
        /// 可以赋值更多信息
        /// </summary>
        public string json;
    }
}
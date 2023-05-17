using Net;
using System;
using System.Collections.Generic;

namespace Net.MMORPG
{
    /// <summary>
    /// 巡逻路径点
    /// </summary>
    [Serializable]
    public class PatrolPath
    {
        /// <summary>
        /// 所有巡逻路径点
        /// </summary>
        public List<Vector3> waypoints = new List<Vector3>();
    }
}
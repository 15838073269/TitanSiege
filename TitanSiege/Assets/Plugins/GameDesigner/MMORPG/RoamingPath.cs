using UnityEngine;
using System.Collections.Generic;

namespace Net.MMORPG
{
    /// <summary>
    /// 巡逻路径点组件
    /// </summary>
    public class RoamingPath : MonoBehaviour
    {
        public List<Vector3> localWaypoints = new List<Vector3>();
        public List<Vector3> waypointsList = new List<Vector3>();
        public bool waypointsFoldout; //编辑器用到, 反射使用
    }
}
/****************************************************
    文件：RoamingPath.cs
	作者：昔莘
    邮箱: 304183153@qq.com
    日期：2022/12/28 23:12:39
	功能：Nothing
*****************************************************/

using System.Collections.Generic;
using UnityEngine;
namespace GF.MainGame.Module.NPC {
    public class RoamingPath : MonoBehaviour {
        public List<Vector3> localWaypoints = new List<Vector3>();
        public List<Vector3> waypointsList = new List<Vector3>();
        public bool waypointsFoldout;
    }
}
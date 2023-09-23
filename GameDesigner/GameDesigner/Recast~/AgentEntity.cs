using System;
using Net.Common;
using System.Collections.Generic;
#if RECAST_NATIVE
using Net.AI.Native;
using static Net.AI.Native.RecastDll;
#else
using Recast;
using static Recast.RecastGlobal;
#endif

namespace Net.AI
{
    [Serializable]
    public class AgentEntity
    {
        public float speed = 5f;
        public float agentHeight = 1f;
        public float angularSpeed = 0.25f;
        public TransformEntity transform = new TransformEntity();
        private readonly List<Vector3> pathPoints = new List<Vector3>();
        public FindPathMode findPathMode;
        public dtStraightPathOptions m_straightPathOptions = dtStraightPathOptions.DT_STRAIGHTPATH_ALL_CROSSINGS;
        private NavmeshSystem navmeshSystem;
        public NavmeshSystem System { get => navmeshSystem; set => navmeshSystem = value; }

        public float RemainingDistance
        {
            get
            {
                if (pathPoints.Count > 0)
                    return Vector3.Distance(transform.Position, pathPoints[0]);
                return 0f;
            }
        }

        public Quaternion Rotation
        {
            get => transform.Rotation;
        }

        public AgentEntity() { }

        public AgentEntity(NavmeshSystem navmeshSystem)
        {
            this.navmeshSystem = navmeshSystem;
        }

        public void SetPositionAndRotation(Vector3 position, Quaternion rotation)
        {
            transform.Position = position;
            transform.Rotation = rotation;
        }

        public void OnUpdate(float dt)
        {
            if (pathPoints.Count > 0)
            {
                int index = pathPoints.Count - 1;
                var nextPos = pathPoints[index];
                transform.Rotation = Quaternion.Lerp(transform.Rotation, Quaternion.LookRotation(new Vector3(nextPos.x, transform.Position.y, nextPos.z) - transform.Position, Vector3.up), angularSpeed);
                transform.Position = Vector3.MoveTowards(transform.Position, nextPos, speed * dt);
                if (Vector3.Distance(transform.Position, nextPos) < 0.1f)
                    pathPoints.RemoveAt(index);
            }
        }

        public bool SetDestination(Vector3 target)
        {
            navmeshSystem.GetPath(transform.Position, target, pathPoints, agentHeight, findPathMode, m_straightPathOptions);
            pathPoints.Reverse(); //反转是因为后面每一步会进行移除, 移除时数组不会进行倒塌操作
            return pathPoints.Count > 0;
        }

        public void OnDrawGizmos(Action<Vector3, Vector3> onDrawLine)
        {
            if (pathPoints.Count > 0)
            {
                int index = pathPoints.Count - 1;
                onDrawLine(transform.Position, pathPoints[index]);
                for (int i = index; i > 0; i--)
                    onDrawLine(pathPoints[i], pathPoints[i - 1]);
            }
        }
    }
}
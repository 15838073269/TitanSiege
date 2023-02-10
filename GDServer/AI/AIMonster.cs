using ECS;
using Net;
using Net.Component;
using Net.Share;

namespace GDServer.AI {
    public class AIMonster : Component, IUpdate {
        internal float m_RoundMin = 50f;
        internal float m_RoundMax = 100f;
        internal NTransform transform;
        internal RoamingPath1 roamingPath;
        internal GDScene scene;
        internal byte state = 0;//是否有服务器同步，0为服务器同步，1为客户端同步
        internal byte state1 = 0;
        private float idleTime = 30f;
        internal int pointIndex=-1;
        internal int lastpointindex;
        public float walkSpeed = 3f;
        internal int id;
        internal int mid;
        internal bool isDeath = false;
        internal int health = 100;
        internal int targetID;//攻击的玩家id,也是同步怪物的客户端id

        public void OnUpdate() {
            if (isDeath)
                return;
            switch (state) {
                case 0:
                    Patrol();
                    break;
                case 1:
                    Authorize();
                    break;
            }
        }

        void Authorize() {
            scene.AddOperation(new Operation(Command.EnemySync, id, transform.position, transform.rotation) {
                cmd1 = state,
                cmd2 = state1,
                index1 = health,
                index2 = targetID,
                buffer = new byte[] { (byte)mid }
            });
            if (targetID == 0) {
                state = 0;
                state1 = 0;
            }
        }

        void Patrol() {
            switch (state1) {
                case 0:
                    if (Time.time > idleTime) {
                        state1 = (byte)RandomHelper.Range(0, 2);
                        idleTime = Time.time + RandomHelper.Range(m_RoundMin, m_RoundMax);
                    }
                    break;
                case 1:
                    if (pointIndex == -1) {
                        pointIndex = RandomHelper.Range(0, roamingPath.waypointsList.Count);
                    }
                    if (lastpointindex != pointIndex) {
                        transform.LookAt(roamingPath.waypointsList[pointIndex]);
                        transform.Translate(0, 0, walkSpeed * Time.deltaTime);
                        float dis = Vector3.Distance(transform.position, roamingPath.waypointsList[pointIndex]);
                        if (dis < 0.1f) {
                            lastpointindex = pointIndex;
                            pointIndex = RandomHelper.Range(0, roamingPath.waypointsList.Count);
                            state1 = 0;
                            idleTime = Time.time + RandomHelper.Range(m_RoundMin, m_RoundMax);
                        }
                    } else {
                        pointIndex = RandomHelper.Range(0, roamingPath.waypointsList.Count);
                        state1 = 0;
                        idleTime = Time.time + RandomHelper.Range(m_RoundMin, m_RoundMax);
                    }
                    break;
            }
            PatrolCall();
        }

        internal void PatrolCall() {
            scene.AddOperation(new Operation(Command.AIMonster, id, transform.position, transform.rotation) {
                cmd1 = state,
                cmd2 = state1,
                index1 = health,
                buffer = new byte[] { (byte)mid }
            });
        }

        internal void OnDamage(int damage) {
            if (isDeath)
                return;
            health -= damage;
            if (health <= 0) {
                isDeath = true;
                health = 0;
                state1 = 4;
                scene.Event.AddEvent(10f, () => {
                    health = 100;
                    isDeath = false;
                    state = 0;
                    state1 = 0;
                    targetID = 0;
                });
            } else {
                state = 1;
            }
        }
    }

}

using ECS;
using Net;
using Net.Component;
using Net.MMORPG;
using Net.Share;
using Net.System;
using System.Threading;
using Titansiege;

namespace GDServer.AI {
    public class AIMonster : Component, IUpdate {
        internal float m_RoundMin = 50f;
        internal float m_RoundMax = 100f;
        internal NTransform transform;
        internal PatrolPath patrolPath;
        internal GDScene scene;
        internal byte state = 0;//是否有服务器同步，0为服务器同步，1为客户端同步
        internal byte patrolstate = 0;//客户端的状态值
        private float idleTime = 30f;
        internal int pointIndex=-1;
        private int lastpointindex = 0;//上一个路点，这个参数主要巡逻时，尽可能不要重复路点
        public float walkSpeed = 3f;
        internal int enemyindex;//客户端怪物的模型索引
        internal int identity;
        internal bool isDeath = false;
        internal int targetID;//攻击的玩家id,也是同步怪物的客户端id
        public NpcsData? current;//当前登录角色
        public FightProp FP = new FightProp();//战斗属性

        public void OnUpdate() {
            if (isDeath)
                return;
            switch (state) {
                case 0:
                    Patrol();
                    break;
                case 1:
                    Authorize();//客户端更新
                    break;
            }
        }
        //某个客户端更新怪物数据，服务端向其他客户端广播
        void Authorize() {
            scene.AddOperation(new Operation(Command.EnemySync, identity, transform.position, transform.rotation) {
                cmd1 = state,
                cmd2 = patrolstate,
                index = enemyindex,//客户端怪物的模型索引，clientManager中的那个,客户端CheckMonster方法会使用这个
                index1 = FP.FightHP,
                index2 = targetID,
            });
            if (targetID == 0) {//没有攻击目标就调整到巡逻状态
                state = 0;
                patrolstate = 0;
            }
        }
        /// <summary>
        /// 服务器巡逻的方法
        /// </summary>
        void Patrol() {
            switch (patrolstate) {
                case 0:
                    if (Time.time > idleTime) {
                        patrolstate = (byte)RandomHelper.Range(0, 2);
                        idleTime = Time.time + RandomHelper.Range(m_RoundMin, m_RoundMax);
                    }
                    break;
                case 1:
                    if (pointIndex == -1) {
                        pointIndex = RandomHelper.Range(0, patrolPath.waypoints.Count);
                    }
                    if (lastpointindex != pointIndex) {
                        float dis = Vector3.Distance(transform.position, patrolPath.waypoints[pointIndex]);
                        if (dis < 0.1f) {
                            lastpointindex = pointIndex;
                            pointIndex = RandomHelper.Range(0, patrolPath.waypoints.Count);
                            patrolstate = 0;
                            idleTime = Time.time + RandomHelper.Range(m_RoundMin, m_RoundMax);
                        }
                        transform.LookAt(patrolPath.waypoints[pointIndex]);
                        transform.Translate(0, 0, walkSpeed * Time.deltaTime);
                    } else {
                        pointIndex = RandomHelper.Range(0, patrolPath.waypoints.Count);
                        patrolstate = 0;
                        idleTime = Time.time + RandomHelper.Range(m_RoundMin, m_RoundMax);
                    }
                    break;
            }
            PatrolCall();
        }

        internal void PatrolCall() {
            scene.AddOperation(new Operation(Command.EnemyPatrol, identity, transform.position, transform.rotation) {
                cmd1 = state,
                cmd2 = patrolstate,
                index = enemyindex,
                index1 = FP.FightHP,
            });
        }

        internal void OnDamage(int damage) {
            if (isDeath)
                return;
            FP.FightHP -= damage;
            if (FP.FightHP <= 0) {
                isDeath = true;
                FP.FightHP = 0;
                Debuger.Log("怪物"+identity+"死亡");
                patrolstate = 5;//客户端怪物的死亡状态值
                ThreadManager.Event.AddEvent(10f, () => {
                    Resurrection();
                });
                PatrolCall();//发送怪物死亡命令
            } else {
                state = 1;
            }
        }
        private void Resurrection() { //怪物复活
            FP.FightHP = current.Shengming + current.Tizhi * 10;
            isDeath= false;
            state = 0;
            patrolstate = 0;
            targetID = 0;
        }

        /// <summary>
        /// 根据属性写入战斗属性
        /// 暂时还未加入道具影响，例如装备，需道具模块开发完成后，再完善
        /// </summary>
        public virtual void UpdateFightProps() {
            float jcDodge = FP.BaseDodge;//基础闪避率，各职业角色和怪物不同
            float jcCrit = FP.BaseCrit;//基础暴击率，各职业角色和怪物不同
            switch (current?.Zhiye) {
                case (int)Zhiye.剑士:
                    FP.Attack = current.Liliang * 10;
                    FP.Defense = current.Liliang * 3 + current.Tizhi * 7;
                    break;
                case (int)Zhiye.法师:
                    FP.Attack = current.Moli * 10;
                    FP.Defense = current.Moli * 4 + current.Tizhi * 6;
                    jcDodge = 0.02f;
                    break;
                case (int)Zhiye.游侠:
                    FP.Attack = current.Minjie * 10;
                    FP.Defense = current.Minjie * 4 + current.Tizhi * 6;
                    jcDodge = 0.03f;
                    break;
                default:
                    break;
            }
            //闪避,基础闪避率0.01f;
            FP.Dodge = jcDodge + (float)current.Minjie / 1000f >= 0.3f ? 0.3f : (float)current.Minjie / 1000f;//属性加成的闪避
            FP.Crit = jcCrit + (float)current.Xingyun * jcCrit >= 0.5f ? 0.5f : (float)current.Xingyun * jcCrit;//暴击率
            FP.FightHP = current.Shengming + current.Tizhi * 10;
            FP.FightMagic = current.Fali + current.Moli * 10;
        }
    }
    
}

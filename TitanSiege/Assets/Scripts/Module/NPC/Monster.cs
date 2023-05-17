/****************************************************
    文件：Monster.cs
	作者：昔莘
    邮箱: 304183153@qq.com
    日期：2022/12/8 11:13:29
	功能：Nothing
*****************************************************/

using Cysharp.Threading.Tasks;
using GameDesigner;
using GF.ConfigTable;
using GF.MainGame.Data;
using Net.Client;
using Net.Component;
using Net.Share;
using Pathfinding.RVO;
using System;  
using UnityEngine;
using Random = System.Random;

namespace GF.MainGame.Module.NPC {
    public class Monster : NPCBase {
        //private Vector3 m_Pos;//怪物的原始位置，用于战斗返回
        //private Vector3 m_Rota;//怪物的原始旋转，用于战斗返回
        //private Vector3 m_Scal;//怪物的原始缩放，用于放大技能后的恢复
        public Material m_Material = null;
        public NPCBase m_target = null;//默认null，是服务器同步，有攻击目标就为本地同步
        public int m_NetState;//和服务器monster对应的state，0为服务端更新，1为客户端更新
        public int m_PatrolState;//服务端巡逻状态字段
        public int m_targetID;
        public RVOController m_RvoController;

        public void OnEnable() {
            if (m_Material == null) {
                return;
            }
            //重置溶解特效
            m_Material.SetFloat("_Cutoff",1);
            //倒放溶解，显示怪物模型
           _ = ShowModel();
        }
        
        /// <summary>
        /// 溶解显示
        /// </summary>
        /// <returns></returns>
        public async UniTask ShowModel() {
            float _cut = 1f;
            while (_cut >= 0) {
                _cut -= 0.1f;
                await UniTask.Delay(TimeSpan.FromSeconds(0.0618));
                m_Material.SetFloat("_Cutoff", _cut);
            }
        }
        /// <summary>
        /// 溶解消失
        /// </summary>
        /// <returns></returns>
        public async UniTask HideModel() {
            float _cut = 0f;
            while (_cut <= 1) {
                _cut += 0.1f;
                await UniTask.Delay(TimeSpan.FromSeconds(0.0618));
                m_Material.SetFloat("_Cutoff", _cut);
            }
        }
        public override void InitNPCAnimaor() {
            //发现一个很奇怪的bug，awake时，获取子物体的某个组件会报一次错误，仅一次，也不影响使用
            //因为怪物生成是服务器按针同步的，本地场景未跳转时，就发送服务器跳场景了，所以导致先创建了一次，后跳的场景，跳场景清除物体，导致物体丢失，该问题已修复
            var render = transform.Find("Body").GetComponent<SkinnedMeshRenderer>();
            m_Material = render.material;
            m_Nab = transform.GetComponent<MonsterAnimatorBase>();
            if (m_Nab == null) {
                m_Nab = transform.gameObject.AddComponent<MonsterAnimatorBase>();
                m_Nab.Init();
            }
            if (m_HPRoot == null) {
                m_HPRoot = transform.Find("HPRoot");
            }
            if (m_RvoController == null) {
                m_RvoController = transform.GetComponent<RVOController>();
            }
        }
        public override void UpdateFightProps() {
            base.UpdateFightProps();
            AppTools.Send<NPCBase>((int)HPEvent.CreateHPUI, this);
        }
        public void Update() {
            if ((m_target != null) && (m_targetID == ClientBase.Instance.UID)) { //如果攻击目标存在，并且就是本机，那就需要负责同步怪物的移动和旋转
                if (NetworkTime.CanSent) {//update的次数由机器配置决定，不确定数量，所以要加上限制，一般不超过30次，NetworkTime.CanSent就是控制发送次数的
                    ClientBase.Instance.AddOperation(new Operation(Command.EnemySync, m_GDID, transform.position, transform.rotation));
                }
            } else if ((m_NetState == 1) && (m_targetID == ClientBase.Instance.UID)&&(m_target == null)) { //如果当前是客户端同步，并且是本机在同步，但怪物的目标已经为null了，一般是脱离攻击范围，就发送消息给服务端，转为服务端控制
                if (NetworkTime.CanSent) {
                    ClientBase.Instance.AddOperation(new Operation(Command.EnemySwitchState, m_GDID) { cmd1 = 0,cmd2 = 0});

                }
            }
        }
        public override void LateUpdate() {
            //if (!isPlaySkill && (Time.time > time)) {//优化以下，减少移动时频繁变更状态机的情况,其实就是三帧相同才转为idle
            //    newPosition = transform.position;
            //    if (newPosition != oldPosition) {
            //        if (Vector3.Distance(oldPosition, newPosition) > 0.02f) {
            //            if (m_State.stateMachine.currState.ID != m_AllStateID["run"]) {
            //                m_State.StatusEntry(m_AllStateID["run"]);
            //            }
            //        } else {
            //            if (oldoldPosition == newPosition) {
            //                if (m_State.stateMachine.currState.ID == m_AllStateID["run"]) {
            //                    m_State.StatusEntry(m_AllStateID["idle"]);
            //                }
            //            }
            //        }

            //    } else {
            //        if (oldoldPosition == newPosition) {
            //            if (m_State.stateMachine.currState.ID == m_AllStateID["run"]) {

            //                m_State.StatusEntry(m_AllStateID["idle"]);
            //            }
            //        }
            //    }
            //    oldoldPosition = oldPosition;
            //    oldPosition = newPosition;
            //    time = Time.time + (1f / 50f);
            //}
        }

        internal void StatusEntry() {//根据服务端转换本地状态
            switch (m_PatrolState) {
                case 0://服务端的0是idle
                    m_State.StatusEntry(m_AllStateID["idle"]);
                    break;
                case 1://服务端的1是移动
                    m_State.StatusEntry(m_AllStateID["run"]);
                    break;
                default:
                    break;
            }
        }
    }
    #region GDNet的状态机
    public class MIdleState : StateBehaviour {
        private Monster m_Self;
        private int m_SendCount;//限制短时间内的数据发送次数，避免重复发送
        public override void OnInit() {
            m_Self = transform.GetComponent<Monster>();
        }
        public override void OnEnter() {
            m_SendCount = 0;
        }
        public override void OnUpdate() {
            if (m_Self.m_target==null) {//没有目标，就让服务器执行
                return;
            }
            if (m_Self.m_target.m_GDID!=ClientBase.Instance.UID) {//目标对象不是本机，也不执行，谁是怪物攻击目标，就由谁来计算
                return;
            }
            if (m_SendCount>0) {//同一时间如果有数据发送，就不再发送
                return;
            }
            //判断目前为止和怪物的距离，然后决定攻击、追击、放弃
            m_Self.transform.LookAt(new Vector3(m_Self.m_target.transform.position.x, m_Self.transform.position.y, m_Self.m_target.transform.position.z));
            float dis = Vector3.Distance( m_Self.transform.position, m_Self.m_target.transform.position );
            byte stateid = 0;
            if (dis > AppConfig.WarnRange) {//放弃
                //返回原位并切换成idle
                stateid = (byte)m_Self.m_AllStateID["idle"];
            } else if (dis <= AppConfig.AttackRange) { //攻击范围内
                //切换攻击状态,使用技能可以自定义各队列计算一下，或者随机
                //一般boss什么时候释放技能需要自定义，否则就随机即可
                //这里暂时没考虑怪物技能CD的问题
                int i = UnityEngine.Random.Range(1, 10);
                string sname = "skill1";
                if (i <= 6) {//普通攻击
                    stateid = (byte)m_Self.m_AllStateID[sname];
                } else { //攻击或者释放技能
                    if (m_Self.m_SkillId.Count == 0) {
                        return;
                    }
                    if (m_Self.m_SkillId.Count == 1) {
                        sname = "skill1";
                    } else {
                        sname = "skill2";
                    }
                    //if (m_Self.m_SkillId.Count == 1) {
                    //    sname = "skill1";
                    //} else {
                    //    int j = UnityEngine.Random.Range(2, m_Self.m_SkillId.Count + 1);//int随机数不覆盖最大值，所以要+1
                    //    sname = "skill" + j;
                    //}
                    stateid = (byte)m_Self.m_AllStateID[sname];
                }
            } else if (dis > AppConfig.AttackRange && dis <= AppConfig.WarnRange) { //追击
                //切换奔跑状态
                stateid = (byte)m_Self.m_AllStateID["run"];
            }
            //cmd1是客户端控制标志，1代表开始客户端同步，0代表服务端控制，cmd2才是怪物的客户端状态
            ClientBase.Instance.AddOperation(new Operation(Command.EnemySwitchState, m_Self.m_GDID) { cmd1=1, cmd2 = stateid }) ;
            m_SendCount++;
        }
    }
    public class MRunState : StateBehaviour {
        private Monster m_Self;
        private int m_SendCount;//限制短时间内的数据发送次数，避免重复发送
        public override void OnInit() {
            m_Self = transform.GetComponent<Monster>();
        }
        public override void OnEnter() {
            m_SendCount = 0;
        }
        public override void OnUpdate() {
            if (m_Self.m_target == null) {//没有目标，就让服务器执行
                return;
            }
            if (m_Self.m_target.m_GDID != ClientBase.Instance.UID) {//目标对象不是本机，也不执行，谁是怪物攻击目标，就由谁来计算
                return;
            }
            if (m_SendCount > 0) {//同一时间如果有数据发送，就不再发送
                return;
            }
            //判断目前为止和怪物的距离，然后决定攻击、追击、放弃
            m_Self.transform.LookAt(new Vector3(m_Self.m_target.transform.position.x, m_Self.transform.position.y, m_Self.m_target.transform.position.z));
            float dis = Vector3.Distance(m_Self.transform.position, m_Self.m_target.transform.position);
            byte stateid = 0;
            if (dis > AppConfig.WarnRange) {//放弃
                //返回原位并切换成idle todo
                stateid = (byte)m_Self.m_AllStateID["idle"];
            } else if (dis <= AppConfig.AttackRange) { //攻击范围内
                //切换攻击状态,使用技能可以自定义各队列计算一下，或者随机
                //一般boss什么时候释放技能需要自定义，否则就随机即可
                //这里暂时没考虑怪物技能CD的问题
                int i = UnityEngine.Random.Range(1, 10);
                string sname = "skill1";
                if (i <= 6) {//普通攻击
                    ClientBase.Instance.AddOperation(new Operation(Command.EnemySwitchState, m_Self.m_GDID) { cmd1 = 1, cmd2 = (byte)m_Self.m_AllStateID[sname] });
                    m_SendCount++;
                } else { //攻击或者释放技能
                    if (m_Self.m_SkillId.Count == 0) {
                        return;
                    }
                    if (m_Self.m_SkillId.Count == 1) {
                        sname = "skill1";
                    } else {
                        sname = "skill2";
                    }
                    //if (m_Self.m_SkillId.Count == 1) {
                    //    sname = "skill1";
                    //} else {
                    //    int j = UnityEngine.Random.Range(1, m_Self.m_SkillId.Count + 1);//int随机数不覆盖最大值，所以要+1
                    //    sname = "skill" + j;
                    //}
                    ClientBase.Instance.AddOperation(new Operation(Command.EnemySwitchState, m_Self.m_GDID) { cmd1 = 1, cmd2 = (byte)m_Self.m_AllStateID[sname] });
                    m_SendCount++;
                }
            } else if (dis > AppConfig.AttackRange && dis <= AppConfig.WarnRange) { //追击
                //每桢向目标移动
                ////Vector3 dir = m_Self.m_target.transform.position - m_Self.transform.position;
                ////Vector3 randomdir = new Vector3(dir.x + RandomHelper.Range(0f,2f), dir.y, dir.z + RandomHelper.Range(0f,2f)) ;//每一次添加随机的向量，防止怪物完全重叠
                //m_Self.transform.LookAt(m_Self.m_target.transform.position);
                //m_Self.transform.position += (m_Self.transform.forward * Time.deltaTime * AppConfig.MonsterSpeed);
                var targetPoint = m_Self.transform.position + m_Self.transform.forward * 100;
                m_Self.m_RvoController.SetTarget(targetPoint, AppConfig.MonsterSpeed, AppConfig.MonsterSpeed+2f);
                var delta = m_Self.m_RvoController.CalculateMovementDelta(m_Self.transform.position, Time.deltaTime);
                m_Self.transform.position = transform.position + delta;
            }
        }
    }
    public class MAttackState : ActionCore {
        private Monster m_Self;
        private int m_SkillID;
        private SkillDataBase m_SData;
        public override void OnInit() {
            m_Self = transform.GetComponent<Monster>();
            m_SkillID = m_Self.m_SkillId[0];
            m_SData = ConfigerManager.m_SkillData.FindNPCByID(m_SkillID);
        }
        /// <summary>
        /// 动画帧事件处理方法
        /// </summary>
        /// <param name="action"></param>
        public override void OnAnimationEvent(StateAction action) {
            if (!m_Self.m_IsDie) {
                m_Self.transform.LookAt(m_Self.m_target.transform.position);//攻击前，先转向
                //gd的状态机已经调用了动作，不用再写攻击部分了
                //直接在动作中触发播放特效就行 PlayEffect(string effname) 
                _ = AppTools.SendReturn<SkillDataBase, NPCBase, int>((int)SkillEvent.CountSkillHurt, m_SData, m_Self);
                
            }
            if (m_Self.m_target!=null) { //每次攻击完就判断一下，如果存在攻击状态，并且攻击目标已经死亡。重新回去巡逻
                if (m_Self.m_target.m_IsDie == true) {
                    m_Self.m_target = null;
                }
            }
        }
    }
    public class MDieState : StateBehaviour {
        private Monster m_Self;
        public override void OnInit() {
            m_Self = transform.GetComponent<Monster>();
        }
        public override void OnEnter() {
            m_Self.m_target = null;
        }
        public override void OnExit() {
            m_Self.m_target = null;
        }
    }
    public class MHurtState : StateBehaviour {

    }
    public class MSkill1State : StateBehaviour {
        private Monster m_Self;
        private int m_SkillID;
        SkillDataBase sd;
        public override void OnInit() {
            m_Self = transform.GetComponent<Monster>();
            m_SkillID = m_Self.m_SkillId[0];
            sd = ConfigerManager.m_SkillData.FindNPCByID(m_SkillID);
        }
        public override void OnEnter() {
            //if (m_Self.IsFight == false) {
            //    m_Self.SetFight(true);
            //    m_Self.ChangeWp();//切换以下武器
            //}
            m_Self.IsFight = false;
            m_Self.m_Resetidletime = AppConfig.FightReset;
            //m_Self.m_Nab.Attack(sd);
            AppTools.SendReturn<SkillDataBase, NPCBase, int>((int)SkillEvent.CountSkillHurt, sd, m_Self);//发送消息让技能模块计算伤害
            if (m_Self.m_target != null) { //每次攻击完就判断一下，如果存在攻击状态，并且攻击目标已经死亡。重新回去巡逻
                if (m_Self.m_target.m_IsDie == true) {
                    m_Self.m_target = null;
                }
            }
        }

        public override void OnExit() {
            m_Self.isPlaySkill = false;
            m_Self.m_Nab.HiddenEffect();
        }
    }
    public class MSkill2State : StateBehaviour {
        private Monster m_Self;
        private int m_SkillID;
        SkillDataBase sd;
        public override void OnInit() {
            m_Self = transform.GetComponent<Monster>();
            if (m_Self.m_SkillId.Count>1) {
                m_SkillID = m_Self.m_SkillId[1];
                sd = ConfigerManager.m_SkillData.FindNPCByID(m_SkillID);
            }
        }
        public override void OnEnter() {
            //if (m_Self.IsFight == false) {
            //    m_Self.SetFight(true);
            //    m_Self.ChangeWp();//切换以下武器
            //}
            m_Self.IsFight = false;
            m_Self.m_Resetidletime = AppConfig.FightReset;
            //m_Self.m_Nab.Attack(sd);
            AppTools.SendReturn<SkillDataBase, NPCBase, int>((int)SkillEvent.CountSkillHurt, sd, m_Self);//发送消息让技能模块计算伤害
            if (m_Self.m_target != null) { //每次攻击完就判断一下，如果存在攻击状态，并且攻击目标已经死亡。重新回去巡逻
                if (m_Self.m_target.m_IsDie == true) {
                    m_Self.m_target = null;
                }
            }
        }
        public override void OnExit() {
            m_Self.isPlaySkill = false;
            m_Self.m_Nab.HiddenEffect();
        }
    }
    
    #endregion
}
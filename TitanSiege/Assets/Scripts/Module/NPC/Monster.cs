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
using GF.Const;
using GF.MainGame.Data;
using GF.NetWork;
using MoreMountains.Feedbacks;
using Net.Client;
using Net.Component;
using Net.Share;
using Net.System;
using Pathfinding.RVO;
using System;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

namespace GF.MainGame.Module.NPC {
    public class Monster : NPCBase {
        //private Vector3 m_Pos;//怪物的原始位置，用于战斗返回
        //private Vector3 m_Rota;//怪物的原始旋转，用于战斗返回
        //private Vector3 m_Scal;//怪物的原始缩放，用于放大技能后的恢复
        public Material m_Material = null;
        public int m_NetState;//和服务器monster对应的state，0为服务端更新，1为客户端更新
        public int m_PatrolState;//服务端巡逻状态字段
        public int m_targetID = 0;
        public RVOController m_RvoController;
        public MMF_Player m_Feel;
        //切换用的shader
        public Shader rongjie;
        public Shader mmtoon;
        public Transform m_Shadow;//怪物的阴影，用来死亡时隐藏，这个阴影并不是实际的光影，而是一张图片，节省性能的办法，所以需要手动处理
        ClientSceneManager c;//客户端管理器，用来查询所有玩家数据的
       
        /// <summary>
        /// 溶解显示,倒放溶解
        /// </summary>
        /// <returns></returns>
        public async UniTask ShowModel() {
            float _cut = 1f;
            while (_cut >= 0) {
                _cut -= 0.1f;
                await UniTask.Delay(TimeSpan.FromSeconds(0.0618));
                m_Material.SetFloat("_Cutoff", _cut);
            }
            //更换shader
            m_Material.shader = mmtoon;
            //还得加上是否显示血条判断
            if (AppTools.GetModule<HPModule>(MDef.HPModule).IsShowHp) {
                AppTools.Send<NPCBase>((int)HPEvent.ShowHP, this);
            }
            m_Shadow.gameObject.SetActive(true);
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
            gameObject.SetActive(false);
        }
        public override void InitNPCAnimaor() {
            c = ClientSceneManager.I as ClientSceneManager;
            //发现一个很奇怪的bug，awake时，获取子物体的某个组件会报一次错误，仅一次，也不影响使用
            //因为怪物生成是服务器按针同步的，本地场景未跳转时，就发送服务器跳场景了，所以导致先创建了一次，后跳的场景，跳场景清除物体，导致物体丢失，该问题已修复
            var render = transform.Find("Body").GetComponent<SkinnedMeshRenderer>();
            m_Material = render.material;
            rongjie = Shader.Find("Ultimate 10+ Shaders/Dissolve");
            mmtoon = Shader.Find("MoreMountains/MMToon");
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
            if (m_Feel == null) {
                m_Feel = transform.Find("Feel").GetComponent<MMF_Player>();
            }
            if (m_Shadow==null) {
                m_Shadow = transform.Find("shadow");
            }
        }
        public override void UpdateFightProps() {
            base.UpdateFightProps();
        }
        public void Update() {
            if (m_IsDie) {
                return;
            }
            if ((AttackTarget != null) && (AttackTarget.m_GDID!= m_targetID)) { //这种情况说明有其他玩家攻击这个怪物了，所以m_targetID会被服务器更换，这里设计以服务器为准
                if (c.identitys.TryGetValue(m_targetID, out var p)) {
                    AttackTarget = p.GetComponent<Player>();
                }
            }
            if ((AttackTarget != null) && (m_targetID == ClientBase.Instance.UID)) { //如果攻击目标存在，并且就是本机，那就需要负责同步怪物的移动和旋转
                if (NetworkTime.CanSent) {//update的次数由机器配置决定，不确定数量，所以要加上限制，一般不超过30次，NetworkTime.CanSent就是控制发送次数的
                    ClientBase.Instance.AddOperation(new Operation(Command.EnemySync, m_GDID, transform.position, transform.rotation));
                }
            } else if ((m_NetState == 1) && (m_targetID == ClientBase.Instance.UID)&&(AttackTarget == null)) { //如果当前是客户端同步，并且是本机在同步，但怪物的目标已经为null了，一般是脱离攻击范围或者玩家死亡，就发送消息给服务端，转为服务端控制
                if (NetworkTime.CanSent) {
                    //即使有NetworkTime.CanSent限制，这里也会执行多次（比如两个怪物执行了3次），虽然没有报错，但对性能有影响，m_NetState = 0;也无法限制，因为这个过程中，怪物一直再被同步修改状态，先关注这个问题，暂时没有方案
                    //脱离战斗，判断是否死亡脱离，否则恢复最大生命
                    if (!m_IsDie) {
                        FP.FightHP = FP.FightMaxHp;
                        AppTools.Send<NPCBase>((int)HPEvent.UpdateHp, this);
                        m_NetState = 0;
                        m_targetID = 0;
                    }
                    ClientBase.Instance.AddOperation(new Operation(Command.EnemySwitchState, m_GDID) { cmd1 = 0,cmd2 = 0,index = FP.FightHP });
                }
            }
        }
        /// <summary>
        /// 根据服务端转换本地状态
        /// </summary>
        internal void StatusEntry() {
            switch (m_PatrolState) {
                case 0://服务端的0是idle
                    m_State.StatusEntry(m_AllStateID["idle"]);
                    break;
                case 1://服务端的1是移动
                    m_State.StatusEntry(m_AllStateID["run"]);
                    break;
                case 5://服务端的5是死亡
                    m_IsDie = true;
                    Debuger.Log("怪物死亡");
                    m_State.StatusEntry(m_AllStateID["die"]);
                    break;
                default:
                    break;
            }
        }
        /// <summary>
        /// 怪物复活特效处理
        /// </summary>
        public void Fuhuo() {
            gameObject.SetActive(true);
            //重置溶解特效
            m_Material.SetFloat("_Cutoff", 1);
            //倒放溶解，显示怪物模型
            _ = ShowModel();
            m_IsDie = false;
            //更新血条
            AppTools.Send<NPCBase>((int)HPEvent.UpdateHp, this);
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
            if (m_Self.AttackTarget == null) {//没有目标，就让服务器执行
                return;
            }
            if (m_Self.AttackTarget.m_GDID!=ClientBase.Instance.UID) {//目标对象不是本机，也不执行，谁是怪物攻击目标，就由谁来计算
                return;
            }
            if (m_SendCount>0) {//同一时间如果有数据发送，就不再发送
                return;
            }
            //判断目前为止和怪物的距离，然后决定攻击、追击、放弃
            m_Self.transform.LookAt(new Vector3(m_Self.AttackTarget.transform.position.x, m_Self.transform.position.y, m_Self.AttackTarget.transform.position.z));
            float dis = Vector3.Distance( m_Self.transform.position, m_Self.AttackTarget.transform.position );
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
            if (m_Self.AttackTarget == null) {//没有目标，就让服务器执行
                return;
            }
            if (m_Self.AttackTarget.m_GDID != ClientBase.Instance.UID) {//目标对象不是本机，也不执行，谁是怪物攻击目标，就由谁来计算
                return;
            }
            if (m_SendCount > 0) {//同一时间如果有数据发送，就不再发送
                return;
            }
            //判断目前为止和怪物的距离，然后决定攻击、追击、放弃
            m_Self.transform.LookAt(new Vector3(m_Self.AttackTarget.transform.position.x, m_Self.transform.position.y, m_Self.AttackTarget.transform.position.z));
            float dis = Vector3.Distance(m_Self.transform.position, m_Self.AttackTarget.transform.position);
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
                if (i <= 5) {//使用技能1
                    ClientBase.Instance.AddOperation(new Operation(Command.EnemySwitchState, m_Self.m_GDID) { cmd1 = 1, cmd2 = (byte)m_Self.m_AllStateID[sname] });
                    m_SendCount++;
                } else { //使用技能2
                    if (m_Self.m_SkillId.Count == 0) {
                        return;
                    }
                    //if (m_Self.m_SkillId.Count == 1) {//如果只有一个技能还是用技能1
                    //    sname = "skill1";
                    //} else {
                    //    sname = "skill2";
                    //}
                    if (m_Self.m_SkillId.Count == 1) {
                        sname = "skill1";
                    } else {
                        int j = UnityEngine.Random.Range(1, m_Self.m_SkillId.Count + 1);//int随机数不覆盖最大值，所以要+1
                        sname = "skill" + j;
                    }
                    ClientBase.Instance.AddOperation(new Operation(Command.EnemySwitchState, m_Self.m_GDID) { cmd1 = 1, cmd2 = (byte)m_Self.m_AllStateID[sname] });
                    m_SendCount++;
                }
            } else if (dis > AppConfig.AttackRange && dis <= AppConfig.WarnRange) { //追击
                //每桢向目标移动
                var targetPoint = m_Self.transform.position + m_Self.transform.forward * 100;
                m_Self.m_RvoController.SetTarget(targetPoint, AppConfig.MonsterSpeed, AppConfig.MonsterSpeed+2f);
                var delta = m_Self.m_RvoController.CalculateMovementDelta(m_Self.transform.position, Time.deltaTime);
                m_Self.transform.position = transform.position + delta;
            }
        }
    }
    public class MAttackAciton : MyAcitonCore {
        private Monster m_Self;
        private int m_SkillID;
        private SkillDataBase m_SData;
        public override void OnInit() {
            m_Self = transform.GetComponent<Monster>();
        }
        public override void OnEnter(StateAction action) {
            if (m_Self.AttackTarget == null) {//如果没有攻击对象，直接返回idle
                m_Self.ChangeState(m_Self.m_AllStateID["idle"]);
            }
            //本游戏设定，怪物最多三个技能,就按照顺序排
            if (action.clipName == "skill1") {
                m_SkillID = m_Self.m_SkillId[0];
            } else if (action.clipName == "skill2") {
                m_SkillID = m_Self.m_SkillId[1];
            } else if (action.clipName == "skill3") {
                m_SkillID = m_Self.m_SkillId[2];
            }
            m_Self.m_CurrentSkillId = m_SkillID;
            if (m_Self.m_CurrentSkillId == -1) {//如果没有技能id，就直接返回
                m_Self.ChangeState(m_Self.m_AllStateID["idle"]);
            }

            if (m_SData == null || (m_SData.id != m_Self.m_CurrentSkillId)) {//减少重复获取数据
                m_SData = ConfigerManager.m_SkillData.FindSkillByID(m_Self.m_CurrentSkillId);
                animEventList.Clear();//因为减少数据获取，所以事件的清理要放在这里
                // 初始化技能配置表中的事件
                for (int i = 0; i < m_SData.skilleventlist.Count; i++) {
                    EventArg e = new EventArg();
                    e.eventType = (SkillEventType)m_SData.skilleventlist[i].eventtype;
                    e.animEventTime = m_SData.skilleventlist[i].eventtime;
                    e.eventEnter = false;
                    e.eventeff = m_SData.skilleventlist[i].eventeff;
                    animEventList.Add(e);
                }
            }
            //怪物的数据滞后，所以一定要后调用父类方法
            base.OnEnter(action);
        }
        /// <summary>
        ///怪物动画帧事件处理方法
        ///和玩家公用一套配置表逻辑，但处理比玩家简单很多，几乎没有特殊情况
        /// </summary>
        /// <param name="action"></param>
        public override void OnAnimationEvent(StateAction action,EventArg eventarg) {
            switch (eventarg.eventType) {
                case SkillEventType.hiddeneff://隐藏游戏特效，这里主要是需要提前隐藏的情况，正常情况下，游戏特效播放结束后，系统会控制自动隐藏
                    effectSpwan.gameObject.SetActive(false);
                    break;
                case SkillEventType.attack:
                    if (m_SData.usecollider != 0) {//使用碰撞
                        switch ((SkillColliderType)m_SData.usecollider) {
                            case SkillColliderType.box:
                                Collider[] hitarr = new Collider[6];
                                Vector3 center = new Vector3(m_Self.transform.position.x, m_Self.transform.position.y, m_Self.transform.position.z);
                                int hitnum = Physics.OverlapBoxNonAlloc(center, new Vector3(m_SData.range / 2, 1f, m_SData.range / 2), hitarr, m_Self.transform.rotation, LayerMask.GetMask("npc"));
                                if (hitnum > 0) {
                                    List<NPCBase> mlist = new List<NPCBase>();
                                    for (int j = 0; j < hitnum; j++) {//判断一下攻击到的是否是怪物
                                        if (hitarr[j].name == m_Self.name) {
                                            continue;
                                        }
                                        Monster m = hitarr[j].transform.GetComponent<Monster>();
                                        if (m != null) {
                                            mlist.Add(m);
                                        }
                                    }
                                    if (mlist.Count > 0) {
                                        AppTools.Send<SkillDataBase, NPCBase, List<NPCBase>>((int)SkillEvent.CountSkillHurt, m_SData, m_Self, mlist);//发送消息让技能模块计算伤害，不知道为什么这里不写上泛型会发布出去消息，
                                    }
                                }
                                break;
                            case SkillColliderType.line:
                                break;
                            case SkillColliderType.circle:
                                break;
                            case SkillColliderType.none:
                                break;
                            default: break;
                        }
                    } else { //不使用碰撞
                        AppTools.Send<SkillDataBase, NPCBase, List<NPCBase>>((int)SkillEvent.CountSkillHurt, m_SData, m_Self, null);//发送消息让技能模块计算伤害
                    }
                    break;
                default:
                    break;
            }
        }
        public override void OnExit(StateAction action) {
            base.OnExit(action);
            if ((activeMode == ActiveMode.Active) && (spwanmode != SpwanMode.SetParent) && effectSpwan!=null) {//如果是隐藏显示模式的，就手动隐藏一下，其他模式不需要，因为有计时器销毁
                effectSpwan.gameObject.SetActive(false);
            }
            m_Self.m_CurrentSkillId = -1;
            if (m_Self.isPlaySkill == true) {//可能提前停止技能控制，所以这里判断一下
                m_Self.isPlaySkill = false;
            }
            //每次攻击完后，判断一下玩家有没有死亡
            if (m_Self.AttackTarget != null && m_Self.AttackTarget.m_IsDie) {
                m_Self.AttackTarget = null;
            }
        }
    }
    public class MDieState : StateBehaviour {
        private Monster m_Self;
        private ushort m_EnterStop = 0;//因为只要不脱离状态，死亡动画播放完成后，onstop会一直调用，需要加个判断，避免onstop中的代码重复调用
        public override void OnInit() {
            m_Self = transform.GetComponent<Monster>();
        }
        public override void OnEnter() {
            //因为怪物的死亡服务端没有通知更新血条，所以要先更新一下血条
            AppTools.Send<NPCBase>((int)HPEvent.UpdateHp,m_Self);
            //取消选中状态
            AppTools.Send<NPCBase>((int)NpcEvent.CanelSelected, m_Self);
            m_Self.AttackTarget = null;
            //死亡5秒后，溶解尸体，隐藏怪物模型
            ThreadManager.Event.AddEvent(4f, () => {
                _ = m_Self.HideModel();
            });
        }
        //注意onstop只要一直处于状态，就会不停调用执行，这里得处理一下，避免重复
        public override void OnStop() {
            //播放完死亡动画，更换shader，隐藏阴影，隐藏血条
            if (m_EnterStop == 0) {
                m_Self.m_Material.shader = m_Self.rongjie;
                m_Self.m_Shadow.gameObject.SetActive(false);
                AppTools.Send<NPCBase>((int)HPEvent.HideHP, m_Self);
                m_EnterStop++;
            }
        }
        public override void OnExit() {
            m_Self.Fuhuo();//复活处理
            m_EnterStop = 0;
        }
    }
    
    #endregion
}
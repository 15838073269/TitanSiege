/****************************************************
    文件：Player.cs
	作者：昔莘
    邮箱: 304183153@qq.com
    日期：2021/11/29 15:36:19
	功能：玩家技能处理脚本，继承个人魔改GDNET的AcitonCore
*****************************************************/
using GF.ConfigTable;
using GF.Const;
using GF.MainGame.Data;
using GF.MainGame.Module.NPC;
using GF.MainGame;
using GF.NetWork;
using GF;
using Net.Client;
using System.Collections.Generic;
using UnityEngine;
using GameDesigner;

public class PSkillAction : MyAcitonCore {
    private Player m_Self;
    private SkillDataBase m_SData = null;
    private NPCBase m_WeiyiMonster = null;//位移技能的目标怪物
    private float m_WeiyiDis = 0f;//位移的距离
    private EventArg m_WeiyiArg = null;//位移技能的参数
    public override void OnInit() {
        m_Self = transform.GetComponent<Player>();
    }
    public override void OnEnter(StateAction action) {
        base.OnEnter(action);
        //先清理一下，防止变量污染
        m_WeiyiMonster = null;
        m_Self.m_Resetidletime = AppConfig.FightReset;
        if (m_Self.m_CurrentSkillId == -1) {//如果没有技能id，就直接返回
            m_Self.m_State.StatusEntry(m_Self.m_AllStateID["fightidle"]);
        }
        if (m_SData == null || (m_SData.id != m_Self.m_CurrentSkillId)) {//减少重复获取数据
            m_SData = ConfigerManager.m_SkillData.FindNPCByID(m_Self.m_CurrentSkillId);
            animEventList.Clear();//因为减少数据获取，所以事件的清理要放在这里
            m_WeiyiArg = null;
            m_WeiyiDis = 0f;
            // 初始化技能配置表中的事件
            for (int i = 0; i < m_SData.skilleventlist.Count; i++) {
                EventArg e = new EventArg();
                e.eventType = (SkillEventType)m_SData.skilleventlist[i].eventtype;
                e.animEventTime = m_SData.skilleventlist[i].eventtime;
                e.eventEnter = false;
                e.eventeff = m_SData.skilleventlist[i].eventeff;
                if (e.eventType == SkillEventType.weiyi) {//如果是位移技能就先赋值位移参数
                    m_WeiyiArg = e;
                    m_WeiyiDis = e.eventeff;
                }
                animEventList.Add(e);
            }
        } 
        //冲锋技能特殊处理，寻找附近最近的怪物
        if (m_WeiyiArg != null && m_WeiyiDis != 0) {
            if (m_Self.AttackTarget == null) { //如果玩家没有冲锋对象，就寻找最近的怪物目标
                if (m_WeiyiArg != null && m_WeiyiDis != 0) {
                    ClientSceneManager c = ClientSceneManager.I as ClientSceneManager;
                    Dictionary<int, Monster> monsters = c.m_MonsterDics;
                    if (monsters != null && monsters.Count > 0) {//场景内有怪物再计算伤害
                        foreach (KeyValuePair<int, Monster> m in monsters) {
                            float dis = Vector3.Distance(m.Value.transform.position, m_Self.transform.position);
                            if (dis <= m_WeiyiArg.eventeff && !m.Value.m_IsDie) {
                                if (m_WeiyiDis > dis) {//如果比上一次大， 就赋值，简单的冒泡排序
                                    m_WeiyiDis = dis;
                                    m_WeiyiMonster = m.Value;
                                }
                            }
                        }
                    }
                }
                if (m_WeiyiMonster != null) {
                    m_Self.AttackTarget = m_WeiyiMonster;//有冲锋对象，就把攻击目标给玩家
                    AppTools.Send<NPCBase>((int)NpcEvent.ChangeSelected, m_Self.AttackTarget);
                    m_Self.transform.LookAt(m_WeiyiMonster.transform.position);
                }
            } else {
                //有攻击对象的话，需要先判断一下位移路径上有没有其他怪物阻挡，如果有，就切换为其他怪物 
                float dis = Vector3.Distance(m_Self.transform.position, m_Self.AttackTarget.transform.position);
                RaycastHit hit;
                //发射一条射线到目标怪物，看看路径上有没有其他怪物,这个射线本质上永远能碰撞到怪物，因为最起码能碰撞到原有的目标怪物,
                if (Physics.Raycast(m_Self.transform.position, (m_Self.AttackTarget.transform.position - m_Self.transform.position), out hit, dis, LayerMask.GetMask("npc"))) {
                    Monster m = hit.transform.GetComponent<Monster>();
                    if (m == null || m == m_Self.AttackTarget) {//如果射线碰撞还是自己，或者碰撞的物体不是怪物，那还是直接赋值
                        m_WeiyiMonster = m_Self.AttackTarget;
                        m_WeiyiDis = dis;
                    } else {//如果射线是别的怪物，那就更换冲锋目标
                        m_WeiyiMonster = m;
                        m_WeiyiDis = Vector3.Distance(m.transform.position, m_Self.transform.position);
                        m_Self.AttackTarget = m;//有冲锋对象，就把攻击目标给玩家
                        AppTools.Send<NPCBase>((int)NpcEvent.ChangeSelected, m);
                    }
                    m_Self.transform.LookAt(m_WeiyiMonster.transform.position);
                } else {
                    //理论上，这个射线本质上永远能碰撞到怪物，因为最起码能碰撞到原有的目标怪物，所以不用else
                }
            }
        }

    }
    /// <summary>
    /// 动画帧事件处理方法
    /// </summary>
    /// <param name="action"></param>
    public override void OnAnimationEvent(StateAction action, EventArg eventarg) {
        switch (eventarg.eventType) {
            case SkillEventType.hiddeneff://隐藏游戏特效，这里主要是需要提前隐藏的情况，正常情况下，游戏特效播放结束后，系统会控制自动隐藏
                effectSpwan.gameObject.SetActive(false);
                //EffectArg arg = obj as EffectArg;
                //if (arg != null) {
                //    if (arg.CurrentEffect != null) {
                //        if (!arg.CurrentEffect.m_IsFollow) {
                //            arg.CurrentEffect.transform.parent = m_EffectFather;
                //            arg.CurrentEffect.transform.localPosition = arg.LastEffectLocalPos;
                //            arg.CurrentEffect.transform.localRotation = arg.LastEffectLocalRotate;
                //        }
                //        arg.CurrentEffect.gameObject.SetActive(false);
                //    }
                //}
                break;
            case SkillEventType.attack:
                if (m_SData.usecollider != 0) {//使用碰撞
                    switch ((SkillColliderType)m_SData.usecollider) {
                        case SkillColliderType.box:
                            if (m_WeiyiArg != null) {//如果是位移技能，且有位移目标
                                if (m_WeiyiMonster != null) {//如果是位移技能，没有目标就不计算伤害，说明是空放技能
                                    m_WeiyiMonster.transform.position += (m_Self.transform.forward * 0.318f);//朝玩家攻击方向的后退一点，模拟击退效果
                                    AppTools.Send((int)SkillEvent.CountSkillHurtWithOne, m_SData, m_Self, m_WeiyiMonster as Monster);//发送消息让技能模块计算伤害
                                }
                            } else {//不是位移技能
                                    //位移的时候发送一条射线过去，碰到谁，就执行掉血
                                RaycastHit[] hitarr = Physics.BoxCastAll(m_Self.transform.position, new Vector3(2f, 2f, 2f), m_Self.transform.forward, Quaternion.identity, 5f);
                                Debuger.Log("hit"+hitarr.Length);
                                if (hitarr.Length > 0) {
                                    List<NPCBase> mlist = new List<NPCBase>();
                                    for (int j = 0; j < hitarr.Length; j++) {
                                        mlist.Add(hitarr[j].transform.GetComponent<Monster>());
                                    }
                                    AppTools.Send((int)SkillEvent.CountSkillHurt, m_SData, m_Self, mlist);//发送消息让技能模块计算伤害
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
                   
                    AppTools.Send<SkillDataBase, NPCBase,List<NPCBase>>((int)SkillEvent.CountSkillHurt, m_SData, m_Self,null);//发送消息让技能模块计算伤害
                }
                break;
            case SkillEventType.weiyi:
                float sec = m_Self.GetAnimatorSeconds(m_Self.m_Nab.m_ani, m_SData.texiao);
                m_Self.isPlaySkill = true;
                if (m_Self.m_GDID == ClientBase.Instance.UID) {//如果不是网络对象，就自行控制特效移动
                    if (eventarg.eventeff > 0) {//速度只要不为0，就会移动
                        float tempdis = 0;
                        if (m_WeiyiDis != 0f && m_WeiyiMonster != null && m_WeiyiDis <= eventarg.eventeff) {//判断一下是否有目标怪物，如果有，就只移动到怪物处
                            if (m_WeiyiDis < AppConfig.AttackRange) { //如果小于攻击范围，就不用位移了
                                tempdis = 0f;
                            } else {
                                tempdis = m_WeiyiDis;
                            }
                        } else {
                            tempdis = eventarg.eventeff;
                        }
                        float t = tempdis / sec;
                        AppTools.Send<float, bool>((int)MoveEvent.SetSkillMove, t, true);
                    } else { //速度如果为0，就是发送不能移动的命令
                        AppTools.Send<float, bool>((int)MoveEvent.SetSkillMove, 0f, true);
                    }
                }
                break;
            case SkillEventType.stopweiyi://停止技能移动，一般用于提前停止
                AppTools.Send<float, bool>((int)MoveEvent.SetSkillMove, 0f, false);
                break;
            default:

                break;
        }
    }
    public override void OnExit(StateAction action) {
        base.OnExit(action);
        if (activeMode == ActiveMode.Active) {//如果是隐藏显示模式的，就手动隐藏一下，其他模式不需要，因为有计时器销毁
            effectSpwan.gameObject.SetActive(false);
        }
        m_Self.isPlaySkill = false;
        m_WeiyiMonster = null;
        //m_WeiyiDis = 0f;
        //m_WeiyiArg = null;
        m_Self.m_CurrentSkillId = -1;
        AppTools.Send<float, bool>((int)MoveEvent.SetSkillMove, 0f, false);
    }
}
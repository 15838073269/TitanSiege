/****************************************************
    �ļ���Player.cs
	���ߣ���ݷ
    ����: 304183153@qq.com
    ���ڣ�2021/11/29 15:36:19
	���ܣ���Ҽ��ܴ���ű����̳и���ħ��GDNET��AcitonCore
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
    private NPCBase m_WeiyiMonster = null;//λ�Ƽ��ܵ�Ŀ�����
    private float m_WeiyiDis = 0f;//λ�Ƶľ���
    private EventArg m_WeiyiArg = null;//λ�Ƽ��ܵĲ���
    public override void OnInit() {
        m_Self = transform.GetComponent<Player>();
    }
    public override void OnEnter(StateAction action) {
        base.OnEnter(action);
        //������һ�£���ֹ������Ⱦ
        m_WeiyiMonster = null;
        m_Self.m_Resetidletime = AppConfig.FightReset;
        if (m_Self.m_CurrentSkillId == -1) {//���û�м���id����ֱ�ӷ���
            m_Self.m_State.StatusEntry(m_Self.m_AllStateID["fightidle"]);
        }
        if (m_SData == null || (m_SData.id != m_Self.m_CurrentSkillId)) {//�����ظ���ȡ����
            m_SData = ConfigerManager.m_SkillData.FindNPCByID(m_Self.m_CurrentSkillId);
            animEventList.Clear();//��Ϊ�������ݻ�ȡ�������¼�������Ҫ��������
            m_WeiyiArg = null;
            m_WeiyiDis = 0f;
            // ��ʼ���������ñ��е��¼�
            for (int i = 0; i < m_SData.skilleventlist.Count; i++) {
                EventArg e = new EventArg();
                e.eventType = (SkillEventType)m_SData.skilleventlist[i].eventtype;
                e.animEventTime = m_SData.skilleventlist[i].eventtime;
                e.eventEnter = false;
                e.eventeff = m_SData.skilleventlist[i].eventeff;
                if (e.eventType == SkillEventType.weiyi) {//�����λ�Ƽ��ܾ��ȸ�ֵλ�Ʋ���
                    m_WeiyiArg = e;
                    m_WeiyiDis = e.eventeff;
                }
                animEventList.Add(e);
            }
        } 
        //��漼�����⴦��Ѱ�Ҹ�������Ĺ���
        if (m_WeiyiArg != null && m_WeiyiDis != 0) {
            if (m_Self.AttackTarget == null) { //������û�г����󣬾�Ѱ������Ĺ���Ŀ��
                if (m_WeiyiArg != null && m_WeiyiDis != 0) {
                    ClientSceneManager c = ClientSceneManager.I as ClientSceneManager;
                    Dictionary<int, Monster> monsters = c.m_MonsterDics;
                    if (monsters != null && monsters.Count > 0) {//�������й����ټ����˺�
                        foreach (KeyValuePair<int, Monster> m in monsters) {
                            float dis = Vector3.Distance(m.Value.transform.position, m_Self.transform.position);
                            if (dis <= m_WeiyiArg.eventeff && !m.Value.m_IsDie) {
                                if (m_WeiyiDis > dis) {//�������һ�δ� �͸�ֵ���򵥵�ð������
                                    m_WeiyiDis = dis;
                                    m_WeiyiMonster = m.Value;
                                }
                            }
                        }
                    }
                }
                if (m_WeiyiMonster != null) {
                    m_Self.AttackTarget = m_WeiyiMonster;//�г����󣬾Ͱѹ���Ŀ������
                    AppTools.Send<NPCBase>((int)NpcEvent.ChangeSelected, m_Self.AttackTarget);
                    m_Self.transform.LookAt(m_WeiyiMonster.transform.position);
                }
            } else {
                //�й�������Ļ�����Ҫ���ж�һ��λ��·������û�����������赲������У����л�Ϊ�������� 
                float dis = Vector3.Distance(m_Self.transform.position, m_Self.AttackTarget.transform.position);
                RaycastHit hit;
                //����һ�����ߵ�Ŀ��������·������û����������,������߱�������Զ����ײ�������Ϊ����������ײ��ԭ�е�Ŀ�����,
                if (Physics.Raycast(m_Self.transform.position, (m_Self.AttackTarget.transform.position - m_Self.transform.position), out hit, dis, LayerMask.GetMask("npc"))) {
                    Monster m = hit.transform.GetComponent<Monster>();
                    if (m == null || m == m_Self.AttackTarget) {//���������ײ�����Լ���������ײ�����岻�ǹ���ǻ���ֱ�Ӹ�ֵ
                        m_WeiyiMonster = m_Self.AttackTarget;
                        m_WeiyiDis = dis;
                    } else {//��������Ǳ�Ĺ���Ǿ͸������Ŀ��
                        m_WeiyiMonster = m;
                        m_WeiyiDis = Vector3.Distance(m.transform.position, m_Self.transform.position);
                        m_Self.AttackTarget = m;//�г����󣬾Ͱѹ���Ŀ������
                        AppTools.Send<NPCBase>((int)NpcEvent.ChangeSelected, m);
                    }
                    m_Self.transform.LookAt(m_WeiyiMonster.transform.position);
                } else {
                    //�����ϣ�������߱�������Զ����ײ�������Ϊ����������ײ��ԭ�е�Ŀ�������Բ���else
                }
            }
        }

    }
    /// <summary>
    /// ����֡�¼�������
    /// </summary>
    /// <param name="action"></param>
    public override void OnAnimationEvent(StateAction action, EventArg eventarg) {
        switch (eventarg.eventType) {
            case SkillEventType.hiddeneff://������Ϸ��Ч��������Ҫ����Ҫ��ǰ���ص��������������£���Ϸ��Ч���Ž�����ϵͳ������Զ�����
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
                if (m_SData.usecollider != 0) {//ʹ����ײ
                    switch ((SkillColliderType)m_SData.usecollider) {
                        case SkillColliderType.box:
                            if (m_WeiyiArg != null) {//�����λ�Ƽ��ܣ�����λ��Ŀ��
                                if (m_WeiyiMonster != null) {//�����λ�Ƽ��ܣ�û��Ŀ��Ͳ������˺���˵���ǿշż���
                                    m_WeiyiMonster.transform.position += (m_Self.transform.forward * 0.318f);//����ҹ�������ĺ���һ�㣬ģ�����Ч��
                                    AppTools.Send((int)SkillEvent.CountSkillHurtWithOne, m_SData, m_Self, m_WeiyiMonster as Monster);//������Ϣ�ü���ģ������˺�
                                }
                            } else {//����λ�Ƽ���
                                    //λ�Ƶ�ʱ����һ�����߹�ȥ������˭����ִ�е�Ѫ
                                RaycastHit[] hitarr = Physics.BoxCastAll(m_Self.transform.position, new Vector3(2f, 2f, 2f), m_Self.transform.forward, Quaternion.identity, 5f);
                                Debuger.Log("hit"+hitarr.Length);
                                if (hitarr.Length > 0) {
                                    List<NPCBase> mlist = new List<NPCBase>();
                                    for (int j = 0; j < hitarr.Length; j++) {
                                        mlist.Add(hitarr[j].transform.GetComponent<Monster>());
                                    }
                                    AppTools.Send((int)SkillEvent.CountSkillHurt, m_SData, m_Self, mlist);//������Ϣ�ü���ģ������˺�
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
                } else { //��ʹ����ײ
                   
                    AppTools.Send<SkillDataBase, NPCBase,List<NPCBase>>((int)SkillEvent.CountSkillHurt, m_SData, m_Self,null);//������Ϣ�ü���ģ������˺�
                }
                break;
            case SkillEventType.weiyi:
                float sec = m_Self.GetAnimatorSeconds(m_Self.m_Nab.m_ani, m_SData.texiao);
                m_Self.isPlaySkill = true;
                if (m_Self.m_GDID == ClientBase.Instance.UID) {//�������������󣬾����п�����Ч�ƶ�
                    if (eventarg.eventeff > 0) {//�ٶ�ֻҪ��Ϊ0���ͻ��ƶ�
                        float tempdis = 0;
                        if (m_WeiyiDis != 0f && m_WeiyiMonster != null && m_WeiyiDis <= eventarg.eventeff) {//�ж�һ���Ƿ���Ŀ��������У���ֻ�ƶ������ﴦ
                            if (m_WeiyiDis < AppConfig.AttackRange) { //���С�ڹ�����Χ���Ͳ���λ����
                                tempdis = 0f;
                            } else {
                                tempdis = m_WeiyiDis;
                            }
                        } else {
                            tempdis = eventarg.eventeff;
                        }
                        float t = tempdis / sec;
                        AppTools.Send<float, bool>((int)MoveEvent.SetSkillMove, t, true);
                    } else { //�ٶ����Ϊ0�����Ƿ��Ͳ����ƶ�������
                        AppTools.Send<float, bool>((int)MoveEvent.SetSkillMove, 0f, true);
                    }
                }
                break;
            case SkillEventType.stopweiyi://ֹͣ�����ƶ���һ��������ǰֹͣ
                AppTools.Send<float, bool>((int)MoveEvent.SetSkillMove, 0f, false);
                break;
            default:

                break;
        }
    }
    public override void OnExit(StateAction action) {
        base.OnExit(action);
        if (activeMode == ActiveMode.Active) {//�����������ʾģʽ�ģ����ֶ�����һ�£�����ģʽ����Ҫ����Ϊ�м�ʱ������
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
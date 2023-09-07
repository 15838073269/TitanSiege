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
using System.Drawing.Imaging;

public class PSkillAction : MyAcitonCore {
    private Player m_Self;
    private SkillDataBase m_SData = null;
    private NPCBase m_WeiyiMonster = null;//λ�Ƽ��ܵ�Ŀ�����
    private float m_WeiyiDis = 0f;//λ�Ƶľ���-
    private EventArg m_WeiyiArg = null;//λ�Ƽ��ܵĲ���
    public override void OnInit() {
        m_Self = transform.GetComponent<Player>();
    }
    public override void OnEnter(StateAction action) {
        base.OnEnter(action);
        
        m_Self.m_Resetidletime = AppConfig.FightReset;//����������ս����̬�л�ʱ��
        //������һ�£���ֹ������Ⱦ
        m_WeiyiMonster = null;
        m_Self.m_Resetidletime = AppConfig.FightReset;
        if (m_Self.m_CurrentSkillId == -1) {//���û�м���id����ֱ�ӷ���
            m_Self.ChangeState(m_Self.m_AllStateID["fightidle"]);
        }
        if (m_SData == null || (m_SData.id != m_Self.m_CurrentSkillId)) {//�����ظ���ȡ����
            m_SData = ConfigerManager.m_SkillData.FindSkillByID(m_Self.m_CurrentSkillId);
            animEventList.Clear();//��Ϊ�������ݻ�ȡ�������¼�������Ҫ��������
            m_WeiyiArg = null;
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
        if (m_WeiyiArg != null) {
            m_WeiyiDis = m_WeiyiArg.eventeff;//λ������ÿ�����ã���Ϊ�ظ������ͷ�ʱ���������ݲ������ã�����Ҫ��¼һ�������eventeff��λ�ƾ��룩��m_WeiyiDis��ÿ�θı�ģ�������Ϊλ���������
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
                //�й�������Ļ�����Ҫ���ж�һ���Ƿ��Ƕ๥����range!=0�����ǵ��幥��(range=0),�๥���Ͳ���ҪŶ����λ��·����û���赲�����幥����Ҫ���ж�һ��λ��·������û�����������赲������У����л�Ϊ�������� 
                if (m_SData.range == 0) {
                    float dis = Vector3.Distance(m_Self.transform.position, m_Self.AttackTarget.transform.position);
                    RaycastHit hit;
                    //����һ�����ߵ�Ŀ��������·������û����������,������߱�������Զ����ײ�������Ϊ����������ײ��ԭ�е�Ŀ�����,
                    if (Physics.Raycast(m_Self.transform.position, (m_Self.AttackTarget.transform.position - m_Self.transform.position), out hit, dis, LayerMask.GetMask("npc"))) {
                        Monster m = hit.transform.GetComponent<Monster>();
                        if (m == null || m == m_Self.AttackTarget) {//���������ײ�����Լ���������ײ�����岻�ǹ���ǻ���ֱ�Ӹ�ֵ
                            m_WeiyiMonster = m_Self.AttackTarget;
                            m_WeiyiDis = dis>m_WeiyiDis? m_WeiyiDis:dis;
                        } else {//��������Ǳ�Ĺ���Ǿ͸������Ŀ��
                            m_WeiyiMonster = m;
                            float dis1 = Vector3.Distance(m.transform.position, m_Self.transform.position);
                            m_WeiyiDis = dis1 > m_WeiyiDis ? m_WeiyiDis : dis1;
                            m_Self.AttackTarget = m;//�г����󣬾Ͱѹ���Ŀ������
                            AppTools.Send<NPCBase>((int)NpcEvent.ChangeSelected, m);
                        }
                        Vector3 pos = new Vector3(m_WeiyiMonster.transform.position.x, m_Self.transform.position.y, m_WeiyiMonster.transform.position.z);
                        m_Self.transform.LookAt(pos);
                    } else {
                        //�����ϣ�������߱�������Զ����ײ�������Ϊ����������ײ��ԭ�е�Ŀ�������Բ���else
                        //������۴��󣬻����и����������κζ�����
                        m_WeiyiMonster = m_Self.AttackTarget;
                        Vector3 pos = new Vector3(m_WeiyiMonster.transform.position.x, m_Self.transform.position.y, m_WeiyiMonster.transform.position.z);
                        m_Self.transform.LookAt(pos);
                    }
                } else {
                    m_WeiyiMonster = m_Self.AttackTarget;
                    //float dis= Vector3.Distance(m_WeiyiMonster.transform.position, m_Self.transform.position);//���ƾ�������bug�����bug������ײ�����ģ������ٷ���˼�ˣ��͸�һ�¼��ܻ��Ʋ��ᱻ���ƾ����
                    //m_WeiyiDis = dis > m_WeiyiDis ? m_WeiyiDis : dis;
                    Vector3 pos = new Vector3(m_WeiyiMonster.transform.position.x, m_Self.transform.position.y, m_WeiyiMonster.transform.position.z);
                    m_Self.transform.LookAt(pos);
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
                break;
            case SkillEventType.attack:
                if (m_SData.usecollider != 0) {//ʹ����ײ
                    switch ((SkillColliderType)m_SData.usecollider) {
                        case SkillColliderType.box:
                            if ((m_WeiyiArg != null) && (m_SData.range == 0)) {//�����λ�Ƽ��ܣ�����λ��Ŀ��,�����ǵ��幥��
                                if (m_WeiyiMonster != null) {//�����λ�Ƽ��ܣ�û��Ŀ��Ͳ������˺���˵���ǿշż���
                                    m_WeiyiMonster.transform.position += (m_Self.transform.forward * 0.318f);//����ҹ�������ĺ���һ�㣬ģ�����Ч��
                                    AppTools.Send<SkillDataBase,Player,Monster>((int)SkillEvent.CountSkillHurtWithOne, m_SData, m_Self, m_WeiyiMonster as Monster);//������Ϣ�ü���ģ������˺�,��֪��Ϊʲô����д�Ϸ��ͻᷢ����ȥ��Ϣ����ʱ��д���ͷ�������Ϣ�������
                                }
                            } else {//����λ�Ƽ��ܣ�������λ�Ƽ��ܣ������Ƕ��幥��
                                //��������ʱ����һ�����߹�ȥ������˭����ִ�е�Ѫ
                                Collider[] hitarr = new Collider[6];
                                Vector3 center = new Vector3(m_Self.transform.position.x, m_Self.transform.position.y, m_Self.transform.position.z);
                                int hitnum = Physics.OverlapBoxNonAlloc(center, new Vector3(m_SData.range/2, 1f, m_SData.range/2), hitarr, m_Self.transform.rotation, LayerMask.GetMask("npc"));
                                if (hitnum > 0) {
                                    List<NPCBase> mlist = new List<NPCBase>();
                                    for (int j = 0; j  < hitnum; j++) {//�ж�һ�¹��������Ƿ��ǹ���
                                        if (hitarr[j].name == m_Self.name) {
                                            continue;
                                        }
                                        Monster m = hitarr[j].transform.GetComponent<Monster>();
                                        if (m != null) {
                                            mlist.Add(m);
                                        }
                                    }
                                    if (mlist.Count>0) {
                                        AppTools.Send<SkillDataBase, NPCBase, List<NPCBase>>((int)SkillEvent.CountSkillHurt, m_SData, m_Self, mlist);//������Ϣ�ü���ģ������˺�����֪��Ϊʲô���ﲻд�Ϸ��ͻᷢ����ȥ��Ϣ��
                                    }
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
            case SkillEventType.weiyi://λ�Ƶ�ֵֻҪ��Ϊ0���ͻ��ƶ���Ϊ0���ǽ�ֹ�ƶ������û��д��һ�����Ǽ��ܾ�������ʩ��ʱ�ƶ�
                m_Self.isPlaySkill = true;//֪ͨ��ʼ���ż��ܣ���ʵ���ǽ�ֹ�ƶ����������û����������¼����Ͳ����ֹ�ƶ�
                if (m_Self.m_GDID == ClientBase.Instance.UID) {//�������������󣬾����п�����Ч�ƶ�
                    if (eventarg.eventeff > 0) {//�ٶ�ֻҪ��Ϊ0���ͻ��ƶ���Ϊ0���ǽ�ֹ�ƶ�
                        float tempdis = 0;
                        if (m_SData.range == 0) {
                            if (m_WeiyiDis < AppConfig.AttackRange) { //���С�ڹ�����Χ���Ͳ���λ����
                                tempdis = 0f;
                            } else {
                                tempdis = m_WeiyiDis;
                            }
                        } else {
                            tempdis = m_WeiyiDis;
                        }
                        float sec = m_Self.GetAnimatorSeconds(m_Self.m_Nab.m_ani, m_SData.texiao);
                        float t = tempdis / sec;
                        Debuger.Log(t);
                        AppTools.Send<float, bool>((int)MoveEvent.SetSkillMove, t, true);
                    } else { //�ٶ����Ϊ0�����Ƿ��Ͳ����ƶ�������
                        AppTools.Send<float, bool>((int)MoveEvent.SetSkillMove, 0f, true);
                    }
                }
                break;
            case SkillEventType.stopweiyi://ֹͣ�����ƶ���һ��������ǰֹͣ,��Ҫ���ڳ�漼�ܵ���ǰ��ֹ����ֹ���������ܻ�û�����꣬���Ի��ǲ����ƶ������Է��ͻ��ǲ����ƶ�������
                AppTools.Send<float, bool>((int)MoveEvent.SetSkillMove, 0f, true);
                break;
            case SkillEventType.addheight://���Ӹ߶ȣ�������Ծ����
                m_Self.transform.position += new Vector3(0, eventarg.eventeff, 0);
                break;
            default:

                break;
        }
    }
    public override void OnExit(StateAction action) {
        base.OnExit(action);
        if ((activeMode == ActiveMode.Active)&&(spwanmode!=SpwanMode.SetParent)) {//�����������ʾģʽ�ģ����ֶ�����һ�£�����ģʽ����Ҫ����Ϊ�м�ʱ������
            effectSpwan.gameObject.SetActive(false);
        }
        m_WeiyiMonster = null;
        //m_WeiyiDis = 0f;
        //m_WeiyiArg = null;
        m_Self.m_CurrentSkillId = -1;
        if (m_Self.isPlaySkill ==true) {//������ǰֹͣ���ܿ��ƣ����������ж�һ��
            m_Self.isPlaySkill = false;
            AppTools.Send<float, bool>((int)MoveEvent.SetSkillMove, 0f, false);
        }
        //�ͷż��ܵ�˲�䣬��������Ѿ�����������ˣ������ܲ���ֹͣ�����ԣ�������ɺ󣬻���Ҫ���һ���Լ���û������
        m_Self.Check();
    }
}
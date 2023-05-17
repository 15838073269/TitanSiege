/****************************************************
    文件：SkillModule.cs
	作者：昔莘
    邮箱: 304183153@qq.com
    日期：2022/11/14 1:27:21
	功能：技能模块
*****************************************************/

using UnityEngine;
using GF.Module;
using GF.MainGame.UI;
using GF.Unity.UI;
using GF.Const;
using GF.MainGame.Data;
using GF.MainGame.Module.NPC;
using GF.Service;
using GF.NetWork;
using Net.Share;
using Net.Client;
using System.Collections.Generic;

namespace GF.MainGame.Module {
    public class SkillModule : GeneralModule {
        private SkillUIWidget m_SkillUI;
        public override void Create() {
            base.Create();
            AppTools.Regist<int>((int)SkillEvent.ClickSkill, ClickSkill);
            AppTools.Regist<ushort>((int)SkillEvent.ManzuXuqiudengji, ManzuXuqiudengji);
            AppTools.Regist<SkillDataBase, NPCBase,int>((int)SkillEvent.CountSkillHurt, CountSkillHurt);
            ///获取所有技能配置表数据
            ///todo
        }

        public override void Release() {
            base.Release();
        }

        public override void Show() {
            base.Show();
            UIPanel p = UIManager.GetInstance.GetUI(AppConfig.MainUIPage);
            m_SkillUI = AppMain.GetInstance.uiroot.FindUIInChild<SkillUIWidget>(p.transform, AppConfig.SkillUIWidget);
        }
        /// <summary>
        /// 点击技能
        /// </summary>
        /// <param name="info"></param>
        /// <param name="npc"></param>
        public void ClickSkill(int skillposid, NPCBase npc) {

            //根据技能连击等待时间做一个计数器，计算是否触发连击动画，目前系统共用一个连击等待时间，设置为2s,2秒内再次触发技能，就会有连击动画，这个功能只会是普攻技能触发，需要判定一下
            //if(sb.skilltype==普攻){
            //开一个协程，或者task处理
            //}
            //toodo
            switch (skillposid) {
                case 0://普攻

                    break;
                case 1://技能1
                    npc.m_State.StatusEntry(npc.m_AllStateID["skill1"]);
                    break;
                case 2://技能2
                    npc.m_State.StatusEntry(npc.m_AllStateID["skill2"]);
                    break;
                case 3://技能3
                    npc.m_State.StatusEntry(npc.m_AllStateID["skill3"]);
                    break;
                case 4://技能4
                    npc.m_State.StatusEntry(npc.m_AllStateID["skill4"]);
                    break;
                default:
                    break;
            }
            
            //AppTools.Send<NPCBase, AniState, object>((int)StateEvent.ChangeStateWithArgs, npc, AniState.attack, info.Data);
        }
        /// <summary>
        /// 点击技能，默认玩家攻击
        /// </summary>
        /// <param name="info"></param>
        public void ClickSkill(int skillposid) {
            ClickSkill(skillposid, UserService.GetInstance.m_CurrentPlayer);
        }
        /// <summary>
        /// 角色升级时执行一下本函数
        /// </summary>
        public void ManzuXuqiudengji(ushort level) {
            m_SkillUI.JiesuoSkill(level);
        }
        /// <summary>
        /// 准备玩家计算技能的伤害
        /// </summary>
        public int CountSkillHurt(SkillDataBase sb, NPCBase npc) {
            int damage = 0;
            if (npc.m_NpcType == NpcType.player) {//玩家攻击怪物的情况
                //string scenename = SceneManager.GetActiveScene().name;
                //先从场景类中获取到当前场景和场景内的所有怪物对象
                //ListSafe<Monster> monsters = AppTools.SendReturn<string, ListSafe<Monster>>((int)NpcEvent.GetMonstersbyScene, scenename);
                ClientSceneManager c = ClientSceneManager.I as ClientSceneManager;
                Dictionary<int,Monster> monsters = c.m_MonsterDics;
                if (monsters != null && monsters.Count > 0) {//场景内有怪物再计算伤害
                    //提前预算一下，距离玩家15以外的怪物就不再计算了，节省性能，15是本项目最大技能范围，一般技能范围不会超过15
                    List<CountSkillArg> tempmonstersarg = new List<CountSkillArg>();
                    foreach (KeyValuePair<int,Monster> m in monsters) {
                        float dis = Vector3.Distance(m.Value.transform.position, npc.transform.position);
                        Debuger.Log(dis + ":" + m.Value.m_GDID);
                        if (dis <= sb.range && !m.Value.m_IsDie) {
                            CountSkillArg temp = new CountSkillArg(m.Value, dis);
                            tempmonstersarg.Add(temp);
                        }
                    }
                    if (tempmonstersarg.Count > 0) {
                        damage = CountData(sb, npc, tempmonstersarg);
                    } else {
                        Debuger.Log("本次技能没有打到任何怪物");
                    }
                }
            } else if (npc.m_NpcType == NpcType.monster) { //怪物攻击玩家，而且只可能攻击的是本人
                Monster m = npc as Monster;
                if (!m.m_target.m_IsDie) {
                    damage = CountData(sb, m, m.m_target as Player);
                }
            }
            return damage;
        }
        /// <summary>
        /// 实际计算玩家技能伤害
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="npc"></param>
        /// <param name="monstersarg"></param>
        private int CountData(SkillDataBase sb, NPCBase npc, List<CountSkillArg> monstersarg) { //实际计算的方法
            int damage = 0;
            if (sb.skillattlist != null && sb.skillattlist.Count > 0) {
                for (int i = 0; i < sb.skillattlist.Count; i++) {
                    skillatt att = sb.skillattlist[i];
                    for (int j = 0; j < monstersarg.Count; j++) {
                        DamageArg damagearg = new DamageArg();
                        //计算是否在技能的攻击角度内,距离小于1.5，就默认能打到，不用计算角度了，离得太近
                        float angle = GetAngle(npc.transform, monstersarg[j].monster.transform);
                        if (monstersarg[j].dis <= AppConfig.AttackRange && InAngle(angle, sb.angle)) { //空留给角度计算
                            damage = GetDamage(sb.shanghai, (SkillType)sb.skilltype, npc, monstersarg[j].monster, out damagearg.damagetype);
                            damagearg.damage = damage;
                            damagearg.npc = monstersarg[j].monster;
                            if (damage != 0) {
                                //切换怪物受击状态
                                // AppTools.Send<NPCBase, AniState>((int)StateEvent.ChangeState, monstersarg[j].monster, AniState.hurt);
                                Debuger.Log($"{UserService.GetInstance.m_CurrentChar.Name}对{monstersarg[j].monster.Data.Name}{monstersarg[j].monster.m_GDID}造成了{damage}点伤害");
                                if (npc.m_GDID == ClientBase.Instance.UID) {//这个判断没啥用，必进的，因为只要开始计算伤害了，就一定是本机玩家
                                    if (monstersarg[j].monster.m_target == null) { //如果被攻击的怪物没有目标玩家
                                        monstersarg[j].monster.m_target = npc;
                                    }
                                    ClientBase.Instance.AddOperation(new Operation(Command.Attack, npc.m_GDID) { index = damage, index1 = monstersarg[j].monster.m_GDID });
                                }
                            } else {
                                Debuger.Log($"{monstersarg[j].monster.Data.Name}闪避了{UserService.GetInstance.m_CurrentChar.Name}的攻击");
                            }

                        }
                        //闪避了就忽略延迟，直接显示
                        //if (att.yanchi != 0f && damagearg.damagetype != DamageType.shangbi) {
                        //    //延迟显示攻击数字即可，没必要就算都延迟
                        //    //ThreadManager.Event.AddEvent(att.yanchi, ShowDamage, damagearg);
                        //} else {
                        //    //没有延迟，直接显示攻击伤害数值飘血
                        //    ShowDamage(damagearg);
                        //}
                    }
                }
            } else {
                Debuger.LogError($"技能{sb.name}未配置攻击点，请检查配置文件id");
            }
            return damage;
        }
        //怪物技能攻击玩家
        private int CountData(SkillDataBase sb, Monster m, Player p) {
            int damage = 0;
            float dis = Vector3.Distance(p.transform.position, m.transform.position);
            if (sb.skillattlist != null && sb.skillattlist.Count > 0) {
                for (int i = 0; i < sb.skillattlist.Count; i++) {
                    skillatt att = sb.skillattlist[i];
                    DamageArg damagearg = new DamageArg();
                    //计算是否在技能的攻击角度内,距离小于1.5，就默认能打到，不用计算角度了，离得太近
                    float angle = GetAngle(m.transform, p.transform);
                    if (dis <= AppConfig.AttackRange && InAngle(angle, sb.angle)) { //空留给角度计算
                        damage = GetDamage(sb.shanghai, (SkillType)sb.skilltype, m, p, out damagearg.damagetype);
                        damagearg.damage = damage;
                        damagearg.npc = p;
                        if (damage != 0) {
                            //切换受击状态
                            // AppTools.Send<NPCBase, AniState>((int)StateEvent.ChangeState, p, AniState.hurt);
                            Debuger.Log($"{m.name}对{UserService.GetInstance.m_CurrentChar.Name}{p.m_GDID}造成了{damage}点伤害");
                            if (m.m_target.m_GDID == ClientBase.Instance.UID) { //只有攻击的是本机玩家，本机才会同步，攻击其他的玩家，本机不发送同步消息
                                ClientBase.Instance.AddOperation(new Operation(Command.EnemyAttack) { index = damage });//这里不用给参数，因为只有攻击是本机玩家才会发送，默认发送本机玩家的即可，所以只用发伤害过去
                            }
                        } else {
                            Debuger.Log($"{UserService.GetInstance.m_CurrentChar.Name}{p.m_GDID}闪避了{m.name}的攻击");
                        }
                    }
                    //闪避了就忽略延迟，直接显示
                    //if (att.yanchi != 0f && damagearg.damagetype!= DamageType.shangbi) {
                    //    //延迟显示攻击数字即可，没必要就算都延迟
                    //    //ThreadManager.Event.AddEvent(att.yanchi,ShowDamage, damagearg);
                    //} else {
                    //    //没有延迟，直接显示攻击伤害数值飘血
                    //    ShowDamage(damagearg);
                    //}
                }
            } else {
                Debuger.LogError($"技能{sb.name}未配置攻击点，请检查配置文件id");
            }
            return damage;
        }
        /// <summary>
        /// 计算是否在攻击夹角内
        /// </summary>
        /// <param name="angle"></param>
        /// <returns></returns>
        private bool InAngle(float angle,float skillangle) {
            if (skillangle == 360) {
                return true;
            } else {
                if (angle <= skillangle / 2f) {
                    return true;
                } else {
                    return false;
                }
            }
        }
        /// <summary>
        /// 获取夹角
        /// </summary>
        /// <param name="att"></param>
        /// <param name="beatt"></param>
        /// <returns></returns>
        private float GetAngle(Transform att, Transform beatt) {
            //计算攻击者正前方和怪物的夹角
            float angleto = Vector3.Angle(att.forward, (beatt.position - att.position).normalized);
            return angleto;
        }
        /// <summary>
        /// 获取伤害值（玩家攻击怪物）
        /// </summary>
        /// <param name="shanghai">技能伤害</param>
        /// <param name="skilltype">技能类型</param>
        /// <param name="att">攻击者</param>
        /// <param name="beatt">被攻击者</param>
        /// <returns></returns>
        private int GetDamage(int shanghai, SkillType skilltype, NPCBase att, NPCBase beatt,out DamageType damagetype) {
            int damage = 0;
            damagetype = DamageType.none;
            if (JsShanbi(beatt.Dodge)) {
                damagetype = DamageType.shangbi;
                return 0;
            }
            //技能加成和属性加成,力量加成物理攻击,1点力量10点攻击
            damage += (att.Attack + shanghai);
            //计算装备伤害  todo

            //暴击，随机2-3倍伤害，幸运加成暴击,幸运加成最高50%；
            float randbj = RandomHelper.Range(0f, 1f);
            if (randbj <= att.Crit) {//暴击成功
                damagetype = DamageType.baoji;
                //暴击随机2-4倍的伤害
                int baojinum = RandomHelper.Range(2, 4);
                damage *= baojinum;
            }
            //防御上不区分法防和物防
            damage = (damage - beatt.Defense) > 0 ? (damage - beatt.Defense) : 1;//不能破防，就强制掉血1点
            return damage;
        }
        
        /// <summary>
        /// 计算闪避
        /// </summary>
        /// <param name="beattmj">被攻击方敏捷</param>
        /// <returns></returns>
        private bool JsShanbi(float dodge) {
            if (RandomHelper.Range(0f, 1f) <= dodge) {//闪避成功
                return true;
            }
            return false;
        }
        /// <summary>
        /// 显示伤害UI飘字
        /// </summary>
        /// <param name="arg">参数</param>
        /// <param name="npc">被伤害的npc，可以是玩家，也可以是怪物</param>
        private void ShowDamage(object arg) { 
            DamageArg damageArg= (DamageArg)arg;
            AppTools.Send<DamageArg>((int)HPEvent.ShowDamgeTxt,damageArg);
        }
    }
    
    public class CountSkillArg{
        public CountSkillArg(Monster mon, float dis) {
            this.monster = mon;
            this.dis = dis;
        }
        public float dis;
        public Monster monster;
    }
    public class DamageArg {
        public DamageType damagetype;
        public int damage;
        public NPCBase npc;
    }
}

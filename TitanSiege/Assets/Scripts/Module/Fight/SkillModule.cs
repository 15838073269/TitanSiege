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
            AppTools.Regist<SkillDataBase, NPCBase,List<NPCBase>>((int)SkillEvent.CountSkillHurt, CountSkillHurt);
            AppTools.Regist<SkillDataBase, Player, Monster>((int)SkillEvent.CountSkillHurtWithOne, CountSkillHurtWithOne);
            //AppTools.Regist<DamageArg>((int)SkillEvent.ShowDamage, ShowDamage);
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
            //如果不是战斗状态，先切换成战斗状态，再切换技能状态
            if (npc.IsFight == false) {
                npc.SetFight(true);
                npc.ChangeWp();//切换以下武器
            }
            npc.m_Resetidletime = AppConfig.FightReset;//重置战斗状态的切换时间
            switch (skillposid) {
                case 0://普攻
                    npc.m_CurrentSkillId = npc.m_SkillId[0];
                    break;
                case 1://技能1
                    npc.m_CurrentSkillId = npc.m_SkillId[1];
                    npc.ChangeState(npc.m_AllStateID["skill1"], npc.m_CurrentSkillId);
                    break;
                case 2://技能2
                    npc.m_CurrentSkillId = npc.m_SkillId[2];
                    npc.ChangeState(npc.m_AllStateID["skill2"], npc.m_CurrentSkillId);
                    break;
                case 3://技能3
                    npc.m_CurrentSkillId = npc.m_SkillId[3];
                    npc.ChangeState(npc.m_AllStateID["skill3"], npc.m_CurrentSkillId);
                    break;
                case 4://技能4
                    npc.m_CurrentSkillId = npc.m_SkillId[4];
                    npc.ChangeState(npc.m_AllStateID["skill4"], npc.m_CurrentSkillId);
                    break;
                default:
                    break;
            }
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
        public void CountSkillHurt(SkillDataBase sb, NPCBase npc,List<NPCBase> mlist = null) {
            if (npc.m_NpcType == NpcType.player) {//玩家攻击怪物的情况
                if (mlist != null) {//说明使用了碰撞判定，那就直接处理怪物即可
                    List<CountSkillArg> tempmonstersarg = new List<CountSkillArg>();
                    for (int i = 0; i < mlist.Count; i++) {
                        float dis = Vector3.Distance(mlist[i].transform.position, npc.transform.position);
                        Debuger.Log(dis + ":" + mlist[i].m_GDID);
                        if (!mlist[i].m_IsDie) {
                            CountSkillArg temp = new CountSkillArg(mlist[i] as Monster, dis);
                            tempmonstersarg.Add(temp);
                        }
                    }
                    if (tempmonstersarg.Count > 0) {
                        Player p = npc as Player;
                        CountData(sb, p, tempmonstersarg,true);
                    } else {
                        Debuger.Log("本次技能没有打到任何怪物");
                    }
                } else {
                    ClientSceneManager c = ClientSceneManager.I as ClientSceneManager;
                    Dictionary<int, Monster> monsters = c.m_MonsterDics;
                    if (monsters != null && monsters.Count > 0) {//场景内有怪物再计算伤害
                        //提前预算一下
                        List<CountSkillArg> tempmonstersarg = new List<CountSkillArg>();
                        foreach (KeyValuePair<int, Monster> m in monsters) {
                            float dis = Vector3.Distance(m.Value.transform.position, npc.transform.position);
                            Debuger.Log(dis + ":" + m.Value.m_GDID);
                            if (dis <= sb.range && !m.Value.m_IsDie) {
                                CountSkillArg temp = new CountSkillArg(m.Value, dis);
                                tempmonstersarg.Add(temp);
                            }
                        }
                        if (tempmonstersarg.Count > 0) {
                            Player p = npc as Player;
                            CountData(sb, p, tempmonstersarg);
                        } else {
                            Debuger.Log("本次技能没有打到任何怪物");
                        }
                    }
                }
            } else if (npc.m_NpcType == NpcType.monster) { //怪物攻击玩家，而且只可能攻击的是本机，攻击不是本机，那伤害是同步过来的
                Monster m = npc as Monster;
                if (!m.AttackTarget.m_IsDie) {
                    CountData(sb, m, m.AttackTarget as Player);
                }
            }
        }
        /// <summary>
        /// 用于碰撞时，单个怪物的伤害计算,这种情况只可能用于玩家伤害计算，因为比较精确，怪物的不用，怪物的直接用数学计算
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="npc"></param>
        /// <param name="m"></param>
        public void CountSkillHurtWithOne(SkillDataBase sb, Player p, Monster m) {
            if (m != null) {
                float dis = Vector3.Distance(m.transform.position, p.transform.position);
                Debuger.Log(dis + ":" + m.m_GDID);
                if (!m.m_IsDie) {
                    CountSkillArg temp = new CountSkillArg(m, dis);
                    CountData(sb, p, temp);
                }
            }
        }
        /// <summary>
        /// 实际计算玩家技能伤害
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="npc"></param>
        /// <param name="monstersarg"></param>
        private void CountData(SkillDataBase sb, Player npc, List<CountSkillArg> monstersarg, bool usecollider = false) { //实际计算的方法
            for (int j = 0; j < monstersarg.Count; j++) {
                int damage = 0;
                DamageArg damagearg = new DamageArg();
                if (usecollider) {
                    damage = GetDamage(sb.shanghai, (SkillType)sb.skilltype, npc, monstersarg[j].monster, out damagearg.damagetype);
                    damagearg.damage = damage;
                    damagearg.npc = monstersarg[j].monster;
                    if (damage != 0) {
                        //切换怪物受击状态
                        // AppTools.Send<NPCBase, AniState>((int)StateEvent.ChangeState, monstersarg[j].monster, AniState.hurt);
                        monstersarg[j].monster.transform.position -= (monstersarg[j].monster.transform.forward * 0.318f);//朝怪物朝向后退一点，模拟击退效果
                        Debuger.Log($"{UserService.GetInstance.m_CurrentChar.Name}对{monstersarg[j].monster.Data.Name}{monstersarg[j].monster.m_GDID}造成了{damage}点伤害");
                    } else {
                        Debuger.Log($"{monstersarg[j].monster.Data.Name}闪避了{UserService.GetInstance.m_CurrentChar.Name}的攻击");
                    }
                } else {
                    //计算是否在技能的攻击角度内,距离小于1.5，就默认能打到，不用计算角度了，离得太近
                    float angle = GetAngle(npc.transform, monstersarg[j].monster.transform);
                    if (monstersarg[j].dis <= AppConfig.AttackRange || InAngle(angle, sb.angle)) { //空留给角度计算
                        damage = GetDamage(sb.shanghai, (SkillType)sb.skilltype, npc, monstersarg[j].monster, out damagearg.damagetype);
                        damagearg.damage = damage;
                        damagearg.npc = monstersarg[j].monster;
                        if (damage != 0) {
                            //切换怪物受击状态
                            Vector3 dir = new Vector3(npc.transform.forward.x,0f, npc.transform.forward.z);
                            monstersarg[j].monster.transform.position -= (monstersarg[j].monster.transform.forward * 0.318f);//朝自身方向的后退一点，模拟击退效果
                            Debuger.Log($"{UserService.GetInstance.m_CurrentChar.Name}对{monstersarg[j].monster.Data.Name}{monstersarg[j].monster.m_GDID}造成了{damage}点伤害");
                        } else {
                            Debuger.Log($"{monstersarg[j].monster.Data.Name}闪避了{UserService.GetInstance.m_CurrentChar.Name}的攻击");
                        }
                    }
                }
                //只要开始计算伤害了，就一定是本机玩家  
                if (monstersarg[j].monster.AttackTarget == null) { //如果被攻击的怪物没有目标玩家
                    monstersarg[j].monster.AttackTarget = npc;
                }
                if (damage != 0) {
                    monstersarg[j].monster.m_Feel.PlayFeedbacks();
                    ClientBase.Instance.AddOperation(new Operation(Command.Attack, npc.m_GDID) { index = damage, index1 = monstersarg[j].monster.m_GDID });
                }
                AppTools.Send<DamageArg>((int)HPEvent.ShowDamgeTxt, damagearg);
            }
        }
        /// <summary>
        /// 使用碰撞时，单个怪物逇伤害计算，只可能是玩家对怪物攻击使用
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="npc"></param>
        /// <param name="monstersarg"></param>
        /// <param name="usecollider"></param>
        private void CountData(SkillDataBase sb, Player npc, CountSkillArg monstersarg) {
            int damage = 0;
            DamageArg damagearg = new DamageArg();
            damage = GetDamage(sb.shanghai, (SkillType)sb.skilltype, npc, monstersarg.monster, out damagearg.damagetype);
            damagearg.damage = damage;
            damagearg.npc = monstersarg.monster;
            if (damage != 0) {
                //切换怪物受击状态
                Debuger.Log($"{UserService.GetInstance.m_CurrentChar.Name}对{monstersarg.monster.Data.Name}{monstersarg.monster.m_GDID}造成了{damage}点伤害");
            } else {
                Debuger.Log($"{monstersarg.monster.Data.Name}闪避了{UserService.GetInstance.m_CurrentChar.Name}的攻击");
            }
            //只要开始计算伤害了，就一定是本机玩家  
            if (monstersarg.monster.AttackTarget == null) { //如果被攻击的怪物没有目标玩家
                monstersarg.monster.AttackTarget = npc;
            }
            if (damage != 0) {
                monstersarg.monster.m_Feel.PlayFeedbacks();
                ClientBase.Instance.AddOperation(new Operation(Command.Attack, npc.m_GDID) { index = damage, index1 = monstersarg.monster.m_GDID });
            }
            AppTools.Send<DamageArg>((int)HPEvent.ShowDamgeTxt, damagearg);
        }
        /// <summary>
        /// 怪物技能攻击玩家
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="m"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        private int CountData(SkillDataBase sb, Monster m, Player p) {
            int damage = 0;
            float dis = Vector3.Distance(p.transform.position, m.transform.position);
            DamageArg damagearg = new DamageArg();
            //计算是否在技能的攻击角度内,距离小于1，就默认能打到，不用计算角度了，离得太近
            float angle = GetAngle(m.transform, p.transform);
            if (dis <= AppConfig.AttackRange || InAngle(angle, sb.angle)) { //空留给角度计算
                damage = GetDamage(sb.shanghai, (SkillType)sb.skilltype, m, p, out damagearg.damagetype);
                damagearg.damage = damage;
                damagearg.npc = p;
                if (damage != 0) {
                    //切换受击状态
                    // AppTools.Send<NPCBase, AniState>((int)StateEvent.ChangeState, p, AniState.hurt);
                    Debuger.Log($"{m.name}对{UserService.GetInstance.m_CurrentChar.Name}{p.m_GDID}造成了{damage}点伤害");
                    if (m.AttackTarget.m_GDID == ClientBase.Instance.UID) { //只有攻击的是本机玩家，本机才会同步，攻击其他的玩家，本机不发送同步消息
                        ClientBase.Instance.AddOperation(new Operation(Command.EnemyAttack) { index = damage });//这里不用给参数，因为只有攻击是本机玩家才会发送，默认发送本机玩家的即可，所以只用发伤害过去
                    }
                } else {
                    Debuger.Log($"{UserService.GetInstance.m_CurrentChar.Name}{p.m_GDID}闪避了{m.name}的攻击");
                }
            }
            AppTools.Send<DamageArg>((int)HPEvent.ShowDamgeTxt, damagearg);
            //闪避了就忽略延迟，直接显示
            //if (att.yanchi != 0f && damagearg.damagetype!= DamageType.shangbi) {
            //    //延迟显示攻击数字即可，没必要就算都延迟
            //    //ThreadManager.Event.AddEvent(att.yanchi,ShowDamage, damagearg);
            //} else {
            //    //没有延迟，直接显示攻击伤害数值飘血
            //    ShowDamage(damagearg);
            //}
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
            if (JsShanbi(beatt.FP.Dodge)) {
                damagetype = DamageType.shangbi;
                return 0;
            }
            //技能加成和属性加成,力量加成物理攻击,1点力量10点攻击
            damage += (att.FP.Attack + shanghai);
            //计算装备伤害  todo

            //暴击，随机2-3倍伤害，幸运加成暴击,幸运加成最高50%；
            float randbj = RandomHelper.Range(0f, 1f);
            if (randbj <= att.FP.Crit) {//暴击成功
                damagetype = DamageType.baoji;
                //暴击随机2-4倍的伤害
                int baojinum = RandomHelper.Range(2, 4);
                damage *= baojinum;
            }
            //防御上不区分法防和物防
            damage = (damage - beatt.FP.Defense) > 0 ? (damage - beatt.FP.Defense) : 1;//不能破防，就强制掉血1点
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
        private void ShowDamage(DamageArg arg) { 
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

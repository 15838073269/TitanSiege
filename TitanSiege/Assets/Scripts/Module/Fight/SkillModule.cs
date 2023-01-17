/****************************************************
    文件：SkillModule.cs
	作者：昔莘
    邮箱: 304183153@qq.com
    日期：2022/11/14 1:27:21
	功能：Nothing
*****************************************************/

using UnityEngine;
using GF.Module;
using GF.MainGame.UI;
using GF.Unity.UI;
using GF.Const;
using GF.MainGame.Data;
using GF.MainGame.Module.NPC;
using GF.Service;
using GF.Unity.AB;
using Net.System;
using GF.NetWork;
using UnityEngine.SceneManagement;
using Net.Share;

namespace GF.MainGame.Module {
    public class SkillModule : GeneralModule {
        private SkillUIWidget m_SkillUI;
        public override void Create() {
            base.Create();
            AppTools.Regist<SkillUIInfo>((int)SkillEvent.ClickSkill, ClickSkill);
            AppTools.Regist<ushort>((int)SkillEvent.ManzuXuqiudengji, ManzuXuqiudengji);
            AppTools.Regist<SkillDataBase, NPCBase>((int)SkillEvent.CountSkillHurt, CountSkillHurt);
            ///获取所有技能配置表数据
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
        public void ClickSkill(SkillUIInfo info, NPCBase npc) {

            //根据技能连击等待时间做一个计数器，计算是否触发连击动画，目前系统共用一个连击等待时间，设置为2s,2秒内再次触发技能，就会有连击动画，这个功能只会是普攻技能触发，需要判定一下
            //if(sb.skilltype==普攻){
            //开一个协程，或者task处理
            //}
            //toodo
            AppTools.Send<NPCBase, AniState, object>((int)StateEvent.ChangeStateWithArgs, npc, AniState.attack, info.m_Data);
        }
        /// <summary>
        /// 点击技能，默认玩家攻击
        /// </summary>
        /// <param name="info"></param>
        public void ClickSkill(SkillUIInfo info) {
            ClickSkill(info, UserService.GetInstance.m_CurrentPlayer);
        }
        /// <summary>
        /// 角色升级时执行一下本函数
        /// </summary>
        public void ManzuXuqiudengji(ushort level) {
            m_SkillUI.JiesuoSkill(level);
        }
        /// <summary>
        /// 准备计算技能的伤害
        /// </summary>
        public void CountSkillHurt(SkillDataBase sb, NPCBase npc) {
            string scenename = SceneManager.GetActiveScene().name;
            //先从场景类中获取到当前场景和场景内的所有怪物对象
            ListSafe<Monster> monsters = AppTools.SendReturn<string, ListSafe<Monster>>((int)NpcEvent.GetMonstersbyScene, scenename);
            if (monsters != null && monsters.Count>0) {//场景内有怪物再计算伤害
                //提前预算一下，距离玩家一百以外的怪物就不再计算了，节省性能
                ListSafe<CountSkillArg> tempmonstersarg = new ListSafe<CountSkillArg>();
                for (int i = 0; i < monsters.Count; i++) {
                    float dis = Vector3.Distance(monsters[i].transform.position, npc.transform.position);
                    Debuger.Log(dis+":"+ monsters[i].m_GDID);
                    if (dis <=100f) {
                        CountSkillArg temp = new CountSkillArg(monsters[i],dis);
                        tempmonstersarg.Add(temp);
                    }
                }
                if (tempmonstersarg.Count > 0) {
                    CountData(sb, npc, tempmonstersarg);
                } else {
                    Debuger.Log("本次技能没有打到任何怪物");
                }
            }
        }
        /// <summary>
        /// 实际计算技能伤害
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="npc"></param>
        /// <param name="monstersarg"></param>
        private void CountData(SkillDataBase sb, NPCBase npc, ListSafe<CountSkillArg> monstersarg) { //实际计算的方法
            if (sb.skillattlist != null && sb.skillattlist.Count > 0) {
                for (int i = 0; i < sb.skillattlist.Count; i++) {
                    skillatt att = sb.skillattlist[i];
                    for (int j = 0; j < monstersarg.Count; j++) {
                        if (monstersarg[j].dis <= att.range) {//计算是否在技能攻击的范围内
                            //计算是否在技能的攻击角度内,距离小于1，就默认能打到，不用计算角度了，离得太近
                            float angle = GetAngle(npc.transform, monstersarg[j].monster.transform);
                            if ((monstersarg[j].dis < 1f&&angle <90f)||InAngle(angle, sb.skillattlist[i].angle)) { //空留给角度计算
                                int damage = GetDamage(sb.shanghai, (SkillType)sb.skilltype, npc.Data, monstersarg[j].monster.Data);
                                if (damage != 0) {
                                    //飘字 todo
                                    Debuger.Log($"{npc.Data.Name}对{monstersarg[j].monster.Data.Name}造成了{damage}点伤害");
                                } else {
                                    Debuger.Log($"{monstersarg[j].monster.Data.Name}闪避了{npc.m_GDID}的伤害");
                                }
                            }
                            //todo
                        }
                    }
                    if (att.yanchi != 0f) {
                        //延迟显示攻击数字即可，没必要就算都延迟
                        //ThreadManager.Event.AddEvent(att.yanchi, CountData, arg);
                    } else {
                        //没有延迟，直接显示攻击伤害数值飘血
                    }
                }
            } else {
                Debuger.LogError($"技能{sb.name}未配置攻击点，请检查配置文件id");
            }
        }
        /// <summary>
        /// 计算怪物是否在攻击夹角内
        /// </summary>
        /// <param name="att"></param>
        /// <param name="beatt"></param>
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
        /// 获取伤害值
        /// </summary>
        /// <param name="shanghai">技能伤害</param>
        /// <param name="skilltype">技能类型</param>
        /// <param name="att">攻击者</param>
        /// <param name="beatt">被攻击者</param>
        /// <returns></returns>
        private int GetDamage(int shanghai, SkillType skilltype, NPCDataBase att, NPCDataBase beatt) {
            int damage = -1;
            //闪避,基础闪避率0.01f;
            float tempsb = (float)beatt.Minjie / 1000f >= 0.3f ? 0.3f : (float)beatt.Minjie / 1000f;//属性加成的闪避
            float shanbi = 0.01f + tempsb;
            float randsb = RandomHelper.Range(0f, 1f);
            if (randsb <= shanbi) {//闪避成功
                 //todo 飘字 miss

                return 0;
            }
            if (skilltype == SkillType.wuli) {
                
                //属性加成,力量加成物理攻击,1点力量10点攻击
                damage += att.Liliang * 10;
                //计算装备伤害  todo

                //暴击，随机2-3倍伤害，幸运加成暴击,幸运加成最高50%；
                if (att.Xingyun>0) {
                    float baoji = (float)att.Xingyun * 0.05f >= 0.5f ? 0.5f : (float)att.Xingyun * 0.05f;
                    float randbj = RandomHelper.Range(0f, 1f);
                    if (randbj <= baoji) {//闪避成功
                        //todo 飘字暴击
                        damage *= 2;
                    }
                }
                //计算属性及装备防御，装备防御 todo,力量加成30%的防御，防御上不区分法防和物防
                int fangyu = beatt.Liliang * 3 + beatt.Tizhi * 7;
                damage = (damage - fangyu) > 0 ? (damage - fangyu) : 1;//不能破防，就强制掉血1点
            } else if (skilltype == SkillType.fashu) {
                
                //属性加成,魔法加成法术攻击,1点魔力10点攻击
                damage += att.Moli * 10;
                //计算装备伤害  todo

                //暴击，随机2-3倍伤害，幸运加成暴击,幸运加成最高50%；
                if (att.Xingyun > 0) {
                    float baoji = (float)att.Xingyun * 0.05f >= 0.5f ? 0.5f : (float)att.Xingyun * 0.05f;
                    float randbj = RandomHelper.Range(0f, 1f);
                    if (randbj <= baoji) {//闪避成功
                        //todo 飘字暴击
                        damage *= 2;
                    }
                }
                //计算属性及装备防御，装备防御 todo,魔力加成30%防御，防御上不区分法防和物防
                int fangyu = beatt.Moli * 3 + beatt.Tizhi * 7;
                damage = (damage - fangyu) >= 0 ? (damage - fangyu) : 1;//不能破防，就强制掉血1点
            }
            return damage;
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
}

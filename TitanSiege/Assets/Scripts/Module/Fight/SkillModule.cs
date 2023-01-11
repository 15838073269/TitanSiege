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
            Debuger.Log("开始计算伤害");
            string scenename = SceneManager.GetActiveScene().name;
            //先从场景类中获取到当前场景和场景内的所有怪物对象
            ListSafe<Monster> monsters = AppTools.SendReturn<string, ListSafe<Monster>>((int)NpcEvent.GetMonstersbyScene, scenename);
            if (monsters != null && monsters.Count>0) {//场景内有怪物再计算伤害
                //提前预算一下，距离玩家二百以外的怪物就不再计算了，节省性能
                ListSafe<CountSkillArg> tempmonstersarg = new ListSafe<CountSkillArg>();
                for (int i = 0; i < monsters.Count; i++) {
                    float dis = Vector3.Distance(monsters[i].transform.position, npc.transform.position);
                    if (dis <=200f) {
                        CountSkillArg temp = new CountSkillArg(monsters[i],dis);
                        tempmonstersarg.Add(temp);
                    }
                }
                if (tempmonstersarg.Count>0) {
                    CountData(sb, npc, tempmonstersarg);
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
                        if (monstersarg[j].dis<= att.range) {//计算是否在技能攻击的范围内
                            //计算是否在技能的攻击角度内
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

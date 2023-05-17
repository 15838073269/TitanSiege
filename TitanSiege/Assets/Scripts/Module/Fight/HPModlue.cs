/****************************************************
    文件：HPModlue.cs
	作者：昔莘
    邮箱: 304183153@qq.com
    日期：2023/3/12 20:35:23
	功能：Nothing
*****************************************************/

using UnityEngine;
using GF.Module;
using GF.MainGame.UI;
using GF.Unity.AB;
using GF.Pool;
using GF.MainGame.Module.NPC;
using GF.Service;
using System.Collections.Generic;
using GF.Const;

namespace GF.MainGame.Module {
    public class HPModule : GeneralModule {
        public bool IsShowHp = true;
        private  Transform m_HpUIRoot;
        private ClassObjectPool<DamageUIWidget> m_UIPool;
        private Dictionary<NPCBase, DamageUIWidget> m_HPDic;
        public override void Create() {
            base.Create();
            m_UIPool = new ClassObjectPool<DamageUIWidget>(50,true);
            m_UIPool.CretateObj += CreateDamageUI;
            m_HPDic = new Dictionary<NPCBase, DamageUIWidget>();
            m_HpUIRoot = AppMain.GetInstance.uiroot.FindUI("HpUIRoot").transform;
            if (m_HpUIRoot == null) {
                Debuger.LogError("未设置血条UI的父级，请检查！");
                return;
            }
            m_HpUIRoot.gameObject.SetActive(IsShowHp);
            //处理事件
            AppTools.Regist<NPCBase>((int)HPEvent.CreateHPUI, CreateHPUI);
            AppTools.Regist<DamageArg>((int)HPEvent.ShowDamgeTxt, ShowDamgeTxt);
            AppTools.Regist<NPCBase>((int)HPEvent.ShowHP, ShowHP);
            AppTools.Regist<NPCBase>((int)HPEvent.HideHP, HideHP);
        }
        /// <summary>
        /// 创建血条,这个是给对象池使用的
        /// </summary>
        /// <returns></returns>
        public DamageUIWidget CreateDamageUI() {
            GameObject go = ObjectManager.GetInstance.InstanceObject(AppConfig.DamageUIPath, setTranform: true, bClear: false, isfullpath: false, father: m_HpUIRoot);
            DamageUIWidget dui = go.GetComponent<DamageUIWidget>();
            return dui;
        }
        /// <summary>
        /// 怪物创建自身血条的入口
        /// </summary>
        /// <param name="npc"></param>
        public void CreateHPUI(NPCBase npc) {
            if (m_HPDic.ContainsKey(npc)) {
                Debuger.LogError($"{npc.name}{npc.m_GDID}重复创建血条，请检查");
                return;
            } 
            DamageUIWidget dwt = m_UIPool.GetObj(true);
            dwt.InitPos(npc.m_HPRoot);
            //如果是本机玩家
            float amount = 1.0f;
            if (npc.m_NpcType == Const.NpcType.player && npc.m_IsNetPlayer == false) {
                amount = (float)npc.FightHP / (float)UserService.GetInstance.m_CurrentChar.Shengming;
            } else if (npc.m_IsNetPlayer) {//网络玩家,todo
                
            } else if (npc.m_NpcType == Const.NpcType.monster) { //怪物
                amount = (float)npc.FightHP / (float)npc.FightMaxHp;
            }
            dwt.m_Red.fillAmount = amount;
            dwt.m_RedTo.fillAmount = amount;
            m_HPDic.Add(npc,dwt);
        }
        /// <summary>
        /// 显示攻击伤害的飘字
        /// </summary>
        /// <param name="arg"></param>
        public void ShowDamgeTxt(DamageArg arg) {
            DamageUIWidget hpui = null;
            m_HPDic.TryGetValue(arg.npc, out hpui);
            if (hpui != null) {
                hpui.SetAndShowDamgeTxt(arg.damage, arg.damagetype);
            } else {
                Debuger.LogError($"{arg.npc.name}{arg.npc.m_GDID}没有创建血条，请检查");
            }
        }
        private void ShowHP(NPCBase npc) {
            DamageUIWidget hpui = null;
            m_HPDic.TryGetValue(npc, out hpui);
            if (hpui != null) {
                hpui.m_IsInscreen = true;
                hpui.gameObject.SetActive(true);
            } else {
                Debuger.LogError($"{npc.name}{npc.m_GDID}没有创建血条，请检查");
            }
        }
        private void HideHP(NPCBase npc) {
            DamageUIWidget hpui = null;
            m_HPDic.TryGetValue(npc, out hpui);
            if (hpui != null) {
                hpui.m_IsInscreen = false;
                hpui.gameObject.SetActive(false);
            } else {
                Debuger.LogError($"{npc.name}{npc.m_GDID}没有创建血条，请检查");
            }
        }
        public override void Show() {
            base.Show();
            m_HpUIRoot.gameObject.SetActive(IsShowHp);
        }

        public override void Release() {
            base.Release();
        }
    }
}

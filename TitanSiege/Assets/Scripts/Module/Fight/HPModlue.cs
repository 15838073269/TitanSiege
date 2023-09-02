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
using Net.Client;
using GF.Unity.UI;
using Cysharp.Threading.Tasks;
using MoreMountains.Feedbacks;
using System;

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
            m_HpUIRoot = AppMain.GetInstance.uiroot.transform.Find("HPUIRoot");
            if (m_HpUIRoot == null) {
                Debuger.LogError("未设置血条UI的父级，请检查！");
                return;
            }
            m_HpUIRoot.SetAsFirstSibling();
            m_HpUIRoot.gameObject.SetActive(IsShowHp);
            //处理事件
            AppTools.Regist<NPCBase>((int)HPEvent.CreateHPUI, CreateHPUI);
            AppTools.Regist<DamageArg>((int)HPEvent.ShowDamgeTxt, ShowDamgeTxt);
            AppTools.Regist<NPCBase>((int)HPEvent.ShowHP, ShowHP);
            AppTools.Regist<NPCBase>((int)HPEvent.HideHP, HideHP);
            AppTools.Regist<NPCBase>((int)HPEvent.UpdateHp, UpdateHp);
            AppTools.Regist<NPCBase>((int)HPEvent.UpdateMp, UpdateMp);
        }
        /// <summary>
        /// 创建血条,这个是给对象池使用的
        /// </summary>
        /// <returns></returns>
        public DamageUIWidget CreateDamageUI() {
            GameObject go = ObjectManager.GetInstance.InstanceObject(AppConfig.DamageUIPath, setTranform: true, bClear: false, isfullpath: false, father: m_HpUIRoot.transform);
            DamageUIWidget dui = go.GetComponent<DamageUIWidget>();
            return dui;
        }
        /// <summary>
        /// 怪物创建自身血条的入口
        /// </summary>
        /// <param name="npc"></param>
        public void CreateHPUI(NPCBase npc) {
            if (m_HPDic.ContainsKey(npc)) {
                //Debuger.Log($"{npc.name}{npc.m_GDID}重复创建血条，请检查");
                return;
            } 
            DamageUIWidget dwt = m_UIPool.GetObj(true);
            dwt.InitPos(npc.m_HPRoot);
            if (npc.m_NpcType == Const.NpcType.player) {//玩家直接显示名称
                Player p = npc as Player;
                dwt.m_NameTxt.text = p.m_PlayerName;
            } else if (npc.m_NpcType == Const.NpcType.monster) { //怪物
                dwt.m_NameTxt.text = npc.Data.Name;
            }
            float amount = (float)npc.FP.FightHP / (float)npc.FP.FightMaxHp;
            dwt.m_Red.fillAmount = amount;
            dwt.m_RedTo.fillAmount = amount;
            m_HPDic.Add(npc,dwt);
            //创建完，更新一下血条蓝条
            UpdateHp(npc);
            UpdateMp(npc);
        }
        /// <summary>
        /// 更新血条，如果是玩家，血条更新还有面板上也要更新
        /// </summary>
        /// <param name="npc"></param>
        public void UpdateHp(NPCBase npc) {
            DamageUIWidget dwt = null;
            if (!m_HPDic.TryGetValue(npc,out dwt)) {
                //这里可能因为物体还没启动起来导致的数据无法读取
                //Debuger.LogError($"{npc.name}{npc.m_GDID}没有血条，请检查");
                return;
            }
            float amount = (float)npc.FP.FightHP / (float)npc.FP.FightMaxHp;
            dwt.m_Red.fillAmount = amount;
            _ = RedToNum(amount, dwt);//开一个task
            //更新面板上的血条
            if (npc.m_GDID == ClientBase.Instance.UID) { //如果更新的是本机玩家
                //todo
            }
        }
        public async UniTask RedToNum(float amount, DamageUIWidget dwt) {
            while (dwt.m_RedTo.fillAmount > amount) {
                dwt.m_RedTo.fillAmount -= 0.003f;
                await UniTask.Delay(1000);
            }
        }
        /// <summary>
        /// 更新蓝条,蓝条只有玩家面板上显示，只更新面板
        /// </summary>
        /// <param name="npc"></param>
        public void UpdateMp(NPCBase npc) {
            //更新面板上的蓝条
            if (npc.m_GDID == ClientBase.Instance.UID) { //如果更新的是本机玩家
                //todo
            }
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
                hpui.StopAni(); 
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

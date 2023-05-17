/****************************************************
    文件：BigMapModule.csModuleManager.GetInstance.Register
	作者：昔莘
    邮箱: 304183153@qq.com
    日期：2021/12/3 9:43:51
	功能：大地图管理模块
*****************************************************/

using UnityEngine;
using GF.Module;
using GF.Unity.AB;
using GF.Service;
using GF.MainGame.Bulid;
using System.Collections.Generic;
using GF.Unity.UI;
using GF.MainGame.UI;
using GF.Msg;
using GF.MainGame.Module.NPC;

namespace GF.MainGame.Module{
    public class BulidModule : GeneralModule {
        /// <summary>
        /// 存储所有大地图上的建筑
        /// </summary>
        private Dictionary<string, BulidBase> m_AllBulidDic;
        private EnterWidget m_Enter;
        
        public override void Create() {
            m_AllBulidDic = new Dictionary<string, BulidBase>();
            AppTools.Regist<SetBigMapMoveObjPosArg>((int)BulidEvent.SetBigMapMoveObjPos, SetBigMapMoveObjPos);
            AppTools.Regist((int)BulidEvent.AllBulidDicReset, AllBulidDicReset);
        }
        
        public override void Release() {
            base.Release();
             AppTools.Remove<SetBigMapMoveObjPosArg>( (int)BulidEvent.SetBigMapMoveObjPos, SetBigMapMoveObjPos);
             AppTools.Remove((int)BulidEvent.AllBulidDicReset, AllBulidDicReset);
        }

        public override void Show() {
        }
       
       
        /// <summary>
        /// 注册bulid到大地图用的方法
        /// </summary>
        /// <param name="bulid"></param>
        public void RegisterBulid() {
            GameObject[] objarr = GameObject.FindGameObjectsWithTag("Buliding");
            if (objarr.Length>0) {
                for (int i = 0; i < objarr.Length; i++) {
                    
                    BulidBase bulid = objarr[i].GetComponent<BulidBase>();
                    m_AllBulidDic.Add(bulid.m_Scenename, bulid);
                }
            } else {
                Debuger.Log("注册的建筑物为空");
            }
        }

        /// <summary>
        /// 建筑物的进入按钮显示处理
        /// </summary>
        /// <param name="bulid"></param>
        /// <param name="go"></param>
        private MoveModule m_Mv;
        public void BulidTriggerEnter(BulidBase bulid, GameObject go) {
            //临时操作
            if (m_AllBulidDic.Count == 0) {
                RegisterBulid();
            }
            NPCBase npc=null;
            if (go != null) {
                npc = go.GetComponent<NPCBase>();
                if (npc != null) {
                    if (m_Mv == null) {
                        m_Mv =AppTools.GetModule<MoveModule>(MDef.MoveModule);
                    }
                    if (npc == m_Mv.GetMoveObj()) {
                        OpenEnterUI(bulid.m_Scenename, bulid.transform.position);
                    } else {
                        Debuger.LogError("发生错误，该主控角色没有npcbase脚本");
                    }
                }
            } else {
                Debuger.LogError("发生错误，数据为空");
            }
        }
        public void OpenEnterUI(string scenename, Vector3 pos) {
            UIEnterArg arg = new UIEnterArg();
            arg.scenename = scenename;
            arg.pos = pos;
            m_Enter = UIManager.GetInstance.OpenUIWidget("EnterWidget", arg) as EnterWidget;
        }
        public void BulidTriggerExit() {
            //NPCBase npc = null;
            //if (go != null) {
            //    npc = go.GetComponent<NPCBase>();
            //    if (npc = m_Main) {
            //        CloseEnterUI();
            //    } else {
            //        Debuger.LogError("发生错误，该主控角色没有npcbase脚本");
            //    }
            //} else {
            //    Debuger.LogError("发生错误，数据为空");
            //}
            if (m_Enter != null) {
                m_Enter.Close();
                m_Enter = null;
            }
        }
        /// <summary>
        /// 因为切换场景后物体销毁了，所以需要重新注册一遍
        /// </summary>
        public void AllBulidDicReset() {
            m_AllBulidDic.Clear();
            RegisterBulid();
        }
        //public void SetBigMapMoveObjPos(string last,string scenename) {
        //    BulidBase bulid=null;
        //    if (m_AllBulidDic.TryGetValue(last, out bulid)) {
        //        //大地图的朝向是1,0,1
        //        SetMoveObjToSceneArg arg = new SetMoveObjToSceneArg(bulid.m_Default, new Vector3(1, 0, 1), scenename);
        //         AppTools.Send(ModuleDef.MoveModule, ModuleMsgDef.SetMoveObjToScene, arg);
        //    } else {
        //        Debuger.LogError($"大地图没有场景名为{scenename}的建筑物，请检查名称拼写");
        //    }
        //}
        public void SetBigMapMoveObjPos(SetBigMapMoveObjPosArg posarg) {
            BulidBase bulid = null;
            if (m_AllBulidDic.TryGetValue(posarg.last, out bulid)) {
                ////大地图的朝向是1,0,1
                //SetMoveObjToSceneArg arg = new SetMoveObjToSceneArg(bulid.m_Default, posarg.scenename);
                // AppTools.Send<SetMoveObjToSceneArg>((int)MoveEvent.SetMoveObjToScene, arg);
            } else {
                Debuger.LogError($"大地图没有场景名为{posarg.scenename}的建筑物，请检查名称拼写");
            }
        }
    }
    public class SetBigMapMoveObjPosArg {
        public string last;
        public string scenename;
        public SetBigMapMoveObjPosArg(string last, string scenename) { 
            this.last = last;
            this.scenename = scenename;
        }
    }
}

/****************************************************
    文件：SceneModule.cs
	作者：昔莘
    邮箱: 304183153@qq.com
    日期：2021/12/6 8:46:40
	功能：所有场景的调用管理类
*****************************************************/

using System.Collections.Generic;
using GF.Module;
using GF.Msg;
using GF.Service;
using GF.Unity.UI;
using MoreMountains.Tools;
using Net.Component;
using Net.Config;
using Net.Share;
using UnityEngine;
using UnityEngine.Analytics;

namespace GF.MainGame.Module {
    public class SceneModule:GeneralModule {
        public Dictionary<string, SceneBase> m_AllSceneBaseDic = new Dictionary<string, SceneBase>();

        public void AddSceneBase(SceneBase sb) {
            if (!m_AllSceneBaseDic.ContainsKey(sb.SceneName)) {
                m_AllSceneBaseDic.Add(sb.SceneName, sb);
            }
        }
        public string m_Current;
        public string m_Last;
        public UIMsgBox m_MsgBox;
        public override void Create() {
            AppTools.Regist<string>((int)SceneEvent.OpenScene,OpenScene);
            AppTools.Regist<SceneBase>((int)SceneEvent.AddSceneBase, AddSceneBase);
            AppTools.Regist<string,Transform>((int)SceneEvent.GetSceneDefaultPos, GetSceneDefaultPos);
            ClientManager.Instance.client.AddRpc(this);
        }
        public void OpenScene(string scenename) {
            m_Last = m_Current;
            UILoadingArg uiarg = new UILoadingArg();
            uiarg.tips = "神话世界的故事由你书写";
            uiarg.title = "正在加载神话纪元，请稍等...";
            GameSceneService.GetInstance.SceneLoadedOver += new System.Action<object>(SendSwtichScene);
            Debuger.Log(GameSceneService.GetInstance.SceneLoadedOver);
            GameSceneService.GetInstance.AsyncLoadScene(scenename, uiarg: uiarg,arg:scenename);
            m_Current = scenename;
        }
        public void SendSwtichScene(object o) {
            //发送服务器切换场景角色,先发送了，这里改成先切场景，场景切换成功后，再发送？
            string SceneName = o as string;
            Debuger.Log("发送场景"+ SceneName);
            if (SceneName != "CreateRole") {//排除一下创建角色的场景
                ClientManager.Instance.SendRT("SwitchScene", SceneName);
            }
            GameSceneService.GetInstance.SceneLoadedOver -= SendSwtichScene;
        }
      
        public override void Release() {
            base.Release();
             AppTools.Remove<string>( (int)SceneEvent.OpenScene, OpenScene);
        }
        private string m_ToScene;
        private bool m_IsShowMap = false;
        public void OpenChuanSongUI(string scenename,bool ShowMap,string m_Charname = "") {
            m_ToScene = scenename;
            m_IsShowMap = ShowMap;
            UIMsgBoxArg arg = new UIMsgBoxArg();
            arg.title = "提示";
            if (ShowMap) {
                arg.content = $"是否离开<color=red>{m_Charname}</color>?";
            } else {
                arg.content = $"是否离开{m_AllSceneBaseDic[m_Current].m_CharName},前往<color=red>{m_Charname}</color>?";
            }
            arg.btnname = "确认|取消";
            m_MsgBox = UIManager.GetInstance.OpenWindow("UIMsgBox",arg) as UIMsgBox;
            m_MsgBox.oncloseevent += CloseTishi;

        }
        private void CloseTishi(object arg) {
            if (arg!=null) {
                ushort index = ushort.Parse(arg.ToString());
                if (index == 0) {
                    Debuger.Log(m_ToScene);
                    if (m_IsShowMap) {
                        OpenBigMapUI(m_ToScene); //如果m_ShowMap == true，这里就是当前场景名称
                    } else {
                        if (!string.IsNullOrEmpty(m_ToScene)) {
                            OpenScene(m_ToScene);
                        }
                    }
                } else if (index == 1) {
                    //m_MsgBox.Close();
                }
            }
            
        }
        /// <summary>
        /// 打开大地图ui
        /// </summary>
        public void OpenBigMapUI(string currentscene) {
            UIManager.GetInstance.OpenPage(AppConfig.MapUIPage, currentscene,ishidden:false);
        }
        /// <summary>
        /// 服务器返回场景切换成功操作
        /// </summary>
        [Rpc]
        public void SwitchSceneCallBack(bool state, string msg) {
            if (state) {
                if (!string.IsNullOrEmpty(msg)) {
                    Debuger.Log(msg);
                }
                //对象池清理回收血条
                AppTools.Send((int)HPEvent.CycleAllObj);
            } else {
                Debuger.LogError("服务器切换场景失败！");
            }
           
        }
        /// <summary>
        /// 获取场景的出生点
        /// </summary>
        /// <param name="scenename"></param>
        /// <returns></returns>
        private Transform GetSceneDefaultPos(string scenename) {
            SceneBase scene = null;
            if (string.IsNullOrEmpty(scenename)) {
                scenename = m_Current;
            }
            if (!m_AllSceneBaseDic.TryGetValue(scenename, out scene)) {
                Debuger.LogError(scenename+"场景未启动");
                return null;
            }
            return scene.m_Default;
        }
    }
    public class LoadSceneArg {
        public bool m_Isreturn;
        public string m_SceneName;
    }
}

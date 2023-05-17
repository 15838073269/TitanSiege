
using GF.Service;
using GF.Unity.AB;
using GF.Unity.Audio;
using GF.Unity.UI;
using System;
using System.Collections.Generic;
using Titansiege;
using UnityEngine;
using UnityEngine.UI;

namespace GF.MainGame.UI {
    public class SelectPlayerPage : UIPage {
        public Button m_GameStart;
        public Transform m_RoleInfoFather;
        public Button m_Delete;
        public Button AddButton;
        public Button AddButton1;
        public Button AddButton2;
        private const string RoleInfo = "UIPrefab/roleinfo.prefab";
        private List<RoleInfo> RoleLisdt = new List<RoleInfo>();
        private long currentid = -1;//当前选中角色的id
        List<CharactersData> templist = null;//当前账号下所有角色的数据
        public void Start() {
            m_GameStart.onClick.AddListener(() => { GameStartBtnClick(); });
            m_Delete.onClick.AddListener(DelBtnClick);
            AddButton.onClick.AddListener(CreateRole);
            AddButton1.onClick.AddListener(CreateRole);
            AddButton2.onClick.AddListener(CreateRole);
            AppTools.Regist<long>((int)CreateAndSelectEvent.Selected, Selected);

        }
        protected override void OnOpen(object args = null) {
            base.OnOpen(args);
            if (RoleLisdt.Count != 0) {
                for (int i = 0; i < RoleLisdt.Count; i++) {
                    Destroy(RoleLisdt[i].gameObject);
                }
                RoleLisdt.Clear();
                AddButton.interactable = true;
                AddButton1.interactable = true;
                AddButton2.interactable = true;
            }
            templist = UserService.GetInstance.m_UserModel.m_CharacterList;
            if (templist.Count == 0) {//如果数据读取错误，就强制退出重新登陆
                UIMsgBoxArg arg = new UIMsgBoxArg();
                arg.title = "重要提示";
                arg.content = "数据错误，请退出重新登陆！";
                arg.btnname = "关闭游戏";
                UIMsgBox box = UIManager.GetInstance.OpenWindow(AppConfig.UIMsgBox, arg) as UIMsgBox;
                box.oncloseevent += OnFailed;
            }
            ///按照设计最多只有三个角色
            for (int i = 0; i < templist.Count; i++) {
                GameObject go = null;
                if (i == 0) {
                    go = ObjectManager.GetInstance.InstanceObject(RoleInfo, father: AddButton.transform);
                    AddButton.interactable = false;
                } else if (i == 1) {
                    go = ObjectManager.GetInstance.InstanceObject(RoleInfo, father: AddButton1.transform);
                    AddButton1.interactable = false;
                } else if (i == 2) {
                    go = ObjectManager.GetInstance.InstanceObject(RoleInfo, father: AddButton2.transform);
                    AddButton2.interactable = false;
                }
                //go.transform.localPosition = Vector3.zero;
                RoleInfo r = go.GetComponent<RoleInfo>();
                r.InitInfo(templist[i].Name, templist[i].Zhiye, templist[i].Level, templist[i].ID);
                RoleLisdt.Add(r);
            }
            //这里应该默认加载上一次登陆的角色，还没做，先固定显示第一个角色吧
            Selected(templist[0].ID);
        }
        public void OnDestroy() {
            OnClose();
            AppTools.Remove<long>((int)CreateAndSelectEvent.Selected, Selected);
            for (int i = 0; i < RoleLisdt.Count; i++) {
                ObjectManager.GetInstance.ReleaseObject(RoleLisdt[i].gameObject,destoryCache:true,isrecycle:false);
            }
            RoleLisdt.Clear();
            templist.Clear();
        }
        //public void GameStart(PlayerData pd) {
        //    AppTools.GetModule<CreateAndSelectModule>(MDef.CreateAndSelectModule).GameStart(pd);
        //}
        protected override void OnClose(bool bClear = false, object arg = null) {
            base.OnClose(bClear, arg);
        }
       
        private void DelBtnClick() {
            AppTools.PlayBtnClick();
            if (currentid != -1) {
                CharactersData tempdata = GetCurrentCharData();
                UIMsgBoxArg arg = new UIMsgBoxArg();
                arg.title = "重要提示";
                arg.content = $"角色删除不可恢复，请确定要删除角色{tempdata.Name}";
                arg.btnname = "确定删除|取消";
                UIMsgBox box = UIManager.GetInstance.OpenWindow(AppConfig.UIMsgBox, arg) as UIMsgBox;
                box.oncloseevent += DelRole;
            }
        }
        /// <summary>
        /// 获取当前的角色数据
        /// </summary>
        /// <returns></returns>
        public CharactersData GetCurrentCharData() {
            for (int i = 0; i < templist.Count; i++) {
                if (templist[i].ID == currentid) {
                    return templist[i];
                }
            }
            return null;
        }
        /// <summary>
        /// 删除角色
        /// </summary>
        /// <param name="args"></param>
        private void DelRole(object args = null) {
            if (args == null) { //这种情况下都是点击X按钮过来的
                return;
            }
            ushort i = Convert.ToUInt16(args);
            if (i == 0) {//确定删除
                AppTools.Send<long>((int)CreateAndSelectEvent.DeletePlayer, currentid);
            } else if (i == 1) {//取消删除
                //取消暂时什么都不用做
            }
        }
        private void OnFailed(object args = null) {
            Application.Quit();
        }
        private void CreateRole() {
            AppTools.Send<bool>((int)CreateAndSelectEvent.ShowWind, false);
        }
        /// <summary>
        /// 显示哪个角色
        /// </summary>
        /// <param name="roleid">角色框的id</param>
        private void Selected(long id) {
            AudioService.GetInstance.PlayUIMusic("audio/xuanzhong.wav");
            for (int i = 0; i < RoleLisdt.Count; i++) {
                if (RoleLisdt[i].m_CharacterId == id) {
                    RoleLisdt[i].ShowOrHiden(true);
                    currentid = id;
                } else {
                    RoleLisdt[i].ShowOrHiden(false);
                }
            }
        }

        private void GameStartBtnClick() {
            AppTools.PlayBtnClick();
            if (currentid != -1) {
                UserService.GetInstance.m_CurrentChar = GetCurrentCharData();
                AppTools.Send((int)CreateAndSelectEvent.GameStart);
            } else {
                Debuger.Log("没有选择任何角色！");
            }
            
        }
       
    }
}

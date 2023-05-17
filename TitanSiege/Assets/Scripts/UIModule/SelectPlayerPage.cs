
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
        private long currentid = -1;//��ǰѡ�н�ɫ��id
        List<CharactersData> templist = null;//��ǰ�˺������н�ɫ������
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
            if (templist.Count == 0) {//������ݶ�ȡ���󣬾�ǿ���˳����µ�½
                UIMsgBoxArg arg = new UIMsgBoxArg();
                arg.title = "��Ҫ��ʾ";
                arg.content = "���ݴ������˳����µ�½��";
                arg.btnname = "�ر���Ϸ";
                UIMsgBox box = UIManager.GetInstance.OpenWindow(AppConfig.UIMsgBox, arg) as UIMsgBox;
                box.oncloseevent += OnFailed;
            }
            ///����������ֻ��������ɫ
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
            //����Ӧ��Ĭ�ϼ�����һ�ε�½�Ľ�ɫ����û�����ȹ̶���ʾ��һ����ɫ��
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
                arg.title = "��Ҫ��ʾ";
                arg.content = $"��ɫɾ�����ɻָ�����ȷ��Ҫɾ����ɫ{tempdata.Name}";
                arg.btnname = "ȷ��ɾ��|ȡ��";
                UIMsgBox box = UIManager.GetInstance.OpenWindow(AppConfig.UIMsgBox, arg) as UIMsgBox;
                box.oncloseevent += DelRole;
            }
        }
        /// <summary>
        /// ��ȡ��ǰ�Ľ�ɫ����
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
        /// ɾ����ɫ
        /// </summary>
        /// <param name="args"></param>
        private void DelRole(object args = null) {
            if (args == null) { //��������¶��ǵ��X��ť������
                return;
            }
            ushort i = Convert.ToUInt16(args);
            if (i == 0) {//ȷ��ɾ��
                AppTools.Send<long>((int)CreateAndSelectEvent.DeletePlayer, currentid);
            } else if (i == 1) {//ȡ��ɾ��
                //ȡ����ʱʲô��������
            }
        }
        private void OnFailed(object args = null) {
            Application.Quit();
        }
        private void CreateRole() {
            AppTools.Send<bool>((int)CreateAndSelectEvent.ShowWind, false);
        }
        /// <summary>
        /// ��ʾ�ĸ���ɫ
        /// </summary>
        /// <param name="roleid">��ɫ���id</param>
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
                Debuger.Log("û��ѡ���κν�ɫ��");
            }
            
        }
       
    }
}

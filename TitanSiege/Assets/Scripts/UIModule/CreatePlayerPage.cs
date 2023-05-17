using DG.Tweening;
using GF.ConfigTable;
using GF.MainGame.Data;
using GF.MathLite;
using GF.Service;
using GF.Unity.Audio;
using GF.Unity.UI;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
namespace GF.MainGame.UI {
    public class CreatePlayerPage : UIPage {
        /// <summary>
        /// ��ʿ
        /// </summary>
        public Image jianshi;
        public Image jianshiselect;
        public Image jianshidi;
        public Text jianshitext;
        /// <summary>
        /// ����
        /// </summary>
        public Image jingling;
        public Image jinglingselect;
        public Image jinglingdi;
        public Text jinglingtext;
        /// <summary>
        /// longnv
        /// </summary>
        public Image longnv;
        public Image longnvselect;
        public Image longnvdi;
        public Text longnvtext;
       
        public Text jianshicontent;
        public Text jinglingcontent;
        public Text longnvcontent;

        public Button createplayer;
        public Button returnbtn;
        public InputField createname;
        public Button randbtn;
        public Text notice;

        private ushort currentid;
        private NameData m_AllNames;
        public void Start() {
            AddUIClickListener(jianshi, () => ManSelected(0));
            AddUIClickListener(jingling, () => ManSelected(1));
            AddUIClickListener(longnv, () => ManSelected(2));
            AddUIClickListener(jianshiselect, () => ManSelected(0));
            AddUIClickListener(jinglingselect, () => ManSelected(1));
            AddUIClickListener(longnvselect, () => ManSelected(2));
            randbtn.onClick.AddListener(RandName);
            createplayer.onClick.AddListener(CreatePlayer);
            returnbtn.onClick.AddListener(ReturnSelect);
            initName();
        }

        private void ReturnSelect() {
            if (UserService.GetInstance.m_UserModel.m_CharacterList.Count > 0) {
                AppTools.Send<bool>((int)CreateAndSelectEvent.ShowWind,true);
            } else {
                UIManager.GetInstance.OpenUIWidget(AppConfig.UIMsgTips,"��û���κ���Ϸ��ɫ���޷����أ�");
            }
        }

        protected override void OnOpen(object args = null) {
            base.OnOpen(args);
            ManSelected(0);
        }
        /// <summary>
        /// ���ý�ɫ
        /// </summary>
        /// <param name="playerid"></param>
        private Color c1 = new Color(1f, 1f, 1f, 1f);
        private Color c2 = new Color(82f / 255f, 82f / 255f, 82f / 255f, 1f);
        private Color c3 = new Color(1f, 1f, 1f, 0.2f);
        public void ManSelected(ushort playerid) {
            AudioService.GetInstance.PlayUIMusic("audio/xuanzhong.wav");
            currentid = playerid;
            if (playerid == 0) {
                jianshi.color = c1;
                jianshiselect.color = c1;
                jianshidi.color = c1;
                jianshitext.color = c1;

                jingling.color = c2;
                jinglingselect.color = c2;
                jinglingdi.color = c2;
                jinglingtext.color = c3;

                longnv.color = c2;
                longnvselect.color = c2;
                longnvdi.color = c2;
                longnvtext.color = c3;

                jianshicontent.gameObject.SetActive(true);
                longnvcontent.gameObject.SetActive(false);
                jinglingcontent.gameObject.SetActive(false);
            } else if (playerid == 1) {
                jingling.color = c1;
                jinglingselect.color = c1;
                jinglingdi.color = c1;
                jinglingtext.color = c1;

                jianshi.color = c2;
                jianshiselect.color = c2;
                jianshidi.color = c2;
                jianshitext.color = c3;

                longnv.color = c2;
                longnvselect.color = c2;
                longnvdi.color = c2;
                longnvtext.color = c3;

                jianshicontent.gameObject.SetActive(false);
                longnvcontent.gameObject.SetActive(false);
                jinglingcontent.gameObject.SetActive(true);
            } else if (playerid == 2) {
                longnv.color = c1;
                longnvselect.color = c1;
                longnvdi.color = c1;
                longnvtext.color = c1;

                jingling.color = c2;
                jinglingselect.color = c2;
                jinglingdi.color = c2;
                jinglingtext.color = c3;

                jianshi.color = c2;
                jianshiselect.color = c2;
                jianshidi.color = c2;
                jianshitext.color = c3;

                jianshicontent.gameObject.SetActive(false);
                longnvcontent.gameObject.SetActive(true);
                jinglingcontent.gameObject.SetActive(false);
            }
            //��ʾģ��
            AppTools.Send<ushort>((int)CreateAndSelectEvent.ShowModel, playerid);
        }

        public void OnDestroy() {
            OnClose();
        }
        public void CreatePlayer() {
            AppTools.PlayBtnClick();
            //��ȡ�û����͵�ǰѡ��Ľ�ɫ,���͸�����ģ��
            string cname = createname.text;
            createplayer.interactable = false;//��ֹ���翨�ٵ��µ��ظ����
            if (string.IsNullOrEmpty(cname)) {
                ShowNotice("��ɫ���Ʋ���Ϊ�գ�");
                createplayer.interactable = true;
            } else {
                AppTools.Send<string, ushort>((int)CreateAndSelectEvent.CreatePlayer, cname,currentid);
                createplayer.interactable = true;
            }
        }
        private void initName() {
            m_AllNames = ConfigerManager.GetInstance.FindData<NameData>(CT.TABLE_NAME);
        }
        public void RandName() {
            AudioService.GetInstance.PlayUIMusic("audio/xuanzhong.wav");
            int i = GFRandom.Default.Range(0, m_AllNames.AllNames.Count);
            createname.text = m_AllNames.AllNames[i].Cname;
        }
        protected override void OnEnable() {
            base.OnEnable();
            createplayer.interactable = true;
        }
        public void ShowNotice(string msg) {
            notice.gameObject.SetActive(true);
            notice.text = msg;
            notice.transform.DOShakePosition(1, new Vector3(6, 6, 0), 8, 45, true);
        }
        protected override void OnClose(bool bClear = false, object arg = null) {
            base.OnClose(bClear, arg);
        }
    }
}


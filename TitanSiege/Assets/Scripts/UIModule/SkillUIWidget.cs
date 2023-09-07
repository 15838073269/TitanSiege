/****************************************************
    文件：SkillUIWidget.cs
	作者：昔莘
    邮箱: 304183153@qq.com
    日期：2022/11/14 1:37:29
	功能：Nothing
*****************************************************/

using GF.ConfigTable;
using GF.Const;
using GF.MainGame.Data;
using GF.MainGame.Module.NPC;
using GF.Service;
using GF.Unity.AB;
using GF.Unity.UI;
using UnityEngine;
using UnityEngine.UI;

namespace GF.MainGame.UI {
    public class SkillUIWidget : UIWidget {
        public Button m_CommBtn;
        public Button m_Skill1;
        public Button m_Skill2;
        public Button m_Skill3;
        public Button m_Skill4;
        public  void Start() {
            ///此处的01234只是为了标记他属于哪个位置，不是技能id
            m_CommBtn.onClick.AddListener(() => { SkillClick(0); });
            m_Skill1.onClick.AddListener(() => { SkillClick(1); });
            m_Skill2.onClick.AddListener(() => { SkillClick(2); });
            m_Skill3.onClick.AddListener(() => { SkillClick(3); });
            m_Skill4.onClick.AddListener(() => { SkillClick(4); });
            //获取当前角色id，创建技能图标
            //技能添加的顺序，决定角色技能在面板上的位置，第一个永远是普通攻击，第二个永远是第一个技能，以此类推
            if (UserService.GetInstance.m_CurrentChar.Skills!="") {
                string[] strarr = UserService.GetInstance.m_CurrentChar.Skills.Split('|');
                for(int i = 0;i<strarr.Length;i++) {
                    if (!string.IsNullOrEmpty(strarr[i])) { //数据库存储最后一个字符时“|”,所以会出现一个空白项
                        int tempid = int.Parse(strarr[i]);
                        SkillDataBase sb = ConfigerManager.m_SkillData.FindSkillByID(tempid);
                        SkillUIInfo skui = GetSkillUIInfo(i);
                        skui.Init(sb);
                    }
                }
            }
            
        }
        protected override void OnOpen(object args = null) {
            base.OnOpen(args);
        }
        public override void Close(bool bClear = false, object arg = null) {
            base.Close(bClear, arg);
        }

        protected override void OnClose(bool bClear = false, object arg = null) {
            base.OnClose(bClear, arg);
           
        }

        /// <summary>
        /// 点击技能按钮调用方法
        /// </summary>
        /// <param name="i">哪个位置，不是技能id</param>
        void SkillClick(int i) {
            SkillUIInfo sinfo = GetSkillUIInfo(i);
            bool isskill = AppTools.SendReturn<bool>((int)MoveEvent.IsPlaySkill);
            if (!isskill) {
                sinfo.SetCD();
                AppTools.Send<int>((int)SkillEvent.ClickSkill, i);
            }
        }
        /// <summary>
        /// 获取技能模块
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        private SkillUIInfo GetSkillUIInfo(int i) {
            SkillUIInfo sinfo = null;
            switch (i) {
                case 0:
                    sinfo = m_CommBtn.GetComponent<SkillUIInfo>();
                    break;
                case 1:
                    sinfo = m_Skill1.GetComponent<SkillUIInfo>();
                    break;
                case 2:
                    sinfo = m_Skill2.GetComponent<SkillUIInfo>();
                    break;
                case 3:
                    sinfo = m_Skill3.GetComponent<SkillUIInfo>();
                    break;
                case 4:
                    sinfo = m_Skill4.GetComponent<SkillUIInfo>();
                    break;
                default:
                    Debuger.LogError("非法参数！");
                    break;
            }
            return sinfo;
        }
        /// <summary>
        /// 角色升级时执行一下本函数
        /// 处理手段有点傻，不过先这样了，也能用
        /// </summary>
        public void JiesuoSkill(ushort level) {
            GetSkillUIInfo(1).XuqiuDengji(level);
            GetSkillUIInfo(2).XuqiuDengji(level);
            GetSkillUIInfo(3).XuqiuDengji(level);
            GetSkillUIInfo(4).XuqiuDengji(level);
        }
        public void Update() {
            if (Input.GetKeyDown(KeyCode.Q)) {
                SkillClick(1);
            }
        }
    }
}

/****************************************************
    文件：CreateAndSelectModule.cs
	作者：昔莘
    邮箱: 304183153@qq.com
    日期：2021/6/19 23:22:11
	功能：角色创建和选择模块
*****************************************************/

using UnityEngine;
using GF.Module;
using GF.Unity.UI;
using System.Collections.Generic;
using GF.ConfigTable;
using GF.MainGame.Data;
using GF.MainGame.UI;
using UnityEngine.TextCore.Text;
using GF.MainGame.Module.NPC;
using GF.Service;
using Net.Share;
using Net.Component;
using System.Text;

namespace GF.MainGame.Module {
    public class CreateAndSelectModule : GeneralModule {
        private CreatePlayerPage cpp = null;//创建角色ui页面
        private SelectPlayerPage spp = null;//选择角色界面
        /// <summary>
        /// 创建模块
        /// </summary>
        /// <param name="args">参数</param>
        public override void Create() {
            AppTools.Regist<ushort>((int)CreateAndSelectEvent.ShowModel,ShowModel);
            AppTools.Regist<bool>((int)CreateAndSelectEvent.ShowWind, ShowWind);
            AppTools.Regist<string,ushort>((int)CreateAndSelectEvent.CreatePlayer, CreatePlayer);
            AppTools.Regist((int)CreateAndSelectEvent.GameStart, GameStart);
            AppTools.Regist<long>((int)CreateAndSelectEvent.DeletePlayer, DeletePlayer);
            ConfigerManager.GetInstance.LoadData<NameData>(CT.TABLE_NAME); //初始化名称数据,不常用数据，所以不在配置管理器中加载
            InitModel();
            ClientManager.Instance.client.AddRpc(this);
        }
        public List<CreateroleNPC> m_NpcModelList;
        /// <summary>
        /// 显示某个模型
        /// </summary>
        /// <param name="npc"></param>
        public void ShowModel(ushort npcid) {
            for (int i = 0; i < m_NpcModelList.Count; i++) {
                if (m_NpcModelList[i].m_Id == npcid) {
                    m_NpcModelList[i].gameObject.SetActive(true);
                    m_NpcModelList[i].PlayPose();
                    m_NpcModelList[i].SetRotationZero();
                } else {
                    m_NpcModelList[i].gameObject.SetActive(false);
                }
            }
        }
        /// <summary>
        /// 初始化所有模型/职业
        /// </summary>
        private void InitModel() {
            m_NpcModelList = new List<CreateroleNPC>();
            GameObject[] allgo = GameObject.FindGameObjectsWithTag("createmodel");
            for (int i = 0; i < allgo.Length; i++) {
                CreateroleNPC npctemp = allgo[i].GetComponent<CreateroleNPC>();
                npctemp.gameObject.SetActive(false);
                m_NpcModelList.Add(npctemp);
            }
        }
        /// <summary>
        /// 显示UI
        /// </summary>
        /// <param name="arg">参数</param>
        public void ShowWind(bool ishasplayer) {
            
            if (!ishasplayer) {
                //显示创建角色界面
                ShowModel(0);//默认载入剑士的id
                cpp = UIManager.GetInstance.OpenPage(AppConfig.CreatePlayerPage) as CreatePlayerPage;
            } else {
                //显示选择角色界面
                //打开UI
                spp = UIManager.GetInstance.OpenPage(AppConfig.SelectPlayerPage) as SelectPlayerPage;
            }
        }
        /// <summary>
        /// 创建角色按钮点击后的处理
        /// </summary>
        public void CreatePlayer(string createname, ushort playerid) {
            CharactersData data = null;
            //从配置表获取数据
            NPCDataBaseToPlayerData(ConfigerManager.m_NPCData.FindNPCByID(playerid), out data);
            data.Uid = UserService.GetInstance.m_UserModel.m_Users.ID;
           
            //发送服务器创建角色
            ClientManager.Instance.SendRT("CreateRole",data);
        }
        /// <summary>
        /// 服务器返回创建成功操作
        /// </summary>
        [Rpc]
        public void CreateRoleCallBack(bool state, string msg,CharactersData data) {
            if (state) {
                //创建完成后，进入角色选择界面
                UserService.GetInstance.m_UserModel.m_CharacterList.Add(data);
                UIManager.GetInstance.OpenUIWidget(AppConfig.UIMsgTips, "创建角色成功！");
                ShowWind(true);
            } else {
                //提示出错
                cpp.ShowNotice(msg);
            }
        }
        public void NPCDataBaseToPlayerData(NPCDataBase nb, out CharactersData data) {
            data = new CharactersData();
            data.Name = cpp.createname.text;
            data.Level = (sbyte)nb.Level;
            data.Zhiye = (sbyte)nb.Zhiye;
            data.Exp =nb.Exp;
            data.Shengming = (short)nb.Shengming;
            data.Fali = (short)nb.Fali;
            data.Tizhi = (short)nb.Tizhi;
            data.Liliang = (short)nb.Liliang;
            data.Minjie = (short)nb.Minjie;
            data.Moli = (short)nb.Moli;
            data.Meili = (short)nb.Meili;
            data.Xingyun = (short)nb.Xingyun;
            data.Lianjin = (short)nb.Lianjin;
            data.Duanzao = (short)nb.Duanzao;
            data.Jinbi = nb.Jinbi;
            data.Zuanshi = nb.Zuanshi;
            data.Chenghao = nb.Chenghao;
            StringBuilder friendstr = new StringBuilder();
            if (nb.Friends.Count > 0) {
                for (int i = 0; i < nb.Friends.Count; i++) {
                    Debuger.Log(nb.Friends[i]);
                    friendstr.Append(nb.Friends[i]);
                    friendstr.Append("|");
                }
            }
            data.Friends = friendstr.ToString();
            StringBuilder skillstr = new StringBuilder();
            if (nb.Skills.Count>0) {
                for (int i = 0; i < nb.Skills.Count; i++) {
                    skillstr.Append(nb.Skills[i]);
                    skillstr.Append("|");
                }
            }
            data.Skills = skillstr.ToString();
            data.Prefabpath = nb.Prefabpath;
            data.Headpath = nb.Headpath;
            data.Lihuipath = nb.Lihuipath;
        }
        public override void Release() {
            base.Release();
            //清除随机名称配置表
            ConfigerManager.GetInstance.DelData(CT.TABLE_NAME);
        }

        public void CreateTip(string msg) {
            cpp.ShowNotice(msg);
        }
        public void GameStart() {
            
            //加载游戏主城场景
            UILoadingArg arg = new UILoadingArg();
            arg.tips = "正在加载神话世界....";
            GameSceneService.GetInstance.AsyncLoadScene(AppConfig.MainScene,true, arg);
        }
        public void DeletePlayer(long delid) {
            ClientManager.Instance.SendRT("DeletePlayer", delid);
        }
        [Rpc]
        public void DeletePlayerCallBack(bool state, string msg,long delid) {
            if (state) {
                //删除完成后，重新进入角色选择界面或者创建角色界面
                List<CharactersData> templist = UserService.GetInstance.m_UserModel.m_CharacterList;
                for (int i = 0; i < templist.Count; i++) {
                    if (templist[i].ID == delid) {
                        templist.Remove(templist[i]);
                        break;
                    }
                }
                if (templist.Count > 0) {
                    ShowWind(true);
                } else {
                    ShowWind(false);
                }
            }
            //处理UI，打开登录界面
            UIManager.GetInstance.OpenUIWidget(AppConfig.UIMsgTips, "删除角色成功！");
            
        }
    }
}

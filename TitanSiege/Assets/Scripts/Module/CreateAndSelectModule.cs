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
using Titansiege;
using Net.Client;
using cmd;

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
        public  void CreatePlayer(string createname, ushort playerid) {
            CharactersData data = null;
            //从配置表获取数据
            NPCDataBaseToPlayerData(ConfigerManager.m_NPCData.FindNPCByID(playerid), out data);
            data.Uid = UserService.GetInstance.m_UserModel.m_Users.ID;
           
            //发送服务器创建角色
            ClientManager.Instance.SendRT("CreateRole",data);
            ////上面这种需要服务端回复的，其实用sendrt不好，，也能用，效率低些，应该用call，懒得重新写了，把call方法留在下面参考吧
            //RPCModelTask task = await ClientBase.Instance.Call((ushort)1,data);
            //因为用到unitask，所以方法得加上async UniTaskVoid关键字,添加关键字后，在添加onclick事件时不能直接添加，需要用匿名方法添加，例如()=>_ = DeletePlayer(long delid)
            ////callcall是服务器处理, 客户端响应结果  sendrt是只发送不响应, 可以用于聊天, 不需要关心响应,  addoperation是场景同步, 实时同步
            //if (!task.IsCompleted) {
            //    Debuger.LogError("超时");
            //    return;
            //}
            //int code = task.model.AsInt;//获取服务端发回的状态码
            //CharactersData ca = task.model.As<CharactersData>();//获取服务端返回的数据
            //switch (code) {
            //    default:
            //        break;
            //}
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
            if (UserService.GetInstance.m_CurrentChar.Zhiye != 0) {
                UIManager.GetInstance.OpenUIWidget(AppConfig.UIMsgTips, "该职业正在开发中，敬请期待!");
                return;
            }
            //先把选择的角色数据id传给服务器，让服务器知道选择的哪个角色
            ClientBase.Instance.SendRT((ushort)ProtoType.selectcharacter,UserService.GetInstance.m_CurrentChar.ID);
            //加载游戏主城场景
            UILoadingArg arg = new UILoadingArg();
            arg.tips = "正在加载神话世界....";
            GameSceneService.GetInstance.AsyncLoadScene(AppConfig.MainScene,true, arg);
        }
        public void DeletePlayer(long delid) {
            ClientManager.Instance.SendRT("DeletePlayer", delid);
            ////上面这种需要服务端回复的，其实用sendrt不好，，也能用，效率低些，应该用call，懒得重新写了，把call方法留在下面参考吧
            //RPCModelTask task = await ClientBase.Instance.Call((ushort)1,data);
            //因为用到unitask，所以方法得加上async UniTaskVoid关键字,添加关键字后，在添加onclick事件时不能直接添加，需要用匿名方法添加，例如()=>_ = DeletePlayer(long delid)
            ////callcall是服务器处理, 客户端响应结果  sendrt是只发送不响应, 可以用于聊天, 不需要关心响应,  addoperation是场景同步, 实时同步
            //if (!task.IsCompleted) {
            //    Debuger.LogError("超时");
            //    return;
            //}
            //int code = task.model.AsInt;//获取服务端发回的状态码
            //CharactersData ca = task.model.As<CharactersData>();//获取服务端返回的数据
            //switch (code) {
            //    default:
            //        break;
            //}
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

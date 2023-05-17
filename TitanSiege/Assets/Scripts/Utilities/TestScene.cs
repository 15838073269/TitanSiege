/****************************************************
    文件：TestScene.cs
	作者：昔莘
    邮箱: 304183153@qq.com
    日期：2022/11/4 13:43:18
	功能：Nothing
*****************************************************/

using GF;
using GF.ConfigTable;
using GF.MainGame;
using GF.MainGame.Data;
using GF.Service;
using GF.Unity.UI;
using Net.Component;
using Net.Config;
using System.Text;
using Titansiege;
using UnityEngine;

public class TestScene {
    public TestScene() {
        CreatePlayer("天海无双",0);
    }
    public void CreatePlayer(string createname, ushort playerid) {
        CharactersData data = null;
        //从配置表获取数据
        NPCDataBaseToPlayerData(ConfigerManager.m_NPCData.FindNPCByID(playerid), out data, createname);
        data.Uid =1;
        data.ID = 1;
        //创建完成后，进入角色选择界面
        UserService.GetInstance.m_UserModel.m_CharacterList.Add(data);
        UserService.GetInstance.m_CurrentChar = data;
        UserService.GetInstance.m_CurrentID = playerid;
    }
    public void NPCDataBaseToPlayerData(NPCDataBase nb, out CharactersData data,string name) {
        data = new CharactersData();
        data.Name = name;
        data.Level = (sbyte)nb.Level;
        data.Zhiye = (sbyte)nb.Zhiye;
        data.Exp = nb.Exp;
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
        if (nb.Skills.Count > 0) {
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
}
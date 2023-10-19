using GDServer.Tools;
using MySqlX.XDevAPI;
using Net.Event;
using Net.Share;
using System;
using System.Data;
using System.Diagnostics;
using Titansiege;

//用户登录注册的RPC处理
namespace GDServer.Services
{
    internal class UserService:Singleton<UserService> {
        // [Rpc(cmd = NetCmd.SafeCall)]//使用SafeCall指令后, 第一个参数插入客户端对象, 这个客户端对象就是哪个客户端发送,这个参数就是对应那个客户端的对象
        public bool Login(GDClient client, string account, string password) {
            Debuger.Log($"UserLoginRequest: User:{account}  Pass:{password}");
            string sql = $"select * from users where Username = {account}";
            DataTable dt = MySqlHelper.ExecuteQuery(sql);
            if (dt == null) {
                TServer.Instance.SendRT(client, "LoginFailCallback", false, "用户不存在!");
                return false;
            } else if (dt.Rows.Count == 0) {
                TServer.Instance.SendRT(client, "LoginFailCallback", false, "用户不存在!");
                return false;
            } else {
                if (dt.Rows[0]["Password"].ToString() != password) {
                    TServer.Instance.SendRT(client, "LoginFailCallback", false, "密码错误!");
                    return false;
                } else {
                    long Uid = Convert.ToInt64(dt.Rows[0]["ID"]);
                    if (Uid != -1) {
                        string sql1 = $"select * from characters where Uid = {Uid} and DelRole = 0";
                        DataTable Pdt = MySqlHelper.ExecuteQuery(sql1);
                        if (Pdt.Rows.Count != 0) {
                            for (int i = 0; i < Pdt.Rows.Count; i++) {
                                CharactersData cd = new CharactersData();
                                cd.Init(Pdt.Rows[i]);
                                client.CharacterList.Add(cd);
                            }

                        }
                    }
                    client.PlayerID = account;//这个唯一标识用来判断是不是一个账号，一般用来挤号登录，用账号作为身份唯一标识，需保证账号名不重复，
                    client.User.Init(dt.Rows[0]);
                    TServer.Instance.SendRT(client, "LoginCallback", true, "登录成功!", client.CharacterList, client.User);
                    return true;

                }
            }
        }
        /// <summary>
        /// 当客户端注册的时候, 我们应该检查数据库 账号是否存在, 如果不存在, 则可以注册新的账号, 反则应该提示客户端注册失败
        /// </summary>
        public void Register(GDClient client, string account, string password, string email) {
            Debuger.Log($"UserRegisterRequest: User:{account}  Pass:{password}  Email:{email}");
            string sql = $"select * from users where Username = {account} or Email =  {email}";
            DataTable dt = MySqlHelper.ExecuteQuery(sql);
            if (dt.Rows.Count != 0) {
                TServer.Instance.SendRT(client, "RegisterCallback", false, "用户名或者邮箱已存在!");
                return;
            } else {
                int id = TitansiegeDB.I.GetConfigID(ConfigType.Users);//这个其实就是为了让id自增的，为了不想重写登录注册，所以才手动调用一下
                string sqlinsert = $"insert into users (ID,Username,Password,Email,RegisterDate) values({id},{account},{password},{email},'{TimeUtil.GetCstDateTime().ToString()}')";
                int countinsert = MySqlHelper.ExecuteNonQuery(sqlinsert);
                if (countinsert == 1) {
                    TServer.Instance.SendRT(client, "RegisterCallback", true, "注册成功！");
                } else {
                    TServer.Instance.SendRT(client, "RegisterCallback", false, "注册发生错误，请稍后重试！");
                    Debuger.LogError($"发生插入错误，错误sql：{sqlinsert}");
                }
            }
        }
        [Rpc(cmd = NetCmd.SafeCall)]
        private void CreateRole(GDClient client, CharactersData data) {
            uint length = 0;
            string sqldataname = data.GetCellNameAndTextLength(1,out length);
            CharactersData chongming = CharactersData.Query($"{sqldataname} = '{data[1]}'");
            if (chongming == default) {
                data.ID = TitansiegeDB.I.GetConfigID(ConfigType.CharacterData);
                data.LastDate = TimeUtil.GetCstDateTime();//linux和win下datetime获取时间不一样，需要用这个方法保持一致
                data.NewTableRow();
                TitansiegeDB.I.m_CharactersData.Add(data.ID, data);
                client.CharacterList?.Add(data);
                TServer.Instance.SendRT(client, "CreateRoleCallBack", true, "创建成功！", data);
                //string sql = $"insert into Characters ({data.GetCellName(1)},{data.GetCellName(2)},{data.GetCellName(3)},{data.GetCellName(4)},{data.GetCellName(5)},{data.GetCellName(6)},{data.GetCellName(7)},{data.GetCellName(8)},{data.GetCellName(9)},{data.GetCellName(10)},{data.GetCellName(11)},{data.GetCellName(12)},{data.GetCellName(13)},{data.GetCellName(14)},{data.GetCellName(15)},{data.GetCellName(16)},{data.GetCellName(17)}) values ('{data[1]}',{data[2]},{data[3]},{data[4]},{data[5]},{data[6]},{data[7]},{data[8]},{data[9]},{data[10]},{data[11]},{data[12]},{data[13]},{data[14]},{data[15]},{data[16]},'{data[17]}')";
                //int i = MySqlHelper.ExecuteNonQuery(sql);
                //if (i != 1) {
                //    client.CharacterList.Add(data);
                //    SendRT(client, "CreateRoleCallBack", true, "创建成功！");
                //} else {
                //    SendRT(client, "CreateRoleCallBack", false, "创建角色失败，请稍后再试！");
                //    Debuger.LogError($"发生插入错误，错误sql：{sql}");
                //}
            } else {
                TServer.Instance.SendRT(client, "CreateRoleCallBack", false, "名称重复！", data);
            }
        }
        [Rpc(cmd = NetCmd.SafeCall)]
        private void DeletePlayer(GDClient client, long dataid) {
            CharactersData cdata = null;
            if (TitansiegeDB.I.m_CharactersData.TryGetValue(dataid, out cdata) && cdata != null) {
                cdata.DelRole = true;//将标志改为删除
                TServer.Instance.SendRT(client, "DeletePlayerCallBack", true, "删除成功！", dataid);
            }
        }

        /// <summary>
        /// 解决表的唯一id碰撞问题
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        //internal long GetCharacterID() {
        //    long maxid = 1;
        //    lock (this) {
        //        if (TServer.m_DB.m_CharactersData.Count > 0) {
        //            maxid = TServer.m_DB.m_CharactersData.Keys.Last();
        //            maxid += 1;
        //        }
        //    }
        //    return maxid;
        //}
    
        [Rpc(cmd = NetCmd.SafeCall)]
        public void SwitchScene(GDClient client, string enterscenename) {
            //TServer.Instance.ExitScene(client,false);
            GDScene gs = TServer.Instance.JoinScene(client, enterscenename);//退出当前场景，加入目标场景
            Debuger.Log(gs);
            if (gs == null) {
                TServer.Instance.SendRT(client, "SwitchSceneCallBack", false, enterscenename);
            } else {
                TServer.Instance.SendRT(client, "SwitchSceneCallBack", true, enterscenename);
            }
        }
        protected override void InitSingleton() {
            base.InitSingleton();
            TServer.Instance.AddRpc(this);

        }


        /// <summary>
        /// 处理玩家数据，m1永远为需要装备的，m2永远为需要卸下的
        /// </summary>
        /// <param name="m1">需要装备的道具的效果字典</param>
        /// <param name="m2">需要卸下的道具效果字典</param>
        public void UpdateCharacterData(CharactersData cd, FightProp fp,GDClient client,Dictionary<XiaoGuo, Dictionary<string, EffArgs>> m1 = null, Dictionary<XiaoGuo, Dictionary<string, EffArgs>> m2 = null) {
            //减去旧的数据
            if (m2 != null) {
                foreach (KeyValuePair<XiaoGuo, Dictionary<string, EffArgs>> xiaoguo in m2) {
                    if (xiaoguo.Value.Count > 0) {
                        foreach (KeyValuePair<string, EffArgs> xg in xiaoguo.Value) {
                            switch (xg.Key) {
                                case "liliang":
                                    cd.Liliang -= (short)xg.Value.ivalue;
                                    break;
                                case "tizhi":
                                    cd.Tizhi -= (short)xg.Value.ivalue;
                                    break;
                                case "minjie":
                                    cd.Minjie -= (short)xg.Value.ivalue;
                                    break;
                                case "moli":
                                    cd.Moli -= (short)xg.Value.ivalue;
                                    break;
                                case "xingyun":
                                    cd.Xingyun -= (short)xg.Value.ivalue;
                                    break;
                                case "meili":
                                    cd.Meili -= (short)xg.Value.ivalue;
                                    break;
                                case "maxhp":
                                    //基础生命去加减
                                    cd.Shengming -= (short)xg.Value.ivalue;
                                    fp.FightHP -= (short)xg.Value.ivalue;
                                    if (fp.FightHP <= 0) {
                                        fp.FightHP = 1;
                                    }
                                    break;
                                case "maxmagic":
                                    //基础法力去加减
                                    cd.Fali -= (short)xg.Value.ivalue;
                                    fp.FightMagic -= (short)xg.Value.ivalue;
                                    if (fp.FightMagic < 0) {
                                        fp.FightMagic = 0;
                                    }
                                    break;

                                case "lianjin":
                                    cd.Lianjin -= (short)xg.Value.ivalue;
                                    break;
                                case "duanzao":
                                    cd.Duanzao -= (short)xg.Value.ivalue;
                                    break;
                                
                            }
                        }
                    }
                }
            }
            //加上新的数据
            if (m1 != null) {
                foreach (KeyValuePair<XiaoGuo, Dictionary<string, EffArgs>> xiaoguo in m1) {
                    if (xiaoguo.Value.Count > 0) {
                        foreach (KeyValuePair<string, EffArgs> xg in xiaoguo.Value) {
                            switch (xg.Key) {
                                case "liliang":
                                    cd.Liliang += (short)xg.Value.ivalue;
                                    break;
                                case "tizhi":
                                    cd.Tizhi += (short)xg.Value.ivalue;
                                    break;
                                case "minjie":
                                    cd.Minjie += (short)xg.Value.ivalue;
                                    break;
                                case "moli":
                                    cd.Moli += (short)xg.Value.ivalue;
                                    break;
                                case "xingyun":
                                    cd.Xingyun += (short)xg.Value.ivalue;
                                    break;
                                case "meili":
                                    cd.Meili += (short)xg.Value.ivalue;
                                    break;
                                case "maxhp":
                                    cd.Shengming += (short)xg.Value.ivalue;
                                    fp.FightHP += (short)xg.Value.ivalue;
                                    break;
                                case "maxmagic":
                                    cd.Fali += (short)xg.Value.ivalue;
                                    fp.FightMagic += (short)xg.Value.ivalue;
                                    break;
                                case "lianjin":
                                    cd.Lianjin += (short)xg.Value.ivalue;
                                    break;
                                case "duanzao":
                                    cd.Duanzao += (short)xg.Value.ivalue;
                                    break;
                               
                            }
                        }
                    }
                }
            }
            //添加完属性，更新一下运行时属性,服务端更新运行时属性时，会重复将运行时的装备属性添加上去
            client.UpdateFightProps();
            
        }
    }
    public class EffArgs {
        public string effname;//效果或者需求名称,用来匹配
        public float fvalue;//flaot的效果值
        public int ivalue;//int类型的效果至
    }
}

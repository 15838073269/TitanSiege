using GDServer.Tools;
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
                    client.PlayerID = account;
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
                Debuger.Log(data.ID);
                data.LastDate = TimeUtil.GetCstDateTime();//linux和win下datetime获取时间不一样，需要用这个方法保持一致
                data.NewTableRow();
                TitansiegeDB.I.m_CharactersData.Add(data.ID, data);
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
        /// <summary>
        /// 客户端主动发起退出登录请求
        /// </summary>
        /// <param name="client"></param>
        [Rpc(NetCmd.SafeCall)]
        private void LogOut(GDClient client) {
            TServer.Instance.SendRT(client, "LogOut");//在客户端热更新工程的MsgPanel类找到
            TServer.Instance.SignOut(client);
        }
        [Rpc(cmd = NetCmd.SafeCall)]
        private void SwitchScene(GDClient client, string enterscenename) {
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
    }
}

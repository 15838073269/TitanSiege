
using Net.Server;
using Net.Share;
using Net.System;
using Net.Event;
using Titansiege;
using Net.MMORPG;
using cmd;

//文件最好与服务器端VisualStudio项目的MyCommand.cs同名且代码一样
namespace GDServer.Services
{
    public class TServer : TcpServer<GDClient, GDScene>//服务器类
    {
        /// <summary>
        /// 当开始服务器的时候
        /// </summary>
        /// 
        protected override void OnStarting()
        {
            base.OnStarting();
            SetHeartTime(5, 300);//我们设置心跳检测时间, 时间越小检测越快, 跳检测时间也不能太小, 太小会直接判断为离线状态
            //加载所有mysql数据
            TitansiegeDB.I.InitData();
        }
        /// <summary>
        /// 接收客户端发过来的请求，当接收到第一个请求时，应该用来处理登录方法
        /// 服务器的OnUnClientRequest方法是返回true, 也就是你的账号和密码对的时候,就是返回true, 
        /// 如果账号或密码错误, 则返回false, 也就是账号不对, 就不能登录服务器/
        /// </summary>
        /// <param name="unClient"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        protected override bool OnUnClientRequest(GDClient unClient, RPCModel model)
        {
            switch (model.func)
            {
                case "Register":
                    if (model.pars.Count() != 3 || model.pars[0] == null || model.pars[1] == null || model.pars[2] == null)
                    {//参数不足，就直接返回false,禁止登录
                        Debuger.Log("注册参数不足！");
                        break;
                    }
                    UserService.GetInstance.Register(unClient, model.pars[0].ToString(), model.pars[1].ToString(), model.pars[2].ToString());
                    break;
                case "Login":
                    if (model.pars.Count() != 2 || model.pars[0] == null || model.pars[1] == null)
                    {//参数不足，就直接返回false,禁止登录
                        Debuger.Log("登录参数不足！");
                        break;
                    }
                    // return UserService.GetInstance.Login(unClient, model.pars[0].ToString(), model.pars[1].ToString());
                    bool issuccess =  UserService.GetInstance.Login(unClient, model.pars[0].ToString(), model.pars[1].ToString());
                    if (issuccess) {
                        LoginHandler(unClient);//这一句和return true效果是一样的
                    }
                    break;
                default:
                    break;
            }
            //其实只需要一个这个switch就够了，但因为我不想改以前写好的代码了，就写了两种，两种方式都可以，推荐这一种
            switch (model.methodHash) {
                case (ushort)ProtoType.relogin://重新登录，一般用于断线重连
                    string username = model.pars[0].ToString();
                   
                    if (TitansiegeDB.I.m_Users.TryGetValue(username, out var data)) {
                        unClient.PlayerID = username;//这个唯一标识用来判断是不是一个账号，一般用来挤号登录，用账号作为身份唯一标识，需保证账号名不重复，
                        unClient.User = data;
                        LoginHandler(unClient);//这一句和return true效果是一样的
                        Debuger.Log($"玩家{username}[{unClient.UserID}]断线重连登陆成功");
                        SendRT(unClient, (ushort)ProtoType.relogin, 1);
                    }
                    break;
            }
            return false;//100%必须理解这个, 返回false则永远在这里被调用, 返回true才被服务器认可
        }



        protected override void OnRemoveClient(GDClient client)
        {
            base.OnRemoveClient(client);//当客户端断开连接处理
        }
        /// <summary>
        /// 当开始调用 rpc标签的方法 时, 我们重写这个方法, 我们自己指定应该调用的方法, 这样会大大提高服务器效率
        /// </summary>
        /// <param name="client"></param>
        /// <param name="model"></param>
        protected override void OnRpcExecute(GDClient client, RPCModel model)
        {
            // base.OnRpcExecute(client, model);//反射调用rpc
            //上面默认调用的形式是反射，效率低，其实正确的用法应该是下面通过自定义的methodHash来传输消息的
            //gd里面，客户端sentrt方法时，可以不采用方法名send，而是send过来ushort哈希值，默认hash就存到了消息的methodHash变量中
            switch (model.methodHash) {
                case (ushort)ProtoType.selectcharacter:
                    long charid = model.AsLong;
                    for (int i = 0; i < client.CharacterList?.Count; i++) {
                        if (client.CharacterList[i].ID  == charid) {
                            //选择角色数据赋值
                            client.m_UserItem.Clear();
                            client.current = client.CharacterList[i];
                            client.AddRpc(client.current);
                            client.UpdateFightProps();
                            if (TitansiegeDB.I.m_BagItems.TryGetValue(client.CharacterList[i].ID, out var itemdata) && itemdata != null) {
                                client.m_BagItem = itemdata;
                            } else { //如果查询对应角色没有背包，就创建一个背包数据给它
                                BagitemData bagitemdata = new BagitemData();
                                bagitemdata.Id = TitansiegeDB.I.GetConfigID(ConfigType.BagItem);
                                bagitemdata.Cid = (int)client.CharacterList[i].ID;
                                bagitemdata.Inbag = "36|5,39|5";//默认给5个小血瓶
                                bagitemdata.NewTableRow();
                                TitansiegeDB.I.m_BagItems.Add(client.CharacterList[i].ID, bagitemdata);
                                client.m_BagItem = bagitemdata;
                            }
                            client.InitUserItem();//初始化拥有的道具
                            //读取升级配置表数据
                            if (client.current.Levelupid!=0) { //0就是没配置
                                client.m_LevelUp = ConfigerManager.GetInstance.FindData<LevelUpData>(CT.TABLE_LEVEL).FindByID((ushort)client.current.Levelupid);
                            }
                            Debuger.Log($"客户端（id{client.UserID}）选择了角色id{client.current.ID}");
                            Debuger.Log("加载" + client.m_LevelUp.LevelName + "模板");
                            break;
                        }
                    }
                    break;
                case (ushort)ProtoType.playerupdateprop://这种做法其实是有问题的，每一个网络对象创建时都会请求一次服务器，如果有100人同时在一个场景，
                                                        //一个玩家上线后，100个客户端都会同时请求一次服务器获取这个玩家数据，极容易造成大量并发，
                                                        //九宫格aoi能缓解这个问题，但服务器的负担一样不会小，
                                                        //考虑这里更换成玩家切换场景时，一次性返回本场景所有玩家数据，有新玩家进入场景后，服务器判断一下这个玩家数据是否存在，不存在就再同步一次新增数据？
                                                        //上面这个方案流量会高，貌似还不容九宫格aoi，嗯，先这样吧，回头有个更好的方案再说
                    int uid = (int)model.pars[0];
                    FightProp fp = null;
                    for (int i = 0; i < Clients.Count; i++) {
                        if (Clients[i].UserID == uid) {
                            fp = Clients[i].FP;
                            break;
                        }
                    }
                    Debuger.Log(uid + "发送获取玩家数据");
                    if (fp != null) {
                        //返回玩家属性数据
                        SendRT(client, (ushort)ProtoType.playerupdateprop, 1, fp);
                    } else {
                        SendRT(client, (ushort)ProtoType.playerupdateprop, 0);
                    }
                    break;
                case (ushort)ProtoType.reenterscene://重新进入场景，一般是断线重连后重新登录成功后使用的
                    string scenename = model.pars[0].ToString();
                    UserService.GetInstance.SwitchScene(client, scenename);
                    break;
                case (ushort)ProtoType.addjinbi://添加金币或者钻石，只添加一样，另一样0
                    int jinbi = (int)model.pars[0];
                    if (jinbi < 0) {//减少数量，首先先判断一下现有的,数量不足就返回false
                        if (client.current.Jinbi < jinbi) {
                            SendRT(client, (ushort)ProtoType.addjinbi, -1);
                            return;
                        }
                    }
                    client.current.Jinbi += jinbi;
                    //返回玩家属性数据
                    SendRT(client, (ushort)ProtoType.addjinbi, client.current.Jinbi);
                    break;
                case (ushort)ProtoType.addzuanshi://添加金币或者钻石，只添加一样，另一样0
                    int zuanshi = (int)model.pars[0];
                    if (zuanshi < 0) {//减少数量，首先先判断一下现有的,数量不足就返回false
                        if (client.current.Zuanshi < zuanshi) {
                            SendRT(client, (ushort)ProtoType.addzuanshi, -1);
                            return;
                        }
                    }
                    client.current.Zuanshi += zuanshi;
                    //返回玩家属性数据
                    SendRT(client, (ushort)ProtoType.addzuanshi, client.current.Zuanshi);
                    break;
                case (ushort)ProtoType.TodoItemNum://添加或减少道具
                    int id = (int)model.pars[0];
                    int num = (int)model.pars[1];
                    if (client.m_UserItem.TryGetValue(id, out int ordernum)) {
                        if (num < 0) {//减少数量，首先先判断一下现有的,数量不足就返回false
                            if (ordernum < num) {
                                SendRT(client, (ushort)ProtoType.TodoItemNum, -1);
                                return;
                            }
                        }
                        client.m_UserItem[id] += num;
                        Debuger.Log($"玩家{client.UserID}的道具{id}数量{num},总数{client.m_UserItem[id]}");
                        if (client.m_UserItem[id] == 0) {
                            client.m_UserItem.Remove(id);
                        }
                        //将背包数据写入字符串
                        client.UserItemToStr();
                       SendRT(client, (ushort)ProtoType.TodoItemNum, 0);
                    } else {
                        //背包没有此道具，客户端非法修改，返回客户端删除道具并警告
                        SendRT(client, (ushort)ProtoType.TodoItemNum, -2);
                    }
                    break;
                case (ushort)ProtoType.DestroyItem://删除道具
                    int itemid = (int)model.pars[0];
                    if (client.m_UserItem.ContainsKey(itemid)) {
                        client.m_UserItem[itemid] = 0;
                        client.m_UserItem.Remove(itemid);
                        Debuger.Log($"玩家{client.UserID}的道具{itemid}被销毁");
                        //将背包数据写入字符串
                        client.UserItemToStr();
                        SendRT(client, (ushort)ProtoType.DestroyItem, 0);
                    } else {
                        //背包中没有这个物品，可能是因为处理数量为0时删除了
                        SendRT(client, (ushort)ProtoType.DestroyItem, -1);
                    }
                    break;
                case (ushort)ProtoType.CreateItemToBag://添加道具
                    int itemid1 = (int)model.pars[0];
                    int num1 = (int)model.pars[1];
                    if (client.m_UserItem.ContainsKey(itemid1)) {
                        //道具已存在
                        SendRT(client, (ushort)ProtoType.CreateItemToBag, -1);
                        return;
                    } else {//正常创建
                        client.m_UserItem.Add(itemid1,num1);
                        Debuger.Log($"玩家{client.UserID}的添加道具{itemid1}，添加数量{num1}");
                        //将背包数据写入字符串
                        client.UserItemToStr();
                        SendRT(client, (ushort)ProtoType.CreateItemToBag, 0);
                    }
                    break;
                case (ushort)ProtoType.ChangeEqu://装备道具
                    int equid = (int)model.pars[0];
                    int itemtypenum = (int)model.pars[1];
                    ItemType itemtype = (ItemType)itemtypenum;
                    //获取需要装备的id和数据
                    ItemBase equ =ItemService.GetInstance.GetItem(equid);
                    ItemBase changeitem = null;//已装备的
                    switch (itemtype) {
                        case ItemType.yifu:
                            if (client.current.Yifu>0) {
                                changeitem = ItemService.GetInstance.GetItem(client.current.Yifu);
                            }
                            client.current.Yifu = (short)equid;
                            break;
                        case ItemType.kuzi:
                            if (client.current.Kuzi > 0)
                                changeitem = ItemService.GetInstance.GetItem(client.current.Kuzi);
                            client.current.Kuzi = (short)equid;
                            break;
                        case ItemType.wuqi:
                            if (client.current.Wuqi > 0)
                                changeitem = ItemService.GetInstance.GetItem(client.current.Wuqi);
                            client.current.Wuqi = (short)equid;
                            break;
                        case ItemType.xianglian:
                            if (client.current.Xianglian > 0)
                                changeitem = ItemService.GetInstance.GetItem(client.current.Xianglian);
                            client.current.Xianglian = (short)equid;
                            break;
                        case ItemType.jiezi:
                            if (client.current.Jiezi > 0)
                                changeitem = ItemService.GetInstance.GetItem(client.current.Jiezi);
                            client.current.Jiezi = (short)equid;
                            break;
                        case ItemType.xiezi:
                            if (client.current.Xiezi > 0)
                                changeitem = ItemService.GetInstance.GetItem(client.current.Xiezi);
                            client.current.Xiezi = (short)equid;
                            break;
                        default:
                            Debuger.Log($"玩家{client.UserID}传输未知装备类型，装备id{equid}，请检查数据！");
                            break;
                    }
                    if (changeitem == null) { //原本没有装备
                        UserService.GetInstance.UpdateCharacterData(client.current,client.FP,client,equ.m_PropXiaoguoDic,null);
                    } else { //原本有装备
                        UserService.GetInstance.UpdateCharacterData(client.current, client.FP, client,equ.m_PropXiaoguoDic, changeitem.m_PropXiaoguoDic);
                    }
                    //client.current.Update();
                    SendRT(client, (ushort)ProtoType.ChangeEqu, 0);
                    break;
                case (ushort)ProtoType.XiexiaItem://卸下道具
                    int equid1 = (int)model.pars[0];
                    int itemtypenum1 = (int)model.pars[1];
                    ItemType itemtype1 = (ItemType)itemtypenum1;
                    //获取需要装备的id和数据
                    ItemBase equ1 = ItemService.GetInstance.GetItem(equid1);
                    switch (itemtype1) {
                        case ItemType.yifu:
                            client.current.Yifu = -1;
                            break;
                        case ItemType.kuzi:
                            client.current.Kuzi = -1;
                            break;
                        case ItemType.wuqi:
                            client.current.Wuqi = -1;
                            break;
                        case ItemType.xianglian:
                            client.current.Xianglian = -1;
                            break;
                        case ItemType.jiezi:
                            client.current.Jiezi = -1;
                            break;
                        case ItemType.xiezi:
                            client.current.Xiezi = -1;
                            break;
                        default:
                            Debuger.Log($"玩家{client.UserID}传输未知装备类型，装备id{equid1}，请检查数据！");
                            break;
                    }
                    UserService.GetInstance.UpdateCharacterData(client.current, client.FP, client, null, equ1.m_PropXiaoguoDic);
                    SendRT(client, (ushort)ProtoType.XiexiaItem, 0);
                    break;
                case (ushort)ProtoType.signout://客户端主动发起退出登录
                    SignOut(client);
                    break;
                case (ushort)ProtoType.SendTalk://给全服玩家发送消息
                    string str = (string)model.pars[0];    
                    int talktypenum = (int)model.pars[1];
                    TalkType talktype = (TalkType)talktypenum;
                    if (TalkType.世界 == talktype) {//通过服务器发送全服玩家
                        Multicast(Clients, (ushort)ProtoType.SendTalk, str, talktypenum);
                    } else if (TalkType.队伍 == talktype) {//通过服务器发送队伍的玩家，需要队伍模块开发完成  todo

                    } else if (TalkType.系统 == talktype) { //这里是客户端自己发给服务器的，主要是为了告诉服务器，自己聊天模块加载完成了
                        SendRT(client, (ushort)ProtoType.SendTalk, "【系统】：欢迎来到神话纪元[2]，个人游戏demo，已开源，[3]详细源码请参考gitee仓库:https://gitee.com/xixin32/Titansiege", talktypenum);
                    }
                    break;
                default:
                    base.OnRpcExecute(client, model);//反射调用rpc
                    break;
            }
        }
        
        protected override void OnStartupCompleted() {
            base.OnStartupCompleted();
            //RemoveScene(MainSceneName, false);
            var path = AppDomain.CurrentDomain.BaseDirectory + "\\Data\\";
            Debuger.Log(path);
            var files = Directory.GetFiles(path, "*.mapData");
            foreach (var flie in files) {
                var mapData = MapData.ReadData(flie);
                GDScene gdscene = CreateScene(mapData.name); 
                gdscene.mapData = mapData;
                gdscene.Init();
                Debuger.Log("创建地图:" + gdscene.Name);
            }
            //天空城没有怪物，所以没战斗数据
            GDScene scene = CreateScene("tiankongcheng");
            scene.Init();
            Debuger.Log("创建地图:" + scene.Name);
            MainSceneName = "SelectRole";//指定你的主场景名称, 根据unity的主战斗场景名称设置
            Debuger.Log("主地图名称:SelectRole");
        }
        /// <summary>
        /// 当添加默认场景的时候
        /// </summary>
        /// <returns></returns>
        protected override GDScene OnAddDefaultScene() {
            //创建了一个名为 "SelectRole" 的一个场景对象, 并且可以容纳1000的场景
            Debuger.Log("创建默认场景:SelectRole");
            return new GDScene() { Name = "SelectRole", sceneCapacity = 1000 };
        }

        /// <summary>
        /// 当客户端登录成功时要添加到主场景时
        /// </summary>
        /// <param name="client"></param>
        protected override void OnAddPlayerToScene(GDClient client)
        {
            base.OnAddPlayerToScene(client);//如果不允许登录成功加入主大厅场景, 注释这行代码即可
            JoinScene(client, "SelectRole");
        }

        protected override void OnOperationSync(GDClient client, OperationList list)
        {
            base.OnOperationSync(client, list);//当操作同步处理, 帧同步或状态同步通用

        }
        /// <summary>
        /// 当退出登录时
        /// </summary>
        /// <param name="client"></param>
        public override void OnSignOut(GDClient client) {
            SendRT(client,(ushort)ProtoType.signout);
        }
    }

}

using cmd;
using GDServer.Services;
using MySqlX.XDevAPI;
using Net.Component;
using Net.Server;
using Net.Share;
using Net.System;
using System.Text;
using System.Xml.Linq;
using Titansiege;
using static Google.Protobuf.Reflection.FieldOptions.Types;

namespace GDServer {
    public class GDClient : NetPlayer//客户端类
    {

        internal NTransform transform = new NTransform();
        public List<CharactersData>? CharacterList = new List<CharactersData>();
        public CharactersData? current;//当前登录角色
        public UsersData? User = new UsersData();
        public GDScene scene;
        public bool m_IsDie = false;
        public LevelUpDataBase m_LevelUp;
        public Dictionary<int, int> m_UserItem = new Dictionary<int, int>();//玩家拥有的道具字典
        public FightProp FP = new FightProp();//战斗属性
        public BagitemData m_BagItem;
        public override void OnEnter() {
            m_IsDie = false;
            scene = Scene as GDScene;
        }

        public override void OnUpdate() {
            if (scene == null && current == null) {
                return;
            }
            //不打算每桢同步，而是血量变化后同步
            //scene.AddOperation(new Operation(Command.PlayerState, UserID) {
            //    index = FightHP,
            //    index1 = FightMagic,
            //});
        }

        internal void BeAttacked(int damage) {
            if (m_IsDie)
                return;
            FP.FightHP -= damage;
            if (FP.FightHP <= 0 && !m_IsDie) {
                m_IsDie = true;
            }
            scene.AddOperation(new Operation(Command.PlayerState, UserID) {
                index = FP.FightHP,
                index1 = FP.FightMagic,
                index2 = FP.FightMaxHp,
                index3 = FP.FightMaxMagic,
            });
        }
        public void CostMp(int cost) {
            FP.FightMagic -= cost;
            if (FP.FightMagic < 0) {//正常这里不会进来，因为客户端会直接限制住无魔力释放
                FP.FightMagic = 0;
            }
            scene.AddOperation(new Operation(Command.PlayerState, UserID) {
                index = FP.FightHP,
                index1 = FP.FightMagic,
                index2 = FP.FightMaxHp,
                index3 = FP.FightMaxMagic,
            });
        }
        public void Resurrection() {
            //数据库中不设置最大生命，而是根据等级+装备来计算
            FP.FightHP = FP.FightMaxHp;
            FP.FightMagic = FP.FightMaxMagic;
            m_IsDie = false;
        }
        public override void Dispose() {
            base.Dispose();
        }

        public override void OnExit() {
            base.OnExit();
        }

        public override void OnRemoveClient() {
            base.OnRemoveClient();

        }

        public override void OnSignOut() {
            base.OnSignOut();
            RemoveRpc(User);
            RemoveRpc(current);
           
            User = null;
            current = null;
        }
        /// <summary>
        /// 登录成功后才会调用
        /// </summary>
        public override void OnStart() {
            base.OnStart();
            AddRpc(User); 

            //AddRpc(current); //登陆成功后没有选择角色，此时为空，所以不能在这里加载，应该在选择角色后加载
        }
        public override void OnRpcExecute(RPCModel model) {
            switch (model.methodHash) {
                default:
                    base.OnRpcExecute(model);
                    break;
            }
        }

        /// <summary>
        /// 根据属性写入战斗属性
        /// 暂时还未加入道具影响，例如装备，需道具模块开发完成后，再完善
        /// </summary>
        public virtual void UpdateFightProps() {
            float jcDodge = FP.BaseDodge;//基础闪避率
            float jcCrit = FP.BaseCrit;//基础暴击
            switch (current?.Zhiye) {
                case (int)Zhiye.剑士:
                    FP.Attack = current.Liliang * 10;
                    FP.Defense = current.Liliang * 3 + current.Tizhi * 7;
                    break;
                case (int)Zhiye.法师:
                    FP.Attack = current.Moli * 10;
                    FP.Defense = current.Moli * 4 + current.Tizhi * 6;
                    jcDodge = 0.02f;
                    break;
                case (int)Zhiye.游侠:
                    FP.Attack = current.Minjie * 10;
                    FP.Defense = current.Minjie * 4 + current.Tizhi * 6;
                    jcDodge = 0.03f;
                    break;
                default:
                    break;
            }
            //闪避,基础闪避率0.01f;
            FP.Dodge = jcDodge + (float)current.Minjie / 1000f >= 0.3f ? 0.3f : (float)current.Minjie / 1000f + jcDodge;//属性加成的闪避
            FP.Crit = jcCrit + (float)current.Xingyun * jcCrit >= 0.5f ? 0.5f : (float)current.Xingyun * jcCrit;//暴击率
            FP.FightHP = current.Shengming + current.Tizhi * 10;
            FP.FightMagic = current.Fali + current.Moli * 10;
            FP.FightMaxHp = FP.FightHP;
            FP.FightMaxMagic = FP.FightMagic;
            FP.PlayerName = current.Name;
            //添加装备附加的运行时属性
            if (current.Yifu>0) {
                AddEquFPProp(ItemService.GetInstance.GetItem(current.Yifu).m_PropXiaoguoDic);
            }
            if (current.Kuzi > 0) {
                AddEquFPProp(ItemService.GetInstance.GetItem(current.Kuzi).m_PropXiaoguoDic);
            }
            if (current.Wuqi > 0) {
                AddEquFPProp(ItemService.GetInstance.GetItem(current.Wuqi).m_PropXiaoguoDic);
            }
            if (current.Xianglian > 0) {
                AddEquFPProp(ItemService.GetInstance.GetItem(current.Xianglian).m_PropXiaoguoDic);
            }
            if (current.Jiezi > 0) {
                AddEquFPProp(ItemService.GetInstance.GetItem(current.Jiezi).m_PropXiaoguoDic);
            }
            if (current.Xiezi > 0) {
                AddEquFPProp(ItemService.GetInstance.GetItem(current.Xiezi).m_PropXiaoguoDic);
            }
            //Debuger.Log($"更新玩家{UserID}属性，同步客户端");
            ////发给服务端更新属性
            ////这里和怪的数据一样，先这么办吧，最合适的做法应该还是服务端同步数据，客户端计算
            ///废弃，这里换成客户端直接请求了
            //scene.AddOperation(new Operation(Command.PlayerUpdateProp, UserID) {
            //    name = current.Name,
            //    index = FP.FightHP,
            //    index1 = FP.FightMagic,
            //    index2 = FP.FightMaxHp,
            //    index3 = FP.FightMaxMagic,
            //});
        }
        /// <summary>
        /// 处理玩家数据，加上运行时的装备属性
        /// </summary>
        /// <param name="m1">需要装备的道具的效果字典</param>
        /// <param name="m2">需要卸下的道具效果字典</param>
        private void AddEquFPProp(Dictionary<XiaoGuo, Dictionary<string, EffArgs>> m1 = null) {
            //加上新的数据
            if (m1 != null) {
                foreach (KeyValuePair<XiaoGuo, Dictionary<string, EffArgs>> xiaoguo in m1) {
                    if (xiaoguo.Value.Count > 0) {
                        foreach (KeyValuePair<string, EffArgs> xg in xiaoguo.Value) {
                            switch (xg.Key) {
                                case "gongji":
                                    FP.Attack += (short)xg.Value.ivalue;
                                    break;
                                case "fangyu":
                                    FP.Defense += (short)xg.Value.ivalue;
                                    break;
                                case "shanbi":
                                    FP.Dodge += xg.Value.fvalue;
                                    break;
                                case "baoji":
                                    FP.Crit += xg.Value.fvalue;
                                    break;
                                case "maxhp":
                                    FP.FightMaxHp += (short)xg.Value.ivalue;
                                    FP.FightHP += (short)xg.Value.ivalue;
                                    break;
                                case "maxmagic":
                                    FP.FightMaxMagic += (short)xg.Value.ivalue;
                                    FP.FightMagic += (short)xg.Value.ivalue;
                                    break;
                            }
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 增加经验
        /// </summary>
        /// <param name="exp"></param>
        /// <exception cref="NotImplementedException"></exception>
        internal void AddExp(int exp) {
            int allexp = current.Exp + exp;//总经验
            //检查是否升级
            short uplevel = (short)current.Level;
            int upneedexp = uplevel * uplevel * m_LevelUp.UpExp;//等级平方*升级模板升级系数
            while (allexp >= upneedexp) {//提前计算一下获得的经验可以升多少级
                //这里的逻辑是，如果获得经验加上原有经验大于升级经验，就直接升级，然后减去已使用的升级经验后，再判断剩余经验是否够升级，循环到不能升级位置，从而得出能升级多少次
                uplevel++;
                allexp -= upneedexp;
                upneedexp = uplevel * uplevel * m_LevelUp.UpExp;
            }
            current.Exp = allexp;//还剩的经验值
            short uptime = (short)(uplevel - (short)current.Level);//升多少级
            if (uptime>0) {
                UpProp(uptime);//递归调用，容易堆栈溢出
            }
            //为了节省带宽，这里只发送一个升级命令和，具体升级计算由客户端自己完成
            scene.AddOperation(new Operation(Command.PlayerUpdateProp, UserID) {
                index = uptime,//升级多少次
                index1 = current.Exp,
                index2 = FP.FightMaxHp,//客户端网络玩家使用
            });
        }
        /// <summary>
        /// 升级的方法
        /// </summary>
        /// <param name="uplv">升级多少次</param>
        private void UpProp(short uplv) {
            current.Liliang += (short)(m_LevelUp.Liliang* uplv);
            current.Tizhi += (short)(m_LevelUp.Tizhi * uplv);
            current.Minjie += (short)(m_LevelUp.Minjie * uplv);
            current.Moli += (short)(m_LevelUp.Moli * uplv);
            current.Shengming += (short)(m_LevelUp.Shengming * uplv);
            current.Fali += (short)(m_LevelUp.Fali * uplv);
            current.Meili += (short)(m_LevelUp.Meili * uplv);
            current.Xingyun += (short)(m_LevelUp.Xingyun * uplv);
            current.Lianjin += (short)(m_LevelUp.Lianjin * uplv);
            current.Duanzao += (short)(m_LevelUp.Duanzao * uplv);
            current.Level += (sbyte)uplv;
            current.Update();
            UpdateFightProps();
            //下发所有客户端更新属性
            //scene.AddOperation(new Operation(Command.PlayerUpdateProp, UserID) {
            //    name = current.Name,
            //    index = FP.FightHP,
            //    index1 = FP.FightMagic,
            //    index2 = FP.FightMaxHp,
            //    index3 = FP.FightMaxMagic,
            //});
        }
        /// <summary>
        /// 将数据库中的字符串道具数据初始化成字典
        /// 字符串的结构为：1|1,2|1,234|10,12|1,
        /// </summary>
        /// <param name="str"></param>
        public void InitUserItem() {
            if (m_BagItem!=null) {
                string str = m_BagItem.Inbag;
                string[] strarr = str.Split(',');
                for (int i = 0; i < strarr.Length; i++) {
                    if (!string.IsNullOrEmpty(strarr[i])) {//保险起见，加一层判断
                        string[] strarr1 = strarr[i].Split('|');
                        int itemid = int.Parse(strarr1[0]);
                        int itemnum = int.Parse(strarr1[1]);
                        if (itemid != 0 && itemnum >= 1) {
                            m_UserItem.Add(itemid, itemnum);
                        } else {
                            Debuger.LogError("道具数据解析错误，请检查");
                        }
                    }
                }
                if (m_UserItem.Count > 0) {
                    TServer.Instance.SendRT(this, (ushort)ProtoType.returnbagitem, str);
                }
            }
            else{
                Debuger.Log($"{UserID}角色背包为空");
            }
        }
        /// <summary>
        /// 将现有道具转换成字符串，数据库写入使用
        /// </summary>
        /// <returns></returns>
        public string UserItemToStr() {
            StringBuilder sb = new StringBuilder();
            foreach (KeyValuePair<int, int> tmp in m_UserItem) {
                if (tmp.Key != 0 && tmp.Value > 0) {
                    sb.Append($"{tmp.Key}|{tmp.Value},");
                }
            }
            m_BagItem.Inbag = sb.ToString();
            //m_BagItem.Update();
            return sb.ToString();
        }

        internal void AddHpOrMp(int hp,int mp) {
            FP.FightHP += hp;
            FP.FightMagic += mp;
            scene.AddOperation(new Operation(Command.PlayerState, UserID) {
                index = FP.FightHP,
                index1 = FP.FightMagic,
                index2 = FP.FightMaxHp,
                index3 = FP.FightMaxMagic,
            });
        }

    }
}

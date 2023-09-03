using Net.Component;
using Net.Server;
using Net.Share;
using Titansiege;
using static Google.Protobuf.Reflection.FieldOptions.Types;

namespace GDServer {
    public class GDClient : NetPlayer//客户端类
    {

        internal NTransform transform = new NTransform();
        public List<CharactersData>? CharacterList = new List<CharactersData>();
        public CharactersData? current;//当前登录角色
        public UsersData? User =new UsersData();
        public  GDScene scene;
        public bool m_IsDie = false;

        public FightProp FP = new FightProp();//战斗属性
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
            FP.FightHP -= (short)damage;
            if (FP.FightHP <=0&& !m_IsDie) {
                m_IsDie = true;
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
            FP.Dodge = jcDodge + (float)current.Minjie / 1000f >= 0.3f ? 0.3f : (float)current.Minjie / 1000f;//属性加成的闪避
            FP.Crit = jcCrit + (float)current.Xingyun * jcCrit >= 0.5f ? 0.5f : (float)current.Xingyun * jcCrit;//暴击率
            FP.FightHP = current.Shengming + current.Tizhi * 10;
            Debuger.Log("生命："+ FP.FightHP);
            FP.FightMagic = current.Fali + current.Moli * 10;
            FP.FightMaxHp = FP.FightHP;
            FP.FightMaxMagic = FP.FightMagic;
            FP.PlayerName = current.Name;
            //Debuger.Log($"更新玩家{UserID}属性，同步客户端");
            ////发给服务端更新属性
            ////这里和怪的数据一样，先这么办吧，最合适的做法应该还是服务端同步数据，客户端计算
            //scene.AddOperation(new Operation(Command.PlayerUpdateProp, UserID) {
            //    name = current.Name,
            //    index = FP.FightHP,
            //    index1 = FP.FightMagic,
            //    index2 = FP.FightMaxHp,
            //    index3 = FP.FightMaxMagic,
            //});
        }

    }
    
}


using Net.Server;
using Net.Share;
using GDServer.DB;
using System.Data;
using Net.System;
using System.Linq;
using System;
using System.Numerics;
using Net.Event;

//文件最好与服务器端VisualStudio项目的MyCommand.cs同名且代码一样
namespace GDServer.Services
{
    public class TServer : TcpServer<GDClient, GDScene>//服务器类
    {

        public static DBModel m_DB;
        
        
        /// <summary>
        /// 当开始服务器的时候
        /// </summary>
        /// 
        protected override void OnStarting()
        {
            base.OnStarting();
            SetHeartTime(5, 300);//我们设置心跳检测时间, 时间越小检测越快, 跳检测时间也不能太小, 太小会直接判断为离线状态
            //加载所有mysql数据
            m_DB = new DBModel();
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
                    return UserService.GetInstance.Login(unClient, model.pars[0].ToString(), model.pars[1].ToString());

                default:
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
        /// 改成单例调用
        /// </summary>
        /// <param name="client"></param>
        /// <param name="model"></param>
        protected override void OnRpcExecute(GDClient client, RPCModel model)
        {
            base.OnRpcExecute(client, model);//反射调用rpc
        }
        
        protected override void OnStartupCompleted() {
            base.OnStartupCompleted();
            RemoveScene(MainSceneName, false);
#if !UNITY_EDITOR
            var path = AppDomain.CurrentDomain.BaseDirectory + "Data/";
#else
            var path = UnityEngine.Application.dataPath+"/SceneData/";
#endif
            Debuger.Log(path);
            var files = Directory.GetFiles(path, "*.sceneData");
            foreach (var flie in files) {
                var sceneData = SceneData.ReadData(flie);
                GDScene gdscene = CreateScene(sceneData.name);
                gdscene.sceneData = sceneData;
                gdscene.Init();
                NDebug.Log("创建地图:" + gdscene.Name);
            }
            GDScene scene = CreateScene("tiankongcheng");
            //scene.sceneData = sceneData;
            scene.Init();
            Debuger.Log("创建地图:" + scene.Name);
            
            MainSceneName = "tiankongcheng";//指定你的主战斗场景名称, 根据unity的主战斗场景名称设置
            Debuger.Log("主地图名称:tiankongcheng");
        }
        /// <summary>
        /// 当添加默认场景的时候
        /// </summary>
        /// <returns></returns>
        protected override GDScene OnAddDefaultScene() {
            //我们创建了一个名为 "天空城" 的一个场景对象, 并且可以容纳1000的场景
            return new GDScene() { Name = "tiankongcheng", sceneCapacity = 1000 };
        }

        /// <summary>
        /// 当客户端登录成功时要添加到主场景时
        /// </summary>
        /// <param name="client"></param>
        protected override void OnAddPlayerToScene(GDClient client)
        {
            base.OnAddPlayerToScene(client);//如果不允许登录成功加入主大厅场景, 注释这行代码即可
            //JoinScene(client,"tiankongcheng");
        }

        protected override void OnOperationSync(GDClient client, OperationList list)
        {
            base.OnOperationSync(client, list);//当操作同步处理, 帧同步或状态同步通用
        }

    }
}

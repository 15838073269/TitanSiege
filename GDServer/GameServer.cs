

using GDServer.Services;
using Net.Event;
using Net.Share;
using Net.System;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GDServer
{
    internal class GameServer {
        static void Main(string[] args) {
            //NDebug.WriteFileMode = WriteLogMode.All;
            //NDebug.BindConsoleLog();
            //NDebug.Log("开始创建服务器.....");
            //个人习惯，没有使用框架的NDebug,而是使用自己的Debuger类输出。后续如果发现NDebug更优秀的地方再更换吧
            InitDebuger();
            LoadConfigTable();
            Debuger.Log("开始创建服务器.....");
            TServer server = new TServer();//创建服务器对象
            server.OnlineLimit = 24000;//服务器最大运行2500人连接
            server.LineUp = 24000;
            server.MaxThread = 10; //增加并发线程
            server.RTO = 50;
            server.MTU = 1300;
            server.MTPS = 2048;
            server.AddAdapter(new Net.Adapter.SerializeAdapter3());//添加极速序列化适配器
            server.Log += Debuger.Log;//打印服务器内部信息
            //server.OnNetworkDataTraffic += (df) => {//当统计网络性能,数据传输量
            //    NDebug.Log($"流出:{df.sendNumber}次/{ByteHelper.ToString(df.sendCount)} " +
            //    $"流入:{df.receiveNumber}次/{ByteHelper.ToString(df.receiveCount)} " +
            //    $"发送fps:{df.sendLoopNum} 接收fps:{df.revdLoopNum} 解析:{df.resolveNumber}次 " +
            //    $"总流入:{ByteHelper.ToString(df.inflowTotal)} 总流出:{ByteHelper.ToString(df.outflowTotal)}");
            //    NDebug.Log("登录:" + server.OnlinePlayers + " 未登录:" + server.UnClientNumber);
            //};
            server.Run(8000);//启动8000端口
            Task.Run(() => {//开一个线程累加时间
                while (true) {
                    Thread.Sleep(30);//休眠30毫秒后，时间累加
                    Time.time++;//时间累加
                }
            });
            //ThreadManager.Invoke("tick", Time.deltaTime, () => {//设置时间增加
            //    Time.time = Time.deltaTime;
            //    return true;
            //});
            while (true) {
                Console.ReadLine();
            }
            

        }
        private static void InitDebuger() {
            //Debuger.Init(Application.persistentDataPath + "/Debuger/", new UnityDebugerConsole());
            //设置日志开关
            Debuger.EnableLog = true;//打开日志
            Debuger.Log("日志系统已开启！");
            //Debuger.EnableSave = true;
            if (Debuger.EnableSave) {
                Debuger.Log("日志写入本地已开启！");
            } else {
                Debuger.Log("日志写入本地未开启！");
            }
        }
        /// <summary>
        /// 加载配置表
        /// </summary>
        private static void LoadConfigTable() {
            //ConfigerManager.GetInstance.LoadData<NPCData>(CT.TABLE_NPC);
            ConfigerManager.GetInstance.LoadData<ItemData>(CT.TABLE_ITEM);
            ConfigerManager.GetInstance.LoadData<LevelUpData>(CT.TABLE_LEVEL);
            //ConfigerManager.GetInstance.LoadData<NameData>(CT.TABLE_NAME);  //名字不是每次都用，换到其他地方初始化
            ConfigerManager.GetInstance.InitData();
        }
    }
  
}
namespace Net.Event
{
    using global::System;
    using global::System.IO;
    using global::System.Text;
#if SERVICE && WINDOWS
    using global::System.Drawing;
    using global::System.Windows.Forms;
#endif
    using Net.System;

    public enum LogType
    {
        Log,
        Warning,
        Error,
    }

    public class LogEntity
    {
        public int count;
        public int row = -1;
        public DateTime time;
        public LogType log;
        public string msg;

        public override string ToString()
        {
            return $"[{time.ToString("yyyy-MM-dd HH:mm:ss")}][{log}] {msg}";
        }
    }

    /// <summary>
    /// 控制台输出帮助类
    /// </summary>
    public class ConsoleDebug : IDebug
    {
        private readonly MyDictionary<string, LogEntity> dic = new MyDictionary<string, LogEntity>();
        public int count = 1000;
        private int cursorTop;

        public void Output(DateTime time, LogType log, string msg)
        {
            if (dic.Count > count)
            {
                dic.Clear();
                Console.Clear();
                cursorTop = 0;
            }
            if (!dic.TryGetValue(log + msg, out var entity))
                dic.TryAdd(log + msg, entity = new LogEntity() { time = time, log = log, msg = msg });
            entity.count++;
            if (entity.row == -1)
            {
                entity.row = cursorTop;
                Console.SetCursorPosition(0, cursorTop);
            }
            else
            {
                Console.SetCursorPosition(0, entity.row);
            }
            var info = $"[{time.ToString("yyyy-MM-dd HH:mm:ss")}][";
            Console.Write(info);
            Console.ForegroundColor = log == LogType.Log ? ConsoleColor.Green : log == LogType.Warning ? ConsoleColor.Yellow : ConsoleColor.Red;
            info = $"{log}";
            Console.Write(info);
            Console.ResetColor();
            if (entity.count > 1)
                Console.Write($"] ({entity.count}) {msg}\r\n");
            else
                Console.Write($"] {msg}\r\n");
            if(Console.CursorTop > cursorTop)
                cursorTop = Console.CursorTop;
        }
    }

#if SERVICE && WINDOWS
    /// <summary>
    /// Form窗口程序输出帮助类
    /// </summary>
    public class FormDebug : IDebug
    {
        private MyDictionary<string, LogEntity> dic = new MyDictionary<string, LogEntity>();
        public int count = 1000;
        public ListBox listBox;
        /// <summary>
        /// 字体颜色
        /// </summary>
        public Brush BackgroundColor;
        /// <summary>
        /// 日志颜色
        /// </summary>
        public Brush LogColor = Brushes.Blue;
        /// <summary>
        /// 警告颜色
        /// </summary>
        public Brush WarningColor = Brushes.Yellow;
        /// <summary>
        /// 错误颜色
        /// </summary>
        public Brush ErrorColor = Brushes.Red;

        public FormDebug(ListBox listBox, Brush backgroundColor = null) 
        {
            if (backgroundColor == null)
                backgroundColor = Brushes.Black;
            this.listBox = listBox;
            this.BackgroundColor = backgroundColor;
            listBox.DrawMode = DrawMode.OwnerDrawFixed;
            listBox.DrawItem += DrawItem;
        }

        public void Output(DateTime time, LogType log, string msg)
        {
            if (dic.Count > count)
            {
                dic.Clear();
                listBox.Items.Clear();
            }
            if (!dic.TryGetValue(log + msg, out var entity))
                dic.TryAdd(log + msg, entity = new LogEntity() { time = time, log = log, msg = msg });
            entity.count++;
            if (entity.row == -1)
            {
                entity.row = listBox.Items.Count;
                listBox.Items.Add(entity);
            }
        }

        public void DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index == -1)
                return;
            var entity = listBox.Items[e.Index] as LogEntity;
            e.DrawBackground();
            e.DrawFocusRectangle();
            var y = e.Bounds.Y;
            var msg = $"[{entity.time.ToString("yyyy-MM-dd HH:mm:ss")}][";
            e.Graphics.DrawString(msg, e.Font, BackgroundColor, 0, y);
            var x = msg.Length * 6;
            msg = $"{entity.log}";
            var color = entity.log == LogType.Log ? LogColor : entity.log == LogType.Warning ? WarningColor : ErrorColor;
            e.Graphics.DrawString(msg, e.Font, color, x, y);
            x += msg.Length * 6;
            msg = entity.msg.Split('\r', '\n')[0];
            if (msg.Length >= byte.MaxValue) //文字过多会报异常
                msg = msg.Substring(0, byte.MaxValue);
            if (entity.count > 1)
                e.Graphics.DrawString($"] ({entity.count}) {msg}", e.Font, BackgroundColor, x, y);
            else
                e.Graphics.DrawString($"] {msg}", e.Font, BackgroundColor, x, y);
        }
    }
#endif

    public interface IDebug 
    {
        void Output(DateTime time, LogType log, string msg);
    }

    /// <summary>
    /// 写入日志模式
    /// </summary>
    public enum WriteLogMode 
    {
        /// <summary>
        /// 啥都不干
        /// </summary>
        None = 0,
        /// <summary>
        /// 写入普通日志
        /// </summary>
        Log,
        /// <summary>
        /// 写入警告日志
        /// </summary>
        Warn,
        /// <summary>
        /// 写入错误日志
        /// </summary>
        Error,
        /// <summary>
        /// 三种日志全部写入
        /// </summary>
        All,
        /// <summary>
        /// 只写入警告日志和错误日志
        /// </summary>
        WarnAndError,
    }

    /// <summary>
    /// 消息输入输出处理类
    /// </summary>
    public static class NDebug
    {
        /// <summary>
        /// 输出调式消息
        /// </summary>
        public static event Action<string> LogHandle;
        /// <summary>
        /// 输出调式错误消息
        /// </summary>
        public static event Action<string> LogErrorHandle;
        /// <summary>
        /// 输出调式警告消息
        /// </summary>
        public static event Action<string> LogWarningHandle;
        /// <summary>
        /// 输出信息处理事件
        /// </summary>
        public static event Action<DateTime, LogType, string> Output;
        /// <summary>
        /// 输出日志最多容纳条数
        /// </summary>
        public static int LogMax { get; set; } = 10000;
        /// <summary>
        /// 输出错误日志最多容纳条数
        /// </summary>
        public static int LogErrorMax { get; set; } = 10000;
        /// <summary>
        /// 输出警告日志最多容纳条数
        /// </summary>
        public static int LogWarningMax { get; set; } = 10000;
        /// <summary>
        /// 每次执行可连续输出多少条日志, 默认输出300 * 3条
        /// </summary>
        public static int LogOutputMax { get; set; } = 300;
        private static readonly QueueSafe<object> logQueue = new QueueSafe<object>();
        private static readonly QueueSafe<object> errorQueue = new QueueSafe<object>();
        private static readonly QueueSafe<object> warningQueue = new QueueSafe<object>();
        /// <summary>
        /// 绑定的输入输出对象
        /// </summary>
        public static IDebug Debug { get; set; }
        private static FileStream fileStream;
        private static int writeFileModeID;
        private static WriteLogMode writeFileMode;
        /// <summary>
        /// 写入日志到文件模式
        /// </summary>
        public static WriteLogMode WriteFileMode
        {
            get { return writeFileMode; }
            set 
            {
                writeFileMode = value;
                if (value != WriteLogMode.None & fileStream == null)
                {
                    CreateLogFile();
                    var now = DateTime.Now;
                    var day = now.AddDays(1);
                    day = new DateTime(day.Year, day.Month, day.Day, 0, 0, 0);//明天0点
                    var time = (day - now).TotalMilliseconds / 1000d;//转换成0.x秒
                    writeFileModeID = ThreadManager.Invoke("CreateLogFile", (float)time, CreateLogFile);//每0点会创建新的日志文件
                }
                else if (value == WriteLogMode.None & fileStream != null)
                {
                    fileStream.Close();
                    fileStream = null;
                    ThreadManager.Event.RemoveEvent(writeFileModeID);
                }
            }
        }

        private static bool CreateLogFile()
        {
            try
            {
                var now = DateTime.Now;
                var path = Config.Config.ConfigPath + $"\\Log\\{now.Year}\\{now.Month.ToString("00")}\\";
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                path += $"{now.Year}{now.Month.ToString("00")}{now.Day.ToString("00")}{now.Hour.ToString("00")}{now.Minute.ToString("00")}{now.Second.ToString("00")}.txt";
                if (fileStream != null)
                    fileStream.Close();
                fileStream = new FileStream(path, FileMode.OpenOrCreate); //不加try会导致服务器崩溃闪退问题
                var position = fileStream.Length;
                fileStream.Seek(position, SeekOrigin.Begin);
            }
            catch (Exception ex)
            {
                NDebug.LogError(ex);
            }
            return true;
        }

#if SERVICE
        static NDebug()
        {
            ThreadManager.Invoke("OutputLog", OutputLog, true);
        }

        private static bool OutputLog()
        {
            try
            {
                var sb = new StringBuilder();
                var isWrite = writeFileMode == WriteLogMode.All | writeFileMode == WriteLogMode.Log;
                var currTime = DateTime.Now;
                var logTime = currTime.ToString("yyyy-MM-dd HH:mm:ss");
                var msg = string.Empty;
                var log = string.Empty;
                object message;
                var output = LogOutputMax;
                while (logQueue.TryDequeue(out message))
                {
                    msg = message.ToString();
                    log = $"[{logTime}][Log] {msg}";
                    LogHandle?.Invoke(log);
                    Output?.Invoke(currTime, LogType.Log, msg);
                    if (isWrite)
                        sb.AppendLine(log);
                    if (--output <= 0)
                        break;
                }
                isWrite = writeFileMode == WriteLogMode.All | writeFileMode == WriteLogMode.Warn | writeFileMode == WriteLogMode.WarnAndError;
                output = LogOutputMax;
                while (warningQueue.TryDequeue(out message))
                {
                    msg = message.ToString();
                    log = $"[{logTime}][Warning] {msg}";
                    LogWarningHandle?.Invoke(log);
                    Output?.Invoke(currTime, LogType.Warning, msg);
                    if (isWrite)
                        sb.AppendLine(log);
                    if (--output <= 0)
                        break;
                }
                isWrite = writeFileMode == WriteLogMode.All | writeFileMode == WriteLogMode.Error | writeFileMode == WriteLogMode.WarnAndError;
                output = LogOutputMax;
                while (errorQueue.TryDequeue(out message))
                {
                    msg = message.ToString();
                    log = $"[{logTime}][Error] {msg}";
                    LogErrorHandle?.Invoke(log);
                    Output?.Invoke(currTime, LogType.Error, msg);
                    if (isWrite)
                        sb.AppendLine(log);
                    if (--output <= 0)
                        break;
                }
                if (sb.Length > 0) //肯定有写入长度才大于0
                {
                    var bytes = Encoding.UTF8.GetBytes(sb.ToString());
                    fileStream.Write(bytes, 0, bytes.Length);
                    fileStream.Flush();
                }
            }
            catch (Exception ex)
            {
                errorQueue.Enqueue(ex.Message);
            }
            return true;
        }
#endif

        /// <summary>
        /// 输出调式消息
        /// </summary>
        /// <param name="message"></param>
        public static void Log(object message)
        {
            if (logQueue.Count >= LogMax)
                return;
#if SERVICE
            logQueue.Enqueue(message);
#else
            LogHandle?.Invoke($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}][Log] {message}");
            Output?.Invoke(DateTime.Now, LogType.Log, message.ToString());
#endif
        }

        /// <summary>
        /// 输出错误消息
        /// </summary>
        /// <param name="message"></param>
        public static void LogError(object message)
        {
            if (errorQueue.Count >= LogErrorMax)
                return;
#if SERVICE
            errorQueue.Enqueue(message);
#else
            LogErrorHandle?.Invoke($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}][Error] {message}");
            Output?.Invoke(DateTime.Now, LogType.Error, message.ToString());
#endif
        }

        /// <summary>
        /// 输出警告消息
        /// </summary>
        /// <param name="message"></param>
        public static void LogWarning(object message)
        {
            if (warningQueue.Count >= LogWarningMax)
                return;
#if SERVICE
            warningQueue.Enqueue(message);
#else
            LogWarningHandle?.Invoke($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}][Warning] {message}");
            Output?.Invoke(DateTime.Now, LogType.Warning, message.ToString());
#endif
        }

        public static void BindLogAll(Action<string> log)
        {
            BindLogAll(log, log, log);
        }

        public static void BindLogAll(Action<string> log, Action<string> warning, Action<string> error)
        {
            if (log != null) LogHandle += log;
            if (warning != null) LogWarningHandle += warning;
            if (error != null) LogErrorHandle += error;
        }

        public static void RemoveLogAll(Action<string> log)
        {
            RemoveLogAll(log, log, log);
        }

        public static void RemoveLogAll(Action<string> log, Action<string> warning, Action<string> error)
        {
            if (log != null) LogHandle -= log;
            if (warning != null) LogWarningHandle -= warning;
            if (error != null) LogErrorHandle -= error;
        }

        /// <summary>
        /// 绑定控制台输出
        /// </summary>
        public static void BindConsoleLog()
        {
            BindDebug(new ConsoleDebug());
        }

        /// <summary>
        /// 移除控制台输出
        /// </summary>
        public static void RemoveConsoleLog()
        {
            RemoveDebug();
        }

#if SERVICE && WINDOWS
        /// <summary>
        /// 绑定窗体程序输出
        /// </summary>
        public static void BindFormLog(ListBox listBox, Brush backgroundColor = null)
        {
            BindDebug(new FormDebug(listBox, backgroundColor));
        }

        /// <summary>
        /// 移除窗体程序输出
        /// </summary>
        public static void RemoveFormLog()
        {
            RemoveDebug();
        }
#endif

        /// <summary>
        /// 绑定输出接口
        /// </summary>
        /// <param name="log"></param>
        public static void BindDebug(IDebug debug)
        {
            if (NDebug.Debug != null)
                RemoveDebug();
            NDebug.Debug = debug;
            Output += debug.Output;
        }

        /// <summary>
        /// 移除输出接口
        /// </summary>
        public static void RemoveDebug()
        {
            if (Debug == null)
                return;
            Output -= Debug.Output;
            Debug = null;
        }
    }
}
namespace Net.Event
{
    using global::System;
    using global::System.Reflection;
    using Net.System;

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
        /// 输出日志最多容纳条数
        /// </summary>
        public static int LogMax { get; set; } = 500;
        /// <summary>
        /// 输出错误日志最多容纳条数
        /// </summary>
        public static int LogErrorMax { get; set; } = 500;
        /// <summary>
        /// 输出警告日志最多容纳条数
        /// </summary>
        public static int LogWarningMax { get; set; } = 500;

        private static QueueSafe<object> logQueue = new QueueSafe<object>();
        private static QueueSafe<object> errorQueue = new QueueSafe<object>();
        private static QueueSafe<object> warningQueue = new QueueSafe<object>();

#if SERVICE
        static NDebug()
        {
            Handler();
        }

        private static void Handler()
        {
            ThreadManager.Invoke("Debug-Log", ()=> 
            {
                try
                {
                    if (logQueue.TryDequeue(out object message))
                        LogHandle?.Invoke($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:ms")}][Log] {message}");
                    if (errorQueue.TryDequeue(out message))
                        LogErrorHandle?.Invoke($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:ms")}][Error] {message}");
                    if (warningQueue.TryDequeue(out message))
                        LogWarningHandle?.Invoke($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:ms")}][Warning] {message}");
                    if (logQueue.Count >= LogMax)
                        logQueue = new QueueSafe<object>();
                    if (errorQueue.Count >= LogErrorMax)
                        errorQueue = new QueueSafe<object>();
                    if (warningQueue.Count >= LogWarningMax)
                        warningQueue = new QueueSafe<object>();
                }
                catch (Exception ex)
                {
                    errorQueue.Enqueue(ex.Message);
                }
                return true;
            }, true);
        }
#endif

        /// <summary>
        /// 输出调式消息
        /// </summary>
        /// <param name="message"></param>
        public static void Log(object message)
        {
#if SERVICE
            logQueue.Enqueue(message);
#else
            LogHandle?.Invoke($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:ms")}][Log] {message}");
#endif
        }

        /// <summary>
        /// 输出错误消息
        /// </summary>
        /// <param name="message"></param>
        public static void LogError(object message)
        {
#if SERVICE
            errorQueue.Enqueue(message);
#else
            LogErrorHandle?.Invoke($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:ms")}][Error] {message}");
#endif
        }

        /// <summary>
        /// 输出警告消息
        /// </summary>
        /// <param name="message"></param>
        public static void LogWarning(object message)
        {
#if SERVICE
            warningQueue.Enqueue(message);
#else
            LogWarningHandle?.Invoke($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:ms")}][Warning] {message}");
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
    }
}
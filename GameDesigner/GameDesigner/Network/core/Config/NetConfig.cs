using System;
using System.Collections.Generic;
using System.IO;

namespace Net.Config
{
    public class Config 
    {
        private static bool init;
        private static bool useMemoryStream = true;
        /// <summary>
        /// 使用内存流进行缓存? 默认是文件流缓存, 速度会比较慢, 运行内存占用比较小!
        /// 使用内存流缓存速度会比较快, 但运行内存占用比较大
        /// </summary>
        [Obsolete("文件流已废弃, 统一内存流")]
        public static bool UseMemoryStream
        {
            get 
            {
                Init();
                return useMemoryStream;
            }
            set 
            {
                useMemoryStream = value;
                Save();
            }
        }
        private static int baseCapacity = 1024;
        /// <summary>
        /// 内存接收缓冲区基础容量 默认1024
        /// </summary>
        public static int BaseCapacity
        {
            get
            {
                Init();
                return baseCapacity;
            }
            set
            {
                if (baseCapacity == value)
                    return;
                baseCapacity = value;
                Save();
            }
        }

        /// <summary>
        /// 项目的持久路径, 网络需要处理文件时的目录
        /// </summary>
        public static string BasePath
        {
            get
            {
#if UNITY_STANDALONE || UNITY_WSA || UNITY_WEBGL || UNITY_ANDROID || UNITY_IOS
                var path = Unity.UnitySynchronizationContext.Get(() => {
                    return UnityEngine.Application.persistentDataPath;
                });
#else
                var path = AppDomain.CurrentDomain.BaseDirectory;
#endif
                return path;
            }
        }

        /// <summary>
        /// 配置文件的目录
        /// </summary>
        public static string ConfigPath
        {
            get 
            {
#if UNITY_STANDALONE || UNITY_WSA || UNITY_WEBGL || UNITY_ANDROID || UNITY_IOS
                var path = Unity.UnitySynchronizationContext.Get(() => { //根路径必须保证在项目内, 这样编译之后才能读取
#if UNITY_STANDALONE || UNITY_WSA || UNITY_WEBGL || UNITY_EDITOR
                    return UnityEngine.Application.streamingAssetsPath;//在编辑器时都是流路径
#elif UNITY_ANDROID || UNITY_IOS
                    return UnityEngine.Application.persistentDataPath;//编译之后才是持久路径
#endif
                });
#else
                var path = AppDomain.CurrentDomain.BaseDirectory;
#endif
                return path;
            }
        }

        private static string dataPath;
        /// <summary>
        /// 获取项目路径
        /// </summary>
        public static string DataPath 
        {
            get
            {
                if (!string.IsNullOrEmpty(dataPath))
                    return dataPath;
#if UNITY_STANDALONE || UNITY_WSA || UNITY_WEBGL || UNITY_ANDROID || UNITY_IOS
                dataPath = Unity.UnitySynchronizationContext.Get(() => { //根路径必须保证在项目内, 这样编译之后才能读取
                    return UnityEngine.Application.dataPath;
                });
#else
                dataPath = AppDomain.CurrentDomain.BaseDirectory;
#endif
                return dataPath;
            }
        }

        private static bool mainThreadTick = false;
        /// <summary>
        /// 在主线程处理所有网络功能? 否则会在多线程进行
        /// </summary>
        public static bool MainThreadTick 
        {
            get
            {
                Init();
                return mainThreadTick;
            }
            set
            {
                if (mainThreadTick == value)
                    return;
                mainThreadTick = value;
                Save();
            }
        }

        private static void Init()
        {
            if (init)
                return;
            init = true;
            var configPath = ConfigPath + "/network.config";
            if (File.Exists(configPath))
            {
                var textRows = File.ReadAllLines(configPath);
                foreach (var item in textRows)
                {
                    if (item.Contains("{"))//旧版本json存储
                    {
                        Save();
                        break;
                    }
                    var texts = item.Split('=');
                    var key = texts[0].Trim().ToLower();
                    var value = texts[1].Split('#')[0].Trim();
                    switch (key)
                    {
                        case "usememorystream":
                            if (bool.TryParse(value, out var value1))
                                useMemoryStream = value1;
                            else
                                useMemoryStream = value != "0";
                            break;
                        case "basecapacity":
                            baseCapacity = int.Parse(value);
                            break;
                        case "mainthreadtick":
                            if (bool.TryParse(value, out var value2))
                                mainThreadTick = value2;
                            else
                                mainThreadTick = value != "0";
                            break;
                    }
                }
            }
            else
            {
                Save();
            }
        }

        private static void Save()
        {
            var list = new List<string>();
            var text = $"useMemoryStream={useMemoryStream}#使用运行内存作为缓冲区? 否则使用文件流作为缓冲区";
            list.Add(text);
            text = $"baseCapacity={baseCapacity}#当客户端连接时分配的初始缓冲区大小";
            list.Add(text);
            text = $"mainThreadTick={mainThreadTick}#在主线程处理所有网络功能? 否则会在多线程进行";
            list.Add(text);
            var configPath = ConfigPath + "/network.config";
            var path = Path.GetDirectoryName(configPath);
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            File.WriteAllLines(configPath, list);
        }
    }
}

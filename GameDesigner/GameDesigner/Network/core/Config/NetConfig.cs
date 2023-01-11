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

        public static string GetBasePath()
        {
#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_WSA
#if UNITY_STANDALONE || UNITY_WSA
            var streamingAssetsPath = UnityEngine.Application.streamingAssetsPath;
            if (!Directory.Exists(streamingAssetsPath))
                Directory.CreateDirectory(streamingAssetsPath);
            var path = streamingAssetsPath;
#else
            var path = UnityEngine.Application.persistentDataPath;
#endif
#else
            var path = AppDomain.CurrentDomain.BaseDirectory;
#endif
            return path;
        }

        private static void Init()
        {
            if (init)
                return;
            init = true;
            var configPath = GetBasePath() + "/network.config";
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
                            useMemoryStream = bool.Parse(value);
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
            var text = $"useMemoryStream={useMemoryStream}#使用运行内存作为缓冲区? 否则使用文件流作为缓冲区";
            var configPath = GetBasePath() + "/network.config";
            File.WriteAllLines(configPath, new List<string>() { text });
        }
    }
}

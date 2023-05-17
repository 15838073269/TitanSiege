using Net.Helper;
using Net.Share;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Net.Config
{
    /// <summary>
    /// gdnet应用程序入口
    /// </summary>
    public static class App
    {
        /// <summary>
        /// 初始化GDNet环境
        /// </summary>
        public static void Setup() => Init();

        /// <summary>
        /// 初始化GDNet环境
        /// </summary>
        public static void Init()
        {
            //运行前初始化
            var dict = PersistHelper.Deserialize<Dictionary<string, string>>("runtimeOnLoad.json");
            if (dict.Count > 0) 
            {
                var assemblies = AppDomain.CurrentDomain.GetAssemblies();
                foreach (var assemblie in assemblies)
                {
                    foreach (var item in dict)
                    {
                        var type = assemblie.GetType(item.Key);
                        if (type == null)
                            continue;
                        var method = type.GetMethod(item.Value, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                        if (method == null)
                            continue;
                        method.Invoke(null, null);
                    }
                }
            }
        }

        /// <summary>
        /// 收集运行时初始化方法
        /// </summary>
        public static void Collect()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var dict = new Dictionary<string, string>();
            foreach (var assemblie in assemblies)
            {
                foreach (var type in assemblie.GetTypes().Where(t => !t.IsInterface))
                {
                    var members = type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
                    foreach (var member in members)
                    {
                        var runtimeInitialize = member.GetCustomAttribute<RuntimeInitializeOnLoadMethod>();
                        if (runtimeInitialize == null)
                            continue;
                        dict[type.FullName] = member.Name;
                    }
                }
            }
            PersistHelper.Serialize(dict, "runtimeOnLoad.json");
        }
    }
}

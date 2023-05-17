using Newtonsoft_X.Json;
using System.IO;

namespace Net.Helper 
{
    /// <summary>
    /// 持久化数据记录帮助类
    /// </summary>
    public class PersistHelper
    {
        public static bool Exists(string name)
        {
            var path = GetPath();
            if (!Directory.Exists(path))
                return false;
            var file = path + name;
            if (!File.Exists(file))
                return false;
            return true;
        }

        public static T Deserialize<T>(string name) where T : class, new()
        {
            return Deserialize(name, new T());
        }

        public static T Deserialize<T>(string name, T defaultValue = null) where T : class
        {
            var path = GetPath();
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            var file = path + name;
            if (!File.Exists(file))
                return defaultValue;
            var jsonStr = File.ReadAllText(file);
            var t = JsonConvert.DeserializeObject<T>(jsonStr, new JsonSerializerSettings() { ObjectCreationHandling = ObjectCreationHandling.Replace });
            if (t == null)
                return defaultValue;
            return t;
        }

        public static void Serialize<T>(T obj, string name)
        {
            var path = GetPath();
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            var file = path + name;
            var jsonStr = JsonConvert.SerializeObject(obj);
            File.WriteAllText(file, jsonStr);
        }

        private static string GetPath()
        {
#if UNITY_EDITOR
            var path = "ProjectSettings/gdnet/";
#else
        var path = Net.Config.Config.BasePath + "/ProjectSettings/gdnet/";
#endif
            return path;
        }

        public static string GetFilePath(string name)
        {
            var path = GetPath();
            var file = path + name;
            return file;
        }
    }
}
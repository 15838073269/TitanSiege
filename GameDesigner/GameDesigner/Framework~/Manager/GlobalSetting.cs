using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace Framework 
{
    public class GlobalSetting : ScriptableObject
    {
        public string Version = "1.0.1";
        public bool AutoIncrement = true;

        private static GlobalSetting instance;
        public static GlobalSetting Instance
        {
            get
            {
                if (instance == null)
                    instance = Resources.Load<GlobalSetting>("Global Setting");
                return instance;
            }
        }

        public string CheckAutoIncrement()
        {
            if (AutoIncrement)
            {
                var versions = Version.Split('.');
                var v1s = int.Parse(versions[0]);
                var v2s = int.Parse(versions[1]);
                var v3s = int.Parse(versions[2]);
                if (++v3s >= 10)
                {
                    v3s = 0;
                    if (++v2s >= 10)
                    {
                        v1s++;
                        v2s = 0;
                    }
                }
                Version = $"{v1s}.{v2s}.{v3s}";
#if UNITY_EDITOR
                UnityEditor.EditorUtility.SetDirty(this);
#endif
            }
            return Version;
        }

        public SortedDictionary<string, string> GetVersionPathDict(string root = "")
        {
            var versionFile = root + "AssetBundles/Version.txt";
            if (!File.Exists(versionFile))
                return new SortedDictionary<string, string>() { { "Version", "" } };
            var text = File.ReadAllText(versionFile);
            return GetVersionDict(text);
        }

        public SortedDictionary<string, string> GetVersionDict(string text)
        {
            var lines = text.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            var dict = new SortedDictionary<string, string>();
            for (int i = 0; i < lines.Length; i++)
            {
                if (string.IsNullOrEmpty(lines[i]))
                    continue;
                var line = lines[i].Split('|');
                dict[line[0]] = line[1];
            }
            return dict;
        }

        public void SaveVersionDict(SortedDictionary<string, string> dict, string root = "")
        {
            var versionFile = root + "AssetBundles/Version.txt";
            var sb = new StringBuilder();
            var dict1 = dict.Reverse();
            foreach (var item in dict1)
            {
                sb.AppendLine(item.Key + "|" + item.Value);
            }
            File.WriteAllText(versionFile, sb.ToString());
        }

        public static string ToMD5(string path)
        {
            var bytValue = File.ReadAllBytes(path);
            var md5 = new MD5CryptoServiceProvider();
            var bytHash = md5.ComputeHash(bytValue);
            string sTemp = "";
            for (int i = 0; i < bytHash.Length; i++)
                sTemp += bytHash[i].ToString("X").PadLeft(2, '0');
            var hash = sTemp.ToLower();
            return hash;
        }
    }
}
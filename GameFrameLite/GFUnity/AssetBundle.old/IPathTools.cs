using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace GF.Unity.AssetBundle {
    public class IPathTools {
        public static string GetPlatformFoldName(RuntimePlatform platform) {
            switch (platform) {
                case RuntimePlatform.WindowsPlayer:
                    return "Windows";
                case RuntimePlatform.WindowsEditor:
                    return "Windows";
                case RuntimePlatform.IPhonePlayer:
                    return "IPhone";
                case RuntimePlatform.Android:
                    return "Android";
                default:
                    return "Windows";
            }
        }
        public static string GetAssetBundlePath() {
            string platfolder = GetPlatformFoldName(Application.platform);
            string allpath = System.IO.Path.Combine(GetAppFilePath(), platfolder);
            return allpath;
        }
        public static string GetAppFilePath() {
            string tmpptah;
            if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor) {
                tmpptah = Application.streamingAssetsPath + "/AssetBundle/";
            } else {
                tmpptah = Application.persistentDataPath + "/AssetBundle/";
            }
            return tmpptah;
        }
    }
}


using Cysharp.Threading.Tasks;
using Net.Share;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
#if HYBRIDCLR
using HybridCLR;
#endif

namespace Framework
{
    public class AssetBundleCheckUpdate : MonoBehaviour
    {
        public string url = "http://192.168.1.5/";
        
        void Start()
        {
            var mode = Global.I.Mode;
            if (mode == AssetBundleMode.LocalPath | mode == AssetBundleMode.StreamingAssetsPath)
                _ = LocalLoadAB();
            else
                StartCoroutine(CheckUpdate());
        }

        private async UniTaskVoid LocalLoadAB()
        {
            await Global.Table.Init();
            Global.Resources.InitAssetBundleInfos();
            LoadMetadataForAOTAssemblies();
            Global.I.OnInit();
        }

        private IEnumerator CheckUpdate()
        {
            var versionUrl = url + "AssetBundles/Version.txt";
            var request = UnityWebRequest.Get(versionUrl);
            yield return request.SendWebRequest();
            if (!string.IsNullOrEmpty(request.error))
            {
                Debug.LogError($"{versionUrl} 获取失败:" + request.error);
                Global.UI.Message.ShowUI("资源请求", "版本信息请求失败!", result =>
                {
#if UNITY_EDITOR
                    UnityEditor.EditorApplication.isPlaying = false;
#endif
                    Application.Quit();
                });
                yield break;
            }
            var text = request.downloadHandler.text;
            request.Dispose();
            var abPath = Application.persistentDataPath + "/";
            var oldDict = GlobalSetting.Instance.GetVersionPathDict(abPath);
            var newDict = GlobalSetting.Instance.GetVersionDict(text);
            if (newDict["Version"] != oldDict["Version"])
            {
                bool msgClose = false;
                bool msgResult = false;
                Global.UI.Message.ShowUI("资源请求", "有版本需要更新", result =>
                {
                    msgClose = true;
                    msgResult = result;
                    if (!result)
                    {
#if UNITY_EDITOR
                        UnityEditor.EditorApplication.isPlaying = false;
#endif
                        Application.Quit();
                    }
                });
                while (!msgClose)
                    yield return null;
                if (!msgResult)
                    yield break;
                foreach (var item in newDict)
                {
                    if (item.Key == "Version")
                        continue;
                    if (oldDict.TryGetValue(item.Key, out var value))
                        if (item.Value == value)
                            continue;
                    var abUrl = url + item.Key;
                    var savePath = abPath + item.Key;
                    request = UnityWebRequest.Head(abUrl);
                    yield return request.SendWebRequest();
                    if (!string.IsNullOrEmpty(request.error))
                    {
                        Debug.LogError($"{abUrl} 获取资源失败:" + request.error);
                        yield break;
                    }
                    ulong totalLength = ulong.Parse(request.GetResponseHeader("Content-Length"));
                    request.Dispose();
                    request = new UnityWebRequest(abUrl, "GET", new DownloadHandlerFile(savePath), null);
                    request.SendWebRequest();
                    string progressText;
                    string name = Path.GetFileName(item.Key);
                    while (!request.isDone)
                    {
                        progressText = $"{name}下载进度:{ByteHelper.ToString(request.downloadedBytes)}/{ByteHelper.ToString(totalLength)}";
                        Global.UI.Loading.ShowUI(progressText, request.downloadProgress);
                        yield return null;
                    }
                    progressText = $"{name}下载完成!";
                    Global.UI.Loading.ShowUI(progressText, request.downloadProgress);
                    request.Dispose();
                }
                GlobalSetting.Instance.SaveVersionDict(newDict, abPath);
                yield return new WaitForSeconds(1f);
            }
            _ = LocalLoadAB();
        }

        /// <summary>
        /// 为aot assembly加载原始metadata， 这个代码放aot或者热更新都行。
        /// 一旦加载后，如果AOT泛型函数对应native实现不存在，则自动替换为解释模式执行
        /// </summary>
        private static void LoadMetadataForAOTAssemblies()
        {
#if HYBRIDCLR
            string path;
            if (Global.I.Mode == AssetBundleMode.LocalPath)
                path = Application.streamingAssetsPath + "/AssetBundles/Hotfix/";
            else
                path = Application.persistentDataPath + "/AssetBundles/Hotfix/";
            /// 注意，补充元数据是给AOT dll补充元数据，而不是给热更新dll补充元数据。
            /// 热更新dll不缺元数据，不需要补充，如果调用LoadMetadataForAOTAssembly会返回错误
            /// 
            if (!Directory.Exists(path))
                return;
            var files = Directory.GetFiles(path, "*.bytes");
            var mode = HomologousImageMode.SuperSet;
            foreach (var dllPath in files)
            {
                //if (dllPath.Contains("Assembly-CSharp.dll.bytes"))
                //    continue;
                var dllBytes = File.ReadAllBytes(dllPath);
                // 加载assembly对应的dll，会自动为它hook。一旦aot泛型函数的native函数不存在，用解释器版本代码
                var err = RuntimeApi.LoadMetadataForAOTAssembly(dllBytes, mode);
                Debug.Log($"LoadMetadataForAOTAssembly:{dllPath}. mode:{mode} ret:{err}");
            }
#if !UNITY_EDITOR
            var hotfixDll = path + "Assembly-CSharp.dll.bytes";
            if (File.Exists(hotfixDll))
                System.Reflection.Assembly.Load(File.ReadAllBytes(hotfixDll));
#endif
#endif
        }
    }
}
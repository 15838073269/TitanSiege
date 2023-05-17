using System.Data;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using Cysharp.Threading.Tasks;
using Net.Config;
using Net.Share;

namespace Framework
{
    public class TableManager : MonoBehaviour
    {
        private readonly TableConfig tableConfig = new TableConfig();

        internal async UniTask Init()
        {
            string path;
            if (Global.I.Mode == AssetBundleMode.LocalPath)
                path = Directory.GetCurrentDirectory() + $"/AssetBundles/Table/GameConfig.json";
            else if (Global.I.Mode == AssetBundleMode.StreamingAssetsPath)
                path = Application.streamingAssetsPath + $"/AssetBundles/Table/GameConfig.json";
            else
                path = Application.persistentDataPath + $"/AssetBundles/Table/GameConfig.json";
            using (var request = UnityWebRequest.Get(path)) 
            {
                var oper = request.SendWebRequest();
                while (!oper.isDone)
                {
                    await UniTask.Yield();
                }
                if (!string.IsNullOrEmpty(request.error))
                {
                    Global.Logger.LogError("����˵�GameDesigner/Framework/GenerateExcelData����execl������! " + request.error);
                    return;
                }
                var jsonStr = request.downloadHandler.text;
                tableConfig.LoadTable(jsonStr);
            }
        }

        /// <summary>
        /// ��ȡĳ����
        /// </summary>
        /// <param name="sheetName"></param>
        /// <returns></returns>
        public DataTable GetTable(string sheetName)
        {
            return tableConfig.GetTable(sheetName);
        }

        /// <summary>
        /// ��ȡexcel������ݣ�filterExpression��������: "Name = 'UI_Message'"
        /// </summary>
        /// <typeparam name="T">Ҫ��ȡ������</typeparam>
        /// <param name="filterExpression">���˱��ʽ</param>
        /// <returns></returns>
        public T GetDataConfig<T>(string filterExpression) where T : IDataConfig, new()
        {
            return tableConfig.GetDataConfig<T>(filterExpression);
        }

        /// <summary>
        /// ��ȡexcel������ݣ�filterExpression��������: "Name = 'UI_Message'"
        /// </summary>
        /// <typeparam name="T">Ҫ��ȡ������</typeparam>
        /// <param name="filterExpression">���˱��ʽ</param>
        /// <returns></returns>
        public T[] GetDataConfigs<T>(string filterExpression) where T : IDataConfig, new()
        {
            return tableConfig.GetDataConfigs<T>(filterExpression);
        }
    }
}
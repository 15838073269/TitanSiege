using UnityEditor;
using System.IO;
using ExcelDataReader;
using UnityEngine;
using System;
using System.Data;
using System.Text;

public class ExcelTools
{
    [MenuItem("GameDesigner/Framework/GenerateExcelData", priority = 2)]
    public static void GenerateExcelData()
    {
        string path = "AssetBundles/Table/GameConfig.json";
        if (!Directory.Exists(Path.GetDirectoryName(path)))
            Directory.CreateDirectory(Path.GetDirectoryName(path));
        string excelPath = "Tools/Excel/GameConfig.xls";
        var temp = excelPath + ".temp";
        File.Copy(excelPath, temp, true);
        try
        {
            using (var stream = new FileStream(temp, FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    var dataSet = reader.AsDataSet();
                    var jsonStr = Newtonsoft_X.Json.JsonConvert.SerializeObject(dataSet);
                    File.WriteAllText(path, jsonStr);
                    Debug.Log("生成表格数据完成! " + Environment.CurrentDirectory + "/" + path);
                }
            }
        }
        finally
        {
            File.Delete(temp);
        }
    }

    [MenuItem("GameDesigner/Framework/GenerateExcelDataToCs", priority = 3)]
    public static void GenerateExcelDataToCs()
    {
        string excelPath = "Tools/Excel/GameConfig.xls";
        var temp = excelPath + ".temp";
        File.Copy(excelPath, temp, true);
        try
        {
            using (var stream = new FileStream(temp, FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    var dataSet = reader.AsDataSet();
                    var files = Directory.GetFiles(Application.dataPath, "DataConfigScript.txt", SearchOption.AllDirectories);
                    var excelPath1 = "";
                    foreach (var file in files)
                    {
                        var info = new FileInfo(file);
                        if (info.Directory.Name == "Template")
                        {
                            excelPath1 = file;
                            break;
                        }
                    }
                    var text = File.ReadAllText(excelPath1);
                    foreach (DataTable table in dataSet.Tables)
                    {
                        var text1 = text;
                        text1 = text1.Replace("SHEETNAME", table.TableName);
                        var text2 = text1.Split(new string[] { "SPLIT" }, 0);
                        var sb = new StringBuilder();
                        var sb1 = new StringBuilder();
                        sb.Append(text2[0]);
                        for (int i = 1; i < table.Columns.Count; i++)
                        {
                            var name = table.Rows[0][i].ToString();
                            var typeStr = table.Rows[1][i].ToString();
                            var des = table.Rows[2][i].ToString();

                            var text3 = text2[1].Replace("NAME", name);
                            text3 = text3.Replace("TYPE", typeStr);
                            text3 = text3.Replace("NOTE", des);
                            text3 = text3.TrimStart('\r', '\n');
                            //text3 = text3.TrimEnd('\n', '\r');
                            sb.AppendLine(text3);

                            var text4 = text2[3].Replace("NAME", name);
                            typeStr = typeStr[0].ToString().ToUpper() + typeStr.Substring(1, typeStr.Length - 1);
                            text4 = text4.Replace("TYPE", typeStr);
                            text4 = text4.TrimStart('\r', '\n');
                            text4 = text4.TrimEnd('\n', '\r');
                            sb1.AppendLine(text4);
                        }
                        sb.Append(text2[2]);
                        sb.Append(sb1.ToString());
                        sb.Append(text2[4]);
                        var text5 = sb.ToString();
                        File.WriteAllText($"Assets/Scripts/Data/Config/{table.TableName}DataConfig.cs", text5);
                        Debug.Log($"生成表:{table.TableName}完成!");
                    }
                    Debug.Log("全部表生成完毕!");
                    AssetDatabase.Refresh();
                }
            }
        }
        finally
        {
            File.Delete(temp);
        }
    }
}

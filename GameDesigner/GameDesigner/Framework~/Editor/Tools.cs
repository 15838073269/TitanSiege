#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

public class Tools
{
	[MenuItem("CONTEXT/MonoBehaviour/ChangeFieldNames")]
	private static void ChangeFieldNames(MenuCommand menuCommand)
	{
		var target = menuCommand.context;
		var type = target.GetType();
		var fields = type.GetFields(BindingFlags.Instance| BindingFlags.Public| BindingFlags.NonPublic);
        foreach (var field in fields)
        {
			if (!field.FieldType.IsSubclassOf(typeof(Object))) 
				continue;
			var value = field.GetValue(target);
			if (value == null)
				continue;
			var obj = value as Object;
			obj.name = field.Name;
		}
		EditorUtility.SetDirty(target);
	}

	[MenuItem("GameDesigner/Framework/OpenGameConfig", priority = 5)]
	private static void OpenExcel()
	{
		var path = "Tools/Excel/GameConfig.xls";
		InternalEditorUtility.OpenFileAtLineExternal(path, 0);
	}

	[MenuItem("GameDesigner/Framework/Install", priority = 1)]
	private static void Install()
	{
		var path = "Tools/Excel/";
		if (!Directory.Exists(path))
			Directory.CreateDirectory(path);
		var files = Directory.GetFiles(Application.dataPath, "GameConfig.xls", SearchOption.AllDirectories);
		var excelPath = "";
		foreach (var file in files)
        {
			var info = new FileInfo(file);
			if (info.Directory.Name == "Template")
			{
				excelPath = file;
				break;
			}
        }
		var excelPath1 = path + "GameConfig.xls";
		if (!File.Exists(excelPath1))//如果存在表则不能复制进去了, 避免使用者数据丢失
			File.Copy(excelPath, excelPath1);
		Debug.Log($"复制配置表格文件完成:{excelPath1}");

		var paths = new List<string>() { 
			"Assets/Scripts/Data/DB/", "Assets/Scripts/Data/DBExt/", "Assets/Scripts/Data/Proto/",
			"Assets/Scripts/Data/Binding/", "Assets/Scripts/Data/BindingExt/", "Assets/Resources/Audio",
			"Assets/Resources/Prefabs", "Assets/Resources/UI", "Assets/Scripts/Data/Config", "Assets/Scripts/Data/ConfigExt",
			"Assets/Scripts/Framework/Manager",
		};

        foreach (var item in paths)
        {
			if (!Directory.Exists(item))
				Directory.CreateDirectory(item);
			Debug.Log($"创建的脚本路径:{item}");
		}

		path = "Assets/Plugins/GameDesigner/Framework/Template/Global.txt";
		excelPath1 = "Assets/Scripts/Framework/Manager/Global.cs";
		if(!File.Exists(excelPath1))
			File.Copy(path, excelPath1);

		path = "Assets/Plugins/GameDesigner/Framework/Template/UIManager.txt";
		excelPath1 = "Assets/Scripts/Framework/Manager/UIManager.cs";
		if (!File.Exists(excelPath1))
			File.Copy(path, excelPath1);

		path = "Assets/Plugins/GameDesigner/Framework/Template/TableManager.txt";
		excelPath1 = "Assets/Scripts/Framework/Manager/TableManager.cs";
		if (!File.Exists(excelPath1))
			File.Copy(path, excelPath1);

        path = "Assets/Plugins/GameDesigner/Framework/Template/EventCommand.txt";
        excelPath1 = "Assets/Scripts/Data/EventCommand.cs";
        if (!File.Exists(excelPath1))
            File.Copy(path, excelPath1);

        Debug.Log($"环境安装完成!");
		AssetDatabase.Refresh();
	}
}
#endif
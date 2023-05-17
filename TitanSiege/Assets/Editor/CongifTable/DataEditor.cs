using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using System.IO;
using System.Xml;
using OfficeOpenXml;
using System.ComponentModel;
using GF.ConfigTable;
using Unity.VisualScripting;

public class DataEditor
{
    public static string XmlPath = GF.MainGame.AppConfig.XmlPath;
    public static string BinaryPath = GF.MainGame.AppConfig.BinaryPath;
    public static string ScriptsPath = GF.MainGame.AppConfig.ScriptsPath;
    public static string ExcelPath = GF.MainGame.AppConfig.ExcelPath;
    public static string RegPath = GF.MainGame.AppConfig.RegPath;
    /// <summary>
    /// 类结构转成xml
    /// </summary>
    [MenuItem("Assets/类转xml")]
    public static void AssetsClassToXml()
    {
        UnityEngine.Object[] objs = Selection.objects;//获取选中的对象
        //遍历
        for (int i = 0; i < objs.Length; i++)
        {
            //开启编辑器ui进度条
            EditorUtility.DisplayProgressBar("文件下的类转成xml", $"正在扫描{objs[i].name}... ...", 1.0f / objs.Length * i);
            //根据类名找到类，objs[i].name是类名
            ClassToXml(objs[i].name);
        }
        AssetDatabase.Refresh();
        EditorUtility.ClearProgressBar();
    }
    /// <summary>
    /// 配置表类所在的命名空间
    /// </summary>
    private static string m_namespace = "GF.MainGame.Data";
    /// <summary>
    /// 配置表类所在的程序集
    /// </summary>
    private static string m_assemblyName = "Assembly-CSharp";
    /// <summary>
    /// 实际的类转XML
    /// </summary>
    /// <param name="name">类名</param>
    private static void ClassToXml(string name) {
        if (string.IsNullOrEmpty(name))
            return;

        try {
            Type type = null;
            //AppDomain.CurrentDomain.GetAssemblies() 获取到主程序集，在主程序集中寻找这个类，返回Assembly数组
            //注意，数据表类不能写在其他命名空间内，否则找不到，AppDomain.CurrentDomain.GetAssemblies()只会获取运行时的程序集
            //foreach (var asm in AppDomain.CurrentDomain.GetAssemblies()) {//如果有类名重复，就只找到第一个，所以类名最好不要重复
            //    Debug.Log(asm.GetName());
            //    Type tempType = asm.GetType(name);//在程序集中，根据类名获取类
            //    if (tempType != null) {
            //        type = tempType;
            //        break;
            //    }
            //}
           //换这种直接通过程序集和命名空间的形式获取类的type，当然这相当于写死了
            type = Type.GetType($"{m_namespace}.{name},{m_assemblyName}");
            if (type != null) {
                var temp = Activator.CreateInstance(type);//通过反射new这个类的对象
                if (temp is ExcelBase) {//执行构造方法
                    (temp as ExcelBase).Construction();
                }
                string xmlPath = $"{XmlPath}{name}.xml";
                BinarySerializeOpt.Xmlserialize(xmlPath, temp);//通过工具转换
                Debug.Log(name + "类转xml成功，xml路径为:" + xmlPath);
            } else {
                Debug.Log($"未在程序集中找到{name}脚本");
            }
        } catch(Exception e) {
            Debug.LogError(name + "类转xml失败！错误内容："+e.Message);
        }
    }

    [MenuItem("Assets/Xml转Binary")]
    public static void AssetsXmlToBinary()
    {
        UnityEngine.Object[] objs = Selection.objects;
        for (int i = 0; i < objs.Length; i++)
        {
            EditorUtility.DisplayProgressBar("文件下的xml转成二进制", $"正在扫描{objs[i].name}... ...", 1.0f / objs.Length * i);
            XmlToBinary(objs[i].name);
        }
        AssetDatabase.Refresh();
        EditorUtility.ClearProgressBar();
    }
    [MenuItem("Tools/Xml/Xml转成二进制")]
    public static void AllXmlToBinary() {
        string path = Application.dataPath.Replace("Assets", "") + XmlPath;
        //获取整个文件夹下所有文件路径
        string[] filesPath = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);
        for (int i = 0; i < filesPath.Length; i++) {
            EditorUtility.DisplayProgressBar("查找文件夹下面的Xml", $"正在扫描{filesPath[i]}... ...", 1.0f / filesPath.Length * i);
            if (filesPath[i].EndsWith(".xml")) {
                string tempPath = filesPath[i].Substring(filesPath[i].LastIndexOf("/") + 1);
                tempPath = tempPath.Replace(".xml", "");
                XmlToBinary(tempPath);
            }
        }
        AssetDatabase.Refresh();
        EditorUtility.ClearProgressBar();
    }


    /// <summary>
    /// xml转二进制
    /// </summary>
    /// <param name="name"></param>
    private static void XmlToBinary(string name) {
        if (string.IsNullOrEmpty(name))
            return;

        try {
            Type type = null;
            //AppDomain.CurrentDomain.GetAssemblies() 获取到主程序集，在主程序集中寻找这个类，返回Assembly数组
            //注意，数据表类不能写在其他命名空间内，否则找不到AppDomain.CurrentDomain.GetAssemblies()只会获取运行时的程序集
            //foreach (var asm in AppDomain.CurrentDomain.GetAssemblies()) {
            //    Type tempType = asm.GetType(name);
            //    if (tempType != null) {
            //        type = tempType;
            //        break;
            //    }
            //}
            //换这种直接通过程序集和命名空间的形式获取类的type
            type = Type.GetType($"{m_namespace}.{name},{m_assemblyName}");
            if (type != null) {
                string xmlPath = XmlPath + name + ".xml";
                string binaryPath = BinaryPath + name + ".bytes";
                //xml反序列化成对象
                object obj = BinarySerializeOpt.XmlDeserialize(xmlPath, type);
                //将对象转成二进制文件
                BinarySerializeOpt.BinarySerilize(binaryPath, obj);
                Debug.Log(name + "xml转二进制成功，二进制路径为:" + binaryPath);
            }
        } catch {
            Debug.LogError(name + "xml转二进制失败！");
        }
    }
    #region XML转excel
    /// <summary>
    /// Xml转成excel
    /// </summary>
    ///需要注意的是，功能的类转xml，如果类中的赋值数据没有赋全，生成的xml中也可能不会出现未赋值的属性，但此时如果用类生成的xml转excel就会报错。
    ///简单来说，转成excel的xml必须严格按照reg.xml中规定的属性填写完全，否则转换会报空引用异常
    [MenuItem("Assets/Xml转Excel")]
    public static void AssetsXmlToExcel()
    {
        UnityEngine.Object[] objs = Selection.objects;
        for (int i = 0; i < objs.Length; i++)
        {
            EditorUtility.DisplayProgressBar("文件下的xml转成Excel", $"正在扫描{objs[i].name}... ...", 1.0f / objs.Length * i);
            XmlToExcel(objs[i].name);
        }
        AssetDatabase.Refresh();
        EditorUtility.ClearProgressBar();
    }
    /// <summary>
    /// 反序列化xml到类
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    private static object GetObjFormXml(string name) {
        Type type = null;
        //AppDomain.CurrentDomain.GetAssemblies() 获取到主程序集，在主程序集中寻找这个类，返回Assembly数组
        //注意，数据表类不能写在其他命名空间内，否则找不到AppDomain.CurrentDomain.GetAssemblies()只会获取运行时的程序集
        //foreach (var asm in AppDomain.CurrentDomain.GetAssemblies()) {
        //    Type tempType = asm.GetType(name);
        //    if (tempType != null) {
        //        type = tempType;
        //        break;
        //    }
        //}
        //换这种直接通过程序集和命名空间的形式获取类的type
        type = Type.GetType($"{m_namespace}.{name},{m_assemblyName}");
        if (type != null) {
            string xmlPath = XmlPath + name + ".xml";
            return BinarySerializeOpt.XmlDeserialize(xmlPath, type);
        }

        return null;
    }
    private static void XmlToExcel(string name) {
        string className = "";
        string xmlName = "";
        string excelName = "";
        //第一步，读取regxml获取xml的结构，并把所有包含的list结构对象sheetclass存储到allSheetClassDic中，并映射回来表名，xml名称，和类名
        Dictionary<string, SheetClass> allSheetClassDic = ReadReg(name, ref excelName, ref xmlName, ref className);
        //将xml及数据转换成object对象
        object data = GetObjFormXml(className);
        //定义list通过循环获取最外层的list
        List<SheetClass> outSheetList = new List<SheetClass>();
        foreach (SheetClass sheetClass in allSheetClassDic.Values) {
            if (sheetClass.Depth == 1) {
                outSheetList.Add(sheetClass);
            }
        }
        //读取最外层的list数据
        Dictionary<string, SheetData> sheetDataDic = new Dictionary<string, SheetData>();
        for (int i = 0; i < outSheetList.Count; i++) {
            //从object对象中读取每一列的数据，并存在sheetDataDic中
            ReadData(data, outSheetList[i], allSheetClassDic, sheetDataDic, "");
        }
        //获取excel路径
        string xlsxPath = ExcelPath + excelName;
        if (FileIsUsed(xlsxPath)) {
            Debug.LogError("文件被占用，无法修改");
            return;
        }

        try {
            //通过文件写入将sheetDataDic循环写入excel表中
            FileInfo xlsxFile = new FileInfo(xlsxPath);
            if (xlsxFile.Exists) {
                xlsxFile.Delete();
                xlsxFile = new FileInfo(xlsxPath);
            }
            using (ExcelPackage package = new ExcelPackage(xlsxFile)) {
                foreach (string str in sheetDataDic.Keys) {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(str);
                    SheetData sheetData = sheetDataDic[str];
                    //先在第一行写入所有字段名
                    for (int i = 0; i < sheetData.AllName.Count; i++) {
                        ExcelRange range = worksheet.Cells[1, i + 1];
                        range.Value = sheetData.AllName[i];
                        range.AutoFitColumns();
                    }
                    //循环写入每一行的数据
                    for (int i = 0; i < sheetData.AllData.Count; i++) {
                        RowData rowData = sheetData.AllData[i];
                        for (int j = 0; j < sheetData.AllData[i].RowDataDic.Count; j++) {
                            ExcelRange range = worksheet.Cells[i + 2, j + 1];
                            string vaule = rowData.RowDataDic[sheetData.AllName[j]];
                            range.Value = vaule;
                            range.AutoFitColumns();
                            if (vaule.Contains("\n") || vaule.Contains("\r\n")) {
                                range.Style.WrapText = true;
                            }
                        }
                    }

                    worksheet.Cells.AutoFitColumns();
                }
                package.Save();
            }
        } catch (Exception e) {
            Debug.LogError(e);
            return;
        }
        Debug.Log("生成" + xlsxPath + "成功！！！");
    }
    #endregion
    #region excel转xml及二进制
    /// <summary>
    /// excel转xml
    /// </summary>
    [MenuItem("Tools/Xml/Excel转Xml")]
    public static void AllExcelToXml()
    {
        string[] filePaths = Directory.GetFiles(RegPath, "*", SearchOption.AllDirectories);
        for (int i = 0; i < filePaths.Length; i++)
        {
            if (!filePaths[i].EndsWith(".xml"))
                continue;
            EditorUtility.DisplayProgressBar("查找文件夹下的类", $"正在扫描{filePaths[i]}... ...", 1.0f / filePaths.Length * i);
            string path = filePaths[i].Substring(filePaths[i].LastIndexOf("/") + 1);
            ExcelToXml(path.Replace(".xml", ""));
        }

        AssetDatabase.Refresh();
        EditorUtility.ClearProgressBar();
    }

    private static void ExcelToXml(string name)
    {
        string className = "";
        string xmlName = "";
        string excelName = "";
        //第一步，读取Reg文件，确定类的结构
        Dictionary<string, SheetClass> allSheetClassDic = ReadReg(name, ref excelName, ref xmlName, ref className);

        //第二步，读取excel里面的数据
        string excelPath = ExcelPath + excelName;
        Dictionary<string, SheetData> sheetDataDic = new Dictionary<string, SheetData>();
        try
        {
            //注意FileShare参数必须为FileShare.ReadWrite或者write，才能在文件打开的情况下，读取数据，否则，文件处于打开情况下，会无法读取
            using (FileStream stream = new FileStream(excelPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (ExcelPackage package = new ExcelPackage(stream))
                {
                    ExcelWorksheets worksheetArray = package.Workbook.Worksheets;
                    for (int i = 0; i < worksheetArray.Count; i++)
                    {
                        SheetData sheetData = new SheetData();
                        ExcelWorksheet worksheet = worksheetArray[i + 1];//excel无论行列还是sheet表都是从1开始
                        SheetClass sheetClass = allSheetClassDic[worksheet.Name];
                        int colCount = worksheet.Dimension.End.Column;
                        int rowCount = worksheet.Dimension.End.Row;

                        for (int n = 0; n < sheetClass.VarList.Count; n++)
                        {
                            sheetData.AllName.Add(sheetClass.VarList[n].Name);
                            sheetData.AllType.Add(sheetClass.VarList[n].Type);
                        }

                        for (int m = 1; m < rowCount; m++)
                        {
                            RowData rowData = new RowData();
                            int n = 0;
                            if (string.IsNullOrEmpty(sheetClass.SplitStr) && sheetClass.ParentVar != null
                                && !string.IsNullOrEmpty(sheetClass.ParentVar.Foregin))
                            {
                                //worksheet.Cells[m + 1, 1].Value.ToString().Trim().Replace(" ",'');
                                //Trim()取前后空格，Replace(" ",'')可以去字符串内的所有空格，按需使用
                                rowData.ParnetVlue = worksheet.Cells[m + 1, 1].Value.ToString().Trim();
                                n = 1;
                            }
                            for (; n < colCount; n++)
                            {
                                ExcelRange range = worksheet.Cells[m + 1, n + 1];
                                string value = "";
                                if (range.Value != null)
                                {
                                    value = range.Value.ToString().Trim();
                                }
                                string colValue = worksheet.Cells[1, n + 1].Value.ToString().Trim();
                                rowData.RowDataDic.Add(GetNameFormCol(sheetClass.VarList, colValue), value);
                            }

                            sheetData.AllData.Add(rowData);
                        }
                        sheetDataDic.Add(worksheet.Name, sheetData);
                    }
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            return;
        }

        //根据类的结构，创建类，并且给每个变量赋值（从excel里读出来的值）
        object objClass = CreateClass(className);

        List<string> outKeyList = new List<string>();
        foreach (string str in allSheetClassDic.Keys)
        {
            SheetClass sheetClass = allSheetClassDic[str];
            if (sheetClass.Depth == 1)
            {
                outKeyList.Add(str);
            }
        }

        for (int i = 0; i < outKeyList.Count; i++)
        {
            ReadDataToClass(objClass, allSheetClassDic[outKeyList[i]], sheetDataDic[outKeyList[i]], allSheetClassDic, sheetDataDic, null);
        }
        //打包成xml
        BinarySerializeOpt.Xmlserialize(XmlPath + xmlName, objClass);
        //打包成二进制
        //BinarySerializeOpt.BinarySerilize(BinaryPath + className + ".bytes", objClass);
        Debug.Log(excelName + "表导入unity完成！");
        AssetDatabase.Refresh();
    }
    /// <summary>
    /// 从excel中读取数据到类的对象
    /// </summary>
    /// <param name="objClass"></param>
    /// <param name="sheetClass"></param>
    /// <param name="sheetData"></param>
    /// <param name="allSheetClassDic"></param>
    /// <param name="sheetDataDic"></param>
    /// <param name="keyValue"></param>
    private static void ReadDataToClass(object objClass, SheetClass sheetClass, SheetData sheetData, Dictionary<string, SheetClass> allSheetClassDic, Dictionary<string, SheetData> sheetDataDic, object keyValue) {
        object item = CreateClass(sheetClass.Name);//只是为了得到变量类型
        object list = CreateList(item.GetType());
        for (int i = 0; i < sheetData.AllData.Count; i++) {
            if (keyValue != null && !string.IsNullOrEmpty(sheetData.AllData[i].ParnetVlue)) {
                if (sheetData.AllData[i].ParnetVlue != keyValue.ToString())
                    continue;
            }
            object addItem = CreateClass(sheetClass.Name);
            for (int j = 0; j < sheetClass.VarList.Count; j++) {
                VarClass varClass = sheetClass.VarList[j];
                if (varClass.Type == "list" && string.IsNullOrEmpty(varClass.SplitStr)) {
                    ReadDataToClass(addItem, allSheetClassDic[varClass.ListSheetName], sheetDataDic[varClass.ListSheetName], allSheetClassDic, sheetDataDic, GetMemberValue(addItem, sheetClass.MainKey));
                } else if (varClass.Type == "list") {
                    string value = sheetData.AllData[i].RowDataDic[sheetData.AllName[j]];
                    SetSplitClass(addItem, allSheetClassDic[varClass.ListSheetName], value);
                } else if (varClass.Type == "listStr" || varClass.Type == "listFloat" || varClass.Type == "listInt" || varClass.Type == "listBool" || varClass.Type == "listUshort" || varClass.Type == "listShort") {
                    string value = sheetData.AllData[i].RowDataDic[sheetData.AllName[j]];
                    SetSplitBaseClass(addItem, varClass, value);
                } else {
                    string value = sheetData.AllData[i].RowDataDic[sheetData.AllName[j]];
                    if (string.IsNullOrEmpty(value) && !string.IsNullOrEmpty(varClass.DefaultValue)) {
                        value = varClass.DefaultValue;
                    }
                    if (string.IsNullOrEmpty(value)) {
                        Debug.LogError("表格中有空数据，或者Reg文件未配置defaultValue！" + sheetData.AllName[j]);
                        continue;
                    }
                    SetValue(addItem.GetType().GetProperty(sheetData.AllName[j]), addItem, value, sheetData.AllType[j]);
                }
            }
            list.GetType().InvokeMember("Add", BindingFlags.Default | BindingFlags.InvokeMethod, null, list, new object[] { addItem });
        }
        objClass.GetType().GetProperty(sheetClass.ParentVar.Name).SetValue(objClass, list);
    }
    #endregion
    /// <summary>
    /// 读取reg表结构
    /// </summary>
    /// <param name="name"></param>
    /// <param name="excelName"></param>
    /// <param name="xmlName"></param>
    /// <param name="className"></param>
    /// <returns></returns>
    private static Dictionary<string, SheetClass> ReadReg(string name, ref string excelName, ref string xmlName, ref string className) {
        string regPath = RegPath + name + ".xml";
        if (!File.Exists(regPath)) {
            Debug.LogError("此数据不存在配置变化xml：" + name);
        }
        XmlDocument xml = new XmlDocument();
        XmlReader reader = XmlReader.Create(regPath);
        XmlReaderSettings settings = new XmlReaderSettings();
        settings.IgnoreComments = true;//忽略xml里面的注释
        xml.Load(reader);
        XmlNode xn = xml.SelectSingleNode("data");
        XmlElement xe = (XmlElement)xn;
        className = xe.GetAttribute("name");
        xmlName = xe.GetAttribute("to");
        excelName = xe.GetAttribute("from");
        //储存所有变量的表
        Dictionary<string, SheetClass> allSheetClassDic = new Dictionary<string, SheetClass>();
        ReadXmlNode(xe, allSheetClassDic, 0);
        reader.Close();
        return allSheetClassDic;
    }
    /// <summary>
    /// 递归读取配置
    /// </summary>
    /// <param name="xmlElement">xml元素</param>
    /// <param name="allSheetClassDic">xml所有变量的字典</param>
    /// <param name="depth">深度，用来判断是不是最外层的list</param>
    private static void ReadXmlNode(XmlElement xmlElement, Dictionary<string, SheetClass> allSheetClassDic, int depth) {
        depth++;
        foreach (XmlNode node in xmlElement.ChildNodes) {
            XmlElement xe = (XmlElement)node;
            if (xe.GetAttribute("type") == "list") {
                XmlElement listEle = (XmlElement)node.FirstChild;
                //先把list父节点的variable赋值
                VarClass parentVar = new VarClass() {
                    Name = xe.GetAttribute("name"),
                    Type = xe.GetAttribute("type"),
                    Col = xe.GetAttribute("col"),
                    DefaultValue = xe.GetAttribute("defaultValue"),
                    Foregin = xe.GetAttribute("foregin"),
                    SplitStr = xe.GetAttribute("split"),
                };
                if (parentVar.Type == "list") {
                    parentVar.ListName = ((XmlElement)xe.FirstChild).GetAttribute("name");
                    parentVar.ListSheetName = ((XmlElement)xe.FirstChild).GetAttribute("sheetname");
                }

                SheetClass sheetClass = new SheetClass() {
                    Name = listEle.GetAttribute("name"),
                    SheetName = listEle.GetAttribute("sheetname"),
                    SplitStr = listEle.GetAttribute("split"),
                    MainKey = listEle.GetAttribute("mainKey"),
                    ParentVar = parentVar,
                    Depth = depth,
                };

                if (!string.IsNullOrEmpty(sheetClass.SheetName)) {
                    if (!allSheetClassDic.ContainsKey(sheetClass.SheetName)) {
                        //获取该类下面所有变量
                        foreach (XmlNode insideNode in listEle.ChildNodes) {
                            XmlElement insideXe = (XmlElement)insideNode;

                            VarClass varClass = new VarClass() {
                                Name = insideXe.GetAttribute("name"),
                                Type = insideXe.GetAttribute("type"),
                                Col = insideXe.GetAttribute("col"),
                                DefaultValue = insideXe.GetAttribute("defaultValue"),
                                Foregin = insideXe.GetAttribute("foregin"),
                                SplitStr = insideXe.GetAttribute("split"),
                            };
                            if (varClass.Type == "list") {
                                varClass.ListName = ((XmlElement)insideXe.FirstChild).GetAttribute("name");
                                varClass.ListSheetName = ((XmlElement)insideXe.FirstChild).GetAttribute("sheetname");
                            }

                            sheetClass.VarList.Add(varClass);
                        }
                        allSheetClassDic.Add(sheetClass.SheetName, sheetClass);
                    }
                }

                ReadXmlNode(listEle, allSheetClassDic, depth);
            }
        }
    }
   

    /// <summary>
    /// 自定义类List赋值
    /// </summary>
    /// <param name="objClass"></param>
    /// <param name="sheetClass"></param>
    /// <param name="value"></param>
    private static void SetSplitClass(object objClass, SheetClass sheetClass, string value)
    {
        object item = CreateClass(sheetClass.Name);
        object list = CreateList(item.GetType());
        if (string.IsNullOrEmpty(value))
        {
            Debug.Log("excel里面自定义list的列里有空值！" + sheetClass.Name);
            return;
        }
        else
        {
            string splitStr = sheetClass.ParentVar.SplitStr.Replace("\\n", "\n").Replace("\\r", "\r");
            string[] rowArray = value.Split(new string[] { splitStr }, StringSplitOptions.None);
            for (int i = 0; i < rowArray.Length; i++)
            {
                object addItem = CreateClass(sheetClass.Name);
                string[] valueList = rowArray[i].Trim().Split(new string[] { sheetClass.SplitStr }, StringSplitOptions.None);
                for (int j = 0; j < valueList.Length; j++)
                {
                    SetValue(addItem.GetType().GetProperty(sheetClass.VarList[j].Name), addItem, valueList[j].Trim(), sheetClass.VarList[j].Type);
                }
                list.GetType().InvokeMember("Add", BindingFlags.Default | BindingFlags.InvokeMethod, null, list, new object[] { addItem });
            }

        }
        objClass.GetType().GetProperty(sheetClass.ParentVar.Name).SetValue(objClass, list);
    }

    /// <summary>
    /// 基础List赋值
    /// </summary>
    /// <param name="objClass"></param>
    /// <param name="varClass"></param>
    /// <param name="value"></param>
    private static void SetSplitBaseClass(object objClass, VarClass varClass, string value)
    {
        Type type = null;
        if (varClass.Type == "listStr")
        {
            type = typeof(string);
        }
        else if (varClass.Type == "listFloat")
        {
            type = typeof(float);
        }
        else if (varClass.Type == "listInt")
        {
            type = typeof(int);
        }
        else if (varClass.Type == "listBool")
        {
            type = typeof(bool);
        } else if (varClass.Type == "listShort") {

            type = typeof(short);
        } else if (varClass.Type == "listUshort") {

            type = typeof(ushort);
        }
        object list = CreateList(type);
        //经过多次测试，发现这里不用反射，直接获取类型也能用，用反射只是为了提高代码重用
        //用反射添加string之外的类型，例如ushort\int等会报错，提示添加失败，但直接add不会，原因未找到，所以关于此类类型，特殊处理一下
        List<ushort> list1 = null;
        List<short> list2 = null;
        if (type == typeof(ushort)) {
            list1 = list as List<ushort>;
        } else if(type == typeof(short)) {
            list2 = list as List<short>;
        }
        //varClass.SplitStr是字符串，Split方法只能使用char字符分隔，所以需要用new string[]这种重载
        string[] rowArray = value.Split(new string[] { varClass.SplitStr }, StringSplitOptions.None);
        for (int i = 0; i < rowArray.Length; i++)
        {
            object addItem = rowArray[i].Trim();
            try
            {
                //经过多次测试，发现这里不用反射，直接获取类型也能用，用反射只是为了提高代码重用
                //用反射添加string之外的类型，例如ushort等会报错，提示添加失败，但直接add不会，原因未找到，所以关于此类类型，特殊处理一下
                if (type == typeof(ushort)) {
                    list1.Add(Convert.ToUInt16(addItem));//此处直接强转会报错，需要用Convert方法来处理，具体原理可以百度错误Specified cast is not valid
                } else if (type == typeof(short)) {
                    list2.Add(Convert.ToInt16(addItem));//此处直接强转会报错，需要用Convert方法来处理，具体原理可以百度错误Specified cast is not valid
                } else {
                    if (type == typeof(int)) {
                        addItem = Convert.ToInt32(addItem);//object类型需要强转才能作为int，ushort，short等数据插入对应list
                    }
                    list.GetType().InvokeMember("Add", BindingFlags.InvokeMethod, null, list, new object[] { addItem });
                }
            }
            catch(Exception e)
            {
                Debug.Log($"{varClass.ListSheetName} 里{varClass.Name}列表添加失败！具体数值是：{addItem},错误原因：{e.Message}");
            }
        }
        //用反射添加string之外的类型，例如ushort等会报错，提示添加失败，但直接add不会，原因未找到，所以关于此类类型，特殊处理一下
        if (type == typeof(ushort)) {
            objClass.GetType().GetProperty(varClass.Name).SetValue(objClass, list1);
        } else if (type == typeof(short)) {
            objClass.GetType().GetProperty(varClass.Name).SetValue(objClass, list2);
        } else {
            objClass.GetType().GetProperty(varClass.Name).SetValue(objClass, list);
        }
    }

    /// <summary>
    /// 根据列名获取变量名
    /// </summary>
    /// <param name="varlist"></param>
    /// <param name="col"></param>
    /// <returns></returns>
    private static string GetNameFormCol(List<VarClass> varlist, string col)
    {
        foreach (VarClass varClass in varlist)
        {
            if (varClass.Col == col)
                return varClass.Name;
        }
        return null;
    }
    /// <summary>
    /// 递归读取类里面的数据
    /// 需要注意，类中读取的数据，需要将每一个字段都赋值，否则会报空
    /// </summary>
    /// <param name="data"></param>
    /// <param name="sheetClass"></param>
    /// <param name="allSheetClassDic"></param>
    /// <param name="sheetDataDic"></param>
    
    private static void ReadData(object data, SheetClass sheetClass, Dictionary<string, SheetClass> allSheetClassDic, Dictionary<string, SheetData> sheetDataDic, string mainKey)
    {
        List<VarClass> varList = sheetClass.VarList;
        VarClass varClass = sheetClass.ParentVar;
        object dataList = GetMemberValue(data, varClass.Name);

        int listCount = System.Convert.ToInt32(dataList.GetType().InvokeMember("get_Count", BindingFlags.Default | BindingFlags.InvokeMethod, null, dataList, new object[] { }));

        SheetData sheetData = new SheetData();

        if (!string.IsNullOrEmpty(varClass.Foregin))
        {
            sheetData.AllName.Add(varClass.Foregin);
            sheetData.AllType.Add(varClass.Type);
        }

        for (int i = 0; i < varList.Count; i++)
        {
            if (!string.IsNullOrEmpty(varList[i].Col))
            {
                sheetData.AllName.Add(varList[i].Col);
                sheetData.AllType.Add(varList[i].Type);
            }
        }

        string tempKey = mainKey;
        for (int i = 0; i < listCount; i++)
        {
            object item = dataList.GetType().InvokeMember("get_Item", BindingFlags.Default | BindingFlags.InvokeMethod, null, dataList, new object[] { i });

            RowData rowData = new RowData();
            if (!string.IsNullOrEmpty(varClass.Foregin) && !string.IsNullOrEmpty(tempKey))
            {
                rowData.RowDataDic.Add(varClass.Foregin, tempKey);
            }

            if (!string.IsNullOrEmpty(sheetClass.MainKey))
            {
                mainKey = GetMemberValue(item, sheetClass.MainKey).ToString();
            }

            for (int j = 0; j < varList.Count; j++)
            {
                if (varList[j].Type == "list" && string.IsNullOrEmpty(varList[j].SplitStr))
                {
                    SheetClass tempSheetClass = allSheetClassDic[varList[j].ListSheetName];
                    ReadData(item, tempSheetClass, allSheetClassDic, sheetDataDic, mainKey);
                }
                else if (varList[j].Type == "list")
                {
                    SheetClass tempSheetClass = allSheetClassDic[varList[j].ListSheetName];
                    string value = GetSplitStrList(item, varList[j], tempSheetClass);
                    rowData.RowDataDic.Add(varList[j].Col, value);
                }
                else if (varList[j].Type == "listStr" || varList[j].Type == "listFloat" || varList[j].Type == "listInt" || varList[j].Type == "listBool" || varList[j].Type == "listUshort" || varList[j].Type == "listShort")
                {
                    string value = GetSpliteBaseList(item, varList[j]);
                    rowData.RowDataDic.Add(varList[j].Col, value);
                }
                else
                {
                    object value = GetMemberValue(item, varList[j].Name);
                    if (varList != null)
                    {
                        rowData.RowDataDic.Add(varList[j].Col, value.ToString());
                    }
                    else
                    {
                        Debug.LogError(varList[j].Name + "反射出来为空！");
                    }
                }
            }

            string key = varClass.ListSheetName;
            if (sheetDataDic.ContainsKey(key))
            {
                sheetDataDic[key].AllData.Add(rowData);
            }
            else
            {
                sheetData.AllData.Add(rowData);
                sheetDataDic.Add(key, sheetData);
            }
        }
    }

    /// <summary>
    /// 获取本身是一个类的列表，但是数据比较少；（没办法确定父级结构的）
    /// </summary>
    /// <returns></returns>
    private static string GetSplitStrList(object data, VarClass varClass, SheetClass sheetClass)
    {
        string split = varClass.SplitStr;
        string classSplit = sheetClass.SplitStr;
        string str = "";
        if (string.IsNullOrEmpty(split) || string.IsNullOrEmpty(classSplit))
        {
            Debug.LogError("类的列类分隔符或变量分隔符为空！！！");
            return str;
        }
        object dataList = GetMemberValue(data, varClass.Name);
        int listCount = System.Convert.ToInt32(dataList.GetType().InvokeMember("get_Count", BindingFlags.Default | BindingFlags.InvokeMethod, null, dataList, new object[] { }));
        for (int i = 0; i < listCount; i++)
        {
            object item = dataList.GetType().InvokeMember("get_Item", BindingFlags.Default | BindingFlags.InvokeMethod, null, dataList, new object[] { i });
            for (int j = 0; j < sheetClass.VarList.Count; j++)
            {
                object value = GetMemberValue(item, sheetClass.VarList[j].Name);
                str += value.ToString();
                if (j != sheetClass.VarList.Count - 1)
                {
                    str += classSplit.Replace("\\n", "\n").Replace("\\r", "\r");
                }
            }

            if (i != listCount - 1)
            {
                str += split.Replace("\\n", "\n").Replace("\\r", "\r");
            }
        }
        return str;
    }

    /// <summary>
    /// 获取基础List里面的所有值
    /// </summary>
    /// <returns></returns>
    private static string GetSpliteBaseList(object data, VarClass varClass)
    {
        string str = "";
        if (string.IsNullOrEmpty(varClass.SplitStr))
        {
            Debug.LogError("基础List的分隔符为空！");
            return str;
        }
        object dataList = GetMemberValue(data, varClass.Name);
        int listCount = System.Convert.ToInt32(dataList.GetType().InvokeMember("get_Count", BindingFlags.Default | BindingFlags.InvokeMethod, null, dataList, new object[] { }));

        for (int i = 0; i < listCount; i++)
        {
            object item = dataList.GetType().InvokeMember("get_Item", BindingFlags.Default | BindingFlags.InvokeMethod, null, dataList, new object[] { i });
            str += item.ToString();
            if (i != listCount - 1)
            {
                str += varClass.SplitStr.Replace("\\n", "\n").Replace("\\r", "\r");
            }
        }
        return str;
    }

   


    /// <summary>
    /// 判断文件是否被占用
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    private static bool FileIsUsed(string path)
    {
        bool result = false;

        if (!File.Exists(path))
        {
            result = false;
        }
        else
        {
            FileStream fileStream = null;
            try
            {
                fileStream = File.Open(path, FileMode.Open, FileAccess.ReadWrite, FileShare.None);

                result = false;
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                result = true;
            }
            finally
            {
                if (fileStream != null)
                {
                    fileStream.Close();
                }
            }
        }

        return result;
    }

    /// <summary>
    /// 反射new一個list
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    private static object CreateList(Type type)
    {
        Type listType = typeof(List<>);
        Type specType = listType.MakeGenericType(new System.Type[] { type });//确定list<>里面T的类型
        return Activator.CreateInstance(specType, new object[] { });//new出来这个list
    }

    /// <summary>
    /// 反射变量赋值
    /// </summary>
    /// <param name="info"></param>
    /// <param name="var"></param>
    /// <param name="value"></param>
    /// <param name="type"></param>
    private static void SetValue(PropertyInfo info, object var, string value, string type)
    {
        object val = (object)value;
        if (type == "int")
        {
            val = System.Convert.ToInt32(val);
        }
        else if (type == "bool")
        {
            val = System.Convert.ToBoolean(val);
        }
        else if (type == "float")
        {
            val = System.Convert.ToSingle(val);
        }
        else if (type == "enum")
        {
            val = TypeDescriptor.GetConverter(info.PropertyType).ConvertFromInvariantString(val.ToString());
        } else if (type == "short") {
            val = System.Convert.ToInt16(val);
        } else if (type == "ushort") {
            val = System.Convert.ToUInt16(val);
        }
        info.SetValue(var, val);
    }

    /// <summary>
    /// 反射类里面的变量的具体数值
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="memeberName"></param>
    /// <param name="bindingFlags"></param>
    /// <returns></returns>
    private static object GetMemberValue(object obj, string memeberName, BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance)
    {
        Type type = obj.GetType();
        MemberInfo[] members = type.GetMember(memeberName, bindingFlags);
        //while (members == null || members.Length == 0)
        //{
        //    type = type.BaseType;//获取基类型，可以理解为最顶级的父类
        //    if (type == null)
        //        return;

        //    members = type.GetMember("Name",  BindingFlags.Public | BindingFlags.Default);
        //}
        switch (members[0].MemberType)
        {
            case MemberTypes.Field:
                return type.GetField(memeberName, bindingFlags).GetValue(obj);
            case MemberTypes.Property:
                return type.GetProperty(memeberName, bindingFlags).GetValue(obj);
            default:
                return null;
        }

    }

    /// <summary>
    /// 反射创建类的实例
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    private static object CreateClass(string name)
    {
        object obj = null;
        Type type = null;
        //AppDomain.CurrentDomain.GetAssemblies() 获取到主程序集，在主程序集中寻找这个类，返回Assembly数组
        //注意，数据表类不能写在其他命名空间内，否则找不到AppDomain.CurrentDomain.GetAssemblies()只会获取运行时的程序集
        //foreach (var asm in AppDomain.CurrentDomain.GetAssemblies()) {
        //    Type tempType = asm.GetType(name);
        //    if (tempType != null) {
        //        type = tempType;
        //        break;
        //    }
        //}
        //换这种直接通过程序集和命名空间的形式获取类的type
        type = Type.GetType($"{m_namespace}.{name},{m_assemblyName}");

        if (type != null)
        {
            obj = Activator.CreateInstance(type);
        }
        return obj;
    }


    /// <summary>
    /// xml。binary，类，excel互转测试及示例代码
    /// </summary>
    #region  以下为功能测试代码
    [MenuItem("Tools/测试/测试读取xml")]
    public static void TestReadXml() {
        string xmlPath = Application.dataPath + "/../Data/Reg/MonsterData.xml";
        XmlReader reader = null;
        try {
            XmlDocument xml = new XmlDocument();
            reader = XmlReader.Create(xmlPath);
            xml.Load(reader);
            XmlNode xn = xml.SelectSingleNode("data");
            XmlElement xe = (XmlElement)xn;
            string className = xe.GetAttribute("name");
            string xmlName = xe.GetAttribute("to");
            string excelName = xe.GetAttribute("from");
            reader.Close();
            Debug.LogError(className + "  " + xmlName + "  " + excelName);
            foreach (XmlNode node in xe.ChildNodes) {
                XmlElement tempXe = (XmlElement)node;
                string name = tempXe.GetAttribute("name");
                string type = tempXe.GetAttribute("type");
                Debug.LogError(name + "  " + type);
                XmlNode listNode = tempXe.FirstChild;
                XmlElement listElement = (XmlElement)listNode;
                string listName = listElement.GetAttribute("name");
                string sheetName = listElement.GetAttribute("sheetname");
                string mainKey = listElement.GetAttribute("mainKey");
                Debug.LogError("list: " + listName + "  " + sheetName + "  " + mainKey);
                foreach (XmlNode nd in listElement.ChildNodes) {
                    XmlElement txe = (XmlElement)nd;
                    Debug.LogError($"{txe.GetAttribute("name")} --{txe.GetAttribute("col")} --{txe.GetAttribute("type")}");
                }
            }
        } catch (Exception e) {
            if (reader != null) {
                reader.Close();
            }
            Debug.LogError(e);
        }
    }

    [MenuItem("Tools/测试/测试写入Excel")]
    public static void TestWriteExcel() {
        string xlsxPath = Application.dataPath + "/../Data/Excel/NPCData.xlsx";
        FileInfo xlsxFile = new FileInfo(xlsxPath);
        if (xlsxFile.Exists) {
            xlsxFile.Delete();
            xlsxFile = new FileInfo(xlsxPath);
        }
        //继承了IDisposable接口的就可以使用using去释放
        using (ExcelPackage package = new ExcelPackage(xlsxFile)) {
            ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("怪物配置");
            //worksheet.DefaultColWidth = 10;//sheet页面默认行宽度
            //worksheet.DefaultRowHeight = 30;//sheet页面默认列高度
            //worksheet.Cells.Style.WrapText = true;//设置所有单元格的自动换行，这里注意，自适应宽度必须要在自动换行之前才能起作用。
            //worksheet.InsertColumn();//插入行，从某一行开始插入多少行
            //worksheet.InsertRow();//插入列，从某一列开始插入多少列
            //worksheet.DeleteColumn();//删除行，从某一行开始删除多少行
            //worksheet.DeleteRow();//删除列，从某一列开始删除多少列
            //worksheet.Column(1).Width = 10;//设定第几行宽度
            //worksheet.Row(1).Height = 30;//设定第几列高度
            //worksheet.Column(1).Hidden = true;//设定第几行隐藏
            //worksheet.Row(1).Hidden = true;//设定第几列隐藏
            //worksheet.Column(1).Style.Locked = true;//设定第几行锁定
            //worksheet.Row(1).Style.Locked = true;//设定第几列锁定
            //worksheet.Cells.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;//设定所有单元格对齐方式

            worksheet.Cells.AutoFitColumns();
            ExcelRange range = worksheet.Cells[1, 1];
            range.Value = " 测试sadddddddddddddd\ndddddddddddddddddddasda";
            range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.None;
            //range.Style.Fill.BackgroundColor.SetColor();//设置单元格内背景颜色
            //range.Style.Font.Color.SetColor();//设置单元格内字体颜色
            range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;//对齐方式 
            range.AutoFitColumns();//单元格自适应宽度
            range.Style.WrapText = true;

            ExcelRange range1 = worksheet.Cells[1, 2];

            package.Save();
        }
    }

    [MenuItem("Tools/测试/测试已有类进行反射")]
    public static void TestReflection1() {
        TestInfo testInfo = new TestInfo() {
            Id = 2,
            Name = "测试反射",
            IsA = false,
            AllStrList = new List<string>(),
            AllTestInfoList = new List<TestInfoTwo>(),
        };

        testInfo.AllStrList.Add("测试1111");
        testInfo.AllStrList.Add("测试2222");
        testInfo.AllStrList.Add("测试3333");

        for (int i = 0; i < 3; i++) {
            TestInfoTwo test = new TestInfoTwo();
            test.Id = i + 1;
            test.Name = i + "name";
            testInfo.AllTestInfoList.Add(test);
        }

        GetMemberValue(testInfo, "Name");
        //object list = GetMemberValue(testInfo, "AllStrList");
        //int listCount = System.Convert.ToInt32(list.GetType().InvokeMember("get_Count", BindingFlags.Default | BindingFlags.InvokeMethod, null, list, new object[] { }));

        //for(int i = 0; i < listCount; i++)
        //{
        //    object item = list.GetType().InvokeMember("get_Item", BindingFlags.Default | BindingFlags.InvokeMethod, null, list, new object[] { i });
        //    Debug.LogError(item);
        //}
        object list = GetMemberValue(testInfo, "AllTestInfoList");
        int listCount = System.Convert.ToInt32(list.GetType().InvokeMember("get_Count", BindingFlags.Default | BindingFlags.InvokeMethod, null, list, new object[] { }));
        for (int i = 0; i < listCount; i++) {
            object item = list.GetType().InvokeMember("get_Item", BindingFlags.Default | BindingFlags.InvokeMethod, null, list, new object[] { i });

            object id = GetMemberValue(item, "Id");
            object name = GetMemberValue(item, "Name");
            Debug.LogError(id + " " + name);
        }
    }

    [MenuItem("Tools/测试/测试已有数据进行反射")]
    public static void TestReflection2() {
        object obj = CreateClass("TestInfo");
        PropertyInfo info = obj.GetType().GetProperty("Id");
        SetValue(info, obj, "21", "int");
        //info.SetValue(obj, System.Convert.ToInt32("20"));
        PropertyInfo nameInfo = obj.GetType().GetProperty("Name");
        SetValue(nameInfo, obj, "aqweddad", "string");
        //nameInfo.SetValue(obj, "huhiuhiuhi");
        PropertyInfo isInfo = obj.GetType().GetProperty("IsA");
        SetValue(isInfo, obj, "true", "bool");
        //isInfo.SetValue(obj, System.Convert.ToBoolean("false"));
        PropertyInfo heighInfo = obj.GetType().GetProperty("Heigh");
        SetValue(heighInfo, obj, "51.4", "float");
        //heighInfo.SetValue(obj, System.Convert.ToSingle("22.5"));
        PropertyInfo enumInfo = obj.GetType().GetProperty("TestType");
        SetValue(enumInfo, obj, "VAR1", "enum");
        //object infoValue = TypeDescriptor.GetConverter(enumInfo.PropertyType).ConvertFromInvariantString("VAR1");
        //enumInfo.SetValue(obj, infoValue);
        //反射创建list及数据  list<string>
        Type type = typeof(string);
        object list = CreateList(type);
        for (int i = 0; i < 3; i++) {
            object addItem = "测试填数据" + i;
            list.GetType().InvokeMember("Add", BindingFlags.Default | BindingFlags.InvokeMethod, null, list, new object[] { addItem });//调用list的add方法添加数据
        }

        obj.GetType().GetProperty("AllStrList").SetValue(obj, list);
        //反射创建list及数据  list<类>
        object twoList = CreateList(typeof(TestInfoTwo));
        for (int i = 0; i < 3; i++) {
            object addItem = CreateClass("TestInfoTwo");
            PropertyInfo itemIdInfo = addItem.GetType().GetProperty("Id");
            SetValue(itemIdInfo, addItem, "152" + i, "int");
            PropertyInfo itemNameInfo = addItem.GetType().GetProperty("Name");
            SetValue(itemNameInfo, addItem, "测试类" + i, "string");
            twoList.GetType().InvokeMember("Add", BindingFlags.Default | BindingFlags.InvokeMethod, null, twoList, new object[] { addItem });
        }
        obj.GetType().GetProperty("AllTestInfoList").SetValue(obj, twoList);

        TestInfo testInfo = (obj as TestInfo);
        //foreach (string str in testInfo.AllStrList)
        //{
        //    Debug.LogError(str);
        //}
        foreach (TestInfoTwo test in testInfo.AllTestInfoList) {
            Debug.LogError(test.Id + " " + test.Name);
        }
    }
    #endregion
}
/// <summary>
/// list的表类，xml转excel时，用来读取表信息的
/// 例如<variable name= "AllNPC" type = "list">
///   < list name = "NPCBase" sheetname = "NPC配置表" mainKey = "id" split = "|">
///              < variable name = "id" col = "ID" type = "int" />
/// </summary>
public class SheetClass
{
    //所属父级Var变量，这个就是list之上的variable节点中的class那么
    //就是示例中<variable name= "AllNPC" type = "list">中的AllNPC
    /// </summary>
    public VarClass ParentVar { get; set; }
    //深度
    public int Depth { get; set; }
    //类名 <list name = "NPCBase" sheetname = "NPC配置表" mainKey = "id">中的NPCBase
    public string Name { get; set; }
    //类对应的sheet名 <list name = "NPCBase" sheetname = "NPC配置表" mainKey = "id">中的NPC配置表
    public string SheetName { get; set; }
    //主键 <list name = "NPCBase" sheetname = "NPC配置表" mainKey = "id">中的mainKey
    public string MainKey { get; set; }
    //分隔符 <list name = "NPCBase" sheetname = "NPC配置表" mainKey = "id" split = "|">中的split
    public string SplitStr { get; set; }
    //所包含的变量 list中包含的所有variable
    public List<VarClass> VarList = new List<VarClass>();
}
/// <summary>
/// 变量类，xml转excel时，用来读取variable的
/// 例如<variable name= "AllNPC" type = "list">
///   < list name = "NPCBase" sheetname = "NPC配置表" mainKey = "id" >
///              < variable name = "id" col = "ID" type = "int" />
/// 这里的varclass就是< variable name = "id" col = "ID" type = "int" />的信息
/// </summary>
public class VarClass
{
    //原类里面变量的名称
    public string Name { get; set; }
    //变量类型
    public string Type { get; set; }
    //变量对应的Excel里的列
    public string Col { get; set; }
    //变量的默认值
    public string DefaultValue { get; set; }
    //变量是list的话，外联部分列
    public string Foregin { get; set; }
    //分隔符
    public string SplitStr { get; set; }
    //如果自己是List，对应的list类名
    public string ListName { get; set; }
    //如果自己是list,对应的sheet名
    public string ListSheetName {get;set;}
}
/// <summary>
/// 单个sheet表的所有数据
/// </summary>
public class SheetData
{
    /// <summary>
    /// 第一行，字段行，相当于每一列的名称
    /// </summary>
    public List<string> AllName = new List<string>();
    /// <summary>
    /// 每一列数据的数据类型
    /// </summary>
    public List<string> AllType = new List<string>();
    /// <summary>
    /// 每一列的数据
    /// </summary>
    public List<RowData> AllData = new List<RowData>();
}
/// <summary>
/// 单个sheet表中每一列的数据
/// </summary>
public class RowData
{
    public string ParnetVlue = "";
    public Dictionary<string, string> RowDataDic = new Dictionary<string, string>();
}



#region 以下是测试用的
//以下是测试用的

public enum TestEnum
{
    None = 0,
    VAR1 = 1,
    TEST2 = 2,
}

public class TestInfo
{
    public int Id { get; set; }
    public string Name { get; set; }
    public bool IsA { get; set; }

    public float Heigh { get; set; }

    public TestEnum TestType { get; set; }

    public List<string> AllStrList { get; set; }
    public List<TestInfoTwo> AllTestInfoList { get; set; }
}

public class TestInfoTwo
{
    public int Id { get; set; }
    public string Name { get; set; }
}
#endregion
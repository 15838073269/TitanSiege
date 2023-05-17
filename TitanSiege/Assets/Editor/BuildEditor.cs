/****************************************************
    文件：BuildEditor.cs
	作者：昔莘
    邮箱: 304183153@qq.com
    日期：2021/8/9 16:22:33
	功能：编辑器下打包处理,及版本文件处理
*****************************************************/
//using GF;   unityeditor中使用我们自己的Debuger出错了，并且没报错，不知道原因，但自带的debug不会
using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using GF.Unity.AB;
public class BuildEditor
{

    #region 标准AB包打包
    //打包路径
    private static string m_BuildPath = $"{Application.dataPath}/../AssetBundle/{EditorUserBuildSettings.activeBuildTarget.ToString()}/";
    //配置文件路径
    private static string ABCONFIGPATH = "Assets/Editor/ABConfig.asset";
    //这个字典存储所有文件夹类型的ab包，key是ab包名称，value是ab包对应文件夹路径   
    private static Dictionary<string, string> m_AllFileDir = new Dictionary<string, string>();
    //存储所有已经打包的资源文件的路径，用来过滤重复的资源,例如单个文件的prefab或者资源已经存在于某个文件夹下，就不再打包将它包含进去
    private static List<string> m_AllResDir = new List<string>();
    //单个prefab包,key为prefab的名称，value为prefab所有需要打包的依赖项的路径
    private static Dictionary<string, List<string>> m_AllPrefabDir = new Dictionary<string, List<string>>();
    //单个scene包,key为scene的名称，value为scene所有需要打包的依赖项的路径
    private static Dictionary<string, List<string>> m_AllSceneDir = new Dictionary<string, List<string>>();
    //ab包里有很多资源我们其实不需要关注，例如贴图，需要关注的跟多是音频，动画，prefab等，所以设置一个list存储所有需要关注的资源
    private static List<string> m_ImportantRes = new List<string>();
    //二进制文件路径
    private static string binarypath = "Assets/Art/data/AssetBundleConfig.bytes"; 
     [MenuItem("Build/标准AB包")]
    public static void Build() {
        m_AllFileDir.Clear();//及时清理
        m_AllResDir.Clear();
        m_AllPrefabDir.Clear();
        m_ImportantRes.Clear();
        m_AllSceneDir.Clear();
        //打ab包
        //获取ab包配置文件
        ABConfig abconfig = AssetDatabase.LoadAssetAtPath<ABConfig>(ABCONFIGPATH);
        //处理文件夹形式的资源
        foreach (ABConfig.FileDirABName fileab in abconfig.m_AllFileDirAB) {
            if (m_AllFileDir.ContainsKey(fileab.ABName)) {
                Debug.LogError("ab包" + fileab.ABName + "在配置文件中已存在，请检查重复！");
            } else {
                string newpath = fileab.Path.Replace("\\","/");
                m_AllFileDir.Add(fileab.ABName, newpath);
                m_AllResDir.Add(newpath);
                m_ImportantRes.Add(newpath);//文件夹类的路径是绝对需要关注的
            }
        }
        //通过untiy编辑器方法查找目录下的所有prefab
        string[] allguid = AssetDatabase.FindAssets("t:Prefab",abconfig.m_AllPrefabPath.ToArray());//这个方法返回的并不是完整的路径，而是GUID。
        for (int i = 0; i < allguid.Length; i++) {
            //将GUID转换成路径
            string path = AssetDatabase.GUIDToAssetPath(allguid[i]);
            //Debug.Log(allguid[i] + "+++" + path);
            //使用unity编辑器自带的进度条提示加载进度
            EditorUtility.DisplayProgressBar("查找Prefab","Prefab:"+path,i*1.0f/allguid.Length);
            m_ImportantRes.Add(path);//把prefab加到需要关注list中
            if (!ContainsPath(path)) {
                GameObject obj = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                //获取改资源（一般是prefab）下所有依赖资源
                string[] alldepend = AssetDatabase.GetDependencies(path);
                List<string> tempdependlist = new List<string>();
                for (int j = 0; j < alldepend.Length; j++) {
                    Debug.Log($"{path}++{alldepend[j]}");
                    //判断这个资源是否已经被打包过了
                    if (!ContainsPath(alldepend[j])&&!alldepend[j].EndsWith(".cs")) {
                        m_AllResDir.Add(alldepend[j]);
                        tempdependlist.Add(alldepend[j]);
                    }
                }
                if (m_AllPrefabDir.ContainsKey(obj.name)) {
                    Debug.LogError("预制体名称重复，请检查，重复名为：" + obj.name);
                } else {
                    m_AllPrefabDir.Add(obj.name,tempdependlist);
                }
            }
        }
        ////通过untiy编辑器方法查找目录下的所有Scene
        //string[] allscguid = AssetDatabase.FindAssets("t:Scene", abconfig.m_AllPrefabPath.ToArray());//这个方法返回的并不是完整的路径，而是GUID。
        //for (int i = 0; i < allscguid.Length; i++) {
        //    //将GUID转换成路径
        //    string scpath = AssetDatabase.GUIDToAssetPath(allscguid[i]);
        //    //Debug.Log(allguid[i] + "+++" + path);
        //    //使用unity编辑器自带的进度条提示加载进度
        //    EditorUtility.DisplayProgressBar("查找Scene", "Scene:" + scpath, i * 1.0f / allscguid.Length);
        //    m_ImportantRes.Add(scpath);//把scene加到需要关注list中
        //    if (!ContainsPath(scpath)) {
        //        SceneAsset scene = AssetDatabase.LoadAssetAtPath<SceneAsset>(scpath);
        //        //获取改资源（一般是Scene）下所有依赖资源
        //        string[] alldepend = AssetDatabase.GetDependencies(scpath);
        //        List<string> tempdependlist = new List<string>();
        //        for (int j = 0; j < alldepend.Length; j++) {
        //            //Debug.Log(alldepend[j]);
        //            //判断这个资源是否已经被打包过了
        //            if (!ContainsPath(alldepend[j]) && !alldepend[j].EndsWith(".cs")) {
        //                m_AllResDir.Add(alldepend[j]);
        //                tempdependlist.Add(alldepend[j]);
        //            }
        //        }
        //        if (m_AllSceneDir.ContainsKey(scene.name)) {
        //            Debug.LogError("预制体名称重复，请检查，重复名为：" + scene.name);
        //        } else {
        //            m_AllSceneDir.Add(scene.name, tempdependlist);
        //        }
        //    }
        //}
        //循环m_AllFileDir和m_AllPrefabDir，设置资源的ab包名称
        foreach (string name in m_AllFileDir.Keys) {
            SetABName(name, m_AllFileDir[name]);
        }
        foreach (string name in m_AllPrefabDir.Keys) {
            SetABName(name, m_AllPrefabDir[name]);
        }
        //foreach (string name in m_AllSceneDir.Keys) {
        //    SetABName(name, m_AllSceneDir[name]);
        //}
        BuildAssetBundle();
        //最终打包完成后，需要清理掉已经生成的ab包名称，因为修改资源的abname，meta文件会发生改变，如果使用gitsvn管理项目，会出现很多无关的文件改动，影响使用，所以在系统还没有来得及重新加载meta文件时，清理掉改动，就能减少文件改动。
        string[] oldABNames = AssetDatabase.GetAllAssetBundleNames();//通过编辑器方法获取所有AB包的名称
        for (int i = 0; i < oldABNames.Length; i++) {
            AssetDatabase.RemoveAssetBundleName(oldABNames[i], true);//强制清除所有包名，第二参数true是强制，false是不强制，只清除无用的包名。
            EditorUtility.DisplayProgressBar("清除包名", "包名：" + oldABNames[i], i * 1.0f / oldABNames.Length);//加一个进度条
        }
        AssetDatabase.Refresh();//最后刷新一下，不刷新的话，需要手动refresh一下，才会显示打好的包。
        EditorUtility.ClearProgressBar();//用完之后要清除，否则加载框会停留在界面上。多个进度条加载可以只清除这一次
    }
    /// <summary>
    /// 判断m_AllResDir中是否存在此路径，存在就证明打包过这个资源
    /// </summary>
    /// <param name="path">资源路径</param>
    /// <returns></returns>
    static bool ContainsPath(string path) {
        for (int i = 0; i < m_AllResDir.Count; i++) {
            //path.Replace(m_AllResDir[i],"")[0]=='/'   这一句意思是，如果包含路径，并且除去包含内容后的第一个字符为/，才能认为在同一文件夹下，主要为了防止出现文件夹名称相似而导致的包含判断错误
            //例如  abcd/cccaa/  和 abcd/cccaaav/ttt.prefab
            if ((m_AllResDir[i] == path)||(path.Contains(m_AllResDir[i])&&path.Replace(m_AllResDir[i],"")[0]=='/')) {
                return true;
            }
        }
        return false;
    }
    /// <summary>
    /// 设置资源的ab包名称
    /// </summary>
    /// <param name="abname">包名</param>
    /// <param name="path">路径</param>
    static void SetABName(string abname,string path) {
        AssetImporter assetimport = AssetImporter.GetAtPath(path);//通过路径查找资源
        if (assetimport == null) {
            Debug.LogError("路径不存在："+path);
        } else {
            assetimport.assetBundleName = abname;
        }
    }
    static void SetABName(string abname, List<string> listpath) {
        for (int i = 0; i < listpath.Count; i++) {
            SetABName(abname,listpath[i]);
        }
    }
    /// <summary>
    /// 生成自用的依赖关系配置表，并打包
    /// </summary>
    static void BuildAssetBundle() {
        string[] allbundlenames = AssetDatabase.GetAllAssetBundleNames();
        //key是路径，value是包名
        Dictionary<string, string> ResPathDic = new Dictionary<string, string>();
        for (int i = 0; i < allbundlenames.Length; i++) {
            //根据包名获取到包下所有资源的路径
            string[] allbundlepath = AssetDatabase.GetAssetPathsFromAssetBundle(allbundlenames[i]);
            for (int j = 0; j < allbundlepath.Length; j++) {
                if (allbundlepath[j].EndsWith(".cs")) {//排除cs脚本和不需要关注的资源
                    continue;
                }
                //Debug.Log("ab包"+ allbundlenames[i]+ "下包含的资源路径有："+ allbundlepath[j]);
                ResPathDic.Add(allbundlepath[j], allbundlenames[i]);
            }
        }
        if (!Directory.Exists(m_BuildPath)) {
            Directory.CreateDirectory(m_BuildPath);
        }
        //删除没用的AB包
        DeleteNoUseAB();
        //生成自己的配置表
        WriteDate(ResPathDic);
        
        //打包
        //UnityEditor.EditorUserBuildSettings.activeBuildTarget  是获取当前用户的设置的目标平台
        AssetBundleManifest manifest = BuildPipeline.BuildAssetBundles(m_BuildPath, BuildAssetBundleOptions.ChunkBasedCompression, UnityEditor.EditorUserBuildSettings.activeBuildTarget);
        if (manifest == null) {
            Debug.LogError("打包失败，请检查打包代码！");
        }
    }
    /// <summary>
    /// 写入配置表文件
    /// </summary>
    /// <param name="ResPathDic">组合好的ab包路径名称字典</param>
    static void WriteDate(Dictionary<string, string> ResPathDic) {
        
        AssetBundleConfig abconfig = new AssetBundleConfig();
        abconfig.ABList = new List<ABBase>();
        foreach (string respath in ResPathDic.Keys) {
            if (!IsImportant(respath)) {
                continue;
            }
            ABBase tmpab = new ABBase();
            tmpab.ABname = ResPathDic[respath];
            tmpab.Path = respath;
            tmpab.Crc = Crc32.GetCRC32(respath);//将路径字符串生成唯一的Crc
            tmpab.AssetName = respath.Remove(0,respath.LastIndexOf("/")+1);
            tmpab.ABDepends = new List<string>();
            string[] allresdepends = AssetDatabase.GetDependencies(respath);//获取这个资源所有依赖资源
            for (int i = 0; i < allresdepends.Length; i++) {
                //Debug.Log("ab包"+ ResPathDic[respath] + "的"+ tmpab.AssetName + "资源依赖的有资源：" + allresdepends[i]);
                if (allresdepends[i] == respath|| allresdepends[i].EndsWith(".cs")) {//排除自己和脚本
                    continue;
                }
                string abname = "";
                if (ResPathDic.TryGetValue(allresdepends[i], out abname)) {//查询依赖的资源路径有没有在资源存储字典中
                    if (abname == ResPathDic[respath]) {//如果有，且和资源包名一样，那就说明在一个包里，排除，不是依赖
                        continue;
                    }
                    if (!tmpab.ABDepends.Contains(abname)) {//如果有，且包名不同，那证明这个依赖资源在其他包里，这个其他包就是依赖项
                        tmpab.ABDepends.Add(abname);
                    }
                }
            }
            abconfig.ABList.Add(tmpab);
        }
        //写入xml,生成xml只是为了纠错，不会使用，直接生成二进制，无法看到生成的内容，不清楚到底有没有问题
        //xml路径
        string xmlptah = Application.dataPath + "/AssetBundleConfig.xml";
        if (File.Exists(xmlptah)) {
            File.Delete(xmlptah);
        }
        FileStream fs = new FileStream(xmlptah,FileMode.Create,FileAccess.ReadWrite,FileShare.ReadWrite);
        StreamWriter sw = new StreamWriter(fs,System.Text.Encoding.UTF8);
        XmlSerializer xml = new XmlSerializer(abconfig.GetType());
        xml.Serialize(sw,abconfig);
        sw.Close();
        fs.Close();
        //写入二进制
        //二进制路径
        
        for (int i = 0; i < abconfig.ABList.Count; i++) {
            abconfig.ABList[i].Path = "";//真正读取数据时，不会使用path，只会用crc，path只在xml中方便我们查看而已，所以path可以删除，删除大量无用string，可以有效减少bytes文件的大小
        }
        FileStream binaryfs = new FileStream(binarypath,FileMode.Create,FileAccess.ReadWrite);
        //打开后先清空
        binaryfs.Seek(0,SeekOrigin.Begin);
        binaryfs.SetLength(0);
        BinaryFormatter bf = new BinaryFormatter();
        bf.Serialize(binaryfs,abconfig);
        binaryfs.Close();
        AssetDatabase.Refresh();//这里必须刷新，否则编辑器会认为没有这个文件
        SetABName("data", binarypath);
    }
    /// <summary>
    /// 主要是删除已经删除资源或者改名的ab包,不直接清空，是因为unity打ab包会自动增量打包，清空如果资源很多，每次打包时间很长。
    /// </summary>
    static void DeleteNoUseAB() {
        string[] allnewabnames = AssetDatabase.GetAllAssetBundleNames();
        DirectoryInfo dicinfo = new DirectoryInfo(m_BuildPath);
        FileInfo[] files = dicinfo.GetFiles("*",SearchOption.AllDirectories);
        for (int i = 0; i < files.Length; i++) {
            if (IsUse(files[i].Name, allnewabnames)||files[i].Name.EndsWith(".meta")|| files[i].Name.EndsWith(".manifest") || files[i].Name.EndsWith(".bytes")) {
            //if (IsUse(files[i].Name, allnewabnames)) {
                continue;
            } else {
                Debug.Log("AB包"+ files[i].Name+"不再使用，删除");
                if (File.Exists(files[i].FullName)) {
                    File.Delete(files[i].FullName);
                }
            }
        }
    }
    /// <summary>
    /// 判断是ab包是否还在使用
    /// </summary>
    /// <param name="abname">现有的ab包名称</param>
    /// <param name="abarray">新的的ab包数组</param>
    static bool IsUse(string abname,string[] abarray) {
        for (int i = 0; i < abarray.Length; i++) {
            if (abname == abarray[i]) {
                return true;
            }
        }
        return false;
    }
    /// <summary>
    /// 判断该资源是不是需要关注的，例prefab需要关注，贴图就不需要
    /// </summary>
    /// <param name="path">资源路径</param>
    /// <returns></returns>
    static bool IsImportant(string path) {
        for (int i = 0; i < m_ImportantRes.Count; i++) {
            if (path.Contains(m_ImportantRes[i])) {
                return true;
            }
        }
        return false;
    }
    #endregion



    //[MenuItem("Build/测试打包")]
    //public static void TestBuild() {
    //    //打ab包
    //    //UnityEditor.EditorUserBuildSettings.activeBuildTarget  是获取当前用户的设置的目标平台
    //    BuildPipeline.BuildAssetBundles(Application.streamingAssetsPath, BuildAssetBundleOptions.ChunkBasedCompression, UnityEditor.EditorUserBuildSettings.activeBuildTarget);
    //    AssetDatabase.Refresh();//刷新编辑器，不刷新的话，需要手动refresh一下，才会显示。
    //}
    [MenuItem("Tools/测试版本写入")]
    public static void TestWriteVersion() {
        SaveVesion(PlayerSettings.bundleVersion, PlayerSettings.applicationIdentifier);//通过读取unity的playersetting设置来判断是否写入成功
    }
    /// <summary>
    /// 写入版本号,目前版本信息使用txt用文件流读写，其实可以考虑使用xml或者json序列化存储
    /// </summary>
    /// <param name="version">版本号</param>
    /// <param name="package">包名</param>
    static void SaveVesion(string version, string package) {
        string content = "Versio|" + version + ";Packagename|" + package + ";";
        string savepath = Application.dataPath + "/Resources/Version.txt";
        string all = "";
        string firstline = "";
        using (FileStream fs = new FileStream(savepath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite)) {
            using (StreamReader sr = new StreamReader(fs, System.Text.Encoding.UTF8)) {
                all = sr.ReadToEnd();
                firstline = all.Split('\r')[0];//空格分隔，读取第一行
            }
        }
        using (FileStream fs = new FileStream(savepath, FileMode.OpenOrCreate)) {
            using (StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.UTF8)) {
                if (string.IsNullOrEmpty(all)) {
                    all = content;
                } else {
                    all.Replace(firstline, content);
                }
                sw.Write(all);
            }
        }
    }
    /// <summary>
    /// 拷贝某个文件或者文件夹下所有文件到目标路径
    /// </summary>
    /// <param name="scrpath">原路径</param>
    /// <param name="targetpath">目标路径</param>
    private static void Copy(string scrpath, string targetpath) {
        try {
            if (!Directory.Exists(targetpath)) {
                Directory.CreateDirectory(targetpath);
            }
            string scrdic = Path.Combine(targetpath, Path.GetFileName(scrpath));
            if (Directory.Exists(scrpath)) {//通过exists判断是不是文件夹
                //Path.DirectorySeparatorChar用于分隔目录级别的默认字符（只读）,此字符在Windows上是 '\'，在macOS上是 ' / '
                scrdic += Path.DirectorySeparatorChar;
            }
            if (!Directory.Exists(scrdic)) {
                Directory.CreateDirectory(scrdic);
            }
            string[] filearr = Directory.GetFileSystemEntries(scrpath);//获取文件夹下所有文件的句柄
            foreach (string file in filearr) {
                if (Directory.Exists(file)) {
                    Copy(file, scrdic);//如果还是文件夹就递归调用
                } else {
                    File.Copy(file, scrdic + Path.GetFileName(file), true);
                }
            }
        } catch (Exception) {
            Debug.LogError("无法复制：" + scrpath + "到" + targetpath);
        }

    }
}
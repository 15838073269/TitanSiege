/****************************************************
    文件：BulidAseetBundle.cs
	作者：昔莘
    邮箱: 304183153@qq.com
    日期：2021/6/27 19:39:47
	功能：这是一套新的打包部署方案，整体比原版方案简单，就是根据场景分资源，然后每个场景打一个包。但资源依赖处理上不是很完善，适合小且逻辑不复杂的项目。
*****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System;
using GF;
#region CTO的一键打包部署方案
public class BulidAseetBundle {
   // [MenuItem("Tools/BulidAssets")]
    public static void BulidAssets() {
        //string path = AppConfig.AssetBundlePath;//设置打包的路径
        string path = Application.streamingAssetsPath;
        if (!Directory.Exists(path)) {
            Directory.CreateDirectory(path);
        }
        //调用函数，设置打包参数，       路径，  打包压缩方式，默认none是lzm4   打包目标平台 
        BuildPipeline.BuildAssetBundles(path, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows64);
    }
    //[MenuItem("Tools/LayoutAssets")]
    public static void LayoutAssets() {
        AssetDatabase.RemoveUnusedAssetBundleNames();//清除所有没用的ab名称
        string path = Application.dataPath + "/AssetBundles/";//设置处理的路径
        if (Directory.Exists(path)) {
            DirectoryInfo dir = new DirectoryInfo(path);//创建路径类
            FileSystemInfo[] filesfor = dir.GetFileSystemInfos();//获取路径下所有文件包括子文件夹
            for (int i = 0; i < filesfor.Length; i++) {
                if (filesfor[i] is DirectoryInfo) {//如果是个文件夹，就要再深一层遍历，此处应该只会有文件下，因为此处是AssetBundles文件夹下，都是各个场景文件夹，所以不需要判断有文件的情况。
                    //更新路径
                    string newpath = Path.Combine(path, filesfor[i].Name);
                    SceneDirectory(newpath);
                }
            }
        } else {
            Debug.Log("路径" + path + "不存在！");
        }
    }
    private static void SceneDirectory(string Aseetpath) {
        //设置对应关系，因为路径大小写都有，而assetbundle中只能有小写，需要将两者的对应关系记录下来
        //记录关系的文档
        string txtname = "AssetBundleRecord.txt";
        string txtpath = Aseetpath + txtname;
        FileStream fs = new FileStream(txtpath, FileMode.OpenOrCreate);//如有就打开，没有就创建再打开
        StreamWriter sw = new StreamWriter(fs);
        //存储关联数据的字典
        Dictionary<string, string> recordDic = new Dictionary<string, string>();
        ////调用方法
        //string newpath = SubPath(scenepath);
        //获取路径下所有文件
        DirectoryInfo dir = new DirectoryInfo(Aseetpath);
        if (dir != null) {
            AddAssetBundle(dir, Aseetpath, recordDic);
        } else {
            Debug.Log(Aseetpath + "文件夹不存在！");
        }
        foreach (var k in recordDic.Keys) {
            string str = k + "-" + recordDic[k];
            //Debug.Log(str);
            sw.WriteLine(str);
        }
        sw.Close();
        fs.Close();
    }
    ////处理路径的字符串，拿出最后的路径名
    //public static string SubPath(string fullpath) {
    //    //获取路径中assets所在的位置，从此位置之后开始截取
    //    int tmpcount = fullpath.IndexOf("Assets");
    //    string newpath = fullpath.Substring(tmpcount);//相当于将unity项目的工程目录拆掉了，剩下的目录应该为Art/Scenes/文件夹...
    //    return newpath;
    //}
    //将文件夹下所有资源加上assetbundle标志，从此处开始才到每个场景文件夹下
    public static void AddAssetBundle(DirectoryInfo dirinfo, string path, Dictionary<string, string> recordDic) {
        FileSystemInfo[] filesfor = dirinfo.GetFileSystemInfos();
        for (int i = 0; i < filesfor.Length; i++) {
            if (filesfor[i] is DirectoryInfo) {//如果还是路径，证明文件夹下有目录，递归获取所有数据
                AddAssetBundle((DirectoryInfo)filesfor[i], Path.Combine(path, filesfor[i].Name), recordDic);
            } else {
                FileInfo tmpfile = (FileInfo)filesfor[i];
                if (tmpfile != null && tmpfile.Extension != ".meta") {//meta文件屏蔽掉，不打包
                    path = path.Replace("\\", "/");//更换路径的斜杠
                    string abname = GetABName(path, tmpfile);
                    string assetpath = path.Substring(path.IndexOf("Assets")) + "/" + tmpfile.Name.Replace("\\", "/");
                    AssetImporter importer = AssetImporter.GetAtPath(assetpath);
                    if (importer != null) {
                        importer.assetBundleName = abname;
                        if (tmpfile.Extension == ".unity") {
                            importer.assetBundleVariant = "u3d";
                        } else {
                            importer.assetBundleVariant = "ab";
                        }
                        //存储到字典中，格式  包名称---包对应文件夹的路径
                        string lowabname = abname.ToLower();
                        string[] dicstr = lowabname.Split('/');
                        string dicname = "";
                        if (dicstr.Length == 1) {
                            dicname = dicstr[0];
                        } else {
                            dicname = dicstr[1];
                        }
                        int abcount = path.IndexOf(dicname);
                        int arcount = path.IndexOf("AssetBundles");//Assetbundles是当前此项目资源文件夹的名称，如果该名称有变，需要同时修改这里
                        string newname = path.Substring(arcount + 12, abcount - arcount - 12);//-12是为了去掉AssetBundles
                        recordDic[dicname] = newname + dicname + "." + importer.assetBundleVariant;
                    }
                }
            }
        }

    }
    //获取assetbundle的名字
    public static string GetABName(string fullpath, FileInfo file) {
        string tmpname = "";
        //获取路径中assets所在的位置，从此位置之后开始截取
        int tmpcount = fullpath.IndexOf("AssetBundles");//AssetBundles是当前此项目资源文件夹的名称，如果该名称有变，需要同时修改这里
        string newpath = fullpath.Substring(tmpcount + 13);//相当于将unity项目的工程目录拆掉了，只保留的资源的根目录，AssetBundles 12个字符，再加上一个/，正好13
        Debug.Log(newpath);
       // int centercount = newpath.IndexOf(file.Name);//通过文件名将路径中的文件名去掉，只保留中间部分
        //string centerpath = newpath.Substring(0, newpath.Length - centercount - 1);//减1是因为需要去掉一个\
        //通过这个判断是否有二级目录
        string[] namestr = newpath.Split('/');//Path.Combine拼合的路径都是\的，因为windows下的路径是\，u3d的路径是/的。
        if (namestr.Length > 1) {//此处是因为本程序只进行二级目录打包，每个二级目录内的东西为一个包，如果有需要可以自由修改打包的目录等级
            tmpname = namestr[0] + "/" + namestr[1];
        } else {//这种情况只有在Scenesname根目录下文件，例如场景文件
            tmpname = namestr[0];
        }
        //Debug.Log(tmpname);
        return tmpname;
    }
    #endregion
   
}
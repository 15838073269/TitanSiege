/****************************************************
    文件：BuildApp.cs
	作者：昔莘
    邮箱: 304183153@qq.com
    日期：2021/10/26 15:12:45
	功能：打包工具，用来代码打包，后面可以和一键打包工具结合在一起
*****************************************************/

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
using System.IO;

public class BuildApp 
{
    /// <summary>
    /// app名称
    /// </summary>
    private static string m_AppName = PlayerSettings.productName;
    /// <summary>
    /// 不同平台打包路径
    /// </summary>
    public static string m_AndroidPath = $"{Application.dataPath}/../BuildTarget/Android/";
    public static string m_IOSPath = $"{Application.dataPath}/../BuildTarget/IOS/";
    public static string m_WindowsPath = $"{Application.dataPath}/../BuildTarget/Windows/";
    private static string m_ABPath = $"{Application.dataPath}/../AssetBundle/{EditorUserBuildSettings.activeBuildTarget.ToString()}/";

    [MenuItem("Build/标准包")]
    public static void Build() {
        //打包之前把所有配置表转一下二进制，避免个别表忘记转了
        DataEditor.AllXmlToBinary();
        //打包之前先打一下ab包
        BuildEditor.Build();
        //将对应版本的ab包拷贝daostreamingasset下
        Copy(m_ABPath,Application.streamingAssetsPath);
        //设置打包保存路径
        string savepath = "";
        string date = string.Format("{0:yyyy_MM_DD_HH_mm}", DateTime.Now);
        if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android) {//安卓平台打包路径
            savepath = $"{m_AndroidPath}{m_AppName}_{EditorUserBuildSettings.activeBuildTarget.ToString()}_{date}.apk";
        } else if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.iOS) {//ios平台，ios平台打包是一个xcode工程
            savepath = $"{m_IOSPath}{m_AppName}_{EditorUserBuildSettings.activeBuildTarget.ToString()}_{date}";
        } else if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.StandaloneWindows|| EditorUserBuildSettings.activeBuildTarget == BuildTarget.StandaloneWindows64) {//windows平台是个文件夹，最终打包是exe
            savepath = $"{m_WindowsPath}{m_AppName}_{EditorUserBuildSettings.activeBuildTarget.ToString()}_{date}/{m_AppName}.exe";
        }

        BuildPipeline.BuildPlayer(FindEnableEditorScene(), savepath, BuildTarget.Android, BuildOptions.None)
        ;
        //打完包，把移动过来的ab包删除
        DeleteDir(Application.streamingAssetsPath);
    }
    /// <summary>
    /// 查询当前添加在usesetting中的scene
    /// </summary>
    /// <returns></returns>
    private static string[] FindEnableEditorScene() {
        List<string> allscenes = new List<string>();
        foreach (EditorBuildSettingsScene scene  in EditorBuildSettings.scenes) {
            if (!scene.enabled) {
                continue;
            }
            allscenes.Add(scene.path);
        }
        return allscenes.ToArray();
    }
    /// <summary>
    /// 文件拷贝的方法，一般用来打包时从ab包目录拷贝文件到streammingAssets目录
    /// </summary>
    /// <param name="srcpath">原目录</param>
    /// <param name="targetpath">目标目录</param>
    private static void Copy(string srcpath,string targetpath) {
        try {
            //文件夹目录处理
            if (!Directory.Exists(targetpath)) {
                Directory.CreateDirectory(targetpath);
            }
            string srcdir = Path.Combine(targetpath,Path.GetFileName(srcpath));//如果srcpath是个文件，不是文件夹，就直接和目标目录合并
            if (Directory.Exists(srcpath)) {//如果存在原路径
                srcdir += Path.DirectorySeparatorChar;//Path.DirectorySeparatorChar就是反斜杠
                //这个操作是因为srcpath可能就是1个文件，不是文件夹，所以要先判断一下，如果是文件夹，那么上面的合并就执行为空，此时srcdir就等于targetpath，目标目录也需要加上\
            }
            if (!Directory.Exists(srcdir)) { //如果拼接的路径不存在就创建
                Directory.CreateDirectory(srcdir);
            }
            //文件处理
            string[] files = Directory.GetFileSystemEntries(srcpath);//获取文件下所有文件
            foreach (string file  in files) {
                if (Directory.Exists(file)) {//如果是文件夹
                    Copy(file, srcdir);
                } else {
                    File.Copy(file,srcdir+Path.GetFileName(file),true);//是文件就拷贝过去
                }
            }
        } catch (Exception e) {
            Debug.LogError($"移动文件失败，错误代码：{e.InnerException}");
            throw;
        }
    }
    /// <summary>
    /// 打包后删除文件夹
    /// </summary>
    public static void DeleteDir(string path) {
        try {
            DirectoryInfo dir = new DirectoryInfo(path);
            FileSystemInfo[] fileinfo = dir.GetFileSystemInfos();
            foreach (FileSystemInfo info in fileinfo) {
                if (info is DirectoryInfo) {
                    DirectoryInfo subdir = new DirectoryInfo(info.FullName);
                    subdir.Delete(true);
                } else {
                    File.Delete(info.FullName);
                }
            }
        } catch (Exception e) {
            Debug.LogError(e.InnerException);
            throw;
        }
    }
}
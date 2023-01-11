/****************************************************
    文件：ABConfig.cs
	作者：昔莘
    邮箱: 304183153@qq.com
    日期：2021/8/9 15:37:53
	功能：自定义打包配置表
*****************************************************/
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName ="ABConfig",menuName = "CreateABConfig",order = 0)]
public class ABConfig : ScriptableObject {
    //基于文件下的单个文件进行打包(一般针对prefab)
    //单个文件所在路径，会遍历这个文件夹下所有prefab，所以prefab名称不能重复
    public List<string> m_AllPrefabPath = new List<string>();
    //基文件夹进行打包
    public List<FileDirABName> m_AllFileDirAB = new List<FileDirABName>();
    /// <summary>
    /// 用结构体存储单个ab包名称和路径，因为结构体可以序列化，字典不能序列化
    /// </summary>
    [System.Serializable]//加上标签，对结构体序列化
    public struct FileDirABName {
        public string ABName;
        public string Path;
    }


}
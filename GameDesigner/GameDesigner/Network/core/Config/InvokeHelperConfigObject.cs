using System.Collections.Generic;
using System;
#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_WSA || UNITY_WEBGL
using UnityEngine;

public class InvokeHelperConfigObject : ScriptableObject
{
    public InvokeHelperConfig Config = new InvokeHelperConfig();
}
#elif SERVICE
public class Header : Attribute 
{
    public Header(string text) { }
}
#endif
[Serializable]
public class InvokeHelperConfig
{
    [Header("true:当脚本编译后调用，检测生成SyncVar和Rpc的代码，false:不启用检测")]
    public bool onReloadInvoke = true;
    [Header("true:在unity启用字段同步帮助类生成的代码 false:运行时动态编译字段同步帮助类")]
    public bool syncVarClientEnable;
    [Header("true:在server启用字段同步帮助类生成的代码 false:运行时动态编译字段同步帮助类")]
    public bool syncVarServerEnable;
    [Header("前端Rpc收集, 可视化后端Rpc方法")]
    public bool collectRpc;
    [Header("SyncVar同步字段记录类, 可用于定义字段为基类, 但是赋值却是派生类对象")]
    public bool recordType;
    [Header("生成的脚本存放路径(unity)")]
    public string savePath;
    [Header("收集程序集路径(unity)")]
    public List<string> dllPaths = new List<string>();
    [Header("Rpc辅助")]
#if UNITY_2020_1_OR_NEWER && UNITY_EDITOR
    [NonReorderable]
#endif
    public List<InvokeHelperConfigData> rpcConfig = new List<InvokeHelperConfigData>();
    public bool foldout;
    public int rpcConfigSize;
}

[Serializable]
public class InvokeHelperConfigData 
{
    public string name;
    [Header("VS项目文件路径")]
    public string csprojPath;
    [Header("生成的脚本存放路径")]
    public string savePath;
    [Header("读取配置数据路径")]
    public string readConfigPath;
    [Header("后端Rpc收集, 可视化前端Rpc方法")]
    public bool collectRpc;
    [Header("收集程序集路径")]
    public List<string> dllPaths = new List<string>();
    public bool foldout;
}

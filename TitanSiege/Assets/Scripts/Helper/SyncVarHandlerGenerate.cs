using Net.Helper;
using Net.Serialize;
using Net.Share;
using Net.System;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

/**
/// <summary>此类必须在主项目程序集, 如在unity时必须是Assembly-CSharp程序集, 在控制台项目时必须在Main入口类的程序集</summary>
internal partial class SyncVarHandlerGenerate : ISyncVarHandler
{
    public virtual int SortingOrder => 0;

    public void Init()
    {
        SyncVarGetSetHelper.Cache.Clear();

    }


}*/

/// <summary>SyncVar动态编译定位路径</summary>
internal static class HelperFileInfo 
{
    internal static string GetPath()
    {
        return GetClassFileInfo();
    }

    internal static string GetClassFileInfo([CallerFilePath] string sourceFilePath = "")
    {
        return sourceFilePath;
    }
}
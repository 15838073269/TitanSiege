using Net.Helper;
using Net.Serialize;
using Net.Share;
using Net.System;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

/**internal static class InvokeHelperGenerate
{
#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_WSA
    [UnityEngine.RuntimeInitializeOnLoadMethod]
#else
    [RuntimeInitializeOnLoadMethod]
#endif
    internal static void Init()
    {
        InvokeHelper.Cache.Clear();

    }

}*/

internal static class RpcInvokeHelper
{

}

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

internal static class RpcCallSequencePointHelper
{
    #if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_WSA
    [UnityEngine.RuntimeInitializeOnLoadMethod]
#else
    [RuntimeInitializeOnLoadMethod]
#endif
    internal static void Init()
    {
        RpcCallHelper.Cache.Clear();

        RpcCallHelper.Cache.Add("GF.Service.UserService.LoginCallback", new SequencePoint(){ FilePath = @"D:\project\TitanSiege\TitanSiege\Assets\Scripts\Service\UserService.cs", StartLine = 77 });

        RpcCallHelper.Cache.Add("GF.Service.UserService.LoginFailCallback", new SequencePoint(){ FilePath = @"D:\project\TitanSiege\TitanSiege\Assets\Scripts\Service\UserService.cs", StartLine = 89 });

        RpcCallHelper.Cache.Add("GF.Service.UserService.RegisterCallback", new SequencePoint(){ FilePath = @"D:\project\TitanSiege\TitanSiege\Assets\Scripts\Service\UserService.cs", StartLine = 103 });

        RpcCallHelper.Cache.Add("GF.MainGame.Module.CreateAndSelectModule.CreateRoleCallBack", new SequencePoint(){ FilePath = @"D:\project\TitanSiege\TitanSiege\Assets\Scripts\Module\CreateAndSelectModule.cs", StartLine = 100 });

        RpcCallHelper.Cache.Add("GF.MainGame.Module.CreateAndSelectModule.DeletePlayerCallBack", new SequencePoint(){ FilePath = @"D:\project\TitanSiege\TitanSiege\Assets\Scripts\Module\CreateAndSelectModule.cs", StartLine = 171 });

        RpcCallHelper.Cache.Add("GF.MainGame.Module.SceneModule.SwitchSceneCallBack", new SequencePoint(){ FilePath = @"D:\project\TitanSiege\TitanSiege\Assets\Scripts\Module\SceneModule\SceneModule.cs", StartLine = 88 });

    }
}
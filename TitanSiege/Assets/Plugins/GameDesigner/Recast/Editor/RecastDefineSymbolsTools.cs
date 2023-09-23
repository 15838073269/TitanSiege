#if UNITY_EDITOR
using UnityEditor;

namespace Net.AI
{
    public class RecastDefineSymbolsTools
    {
        [MenuItem("GameDesigner/Recast/使用C++版本")]
        private static void RecastNative()
        {
            BuildTargetGroup currentBuildTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
            string defineSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(currentBuildTargetGroup);
            if (!defineSymbols.Contains("RECAST_NATIVE"))
            {
                defineSymbols += ";RECAST_NATIVE";
            }
            PlayerSettings.SetScriptingDefineSymbolsForGroup(currentBuildTargetGroup, defineSymbols);
        }

        [MenuItem("GameDesigner/Recast/使用Net版本")]
        private static void RecastNet()
        {
            BuildTargetGroup currentBuildTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
            string defineSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(currentBuildTargetGroup);
            if (defineSymbols.Contains("RECAST_NATIVE"))
            {
                defineSymbols = defineSymbols.Replace("RECAST_NATIVE", "");
            }
            PlayerSettings.SetScriptingDefineSymbolsForGroup(currentBuildTargetGroup, defineSymbols);
        }
    }
}
#endif
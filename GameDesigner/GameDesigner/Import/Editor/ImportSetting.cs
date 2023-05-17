#if UNITY_EDITOR
using UnityEditor;

[InitializeOnLoad]
public static class ImportSetting
{
    static ImportSetting() 
    {
        //0 = Recompile And Continue Playing
        //1 = Recompile After Finished Playing
        //2 = Stop Playing And Recompile
        EditorPrefs.SetInt("ScriptCompilationDuringPlay", 1);
    }
}
#endif
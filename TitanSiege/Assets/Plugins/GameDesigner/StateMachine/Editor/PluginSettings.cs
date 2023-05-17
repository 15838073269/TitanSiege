#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public class PluginSettings : EditorWindow
{
    private PluginLanguage language;

    private void Awake()
    {
        language = BlueprintSetting.Instance.language;
        InitPlugin();
    }

    public static void InitPlugin()
    {
        string text;
        if (BlueprintSetting.Instance.language == PluginLanguage.Chinese)
        {
            var obj = Resources.Load<TextAsset>("ChineseLanguage");
            text = System.Text.Encoding.UTF8.GetString(obj.bytes);
        }
        else
        {
            var obj = Resources.Load<TextAsset>("EnglishLanguage");
            text = System.Text.Encoding.UTF8.GetString(obj.bytes);
        }
        if (text.Contains("\r\n")) //个别机器不知道什么情况, 只有\n
            BlueprintSetting.Instance.LANGUAGE = text.Split(new string[] { "\r\n" }, 0);
        else
            BlueprintSetting.Instance.LANGUAGE = text.Split(new string[] { "\n" }, 0);
    }

    [MenuItem("GameDesigner/PluginSettings")]
    static void Init()
    {
        var setting = GetWindow<PluginSettings>();
        setting.maxSize = new Vector2(300, 100);
        setting.Show();
    }

    void OnGUI()
    {
        if (language != BlueprintSetting.Instance.language)
        {
            language = BlueprintSetting.Instance.language;
            InitPlugin();
        }
        BlueprintSetting.Instance.language = (PluginLanguage)EditorGUILayout.EnumPopup(BlueprintSetting.Instance.LANGUAGE[83], BlueprintSetting.Instance.language);
    }
}
#endif
/****************************************************
    文件：OfflineDataEditor.cs
	作者：昔莘
    邮箱: 304183153@qq.com
    日期：2021/9/25 11:4:17
	功能：离线数据的编辑器处理脚本，主要用来记录离线数据
*****************************************************/
using UnityEngine;
using UnityEditor;
using GF.Unity.AB;
public class OfflineDataEditor {
    [MenuItem("Assets/生成离线数据")]  //Assets目录是右键的菜单栏
    public static void AssetsCreateOfflineData() {
        GameObject[] objarr = Selection.gameObjects;//鼠标选中的游戏物体
        for (int i = 0; i < objarr.Length; i++) {
            EditorUtility.DisplayProgressBar("添加离线数据", "正在修改" + objarr[i] + "----", 1.0f / objarr.Length * i);
            CreateOfflineData(objarr[i]);
        }
        EditorUtility.ClearProgressBar();
    }
    [MenuItem("Assets/生成UI离线数据")]  //Assets目录是右键的菜单栏
    public static void AssetsCreateUIOfflineData() {
        GameObject[] objarr = Selection.gameObjects;//鼠标选中的游戏物体
        for (int i = 0; i < objarr.Length; i++) {
            EditorUtility.DisplayProgressBar("添加UI离线数据", "正在修改" + objarr[i] + "----", 1.0f / objarr.Length * i);
            CreateUIOfflineData(objarr[i]);
        }
        EditorUtility.ClearProgressBar();
    }
    [MenuItem("Tools/生成所有UI离线数据")]  //Assets目录是右键的菜单栏
    public static void AllCreateUIOfflineData() {
        string[] strarr = AssetDatabase.FindAssets("t:Prefab", new string[] { "Assets/Art/UIPrefab" });
        for (int i = 0; i < strarr.Length; i++) {
            string strpath = AssetDatabase.GUIDToAssetPath(strarr[i]);
            EditorUtility.DisplayProgressBar("添加UI离线数据", "正在修改" + strarr[i] + "----", 1.0f / strarr.Length * i);
            GameObject obj = AssetDatabase.LoadAssetAtPath<GameObject>(strpath);
            if (obj == null) {
                continue;
            }
            CreateUIOfflineData(obj);
        }
        Debug.Log("UI离线数据生成完毕");
        EditorUtility.ClearProgressBar();
    }

    [MenuItem("Assets/生成特效离线数据")]  //Assets目录是右键的菜单栏
    public static void AssetsCreateEffectOfflineData() {
        GameObject[] objarr = Selection.gameObjects;//鼠标选中的游戏物体
        for (int i = 0; i < objarr.Length; i++) {
            EditorUtility.DisplayProgressBar("添加特效离线数据", "正在修改" + objarr[i] + "----", 1.0f / objarr.Length * i);
            CreateEffectOfflineData(objarr[i]);
        }
        EditorUtility.ClearProgressBar();
    }
    [MenuItem("Tools/生成所有特效离线数据")]  //Assets目录是右键的菜单栏
    public static void AllCreateEffectOfflineData() {
        string[] strarr = AssetDatabase.FindAssets("t:Prefab", new string[] { "Assets/Art/EffectIPrefab" });
        for (int i = 0; i < strarr.Length; i++) {
            string strpath = AssetDatabase.GUIDToAssetPath(strarr[i]);
            EditorUtility.DisplayProgressBar("添加特效离线数据", "正在修改" + strarr[i] + "----", 1.0f / strarr.Length * i);
            GameObject obj = AssetDatabase.LoadAssetAtPath<GameObject>(strpath);
            if (obj == null) {
                continue;
            }
            CreateEffectOfflineData(obj);
        }
        Debug.Log("特效离线数据生成完毕");
        EditorUtility.ClearProgressBar();
    }
    /// <summary>
    /// 创建GameObject离线数据
    /// </summary>
    /// <param name="obj"></param>
    public static void CreateOfflineData(GameObject obj) {
        OfflineDatabase offline = obj.GetComponent<OfflineDatabase>();
        if (offline == null) {
            offline = obj.AddComponent<OfflineDatabase>();
        }
        offline.BindData();
        EditorUtility.SetDirty(obj);
        Debug.Log("修改" + obj.name + "prefab成功");
        Resources.UnloadUnusedAssets();
        AssetDatabase.Refresh();
    }
    /// <summary>
    /// 创建UI离线数据
    /// </summary>
    /// <param name="UI"></param>
    public static void CreateUIOfflineData(GameObject obj) {
        UIOfflineData offline = obj.GetComponent<UIOfflineData>();
        if (offline == null) {
            offline = obj.AddComponent<UIOfflineData>();
        }
        offline.BindData();
        EditorUtility.SetDirty(obj);
        Debug.Log("修改UI" + obj.name + "prefab成功");
        Resources.UnloadUnusedAssets();
        AssetDatabase.Refresh();
    }
    /// <summary>
    /// 创建特效离线数据
    /// </summary>
    /// <param name="obj"></param>
    public static void CreateEffectOfflineData(GameObject obj) {
        EffectOfflineData offline = obj.GetComponent<EffectOfflineData>();
        if (offline == null) {
            offline = obj.AddComponent<EffectOfflineData>();
        }
        offline.BindData();
        EditorUtility.SetDirty(obj);
        Debug.Log("修改特效" + obj.name + "prefab成功");
        Resources.UnloadUnusedAssets();
        AssetDatabase.Refresh();
    }
}
/****************************************************
    文件：SceneDataToXml.cs
	作者：昔莘
    邮箱: 304183153@qq.com
    日期：2022/12/8 10:52:47
	功能：Nothing
*****************************************************/

using GF.ConfigTable;
using GF.MainGame.Data;
using GF.MainGame.Module.NPC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class SceneDataToXml 
{
    [MenuItem("Tools/SceneDataToXml")]
    public static void DataToXml() {//选中场景中SceneBase脚本挂载的物体后执行
        if (Selection.activeObject) {
            GetDataToXml((GameObject)Selection.activeObject);
        }
    }
    public static void GetDataToXml(GameObject go) {
        Transform MonsterFather = go.transform.Find("Monster");
        if (MonsterFather != null) {
            Transform[] MonsterArr = MonsterFather.transform.GetComponentsInChildren<Transform>(true);
            if (MonsterArr.Length > 0) {
                SceneNData snd = new SceneNData();
                snd.Construction();
                for (int i = 0; i < MonsterArr.Length; i++) {
                    NData md = new NData();
                    string path = AssetDatabase.GetAssetPath(MonsterArr[i].gameObject);
                    string[] strarr = path.Split('/');
                    md.prepath = strarr[strarr.Length - 1];
                    md.ndid = i;
                    md.posx = MonsterArr[i].position.x;
                    md.posy = MonsterArr[i].position.y;
                    md.posz = MonsterArr[i].position.z;
                    md.rotax = MonsterArr[i].rotation.x;
                    md.rotay = MonsterArr[i].rotation.y;
                    md.rotaz = MonsterArr[i].rotation.z;
                    md.scalx = MonsterArr[i].localScale.x;
                    md.scaly = MonsterArr[i].localScale.y;
                    md.scalz = MonsterArr[i].localScale.z;
                    snd.AllNDatas.Add(md);
                }
                try {
                    if (snd.AllNDatas.Count >0) {
                        string xmlPath = $"{DataEditor.XmlPath}{snd.m_Name}.xml";
                        BinarySerializeOpt.Xmlserialize(xmlPath, snd);//通过工具转换
                        Debug.Log(snd.m_Name + "类转xml成功，xml路径为:" + xmlPath);
                    } else {
                        Debug.Log($"未在程序集中找到{snd.m_Name}脚本");
                    }
                } catch (Exception e) {
                    Debug.LogError(snd.m_Name + "类转xml失败！错误内容：" + e.Message);
                }
            } else {
                Debug.LogError("Monster父物体下没有任何物体,请检查");
            }
        } else {
            Debug.LogError("Monster的父物体不存在，请检查是否拼写错误");
        }
    }
    private static void ClassToXml(string name) {
        if (string.IsNullOrEmpty(name))
            return;

        
    }
}



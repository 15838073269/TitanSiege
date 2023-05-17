/****************************************************
    文件：ToolsEditor.cs
	作者：昔莘
    邮箱: 304183153@qq.com
    日期：2021/12/14 15:27:15
	功能：Nothing
*****************************************************/

using GF.MainGame.Module;
using GF.MainGame.Module.NPC;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEditor;
using UnityEngine;

public class ToolsEditor : MonoBehaviour {
    [MenuItem("Tools/Clear")]
    public static void ClearAnimationByChild() {
        if (Selection.activeObject) {
            ClearAnimationByChild((GameObject)Selection.activeObject);
        }
    }
    public static void ClearAnimationByChild(GameObject obj) {
        //1.子物体有
        if (obj.transform.childCount > 0) {
            for (int i = 0; i < obj.transform.childCount; i++) {
                GameObject child = obj.transform.GetChild(i).gameObject;
                Animation ani = child.GetComponent<Animation>();
                if (ani != null) {
                    DestroyImmediate(ani);
                }
                ClearAnimationByChild(child);
            }
        }
    }
    static string meshDir = "Assets/Art/CombineMesh/";
    [MenuItem("Tools/ComblineToNew")]
    public static void Combine() {
        GameObject[] objs = Selection.gameObjects;
       
        for (int j = 0; j < objs.Length; j++) {
            //---------------- 先获取材质 -------------------------
            //获取自身和所有子物体中所有MeshRenderer组件
            MeshRenderer[] mrChildren = objs[j].GetComponentsInChildren<MeshRenderer>();
            //新建材质球数组
            Material[] mats = new Material[mrChildren.Length];
            for (int i = 0; i < mrChildren.Length; i++) {
                //生成材质球数组 
                mats[i] = mrChildren[i].sharedMaterial;
            }
            //获取需要合并的所有mesh
            MeshFilter[] mfChildren = objs[j].GetComponentsInChildren<MeshFilter>();
            CombineInstance[] combine = new CombineInstance[mfChildren.Length];

            Matrix4x4 matrix = objs[j].transform.worldToLocalMatrix;
            //创建贴图数组,合并贴图
            Texture2D[] textures = new Texture2D[mrChildren.Length];
            for (int i = 0; i < mrChildren.Length; i++) {
                if (mrChildren[i].transform == objs[j].transform) {
                    continue;
                }
                mats[i] = mrChildren[i].sharedMaterial;
                Texture2D tx = mats[i].GetTexture("_MainTex") as Texture2D;


                Texture2D tx2D = new Texture2D(tx.width, tx.height, TextureFormat.ARGB32, false);
                tx2D.SetPixels(tx.GetPixels(0, 0, tx.width, tx.height));
                tx2D.Apply();
                textures[i] = tx2D;
            }

            Material materialNew = new Material(mats[0].shader);
            materialNew.CopyPropertiesFromMaterial(mats[0]);
            //父物体添加mesh组件，之所以放在这里添加，而不是一开始添加上，是为了防止GetComponentsInChildren遍历到自己
            MeshRenderer myrender = objs[j].AddComponent<MeshRenderer>();
            MeshFilter myfilter = objs[j].AddComponent<MeshFilter>();
           myrender.sharedMaterial = materialNew;


            Texture2D texture = new Texture2D(1024, 1024);
            materialNew.SetTexture("_MainTex", texture);
            Rect[] rects = texture.PackTextures(textures, 10, 1024);

            for (int i = 0; i < mfChildren.Length; i++) {
                if (mfChildren[i].transform == objs[j].transform) {
                    continue;
                }
                Rect rect = rects[i];
                Mesh meshCombine = mfChildren[i].sharedMesh;
                Vector2[] uvs = new Vector2[meshCombine.uv.Length];
                //把网格的uv根据贴图的rect刷一遍
                for (int v = 0; v < uvs.Length; v++) {
                    uvs[v].x = rect.x + meshCombine.uv[v].x * rect.width;
                    uvs[v].y = rect.y + meshCombine.uv[v].y * rect.height;
                }
                meshCombine.uv = uvs;
                combine[i].mesh = meshCombine;
                combine[i].transform = mfChildren[i].transform.localToWorldMatrix;
                mfChildren[i].gameObject.SetActive(false);
            }

            Mesh mesh = new Mesh();
            mesh.name = objs[j].name;
            mesh.CombineMeshes(combine, true,true);
            myfilter.mesh = mesh;
            string path = meshDir + mesh.name + ".asset";
            AssetDatabase.CreateAsset(mesh, path);
        }
        AssetDatabase.Refresh();
    }

    
}
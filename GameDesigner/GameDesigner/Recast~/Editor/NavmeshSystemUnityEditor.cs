#if UNITY_EDITOR
using System.Diagnostics;
using UnityEditor;
using UnityEngine;

namespace Net.AI 
{
    [CustomEditor(typeof(NavmeshSystemUnity))]
    public class NavmeshSystemUnityEditor : Editor
    {
        private NavmeshSystemUnity System;

        private void OnEnable()
        {
            System = target as NavmeshSystemUnity;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("Bake"))
            {
                var stopwatch = Stopwatch.StartNew();
                System.Bake();
                stopwatch.Stop();
                UnityEngine.Debug.Log($"烘焙完成,用时:{stopwatch.ElapsedMilliseconds}ms");
            }
            if (GUILayout.Button("LoadNavMesh"))
            {
                var stopwatch = Stopwatch.StartNew();
                System.Load();
                stopwatch.Stop();
                UnityEngine.Debug.Log($"加载烘焙网格完成,用时:{stopwatch.ElapsedMilliseconds}ms");
            }
            if (GUILayout.Button("LoadMeshObj"))
            {
                var stopwatch = Stopwatch.StartNew();
                System.LoadMeshObj();
                stopwatch.Stop();
                UnityEngine.Debug.Log($"加载网格模型完成,用时:{stopwatch.ElapsedMilliseconds}ms");
            }
            if (GUILayout.Button("SaveNavMesh"))
            {
                var stopwatch = Stopwatch.StartNew();
                System.Save();
                stopwatch.Stop();
                UnityEngine.Debug.Log($"保存烘焙网格完成,用时:{stopwatch.ElapsedMilliseconds}ms");
            }
            if (GUILayout.Button("SaveMeshObj"))
            {
                var stopwatch = Stopwatch.StartNew();
                System.SaveMeshObj();
                stopwatch.Stop();
                UnityEngine.Debug.Log($"保存网格模型完成,用时:{stopwatch.ElapsedMilliseconds}ms");
            }
            if (GUILayout.Button("BakeTerrain"))
            {
                var stopwatch = Stopwatch.StartNew();
                System.BakeTerrain();
                stopwatch.Stop();
                UnityEngine.Debug.Log($"烘焙地形网格完成,用时:{stopwatch.ElapsedMilliseconds}ms");
            }
            if (GUILayout.Button("SaveTerrainMesh"))
            {
                var stopwatch = Stopwatch.StartNew();
                System.SaveTerrainMesh();
                stopwatch.Stop();
                UnityEngine.Debug.Log($"保存地形网格完成,用时:{stopwatch.ElapsedMilliseconds}ms");
            }
        }
    }
}
#endif
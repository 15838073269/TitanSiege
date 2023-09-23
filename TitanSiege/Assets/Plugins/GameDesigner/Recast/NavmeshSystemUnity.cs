#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_WSA || UNITY_WEBGL
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using AmazingAssets.TerrainToMesh;
using UnityEngine;
using Net.Component;
#if RECAST_NATIVE
using Net.AI.Native;
using static Net.AI.Native.RecastDll;
#else
using Recast;
using static Recast.RecastGlobal;
#endif

namespace Net.AI
{
    public class NavmeshSystemUnity : SingleCase<NavmeshSystemUnity>
    {
        public NavmeshSystem System = new NavmeshSystem();
        public LayerMask bakeLayer;
        public string navMashPath;
        public int vertexCountHorizontal = 100;
        public int vertexCountVertical = 100;
        private Mesh navMesh;
        public bool drawNavmesh = true;
        public bool drawWireNavmesh = true;

        void Start()
        {
            Load();
        }

        public void ExportMesh(string filePath, Mesh mesh)
        {
            using (StreamWriter sw = new StreamWriter(filePath))
            {
                foreach (Vector3 vertex in mesh.vertices)
                {
                    sw.WriteLine("v " + vertex.x + " " + vertex.y + " " + vertex.z);
                }

                foreach (Vector3 normal in mesh.normals)
                {
                    sw.WriteLine("vn " + normal.x + " " + normal.y + " " + normal.z);
                }

                foreach (Vector2 uv in mesh.uv)
                {
                    sw.WriteLine("vt " + uv.x + " " + uv.y);
                }

                for (int submesh = 0; submesh < mesh.subMeshCount; submesh++)
                {
                    int[] triangles = mesh.GetTriangles(submesh);
                    for (int i = 0; i < triangles.Length; i += 3)
                    {
                        sw.WriteLine("f " + (triangles[i] + 1) + "/" + (triangles[i] + 1) + "/" + (triangles[i] + 1) +
                                     " " + (triangles[i + 1] + 1) + "/" + (triangles[i + 1] + 1) + "/" + (triangles[i + 1] + 1) +
                                     " " + (triangles[i + 2] + 1) + "/" + (triangles[i + 2] + 1) + "/" + (triangles[i + 2] + 1));
                    }
                }
            }
        }

        public string ExportMeshText(Mesh mesh)
        {
            var sw = new StringBuilder();
            foreach (Vector3 vertex in mesh.vertices)
            {
                sw.AppendLine("v " + vertex.x + " " + vertex.y + " " + vertex.z);
            }

            foreach (Vector3 normal in mesh.normals)
            {
                sw.AppendLine("vn " + normal.x + " " + normal.y + " " + normal.z);
            }

            foreach (Vector2 uv in mesh.uv)
            {
                sw.AppendLine("vt " + uv.x + " " + uv.y);
            }

            for (int submesh = 0; submesh < mesh.subMeshCount; submesh++)
            {
                int[] triangles = mesh.GetTriangles(submesh);
                for (int i = 0; i < triangles.Length; i += 3)
                {
                    sw.AppendLine("f " + (triangles[i] + 1) + "/" + (triangles[i] + 1) + "/" + (triangles[i] + 1) +
                                 " " + (triangles[i + 1] + 1) + "/" + (triangles[i + 1] + 1) + "/" + (triangles[i + 1] + 1) +
                                 " " + (triangles[i + 2] + 1) + "/" + (triangles[i + 2] + 1) + "/" + (triangles[i + 2] + 1));
                }
            }

            return sw.ToString();
        }

        public void Load()
        {
            System.Init(navMashPath);
            UpdateNavMeshFace();
        }

        public void LoadMeshObj()
        {
            System.Init();
            LoadMeshFile(System.Sample, navMashPath);
            Build(System.Sample);
            UpdateNavMeshFace();
        }

        public void Save()
        {
            System.Init();
            SaveNavMesh(System.Sample, navMashPath);
        }

        public void Bake()
        {
            var mesh = Merge();
            var objText = ExportMeshText(mesh);
            System.Init();
            LoadMeshData(System.Sample, objText);
            Build(System.Sample);
            UpdateNavMeshFace();
        }

        public void SaveMeshObj()
        {
            var mesh = Merge();
            var objText = ExportMeshText(mesh);
            File.WriteAllText(navMashPath, objText);
        }

        private unsafe void UpdateNavMeshFace()
        {
            int vertsCount = GetDrawNavMeshCount(System.Sample);
            float* vertsArray = stackalloc float[vertsCount];
            GetDrawNavMesh(System.Sample, vertsArray, out vertsCount);
            var m_Triangles = new List<RenderTriangle>();
            var col = new Color(0f, 1f, 1f, 1f);
            for (int i = 0; i < vertsCount; i += 9)
            {
                var a = new UnityEngine.Vector3(vertsArray[i + 0], vertsArray[i + 1], vertsArray[i + 2]);
                var b = new UnityEngine.Vector3(vertsArray[i + 3], vertsArray[i + 4], vertsArray[i + 5]);
                var c = new UnityEngine.Vector3(vertsArray[i + 6], vertsArray[i + 7], vertsArray[i + 8]);
                m_Triangles.Add(new RenderTriangle(a, b, c, col));
            }
            if (navMesh != null)
                DestroyImmediate(navMesh, true);
            navMesh = new Mesh();
            int triCount = m_Triangles.Count;
            var verts = new UnityEngine.Vector3[3 * triCount];
            var tris = new int[3 * triCount];
            var colors = new UnityEngine.Color[3 * triCount];
            for (int i = 0; i < triCount; ++i)
            {
                var tri = m_Triangles[i];
                int v = i * 3;
                for (int j = 0; j < 3; ++j)
                {
                    verts[v + j] = tri.m_Verts[j];
                    tris[v + j] = v + j;
                    colors[v + j] = tri.m_Colors[j];
                }
            }
            navMesh.vertices = verts;
            navMesh.triangles = tris;
            navMesh.colors = colors;
            navMesh.RecalculateNormals();
        }

        private Mesh Merge()
        {
            var meshFilters = FindObjectsOfType<MeshFilter>().Where(mf => ((1 << mf.gameObject.layer) & bakeLayer) > 0).ToArray();
            var mergedMesh = new Mesh();
            var combine = new CombineInstance[meshFilters.Length];
            for (int i = 0; i < meshFilters.Length; i++)
            {
                combine[i].mesh = meshFilters[i].sharedMesh;
                combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            }
            mergedMesh.CombineMeshes(combine);
            return mergedMesh;
        }

        public void BakeTerrain()
        {
            var terrain = FindObjectOfType<Terrain>();
            var mesh = terrain.terrainData.TerrainToMesh().ExportMesh(vertexCountHorizontal, vertexCountVertical, Normal.CalculateFromMesh);//AddTerrain(terrain);
            var objText = ExportMeshText(mesh);
            System.Init();
            LoadMeshData(System.Sample, objText);
            Build(System.Sample);
            UpdateNavMeshFace();
        }

        public void SaveTerrainMesh()
        {
            var terrain = FindObjectOfType<Terrain>();
            var mesh = terrain.terrainData.TerrainToMesh().ExportMesh(vertexCountHorizontal, vertexCountVertical, Normal.CalculateFromMesh); // AddTerrain(terrain);
            ExportMesh("Assets/Terrain.obj", mesh);
        }

        public List<Vector3> GetPath(Vector3 currPosition, Vector3 destination, float agentHeight = 1f, FindPathMode pathMode = FindPathMode.FindPathStraight)
        {
            var paths = new List<Vector3>();
            GetPath(currPosition, destination, paths, agentHeight, pathMode);
            return paths;
        }

        public void GetPath(Vector3 currPosition, Vector3 destination, List<Vector3> paths, float agentHeight = 1f, FindPathMode pathMode = FindPathMode.FindPathStraight)
        {
            System.GetPath(currPosition, destination, paths, agentHeight, pathMode);
        }

        private void OnDrawGizmos()
        {
            if (navMesh == null)
                return;
            if (drawNavmesh)
            {
                Gizmos.color = new Color(0f, 1f, 1f, 0.5f);
                Gizmos.DrawMesh(navMesh);
            }
            if (drawWireNavmesh)
            {
                Gizmos.color = new Color(0f, 0f, 0f, 0.3f);
                Gizmos.DrawWireMesh(navMesh);
            }
        }

        private void OnValidate()
        {
            if (string.IsNullOrEmpty(navMashPath))
            {
                navMashPath = Application.dataPath + "/" + UnityEngine.SceneManagement.SceneManager.GetActiveScene().name + "NavMesh.bin";
            }
        }

        private void OnDestroy()
        {
            System.Free();
        }
    }
}
#endif
using System;
using System.Runtime.InteropServices;

namespace Net.AI.Native
{
	public enum SamplePartitionType
	{
		SAMPLE_PARTITION_WATERSHED,
		SAMPLE_PARTITION_MONOTONE,
		SAMPLE_PARTITION_LAYERS
	};

    public enum dtStraightPathOptions
    {
        DT_STRAIGHTPATH_None_CROSSINGS,
        DT_STRAIGHTPATH_AREA_CROSSINGS,
        DT_STRAIGHTPATH_ALL_CROSSINGS
    }

    [Serializable]
	public struct BuildSettings
	{
		// Cell size in world units
		public float cellSize;
		// Cell height in world units
		public float cellHeight;
		// Agent height in world units
		public float agentHeight;
		// Agent radius in world units
		public float agentRadius;
		// Agent max climb in world units
		public float agentMaxClimb;
		// Agent max slope in degrees
		public float agentMaxSlope;
		// Region minimum size in voxels.
		// regionMinSize = sqrt(regionMinArea)
		public float regionMinSize;
		// Region merge size in voxels.
		// regionMergeSize = sqrt(regionMergeArea)
		public float regionMergeSize;
		// Edge max length in world units
		public float edgeMaxLen;
		// Edge max error in voxels
		public float edgeMaxError;
		public float vertsPerPoly;
		// Detail sample distance in voxels
		public float detailSampleDist;
		// Detail sample max error in voxel heights.
		public float detailSampleMaxError;
		// Partition type, see SamplePartitionType
		public SamplePartitionType partitionType;
		// Bounds of the area to mesh
		public float[] navMeshBMin;
		public float[] navMeshBMax;
		// Size of the tiles in voxels
		public float tileSize;

		public static BuildSettings Default => new BuildSettings()
		{
			cellSize = 0.3f,
			cellHeight = 0.2f,
			agentHeight = 2.0f,
			agentRadius = 0.6f,
			agentMaxClimb = 0.9f,
			agentMaxSlope = 45.0f,
			regionMinSize = 8,
			regionMergeSize = 20,
			edgeMaxLen = 12.0f,
			edgeMaxError = 1.3f,
			vertsPerPoly = 6.0f,
			detailSampleDist = 6.0f,
			detailSampleMaxError = 1.0f,
			partitionType = SamplePartitionType.SAMPLE_PARTITION_WATERSHED,
		};
	}

	public class RecastDll
	{
		/*创建寻路网格实例*/
		[DllImport("RecastDll.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr CreateSoloMesh();

		/*收集构建寻路网格参数*/
		[DllImport("RecastDll.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void CollectSettings(IntPtr sample, BuildSettings settings);

		/*设置构建寻路网格参数*/
		[DllImport("RecastDll.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void SetBuildSettings(IntPtr sample, BuildSettings settings);

		/*加载网格模型.obj*/
		[DllImport("RecastDll.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern bool LoadMeshFile(IntPtr sample, string path);

		/*加载网格模型.obj*/
		[DllImport("RecastDll.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern bool LoadMeshData(IntPtr sample, string meshData);

		/*加载网格数据 unity*/
		[DllImport("RecastDll.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern bool LoadMesh(IntPtr sample, float[] m_verts, int[] m_tris, int m_vertCount, int m_triCount);

		/*加载已经烘焙好的网格文件.bin*/
		[DllImport("RecastDll.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern bool LoadNavMesh(IntPtr sample, string path);

		/*保存已经烘焙好的网格文件.bin*/
		[DllImport("RecastDll.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void SaveNavMesh(IntPtr sample, string path);

		/*构建寻路网格*/
		[DllImport("RecastDll.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern bool Build(IntPtr sample);

		/*查找直线路径*/
		[DllImport("RecastDll.dll", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void FindPathStraight(IntPtr sample, float* m_spos, float* m_epos, float* outPoints, out int outPointCount, dtStraightPathOptions m_straightPathOptions);

		/*查找跟随路径 -- 当出现坡度时不是直线行走, 而是先上坡再下坡*/
		[DllImport("RecastDll.dll", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void FindPathFollow(IntPtr sample, float* m_spos, float* m_epos, float* outPoints, out int outPointCount, dtStraightPathOptions m_straightPathOptions);

		/*释放寻路网格实例*/
		[DllImport("RecastDll.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void FreeSoloMesh(IntPtr sample);

		/*获取绘制烘焙网格面*/
		[DllImport("RecastDll.dll", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void GetDrawNavMesh(IntPtr sample, float* vertsArray, out int vertsCount);

		/*获取绘制烘焙网格顶点长度*/
		[DllImport("RecastDll.dll", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int GetDrawNavMeshCount(IntPtr sample);
	}
}
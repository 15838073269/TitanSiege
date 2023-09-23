using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
#if RECAST_NATIVE
using Net.AI.Native;
using static Net.AI.Native.RecastDll;
#else
using Recast;
using static Recast.RecastGlobal;
#endif

namespace Net.AI
{
    [Serializable]
    public unsafe class NavmeshSystem
    {
#if RECAST_NATIVE
        private IntPtr sample;
#else
        private Sample_SoloMesh sample;
#endif
        public BuildSettings buildSettings = BuildSettings.Default;
        private float* m_Paths; // = new float[2048 * 3];
        private float* m_spos;
        private float* m_epos;
#if RECAST_NATIVE
        public IntPtr Sample => sample;
#else
        public Sample_SoloMesh Sample => sample;
#endif
        public void Init()
        {
            if (sample == default)
            {
                m_Paths = (float*)Marshal.AllocHGlobal(sizeof(float) * (2048 * 3));
                m_spos = (float*)Marshal.AllocHGlobal(sizeof(float) * 3);
                m_epos = (float*)Marshal.AllocHGlobal(sizeof(float) * 3);
                sample = CreateSoloMesh();
            }
            SetBuildSettings(sample, buildSettings);
        }

        public void Init(string navmeshPath)
        {
            Init();
            LoadNavMesh(sample, navmeshPath);
        }

        public List<Vector3> GetPath(Vector3 currPosition, Vector3 destination, float agentHeight = 1f, FindPathMode pathMode = FindPathMode.FindPathStraight, dtStraightPathOptions m_straightPathOptions = dtStraightPathOptions.DT_STRAIGHTPATH_ALL_CROSSINGS)
        {
            var paths = new List<Vector3>();
            GetPath(currPosition, destination, paths, agentHeight, pathMode, m_straightPathOptions);
            return paths;
        }

        public unsafe void GetPath(Vector3 currPosition, Vector3 destination, List<Vector3> paths, float agentHeight = 1f, FindPathMode pathMode = FindPathMode.FindPathStraight, dtStraightPathOptions m_straightPathOptions = dtStraightPathOptions.DT_STRAIGHTPATH_ALL_CROSSINGS)
        {
            m_spos[0] = currPosition.x;
            m_spos[1] = currPosition.y;
            m_spos[2] = currPosition.z;

            m_epos[0] = destination.x;
            m_epos[1] = destination.y;
            m_epos[2] = destination.z;

            paths.Clear();
            int outPointCount;
            if (pathMode == FindPathMode.FindPathStraight)
            {
                FindPathStraight(sample, m_spos, m_epos, m_Paths, out outPointCount, m_straightPathOptions);
            }
            else
            {
                FindPathFollow(sample, m_spos, m_epos, m_Paths, out outPointCount, m_straightPathOptions);
            }
            for (int i = 1; i < outPointCount; i++) //为什么不能要最后一条线? 因为后面一条线偶尔出现y=1的问题, 最后一个不要也不影响
            {
                int v = i * 3;
                paths.Add(new Vector3(m_Paths[v - 3], m_Paths[v - 2] + agentHeight, m_Paths[v - 1])); //a线
                paths.Add(new Vector3(m_Paths[v + 0], m_Paths[v + 1] + agentHeight, m_Paths[v + 2])); //b线
            }
        }

        public void Free()
        {
            if (sample != null)
            {
                FreeSoloMesh(sample);
                if (m_Paths != null)
                {
                    Marshal.FreeHGlobal((IntPtr)m_Paths);
                    m_Paths = null;
                }
                if (m_spos != null)
                {
                    Marshal.FreeHGlobal((IntPtr)m_spos);
                    m_spos = null;
                }
                if (m_epos != null)
                {
                    Marshal.FreeHGlobal((IntPtr)m_epos);
                    m_epos = null;
                }
                sample = default;
            }
        }
    }
}
#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_WSA || UNITY_WEBGL
using UnityEngine;
using Net.AOI;
using Grid = Net.AOI.Grid;

namespace Net.Component
{
    public class AOIManager : SingleCase<AOIManager>
    {
        public GridWorld world = new GridWorld();
        public float xPos = -500f;
        public float zPos = -500f;
        public uint xMax = 50;
        public uint zMax = 50;
        public int width = 20;
        public int height = 20;
        public bool EditInit;
        public bool showText;

        protected override void Awake()
        {
            base.Awake();
            world.Init(xPos, zPos, xMax, zMax, width, height);
        }

        private void Update()
        {
            world.UpdateHandler();
        }

        private void OnDrawGizmos()
        {
            if (EditInit)
            {
                EditInit = false;
                world.Init(xPos, zPos, xMax, zMax, width, height);
            }
            Gizmos.color = Color.cyan;
            for (int i = 0; i < world.grids.Count; i++)
            {
                Draw(world.grids[i]);
            }
        }

        private void Draw(Grid grid)
        {
            var pos = grid.rect.center;
            var size = grid.rect.size;
            if(world.gridType == GridType.Horizontal)
                Gizmos.DrawWireCube(new UnityEngine.Vector3(pos.x, 0f, pos.y), new UnityEngine.Vector3(size.x, 0, size.y));
            else
                Gizmos.DrawWireCube(new UnityEngine.Vector3(pos.x, pos.y, 0f), new UnityEngine.Vector3(size.x, size.y, 0));
#if UNITY_EDITOR
            if (showText) 
            {
                if (world.gridType == GridType.Horizontal)
                    UnityEditor.Handles.Label(new UnityEngine.Vector3(grid.rect.x, 1f, grid.rect.y), grid.rect.position.ToString());
                else
                    UnityEditor.Handles.Label(new UnityEngine.Vector3(grid.rect.x, grid.rect.y, 0f), grid.rect.position.ToString());
            }
#endif
        }
    }
}
#endif
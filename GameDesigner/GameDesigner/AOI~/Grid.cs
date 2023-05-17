using Net.Share;
using Net.System;
using System.Collections.Generic;

namespace Net.AOI
{
    /// <summary>
    /// 格子类
    /// </summary>
    public class Grid
    {
        public Rect rect;
        public List<Grid> grids = new List<Grid>();//九宫格列表
        public FastList<IGridBody> gridBodies = new FastList<IGridBody>();//格子的物体
        /// <summary>
        /// 获取九宫格的所有物体
        /// </summary>
        /// <returns></returns>
        public List<IGridBody> GetGridBodiesAll()
        {
            var gridBodies = new List<IGridBody>();
            foreach (var item in grids)
                gridBodies.AddRange(item.gridBodies);
            return gridBodies;
        }
        public override string ToString()
        {
            return $"{rect}";
        }
    }
}

namespace Net.AOI
{
    /// <summary>
    /// 九宫格格子物体接口
    /// </summary>
    public interface IGridBody
    {
        /// <summary>
        /// 格子的ID
        /// </summary>
        int ID { get; set; }
        /// <summary>
        /// 唯一标识
        /// </summary>
        int Identity { get; set; }
        /// <summary>
        /// 格子的位置
        /// </summary>
        Vector3 Position { get; set; }
        /// <summary>
        /// 当前所在的格子
        /// </summary>
        Grid Grid { get; set; }
        /// <summary>
        /// 主角? true:其他演员进入或退出都会通知自己 false:当自己进入新的格子时, 别人会通知你进来了, 当你退出旧的格子时, 别人也会通知你退出了旧的格子, 而你是无感知的
        /// </summary>
        bool MainRole { get; set; }
        /// <summary>
        /// 当初始化完成后调用
        /// </summary>
        void OnStart();
        /// <summary>
        /// 当更新方法, 可以更新位置=unity的transform.position
        /// </summary>
        void OnBodyUpdate();
        /// <summary>
        /// 当body物体进入感兴趣区域
        /// </summary>
        void OnEnter(IGridBody body);
        /// <summary>
        /// 当body物体退出感兴趣区域
        /// </summary>
        void OnExit(IGridBody body);
    }
}

namespace Net.AOI
{
    /// <summary>
    /// 格子演员接口, 怪物, 角色都属于演员
    /// </summary>
    public interface IGridActor
    {
        /// <summary>
        /// 演员ID 可用于实例化客户端哪个id的预制体
        /// </summary>
        int ActorID { get; set; }
        /// <summary>
        /// 头发
        /// </summary>
        int Hair { get; set; }
        /// <summary>
        /// 头部
        /// </summary>
        int Head { get; set; }
        /// <summary>
        /// 上衣
        /// </summary>
        int Jacket { get; set; }
        /// <summary>
        /// 腰带
        /// </summary>
        int Belt { get; set; }
        /// <summary>
        /// 裤子
        /// </summary>
        int Pants { get; set; }
        /// <summary>
        /// 鞋子
        /// </summary>
        int Shoe { get; set; }
        /// <summary>
        /// 武器
        /// </summary>
        int Weapon { get; set; }
    }
}
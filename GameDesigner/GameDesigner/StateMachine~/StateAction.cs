namespace GameDesigner
{
    /// <summary>
    /// ARPG状态动作
    /// </summary>
	[System.Serializable]
    public sealed class StateAction : StateBase
    {
        /// <summary>
        /// 动画剪辑名称
        /// </summary>
		public string clipName = "";
        /// <summary>
        /// 动画剪辑索引
        /// </summary>
		public int clipIndex = 0;
        /// <summary>
        /// 当前动画时间
        /// </summary>
		public float animTime = 0;
        /// <summary>
        /// 动画结束时间
        /// </summary>
		public float animTimeMax = 100;
        /// <summary>
        /// 动画倒入, 当在一帧内调用了多次Play方法，只会应用前一次调用的动画，最后要播放的动画没有真正被播放出来的问题
        /// </summary>
        public bool rewind;

        /// <summary>
        /// 动作是否完成?, 当动画播放结束后为True, 否则为false
        /// </summary>
        public bool IsComplete => animTime >= animTimeMax - 1;
    }
}
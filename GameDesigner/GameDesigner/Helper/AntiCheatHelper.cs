using System;

namespace Net.Helper
{
    public class AntiCheatHelper
    {
        /// <summary>
        /// 是否激活防作弊检测
        /// </summary>
        public static bool IsActive { get; set; }
        /// <summary>
        /// 当检测到作弊, 参数1: 哪个属性被修改 参数2:原值 参数3:被改的值
        /// </summary>
        public static Action<string, object, object> OnDetected { get; set; }
    }
}
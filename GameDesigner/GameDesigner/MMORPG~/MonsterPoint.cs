#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_WSA || UNITY_WEBGL
namespace Net.MMORPG
{
    using UnityEngine;

    //记录数据给服务器使用
    public class MonsterPoint : MonoBehaviour
    {
        /// <summary>
        /// 怪物信息数据
        /// </summary>
        public MonsterData[] monsters;
    }
}
#endif
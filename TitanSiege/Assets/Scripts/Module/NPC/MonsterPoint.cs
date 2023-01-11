/****************************************************
    文件：MonsterPoint.cs
	作者：昔莘
    邮箱: 304183153@qq.com
    日期：2022/12/28 23:14:26
	功能：Nothing
*****************************************************/

using UnityEngine;
namespace GF.MainGame.Module.NPC {
    public class MonsterPoint : MonoBehaviour {
        [Header("对应SceneManager组件的monsters字段索引")]
        public MonsterData[] monsters;
    }
}
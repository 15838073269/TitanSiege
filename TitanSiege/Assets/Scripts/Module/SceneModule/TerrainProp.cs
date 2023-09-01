/****************************************************
    文件：TerrainProp.cs
	作者：昔莘
    邮箱: 304183153@qq.com
    日期：2023/9/1 23:49:36
	功能：Nothing
*****************************************************/

using GF.MainGame.Module.NPC;
using GF.MainGame.Module;
using GF.MainGame;
using GF.Service;
using UnityEngine;
using UnityEngine.EventSystems;

public class TerrainProp : MonoBehaviour , IPointerDownHandler {
    /// <summary>
    /// 选中事件
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerDown(PointerEventData eventData) {
        AppTools.Send<NPCBase>((int)NpcEvent.CanelSelected,null);
    }
}
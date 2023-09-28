/****************************************************
    文件：ItemModule.cs
	作者：昔莘
    邮箱: 304183153@qq.com
    日期：2023/9/28 12:30:43
	功能：Nothing
*****************************************************/

using UnityEngine;
using GF.Module;
using System.Resources;
using GF.Unity.AB;
using GF.MainGame.Data;
using System.Collections.Generic;
using GF.ConfigTable;

namespace GF.MainGame.Module {
    public class ItemModule : GeneralModule {
        public ItemData m_Data;
        public Dictionary<int,Sprite> m_PinzhiDic;
        public override void Create() {
            base.Create();
            m_Data = ConfigerManager.GetInstance.FindData<ItemData>(CT.TABLE_ITEM);
            m_PinzhiDic = new Dictionary<int, Sprite>();
            m_PinzhiDic[(int)Pinzhi.bai] = Unity.AB.ResourceManager.GetInstance.LoadResource<Sprite>("UIRes/item/bai.PNG", bClear:false);
            m_PinzhiDic[(int)Pinzhi.lv] = Unity.AB.ResourceManager.GetInstance.LoadResource<Sprite>("UIRes/item/lv.PNG", bClear: false);
            m_PinzhiDic[(int)Pinzhi.lan] = Unity.AB.ResourceManager.GetInstance.LoadResource<Sprite>("UIRes/item/lan.PNG", bClear: false);
            m_PinzhiDic[(int)Pinzhi.zi] = Unity.AB.ResourceManager.GetInstance.LoadResource<Sprite>("UIRes/item/zi.PNG", bClear: false);
            m_PinzhiDic[(int)Pinzhi.cheng] = Unity.AB.ResourceManager.GetInstance.LoadResource<Sprite>("UIRes/item/cheng.PNG", bClear: false);
        }
      
        public override void Release() {
            base.Release();
        }
    }
}

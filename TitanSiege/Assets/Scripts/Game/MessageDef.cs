using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace GF.MainGame {
   
    public class UIMsgDef {
        //public static readonly string TEST_MESSAGE = "TEST_MESSAGE";//这是测试消息Key

    }
    public enum BulidEvent {
        SetBigMapMoveObjPos = (int)MDef.BulidModule+1,
        AllBulidDicReset,
    }
    public enum LoginEvent {
        Login = (int)MDef.LoginModule + 1,
        Register,
    }
    public enum NpcEvent {
        ChangeSelected = (int)MDef.NPCModule + 1,
        RotateTo = (int)MDef.NPCModule + 1,
        SetRotationZero,
        //PlayerAniMove,
        Removeplayer,
        AddPlayer,
        CanelSelected,
        FuhuoPlayer,
        //AddMonster,
        //GetMonstersbyScene,
        //RemoveMonsterByScene,
        //RemoveMonster,
    }
    public enum CreateAndSelectEvent {
        ShowModel = (int)MDef.CreateAndSelectModule+1,
        ShowWind,
        CreatePlayer,
        Selected,
        DeletePlayer,
        GameStart,
    }
    public enum MoveEvent {
        TEST_MESSAGE = (int)MDef.MoveModule + 1,//这是测试消息Key
        SetMoveObjToScene,
        AddFollow,
        BreakFollow,
        EnterFight,
        SetMoveObjByID,
        OpenHeadCamera,
        StopMove,
        SetCameraToTarget,
        SetCanMove,
        SetSkillMove,
        ShakeCamera,
        IsPlaySkill,
    }
    public enum SceneEvent {
        OpenScene = (int)MDef.SceneModule+1,
        GetSceneDefaultPos,
        AddSceneBase,

    }
    public enum MainUIEvent {
        UpdateHpMp = (int)MDef.MainUIModule + 1,
        ChangeEquItem,
        XiexiaItem,
    }
    public enum SkillEvent {
        ClickSkill = (int)MDef.SkillModule + 1,
        ManzuXuqiudengji,
        CountSkillHurt,
        CountSkillHurtWithOne,
        ShowDamage,
    }
    public enum StateEvent {
        ChangeState = (int)MDef.StateModule + 1,
        ChangeStateWithArgs,
    }
    public enum HPEvent {
        CreateHPUI = (int)MDef.HPModule + 1,
        ShowDamgeTxt,
        ShowHP,
        HideHP,
        UpdateHp,
        UpdateMp,
        CycleObj,
        CycleAllObj,
    }
    public enum DieEvent {
        ShowUI = (int)MDef.DieUIModule + 1,
        HideUI,

    }
    public enum ItemEvent {
        ShowBag = (int)MDef.ItemModule + 1,
        ShowItemDesc,
        ChangeEquItem,
        AddItemUIIfNoneCreateInBag,
        BagIsFull,
        CreateItemUI,
        RecycleItemUI,
        ToDoItemNum,
        AddJinbiOrZuanshi,
        GetItemProp,
        RecycleItemUINoServer,
        GetItemUI,
    }
    public enum TalkEvent {
        AddEmoji = (int)MDef.TalkModule + 1,
        GetTalkUI,
        RecycleTalkUI,
        AddOneTalk,
        TalkToggleChange,
    }
}


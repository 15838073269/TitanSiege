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
        RotateTo = (int)MDef.NPCModule + 1,
        SetRotationZero,
        PlayerAniMove,
        AddMonster,
        AddPlayer,
        RemoveMonsterByScene,
        RemoveMonster,
        Removeplayer,
        GetMonstersbyScene,
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
    }
    public enum MainUIEvent {
        
    }
    public enum SkillEvent {
        ClickSkill = (int)MDef.SkillModule + 1,
        ManzuXuqiudengji,
        CountSkillHurt,
    }
    public enum StateEvent {
        ChangeState = (int)MDef.StateModule + 1,
        ChangeStateWithArgs,
    }
}


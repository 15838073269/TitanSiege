/****************************************************
    文件：MoveModlue.cs
	作者：昔莘
    邮箱: 304183153@qq.com
    日期：2021/11/30 11:7:7
	功能：移动模块
*****************************************************/

using UnityEngine;
using GF.Module;
using GF.Unity.UI;
using DG.Tweening;
using GF.Unity.AB;
using GF.MainGame.UI;
using GF.MainGame.Data;
using GF.ConfigTable;
using GF.MainGame.Module.NPC;
using GF.Service;
using GF.Const;
using Net.Client;
using Net.UnityComponent;
//需要做一个修改，把玩家自己也放到队伍队列中，需要修改队列的
namespace GF.MainGame.Module {
    public class MoveModule : GeneralModule {
        public Transform m_FatherTrans = null;//主控角色的父物体
        private NPCBase m_MoveObj = null;
        private Transform m_CameraTarget = null;
        //private Camera m_HeadCamera = null;
        public NPCBase GetMoveObj() {
            return m_MoveObj;
        }
        private JoyUIWidget m_Joy =null;

        private TeamQueueModule m_TeamQueue = null;
        public float m_Speed = AppConfig.PlayerSpeed;
        public float m_SkillMoveSpeed = 0f;
        public bool m_IsSkill = false;
        private bool m_CanMove = false;
        public void SetCanMove(bool canmove) {
            m_CanMove = canmove;
        }
        public override void Create() {
          
            //初始化移动队列
            if (AppTools.HasModule(MDef.TeamQueueModule)) {
                m_TeamQueue = AppTools.GetModule<TeamQueueModule>(MDef.TeamQueueModule);
            } else {
                m_TeamQueue = AppTools.CreateModule<TeamQueueModule>(MDef.TeamQueueModule);
            }
            GlobalEvent.OnUpdate += MoveUpdate;
            m_Camera = Camera.main.GetComponent<CameraController>();
            
            //注册消息的方法
           
            AppTools.Regist<NPCBase>((int)MoveEvent.AddFollow, AddFollow);
            AppTools.Regist<NPCBase>((int)MoveEvent.BreakFollow, BreakFollow);
            AppTools.Regist((int)MoveEvent.SetMoveObjByID, SetMoveObjByID);
            AppTools.Regist<Transform>((int)MoveEvent.SetMoveObjToScene, SetMoveObjToScene);
            //AppTools.Regist<bool>((int)MoveEvent.OpenHeadCamera, OpenHeadCamera);
            AppTools.Regist((int)MoveEvent.StopMove, StopMove);
            AppTools.Regist((int)MoveEvent.SetCameraToTarget, SetCameraToTarget);
            AppTools.Regist<bool>((int)MoveEvent.SetCanMove, SetCanMove);
            AppTools.Regist<float,bool>((int)MoveEvent.SetSkillMove, SetSkillMove);
            AppTools.Regist<bool>((int)MoveEvent.IsPlaySkill, IsPlaySkill);
            SetMoveObjByID();
        }
       
        public bool IsPlaySkill() {
            return m_IsSkill;
        }
        public void SetMoveObjByID() {
            if (UserService.GetInstance.m_CurrentID==-1) {
                Debuger.Log($"非法登录，请重新登录！");
                return;
            }
            NPCDataBase nb = ConfigerManager.m_NPCData.FindNPCByID((ushort)UserService.GetInstance.m_CurrentID);
            if (m_FatherTrans == null) {
                m_FatherTrans = (new GameObject("PlayerFahter")).transform;
                m_FatherTrans.parent = AppMain.GetInstance.SceneTransform;
            }
            if ( UserService.GetInstance.m_CurrentChar != null) {
                GameObject player = ObjectManager.GetInstance.InstanceObject(nb.Prefabpath, bClear: false, father: m_FatherTrans);
                if (player != null) {
                    //把自己给注册上，方便使用
                    player.transform.forward = Vector3.forward;
                    Player p = player.GetComponent<Player>();
                    if (p == null) {
                        p = player.AddComponent<Player>();
                    }
                    p.m_GDID = ClientBase.Instance.UID;
                    Debug.Log(p.m_GDID);
                    p.m_NpcType = NpcType.player;
                    NetworkObject no = p.GetComponent<NetworkObject>();
                    no.Identity = ClientBase.Instance.UID;
                    UserService.GetInstance.m_CurrentPlayer = p;
                    p.m_PlayerName = UserService.GetInstance.m_CurrentChar.Name;
                    SetMoveObj(player.GetComponent<Player>());
                } else {
                    Debuger.LogError("控制角色加载失败");
                }
            } else {
                Debuger.LogError($"发生错误，非法登录，无法获取角色数据！");
            }
        }

        /// <summary>
        /// 设置主控制移动对象
        /// 这个方法只是初始化角色和位置，位置还放在了0.0.0，只有各场景调用SetMoveObjToScene方法后，才会把角色放到指定位置
        /// </summary>
        /// <param name="moveobj"></param>
        public void SetMoveObj(NPCBase moveobj) {
            if (m_FatherTrans == null) {//这里其实不用加，但考虑到如果玩家主控角色不是默认模型，例如后期元神夺舍其他角色时，且直接读取存档的话，就可能出现m_FatherTrans为空的情况，需要考虑进去
                m_FatherTrans = (new GameObject("PlayerFahter")).transform;
                m_FatherTrans.parent = AppMain.GetInstance.SceneTransform;
            }
            
            m_MoveObj = moveobj;
            if (m_TeamQueue.m_ListQueue.Count > 0) {
                m_TeamQueue.m_ListQueue.RemoveAt(0);
                m_TeamQueue.m_ListQueue.Insert(0, m_MoveObj);
            } else {
                m_TeamQueue.m_ListQueue.Add(m_MoveObj);
            }
            //设置各个物体的位置
            m_MoveObj.transform.localPosition = Vector3.zero;
            m_CanMove = true;
            GameObject go = new GameObject("target");
            go.transform.parent = m_FatherTrans;
            go.transform.position = m_MoveObj.transform.position;
            m_CameraTarget = go.transform;
            //再执行摄像机跟随
            m_Camera.BeginFollow(m_CameraTarget);
            ////设置UIReadering，显示角色模型
            //m_HeadCamera = ObjectManager.GetInstance.InstanceObject("NPCPrefab/headcamera.prefab", bClear:false,father:m_MoveObj.transform).GetComponent<Camera>();
            //m_HeadCamera.transform.localPosition = Vector3.zero;
            //m_HeadCamera.transform.localPosition = m_MoveObj.transform.position + m_MoveObj.transform.forward*6f + new Vector3(0, 0.98f, 0);
            //m_HeadCamera.transform.localEulerAngles = new Vector3(0, 180f + m_MoveObj.transform.localEulerAngles.y, 0);
            //m_HeadCamera.transform.localScale = Vector3.one;
            //m_HeadCamera.gameObject.SetActive(false);
            //最后显示操作ui
            Show();
        }
        /// <summary>
        /// 显示角色信息reader
        /// </summary>
        /// <param name="open"></param>
        //public void OpenHeadCamera(bool open) {
        //    m_HeadCamera.gameObject.SetActive(open);
        //}
        
        public override void Show() {
            UIPanel p = UIManager.GetInstance.GetUI(AppConfig.MainUIPage);
            m_Joy = AppMain.GetInstance.uiroot.FindUIInChild<JoyUIWidget>(p.transform, AppConfig.JoyUIWidget);
        }
        /// <summary>
        /// 行走
        /// </summary>
        public void MoveUpdate() {
            if (m_CanMove && m_Joy != null) {
                float x, y;
                x = m_Joy.input.x;//摇杆的x值
                y = m_Joy.input.y;//摇杆的y值
                if ((x!=0||y!=0)||m_IsSkill) {
                    //计算方面dir
                    if (!m_IsSkill) {
                        SetDir(x, y);
                    }
                    //移动
                    SetMove();
                } else {
                    StopMove();
                    if (m_Camera.IsCanFollow == true) {
                        //停止移动
                        m_Camera.IsCanFollow = false;//相机停止跟随
                    }
                }
                if (m_TeamQueue.m_ListQueue.Count>1) {
                    for (int i = 1; i < m_TeamQueue.m_ListQueue.Count; i++) {//从1开始，因为第0个是主角自己
                        //如果是第一个队友，就以主控角色为跟随目标，如果第2、3、4....个，就以前一个队友为主控目标
                        if (i==1) {
                            TeamFollowMove(m_TeamQueue.m_ListQueue[i]);
                        } else {
                            TeamFollowMove(m_TeamQueue.m_ListQueue[i], m_TeamQueue.m_ListQueue[(i-1)]);
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 根据摇杆计算方向dir
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        private void SetDir(float x,float y) {
            //求出joy旋转角度的正切值x/y,通过反正切求出旋转角度,正切是对边比临边
            float du = Mathf.Atan2(x, y) * Mathf.Rad2Deg;
            //float du = Vector2.SignedAngle(new Vector2(x,y),new Vector2(0,1))+m_CameraTarget.eulerAngles.y; //这种方法也可以获取的旋转角度，（0，1）就是平面ui的正方向，然后求夹角
            //将相机foreword向量按照y轴旋转对应角度后，即可得到目标的移动的方向
            Vector3 dir = Quaternion.AngleAxis(du, Vector3.up) * m_CameraTarget.forward;
            m_MoveObj.transform.rotation = Quaternion.Lerp(m_MoveObj.transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * m_Speed * 1.618f);
        }
        /// <summary>
        /// 移动
        /// </summary>
        /// <param name="movedistance">移动的距离，默认为1，意思是单位距离移动，</param>
        private void SetMove() {
            //控制移动
            //相机准备跟随
            m_Camera.IsCanFollow = true;
            //前面方向已经设置好了，这里就直接向前移动即可
            //+new Vector3(0,-0.1f,0)，是为了给他一个向下的力，模拟重力
            if (m_SkillMoveSpeed != 0f && m_IsSkill) {
                m_MoveObj.m_Character.SimpleMove((m_MoveObj.transform.forward + new Vector3(0, -0.1f, 0)) * m_SkillMoveSpeed);
            } else {
                if (!m_IsSkill) {
                    m_MoveObj.m_Character.SimpleMove((m_MoveObj.transform.forward + new Vector3(0, -0.1f, 0)) * m_Speed);
                }
            }
            m_CameraTarget.position = m_MoveObj.transform.position;
            //把相机目标跟上玩家，这么做就是为了不让相机目标随玩家旋转，让相机目标旋转和相机旋转保持一致
            //if ((m_MoveObj.CurrentState == AniState.none||m_MoveObj.CurrentState == AniState.wait || m_MoveObj.CurrentState == AniState.idle)&&(m_IsSkill==false)) {
            //    if (m_MoveObj.IsFight) {
            //        AppTools.Send<NPCBase, AniState>((int)StateEvent.ChangeState, m_MoveObj, AniState.fightrun);
            //    } else {
            //        AppTools.Send<NPCBase, AniState>((int)StateEvent.ChangeState, m_MoveObj, AniState.run);
            //    }
            //}
            if ((m_MoveObj.m_State.stateMachine.currState.ID == m_MoveObj.m_AllStateID["fightidle"] || m_MoveObj.m_State.stateMachine.currState.ID == m_MoveObj.m_AllStateID["idle"]) &&(m_IsSkill==false)) {
                if (m_MoveObj.IsFight) {
                    m_MoveObj.m_State.StatusEntry(m_MoveObj.m_AllStateID["fightrun"]);
                } else {
                    m_MoveObj.m_State.StatusEntry(m_MoveObj.m_AllStateID["run"]);
                }
            }
        }
        /// <summary>
        /// 设置技能移动
        /// </summary>
        private void SetSkillMove(float movespeed,bool ismove) {
            m_IsSkill = ismove;
            m_SkillMoveSpeed = movespeed;
            m_Joy.m_IsControl = !ismove;
        }
        /// <summary>
        /// 停止移动，有些时候触发事件后，需要马上停止移动，就调用这个
        /// </summary>
        public void StopMove() {
            if (m_Joy.input != Vector2.zero) {
                m_Joy.input  = Vector2.zero;
            }
            //if (m_MoveObj.CurrentState == AniState.run || m_MoveObj.CurrentState == AniState.fightrun) {
            //    if (m_MoveObj.IsFight) {
            //        AppTools.Send<NPCBase, AniState>((int)StateEvent.ChangeState, m_MoveObj, AniState.wait);
            //    } else {
            //        AppTools.Send<NPCBase, AniState>((int)StateEvent.ChangeState, m_MoveObj, AniState.idle);
            //    }
            //}
            if (m_MoveObj.m_State.stateMachine.currState.ID == m_MoveObj.m_AllStateID["run"] || m_MoveObj.m_State.stateMachine.currState.ID == m_MoveObj.m_AllStateID["fightrun"]) {
                if (m_MoveObj.IsFight) {
                    m_MoveObj.m_State.StatusEntry(m_MoveObj.m_AllStateID["fightidle"]);
                } else {
                    m_MoveObj.m_State.StatusEntry(m_MoveObj.m_AllStateID["idle"]);
                }
            }
        }
        /// <summary>
        /// 处理team队员的移动
        /// </summary>
        /// <param name="npc1">跟随角色</param>
        /// <param name="npc2">被跟随角色</param>
        private void TeamFollowMove(NPCBase npc1,NPCBase npc2 = null) {
            if (npc2 ==null) {
                npc2 = m_MoveObj;
            }
            Vector3 dir = npc2.transform.position - npc1.transform.position;
            if (dir.magnitude>2f) {
                //控制移动
                //npc1.AniMove(m_Speed - 0.5f);////改成npcbase根据位置自行掉用动画
                npc1.transform.rotation = npc2.transform.rotation;
                npc1.transform.Translate(dir.normalized * Time.deltaTime*(m_Speed-0.5f),Space.World);
            } else {
                //停止移动
                //float newspeed = Mathf.Lerp(npc1.m_ani.GetFloat("speed"), 0, 0.8f);//不要猛的停止，给个缓冲时间
                //npc1.m_ani.SetFloat("run", 0f);////改成npcbase根据位置自行掉用动画
            }
        }
        /// <summary>
        /// 开始跟随，调整位置
        /// 游戏只能NPC跟随主角
        /// </summary>
        /// <param name="follow">跟随角色</param>
        private Tween movetw;
        private NPCBase m_Follow;
        private NPCBase Befollow;
        public void AddFollow(NPCBase follow) {
            if (m_TeamQueue.IsContainsNPC(follow)) {
                Debuger.Log($"已存在此角色{follow.name}，不能重复添加到队伍！");
                return;
            }
            int count = m_TeamQueue.m_ListQueue.Count;
            if (count > 0) {
                Befollow = m_TeamQueue.m_ListQueue[(count - 1)];
            } else {
                Befollow = m_MoveObj;
            }
            Vector3 dir = Befollow.transform.position - follow.transform.position;
            float distance = dir.magnitude;
            follow.m_Character.enabled = false;
            follow.transform.LookAt(m_MoveObj.transform);
            if (distance > 2f) {//如果两个人位置间隔大于2，需要让follow跑过去
                //减去3个forward，相当于跟随角色，也就是主控角色身后-3的位置
                movetw = follow.transform.DOMove(Befollow.transform.position- Befollow.transform.forward- Befollow.transform.forward - Befollow.transform.forward, distance / m_Speed);
                movetw.SetAutoKill(false);
                m_Follow = follow;
                movetw.OnComplete(OnTwComplete);//设置动画回调
            } else {
                //如果位置近，就把follow放到team物体下，并添加到team list中处理
                follow.transform.rotation = Befollow.transform.rotation;
                follow.transform.parent = m_FatherTrans;
                AddMoveDic();
            }
        }
        /// <summary>
        /// doTween动画完成回调
        /// </summary>
        public void OnTwComplete() {
            if (m_Follow!=null) {
                m_Follow.transform.parent = m_FatherTrans;
                m_Follow.transform.rotation = Befollow.transform.rotation;
                Befollow = null;
                AddMoveDic();
                if (movetw!=null) {
                    movetw.Kill();
                    movetw = null; 
                }
            }
        }
        /// <summary>
        /// 往字典添加ani
        /// </summary>
        private void AddMoveDic() {
            m_TeamQueue.m_ListQueue.Add(m_Follow);
            //清空
            m_Follow = null;
        }

        public void BreakFollow(NPCBase follow) {
            if (m_TeamQueue.IsContainsNPC(follow)) {
                follow.m_Character.enabled = true;
                m_TeamQueue.m_ListQueue.Remove(follow);
                //follow.transform.parent = AppTools.SendReturn<Transform>((int)SceneEvent.GetAllNPCFather);
            } else {
                Debuger.Log($"队伍没有{follow.name}这个角色");
            }
        }

        public void SetMoveObjToScene(Transform def) {
            if (!NetWork.ClientSceneManager.Instance.identitys.ContainsKey(m_MoveObj.m_GDID)) {
                NetWork.ClientSceneManager.Instance.identitys.Add(m_MoveObj.m_GDID, m_MoveObj.GetComponent<NetworkObject>());
            }
            m_CameraTarget.position = def.position;
            m_CameraTarget.rotation = def.rotation;
            m_MoveObj.transform.position = def.position;
            m_MoveObj.transform.rotation = def.rotation;
            m_MoveObj.transform.localScale = def.localScale;
            SetCameraToTarget();
        }
        
        /// <summary>
        /// 调整摄像机朝向主控角色
        /// </summary>
         //相机跟随
        private CameraController m_Camera;
        public float camerahigh = 3f;
        public float cameraback = 2f;
        public void SetCameraToTarget() {
            if (m_MoveObj != null) {
                //减去几个forword，相当于在角色身后退后几个单位
                m_Camera.transform.position = m_MoveObj.transform.position + new Vector3(0, camerahigh, 0) - m_MoveObj.transform.forward * cameraback;
                m_Camera.transform.rotation = Quaternion.Euler(m_MoveObj.transform.rotation.eulerAngles + new Vector3(45f, 0, 0));
                m_Camera.IsInBigMap = false;
                m_Speed = 4f;
                m_Camera.BeginFollow(m_CameraTarget);
            }
        }
       
        public override void Release() {
            base.Release();
        }
        public void SetCameraStartRoate() {
            m_Camera.SetStartRoate();
        }
        public void SetCameraRoate(float x) {
            m_Camera.SetCameraRoate(x);
        }
    }
  
}

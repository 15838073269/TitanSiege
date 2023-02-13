/****************************************************
    文件：Monster.cs
	作者：昔莘
    邮箱: 304183153@qq.com
    日期：2022/12/8 11:13:29
	功能：Nothing
*****************************************************/

using Cysharp.Threading.Tasks;
using GF.Const;
using GF.MainGame.Data;
using System;
using UnityEditor;
using UnityEngine;
using Progress = Cysharp.Threading.Tasks.Progress;

namespace GF.MainGame.Module.NPC {
    public class Monster : NPCBase {
        private Vector3 m_Pos;//怪物的原始位置，用于战斗返回
        private Vector3 m_Rota;//怪物的原始旋转，用于战斗返回
        private Vector3 m_Scal;//怪物的原始缩放，用于放大技能后的恢复
        public Material m_Material = null;
        public bool m_SynchSign = false;//默认false，是服务器同步，true为本地同步
        public int m_TargetID = -1;
        public async void OnEnable() {
            if (m_Material == null) {
                return;
            }
            //重置溶解特效
            m_Material.SetFloat("_Cutoff",1);
            //倒放溶解，显示怪物模型
            ShowModel();
        }
        
        /// <summary>
        /// 溶解显示
        /// </summary>
        /// <returns></returns>
        public async UniTask ShowModel() {
            float _cut = 1f;
            while (_cut >= 0) {
                _cut -= 0.1f;
                await UniTask.Delay(TimeSpan.FromSeconds(0.0618));
                m_Material.SetFloat("_Cutoff", _cut);
            }
        }
        /// <summary>
        /// 溶解消失
        /// </summary>
        /// <returns></returns>
        public async UniTask HideModel() {
            float _cut = 0f;
            while (_cut <= 1) {
                _cut += 0.1f;
                await UniTask.Delay(TimeSpan.FromSeconds(0.0618));
                m_Material.SetFloat("_Cutoff", _cut);
            }
        }
        public override void InitNPCAnimaor() {
            //发现一个很奇怪的bug，awake时，获取子物体的某个组件会报一次错误，仅一次，也不影响使用
            //经查，发现加载怪物时，会连续创建双倍数量的怪物，怀疑是加载怪物脚本太慢了导致的重复加载，但地图只显示了一个，并且无任何异常，第一次创建的去哪里了？而且第一次创建结束后，第二次的第一次会报一次错，找不到任何子物体及任何组件，放到onenabled也一样，找不到问题原因
            Transform body = transform.Find("Body");
            if (body == null) {//找不到问题，先强行屏蔽一下吧
                return;
            }
            var render = body.GetComponent<SkinnedMeshRenderer>();
            m_Material = render.material;
            m_Nab = transform.GetComponent<MonsterAnimatorBase>();
            if (m_Nab == null) {
                m_Nab = transform.gameObject.AddComponent<MonsterAnimatorBase>();
                m_Nab.Init();
            }
        }
        public override void LateUpdate() {
            if (!isPlaySkill && (Time.time > time)) {//优化以下，减少移动时频繁变更状态机的情况,其实就是三帧相同才转为idle
                newPosition = transform.position;
                if (newPosition != oldPosition) {
                    if (Vector3.Distance(oldPosition, newPosition) > 0.02f) {
                        if (CurrentState != AniState.run) {
                            AppTools.Send<NPCBase, AniState>((int)StateEvent.ChangeState, this, AniState.run);
                        }
                    } else {
                        if (oldoldPosition == newPosition) {
                            if (CurrentState == AniState.run) {
                                AppTools.Send<NPCBase, AniState>((int)StateEvent.ChangeState, this, AniState.idle);
                            }
                        }
                    }
                    
                } else {
                    if (oldoldPosition == newPosition) {
                        if (CurrentState == AniState.run) {
                            AppTools.Send<NPCBase, AniState>((int)StateEvent.ChangeState, this, AniState.idle);
                        }
                    }
                }
                oldoldPosition = oldPosition;
                oldPosition = newPosition;
                time = Time.time + (1f / 50f);
            }
        }

        
    }
}
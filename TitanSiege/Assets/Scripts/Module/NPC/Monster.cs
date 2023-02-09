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
        public Material m_Material;
        public async void OnEnable() {
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
            m_Material = transform.Find("Body").GetComponentInChildren<Renderer>().material;
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
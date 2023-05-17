using Cysharp.Threading.Tasks;
using GF.Const;
using GF;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMonster : MonoBehaviour
{
    public Material m_Material;
    // Start is called before the first frame update
    void Awake()
    {
        m_Material = transform.Find("Body").GetComponentInChildren<Renderer>().material;
    }
    public void OnEnable() {
        //重置溶解特效
        m_Material.SetFloat("_Cutoff", 1);
        tt();
    }
    public async UniTask tt() {
        float _cut = 1f;
        while (_cut>=0) {
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
    //测试血条
    public GF.MainGame.UI.DamageUIWidget dui;

    public void Update()
    {
        //测试
        if (Input.GetKeyDown(KeyCode.V)) {
            dui.SetAndShowDamgeTxt(123, DamageType.baoji);
            dui.SetAndShowDamgeTxt(20, DamageType.none);
            dui.SetAndShowDamgeTxt(20, DamageType.shangbi);
        }
    }
}

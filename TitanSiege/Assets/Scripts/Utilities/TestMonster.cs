using Cysharp.Threading.Tasks;
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
    // Update is called once per frame
    void Update()
    {
        
    }
}

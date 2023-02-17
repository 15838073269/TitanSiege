
using UnityEngine;
using UnityEngine.Pool;
using GF.Module;
using GF.MainGame.UI;
using GF.Unity.UI;
using UnityEngine.SceneManagement;
using GF.Unity.AB;

namespace GF.MainGame.Module {
    public class DamageFontModule : GeneralModule {
        ObjectPool<DamageUIWidget> m_UIPool = new ObjectPool<DamageUIWidget>(CreateDamageUI,maxSize:20);
        public override void Create() {
            base.Create();
            
        }
        /// <summary>
        /// 创建飘字预制体
        /// </summary>
        /// <returns></returns>
        public static DamageUIWidget CreateDamageUI() {
            HpUIPage hpuipage = UIManager.GetInstance.GetUI<HpUIPage>("HpUIPage");
            GameObject go = ObjectManager.GetInstance.InstanceObject(AppConfig.DamageUIPath, setTranform: true, bClear: false, isfullpath: false, father: hpuipage.transform);
            DamageUIWidget dui = go.GetComponent<DamageUIWidget>();
            return dui;
        }
       
        public override void Release() {
            base.Release();
        }

        public override void Show() {
            base.Show();
        }

    }

}

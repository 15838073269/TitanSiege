using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using GF.Module;
using GF.MainGame.UI;
using UnityEngine.Pool;

namespace GF.MainGame.Module {
    public class DamageFontModule : GeneralModule {
        GF.Pool.ClassObjectPool<DamageUIWidget> m_UIPool = new Pool.ClassObjectPool<DamageUIWidget>(40);
        public override void Create() {
            base.Create();

        }

        public override void Release() {
            base.Release();
        }

        public override void Show() {
            base.Show();
        }

    }

}

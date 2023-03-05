using GF.Const;
using GF.Unity.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GF.MainGame.UI {
    public class DamageUIWidget : UIWidget {
        public Text m_CrititalTxt;
        public Text m_DodgeTxt;
        public Text m_DamageTxt;
        public Image m_RedTo;
        public Image m_Red;
        public void OnEnable() {
            m_CrititalTxt.gameObject.SetActive(false);
            m_DodgeTxt.gameObject.SetActive(false);
            m_DamageTxt.gameObject.SetActive(false);
        }
        protected override void OnOpen(object args = null) {
            base.OnOpen(args);
            m_CrititalTxt.gameObject.SetActive(false);
            m_DodgeTxt.gameObject.SetActive(false);
            m_DamageTxt.gameObject.SetActive(false);
        }
        public void SetAndShow(int damage,DamageType damagetype) {
            switch (damagetype) {
                case DamageType.none:
                    m_DamageTxt.text = damage + "";
                    m_DamageTxt.gameObject.SetActive(true);
                    break;
                case DamageType.shangbi:
                    m_DodgeTxt.gameObject.SetActive(true);
                    break;
                case DamageType.baoji:
                    m_CrititalTxt.text = $"±©»÷{damage.ToString()}";
                    m_CrititalTxt.gameObject.SetActive(true);
                    break;
                default:
                    m_DamageTxt.text = damage + "";
                    m_DamageTxt.gameObject.SetActive(true);
                    break;
            }
            
        }
        protected override void OnUpdate() {
            base.OnUpdate();
        }
        protected override void OnClose(bool bClear = false, object arg = null) {
            base.OnClose(bClear, arg);
        }
        public override void Close(bool bClear = false, object arg = null) {
            base.Close(bClear, arg);
        }

        

        

       
    }
}


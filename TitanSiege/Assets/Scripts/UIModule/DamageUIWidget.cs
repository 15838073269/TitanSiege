using GF.Const;
using GF.Unity.UI;
using UnityEngine;
using UnityEngine.UI;

namespace GF.MainGame.UI {
    public class DamageUIWidget : UIWidget {
        public Text m_CrititalTxt;
        public Text m_DodgeTxt;
        public Text m_DamageTxt;
        public Text m_NameTxt;
        public Image m_RedTo;
        public Image m_Red;
       
        public bool m_IsInscreen;
        private Animation m_CrititalAni;
        private Animation m_DodgeAni;
        private Animation m_DamageAni;
        private RectTransform m_Rect;
        private Transform m_HPRoot;
        private RectTransform m_HPParentRect;//父级的rect组件，坐标系转换成Archposition需要使用
        private Camera m_UICamera;

        protected override void OnOpen(object args = null) {
            base.OnOpen(args);

        }
        //初始化
        public void InitPos(Transform hproot) {
            m_IsInscreen = false;
            m_UICamera = UIManager.GetInstance.m_UICamera;
            m_HPRoot = hproot;
            m_HPParentRect = transform.parent.GetComponent<RectTransform>();
            m_Rect = transform.GetComponent<RectTransform>();
            //初始化一下位置
            Vector2 scenepos = Camera.main.WorldToScreenPoint(m_HPRoot.transform.position);//通过摄像机将怪物的血条位置转换成屏幕坐标,世界坐标转屏幕坐标                                                                           //UI屏幕坐标转换为世界坐标
            Vector2 uipos = ScreenPointToUILocalPoint(scenepos);//将屏幕坐标转换成ui的anchoredPosition
            m_Rect.anchoredPosition = uipos;
            m_CrititalAni = m_CrititalTxt.gameObject.GetComponent<Animation>();
            m_DodgeAni = m_DodgeTxt.gameObject.GetComponent<Animation>();
            m_DamageAni = m_DamageTxt.gameObject.GetComponent<Animation>();
        }
        /// <summary>
        /// 设置并显示文字
        /// </summary>
        /// <param name="damage"></param>
        /// <param name="damagetype"></param>
        public void SetAndShowDamgeTxt(int damage,DamageType damagetype) {
            switch (damagetype) {
                case DamageType.none:
                    m_DamageAni.Stop();
                    m_DamageTxt.text = damage + "";
                    m_DamageAni.Play(); //当normalizedTime为零时就可以做到重播的效果
                    break;
                case DamageType.shangbi:
                    m_DodgeAni.Stop();
                    m_DodgeAni.Play();//当normalizedTime为零时就可以做到重播的效果
                    break;
                case DamageType.baoji:
                    m_CrititalAni.Stop();
                    m_CrititalTxt.text = $"暴击{damage.ToString()}";
                    m_CrititalAni.Play();
                    break;
                default:
                    m_DamageAni.Stop();
                    m_DamageTxt.text = damage + "";
                    m_DamageAni.Play();
                    break;
            }
        }
        //隐藏血条前，先把动画清理一下
        public void StopAni() {
            m_DamageAni.Stop();
            m_DodgeAni.Stop();
            m_CrititalAni.Stop();
        }
        //上一次怪物和玩家的距离
        //float lastdis = 0;
       
        protected override void OnUpdate() {
            base.OnUpdate();
            if (m_IsInscreen) {//判断是否在屏幕内
                //血条位置跟随
                Vector3 scenepos = Camera.main.WorldToScreenPoint(m_HPRoot.transform.position);//通过摄像机将怪物的血条位置转换成屏幕坐标,世界坐标转屏幕坐标   
                //float dis = Vector3.Distance(m_HPRoot.transform.position,UserService.GetInstance.m_CurrentPlayer.transform.position);
                //if ((dis > lastdis)&&lastdis!=0 && dis <= 5f) {//远小,距离近时才出发近大远小
                //    if (m_Rect.sizeDelta.magnitude>m_MinScale.magnitude) { //比较模长
                //        m_Rect.sizeDelta -= m_Zengliang;
                //    }
                //} else if (dis < lastdis && dis <= 5f) {//近大，距离近时才出发近大远小
                //    if (m_Rect.sizeDelta.magnitude < m_MaxScale.magnitude) { //比较模长
                //        m_Rect.sizeDelta += m_Zengliang;
                //    }
                //}
                //lastdis = dis;
                Vector2 uipos = ScreenPointToUILocalPoint(scenepos);//将屏幕坐标转换成ui的anchoredPosition
                m_Rect.anchoredPosition = uipos;
            } 
        }
       
        // 屏幕坐标转换为 UGUI RectTransform 的 anchoredPosition
        public Vector2 ScreenPointToUILocalPoint( Vector3 screenPoint) {
            Vector2 localPos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(m_HPParentRect, screenPoint, m_UICamera, out localPos);
            return localPos;
        }
        
        protected override void OnClose(bool bClear = false, object arg = null) {
            base.OnClose(bClear, arg);
        }
        public override void Close(bool bClear = false, object arg = null) {
            base.Close(bClear, arg);
        }

        

        

       
    }
}


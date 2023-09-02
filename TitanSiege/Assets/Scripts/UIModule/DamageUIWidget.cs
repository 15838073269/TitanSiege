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
        private RectTransform m_HPParentRect;//������rect���������ϵת����Archposition��Ҫʹ��
        private Camera m_UICamera;

        protected override void OnOpen(object args = null) {
            base.OnOpen(args);

        }
        //��ʼ��
        public void InitPos(Transform hproot) {
            m_IsInscreen = false;
            m_UICamera = UIManager.GetInstance.m_UICamera;
            m_HPRoot = hproot;
            m_HPParentRect = transform.parent.GetComponent<RectTransform>();
            m_Rect = transform.GetComponent<RectTransform>();
            //��ʼ��һ��λ��
            Vector2 scenepos = Camera.main.WorldToScreenPoint(m_HPRoot.transform.position);//ͨ��������������Ѫ��λ��ת������Ļ����,��������ת��Ļ����                                                                           //UI��Ļ����ת��Ϊ��������
            Vector2 uipos = ScreenPointToUILocalPoint(scenepos);//����Ļ����ת����ui��anchoredPosition
            m_Rect.anchoredPosition = uipos;
            m_CrititalAni = m_CrititalTxt.gameObject.GetComponent<Animation>();
            m_DodgeAni = m_DodgeTxt.gameObject.GetComponent<Animation>();
            m_DamageAni = m_DamageTxt.gameObject.GetComponent<Animation>();
        }
        /// <summary>
        /// ���ò���ʾ����
        /// </summary>
        /// <param name="damage"></param>
        /// <param name="damagetype"></param>
        public void SetAndShowDamgeTxt(int damage,DamageType damagetype) {
            switch (damagetype) {
                case DamageType.none:
                    m_DamageAni.Stop();
                    m_DamageTxt.text = damage + "";
                    m_DamageAni.Play(); //��normalizedTimeΪ��ʱ�Ϳ��������ز���Ч��
                    break;
                case DamageType.shangbi:
                    m_DodgeAni.Stop();
                    m_DodgeAni.Play();//��normalizedTimeΪ��ʱ�Ϳ��������ز���Ч��
                    break;
                case DamageType.baoji:
                    m_CrititalAni.Stop();
                    m_CrititalTxt.text = $"����{damage.ToString()}";
                    m_CrititalAni.Play();
                    break;
                default:
                    m_DamageAni.Stop();
                    m_DamageTxt.text = damage + "";
                    m_DamageAni.Play();
                    break;
            }
        }
        //����Ѫ��ǰ���ȰѶ�������һ��
        public void StopAni() {
            m_DamageAni.Stop();
            m_DodgeAni.Stop();
            m_CrititalAni.Stop();
        }
        //��һ�ι������ҵľ���
        //float lastdis = 0;
       
        protected override void OnUpdate() {
            base.OnUpdate();
            if (m_IsInscreen) {//�ж��Ƿ�����Ļ��
                //Ѫ��λ�ø���
                Vector3 scenepos = Camera.main.WorldToScreenPoint(m_HPRoot.transform.position);//ͨ��������������Ѫ��λ��ת������Ļ����,��������ת��Ļ����   
                //float dis = Vector3.Distance(m_HPRoot.transform.position,UserService.GetInstance.m_CurrentPlayer.transform.position);
                //if ((dis > lastdis)&&lastdis!=0 && dis <= 5f) {//ԶС,�����ʱ�ų�������ԶС
                //    if (m_Rect.sizeDelta.magnitude>m_MinScale.magnitude) { //�Ƚ�ģ��
                //        m_Rect.sizeDelta -= m_Zengliang;
                //    }
                //} else if (dis < lastdis && dis <= 5f) {//���󣬾����ʱ�ų�������ԶС
                //    if (m_Rect.sizeDelta.magnitude < m_MaxScale.magnitude) { //�Ƚ�ģ��
                //        m_Rect.sizeDelta += m_Zengliang;
                //    }
                //}
                //lastdis = dis;
                Vector2 uipos = ScreenPointToUILocalPoint(scenepos);//����Ļ����ת����ui��anchoredPosition
                m_Rect.anchoredPosition = uipos;
            } 
        }
       
        // ��Ļ����ת��Ϊ UGUI RectTransform �� anchoredPosition
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


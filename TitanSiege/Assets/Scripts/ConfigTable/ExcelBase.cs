/****************************************************
    �ļ���BinarySerialize.cs
	���ߣ���ݷ
    ����: 304183153@qq.com
    ���ڣ�2021/10/27 17:11:37
	���ܣ����ñ�Ļ���,��Ҫע�⣬���ñ��ಢ���Ǵ洢���ݵģ����Ƕ����ݵĲ���,��������ݽṹ��������
*****************************************************/
namespace GF.ConfigTable {
    
    [System.Serializable]
    public abstract class ExcelBase {
        public string m_Name;
#if UNITY_EDITOR
        /// <summary>
        /// �༭�������ɻ�������
        /// </summary>
        public virtual void Construction() { }
#endif
        /// <summary>
        /// ��ʼ��
        /// </summary>
        public virtual void Init() {
            m_Name = this.GetType().Name;
        }
        public abstract void Clear();
    }
}


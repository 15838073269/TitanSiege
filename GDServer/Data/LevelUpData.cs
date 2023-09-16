
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
namespace GDServer {
    //���ݱ���д�����ռ��ڣ�����AppDomain.CurrentDomain.GetAssemblies()�Ҳ���������Ϊ��ֻ�ܻ�ȡ������ʱ���صĳ��򼯣�MainGame.Data ����ʱ������Ҫ����
    //Ŀǰͨ��д�������ռ�ķ�ʽ��ʵ���ˣ�����д�������ռ���
    [System.Serializable]
    public class LevelUpData : ExcelBase {

        /// <summary>
        /// ��ʼ��
        /// </summary>
        public override void Init() {
            base.Init();
            if (AllLevelData == null) {
                AllLevelData = new List<LevelUpDataBase>();
            }
            m_AllLevelDataDic.Clear();
            for (ushort i = 0; i < AllLevelData.Count; i++) {
                if (m_AllLevelDataDic.ContainsKey(AllLevelData[i].ID)) {
                    Debuger.LogError($"id{AllLevelData[i].ID}�Ѵ���");
                } else {
                    m_AllLevelDataDic.Add(AllLevelData[i].ID, AllLevelData[i]);
                }
            }
        }
        /// <summary>
        /// ����id��ȡnpc����
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public LevelUpDataBase FindByID(ushort id) {
            LevelUpDataBase npc;
            m_AllLevelDataDic.TryGetValue(id, out npc);
            return npc;
        }

        public override void Clear() {
            throw new System.NotImplementedException();
        }

        [XmlIgnore]//�����ǩ������Ҫ���л���dic���л��ᱨ��
        public Dictionary<ushort, LevelUpDataBase> m_AllLevelDataDic = new Dictionary<ushort, LevelUpDataBase>();
        /// <summary>
        /// ���ﲻҪnew���������л�ʱ�����ܻᵼ�����ݵ���
        /// AllNPCֻ�����ڷ����л���ֵ
        /// </summary>
        [XmlElement("AllLevelData")]
        public List<LevelUpDataBase> AllLevelData { get; set; }
    }
    /// <summary>
    /// NPC�����ݽṹ��
    /// </summary>
    [System.Serializable]
    public class LevelUpDataBase {
        //iD
        [XmlAttribute("ID")]
        public ushort ID { get; set; }
        [XmlAttribute("��ֵ����")]
        public string LevelName { get; set; }

        [XmlAttribute("����")]
        public ushort Shengming { get; set; }
        [XmlAttribute("����")]
        public ushort Fali { get; set; }
       
        [XmlAttribute("��������")]//����ֵ������˵�����UpExp��100��5��ʵ������������� 100*(5*5),6���� 100*(6*6)
        public int UpExp { get; set; }

        [XmlAttribute("����")]//�Ӳ����������ֵ
        public ushort Tizhi { get; set; }
        [XmlAttribute("����")]//�Ӳ��⹦�˺����⹦����
        public ushort Liliang { get; set; }
        [XmlAttribute("����")]//��ǿ�������ʣ���ǿ��������Ч���ĳ��ּ��ʣ����繦����������������������������ä��������������
        public ushort Minjie { get; set; }
        [XmlAttribute("ħ��")]//�Ӳ����ܺ����У�ս���ƶ���Χ���Ӳ�ս�����ٶ�
        public ushort Moli { get; set; }

        [XmlAttribute("����")]//�Ӳ㷨�����˺����������˺�����ֵ
        public ushort Meili { get; set; }
        [XmlAttribute("����")]
        public ushort Xingyun { get; set; }

        [XmlAttribute("����")]
        public ushort Lianjin { get; set; }
        [XmlAttribute("����")]
        public ushort Duanzao { get; set; }
        
    }
}

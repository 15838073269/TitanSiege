using Net.System;

namespace GDServer.DB
{
    public class DBModel
    {
        public Dictionary<long, CharactersData> m_CharactersData;
        public Dictionary<long, UsersData> m_Users;
        public DBModel()
        {
            m_CharactersData = new Dictionary<long, CharactersData>();
            m_Users = new Dictionary<long, UsersData>();
            TitansiegeDB.I.Init((list) =>
            {
                foreach (var item in list)
                {
                    if (item is CharactersData data1)
                    {
                        m_CharactersData.Add(data1.ID, data1);
                    }
                    if (item is UsersData data2)
                    {
                        m_Users.Add(data2.ID, data2);
                    }
                }
            });
            Debuger.Log($"数据加载完毕,共加载数据{m_CharactersData.Count + m_Users.Count}条");
            //mysql数据检查更新
            ThreadManager.Invoke("MySqlExecuted", 1f, TitansiegeDB.I.Executed, true);

            //var student = new CharactersData();//增  如果参数直接填写则不需要NewTableRow
            //student.ID = 1;//字段太多可以这样设置
            //student.Name = "fasdsad";

            //student.NewTableRow();//要调用这个才能同步到mysql

            //CharactersData.Add(student.Name, student);

            ////var student1 = studentDatas["张三"];//查

            ////student1.Exp = 4565;//改

            //student1.Delete();//删
        }

    }
}

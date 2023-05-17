#### 使用


```
class Program
{
    static void Main(string[] args)
    {
        NDebug.BindLogAll(Console.WriteLine);

        var studentDatas = new Dictionary<string, StudentData>();
        TestDB.I.Init((list)=> 
        {
            foreach (var item in list)
            {
                if (item is StudentData data1) 
                {
                    studentDatas.Add(data1.Name, data1);
                }
            }
        });
        //mysql线程中心
        ThreadManager.Invoke("MySqlExecutedContext", TestDB.I.ExecutedContext, true);
        //mysql数据检查更新
        ThreadManager.Invoke("MySqlExecuted", 1f, TestDB.I.Executed, true);

        var student = new StudentData(1, 456, "张三");//增

        studentDatas.Add(student.Name, student);

        var student1 = studentDatas["张三"];//查

        student1.Schoolid = 4565;//改

        student1.Delete();//删

        Console.ReadLine();
    }
}
```
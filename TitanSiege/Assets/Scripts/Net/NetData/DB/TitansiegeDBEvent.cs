namespace Titansiege
{
    public class TitansiegeDBEvent
    {
        private static Net.Client.ClientBase client;
        /// <summary>
        /// 设置同步到服务器的客户端对象, 如果不设置, 则默认是ClientBase.Instance对象
        /// </summary>
        public static Net.Client.ClientBase Client
        {
            get
            {
                if (client == null)
                    client = Net.Client.ClientBase.Instance;
                return client;
            }
            set => client = value;
        }
    }
}
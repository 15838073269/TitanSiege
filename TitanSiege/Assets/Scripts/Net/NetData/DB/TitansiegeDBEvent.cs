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

        /// <summary>
        /// 当服务器属性同步给客户端, 如果需要同步属性到客户端, 需要监听此事件, 并且调用发送给客户端
        /// 参数1: 要发送给哪个客户端
        /// 参数2: cmd
        /// 参数3: methodHash
        /// 参数4: pars
        /// </summary>
        public static System.Action<Net.Server.NetPlayer, byte, ushort, object[]> OnSyncProperty;

        /// <summary>BagitemData类对象属性同步id索引</summary>
		public static int BagitemData_SyncID = 0;
		/// <summary>CharactersData类对象属性同步id索引</summary>
		public static int CharactersData_SyncID = 0;
		/// <summary>ConfigData类对象属性同步id索引</summary>
		public static int ConfigData_SyncID = 0;
		/// <summary>NpcsData类对象属性同步id索引</summary>
		public static int NpcsData_SyncID = 0;
		/// <summary>UsersData类对象属性同步id索引</summary>
		public static int UsersData_SyncID = 0;
		
    }
}
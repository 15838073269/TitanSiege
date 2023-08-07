namespace Net.Share
{
    /// <summary>
    /// 远程过程调用模型,此类负责网络通讯中数据解析临时存储的对象
    /// </summary>
    public struct RPCModel
    {
        /// <summary>
        /// 内核? true:数据经过框架内部序列化 false:数据由开发者自己处理
        /// </summary>
        public bool kernel;
        /// <summary>
        /// 网络指令
        /// </summary>
        public byte cmd;
        /// <summary>
        /// 这是内存池数据，这个字段要配合index，count两字段使用，如果想得到实际数据，请使用Buffer属性
        /// </summary>
        public byte[] buffer;
        public int index, count;
        /// <summary>
        /// 数据缓存器(正确的数据段)
        /// </summary>
        public byte[] Buffer
        {
            get
            {
                if (isFill)
                    return buffer;
                if (count == 0)
                    return new byte[0];//byte[]不能为空,否则出错
                var array = new byte[count];
                global::System.Buffer.BlockCopy(buffer, index, array, 0, count);
                return array;
            }
            set
            {
                buffer = value;
                count = value.Length;
            }
        }
        /// <summary>
        /// 远程函数名
        /// </summary>
        public string func;
        /// <summary>
        /// 远程方法哈希值
        /// </summary>
        public ushort methodHash;
        /// <summary>
        /// 远程参数
        /// </summary>
        public object[] pars;
        /// <summary>
        /// 数据是否经过内部序列化?
        /// </summary>
        public bool serialize;
        /// <summary>
        /// 标记此数据为大数据
        /// </summary>
        public bool bigData;
        /// <summary>
        /// 参数To或As调用一次+1
        /// </summary>
        private int parsIndex;
        /// <summary>
        /// 当数据已经填充, 获取Buffer可直接返回真正数据
        /// </summary>
        private bool isFill;

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="buffer"></param>
        public RPCModel(byte cmd, byte[] buffer) : this()
        {
            this.cmd = cmd;
            this.buffer = buffer;
            count = buffer.Length;
        }

        /// <summary>
        /// 构造Send
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="func"></param>
        /// <param name="pars"></param>
        public RPCModel(byte cmd, string func, object[] pars) : this()
        {
            kernel = true;
            serialize = true;
            this.cmd = cmd;
            this.func = func;
            this.pars = pars;
        }

        public RPCModel(byte cmd, ushort methodHash, object[] pars) : this()
        {
            kernel = true;
            serialize = true;
            this.cmd = cmd;
            this.methodHash = methodHash;
            this.pars = pars;
        }

        /// <summary>
        /// 构造Send
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="buffer"></param>
        /// <param name="kernel"></param>
        public RPCModel(byte cmd, byte[] buffer, bool kernel) : this()
        {
            this.cmd = cmd;
            this.buffer = buffer;
            this.kernel = kernel;
            count = buffer.Length;
        }

        public RPCModel(byte cmd, bool kernel, byte[] buffer, int index, int size) : this()
        {
            this.cmd = cmd;
            this.buffer = buffer;
            this.index = index;
            this.count = size;
            this.kernel = kernel;
        }

        /// <summary>
        /// 构造SendRT可靠传输
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="buffer"></param>
        /// <param name="kernel"></param>
        /// <param name="serialize"></param>
        public RPCModel(byte cmd, byte[] buffer, bool kernel, bool serialize) : this()
        {
            this.cmd = cmd;
            this.buffer = buffer;
            this.kernel = kernel;
            this.serialize = serialize;
            count = buffer.Length;
        }

        public RPCModel(byte cmd, byte[] buffer, bool kernel, bool serialize, ushort methodHash) : this()
        {
            this.cmd = cmd;
            this.buffer = buffer;
            this.kernel = kernel;
            this.serialize = serialize;
            count = buffer.Length;
            this.methodHash = methodHash; 
        }

        /// <summary>
        /// 构造SendRT可靠传输
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="func"></param>
        /// <param name="pars"></param>
        /// <param name="kernel"></param>
        /// <param name="serialize"></param>
        public RPCModel(byte cmd, string func, object[] pars, bool kernel, bool serialize) : this()
        {
            this.cmd = cmd;
            this.func = func;
            this.pars = pars;
            this.kernel = kernel;
            this.serialize = serialize;
        }

        public RPCModel(byte cmd, string func, object[] pars, bool kernel, bool serialize, ushort methodHash) : this()
        {
            this.cmd = cmd;
            this.func = func;
            this.pars = pars;
            this.kernel = kernel;
            this.serialize = serialize;
            this.methodHash = methodHash; 
        }

        /// <summary>
        /// 每次调用参数都会指向下一个参数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T To<T>()
        {
            var t = (T)pars[parsIndex];
            parsIndex++;
            return t;
        }

        /// <summary>
        /// 每次调用参数都会指向下一个参数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T As<T>() where T : class
        {
            var t = pars[parsIndex] as T;
            parsIndex++;
            return t;
        }

        public byte AsByte { get => To<byte>(); }
        public sbyte AsSbyte { get => To<sbyte>(); }
        public bool AsBoolen { get => To<bool>(); }
        public short AsShort { get => To<short>(); }
        public ushort AsUshort { get => To<ushort>(); }
        public char AsChar { get => To<char>(); }
        public int AsInt { get => To<int>(); }
        public uint AsUint { get => To<uint>(); }
        public float AsFloat { get => To<float>(); }
        public long AsLong { get => To<long>(); }
        public ulong AsUlong { get => To<ulong>(); }
        public double AsDouble { get => To<double>(); }
        public string AsString { get => As<string>(); }

        public object Obj
        {
            get
            {
                var obj = pars[parsIndex];
                parsIndex++;
                return obj;
            }
        }

        /// <summary>
        /// 类信息字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var fields = typeof(NetCmd).GetFields(global::System.Reflection.BindingFlags.Static | global::System.Reflection.BindingFlags.Public);
            string cmdStr = "";
            for (int i = 0; i < fields.Length; i++)
            {
                if (cmd.Equals(fields[i].GetValue(null)))
                {
                    cmdStr = fields[i].Name;
                    break;
                }
            }
            return $"指令:{cmdStr} 内核:{kernel} 方法:{func} 哈希方法:{methodHash} 数据:{(buffer != null ? buffer.Length : 0)}";
        }

        public void Flush()
        {
            buffer = Buffer;
            index = 0;
            count = buffer.Length;
            isFill = true;
        }
    }
}

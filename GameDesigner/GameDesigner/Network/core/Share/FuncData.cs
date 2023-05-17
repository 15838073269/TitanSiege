using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Net.Share
{
    /// <summary>
    /// 函数数据
    /// </summary>
    public struct FuncData
    {
        /// <summary>
        /// 函数名称
        /// </summary>
        public string name;
        /// <summary>
        /// 方法哈希
        /// </summary>
        public ushort hash;
        /// <summary>
        /// 参数数组
        /// </summary>
        public object[] pars;
        /// <summary>
        /// 序列化是否错误?
        /// </summary>
        public bool error;
        public int parsIndex;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="func"></param>
        /// <param name="pars"></param>
        public FuncData(string func, object[] pars)
        {
            error = false;
            name = func;
            hash = 0;
            this.pars = pars;
            parsIndex = 0;
        }

        /// <summary>
        /// 取出index的参数值
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public object this[int index]
        {
            get
            {
                return pars[index];
            }
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

        public override string ToString()
        {
            return $"{name}:{pars}";
        }
    }
}

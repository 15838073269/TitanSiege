namespace Net.Serialize
{
    using global::System;
    using global::System.Collections.Generic;
    using global::System.Reflection;
    using Net.System;
    using Net.Share;
    using Net.Event;
    using Binding;

    /// <summary>
    /// 快速序列化2接口--动态匹配
    /// </summary>
    public interface ISerialize
    {
        /// <summary>
        /// 序列化写入
        /// </summary>
        /// <param name="value"></param>
        /// <param name="stream"></param>
        void WriteValue(object value, Segment stream);
        /// <summary>
        /// 反序列化读取
        /// </summary>
        /// <param name="stream"></param>
        object ReadValue(Segment stream);
    }

    /// <summary>
    /// 快速序列化2接口---类型匹配
    /// </summary>
    public interface ISerialize<T>
    {
        /// <summary>
        /// 序列化写入
        /// </summary>
        /// <param name="value"></param>
        /// <param name="stream"></param>
        void Write(T value, Segment stream);
        /// <summary>
        /// 反序列化读取
        /// </summary>
        /// <param name="stream"></param>
        T Read(Segment stream);
    }

    /// <summary>
    /// 类型绑定查找收集接口
    /// </summary>
    public interface IBindingType
    {
        /// <summary>
        /// 收集序列化类型的顺序 -!!!!- 如果有多个项目继承绑定类型时, 必须设置顺序, 否则会出现, 后端和前端收集的传输类型不一样的问题
        /// </summary>
        int SortingOrder { get; }
        /// <summary>
        /// 收集的绑定类型列表
        /// </summary>
        Dictionary<Type, Type> BindTypes { get; }
    }

    internal class TypeBind
    {
        public object bind;
        public ushort hashCode;
    }

    /// <summary>
    /// 极速序列化2版本
    /// </summary>
    public class NetConvertFast2 : NetConvertBase
    {
        private static readonly MyDictionary<ushort, Type> Types = new MyDictionary<ushort, Type>();
        private static readonly MyDictionary<Type, ushort> Types1 = new MyDictionary<Type, ushort>();
        private static readonly MyDictionary<Type, TypeBind> Types2 = new MyDictionary<Type, TypeBind>();
        private static readonly MyDictionary<Type, Type> BindTypes = new MyDictionary<Type, Type>();

        static NetConvertFast2()
        {
            Init();
        }

        /// <summary>
        /// 初始化网络转换类型
        /// </summary>
        public static bool Init()
        {
            Types.Clear();
            Types1.Clear();
            Types2.Clear();
            BindTypes.Clear();
            AddBaseType();
            InitBindInterfaces();
            return true;
        }

        /// <summary>
        /// 添加网络基本类型， int，float，bool，string......
        /// </summary>
        public static void AddBaseType()
        {
            AddBaseType3<byte>();
            AddBaseType3<sbyte>();
            AddBaseType3<bool>();
            AddBaseType3<short>();
            AddBaseType3<ushort>();
            AddBaseType3<char>();
            AddBaseType3<int>();
            AddBaseType3<uint>();
            AddBaseType3<float>();
            AddBaseType3<long>();
            AddBaseType3<ulong>();
            AddBaseType3<double>();
            AddBaseType3<string>();
            AddBaseType3<DateTime>();
            AddBaseType3<decimal>();
            AddBaseType3<DBNull>();
        }

        /// <summary>
        /// 添加可序列化的参数类型, 网络参数类型 如果不进行添加将不会被序列化,反序列化
        /// </summary>
        /// <typeparam name="T">要添加的网络类型</typeparam>
        public static void AddSerializeType<T>()
        {
            AddSerializeType(typeof(T));
        }

        /// <summary>
        /// 添加所有可序列化的类型, 网络参数类型 如果不进行添加将不会被序列化,反序列化
        /// </summary>
        /// <param name="types"></param>
        public static void AddSerializeType3s(Type[] types)
        {
            foreach (var type in types)
            {
                if (type.IsGenericType)
                    AddSerializeType(type);
                else
                    AddSerializeType3(type);
            }
        }

        /// <summary>
        /// 添加可序列化的3个参数类型(T类,T类数组,T类List泛型), 网络参数类型 如果不进行添加将不会被序列化,反序列化
        /// </summary>
        /// <typeparam name="T">要添加的网络类型</typeparam>
        public static void AddSerializeType3<T>()
        {
            AddSerializeType(typeof(T));
            AddSerializeType(typeof(T[]));
            AddSerializeType(typeof(List<T>));
        }

        /// <summary>
        /// 添加可序列化的3个参数类型(T类,T类数组,T类List泛型), 网络参数类型 如果不进行添加将不会被序列化,反序列化
        /// </summary>
        public static void AddSerializeType3(Type type)
        {
            AddSerializeType(type);
            AddSerializeType(Array.CreateInstance(type, 0).GetType());
            AddSerializeType(typeof(List<>).MakeGenericType(type));
        }

        /// <summary>
        /// 添加可序列化的参数类型, 网络参数类型 如果不进行添加将不会被序列化,反序列化
        /// </summary>
        /// <param name="type">要添加的网络类型</param>
        public static void AddSerializeType(Type type)
        {
            if (Types2.ContainsKey(type))
                throw new Exception($"已经添加{type}键，不需要添加了!");
            if (!BindTypes.TryGetValue(type, out Type bindType))
                throw new Exception($"类型{type}尚未实现绑定类型,请使用工具生成绑定类型!");
            ushort hashType = (ushort)Types.Count;
            Types.Add(hashType, type);
            Types1.Add(type, hashType);
            Types2.Add(type, new TypeBind() { bind = Activator.CreateInstance(bindType), hashCode = hashType } );
        }

        private static void AddBaseType3<T>()
        {
            AddBaseType<T>();
            AddBaseArrayType<T>();
            AddBaseListType<T>();
        }

        private static void AddBaseType<T>()
        {
            var type = typeof(T);
            if (Types2.ContainsKey(type))
                return;
            ushort hashType = (ushort)Types.Count;
            Types.Add(hashType, type);
            Types1.Add(type, hashType);
            Types2.Add(type, new TypeBind() { bind = Activator.CreateInstance(typeof(BaseBind<T>)), hashCode = hashType });
        }

        private static void AddBaseArrayType<T>()
        {
            var type = typeof(T[]);
            if (Types2.ContainsKey(type))
                return;
            ushort hashType = (ushort)Types.Count;
            Types.Add(hashType, type);
            Types1.Add(type, hashType);
            Types2.Add(type, new TypeBind() { bind = Activator.CreateInstance(typeof(BaseArrayBind<T>)), hashCode = hashType });
        }

        private static void AddBaseListType<T>()
        {
            var type = typeof(List<T>);
            if (Types2.ContainsKey(type))
                return;
            ushort hashType = (ushort)Types.Count;
            Types.Add(hashType, type);
            Types1.Add(type, hashType);
            Types2.Add(type, new TypeBind() { bind = Activator.CreateInstance(typeof(BaseListBind<T>)), hashCode = hashType });
        }

        public static void InitBindInterfaces()
        { 
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var bindTypes = new List<IBindingType>();
            foreach (Assembly assembly in assemblies)
            {
                var type = assembly.GetType("Binding.BindingType");
                if (type != null)
                {
                    var bindObj = (IBindingType)Activator.CreateInstance(type);
                    bindTypes.Add(bindObj);
                }
            }
            bindTypes.Sort((a, b)=> a.SortingOrder.CompareTo(b.SortingOrder));
            foreach (var bindObj in bindTypes)
            {
                foreach (var bindType in bindObj.BindTypes)
                {
                    BindTypes.Add(bindType.Key, bindType.Value);
                    AddSerializeType(bindType.Key);
                }
            }
        }

        /// <summary>
        /// 添加可序列化的参数类型, 网络参数类型 如果不进行添加将不会被序列化,反序列化
        /// </summary>
        /// <param name="types"></param>
        public static void AddSerializeType(params Type[] types)
        {
            foreach (Type type in types)
            {
                AddSerializeType(type);
            }
        }

        public static Segment SerializeObject<T>(T value)
        {
            var stream = BufferPool.Take();
            try
            {
                Type type = value.GetType();
                if(Types2.TryGetValue(type, out TypeBind typeBind))
                {
                    var bind = (ISerialize<T>)typeBind.bind;
                    bind.Write(value, stream);
                }
                else if (type.IsEnum)
                {
                    var bind = new BaseBind<int>();
                    bind.Write(value.GetHashCode(), stream);
                }
                else throw new Exception($"请注册或绑定:{type}类型后才能序列化!");
            }
            catch (Exception ex)
            {
                stream.Position = 0;
                NDebug.LogError("序列化:" + value + "出错 详细信息:" + ex);
            }
            finally
            {
                stream.Count = stream.Position;
                stream.Position = 0;
            }
            return stream;
        }

        public static void SerializeObject<T>(T value, Segment stream)
        {
            try
            {
                Type type = value.GetType();
                if (Types2.TryGetValue(type, out TypeBind typeBind))
                {
                    var bind = (ISerialize<T>)typeBind.bind;
                    bind.Write(value, stream);
                }
                else if (type.IsEnum)
                {
                    var bind = new BaseBind<int>();
                    bind.Write(value.GetHashCode(), stream);
                }
                else throw new Exception($"请注册或绑定:{type}类型后才能序列化!");
            }
            catch (Exception ex)
            {
                stream.Position = 0;
                NDebug.LogError("序列化:" + value + "出错 详细信息:" + ex);
            }
        }

        public static Segment SerializeObject(object value)
        {
            var stream = BufferPool.Take();
            try
            {
                Type type = value.GetType();
                if (Types2.TryGetValue(type, out TypeBind typeBind))
                {
                    var bind = (ISerialize)typeBind.bind;
                    bind.WriteValue(value, stream);
                }
                else if (type.IsEnum) 
                {
                    var bind = new BaseBind<int>();
                    bind.Write(value.GetHashCode(), stream);
                }
                else throw new Exception($"请注册或绑定:{type}类型后才能序列化!");
            }
            catch (Exception ex)
            {
                stream.Position = 0;
                NDebug.LogError("序列化:" + value + "出错 详细信息:" + ex);
            }
            finally
            {
                stream.Count = stream.Position;
                stream.Position = 0;
            }
            return stream;
        }

        public static T DeserializeObject<T>(Segment segment, bool isPush = true)
        {
            Type type = typeof(T);
            if (Types2.TryGetValue(type, out TypeBind typeBind)) 
            {
                var bind = (ISerialize<T>)typeBind.bind;
                T value = bind.Read(segment);
                if (isPush) BufferPool.Push(segment);
                return value;
            }
            else if (type.IsEnum)
            {
                var bind = new BaseBind<int>();
                T value = (T)(object)bind.Read(segment);
                if (isPush) BufferPool.Push(segment);
                return value;
            }
            else throw new Exception($"请注册或绑定:{type}类型后才能反序列化!");
        }

        public static object DeserializeObject(Type type, Segment segment, bool isPush = true)
        {
            if (Types2.TryGetValue(type, out TypeBind typeBind))
            {
                var bind = (ISerialize)typeBind.bind;
                object value = bind.ReadValue(segment);
                if(isPush) BufferPool.Push(segment);
                return value;
            }
            else if (type.IsEnum)
            {
                var bind = new BaseBind<int>();
                object value = bind.Read(segment);
                if (isPush) BufferPool.Push(segment);
                return value;
            }
            throw new Exception($"请注册或绑定:{type}类型后才能反序列化!");
        }

        /// <summary>
        /// 索引取类型
        /// </summary>
        /// <param name="typeIndex"></param>
        /// <returns></returns>
        private static Type IndexToType(ushort typeIndex)
        {
            if (Types.TryGetValue(typeIndex, out Type type))
                return type;
            return null;
        }

        /// <summary>
        /// 类型取索引
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static ushort TypeToIndex(Type type)
        {
            if (Types1.TryGetValue(type, out ushort typeHash))
                return typeHash;
            throw new KeyNotFoundException($"没有注册[{type}]类为序列化对象, 请使用序列化生成工具生成{type}绑定类! (如果是基类,请联系作者修复!谢谢)");
        }

        public static byte[] SerializeModel(RPCModel model)
        {
            var stream = BufferPool.Take();
            byte[] buffer1 = new byte[0];
            try
            {
                byte head = 0;
                bool hasFunc = !string.IsNullOrEmpty(model.func);
                bool hasMask = model.methodHash != 0;
                SetBit(ref head, 1, hasFunc);
                SetBit(ref head, 2, hasMask);
                stream.WriteByte(head);
                if (hasFunc) stream.Write(model.func);
                if (hasMask) stream.Write(model.methodHash);
                foreach (var obj in model.pars)
                {
                    Type type;
                    if (obj == null)
                    {
                        type = typeof(DBNull);
                        stream.Write(TypeToIndex(type));
                        continue;
                    }
                    type = obj.GetType();
                    stream.Write(TypeToIndex(type));
                    if (Types2.TryGetValue(type, out TypeBind typeBind))
                    {
                        var bind = (ISerialize)typeBind.bind;
                        bind.WriteValue(obj, stream);
                    }
                    else if (type.IsEnum)
                    {
                        var bind = new BaseBind<int>();
                        bind.Write(obj.GetHashCode(), stream);
                    }
                    else throw new Exception($"请注册或绑定:{type}类型后才能序列化!");
                }
                buffer1 = stream.ToArray(true);
            }
            catch (Exception ex)
            {
                NDebug.LogError($"序列化func:[{model.func}]hash:[{model.methodHash}]出错 详细信息:" + ex);
            }
            return buffer1;
        }

        public static FuncData DeserializeModel(Segment segment, bool isPush = true)
        {
            FuncData obj = default;
            try
            {
                byte head = segment.ReadByte();
                bool hasFunc = GetBit(head, 1);
                bool hasMask = GetBit(head, 2);
                if (hasFunc) obj.name = segment.ReadString();
                if (hasMask) obj.hash = segment.ReadUInt16();
                var list = new List<object>();
                int count = segment.Offset + segment.Count;
                while (segment.Position < count)
                {
                    ushort typeIndex = segment.ReadUInt16();
                    var type = IndexToType(typeIndex);
                    if (type == null)
                    {
                        obj.error = true;
                        break;
                    }
                    if (type == typeof(DBNull))
                    {
                        list.Add(null);
                        continue;
                    }
                    var obj1 = DeserializeObject(type, segment, false);
                    list.Add(obj1);
                }
                obj.pars = list.ToArray();
                if(isPush)
                    BufferPool.Push(segment);
            }
            catch (Exception ex)
            {
                obj.error = true;
                NDebug.LogError($"解析func:[{obj.name}]hash:[{obj.hash}]出错 详细信息:" + ex);
            }
            return obj;
        }
    }
}
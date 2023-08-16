using Net.System;
using Net.Serialize;
using System;

namespace Binding
{
    /// <summary>
    /// 基础类型绑定
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public readonly struct BaseBind<T> : ISerialize<T>, ISerialize
    {
        public void Write(T value, ISegment stream)
        {
            stream.WriteValue(value);
        }
        public T Read(ISegment stream)
        {
            return stream.ReadValue<T>();
        }

        public void WriteValue(object value, ISegment stream)
        {
            stream.WriteValue(value);
        }

        object ISerialize.ReadValue(ISegment stream)
        {
            return stream.ReadValue<T>();
        }
    }

    /// <summary>
    /// 基础类型数组绑定
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public readonly struct BaseArrayBind<T> : ISerialize<T[]>, ISerialize
    {
        public void Write(T[] value, ISegment stream)
        {
            stream.WriteArray(value);
        }
        public T[] Read(ISegment stream)
        {
            return stream.ReadArray<T>();
        }

        public void WriteValue(object value, ISegment stream)
        {
            stream.WriteArray(value);
        }

        object ISerialize.ReadValue(ISegment stream)
        {
            return stream.ReadArray<T>();
        }
    }

    /// <summary>
    /// 基础类型泛型绑定
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public readonly struct BaseListBind<T> : ISerialize<System.Collections.Generic.List<T>>, ISerialize
    {
        public void Write(System.Collections.Generic.List<T> value, ISegment stream)
        {
            stream.WriteList(value);
        }
        public System.Collections.Generic.List<T> Read(ISegment stream)
        {
            return stream.ReadList<T>();
        }

        public void WriteValue(object value, ISegment stream)
        {
            stream.WriteList(value);
        }

        object ISerialize.ReadValue(ISegment stream)
        {
            return stream.ReadList<T>();
        }
    }

    /// <summary>
    /// 字典绑定
    /// </summary>
    public readonly struct DictionaryBind<TKey, TValue>
    {
        public void Write(System.Collections.Generic.Dictionary<TKey, TValue> value, ISegment stream, ISerialize<TValue> bind)
        {
            int count = value.Count;
            stream.Write(count);
            if (count == 0) return;
            foreach (var value1 in value)
            {
                stream.WriteValue(value1.Key);
                bind.Write(value1.Value, stream);
            }
        }

        public System.Collections.Generic.Dictionary<TKey, TValue> Read(ISegment stream, ISerialize<TValue> bind)
        {
            var count = stream.ReadInt32();
            var value = new System.Collections.Generic.Dictionary<TKey, TValue>();
            if (count == 0) return value;
            for (int i = 0; i < count; i++)
            {
                var key = stream.ReadValue<TKey>();
                var tvalue = bind.Read(stream);
                value.Add(key, tvalue);
            }
            return value;
        }
    }

    /// <summary>
    /// My字典绑定
    /// </summary>
    public readonly struct MyDictionaryBind<TKey, TValue>
    {
        public void Write(MyDictionary<TKey, TValue> value, ISegment stream, ISerialize<TValue> bind)
        {
            int count = value.Count;
            stream.Write(count);
            if (count == 0) return;
            foreach (var value1 in value)
            {
                stream.WriteValue(value1.Key);
                bind.Write(value1.Value, stream);
            }
        }

        public MyDictionary<TKey, TValue> Read(ISegment stream, ISerialize<TValue> bind)
        {
            var count = stream.ReadInt32();
            var value = new MyDictionary<TKey, TValue>();
            if (count == 0) return value;
            for (int i = 0; i < count; i++)
            {
                var key = stream.ReadValue<TKey>();
                var tvalue = bind.Read(stream);
                value.Add(key, tvalue);
            }
            return value;
        }
    }

    #region "基元类型绑定"
    public readonly struct SystemByteBind : ISerialize<System.Byte>, ISerialize
    {
        public void Write(System.Byte value, ISegment stream)
        {
            stream.Write(value);
        }
        public System.Byte Read(ISegment stream)
        {
            return stream.ReadByte();
        }

        public void WriteValue(object value, ISegment stream)
        {
            stream.Write((System.Byte)value);
        }

        object ISerialize.ReadValue(ISegment stream)
        {
            return stream.ReadByte();
        }
    }
    public readonly struct SystemSByteBind : ISerialize<System.SByte>, ISerialize
    {
        public void Write(System.SByte value, ISegment stream)
        {
            stream.Write(value);
        }
        public System.SByte Read(ISegment stream)
        {
            return stream.ReadSByte();
        }

        public void WriteValue(object value, ISegment stream)
        {
            stream.Write((System.SByte)value);
        }

        object ISerialize.ReadValue(ISegment stream)
        {
            return stream.ReadSByte();
        }
    }
    public readonly struct SystemBooleanBind : ISerialize<System.Boolean>, ISerialize
    {
        public void Write(System.Boolean value, ISegment stream)
        {
            stream.Write(value);
        }
        public System.Boolean Read(ISegment stream)
        {
            return stream.ReadBoolean();
        }

        public void WriteValue(object value, ISegment stream)
        {
            stream.Write((System.Boolean)value);
        }

        object ISerialize.ReadValue(ISegment stream)
        {
            return stream.ReadBoolean();
        }
    }
    public readonly struct SystemInt16Bind : ISerialize<System.Int16>, ISerialize
    {
        public void Write(System.Int16 value, ISegment stream)
        {
            stream.Write(value);
        }
        public System.Int16 Read(ISegment stream)
        {
            return stream.ReadInt16();
        }

        public void WriteValue(object value, ISegment stream)
        {
            stream.Write((System.Int16)value);
        }

        object ISerialize.ReadValue(ISegment stream)
        {
            return stream.ReadInt16();
        }
    }
    public readonly struct SystemUInt16Bind : ISerialize<System.UInt16>, ISerialize
    {
        public void Write(System.UInt16 value, ISegment stream)
        {
            stream.Write(value);
        }
        public System.UInt16 Read(ISegment stream)
        {
            return stream.ReadUInt16();
        }

        public void WriteValue(object value, ISegment stream)
        {
            stream.Write((System.UInt16)value);
        }

        object ISerialize.ReadValue(ISegment stream)
        {
            return stream.ReadUInt16();
        }
    }
    public readonly struct SystemCharBind : ISerialize<System.Char>, ISerialize
    {
        public void Write(System.Char value, ISegment stream)
        {
            stream.Write(value);
        }
        public System.Char Read(ISegment stream)
        {
            return stream.ReadChar();
        }

        public void WriteValue(object value, ISegment stream)
        {
            stream.Write((System.Char)value);
        }

        object ISerialize.ReadValue(ISegment stream)
        {
            return stream.ReadChar();
        }
    }
    public readonly struct SystemInt32Bind : ISerialize<System.Int32>, ISerialize
    {
        public void Write(System.Int32 value, ISegment stream)
        {
            stream.Write(value);
        }
        public System.Int32 Read(ISegment stream)
        {
            return stream.ReadInt32();
        }

        public void WriteValue(object value, ISegment stream)
        {
            stream.Write((System.Int32)value);
        }

        object ISerialize.ReadValue(ISegment stream)
        {
            return stream.ReadInt32();
        }
    }
    public readonly struct SystemUInt32Bind : ISerialize<System.UInt32>, ISerialize
    {
        public void Write(System.UInt32 value, ISegment stream)
        {
            stream.Write(value);
        }
        public System.UInt32 Read(ISegment stream)
        {
            return stream.ReadUInt32();
        }

        public void WriteValue(object value, ISegment stream)
        {
            stream.Write((System.UInt32)value);
        }

        object ISerialize.ReadValue(ISegment stream)
        {
            return stream.ReadUInt32();
        }
    }
    public readonly struct SystemSingleBind : ISerialize<System.Single>, ISerialize
    {
        public void Write(System.Single value, ISegment stream)
        {
            stream.Write(value);
        }
        public System.Single Read(ISegment stream)
        {
            return stream.ReadSingle();
        }

        public void WriteValue(object value, ISegment stream)
        {
            stream.Write((System.Single)value);
        }

        object ISerialize.ReadValue(ISegment stream)
        {
            return stream.ReadSingle();
        }
    }
    public readonly struct SystemInt64Bind : ISerialize<System.Int64>, ISerialize
    {
        public void Write(System.Int64 value, ISegment stream)
        {
            stream.Write(value);
        }
        public System.Int64 Read(ISegment stream)
        {
            return stream.ReadInt64();
        }

        public void WriteValue(object value, ISegment stream)
        {
            stream.Write((System.Int64)value);
        }

        object ISerialize.ReadValue(ISegment stream)
        {
            return stream.ReadInt64();
        }
    }
    public readonly struct SystemUInt64Bind : ISerialize<System.UInt64>, ISerialize
    {
        public void Write(System.UInt64 value, ISegment stream)
        {
            stream.Write(value);
        }
        public System.UInt64 Read(ISegment stream)
        {
            return stream.ReadUInt64();
        }

        public void WriteValue(object value, ISegment stream)
        {
            stream.Write((System.UInt64)value);
        }

        object ISerialize.ReadValue(ISegment stream)
        {
            return stream.ReadUInt64();
        }
    }
    public readonly struct SystemDoubleBind : ISerialize<System.Double>, ISerialize
    {
        public void Write(System.Double value, ISegment stream)
        {
            stream.Write(value);
        }
        public System.Double Read(ISegment stream)
        {
            return stream.ReadDouble();
        }

        public void WriteValue(object value, ISegment stream)
        {
            stream.Write((System.Double)value);
        }

        object ISerialize.ReadValue(ISegment stream)
        {
            return stream.ReadDouble();
        }
    }
    public readonly struct SystemStringBind : ISerialize<System.String>, ISerialize
    {
        public void Write(System.String value, ISegment stream)
        {
            stream.Write(value);
        }
        public System.String Read(ISegment stream)
        {
            return stream.ReadString();
        }

        public void WriteValue(object value, ISegment stream)
        {
            stream.Write((System.String)value);
        }

        object ISerialize.ReadValue(ISegment stream)
        {
            return stream.ReadString();
        }
    }
    public readonly struct SystemDecimalBind : ISerialize<System.Decimal>, ISerialize
    {
        public void Write(System.Decimal value, ISegment stream)
        {
            stream.Write(value);
        }
        public System.Decimal Read(ISegment stream)
        {
            return stream.ReadDecimal();
        }

        public void WriteValue(object value, ISegment stream)
        {
            stream.Write((System.Decimal)value);
        }

        object ISerialize.ReadValue(ISegment stream)
        {
            return stream.ReadDecimal();
        }
    }
    public readonly struct SystemDateTimeBind : ISerialize<System.DateTime>, ISerialize
    {
        public void Write(System.DateTime value, ISegment stream)
        {
            stream.Write(value);
        }
        public System.DateTime Read(ISegment stream)
        {
            return stream.ReadDateTime();
        }

        public void WriteValue(object value, ISegment stream)
        {
            stream.Write((System.DateTime)value);
        }

        object ISerialize.ReadValue(ISegment stream)
        {
            return stream.ReadDateTime();
        }
    }
    public readonly struct SystemTimeSpanBind : ISerialize<System.TimeSpan>, ISerialize
    {
        public void Write(System.TimeSpan value, ISegment stream)
        {
            stream.Write(value);
        }
        public System.TimeSpan Read(ISegment stream)
        {
            return stream.ReadTimeSpan();
        }

        public void WriteValue(object value, ISegment stream)
        {
            stream.Write((System.TimeSpan)value);
        }

        object ISerialize.ReadValue(ISegment stream)
        {
            return stream.ReadTimeSpan();
        }
    }
#if CORE
    public readonly struct SystemTimeOnlyBind : ISerialize<System.TimeOnly>, ISerialize
    {
        public void Write(System.TimeOnly value, ISegment stream)
        {
            stream.Write(value);
        }
        public System.TimeOnly Read(ISegment stream)
        {
            return stream.ReadTimeOnly();
        }

        public void WriteValue(object value, ISegment stream)
        {
            stream.Write((System.TimeOnly)value);
        }

        object ISerialize.ReadValue(ISegment stream)
        {
            return stream.ReadTimeOnly();
        }
    }
#endif
    public readonly struct SystemDateTimeOffsetBind : ISerialize<System.DateTimeOffset>, ISerialize
    {
        public void Write(System.DateTimeOffset value, ISegment stream)
        {
            stream.Write(value);
        }
        public System.DateTimeOffset Read(ISegment stream)
        {
            return stream.ReadDateTimeOffset();
        }

        public void WriteValue(object value, ISegment stream)
        {
            stream.Write((System.DateTimeOffset)value);
        }

        object ISerialize.ReadValue(ISegment stream)
        {
            return stream.ReadDateTimeOffset();
        }
    }
    public readonly struct SystemDBNullBind : ISerialize<System.DBNull>, ISerialize //这个类用作Null参数, 不需要写入和返回null即可
    {
        public void Write(System.DBNull value, ISegment stream)
        {
        }
        public System.DBNull Read(ISegment stream)
        {
            return null;
        }

        public void WriteValue(object value, ISegment stream)
        {
        }

        object ISerialize.ReadValue(ISegment stream)
        {
            return null;
        }
    }
    public readonly struct SystemEnumBind<T> : ISerialize<T>, ISerialize where T : System.Enum
    {
        public void Write(T value, ISegment stream)
        {
            stream.Write(value);
        }
        public T Read(ISegment stream)
        {
            return stream.ReadEnum<T>();
        }

        public void WriteValue(object value, ISegment stream)
        {
            stream.Write((T)value);
        }

        object ISerialize.ReadValue(ISegment stream)
        {
            return stream.ReadEnum<T>();
        }
    }
    #endregion

    #region "基元类型数组绑定"
    public readonly struct SystemByteArrayBind : ISerialize<System.Byte[]>, ISerialize
    {
        public void Write(System.Byte[] value, ISegment stream)
        {
            stream.Write(value);
        }

        public System.Byte[] Read(ISegment stream)
        {
            return stream.ReadByteArray();
        }

        public void WriteValue(object value, ISegment stream)
        {
            Write((System.Byte[])value, stream);
        }

        public object ReadValue(ISegment stream)
        {
            return Read(stream);
        }
    }
    public readonly struct SystemSByteArrayBind : ISerialize<System.SByte[]>, ISerialize
    {
        public void Write(System.SByte[] value, ISegment stream)
        {
            stream.Write(value);
        }

        public System.SByte[] Read(ISegment stream)
        {
            return stream.ReadSByteArray();
        }

        public void WriteValue(object value, ISegment stream)
        {
            Write((System.SByte[])value, stream);
        }

        public object ReadValue(ISegment stream)
        {
            return Read(stream);
        }
    }
    public readonly struct SystemBooleanArrayBind : ISerialize<System.Boolean[]>, ISerialize
    {
        public void Write(System.Boolean[] value, ISegment stream)
        {
            stream.Write(value);
        }

        public System.Boolean[] Read(ISegment stream)
        {
            return stream.ReadBooleanArray();
        }

        public void WriteValue(object value, ISegment stream)
        {
            Write((System.Boolean[])value, stream);
        }

        public object ReadValue(ISegment stream)
        {
            return Read(stream);
        }
    }
    public readonly struct SystemInt16ArrayBind : ISerialize<System.Int16[]>, ISerialize
    {
        public void Write(System.Int16[] value, ISegment stream)
        {
            stream.Write(value);
        }

        public System.Int16[] Read(ISegment stream)
        {
            return stream.ReadInt16Array();
        }

        public void WriteValue(object value, ISegment stream)
        {
            Write((System.Int16[])value, stream);
        }

        public object ReadValue(ISegment stream)
        {
            return Read(stream);
        }
    }
    public readonly struct SystemUInt16ArrayBind : ISerialize<System.UInt16[]>, ISerialize
    {
        public void Write(System.UInt16[] value, ISegment stream)
        {
            stream.Write(value);
        }

        public System.UInt16[] Read(ISegment stream)
        {
            return stream.ReadUInt16Array();
        }

        public void WriteValue(object value, ISegment stream)
        {
            Write((System.UInt16[])value, stream);
        }

        public object ReadValue(ISegment stream)
        {
            return Read(stream);
        }
    }
    public readonly struct SystemCharArrayBind : ISerialize<System.Char[]>, ISerialize
    {
        public void Write(System.Char[] value, ISegment stream)
        {
            stream.Write(value);
        }

        public System.Char[] Read(ISegment stream)
        {
            return stream.ReadCharArray();
        }

        public void WriteValue(object value, ISegment stream)
        {
            Write((System.Char[])value, stream);
        }

        public object ReadValue(ISegment stream)
        {
            return Read(stream);
        }
    }
    public readonly struct SystemInt32ArrayBind : ISerialize<System.Int32[]>, ISerialize
    {
        public void Write(System.Int32[] value, ISegment stream)
        {
            stream.Write(value);
        }

        public System.Int32[] Read(ISegment stream)
        {
            return stream.ReadInt32Array();
        }

        public void WriteValue(object value, ISegment stream)
        {
            Write((System.Int32[])value, stream);
        }

        public object ReadValue(ISegment stream)
        {
            return Read(stream);
        }
    }
    public readonly struct SystemUInt32ArrayBind : ISerialize<System.UInt32[]>, ISerialize
    {
        public void Write(System.UInt32[] value, ISegment stream)
        {
            stream.Write(value);
        }

        public System.UInt32[] Read(ISegment stream)
        {
            return stream.ReadUInt32Array();
        }

        public void WriteValue(object value, ISegment stream)
        {
            Write((System.UInt32[])value, stream);
        }

        public object ReadValue(ISegment stream)
        {
            return Read(stream);
        }
    }
    public readonly struct SystemSingleArrayBind : ISerialize<System.Single[]>, ISerialize
    {
        public void Write(System.Single[] value, ISegment stream)
        {
            stream.Write(value);
        }

        public System.Single[] Read(ISegment stream)
        {
            return stream.ReadSingleArray();
        }

        public void WriteValue(object value, ISegment stream)
        {
            Write((System.Single[])value, stream);
        }

        public object ReadValue(ISegment stream)
        {
            return Read(stream);
        }
    }
    public readonly struct SystemInt64ArrayBind : ISerialize<System.Int64[]>, ISerialize
    {
        public void Write(System.Int64[] value, ISegment stream)
        {
            stream.Write(value);
        }

        public System.Int64[] Read(ISegment stream)
        {
            return stream.ReadInt64Array();
        }

        public void WriteValue(object value, ISegment stream)
        {
            Write((System.Int64[])value, stream);
        }

        public object ReadValue(ISegment stream)
        {
            return Read(stream);
        }
    }
    public readonly struct SystemUInt64ArrayBind : ISerialize<System.UInt64[]>, ISerialize
    {
        public void Write(System.UInt64[] value, ISegment stream)
        {
            stream.Write(value);
        }

        public System.UInt64[] Read(ISegment stream)
        {
            return stream.ReadUInt64Array();
        }

        public void WriteValue(object value, ISegment stream)
        {
            Write((System.UInt64[])value, stream);
        }

        public object ReadValue(ISegment stream)
        {
            return Read(stream);
        }
    }
    public readonly struct SystemDoubleArrayBind : ISerialize<System.Double[]>, ISerialize
    {
        public void Write(System.Double[] value, ISegment stream)
        {
            stream.Write(value);
        }

        public System.Double[] Read(ISegment stream)
        {
            return stream.ReadDoubleArray();
        }

        public void WriteValue(object value, ISegment stream)
        {
            Write((System.Double[])value, stream);
        }

        public object ReadValue(ISegment stream)
        {
            return Read(stream);
        }
    }
    public readonly struct SystemStringArrayBind : ISerialize<System.String[]>, ISerialize
    {
        public void Write(System.String[] value, ISegment stream)
        {
            stream.Write(value);
        }

        public System.String[] Read(ISegment stream)
        {
            return stream.ReadStringArray();
        }

        public void WriteValue(object value, ISegment stream)
        {
            Write((System.String[])value, stream);
        }

        public object ReadValue(ISegment stream)
        {
            return Read(stream);
        }
    }
    public readonly struct SystemDecimalArrayBind : ISerialize<System.Decimal[]>, ISerialize
    {
        public void Write(System.Decimal[] value, ISegment stream)
        {
            stream.Write(value);
        }

        public System.Decimal[] Read(ISegment stream)
        {
            return stream.ReadDecimalArray();
        }

        public void WriteValue(object value, ISegment stream)
        {
            Write((System.Decimal[])value, stream);
        }

        public object ReadValue(ISegment stream)
        {
            return Read(stream);
        }
    }
    public readonly struct SystemDateTimeArrayBind : ISerialize<System.DateTime[]>, ISerialize
    {
        public void Write(System.DateTime[] value, ISegment stream)
        {
            stream.Write(value);
        }

        public System.DateTime[] Read(ISegment stream)
        {
            return stream.ReadDateTimeArray();
        }

        public void WriteValue(object value, ISegment stream)
        {
            Write((System.DateTime[])value, stream);
        }

        public object ReadValue(ISegment stream)
        {
            return Read(stream);
        }
    }
    public readonly struct SystemTimeSpanArrayBind : ISerialize<System.TimeSpan[]>, ISerialize
    {
        public void Write(System.TimeSpan[] value, ISegment stream)
        {
            stream.Write(value);
        }

        public System.TimeSpan[] Read(ISegment stream)
        {
            return stream.ReadTimeSpanArray();
        }

        public void WriteValue(object value, ISegment stream)
        {
            Write((System.TimeSpan[])value, stream);
        }

        public object ReadValue(ISegment stream)
        {
            return Read(stream);
        }
    }
#if CORE
    public readonly struct SystemTimeOnlyArrayBind : ISerialize<System.TimeOnly[]>, ISerialize
    {
        public void Write(System.TimeOnly[] value, ISegment stream)
        {
            stream.Write(value);
        }

        public System.TimeOnly[] Read(ISegment stream)
        {
            return stream.ReadTimeOnlyArray();
        }

        public void WriteValue(object value, ISegment stream)
        {
            Write((System.TimeOnly[])value, stream);
        }

        public object ReadValue(ISegment stream)
        {
            return Read(stream);
        }
    }
#endif
    public readonly struct SystemDateTimeOffsetArrayBind : ISerialize<System.DateTimeOffset[]>, ISerialize
    {
        public void Write(System.DateTimeOffset[] value, ISegment stream)
        {
            stream.Write(value);
        }

        public System.DateTimeOffset[] Read(ISegment stream)
        {
            return stream.ReadDateTimeOffsetArray();
        }

        public void WriteValue(object value, ISegment stream)
        {
            Write((System.DateTimeOffset[])value, stream);
        }

        public object ReadValue(ISegment stream)
        {
            return Read(stream);
        }
    }
    public readonly struct SystemDBNullArrayBind : ISerialize<System.DBNull[]>, ISerialize //这个类用作Null参数, 不需要写入和返回null即可
    {
        public void Write(System.DBNull[] value, ISegment stream)
        {
        }
        public System.DBNull[] Read(ISegment stream)
        {
            return null;
        }

        public void WriteValue(object value, ISegment stream)
        {
        }

        object ISerialize.ReadValue(ISegment stream)
        {
            return null;
        }
    }
    public readonly struct SystemEnumArrayBind<T> : ISerialize<T[]>, ISerialize where T : System.Enum
    {
        public void Write(T[] value, ISegment stream)
        {
            stream.Write(value.Length);
            for (int i = 0; i < value.Length; i++)
            {
                stream.Write(value[i]);
            }
        }
        public T[] Read(ISegment stream)
        {
            var count = stream.ReadInt32();
            var value = new T[count];
            for (int i = 0; i < count; i++)
            {
                value[i] = stream.ReadEnum<T>();
            }
            return value;
        }

        public void WriteValue(object value, ISegment stream)
        {
            Write((T[])value, stream);
        }

        object ISerialize.ReadValue(ISegment stream)
        {
            return Read(stream);
        }
    }
    #endregion

    #region "基元类型List绑定"
    public readonly struct SystemCollectionsGenericListSystemByteBind : ISerialize<System.Collections.Generic.List<System.Byte>>, ISerialize
    {
        public void Write(System.Collections.Generic.List<System.Byte> value, ISegment stream)
        {
            stream.Write(value);
        }

        public System.Collections.Generic.List<System.Byte> Read(ISegment stream)
        {
            return stream.ReadByteList();
        }

        public void WriteValue(object value, ISegment stream)
        {
            Write((System.Collections.Generic.List<System.Byte>)value, stream);
        }

        public object ReadValue(ISegment stream)
        {
            return Read(stream);
        }
    }
    public readonly struct SystemCollectionsGenericListSystemSByteBind : ISerialize<System.Collections.Generic.List<System.SByte>>, ISerialize
    {
        public void Write(System.Collections.Generic.List<System.SByte> value, ISegment stream)
        {
            stream.Write(value);
        }

        public System.Collections.Generic.List<System.SByte> Read(ISegment stream)
        {
            return stream.ReadSByteList();
        }

        public void WriteValue(object value, ISegment stream)
        {
            Write((System.Collections.Generic.List<System.SByte>)value, stream);
        }

        public object ReadValue(ISegment stream)
        {
            return Read(stream);
        }
    }
    public readonly struct SystemCollectionsGenericListSystemBooleanBind : ISerialize<System.Collections.Generic.List<System.Boolean>>, ISerialize
    {
        public void Write(System.Collections.Generic.List<System.Boolean> value, ISegment stream)
        {
            stream.Write(value);
        }

        public System.Collections.Generic.List<System.Boolean> Read(ISegment stream)
        {
            return stream.ReadBooleanList();
        }

        public void WriteValue(object value, ISegment stream)
        {
            Write((System.Collections.Generic.List<System.Boolean>)value, stream);
        }

        public object ReadValue(ISegment stream)
        {
            return Read(stream);
        }
    }
    public readonly struct SystemCollectionsGenericListSystemInt16Bind : ISerialize<System.Collections.Generic.List<System.Int16>>, ISerialize
    {
        public void Write(System.Collections.Generic.List<System.Int16> value, ISegment stream)
        {
            stream.Write(value);
        }

        public System.Collections.Generic.List<System.Int16> Read(ISegment stream)
        {
            return stream.ReadInt16List();
        }

        public void WriteValue(object value, ISegment stream)
        {
            Write((System.Collections.Generic.List<System.Int16>)value, stream);
        }

        public object ReadValue(ISegment stream)
        {
            return Read(stream);
        }
    }
    public readonly struct SystemCollectionsGenericListSystemUInt16Bind : ISerialize<System.Collections.Generic.List<System.UInt16>>, ISerialize
    {
        public void Write(System.Collections.Generic.List<System.UInt16> value, ISegment stream)
        {
            stream.Write(value);
        }

        public System.Collections.Generic.List<System.UInt16> Read(ISegment stream)
        {
            return stream.ReadUInt16List();
        }

        public void WriteValue(object value, ISegment stream)
        {
            Write((System.Collections.Generic.List<System.UInt16>)value, stream);
        }

        public object ReadValue(ISegment stream)
        {
            return Read(stream);
        }
    }
    public readonly struct SystemCollectionsGenericListSystemCharBind : ISerialize<System.Collections.Generic.List<System.Char>>, ISerialize
    {
        public void Write(System.Collections.Generic.List<System.Char> value, ISegment stream)
        {
            stream.Write(value);
        }

        public System.Collections.Generic.List<System.Char> Read(ISegment stream)
        {
            return stream.ReadCharList();
        }

        public void WriteValue(object value, ISegment stream)
        {
            Write((System.Collections.Generic.List<System.Char>)value, stream);
        }

        public object ReadValue(ISegment stream)
        {
            return Read(stream);
        }
    }
    public readonly struct SystemCollectionsGenericListSystemInt32Bind : ISerialize<System.Collections.Generic.List<System.Int32>>, ISerialize
    {
        public void Write(System.Collections.Generic.List<System.Int32> value, ISegment stream)
        {
            stream.Write(value);
        }

        public System.Collections.Generic.List<System.Int32> Read(ISegment stream)
        {
            return stream.ReadInt32List();
        }

        public void WriteValue(object value, ISegment stream)
        {
            Write((System.Collections.Generic.List<System.Int32>)value, stream);
        }

        public object ReadValue(ISegment stream)
        {
            return Read(stream);
        }
    }
    public readonly struct SystemCollectionsGenericListSystemUInt32Bind : ISerialize<System.Collections.Generic.List<System.UInt32>>, ISerialize
    {
        public void Write(System.Collections.Generic.List<System.UInt32> value, ISegment stream)
        {
            stream.Write(value);
        }

        public System.Collections.Generic.List<System.UInt32> Read(ISegment stream)
        {
            return stream.ReadUInt32List();
        }

        public void WriteValue(object value, ISegment stream)
        {
            Write((System.Collections.Generic.List<System.UInt32>)value, stream);
        }

        public object ReadValue(ISegment stream)
        {
            return Read(stream);
        }
    }
    public readonly struct SystemCollectionsGenericListSystemSingleBind : ISerialize<System.Collections.Generic.List<System.Single>>, ISerialize
    {
        public void Write(System.Collections.Generic.List<System.Single> value, ISegment stream)
        {
            stream.Write(value);
        }

        public System.Collections.Generic.List<System.Single> Read(ISegment stream)
        {
            return stream.ReadSingleList();
        }

        public void WriteValue(object value, ISegment stream)
        {
            Write((System.Collections.Generic.List<System.Single>)value, stream);
        }

        public object ReadValue(ISegment stream)
        {
            return Read(stream);
        }
    }
    public readonly struct SystemCollectionsGenericListSystemInt64Bind : ISerialize<System.Collections.Generic.List<System.Int64>>, ISerialize
    {
        public void Write(System.Collections.Generic.List<System.Int64> value, ISegment stream)
        {
            stream.Write(value);
        }

        public System.Collections.Generic.List<System.Int64> Read(ISegment stream)
        {
            return stream.ReadInt64List();
        }

        public void WriteValue(object value, ISegment stream)
        {
            Write((System.Collections.Generic.List<System.Int64>)value, stream);
        }

        public object ReadValue(ISegment stream)
        {
            return Read(stream);
        }
    }
    public readonly struct SystemCollectionsGenericListSystemUInt64Bind : ISerialize<System.Collections.Generic.List<System.UInt64>>, ISerialize
    {
        public void Write(System.Collections.Generic.List<System.UInt64> value, ISegment stream)
        {
            stream.Write(value);
        }

        public System.Collections.Generic.List<System.UInt64> Read(ISegment stream)
        {
            return stream.ReadUInt64List();
        }

        public void WriteValue(object value, ISegment stream)
        {
            Write((System.Collections.Generic.List<System.UInt64>)value, stream);
        }

        public object ReadValue(ISegment stream)
        {
            return Read(stream);
        }
    }
    public readonly struct SystemCollectionsGenericListSystemDoubleBind : ISerialize<System.Collections.Generic.List<System.Double>>, ISerialize
    {
        public void Write(System.Collections.Generic.List<System.Double> value, ISegment stream)
        {
            stream.Write(value);
        }

        public System.Collections.Generic.List<System.Double> Read(ISegment stream)
        {
            return stream.ReadDoubleList();
        }

        public void WriteValue(object value, ISegment stream)
        {
            Write((System.Collections.Generic.List<System.Double>)value, stream);
        }

        public object ReadValue(ISegment stream)
        {
            return Read(stream);
        }
    }
    public readonly struct SystemCollectionsGenericListSystemStringBind : ISerialize<System.Collections.Generic.List<System.String>>, ISerialize
    {
        public void Write(System.Collections.Generic.List<System.String> value, ISegment stream)
        {
            stream.Write(value);
        }

        public System.Collections.Generic.List<System.String> Read(ISegment stream)
        {
            return stream.ReadStringList();
        }

        public void WriteValue(object value, ISegment stream)
        {
            Write((System.Collections.Generic.List<System.String>)value, stream);
        }

        public object ReadValue(ISegment stream)
        {
            return Read(stream);
        }
    }
    public readonly struct SystemCollectionsGenericListSystemDecimalBind : ISerialize<System.Collections.Generic.List<System.Decimal>>, ISerialize
    {
        public void Write(System.Collections.Generic.List<System.Decimal> value, ISegment stream)
        {
            stream.Write(value);
        }

        public System.Collections.Generic.List<System.Decimal> Read(ISegment stream)
        {
            return stream.ReadDecimalList();
        }

        public void WriteValue(object value, ISegment stream)
        {
            Write((System.Collections.Generic.List<System.Decimal>)value, stream);
        }

        public object ReadValue(ISegment stream)
        {
            return Read(stream);
        }
    }
    public readonly struct SystemCollectionsGenericListSystemDateTimeBind : ISerialize<System.Collections.Generic.List<System.DateTime>>, ISerialize
    {
        public void Write(System.Collections.Generic.List<System.DateTime> value, ISegment stream)
        {
            stream.Write(value);
        }

        public System.Collections.Generic.List<System.DateTime> Read(ISegment stream)
        {
            return stream.ReadDateTimeList();
        }

        public void WriteValue(object value, ISegment stream)
        {
            Write((System.Collections.Generic.List<System.DateTime>)value, stream);
        }

        public object ReadValue(ISegment stream)
        {
            return Read(stream);
        }
    }
    public readonly struct SystemCollectionsGenericListSystemTimeSpanBind : ISerialize<System.Collections.Generic.List<System.TimeSpan>>, ISerialize
    {
        public void Write(System.Collections.Generic.List<System.TimeSpan> value, ISegment stream)
        {
            stream.Write(value);
        }

        public System.Collections.Generic.List<System.TimeSpan> Read(ISegment stream)
        {
            return stream.ReadTimeSpanList();
        }

        public void WriteValue(object value, ISegment stream)
        {
            Write((System.Collections.Generic.List<System.TimeSpan>)value, stream);
        }

        public object ReadValue(ISegment stream)
        {
            return Read(stream);
        }
    }
#if CORE
    public readonly struct SystemCollectionsGenericListSystemTimeOnlyBind : ISerialize<System.Collections.Generic.List<System.TimeOnly>>, ISerialize
    {
        public void Write(System.Collections.Generic.List<System.TimeOnly> value, ISegment stream)
        {
            stream.Write(value);
        }

        public System.Collections.Generic.List<System.TimeOnly> Read(ISegment stream)
        {
            return stream.ReadTimeOnlyList();
        }

        public void WriteValue(object value, ISegment stream)
        {
            Write((System.Collections.Generic.List<System.TimeOnly>)value, stream);
        }

        public object ReadValue(ISegment stream)
        {
            return Read(stream);
        }
    }
#endif
    public readonly struct SystemCollectionsGenericListSystemDateTimeOffsetBind : ISerialize<System.Collections.Generic.List<System.DateTimeOffset>>, ISerialize
    {
        public void Write(System.Collections.Generic.List<System.DateTimeOffset> value, ISegment stream)
        {
            stream.Write(value);
        }

        public System.Collections.Generic.List<System.DateTimeOffset> Read(ISegment stream)
        {
            return stream.ReadDateTimeOffsetList();
        }

        public void WriteValue(object value, ISegment stream)
        {
            Write((System.Collections.Generic.List<System.DateTimeOffset>)value, stream);
        }

        public object ReadValue(ISegment stream)
        {
            return Read(stream);
        }
    }
    public readonly struct SystemCollectionsGenericListSystemDBNullBind : ISerialize<System.Collections.Generic.List<System.DBNull>>, ISerialize //这个类用作Null参数, 不需要写入和返回null即可
    {
        public void Write(System.Collections.Generic.List<System.DBNull> value, ISegment stream)
        {
        }
        public System.Collections.Generic.List<System.DBNull> Read(ISegment stream)
        {
            return null;
        }

        public void WriteValue(object value, ISegment stream)
        {
        }

        object ISerialize.ReadValue(ISegment stream)
        {
            return null;
        }
    }
    public readonly struct SystemCollectionsGenericListSystemEnumBind<T> : ISerialize<System.Collections.Generic.List<T>>, ISerialize where T : System.Enum
    {
        public void Write(System.Collections.Generic.List<T> value, ISegment stream)
        {
            stream.Write(value.Count);
            for (int i = 0; i < value.Count; i++)
            {
                stream.Write(value[i]);
            }
        }
        public System.Collections.Generic.List<T> Read(ISegment stream)
        {
            var count = stream.ReadInt32();
            var value = new System.Collections.Generic.List<T>(count);
            for (int i = 0; i < count; i++)
            {
                value.Add(stream.ReadEnum<T>());
            }
            return value;
        }

        public void WriteValue(object value, ISegment stream)
        {
            Write((System.Collections.Generic.List<T>)value, stream);
        }

        object ISerialize.ReadValue(ISegment stream)
        {
            return Read(stream);
        }
    }
    #endregion
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Net.System
{
    /// <summary>
    /// 内存数据片段接口
    /// </summary>
    public interface ISegment : IDisposable
    {
        /// <summary>
        /// 总内存
        /// </summary>
        byte[] Buffer { get; set; }
        /// <summary>
        /// 片的开始位置
        /// </summary>
        int Offset { get; set; }
        /// <summary>
        /// 片的长度
        /// </summary>
        int Count { get; set; }
        /// <summary>
        /// 读写位置
        /// </summary>
        int Position { get; set; }
        /// <summary>
        /// 获取总长度
        /// </summary>
        int Length { get; }
        bool IsDespose { get; set; }
        bool IsRecovery { get; set; }
        int ReferenceCount { get; set; }

        /// <summary>
        /// 获取或设置总内存位置索引
        /// </summary>
        /// <param name="index">内存位置索引</param>
        /// <returns></returns>
        byte this[int index] { get; set; }

        void Init();

        void SetPosition(int position);

        void Dispose();

        void Close();

        /// <summary>
        /// 复制分片数据
        /// </summary>
        /// <param name="recovery">复制数据后立即回收此分片?</param>
        /// <returns></returns>
        byte[] ToArray(bool recovery = false, bool resetPos = false);

        unsafe void Write(void* ptr, int count);

        void WriteValue<T>(T value);

        T ReadValue<T>();

        object ReadValue(Type type);

        object ReadValue(TypeCode type);

        byte[] Read(int count);

        void WriteList(object value);

        void WriteArray<T>(T[] array);
        unsafe void WriteArray(object value);
        List<T> ReadList<T>();
        object ReadList(Type type);
        T[] ReadArray<T>();
        object ReadArray(Type type);
        object ReadArray(TypeCode type);
        void SetLength(int length);

        void SetPositionLength(int length);

#region Write
        void WriteByte(byte value);
        void WriteSByte(sbyte value);
        void Write(byte value);
        void Write(sbyte value);
        void Write(bool value);
        void Write(short value);
        unsafe void Write(ushort value);
        unsafe void WriteFixed(ushort value);
        void Write(char value);
        void Write(int value);
        unsafe void Write(uint value);
        unsafe void WriteFixed(uint value);
        unsafe void Write(float value);
        void Write(long value);
        unsafe void Write(ulong value);
        unsafe void WriteFixed(ulong value);
        unsafe void Write(double value);
        void Write(DateTime value);
        void Write(TimeSpan value);
#if CORE
        void Write(TimeOnly value);
#endif
        void Write(DateTimeOffset value);
        unsafe void Write(decimal value);
        void Write(Enum value);
        unsafe void Write(string value);
        /// <summary>
        /// 写入字节数组
        /// </summary>
        /// <param name="value"></param>
        /// <param name="recordLength">是否记录此次写入的字节长度?</param>
        void Write(byte[] value, bool recordLength = true);
        void Write(byte[] value, int index, int count);
        /// <summary>
        /// 写入字节数组
        /// </summary>
        /// <param name="value"></param>
        /// <param name="recordLength">是否记录此次写入的字节长度?</param>
        
        void Write(sbyte[] value, bool recordLength = true);
        unsafe void Write(bool[] value);
        unsafe void Write(short[] value);
        unsafe void Write(ushort[] value);
        unsafe void Write(char[] value);
        unsafe void Write(int[] value);
        unsafe void Write(uint[] value);
        unsafe void Write(float[] value);
        unsafe void Write(long[] value);
        unsafe void Write(ulong[] value);
        unsafe void Write(DateTime[] value);
        unsafe void Write(TimeSpan[] value);
#if CORE
        unsafe void Write(TimeOnly[] value);
#endif
        unsafe void Write(DateTimeOffset[] value);
        unsafe void Write(double[] value);
        unsafe void Write(decimal[] value);
        unsafe void Write(string[] value);
        unsafe void WriteGeneric(IEnumerable value);
        unsafe void Write(ICollection<byte> value);
        unsafe void Write(ICollection<sbyte> value);
        unsafe void Write(ICollection<bool> value);
        unsafe void Write(ICollection<short> value);
        unsafe void Write(ICollection<ushort> value);
        unsafe void Write(ICollection<char> value);
        unsafe void Write(ICollection<int> value);
        unsafe void Write(ICollection<uint> value);
        unsafe void Write(ICollection<float> value);
        unsafe void Write(ICollection<long> value);
        unsafe void Write(ICollection<ulong> value);
        unsafe void Write(ICollection<double> value);
        unsafe void Write(ICollection<decimal> value);
        unsafe void Write(ICollection<string> value);
        unsafe void Write(ICollection<DateTime> value);
        unsafe void Write(ICollection<TimeSpan> value);
#if CORE
        unsafe void Write(ICollection<TimeOnly> value);
#endif
        unsafe void Write(ICollection<DateTimeOffset> value);
        unsafe void Write(ICollection<Enum> value);
#endregion

#region Read
        byte ReadByte();
        sbyte ReadSByte();
        bool ReadBoolean();
        unsafe short ReadInt16();
        unsafe ushort ReadUInt16();
        unsafe ushort ReadUInt16Fixed();
        char ReadChar();
        unsafe int ReadInt32();
        unsafe uint ReadUInt32();
        unsafe uint ReadUInt32Fixed();
        unsafe float ReadFloat();
        unsafe float ReadSingle();
        unsafe long ReadInt64();
        unsafe ulong ReadUInt64();
        unsafe ulong ReadUInt64Fixed();
        unsafe double ReadDouble();
        DateTime ReadDateTime();
        TimeSpan ReadTimeSpan();
#if CORE
        TimeOnly ReadTimeOnly();
#endif
        DateTimeOffset ReadDateTimeOffset();
        unsafe decimal ReadDecimal();
        unsafe T ReadEnum<T>() where T : Enum;
        unsafe object ReadEnum(Type type);
        unsafe string ReadString();
        unsafe byte[] ReadByteArray();
        unsafe sbyte[] ReadSByteArray();
        unsafe bool[] ReadBooleanArray();
        unsafe short[] ReadInt16Array();
        unsafe ushort[] ReadUInt16Array();
        unsafe char[] ReadCharArray();
        unsafe int[] ReadInt32Array();
        unsafe uint[] ReadUInt32Array();
        unsafe float[] ReadFloatArray();
        unsafe float[] ReadSingleArray();
        unsafe long[] ReadInt64Array();
        unsafe ulong[] ReadUInt64Array();
        unsafe double[] ReadDoubleArray();
        unsafe DateTime[] ReadDateTimeArray();
        unsafe TimeSpan[] ReadTimeSpanArray();
#if CORE
        unsafe TimeOnly[] ReadTimeOnlyArray();
#endif
        unsafe DateTimeOffset[] ReadDateTimeOffsetArray();
        unsafe decimal[] ReadDecimalArray();
        unsafe string[] ReadStringArray();
        ICollection ReadGeneric(Type type);
        List<byte> ReadByteList();
        T ReadByteGeneric<T>() where T : ICollection<byte>, new();
        List<sbyte> ReadSByteList();
        T ReadSByteGeneric<T>() where T : ICollection<sbyte>, new();
        List<bool> ReadBooleanList();
        T ReadBooleanGeneric<T>() where T : ICollection<bool>, new();
        List<short> ReadInt16List();
        T ReadInt16Generic<T>() where T : ICollection<short>, new();
        List<ushort> ReadUInt16List();
        T ReadUInt16Generic<T>() where T : ICollection<ushort>, new();
        List<char> ReadCharList();
        T ReadCharGeneric<T>() where T : ICollection<char>, new();
        List<int> ReadInt32List();
        T ReadInt32Generic<T>() where T : ICollection<int>, new();
        List<uint> ReadUInt32List();
        T ReadUInt32Generic<T>() where T : ICollection<uint>, new();
        List<float> ReadFloatList();
        List<float> ReadSingleList();
        T ReadSingleGeneric<T>() where T : ICollection<float>, new();
        List<long> ReadInt64List();
        T ReadInt64Generic<T>() where T : ICollection<long>, new();
        List<ulong> ReadUInt64List();
        T ReadUInt64Generic<T>() where T : ICollection<ulong>, new();
        List<double> ReadDoubleList();
        T ReadDoubleGeneric<T>() where T : ICollection<double>, new();
        List<DateTime> ReadDateTimeList();
        T ReadDateTimeGeneric<T>() where T : ICollection<DateTime>, new();
        List<TimeSpan> ReadTimeSpanList();
        T ReadTimeSpanGeneric<T>() where T : ICollection<TimeSpan>, new();
#if CORE
        List<TimeOnly> ReadTimeOnlyList();
        T ReadTimeOnlyGeneric<T>() where T : ICollection<TimeOnly>, new();
#endif
        List<DateTimeOffset> ReadDateTimeOffsetList();
        T ReadDateTimeOffsetGeneric<T>() where T : ICollection<DateTimeOffset>, new();
        List<decimal> ReadDecimalList();
        T ReadDecimalGeneric<T>() where T : ICollection<decimal>, new();
        List<string> ReadStringList();
        T ReadStringGeneric<T>() where T : ICollection<string>, new();
#endregion

        void Flush(bool resetPos = true);
    }
}
#if CORE
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Net.Serialize
{
    public class NetConvertHelper
    {
        public static unsafe void WriteArray<T>(byte* ptr, ref int offset, T[] value) where T : struct
        {
            if (value != null)
            {
                fixed (void* fa1Ptr = &value[0])
                {
                    int fa1Len = value.Length;
                    Unsafe.WriteUnaligned(ptr + offset, fa1Len);
                    offset += 2;
                    int count = fa1Len * Unsafe.SizeOf<T>();
                    Unsafe.CopyBlock(ptr + offset, fa1Ptr, (uint)count);
                    offset += count;
                }
            }
            else
            {
                Unsafe.WriteUnaligned(ptr + offset, 0);
                offset += 2;
            }
        }

        public static unsafe void WriteArray(byte* ptr, ref int offset, string[] value)
        {
            if (value != null)
            {
                int fa1Len = value.Length;
                Unsafe.WriteUnaligned(ptr + offset, fa1Len);
                offset += 2;
                for (int i = 0; i < fa1Len; i++)
                {
                    Write(ptr, ref offset, value[i]);
                }
            }
            else
            {
                Unsafe.WriteUnaligned(ptr + offset, 0);
                offset += 2;
            }
        }

        public static unsafe void WriteCollection<T>(byte* ptr, ref int offset, ICollection<T> value) where T : struct
        {
            WriteArray(ptr, ref offset, value?.ToArray());
        }

        public static unsafe void WriteCollection(byte* ptr, ref int offset, ICollection<string> value)
        {
            WriteArray(ptr, ref offset, value?.ToArray());
        }

        public static unsafe T[] ReadArray<T>(byte* ptr, ref int offset) where T : struct
        {
            var arrayLen = Unsafe.ReadUnaligned<ushort>(ptr + offset);
            offset += 2;
            if (arrayLen > 0)
            {
                var value = new T[arrayLen];
                fixed (void* fa1Ptr = &value[0])
                {
                    int count = arrayLen * Unsafe.SizeOf<T>();
                    Unsafe.CopyBlock(fa1Ptr, ptr + offset, (uint)count);
                    offset += count;
                }
                return value;
            }
            return default;
        }

        public static unsafe T ReadCollection<T, T1>(byte* ptr, ref int offset) where T : ICollection<T1>, new() where T1 : struct
        {
            var arrayLen = Unsafe.ReadUnaligned<ushort>(ptr + offset);
            offset += 2;
            if (arrayLen > 0)
            {
                var value = new T();
                var newValue = new T1[arrayLen];
                fixed (void* fa1Ptr = &newValue[0])
                {
                    int count = arrayLen * Unsafe.SizeOf<T1>();
                    Unsafe.CopyBlock(fa1Ptr, ptr + offset, (uint)count);
                    offset += count;
                    for (int i = 0; i < arrayLen; i++)
                    {
                        value.Add(newValue[i]);
                    }
                }
                return value;
            }
            return default;
        }

        public static unsafe T ReadCollection<T>(byte* ptr, ref int offset) where T : ICollection<string>, new()
        {
            var newValue = ReadArray(ptr, ref offset);
            if (newValue == null)
                return default;
            var value = new T();
            for (int i = 0; i < newValue.Length; i++)
            {
                value.Add(newValue[i]);
            }
            return value;
        }

        public static unsafe string[] ReadArray(byte* ptr, ref int offset)
        {
            var arrayLen = Unsafe.ReadUnaligned<ushort>(ptr + offset);
            offset += 2;
            if (arrayLen > 0)
            {
                var value = new string[arrayLen];
                for (int i = 0; i < arrayLen; i++)
                {
                    value[i] = Read(ptr, ref offset);
                }
                return value;
            }
            return default;
        }

        public static unsafe void Write<T>(byte* ptr, ref int offset, T value) where T : struct
        {
            Unsafe.WriteUnaligned(ptr + offset, value);
            offset += Unsafe.SizeOf<T>();
        }

        public static unsafe void Write(byte* ptr, ref int offset, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                var charSpan = value.AsSpan();
                var byteSpan = new Span<byte>(ptr + offset + 2, value.Length * 3);
                var f15Count = Encoding.UTF8.GetBytes(charSpan, byteSpan);
                Unsafe.WriteUnaligned(ptr + offset, (ushort)f15Count);
                offset += 2 + f15Count;
            }
            else
            {
                Unsafe.WriteUnaligned(ptr + offset, 0);
                offset += 2;
            }
        }

        public static unsafe T Read<T>(byte* ptr, ref int offset) where T : struct
        {
            var value = Unsafe.ReadUnaligned<T>(ptr + offset);
            offset += Unsafe.SizeOf<T>();
            return value;
        }

        public static unsafe string Read(byte* ptr, ref int offset)
        {
            var f15Count = Unsafe.ReadUnaligned<ushort>(ptr + offset);
            offset += 2;
            if (f15Count > 0)
            {
                var value = Encoding.UTF8.GetString(new ReadOnlySpan<byte>(ptr + offset, f15Count));
                offset += f15Count;
                return value;
            }
            return string.Empty;
        }
    }
}
#endif
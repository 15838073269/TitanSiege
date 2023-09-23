using Net.Event;
using Net.Serialize;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Net.System
{
    /// <summary>
    /// 内存数据片段
    /// </summary>
    public class Segment2 : Segment
    {
        public int RecordIndex { get; set; }
        public byte RecordBitIndex { get; set; }
        
        /// <summary>
        /// 构造内存分片
        /// </summary>
        /// <param name="buffer"></param>
        public Segment2(byte[] buffer) : this(buffer, 0, buffer.Length)
        {
        }

        /// <summary>
        /// 构造内存分片
        /// </summary>
        /// <param name="buffer"></param>
        public Segment2(byte[] buffer, bool isRecovery) : this(buffer, 0, buffer.Length, isRecovery)
        {
        }

        /// <summary>
        /// 构造内存分片
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="index"></param>
        /// <param name="count"></param>
        public Segment2(byte[] buffer, int index, int count, bool isRecovery = true) : base(buffer, index, count, isRecovery) 
        {
        }

        public static implicit operator Segment2(byte[] buffer)
        {
            return new Segment2(buffer);
        }

        public static implicit operator byte[](Segment2 segment)
        {
            return segment.Buffer;
        }

        ~Segment2()
        {
            if (IsRecovery && BufferPool.Log)
                NDebug.LogError("片段内存泄漏!请检查代码正确Push内存池!");
            Dispose();
        }

        public override void Init()
        {
            base.Init();
            RecordIndex = 0;
            RecordBitIndex = 0;
        }

        public override void SetPosition(int position)
        {
            Position = position;
            RecordIndex = 0;
            RecordBitIndex = 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override unsafe void Write(ushort value) 
        {
            WriteUInt32Internal(value, 2); // 2 ^ 2 = 最大值4
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Write(uint value)
        {
            WriteUInt32Internal(value, 3); // 2 ^ 3 = 最大值8
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private unsafe void WriteUInt32Internal(uint value, byte bitCount)
        {
            CheckRecordBitInternal(bitCount);
            if (value == 0)
            {
                RecordBitIndex += bitCount;
                return;
            }
            fixed (byte* ptr = &Buffer[Position])
            {
                byte num = 0;
                while (value > 0)
                {
                    ptr[num] = (byte)(value >> 0);
                    value >>= 8;
                    num++;
                }
                SetRecordBitInternal(num, bitCount);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void CheckRecordBitInternal(byte bitCount)
        {
            if (((RecordBitIndex - 1) + bitCount) > 8 | RecordBitIndex == 0)
            {
                RecordBitIndex = 1;
                RecordIndex = Position;
                Position++;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SetRecordBitInternal(byte byteCount, byte bitCount)
        {
            NetConvertBase.SetByteBits(ref Buffer[RecordIndex], RecordBitIndex, (byte)(RecordBitIndex + bitCount), byteCount, (byte)(9 - bitCount));
            Position += byteCount;
            RecordBitIndex += bitCount;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override unsafe void Write(ulong value)
        {
            WriteUInt64Internal(value, 4); // 2 ^ 4 = 4位的二进制最大值16
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private unsafe void WriteUInt64Internal(ulong value, byte bitCount)
        {
            CheckRecordBitInternal(bitCount);
            if (value == 0)
            {
                RecordBitIndex += bitCount;
                return;
            }
            fixed (byte* ptr = &Buffer[Position])
            {
                byte num = 0;
                while (value > 0)
                {
                    ptr[num] = (byte)(value >> 0);
                    value >>= 8;
                    num++;
                }
                SetRecordBitInternal(num, bitCount);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override unsafe void Write(string value)
        {
            CheckRecordBitInternal(3);
            if (string.IsNullOrEmpty(value))
            {
                RecordBitIndex += 3; // 2 ^ 3 = 最大值8
                return;
            }
            int count = value.Length;
            fixed (char* ptr = value)
            {
                int byteCount = UTF8Encoding.UTF8.GetByteCount(ptr, count);
                Write(byteCount);
                fixed (byte* ptr1 = &Buffer[Position])
                {
                    Encoding.UTF8.GetBytes(ptr, count, ptr1, byteCount);
                    Position += byteCount;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override unsafe ushort ReadUInt16()
        {
            return (ushort)ReadUInt32Internal(2); // 2 ^ 2 = 最大值4
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override unsafe uint ReadUInt32() 
        {
            return ReadUInt32Internal(3); // 2 ^ 3 = 最大值8
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private unsafe uint ReadUInt32Internal(byte bitCount)
        {
            CheckRecordBitInternal(bitCount);
            var num = NetConvertBase.GetByteBits(Buffer[RecordIndex], RecordBitIndex, (byte)(RecordBitIndex + bitCount), (byte)(9 - bitCount));
            RecordBitIndex += bitCount;
            if (num == 0)
                return 0;
            fixed (byte* ptr = &Buffer[Position])
            {
                Position += num;
                uint value = 0;
                if (BitConverter.IsLittleEndian)
                {
                    for (byte i = 0; i < num; i++)
                        value |= (uint)ptr[i] << (i * 8);
                    return value;
                }
                else
                {
                    num -= 1;
                    for (byte i = num; i >= 0; i--)
                        value |= (uint)ptr[i] << (i * 8);
                    return value;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override unsafe ulong ReadUInt64()
        {
            return ReadUInt64Internal(4); // 2 ^ 4 = 最大值16
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private unsafe ulong ReadUInt64Internal(byte bitCount)
        {
            CheckRecordBitInternal(bitCount);
            var num = NetConvertBase.GetByteBits(Buffer[RecordIndex], RecordBitIndex, (byte)(RecordBitIndex + bitCount), (byte)(9 - bitCount));
            RecordBitIndex += bitCount;
            if (num == 0)
                return 0;
            fixed (byte* ptr = &Buffer[Position])
            {
                Position += num;
                ulong value = 0;
                if (BitConverter.IsLittleEndian)
                {
                    for (byte i = 0; i < num; i++)
                        value |= (ulong)ptr[i] << (i * 8);
                    return value;
                }
                else
                {
                    num -= 1;
                    for (byte i = num; i >= 0; i--)
                        value |= (ulong)ptr[i] << (i * 8);
                    return value;
                }
            }
        }

        public override void Flush(bool resetPos = true)
        {
            if (Position > Count)
                Count = Position;
            if (resetPos)
            {
                Position = Offset;
                RecordIndex = 0;
                RecordBitIndex = 0;
            }
        }
    }
}

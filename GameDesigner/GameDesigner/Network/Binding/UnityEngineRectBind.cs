using Net.Serialize;
using Net.System;
using System;
using System.Collections.Generic;

namespace Binding
{
    public struct UnityEngineRectBind : ISerialize<UnityEngine.Rect>, ISerialize
    {
        public void Write(UnityEngine.Rect value, ISegment stream)
        {
            int pos = stream.Position;
            stream.Position += 1;
            byte[] bits = new byte[1];

            if (value.x != 0)
            {
                NetConvertBase.SetBit(ref bits[0], 1, true);
                stream.Write(value.x);
            }

            if (value.y != 0)
            {
                NetConvertBase.SetBit(ref bits[0], 2, true);
                stream.Write(value.y);
            }

            if (value.width != 0)
            {
                NetConvertBase.SetBit(ref bits[0], 3, true);
                stream.Write(value.width);
            }

            if (value.height != 0)
            {
                NetConvertBase.SetBit(ref bits[0], 4, true);
                stream.Write(value.height);
            }

            int pos1 = stream.Position;
            stream.Position = pos;
            stream.Write(bits, 0, 1);
            stream.Position = pos1;
        }
		
		public UnityEngine.Rect Read(ISegment stream)
		{
			byte[] bits = stream.Read(1);
			var value = new UnityEngine.Rect();

			if(NetConvertBase.GetBit(bits[0], 1))
				value.x = stream.ReadSingle();

			if(NetConvertBase.GetBit(bits[0], 2))
				value.y = stream.ReadSingle();

			if(NetConvertBase.GetBit(bits[0], 3))
				value.width = stream.ReadSingle();

			if(NetConvertBase.GetBit(bits[0], 4))
				value.height = stream.ReadSingle();

			return value;
		}

        public void WriteValue(object value, ISegment stream)
        {
            Write((UnityEngine.Rect)value, stream);
        }

        public object ReadValue(ISegment stream)
        {
            return Read(stream);
        }
    }
}

namespace Binding
{
	public struct UnityEngineRectArrayBind : ISerialize<UnityEngine.Rect[]>, ISerialize
	{
		public void Write(UnityEngine.Rect[] value, ISegment stream)
		{
			int count = value.Length;
			stream.Write(count);
			if (count == 0) return;
			var bind = new UnityEngineRectBind();
			foreach (var value1 in value)
				bind.Write(value1, stream);
		}

		public UnityEngine.Rect[] Read(ISegment stream)
		{
			var count = stream.ReadInt32();
			var value = new UnityEngine.Rect[count];
			if (count == 0) return value;
			var bind = new UnityEngineRectBind();
			for (int i = 0; i < count; i++)
				value[i] = bind.Read(stream);
			return value;
		}

		public void WriteValue(object value, ISegment stream)
		{
			Write((UnityEngine.Rect[])value, stream);
		}

		public object ReadValue(ISegment stream)
		{
			return Read(stream);
		}
	}
}
namespace Binding
{
	public struct UnityEngineRectGenericBind : ISerialize<List<UnityEngine.Rect>>, ISerialize
	{
		public void Write(List<UnityEngine.Rect> value, ISegment stream)
		{
			int count = value.Count;
			stream.Write(count);
			if (count == 0) return;
			var bind = new UnityEngineRectBind();
			foreach (var value1 in value)
				bind.Write(value1, stream);
		}

		public List<UnityEngine.Rect> Read(ISegment stream)
		{
			var count = stream.ReadInt32();
			var value = new List<UnityEngine.Rect>(count);
			if (count == 0) return value;
			var bind = new UnityEngineRectBind();
			for (int i = 0; i < count; i++)
				value.Add(bind.Read(stream));
			return value;
		}

		public void WriteValue(object value, ISegment stream)
		{
			Write((List<UnityEngine.Rect>)value, stream);
		}

		public object ReadValue(ISegment stream)
		{
			return Read(stream);
		}
	}
}
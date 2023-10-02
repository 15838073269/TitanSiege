using Net.Serialize;
using Net.System;
using System;
using System.Collections.Generic;

namespace Binding
{
    public readonly struct TitansiegeBagitemDataBind : ISerialize<Titansiege.BagitemData>, ISerialize
    {
        public void Write(Titansiege.BagitemData value, ISegment stream)
        {
            int pos = stream.Position;
            stream.Position += 1;
            var bits = new byte[1];

            if (value.Id != 0)
            {
                NetConvertBase.SetBit(ref bits[0], 1, true);
                stream.Write(value.Id);
            }

            if (value.Cid != 0)
            {
                NetConvertBase.SetBit(ref bits[0], 2, true);
                stream.Write(value.Cid);
            }

            if (!string.IsNullOrEmpty(value.Inbag))
            {
                NetConvertBase.SetBit(ref bits[0], 3, true);
                stream.Write(value.Inbag);
            }

            int pos1 = stream.Position;
            stream.Position = pos;
            stream.Write(bits, 0, 1);
            stream.Position = pos1;
        }
		
        public Titansiege.BagitemData Read(ISegment stream) 
        {
            var value = new Titansiege.BagitemData();
            Read(ref value, stream);
            return value;
        }

		public void Read(ref Titansiege.BagitemData value, ISegment stream)
		{
			var bits = stream.Read(1);

			if(NetConvertBase.GetBit(bits[0], 1))
				value.Id = stream.ReadInt32();

			if(NetConvertBase.GetBit(bits[0], 2))
				value.Cid = stream.ReadInt32();

			if(NetConvertBase.GetBit(bits[0], 3))
				value.Inbag = stream.ReadString();

		}

        public void WriteValue(object value, ISegment stream)
        {
            Write((Titansiege.BagitemData)value, stream);
        }

        public object ReadValue(ISegment stream)
        {
            return Read(stream);
        }
    }
}

namespace Binding
{
	public readonly struct TitansiegeBagitemDataArrayBind : ISerialize<Titansiege.BagitemData[]>, ISerialize
	{
		public void Write(Titansiege.BagitemData[] value, ISegment stream)
		{
			int count = value.Length;
			stream.Write(count);
			if (count == 0) return;
			var bind = new TitansiegeBagitemDataBind();
			foreach (var value1 in value)
				bind.Write(value1, stream);
		}

		public Titansiege.BagitemData[] Read(ISegment stream)
		{
			var count = stream.ReadInt32();
			var value = new Titansiege.BagitemData[count];
			if (count == 0) return value;
			var bind = new TitansiegeBagitemDataBind();
			for (int i = 0; i < count; i++)
				value[i] = bind.Read(stream);
			return value;
		}

		public void WriteValue(object value, ISegment stream)
		{
			Write((Titansiege.BagitemData[])value, stream);
		}

		public object ReadValue(ISegment stream)
		{
			return Read(stream);
		}
	}
}

namespace Binding
{
	public readonly struct SystemCollectionsGenericListTitansiegeBagitemDataBind : ISerialize<System.Collections.Generic.List<Titansiege.BagitemData>>, ISerialize
	{
		public void Write(System.Collections.Generic.List<Titansiege.BagitemData> value, ISegment stream)
		{
			int count = value.Count;
			stream.Write(count);
			if (count == 0) return;
			var bind = new TitansiegeBagitemDataBind();
			foreach (var value1 in value)
				bind.Write(value1, stream);
		}

		public System.Collections.Generic.List<Titansiege.BagitemData> Read(ISegment stream)
		{
			var count = stream.ReadInt32();
			var value = new System.Collections.Generic.List<Titansiege.BagitemData>(count);
			if (count == 0) return value;
			var bind = new TitansiegeBagitemDataBind();
			for (int i = 0; i < count; i++)
				value.Add(bind.Read(stream));
			return value;
		}

		public void WriteValue(object value, ISegment stream)
		{
			Write((System.Collections.Generic.List<Titansiege.BagitemData>)value, stream);
		}

		public object ReadValue(ISegment stream)
		{
			return Read(stream);
		}
	}
}

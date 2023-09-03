using Net.Serialize;
using Net.System;
using System;
using System.Collections.Generic;

namespace Binding
{
    public readonly struct TitansiegeNpcsDataBind : ISerialize<Titansiege.NpcsData>, ISerialize
    {
        public void Write(Titansiege.NpcsData value, ISegment stream)
        {
            int pos = stream.Position;
            stream.Position += 3;
            var bits = new byte[3];

            if (value.ID != 0)
            {
                NetConvertBase.SetBit(ref bits[0], 1, true);
                stream.Write(value.ID);
            }

            if (value.Zhiye != 0)
            {
                NetConvertBase.SetBit(ref bits[0], 2, true);
                stream.Write(value.Zhiye);
            }

            if (value.Level != 0)
            {
                NetConvertBase.SetBit(ref bits[0], 3, true);
                stream.Write(value.Level);
            }

            if (value.Exp != 0)
            {
                NetConvertBase.SetBit(ref bits[0], 4, true);
                stream.Write(value.Exp);
            }

            if (value.Shengming != 0)
            {
                NetConvertBase.SetBit(ref bits[0], 5, true);
                stream.Write(value.Shengming);
            }

            if (value.Fali != 0)
            {
                NetConvertBase.SetBit(ref bits[0], 6, true);
                stream.Write(value.Fali);
            }

            if (value.Tizhi != 0)
            {
                NetConvertBase.SetBit(ref bits[0], 7, true);
                stream.Write(value.Tizhi);
            }

            if (value.Liliang != 0)
            {
                NetConvertBase.SetBit(ref bits[0], 8, true);
                stream.Write(value.Liliang);
            }

            if (value.Minjie != 0)
            {
                NetConvertBase.SetBit(ref bits[1], 1, true);
                stream.Write(value.Minjie);
            }

            if (value.Moli != 0)
            {
                NetConvertBase.SetBit(ref bits[1], 2, true);
                stream.Write(value.Moli);
            }

            if (value.Meili != 0)
            {
                NetConvertBase.SetBit(ref bits[1], 3, true);
                stream.Write(value.Meili);
            }

            if (value.Xingyun != 0)
            {
                NetConvertBase.SetBit(ref bits[1], 4, true);
                stream.Write(value.Xingyun);
            }

            if (value.Jinbi != 0)
            {
                NetConvertBase.SetBit(ref bits[1], 5, true);
                stream.Write(value.Jinbi);
            }

            if (value.Zuanshi != 0)
            {
                NetConvertBase.SetBit(ref bits[1], 6, true);
                stream.Write(value.Zuanshi);
            }

            if (!string.IsNullOrEmpty(value.Skills))
            {
                NetConvertBase.SetBit(ref bits[1], 7, true);
                stream.Write(value.Skills);
            }

            if (!string.IsNullOrEmpty(value.Prefabpath))
            {
                NetConvertBase.SetBit(ref bits[1], 8, true);
                stream.Write(value.Prefabpath);
            }

            if (!string.IsNullOrEmpty(value.Headpath))
            {
                NetConvertBase.SetBit(ref bits[2], 1, true);
                stream.Write(value.Headpath);
            }

            if (!string.IsNullOrEmpty(value.Lihuipath))
            {
                NetConvertBase.SetBit(ref bits[2], 2, true);
                stream.Write(value.Lihuipath);
            }

            if (value.LastDate != default)
            {
                NetConvertBase.SetBit(ref bits[2], 3, true);
                stream.Write(value.LastDate);
            }

            int pos1 = stream.Position;
            stream.Position = pos;
            stream.Write(bits, 0, 3);
            stream.Position = pos1;
        }
		
        public Titansiege.NpcsData Read(ISegment stream) 
        {
            var value = new Titansiege.NpcsData();
            Read(ref value, stream);
            return value;
        }

		public void Read(ref Titansiege.NpcsData value, ISegment stream)
		{
			var bits = stream.Read(3);

			if(NetConvertBase.GetBit(bits[0], 1))
				value.ID = stream.ReadInt64();

			if(NetConvertBase.GetBit(bits[0], 2))
				value.Zhiye = stream.ReadSByte();

			if(NetConvertBase.GetBit(bits[0], 3))
				value.Level = stream.ReadSByte();

			if(NetConvertBase.GetBit(bits[0], 4))
				value.Exp = stream.ReadInt32();

			if(NetConvertBase.GetBit(bits[0], 5))
				value.Shengming = stream.ReadInt32();

			if(NetConvertBase.GetBit(bits[0], 6))
				value.Fali = stream.ReadInt32();

			if(NetConvertBase.GetBit(bits[0], 7))
				value.Tizhi = stream.ReadInt16();

			if(NetConvertBase.GetBit(bits[0], 8))
				value.Liliang = stream.ReadInt16();

			if(NetConvertBase.GetBit(bits[1], 1))
				value.Minjie = stream.ReadInt16();

			if(NetConvertBase.GetBit(bits[1], 2))
				value.Moli = stream.ReadInt16();

			if(NetConvertBase.GetBit(bits[1], 3))
				value.Meili = stream.ReadInt16();

			if(NetConvertBase.GetBit(bits[1], 4))
				value.Xingyun = stream.ReadInt16();

			if(NetConvertBase.GetBit(bits[1], 5))
				value.Jinbi = stream.ReadInt32();

			if(NetConvertBase.GetBit(bits[1], 6))
				value.Zuanshi = stream.ReadInt32();

			if(NetConvertBase.GetBit(bits[1], 7))
				value.Skills = stream.ReadString();

			if(NetConvertBase.GetBit(bits[1], 8))
				value.Prefabpath = stream.ReadString();

			if(NetConvertBase.GetBit(bits[2], 1))
				value.Headpath = stream.ReadString();

			if(NetConvertBase.GetBit(bits[2], 2))
				value.Lihuipath = stream.ReadString();

			if(NetConvertBase.GetBit(bits[2], 3))
				value.LastDate = stream.ReadDateTime();

		}

        public void WriteValue(object value, ISegment stream)
        {
            Write((Titansiege.NpcsData)value, stream);
        }

        public object ReadValue(ISegment stream)
        {
            return Read(stream);
        }
    }
}

namespace Binding
{
	public readonly struct TitansiegeNpcsDataArrayBind : ISerialize<Titansiege.NpcsData[]>, ISerialize
	{
		public void Write(Titansiege.NpcsData[] value, ISegment stream)
		{
			int count = value.Length;
			stream.Write(count);
			if (count == 0) return;
			var bind = new TitansiegeNpcsDataBind();
			foreach (var value1 in value)
				bind.Write(value1, stream);
		}

		public Titansiege.NpcsData[] Read(ISegment stream)
		{
			var count = stream.ReadInt32();
			var value = new Titansiege.NpcsData[count];
			if (count == 0) return value;
			var bind = new TitansiegeNpcsDataBind();
			for (int i = 0; i < count; i++)
				value[i] = bind.Read(stream);
			return value;
		}

		public void WriteValue(object value, ISegment stream)
		{
			Write((Titansiege.NpcsData[])value, stream);
		}

		public object ReadValue(ISegment stream)
		{
			return Read(stream);
		}
	}
}

namespace Binding
{
	public readonly struct SystemCollectionsGenericListTitansiegeNpcsDataBind : ISerialize<System.Collections.Generic.List<Titansiege.NpcsData>>, ISerialize
	{
		public void Write(System.Collections.Generic.List<Titansiege.NpcsData> value, ISegment stream)
		{
			int count = value.Count;
			stream.Write(count);
			if (count == 0) return;
			var bind = new TitansiegeNpcsDataBind();
			foreach (var value1 in value)
				bind.Write(value1, stream);
		}

		public System.Collections.Generic.List<Titansiege.NpcsData> Read(ISegment stream)
		{
			var count = stream.ReadInt32();
			var value = new System.Collections.Generic.List<Titansiege.NpcsData>(count);
			if (count == 0) return value;
			var bind = new TitansiegeNpcsDataBind();
			for (int i = 0; i < count; i++)
				value.Add(bind.Read(stream));
			return value;
		}

		public void WriteValue(object value, ISegment stream)
		{
			Write((System.Collections.Generic.List<Titansiege.NpcsData>)value, stream);
		}

		public object ReadValue(ISegment stream)
		{
			return Read(stream);
		}
	}
}

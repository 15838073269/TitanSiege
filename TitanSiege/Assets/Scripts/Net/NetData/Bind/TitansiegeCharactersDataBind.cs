using Net.Serialize;
using Net.System;
using System;
using System.Collections.Generic;

namespace Binding
{
    public struct TitansiegeCharactersDataBind : ISerialize<Titansiege.CharactersData>, ISerialize
    {
        public void Write(Titansiege.CharactersData value, Segment stream)
        {
            int pos = stream.Position;
            stream.Position += 5;
            byte[] bits = new byte[5];

            if (value.ID != 0)
            {
                NetConvertBase.SetBit(ref bits[0], 1, true);
                stream.Write(value.ID);
            }

            if (!string.IsNullOrEmpty(value.Name))
            {
                NetConvertBase.SetBit(ref bits[0], 2, true);
                stream.Write(value.Name);
            }

            if (value.Zhiye != 0)
            {
                NetConvertBase.SetBit(ref bits[0], 3, true);
                stream.Write(value.Zhiye);
            }

            if (value.Level != 0)
            {
                NetConvertBase.SetBit(ref bits[0], 4, true);
                stream.Write(value.Level);
            }

            if (value.Exp != 0)
            {
                NetConvertBase.SetBit(ref bits[0], 5, true);
                stream.Write(value.Exp);
            }

            if (value.Shengming != 0)
            {
                NetConvertBase.SetBit(ref bits[0], 6, true);
                stream.Write(value.Shengming);
            }

            if (value.Fali != 0)
            {
                NetConvertBase.SetBit(ref bits[0], 7, true);
                stream.Write(value.Fali);
            }

            if (value.Tizhi != 0)
            {
                NetConvertBase.SetBit(ref bits[0], 8, true);
                stream.Write(value.Tizhi);
            }

            if (value.Liliang != 0)
            {
                NetConvertBase.SetBit(ref bits[1], 1, true);
                stream.Write(value.Liliang);
            }

            if (value.Minjie != 0)
            {
                NetConvertBase.SetBit(ref bits[1], 2, true);
                stream.Write(value.Minjie);
            }

            if (value.Moli != 0)
            {
                NetConvertBase.SetBit(ref bits[1], 3, true);
                stream.Write(value.Moli);
            }

            if (value.Meili != 0)
            {
                NetConvertBase.SetBit(ref bits[1], 4, true);
                stream.Write(value.Meili);
            }

            if (value.Xingyun != 0)
            {
                NetConvertBase.SetBit(ref bits[1], 5, true);
                stream.Write(value.Xingyun);
            }

            if (value.Lianjin != 0)
            {
                NetConvertBase.SetBit(ref bits[1], 6, true);
                stream.Write(value.Lianjin);
            }

            if (value.Duanzao != 0)
            {
                NetConvertBase.SetBit(ref bits[1], 7, true);
                stream.Write(value.Duanzao);
            }

            if (value.Jinbi != 0)
            {
                NetConvertBase.SetBit(ref bits[1], 8, true);
                stream.Write(value.Jinbi);
            }

            if (value.Zuanshi != 0)
            {
                NetConvertBase.SetBit(ref bits[2], 1, true);
                stream.Write(value.Zuanshi);
            }

            if (!string.IsNullOrEmpty(value.Chenghao))
            {
                NetConvertBase.SetBit(ref bits[2], 2, true);
                stream.Write(value.Chenghao);
            }

            if (!string.IsNullOrEmpty(value.Friends))
            {
                NetConvertBase.SetBit(ref bits[2], 3, true);
                stream.Write(value.Friends);
            }

            if (!string.IsNullOrEmpty(value.Skills))
            {
                NetConvertBase.SetBit(ref bits[2], 4, true);
                stream.Write(value.Skills);
            }

            if (!string.IsNullOrEmpty(value.Prefabpath))
            {
                NetConvertBase.SetBit(ref bits[2], 5, true);
                stream.Write(value.Prefabpath);
            }

            if (!string.IsNullOrEmpty(value.Headpath))
            {
                NetConvertBase.SetBit(ref bits[2], 6, true);
                stream.Write(value.Headpath);
            }

            if (!string.IsNullOrEmpty(value.Lihuipath))
            {
                NetConvertBase.SetBit(ref bits[2], 7, true);
                stream.Write(value.Lihuipath);
            }

            if (value.Wuqi != 0)
            {
                NetConvertBase.SetBit(ref bits[2], 8, true);
                stream.Write(value.Wuqi);
            }

            if (value.Toukui != 0)
            {
                NetConvertBase.SetBit(ref bits[3], 1, true);
                stream.Write(value.Toukui);
            }

            if (value.Yifu != 0)
            {
                NetConvertBase.SetBit(ref bits[3], 2, true);
                stream.Write(value.Yifu);
            }

            if (value.Xiezi != 0)
            {
                NetConvertBase.SetBit(ref bits[3], 3, true);
                stream.Write(value.Xiezi);
            }

            if (value.MapID != 0)
            {
                NetConvertBase.SetBit(ref bits[3], 4, true);
                stream.Write(value.MapID);
            }

            if (value.MapPosX != 0)
            {
                NetConvertBase.SetBit(ref bits[3], 5, true);
                stream.Write(value.MapPosX);
            }

            if (value.MapPosY != 0)
            {
                NetConvertBase.SetBit(ref bits[3], 6, true);
                stream.Write(value.MapPosY);
            }

            if (value.MapPosZ != 0)
            {
                NetConvertBase.SetBit(ref bits[3], 7, true);
                stream.Write(value.MapPosZ);
            }

            if (value.Uid != 0)
            {
                NetConvertBase.SetBit(ref bits[3], 8, true);
                stream.Write(value.Uid);
            }

            if (value.LastDate != default)
            {
                NetConvertBase.SetBit(ref bits[4], 1, true);
                stream.Write(value.LastDate);
            }

            if (value.DelRole != false)
            {
                NetConvertBase.SetBit(ref bits[4], 2, true);
                stream.Write(value.DelRole);
            }

            int pos1 = stream.Position;
            stream.Position = pos;
            stream.Write(bits, 0, 5);
            stream.Position = pos1;
        }
		
		public Titansiege.CharactersData Read(Segment stream)
		{
			byte[] bits = stream.Read(5);
			var value = new Titansiege.CharactersData();

			if(NetConvertBase.GetBit(bits[0], 1))
				value.ID = stream.ReadInt64();

			if(NetConvertBase.GetBit(bits[0], 2))
				value.Name = stream.ReadString();

			if(NetConvertBase.GetBit(bits[0], 3))
				value.Zhiye = stream.ReadSByte();

			if(NetConvertBase.GetBit(bits[0], 4))
				value.Level = stream.ReadSByte();

			if(NetConvertBase.GetBit(bits[0], 5))
				value.Exp = stream.ReadInt32();

			if(NetConvertBase.GetBit(bits[0], 6))
				value.Shengming = stream.ReadInt32();

			if(NetConvertBase.GetBit(bits[0], 7))
				value.Fali = stream.ReadInt32();

			if(NetConvertBase.GetBit(bits[0], 8))
				value.Tizhi = stream.ReadInt16();

			if(NetConvertBase.GetBit(bits[1], 1))
				value.Liliang = stream.ReadInt16();

			if(NetConvertBase.GetBit(bits[1], 2))
				value.Minjie = stream.ReadInt16();

			if(NetConvertBase.GetBit(bits[1], 3))
				value.Moli = stream.ReadInt16();

			if(NetConvertBase.GetBit(bits[1], 4))
				value.Meili = stream.ReadInt16();

			if(NetConvertBase.GetBit(bits[1], 5))
				value.Xingyun = stream.ReadInt16();

			if(NetConvertBase.GetBit(bits[1], 6))
				value.Lianjin = stream.ReadInt16();

			if(NetConvertBase.GetBit(bits[1], 7))
				value.Duanzao = stream.ReadInt16();

			if(NetConvertBase.GetBit(bits[1], 8))
				value.Jinbi = stream.ReadInt32();

			if(NetConvertBase.GetBit(bits[2], 1))
				value.Zuanshi = stream.ReadInt32();

			if(NetConvertBase.GetBit(bits[2], 2))
				value.Chenghao = stream.ReadString();

			if(NetConvertBase.GetBit(bits[2], 3))
				value.Friends = stream.ReadString();

			if(NetConvertBase.GetBit(bits[2], 4))
				value.Skills = stream.ReadString();

			if(NetConvertBase.GetBit(bits[2], 5))
				value.Prefabpath = stream.ReadString();

			if(NetConvertBase.GetBit(bits[2], 6))
				value.Headpath = stream.ReadString();

			if(NetConvertBase.GetBit(bits[2], 7))
				value.Lihuipath = stream.ReadString();

			if(NetConvertBase.GetBit(bits[2], 8))
				value.Wuqi = stream.ReadInt16();

			if(NetConvertBase.GetBit(bits[3], 1))
				value.Toukui = stream.ReadInt16();

			if(NetConvertBase.GetBit(bits[3], 2))
				value.Yifu = stream.ReadInt16();

			if(NetConvertBase.GetBit(bits[3], 3))
				value.Xiezi = stream.ReadInt16();

			if(NetConvertBase.GetBit(bits[3], 4))
				value.MapID = stream.ReadInt32();

			if(NetConvertBase.GetBit(bits[3], 5))
				value.MapPosX = stream.ReadInt32();

			if(NetConvertBase.GetBit(bits[3], 6))
				value.MapPosY = stream.ReadInt32();

			if(NetConvertBase.GetBit(bits[3], 7))
				value.MapPosZ = stream.ReadInt32();

			if(NetConvertBase.GetBit(bits[3], 8))
				value.Uid = stream.ReadInt64();

			if(NetConvertBase.GetBit(bits[4], 1))
				value.LastDate = stream.ReadDateTime();

			if(NetConvertBase.GetBit(bits[4], 2))
				value.DelRole = stream.ReadBoolean();

			return value;
		}

        public void WriteValue(object value, Segment stream)
        {
            Write((Titansiege.CharactersData)value, stream);
        }

        public object ReadValue(Segment stream)
        {
            return Read(stream);
        }
    }
}

namespace Binding
{
	public struct TitansiegeCharactersDataArrayBind : ISerialize<Titansiege.CharactersData[]>, ISerialize
	{
		public void Write(Titansiege.CharactersData[] value, Segment stream)
		{
			int count = value.Length;
			stream.Write(count);
			if (count == 0) return;
			var bind = new TitansiegeCharactersDataBind();
			foreach (var value1 in value)
				bind.Write(value1, stream);
		}

		public Titansiege.CharactersData[] Read(Segment stream)
		{
			var count = stream.ReadInt32();
			var value = new Titansiege.CharactersData[count];
			if (count == 0) return value;
			var bind = new TitansiegeCharactersDataBind();
			for (int i = 0; i < count; i++)
				value[i] = bind.Read(stream);
			return value;
		}

		public void WriteValue(object value, Segment stream)
		{
			Write((Titansiege.CharactersData[])value, stream);
		}

		public object ReadValue(Segment stream)
		{
			return Read(stream);
		}
	}
}
namespace Binding
{
	public struct TitansiegeCharactersDataGenericBind : ISerialize<List<Titansiege.CharactersData>>, ISerialize
	{
		public void Write(List<Titansiege.CharactersData> value, Segment stream)
		{
			int count = value.Count;
			stream.Write(count);
			if (count == 0) return;
			var bind = new TitansiegeCharactersDataBind();
			foreach (var value1 in value)
				bind.Write(value1, stream);
		}

		public List<Titansiege.CharactersData> Read(Segment stream)
		{
			var count = stream.ReadInt32();
			var value = new List<Titansiege.CharactersData>(count);
			if (count == 0) return value;
			var bind = new TitansiegeCharactersDataBind();
			for (int i = 0; i < count; i++)
				value.Add(bind.Read(stream));
			return value;
		}

		public void WriteValue(object value, Segment stream)
		{
			Write((List<Titansiege.CharactersData>)value, stream);
		}

		public object ReadValue(Segment stream)
		{
			return Read(stream);
		}
	}
}
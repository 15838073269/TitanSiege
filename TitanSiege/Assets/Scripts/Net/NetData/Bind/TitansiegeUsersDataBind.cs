using Net.Serialize;
using Net.System;
using System;
using System.Collections.Generic;

namespace Binding
{
    public struct TitansiegeUsersDataBind : ISerialize<Titansiege.UsersData>, ISerialize
    {
        public void Write(Titansiege.UsersData value, Segment stream)
        {
            int pos = stream.Position;
            stream.Position += 1;
            byte[] bits = new byte[1];

            if (value.ID != 0)
            {
                NetConvertBase.SetBit(ref bits[0], 1, true);
                stream.Write(value.ID);
            }

            if (!string.IsNullOrEmpty(value.Username))
            {
                NetConvertBase.SetBit(ref bits[0], 2, true);
                stream.Write(value.Username);
            }

            if (!string.IsNullOrEmpty(value.Password))
            {
                NetConvertBase.SetBit(ref bits[0], 3, true);
                stream.Write(value.Password);
            }

            if (value.RegisterDate != default)
            {
                NetConvertBase.SetBit(ref bits[0], 4, true);
                stream.Write(value.RegisterDate);
            }

            if (!string.IsNullOrEmpty(value.Email))
            {
                NetConvertBase.SetBit(ref bits[0], 5, true);
                stream.Write(value.Email);
            }

            int pos1 = stream.Position;
            stream.Position = pos;
            stream.Write(bits, 0, 1);
            stream.Position = pos1;
        }
		
		public Titansiege.UsersData Read(Segment stream)
		{
			byte[] bits = stream.Read(1);
			var value = new Titansiege.UsersData();

			if(NetConvertBase.GetBit(bits[0], 1))
				value.ID = stream.ReadInt64();

			if(NetConvertBase.GetBit(bits[0], 2))
				value.Username = stream.ReadString();

			if(NetConvertBase.GetBit(bits[0], 3))
				value.Password = stream.ReadString();

			if(NetConvertBase.GetBit(bits[0], 4))
				value.RegisterDate = stream.ReadDateTime();

			if(NetConvertBase.GetBit(bits[0], 5))
				value.Email = stream.ReadString();

			return value;
		}

        public void WriteValue(object value, Segment stream)
        {
            Write((Titansiege.UsersData)value, stream);
        }

        public object ReadValue(Segment stream)
        {
            return Read(stream);
        }
    }
}

namespace Binding
{
	public struct TitansiegeUsersDataArrayBind : ISerialize<Titansiege.UsersData[]>, ISerialize
	{
		public void Write(Titansiege.UsersData[] value, Segment stream)
		{
			int count = value.Length;
			stream.Write(count);
			if (count == 0) return;
			var bind = new TitansiegeUsersDataBind();
			foreach (var value1 in value)
				bind.Write(value1, stream);
		}

		public Titansiege.UsersData[] Read(Segment stream)
		{
			var count = stream.ReadInt32();
			var value = new Titansiege.UsersData[count];
			if (count == 0) return value;
			var bind = new TitansiegeUsersDataBind();
			for (int i = 0; i < count; i++)
				value[i] = bind.Read(stream);
			return value;
		}

		public void WriteValue(object value, Segment stream)
		{
			Write((Titansiege.UsersData[])value, stream);
		}

		public object ReadValue(Segment stream)
		{
			return Read(stream);
		}
	}
}
namespace Binding
{
	public struct TitansiegeUsersDataGenericBind : ISerialize<List<Titansiege.UsersData>>, ISerialize
	{
		public void Write(List<Titansiege.UsersData> value, Segment stream)
		{
			int count = value.Count;
			stream.Write(count);
			if (count == 0) return;
			var bind = new TitansiegeUsersDataBind();
			foreach (var value1 in value)
				bind.Write(value1, stream);
		}

		public List<Titansiege.UsersData> Read(Segment stream)
		{
			var count = stream.ReadInt32();
			var value = new List<Titansiege.UsersData>(count);
			if (count == 0) return value;
			var bind = new TitansiegeUsersDataBind();
			for (int i = 0; i < count; i++)
				value.Add(bind.Read(stream));
			return value;
		}

		public void WriteValue(object value, Segment stream)
		{
			Write((List<Titansiege.UsersData>)value, stream);
		}

		public object ReadValue(Segment stream)
		{
			return Read(stream);
		}
	}
}
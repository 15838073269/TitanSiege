using Net.Serialize;
using Net.System;
using System;
using System.Collections.Generic;

namespace Binding
{
    public readonly struct FightPropBind : ISerialize<FightProp>, ISerialize
    {
        public void Write(FightProp value, ISegment stream)
        {
            int pos = stream.Position;
            stream.Position += 2;
            var bits = new byte[2];

            if (value.Attack != 0)
            {
                NetConvertBase.SetBit(ref bits[0], 1, true);
                stream.Write(value.Attack);
            }

            if (value.Defense != 0)
            {
                NetConvertBase.SetBit(ref bits[0], 2, true);
                stream.Write(value.Defense);
            }

            if (value.Dodge != 0)
            {
                NetConvertBase.SetBit(ref bits[0], 3, true);
                stream.Write(value.Dodge);
            }

            if (value.Crit != 0)
            {
                NetConvertBase.SetBit(ref bits[0], 4, true);
                stream.Write(value.Crit);
            }

            if (value.FightHP != 0)
            {
                NetConvertBase.SetBit(ref bits[0], 5, true);
                stream.Write(value.FightHP);
            }

            if (value.FightMaxHp != 0)
            {
                NetConvertBase.SetBit(ref bits[0], 6, true);
                stream.Write(value.FightMaxHp);
            }

            if (value.FightMagic != 0)
            {
                NetConvertBase.SetBit(ref bits[0], 7, true);
                stream.Write(value.FightMagic);
            }

            if (value.FightMaxMagic != 0)
            {
                NetConvertBase.SetBit(ref bits[0], 8, true);
                stream.Write(value.FightMaxMagic);
            }

            if (value.BaseDodge != 0)
            {
                NetConvertBase.SetBit(ref bits[1], 1, true);
                stream.Write(value.BaseDodge);
            }

            if (value.BaseCrit != 0)
            {
                NetConvertBase.SetBit(ref bits[1], 2, true);
                stream.Write(value.BaseCrit);
            }

            if (!string.IsNullOrEmpty(value.PlayerName))
            {
                NetConvertBase.SetBit(ref bits[1], 3, true);
                stream.Write(value.PlayerName);
            }

            int pos1 = stream.Position;
            stream.Position = pos;
            stream.Write(bits, 0, 2);
            stream.Position = pos1;
        }
		
        public FightProp Read(ISegment stream) 
        {
            var value = new FightProp();
            Read(ref value, stream);
            return value;
        }

		public void Read(ref FightProp value, ISegment stream)
		{
			var bits = stream.Read(2);

			if(NetConvertBase.GetBit(bits[0], 1))
				value.Attack = stream.ReadInt32();

			if(NetConvertBase.GetBit(bits[0], 2))
				value.Defense = stream.ReadInt32();

			if(NetConvertBase.GetBit(bits[0], 3))
				value.Dodge = stream.ReadSingle();

			if(NetConvertBase.GetBit(bits[0], 4))
				value.Crit = stream.ReadSingle();

			if(NetConvertBase.GetBit(bits[0], 5))
				value.FightHP = stream.ReadInt32();

			if(NetConvertBase.GetBit(bits[0], 6))
				value.FightMaxHp = stream.ReadInt32();

			if(NetConvertBase.GetBit(bits[0], 7))
				value.FightMagic = stream.ReadInt32();

			if(NetConvertBase.GetBit(bits[0], 8))
				value.FightMaxMagic = stream.ReadInt32();

			if(NetConvertBase.GetBit(bits[1], 1))
				value.BaseDodge = stream.ReadSingle();

			if(NetConvertBase.GetBit(bits[1], 2))
				value.BaseCrit = stream.ReadSingle();

			if(NetConvertBase.GetBit(bits[1], 3))
				value.PlayerName = stream.ReadString();

		}

        public void WriteValue(object value, ISegment stream)
        {
            Write((FightProp)value, stream);
        }

        public object ReadValue(ISegment stream)
        {
            return Read(stream);
        }
    }
}

namespace Binding
{
	public readonly struct FightPropArrayBind : ISerialize<FightProp[]>, ISerialize
	{
		public void Write(FightProp[] value, ISegment stream)
		{
			int count = value.Length;
			stream.Write(count);
			if (count == 0) return;
			var bind = new FightPropBind();
			foreach (var value1 in value)
				bind.Write(value1, stream);
		}

		public FightProp[] Read(ISegment stream)
		{
			var count = stream.ReadInt32();
			var value = new FightProp[count];
			if (count == 0) return value;
			var bind = new FightPropBind();
			for (int i = 0; i < count; i++)
				value[i] = bind.Read(stream);
			return value;
		}

		public void WriteValue(object value, ISegment stream)
		{
			Write((FightProp[])value, stream);
		}

		public object ReadValue(ISegment stream)
		{
			return Read(stream);
		}
	}
}

namespace Binding
{
	public readonly struct SystemCollectionsGenericListFightPropBind : ISerialize<System.Collections.Generic.List<FightProp>>, ISerialize
	{
		public void Write(System.Collections.Generic.List<FightProp> value, ISegment stream)
		{
			int count = value.Count;
			stream.Write(count);
			if (count == 0) return;
			var bind = new FightPropBind();
			foreach (var value1 in value)
				bind.Write(value1, stream);
		}

		public System.Collections.Generic.List<FightProp> Read(ISegment stream)
		{
			var count = stream.ReadInt32();
			var value = new System.Collections.Generic.List<FightProp>(count);
			if (count == 0) return value;
			var bind = new FightPropBind();
			for (int i = 0; i < count; i++)
				value.Add(bind.Read(stream));
			return value;
		}

		public void WriteValue(object value, ISegment stream)
		{
			Write((System.Collections.Generic.List<FightProp>)value, stream);
		}

		public object ReadValue(ISegment stream)
		{
			return Read(stream);
		}
	}
}

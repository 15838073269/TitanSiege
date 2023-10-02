using Binding;
using Net.System;

public static class BindingExtension
{

    public static ISegment SerializeObject(this Titansiege.UsersData value)
    {
        var segment = BufferPool.Take();
        var bind = new TitansiegeUsersDataBind();
        bind.Write(value, segment);
        return segment;
    }

    public static Titansiege.UsersData DeserializeObject(this Titansiege.UsersData value, ISegment segment, bool isPush = true)
    {
        var bind = new TitansiegeUsersDataBind();
        bind.Read(ref value, segment);
        if (isPush) BufferPool.Push(segment);
        return value;
    }

    public static ISegment SerializeObject(this Titansiege.NpcsData value)
    {
        var segment = BufferPool.Take();
        var bind = new TitansiegeNpcsDataBind();
        bind.Write(value, segment);
        return segment;
    }

    public static Titansiege.NpcsData DeserializeObject(this Titansiege.NpcsData value, ISegment segment, bool isPush = true)
    {
        var bind = new TitansiegeNpcsDataBind();
        bind.Read(ref value, segment);
        if (isPush) BufferPool.Push(segment);
        return value;
    }

    public static ISegment SerializeObject(this FightProp value)
    {
        var segment = BufferPool.Take();
        var bind = new FightPropBind();
        bind.Write(value, segment);
        return segment;
    }

    public static FightProp DeserializeObject(this FightProp value, ISegment segment, bool isPush = true)
    {
        var bind = new FightPropBind();
        bind.Read(ref value, segment);
        if (isPush) BufferPool.Push(segment);
        return value;
    }

    public static ISegment SerializeObject(this Titansiege.CharactersData value)
    {
        var segment = BufferPool.Take();
        var bind = new TitansiegeCharactersDataBind();
        bind.Write(value, segment);
        return segment;
    }

    public static Titansiege.CharactersData DeserializeObject(this Titansiege.CharactersData value, ISegment segment, bool isPush = true)
    {
        var bind = new TitansiegeCharactersDataBind();
        bind.Read(ref value, segment);
        if (isPush) BufferPool.Push(segment);
        return value;
    }

    public static ISegment SerializeObject(this Titansiege.BagitemData value)
    {
        var segment = BufferPool.Take();
        var bind = new TitansiegeBagitemDataBind();
        bind.Write(value, segment);
        return segment;
    }

    public static Titansiege.BagitemData DeserializeObject(this Titansiege.BagitemData value, ISegment segment, bool isPush = true)
    {
        var bind = new TitansiegeBagitemDataBind();
        bind.Read(ref value, segment);
        if (isPush) BufferPool.Push(segment);
        return value;
    }

}
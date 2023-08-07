using Net.Serialize;
using Net.System;

public static class BindingExtension
{
    public static ISegment SerializeObject(this object value) => NetConvertFast2.SerializeObject(value);

    public static object DeserializeObject(this object value, ISegment segment, bool isPush = true)
    {
        return NetConvertFast2.DeserializeObject(value.GetType(), segment, isPush);
    }
}
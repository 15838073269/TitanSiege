using System;

public static class ObjectConverter
{
    public static bool AsBool(object self)
    {
        var str = self.ToString();
        if (!bool.TryParse(str, out var value))
            return str != "0";
        return value;
    }

    public static byte AsByte(object self)
    {
        var str = self.ToString();
        byte.TryParse(str, out var value);
        return value;
    }

    public static sbyte AsSbyte(object self)
    {
        var str = self.ToString();
        sbyte.TryParse(str, out var value);
        return value;
    }

    public static short AsShort(object self)
    {
        var str = self.ToString();
        short.TryParse(str, out var value);
        return value;
    }

    public static ushort AsUshort(object self)
    {
        var str = self.ToString();
        ushort.TryParse(str, out var value);
        return value;
    }

    public static char AsChar(object self)
    {
        var str = self.ToString();
        char.TryParse(str, out var value);
        return value;
    }

    public static int AsInt(object self)
    {
        var str = self.ToString();
        int.TryParse(str, out var value);
        return value;
    }

    public static uint AsUint(object self)
    {
        var str = self.ToString();
        uint.TryParse(str, out var value);
        return value;
    }

    public static float AsFloat(object self)
    {
        var str = self.ToString();
        float.TryParse(str, out var value);
        return value;
    }

    public static long AsLong(object self)
    {
        var str = self.ToString();
        long.TryParse(str, out var value);
        return value;
    }

    public static ulong AsUlong(object self)
    {
        var str = self.ToString();
        ulong.TryParse(str, out var value);
        return value;
    }

    public static double AsDouble(object self)
    {
        var str = self.ToString();
        double.TryParse(str, out var value);
        return value;
    }

    public static decimal AsDecimal(object self)
    {
        var str = self.ToString();
        decimal.TryParse(str, out var value);
        return value;
    }

    public static DateTime AsDateTime(object self)
    {
        var str = self.ToString();
        DateTime.TryParse(str, out var value);
        return value;
    }

    public static string AsString(object self)
    {
        var str = self.ToString();
        return str;
    }
}
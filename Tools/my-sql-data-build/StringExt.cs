public static class StringExt
{
    /// <summary>
    /// 第一个字符为大写
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static unsafe string ToUpperFirst(this string str)
    {
        if (str == null) return null;
        string ret = string.Copy(str);
        fixed (char* ptr = ret)
            *ptr = char.ToUpper(*ptr);
        return ret;
    }

    public static unsafe string ToUpperAll(this string str)
    {
        if (str == null) return null;
        string ret = "";
        foreach (var cha in str)
            ret += char.ToUpper(cha);
        return ret;
    }

    /// <summary>
    /// 第一个字符为小写
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static unsafe string ToLowerFirst(this string str)
    {
        if (str == null) return null;
        string ret = string.Copy(str);
        fixed (char* ptr = ret)
            *ptr = char.ToLower(*ptr);
        return ret;
    }
}
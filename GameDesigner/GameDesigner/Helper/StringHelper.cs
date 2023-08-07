using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Net.Helper
{
    public static class StringHelper
    {
        /// <summary>
        /// 检查sql的字符串类型的值合法性
        /// </summary>
        /// <param name="value"></param>
        /// <param name="length"></param>
        public static void CheckSqlString(ref string value, int length)
        {
            value = value.Replace("\\", "\\\\"); //如果包含\必须转义, 否则出现 \'时就会出错
            value = value.Replace("'", "\\\'"); //出现'时必须转转义成\'
            if (value.Length >= length)
                value = value.Substring(0, length);
        }

        /// <summary>
        /// 查找一个字符在text出现了几次
        /// </summary>
        /// <param name="text"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int FindHitCount(string text, char value)
        {
            int count = 0;
            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] == value)
                    count++;
            }
            return count;
        }

        /// <summary>
        /// 一个字符在text出现的第hitcount次后被移除
        /// </summary>
        /// <param name="text"></param>
        /// <param name="value"></param>
        /// <param name="hitCount"></param>
        public static void RemoveHit(ref string text, char value, int hitCount)
        {
            int count = 0;
            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] == value)
                {
                    if (count == hitCount)
                    {
                        text = text.Remove(i);
                        break;
                    }
                    count++;
                }
            }
        }

        public static int IndexOf(List<char> chars, string text)
        {
            for (int i = 0; i < chars.Count; i++)
            {
                if (chars[i] == text[0])
                {
                    int index = i + 1;
                    int index1 = 1;
                    while (index < chars.Count & index1 < text.Length)
                    {
                        if (chars[index] != text[index1])
                            goto J;
                    }
                    return i;
                }
            J:;
            }
            return -1;
        }

        public static void Remove(List<char> chars, int index, int count)
        {
            chars.RemoveRange(index, count);
        }

        public static string ToString(List<char> chars)
        {
            var stringBuilder = new StringBuilder();
            for (int i = 0; i < chars.Count; i++)
                stringBuilder.Append(chars[i]);
            return stringBuilder.ToString();
        }

        public static List<char> Substring(List<char> chars, int index, int count)
        {
            return chars.GetRange(index, count);
        }

        public static string ReplaceClear(this string self, params string[] pars)
        {
            foreach (var item in pars)
                self = self.Replace(item, "");
            return self;
        }
    }
}
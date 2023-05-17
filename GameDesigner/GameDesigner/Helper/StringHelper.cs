using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Net.Helper
{
    public class StringHelper
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
    }
}
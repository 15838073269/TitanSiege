using System.Collections.Generic;

namespace Net.Helper
{
    internal class FilterText
    {
        internal bool wordEnd;
        internal Dictionary<char, FilterText> wordDic = new Dictionary<char, FilterText>();
    }

    /// <summary>
    /// 过滤文字帮助类
    /// </summary>
    public class FilterTextHelper
    {
        private static readonly FilterText filter = new FilterText();

        /// <summary>
        /// 初始化要过滤的所有文本内容
        /// </summary>
        /// <param name="filterData"></param>
        public static void Init(string[] filterData)
        {
            foreach (var text in filterData)
            {
                AddText(text);
            }
        }

        /// <summary>
        /// 动态插入过滤文本
        /// </summary>
        /// <param name="text"></param>
        public static void AddText(string text) 
        {
            if (string.IsNullOrEmpty(text))
                return;
            FilterFor(filter, text, 0);
        }

        private static void FilterFor(FilterText filterN, string text, int index)
        {
            if (!filterN.wordDic.TryGetValue(text[index], out var filter1))
                filterN.wordDic[text[index]] = filter1 = new FilterText();
            if (index + 1 < text.Length)
                FilterFor(filter1, text, index + 1);
            else
                filter1.wordEnd = true;
        }

        private static void SearchFilterText(FilterText filterN, string text, int index, List<int> containList, ref int getCount)
        {
            for (int i = index; i < text.Length; i++)
            {
                var word = text[i];
                if (filterN.wordDic.TryGetValue(word, out var filter1))
                {
                    containList.Add(i);
                    if (filter1.wordDic.Count == 0 | filter1.wordEnd)
                    {
                        getCount++;
                        continue;//解决多次复制 敏感字, 只过滤其中一部分的问题
                    }
                    if (i + 1 < text.Length)
                    {
                        SearchFilterText(filter1, text, i + 1, containList, ref getCount);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// 过滤文字
        /// </summary>
        /// <param name="text">原文本</param>
        /// <param name="containCount">如果出现多少个字以上则进行过滤处理, 默认0是完全出现才过滤</param>
        /// <param name="filterLen">总过滤的字符数</param>
        /// <returns>过滤后的文字, 过滤的文字以*代替</returns>
        public static string FilterText(string text, int containCount, out int filterLen)
        {
            var containList = new List<int>();
            int count = text.Length;
            for (int i = 0; i < count; i++)
            {
                var word = text[i];
                if (filter.wordDic.ContainsKey(word))
                {
                    int get = 0;
                    var containList1 = new List<int>();
                    SearchFilterText(filter, text, i, containList1, ref get);
                    if (get > 0 | (containList1.Count >= containCount & containCount != 0))
                    {
                        containList.AddRange(containList1);
                        break;//如果已经包含敏感词,就没必要检测下个单词了
                    }
                }
            }
            filterLen = containList.Count;
            foreach (var wordIndex in containList)
            {
                text = text.Replace(text[wordIndex], '*');
            }
            return text;
        }

        /// <summary>
        /// 检查是否包含过滤文字
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static bool ContainsFilterText(string text)
        {
            int count = text.Length;
            for (int i = 0; i < count; i++)
            {
                var word = text[i];
                if (filter.wordDic.ContainsKey(word))
                {
                    int get = 0;
                    var containList1 = new List<int>();
                    SearchFilterText(filter, text, i, containList1, ref get);
                    if (get > 0)
                        return true;
                }
            }
            return false;
        }
    }
}

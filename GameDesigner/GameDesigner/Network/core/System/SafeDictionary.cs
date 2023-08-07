namespace Net.System
{
    /// <summary>
    /// 安全字典, 无GC快速字典
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class SafeDictionary<TKey, TValue> : MyDictionary<TKey, TValue>
    {
        protected override bool Insert(TKey key, TValue value, bool tryAdd, out TValue oldValue)
        {
            lock (this)
            {
                return base.Insert(key, value, tryAdd, out oldValue);
            }
        }

        public override bool TryRemove(TKey key, out TValue value)
        {
            lock (this)
            {
                return base.TryRemove(key, out value);
            }
        }
    }
}

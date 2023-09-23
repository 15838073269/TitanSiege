using System;

namespace Net.System 
{
    public class HashSetSafe<T> : FastHashSet<T> 
    {
        private readonly object SyncRoot = new object();

        public override bool Add(T item)
        {
            lock (SyncRoot) return base.Add(item);
        }

        public override bool Remove(T item)
        {
            lock (SyncRoot) return base.Remove(item);
        }

        public override void Clear()
        {
            lock (SyncRoot) base.Clear();
        }

        public override int RemoveWhere(Predicate<T> match)
        {
            lock (SyncRoot) return base.RemoveWhere(match);
        }
    }
}
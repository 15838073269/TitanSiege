
using System;
using System.Collections.Generic;

namespace GF.Utils
{ 
    public class DictionarySafe<TKey, TValue> : Dictionary<TKey, TValue> 
    {
        public new TValue this[TKey key]
        {
            set { base[key] = value; }
            get
            {
                TValue value = default(TValue);
                TryGetValue(key, out value);
                return value;
            }
        }


    }
}

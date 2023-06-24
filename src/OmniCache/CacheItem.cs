using System;
namespace OmniCache
{
    public class CacheItem<T> where T : class
    {
        public T Value { get; set; }

        public CacheItem(T val)
        {
            Value = val;
        }

        public CacheItem()
        {
            Value = null;
        }
    }
}


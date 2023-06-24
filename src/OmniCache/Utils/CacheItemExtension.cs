using System;
namespace OmniCache.Utils
{
	public static class CacheItemExtension
	{
        public static List<T> Unbox<T>(this List<CacheItem<T>> list) where T : class
        {
            List<T> ret = new List<T>();
            for (int i = 0; i < list.Count; i++)
            {
                CacheItem<T> cacheObj = list[i];
                if (cacheObj != null)
                {
                    ret.Add(cacheObj.Value);
                }
                else
                {
                    ret.Add(null);
                }
            }
            return ret;
        }
    }
}


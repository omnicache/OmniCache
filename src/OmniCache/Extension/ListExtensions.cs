using System;
using System.Linq.Expressions;
using System.Reflection;

namespace OmniCache.Extension
{
	public static class ListExtensions
	{
      
        public static List<U> GetList<T, U>(this List<T> movies, Func<T, U> selector)
        {
            return movies.Select(selector).ToList();
        }

    }
}


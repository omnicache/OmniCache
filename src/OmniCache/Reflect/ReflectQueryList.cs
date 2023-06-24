using System;
using System.Reflection;

namespace OmniCache.Reflect
{
	public class ReflectQueryList
    {
        public List<ReflectQuery> Queries = new List<ReflectQuery>();
        private Type Type;

        public ReflectQueryList()
        {
        }

        public ReflectQuery FindQuery<T>(Query<T> query)
        {
            return Queries.Find(q => q.Query == query);
        }


    }
}


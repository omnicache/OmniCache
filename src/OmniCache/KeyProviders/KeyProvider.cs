using System;
using OmniCache.QueryExpression.Utils;
using OmniCache.Reflect;

namespace OmniCache.KeyProviders
{
	public class KeyProvider
	{

		public KeyProvider()
		{
		}

        public string GetCacheKeyFromKey<T>(object key)
        {
            ReflectClass rc = ReflectStorage.GetClass(typeof(T), true);

            string cacheKey = rc.Name + "(" + typeof(T).Namespace + ")_" + rc.KeyField.Name + "_" + key;

            return cacheKey;
        }

        public string GetCacheKey<T>(object obj)
        {
            ReflectClass rc = ReflectStorage.GetClass(typeof(T));
            object key = rc.KeyField.GetValue(obj);

            string cacheKey = rc.Name + "("+ typeof(T).Namespace + ")_" + rc.KeyField.Name + "_" + key;
            return cacheKey;
        }

        public string GetCacheKey<T>(Query<T> query, object[] queryParams)
        {
            ReflectClass rc = ReflectStorage.GetClass(typeof(T), true);
            ReflectQuery reflectQuery = rc.Queries.FindQuery(query);

            if(reflectQuery==null)
            {
                throw new Exception($"Query not found:{query.ToString()}. Ensure class {typeof(T).Name} has been loaded");
            }
            string cacheKey = rc.Name + "_" + reflectQuery.ClassFoundIn + "_" + reflectQuery.FieldName;// + "_" + rc.KeyField.Name;

            if(query._HasGreaterLessThanOp)
            {
                cacheKey += OmniCacheConstants.KEY_HASH_DELIM;            //signifies that has <> operators, and will need to use HashMap
            }
            if(queryParams!=null)
            {
                cacheKey += QueryHashParamUtils.GetHashKeyFromQueryParams(queryParams);                
            }
            return cacheKey;
        }

        public string GetCacheKeyFromGreaterThanLessThanOp<T>(Query<T> query, string itemKey)
        {
            ReflectClass rc = ReflectStorage.GetClass(typeof(T), true);
            ReflectQuery reflectQuery = rc.Queries.FindQuery(query);

            if (reflectQuery == null)
            {
                throw new Exception($"Query not found:{query.ToString()}. Ensure class {typeof(T).Name} has been loaded");
            }

            string cacheKey = rc.Name + "_" + reflectQuery.ClassFoundIn + "_" + reflectQuery.FieldName;// + "_" + rc.KeyField.Name;
            cacheKey += OmniCacheConstants.KEY_HASH_DELIM;
            cacheKey += itemKey;
            return cacheKey;
        }
    }
}


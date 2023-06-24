using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using OmniCache.CacheProvider;
using OmniCache.QueryExpression;
using OmniCache.QueryExpression.Utils;
using OmniCache.Reflect;
using OmniCache.Utils;

namespace OmniCache.KeyProviders
{
    public class InvalidKeyProvider
    {
        private KeyProvider _keyProvider;
        private CacheProviderService _cacheProviderService;

        public InvalidKeyProvider(KeyProvider keyProvider, CacheProviderService cacheProvider)
        {
            _keyProvider = keyProvider;
            _cacheProviderService = cacheProvider;
        }

        public async Task<List<string>> GetInvalidatedKeysAsync<T>(T before, T after) where T : class, new()
        {
            List<string> invalidated = new List<string>();

            ReflectClass? cls = ReflectStorage.GetClass(typeof(T));

            if (cls == null || cls.Queries == null)
            {
                return invalidated;
            }

            for (int i = 0; i < cls.Queries.Queries.Count; i++)
            {
                ReflectQuery reflectQuery = cls.Queries.Queries[i];
                Query<T> query = (Query<T>)reflectQuery.Query;

                List<string> invalid;

                if (query._HasGreaterLessThanOp)
                {
                    invalid = await GetInvalidatedKeysForQueryHashAsync<T>(cls, query, before, after);
                }
                else
                {
                    invalid = await GetInvalidatedKeysForQueryAsync<T>(cls, query, before, after);
                }
                
                if (invalid != null && invalid.Count > 0)
                {
                    invalidated.AddRange(invalid);
                }
            }

            return invalidated;
        }

        private async Task<List<string>> GetInvalidatedKeysForQueryHashAsync<T>(ReflectClass cls, Query<T> query, T before, T after) where T : class, new()
        {

            string hashKey = _keyProvider.GetCacheKey<T>(query, null);

            List<string> allKeys = await _cacheProviderService.GetAllHashKeysAsync<List<string>>(hashKey);

            List<string> invalidated = new List<string>();

            if(allKeys != null)
            {                
                foreach(var paramKey in allKeys)
                {

                    bool invalidate = false;

                    string fullKey = _keyProvider.GetCacheKeyFromGreaterThanLessThanOp(query, paramKey);

                    if (query._OrderBy == null)
                    {
                        invalidate = ShouldInvalidateUnorderedList(cls, query, paramKey, before, after);
                    }
                    else
                    {
                        invalidate = await ShouldInvalidateOrderedListAsync(cls, query, paramKey, fullKey, after);
                    }

                    if (invalidate)
                    {                        
                        invalidated.Add(fullKey);
                    }
                }
            }

            return invalidated;
        }

        private bool ShouldInvalidateUnorderedList<T>(ReflectClass cls, Query<T> query, string paramKey, T before, T after) where T : class, new()
        {
            List<object> queryParams = GetQueryParamsFromKey(cls, query, paramKey);

            var compiledQuery = query.Generate(queryParams.ToArray()).Compile();

            bool beforeOutput = compiledQuery.Invoke(before);
            bool afterOutput = compiledQuery.Invoke(after);

            bool invalidate = beforeOutput != afterOutput;
            return invalidate;
        }

        private async Task<bool> ShouldInvalidateOrderedListAsync<T>(ReflectClass cls, Query<T> query, string paramKey, string fullKey, T afterObject) where T : class, new()
        {
            if(query._OrderBy ==null)       //don't need take
            {
                return false;
            }

            CacheItem<List<string>> cachedKeys = await _cacheProviderService.GetAsync<List<string>>(fullKey);
            if(cachedKeys==null)
            {
                return true;        //Missing. Invalidate to remove query from hash
            }

            List<string> keys = cachedKeys.Value;
            if(keys==null)
            {
                return true;       //Empty list. This object belongs in that list so invalidate.
            }

            List<CacheItem<T>> cachedObjects = await _cacheProviderService.GetAsync<T>(keys);

            List<T> beforeList = new List<T>();

            for (int i = 0; i < cachedObjects.Count; i++)
            {
                CacheItem<T> obj = cachedObjects[i];
                if (obj == null)
                {
                    return true;        //Item in list is missing. Just invalidate entire list
                }
                beforeList.Add(obj.Value);
            }

            List<T> afterList = new List<T>(beforeList);
            bool listContainsObject = ReflectionUtils.ListContainsObject(beforeList, afterObject);
            if(!listContainsObject)
            {
                afterList.Add(afterObject);
            }

            List<object> queryParams = GetQueryParamsFromKey(cls, query, paramKey);
            var whereFunc = query.Generate(queryParams.ToArray()).Compile();
            afterList = afterList.Where(whereFunc).ToList();
            
            var orderByFunc = query._OrderBy.Compile();
            if (query._OrderByDesc)
            {                
                afterList = afterList.OrderByDescending(orderByFunc).ToList();
            }
            else
            {
                afterList = afterList.OrderBy(orderByFunc).ToList();
            }

            List<object> beforeKeys = ReflectionUtils.GetKeysFromList<T>(beforeList);
            List<object> afterKeys = ReflectionUtils.GetKeysFromList<T>(afterList);

            if (query._Take == 0 || beforeKeys.Count <= query._Take)
            {
                if (beforeKeys.Count != afterKeys.Count)
                {
                    return true;
                }
            }

            if (beforeKeys.Count > afterKeys.Count)      //after keys should have equal or more.
            {
                return true;
            }

            for(int i=0;i<beforeKeys.Count;i++)         //If list is not the same, then needs invalidation
            {
                object beforeKey = beforeKeys[i];
                object afterKey = afterKeys[i];

                if(!beforeKey.Equals(afterKey))
                {
                    return true;
                }
            }

            return false;

        }

        private static List<object> GetQueryParamsFromKey<T>(ReflectClass cls, Query<T> query, string key)
        {
            
            string[] parts = QueryHashParamUtils.GetQueryParamsFromHashKey(key);

            if (parts.Length != query.ParamList.Count)
            {
                throw new Exception($"Query {query._QueryName} does not match param count({query.ParamList.Count}) in cache({parts.Length})");
            }

            List<object> paramVals = new List<object>();
            for (int i = 0; i < query.ParamList.Count; i++)
            {
                QueryParamDetail param = query.ParamList[i];
                string paramText = parts[i];

                if (param.PropertyName != null)         //Params that are constants don't have property names
                {
                    ReflectField? field = cls.GetField(param.PropertyName);
                    if (field == null)
                    {
                        throw new Exception($"Cannot find property {param.PropertyName} for class {cls.Name}");
                    }

                    object val = ReflectionUtils.ConvertStringToPropertyType(field.PropertyInfo, paramText);
                    paramVals.Add(val);
                }
            }
            return paramVals;
        }

        private async Task<List<string>> GetInvalidatedKeysForQueryAsync<T>(ReflectClass cls, Query<T> query, T before, T after) where T : class, new()
        {        

            List<string> invalidated = new List<string>();

            List<object> beforeParams = new List<object>();
            List<object> afterParams = new List<object>();

            bool invalidateBefore = false;
            bool invalidateAfter = false;

            if(before==null)
            {
                invalidateAfter = true;
            }

            for (int i=0;i<query.ParamList.Count;i++)
            {
                QueryParamDetail param = query.ParamList[i];

                if (param.PropertyName == OmniCacheConstants.PARAM_PROPNAME_CONSTANT)  //Params that are constants don't have property names
                {

                }
                else
                {
                    ReflectField? field = cls.GetField(param.PropertyName);
                    if (field == null)
                    {
                        throw new Exception($"Cannot find property {param.PropertyName} for class {cls.Name}");
                    }

                    object beforeVal = field.GetValue(before);
                    object afterVal = field.GetValue(after);

                    beforeParams.Add(beforeVal);
                    afterParams.Add(afterVal);

                    if (beforeVal != null || afterVal != null)                    
                    {
                        if ((beforeVal == null && afterVal != null) || !beforeVal.Equals(afterVal))
                        {
                            invalidateBefore = true;
                            invalidateAfter = true;
                        }
                    }
                }
            }
            
            string beforeKey = _keyProvider.GetCacheKey<T>(query, beforeParams.ToArray());
            string afterKey = _keyProvider.GetCacheKey<T>(query, afterParams.ToArray());

            if (before != null)
            {
                if (!invalidateBefore || !invalidateAfter)
                {
                    bool beforeOutput = query.Generate(beforeParams.ToArray()).Compile().Invoke(before);
                    bool afterOutput = query.Generate(afterParams.ToArray()).Compile().Invoke(after);

                    if (beforeOutput != afterOutput)
                    {
                        invalidateBefore = true;
                        invalidateAfter = true;
                    }
                }
            }

            if (invalidateBefore)
            {
                invalidated.Add(beforeKey);
            }

            if (invalidateAfter)
            {
                invalidated.Add(afterKey);
            }

            return invalidated;
        }
    }
}


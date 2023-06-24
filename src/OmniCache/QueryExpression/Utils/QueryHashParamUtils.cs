using System;
namespace OmniCache.QueryExpression.Utils
{
	public class QueryHashParamUtils
	{
		private QueryHashParamUtils()
		{
		}

		public static string GetHashKeyFromQueryParams(object[] queryParams)
		{
            string key = "";

            for (int i = 0; i < queryParams.Length; i++)
            {
                var queryParam = queryParams[i];
                key += OmniCacheConstants.KEY_PARAM_DELIM + queryParam;
            }

            return key;
        }

		public static string[] GetQueryParamsFromHashKey(string key)
		{
            string[] parts = key.Split(OmniCacheConstants.KEY_PARAM_DELIM);

            string[] newArray = parts.Skip(1).ToArray();
			return newArray;
        }

    }
}


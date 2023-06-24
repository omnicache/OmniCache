using System;
using System.Text.Json;

namespace OmniCache.Utils
{
	public static class JsonUtils
	{
        public static string ToJsonString(object obj)
        {
            if (obj == null)
            {
                return null;
            }

            string json = JsonSerializer.Serialize(obj);
            return json;
        }

        public static T FromJsonString<T>(string json) where T : class        
        {
            if(json == null)
            {
                return null;
            }

            var obj = JsonSerializer.Deserialize<T>(json);
            return obj;
        }
    }
}


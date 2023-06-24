using System;
using Microsoft.Extensions.DependencyInjection;

namespace OmniCache
{
	public static class ConfigStorageExtensions
    {
		public static IServiceCollection AddOmniCacheConfig<T>(this IServiceCollection services, T config) where T : class        
		{
			ConfigStorage.SetConfig<T>(config);

            return services;
		}
	}

    public class ConfigStorage
	{
		private static Dictionary<Type, object> _configStore = new Dictionary<Type, object>();
		private ConfigStorage()
		{
		}

		public static void SetConfig<T>(T config) where T:class
		{
			Type type = typeof(T);			
			_configStore[type] = config;
		}

		public static T GetConfig<T>() where T : class
        {
            Type type = typeof(T);
            if (!_configStore.ContainsKey(type))
            {
				return null;
            }

			return (T)_configStore[type];
        }
	}
}


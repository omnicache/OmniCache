using System;
using System.Collections.Concurrent;
using System.Reflection;
using OmniCache;
using OmniCache.Utils;

namespace OmniCache.Reflect
{
	public class ReflectStorage
	{
        private static ConcurrentDictionary<Type, (bool, ReflectClass)> Classes = new ConcurrentDictionary<Type, (bool, ReflectClass)>();

        public ReflectStorage()
		{
		}

        public static void ClearAllQueries()
        {
            Classes.Clear();
        }

        public static void LoadAllQueries(Assembly assembly)
        {
            try
            {
                Type[] types = assembly.GetTypes();
                
                foreach (Type type in types)
                {
                    LoadQueries(type);
                }

            }
            catch (ReflectionTypeLoadException e)
            {
                
                foreach (Type type in e.Types)
                {
                    if (type != null)
                    {
                        try
                        {
                            LoadQueries(type);
                        }
                        catch(Exception ex)
                        {
                            //ignore types that can't be loaded.
                        }
                    }
                }

            }

        }

        public static void LoadAllQueries(Type type)
        {                        
            LoadQueries(type);             
        }

        public static ReflectClass? GetClass(Type type, bool required = false)
        {
            if (Classes.ContainsKey(type))
            {
                (bool processed, ReflectClass cls) = Classes[type];

                //if(!processed)        //dont need for now...
                //{
                //    throw new Exception($"LoadAllQueries not called for Class {type.Name}");
                //}
                return cls;
            }

            if (required)
            {
                throw new Exception($"Class {type.Name} missing from Cache store");
            }
            return null;
        }

        private static ReflectClass AddClassIfNotExists(Type type)
        {
            ReflectClass cls = null;
            
            if (Classes.ContainsKey(type))
            {
                (bool processed, cls) = Classes[type];
                return cls;
            }
                           
            if (Attribute.IsDefined(type, typeof(CacheableAttribute)))
            {
                cls = new ReflectClass(type);
            }
            
            Classes[type] = (false, cls);
            return cls;
        }

        private static void ValidateClass(Type type)
        {
            CheckNonStaticQueries(type);
            CheckQueriesWithoutClassAttribute(type);
        }

        private static void LoadQueries(Type type)
        {
            ValidateClass(type);

            ReflectClass cls = AddClassIfNotExists(type);
            (bool processed, cls) = Classes[type];
            if(processed)
            {
                return;
            }
            Classes[type] = (true, cls);

            
            FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.FlattenHierarchy);

            foreach (FieldInfo field in fields)
            {
                if (typeof(IQuery).IsAssignableFrom(field.FieldType))
                {
                    IQuery query = (IQuery)field.GetValue(null);        //static so no need for an object

                    Type genericType = ReflectionUtils.GetGenericClassFromField(field);

                    query.Init($"{type.Name}.{field.Name}");

                    AddQueryToCollection(query, genericType, field);
                }
            }
        }

        private static void AddQueryToCollection(IQuery query, Type type, FieldInfo field)
        {            
            ReflectClass cls = AddClassIfNotExists(type);            

            ReflectQuery cq = new ReflectQuery();            
            cq.Load(type, query, field);

            var clsQueries = cls.Queries;
            var queries = clsQueries.Queries;
            queries.Add(cq);

        }


        private static void CheckNonStaticQueries(Type type)
        {            
            FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);

            foreach (FieldInfo field in fields)
            {
                if (field.FieldType == typeof(IQuery))
                {
                    throw new Exception($"Query {type.Name}.{field.Name} must be static");
                }
            }

        }

        private static void CheckQueriesWithoutClassAttribute(Type type)
        {           
            FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.FlattenHierarchy);

            foreach (FieldInfo field in fields)
            {

                if (typeof(IQuery).IsAssignableFrom(field.FieldType))
                {
                    Type genericType = ReflectionUtils.GetGenericClassFromField(field);

                    if (!Attribute.IsDefined(genericType, typeof(CacheableAttribute)))
                    {
                        throw new Exception($"Class {genericType.Name} needs to have the Cacheable attribute");
                    }
                        
                }
            }
        }
    }
}


using System;
using System.Data;
using System.Linq.Expressions;
using System.Net.Mime;
using System.Reflection;
using OmniCache.Reflect;

namespace OmniCache.Utils
{
	public class ReflectionUtils
	{
		private ReflectionUtils()
		{
		}

        public static Type GetListType(Type listType)
        {
            var arguments = listType.GetGenericArguments();
            if(arguments==null || arguments.Count()<1)
            {
                return null;
            }
            return arguments[0];
        }

        public static Type GetTypeFromObject(object obj)
        {
            Type type = ReflectionUtils.GetPropertyVal(obj, "Type") as Type;
            return type;
        }

        public static object GetPropertyVal(Object obj, string propertyName)
        {
            if(obj==null)
            {
                return null;
            }

            var prop = obj.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            if (prop == null)
            {
                return null;
            }

            var val = prop.GetValue(obj);
            return val;
        }

        public static object ConvertStringToPropertyType(PropertyInfo propertyInfo, string stringValue)
        {
            Type propertyType = propertyInfo.PropertyType;

            // If the property type is nullable, we need to get the underlying type
            if (Nullable.GetUnderlyingType(propertyType) != null)
            {
                propertyType = Nullable.GetUnderlyingType(propertyType);
            }

            return Convert.ChangeType(stringValue, propertyType);
        }

        public static Type GetGenericClassFromField(FieldInfo field)
        {

            Type[] typeArguments = field.FieldType.GetGenericArguments();
            if (typeArguments.Length != 1)
            {
                throw new Exception($"Invalid generic paramater count:{field.FieldType.Name}");
            }

            Type genericType = typeArguments[0];
          
            return genericType;
        }


        public static List<object> GetKeysFromList<T>(List<T> objList) where T : class, new()
        {
            if (objList == null)
            {
                return null;
            }

            List<object> keys = new List<object>();

            ReflectClass? cls = ReflectStorage.GetClass(typeof(T));

            for (int i = 0; i < objList.Count; i++)
            {
                object obj = objList[i];
                object objKey = null;

                if (obj != null)
                {
                    objKey = cls.KeyField.GetValue(obj);                    
                }
                keys.Add(objKey);
            }
            return keys;
        }

        public static bool ListContainsObject<T>(List<T> objList, T toFind) where T : class, new()
        {
            if (objList == null)
            {
                return false;
            }

            ReflectClass? cls = ReflectStorage.GetClass(typeof(T));
            object toFindKey = cls.KeyField.GetValue(toFind);

            for (int i = 0; i < objList.Count; i++)
            {
                object obj = objList[i];
                if (obj != null)
                {
                    object objKey = cls.KeyField.GetValue(obj);
                    
                    if (toFindKey.Equals(objKey))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static object ConvertKeyFieldType<T>(object key) where T : class, new()
        {
            if (key == null)
            {
                return null;
            }

            ReflectClass? cls = ReflectStorage.GetClass(typeof(T));
            if(cls==null)
            {
                return key;
            }
            Type toType = cls.KeyField.Type;
            Type fromType = key.GetType();

            if(toType==fromType)
            {
                return key;
            }

            if (!TypeUtils.CanConvert(fromType, toType))
            {
                return key;                
            }

            return Convert.ChangeType(key, toType);
        }

    }
}


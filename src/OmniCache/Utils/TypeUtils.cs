using System;
using System.ComponentModel;
using System.Net.Mime;

namespace OmniCache.Utils
{
	public class TypeUtils
	{
		private TypeUtils()
		{
		}

        public static bool IsSameTypeOrNullable(Type typeA, Type typeB)
        {
            if (typeA == typeB)
            {
                return true;
            }

            if (typeA.IsGenericType && typeA.GetGenericTypeDefinition() == typeof(Nullable<>) && typeA.GetGenericArguments()[0] == typeB)
            {
                return true;
            }

            if (typeB.IsGenericType && typeB.GetGenericTypeDefinition() == typeof(Nullable<>) && typeB.GetGenericArguments()[0] == typeA)
            {
                return true;
            }

            return false;
        }

        public static bool CanConvert(Type fromType, Type toType)
        {            
            var listTypeFrom = ReflectionUtils.GetListType(fromType);            
            if (listTypeFrom != null)
            {
                var listTypeTo = ReflectionUtils.GetListType(toType);
                if(listTypeTo!=null)
                {
                    fromType = listTypeFrom;
                    toType = listTypeTo;
                }
            }

            if (Nullable.GetUnderlyingType(toType) != null)
            {            
                toType = Nullable.GetUnderlyingType(toType);               
            }

            if(fromType == toType)
            {
                return true;
            }
            TypeConverter converter = TypeDescriptor.GetConverter(fromType);
            return converter.CanConvertTo(toType);
        }

        public static object Convert(object obj, Type toType)
        {
            if(obj==null)
            {
                return obj;
            }

            Type fromType = obj.GetType();
            if(fromType == toType)
            {
                return obj;
            }

            TypeConverter converter = TypeDescriptor.GetConverter(fromType);
            if(converter.CanConvertTo(toType))
            {
                return System.Convert.ChangeType(obj, toType);
            }

            Type baseNullableToType = null;

            if (toType.IsGenericType && toType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {             
                baseNullableToType = Nullable.GetUnderlyingType(toType);

                if(fromType == baseNullableToType)
                {
                    return obj;
                }
                converter = TypeDescriptor.GetConverter(fromType);
                if (converter.CanConvertTo(baseNullableToType))
                {                                   
                    return System.Convert.ChangeType(obj, baseNullableToType);      //int -> int?
                }
                else
                {
                    throw new Exception($"Cannot convert from type {fromType.Name} to {toType.Name}");
                }
            }

            //from this point, we only accept lists. Everything else should have been handled.

            if (!(toType.IsGenericType && toType.GetGenericTypeDefinition() == typeof(List<>)))
            {
                throw new Exception($"Cannot convert from type {fromType.Name} to {toType.Name}");
            }

            if (!(fromType.IsGenericType && fromType.GetGenericTypeDefinition() == typeof(List<>)))
            {
                throw new Exception($"Cannot convert from type {fromType.Name} to {toType.Name}");
            }

            Type fromElementType = fromType.GetGenericArguments()[0];
            Type toElementType = toType.GetGenericArguments()[0];
            Type newToElementType = toElementType;

            if (fromElementType != toElementType)
            {
                converter = TypeDescriptor.GetConverter(fromElementType);
                if (!converter.CanConvertTo(toElementType))
                {
                    if (toElementType.IsGenericType && toElementType.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        Type toElementBaseType = Nullable.GetUnderlyingType(toElementType);
                        newToElementType = toElementBaseType;

                        converter = TypeDescriptor.GetConverter(fromElementType);
                        if (!converter.CanConvertTo(toElementBaseType))
                        {
                            throw new Exception($"Cannot convert from type {fromType.Name} to {toType.Name}");
                        }
                    }
                    else
                    {
                        throw new Exception($"Cannot convert from type {fromType.Name} to {toType.Name}");
                    }
                }
            }


            var objList = (System.Collections.IList)obj;

            var list = Activator.CreateInstance(toType);
            foreach (var item in objList)
            {
                object converted = item;
                if(converted != null)
                {
                    converted = System.Convert.ChangeType(item, newToElementType);
                }
                
                ((System.Collections.IList)list).Add(converted);
            }
            return list;

        }


    }
}


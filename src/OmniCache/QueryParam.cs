using System;
using System.Linq.Expressions;

namespace OmniCache
{
    public class QueryParam
    {
        public int ParamNo { get; set; }

        public QueryParam(int paramNo)
        {
            ParamNo = paramNo;
        }

        
        public static explicit operator int(QueryParam obj)
        {
            return 0;
        }

        public static bool operator ==(QueryParam obj1, QueryParam obj2)
        {
            return true;
        }

        public static bool operator !=(QueryParam obj1, QueryParam obj2)
        {
            return true;
        }

        public static bool operator ==(QueryParam obj1, object obj2)
        {
            return true;
        }

        public static bool operator !=(QueryParam obj1, object obj2)
        {
            return true;
        }

        public static bool operator ==(object obj2, QueryParam obj1)
        {
            return true;
        }

        public static bool operator !=(object obj2, QueryParam obj1)
        {
            return true;
        }

        public static bool operator >(QueryParam obj1, QueryParam obj2)
        {
            return true;
        }

        public static bool operator >=(QueryParam obj1, QueryParam obj2)
        {
            return true;
        }

        public static bool operator <(QueryParam obj1, QueryParam obj2)
        {
            return true;
        }

        public static bool operator <=(QueryParam obj1, QueryParam obj2)
        {
            return true;
        }

        public static bool operator >(QueryParam obj1, object obj2)
        {
            return true;
        }

        public static bool operator >=(QueryParam obj1, object obj2)
        {
            return true;
        }

        public static bool operator <(QueryParam obj1, object obj2)
        {
            return true;
        }

        public static bool operator <=(QueryParam obj1, object obj2)
        {
            return true;
        }

        public static bool operator >(object obj2, QueryParam obj1)
        {
            return true;
        }

        public static bool operator >=(object obj2, QueryParam obj1)
        {
            return true;
        }

        public static bool operator <(object obj2, QueryParam obj1)
        {
            return true;
        }

        public static bool operator <=(object obj2, QueryParam obj1)
        {
            return true;
        }

        public bool Contains<T>(T obj)//MemberExpression member)
        {
            return false;
        }

    }
}


using System;
using System.Reflection;

namespace OmniCache.Reflect
{
    public class ReflectQuery
    {
        public IQuery Query;
        public Type Type;
        public Type ClassFoundIn;
        public string FieldName;

        public ReflectQuery()
        {
        }

        public void Load(Type type, IQuery query, FieldInfo field)
        {
            Type = type;
            Query = query;
            ClassFoundIn = field.DeclaringType;
            FieldName = field.Name;

        }

    }
}

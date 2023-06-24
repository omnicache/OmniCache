using System;
using System.Reflection;

namespace OmniCache.Reflect
{
	public class ReflectField
	{
        public String Name { get; set; }
        public Type Type { get; set; }
        public PropertyInfo PropertyInfo { get; set; }

        public ReflectField()
		{
		}

        public override string ToString()
        {
            return Name + ":" + Type.Name;
        }


        public object GetValue(object obj)
        {
            if(obj==null)
            {
                return null;
            }

            return PropertyInfo.GetValue(obj);
        }

        public void SetValue(object obj, object val)
        {

            PropertyInfo.SetValue(obj, val);

        }
    }
}


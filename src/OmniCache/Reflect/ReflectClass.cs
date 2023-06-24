using System;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Reflection;

namespace OmniCache.Reflect
{
	public class ReflectClass
	{		
        public Type Type { get; set; }
        public String Name { get; set; }
        public ReflectField KeyField { get; set; }
        public List<ReflectField> Fields = new List<ReflectField>();

        public ReflectQueryList Queries = new ReflectQueryList();

        public ReflectClass(Type type)
        {
            Type = type;
            Name = type.Name;

            Load(type);            
        }

        public ReflectField? GetField(string name)
        {
            
            var field = Fields.Find(f => f.Name == name);
            return field;
            
        }

        private void Load(Type type)
        {
            
            PropertyInfo[] props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
            foreach (PropertyInfo prop in props)
            {

                ReflectField field = new ReflectField();
                field.Name = prop.Name;
                field.Type = prop.PropertyType;
                field.PropertyInfo = prop;
                Fields.Add(field);

                if(field.Name.ToLower()=="id")
                {
                    KeyField = field;
                }
                else
                {
                    if (KeyField != null && KeyField.Name.ToLower() == "id")
                    {
                        //already have the right key
                    }
                    else
                    {                        
                        if(prop.GetCustomAttribute<KeyAttribute>() != null)
                        {
                            if(KeyField!=null)
                            {
                                throw new Exception("Cache Key error - Use Id property or only one key attribute");
                            }
                            KeyField = field;
                        }
                        
                    }
                    
                }
            }

            if(KeyField==null)
            {
                throw new Exception("Cache Key error - Key not found. Use Id property or a key attribute");
            }
        }

        

    }
}


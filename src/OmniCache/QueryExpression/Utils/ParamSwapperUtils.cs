using System;
using OmniCache.Utils;
using System.Linq.Expressions;

namespace OmniCache.QueryExpression.Utils
{
	public class ParamSwapperUtils
	{

        private static Type GetClassNameFromExpression(Expression otherExpression)
        {
            object toCheck = otherExpression;

            var operand = ReflectionUtils.GetPropertyVal(toCheck, "Operand");
            if (operand != null)
            {
                toCheck = operand;
            }

            var member = ReflectionUtils.GetPropertyVal(toCheck, "Member");
            if (member == null)
            {
                return null;
            }

            var declaringType = (Type)ReflectionUtils.GetPropertyVal(member, "DeclaringType");
            if (declaringType == null)
            {
                return null;
            }
            return declaringType;
        }

        public static (Type, string) GetClassAndPropertyName(string queryName, Expression otherExpression)
        {
            var newExp = ExpressionUtils.RemoveConvertsFromParam(otherExpression);
            if (newExp != otherExpression)
            {
        
                otherExpression = newExp;
            }

          
            Type classType = GetClassNameFromExpression(otherExpression);
            if (classType == null)
            {
                throw new Exception($"Query {queryName} Expression type hasn't been implemented: {otherExpression.ToString()}");
            }

            var text = otherExpression.ToString();      //will be in format "book.name"
            if (text != null)
            {
                string[] parts = text.Split('.');
                if (parts.Length == 2)
                {
                    if (parts[1].Contains(")"))
                    {
                        throw new Exception($"Query {queryName} - Could not unwrap query condition :{otherExpression.ToString()}");

                    }
                    return (classType, parts[1]);
                }
            }

            throw new Exception($"Query {queryName} Expression type hasn't been implemented: {otherExpression.ToString()}");
        }
    }
}


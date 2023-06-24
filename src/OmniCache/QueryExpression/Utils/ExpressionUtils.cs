using System;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Xml.Linq;
using OmniCache.Utils;

namespace OmniCache.QueryExpression.Utils
{
	public class ExpressionUtils
	{
		private ExpressionUtils()
		{
		}

        public static bool IsQueryParam(Expression expression)
        {
            expression = RemoveConvertFromParam(expression);
            return expression is NewExpression newExpression && newExpression.Type == typeof(QueryParam);
        }


        public static Expression RemoveConvertsFromParam(Expression exp)
        {
            bool done = false;
            while(!done)
            {
                if(exp==null)
                {
                    return null;
                }

                Type type = exp.GetType();
                if (type != typeof(UnaryExpression))
                {
                    return exp;
                }

                UnaryExpression unaryExpression = (UnaryExpression)exp;
                if (exp.NodeType != ExpressionType.Convert)
                {
                    return exp;
                }
                exp = unaryExpression.Operand;
            }
            return exp;
        }

        public static Expression RemoveConvertFromParam(Expression exp)
        {
            Type type = exp.GetType();
            if (type == typeof(UnaryExpression))
            {
                UnaryExpression unaryExpression = (UnaryExpression)exp;
                if (exp.NodeType == ExpressionType.Convert)
                {
                    return unaryExpression.Operand;
                }
            }

            return exp;
        }

        public static ParameterExpression GetObjectFromLambdaExpression(Expression expression)
        {
            if(expression.NodeType != ExpressionType.Lambda)
            {
                return null;
            }
            var paramCol = ReflectionUtils.GetPropertyVal(expression, "Parameters") as ReadOnlyCollection<ParameterExpression>;
            if(paramCol==null)
            {
                return null;
            }

            return paramCol[0];                
        }
    }
}


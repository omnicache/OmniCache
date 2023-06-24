using System;
using System.Linq.Expressions;

namespace OmniCache.QueryExpression.InputSetter
{
	public class InputSetterHelper
	{
		private InputSetterHelper()
		{
		}

        public static Expression<Func<T, bool>> SetParamsWithValues<T>(string queryName, Expression<Func<T, bool>> expression, InputParameters inputParams)
        {
            var parameterReplacer = new InputSetterVisitor<T>(queryName, inputParams);
            var modifiedExpression = (Expression<Func<T, bool>>)parameterReplacer.Visit(expression);
            return modifiedExpression;
        }

    }
}


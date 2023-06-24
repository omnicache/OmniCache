using System;
using System.Linq.Expressions;

namespace OmniCache.QueryExpression.Utils
{
	public class QueryParamNodeUtils
	{
		private QueryParamNodeUtils()
		{
		}

        public static int GetNodeQueryParamNo<T>(string queryName, NewExpression node)
        {
            Expression argument = node.Arguments.FirstOrDefault();

            if (argument != null && argument.NodeType == ExpressionType.Constant)
            {
                ConstantExpression constantExpression = (ConstantExpression)argument;
                object queryParamValue = constantExpression.Value;

                int paramVal = (int)queryParamValue;
                return paramVal;
            }
            throw new Exception($"Query {queryName} missing query parameter");
        }
    }
}


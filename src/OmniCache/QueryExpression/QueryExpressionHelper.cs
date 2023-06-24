using System;
using System.IO;
using System.Linq.Expressions;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Text.Json;
using System.Xml.Linq;
using OmniCache.QueryExpression.InputSetter;
using OmniCache.QueryExpression.ParamSwapper;
using Microsoft.VisualBasic;

namespace OmniCache.QueryExpression
{
	public class QueryExpressionHelper
	{
       
		private QueryExpressionHelper()
		{
		}

        
        public static Expression<Func<T, bool>> SwapQueryParamsWithParameterExpression<T>(string queryName, Expression<Func<T, bool>> expression, out List<QueryParamDetail> paramList, out bool hasGreaterLessThanOp)
        {
            return ParamSwapperHelper.SwapQueryParamsWithParameterExpression<T>(queryName, expression, out paramList, out hasGreaterLessThanOp);
        }

        public static Expression<Func<T, bool>> SetParamsWithValues<T>(string queryName, Expression<Func<T, bool>> expression, InputParameters inputParams)
        {
            return InputSetterHelper.SetParamsWithValues<T>(queryName, expression, inputParams);
        }


        

    }
}


using System;
using System.Linq.Expressions;

namespace OmniCache.QueryExpression.ParamSwapper
{
	public class ParamSwapperHelper
	{
		private ParamSwapperHelper()
		{
		}

        public static Expression<Func<T, bool>> SwapQueryParamsWithParameterExpression<T>(string queryName, Expression<Func<T, bool>> expression, out List<QueryParamDetail> paramList, out bool hasGreaterLessThanOp)
        {

            //Swap QueryParams Contains methods
            var containsVisitor = new ContainsParamSwapperVisitor<T>(queryName);
            var replacedExpression1 = (Expression<Func<T, bool>>)containsVisitor.Visit(expression);
            var paramList1 = containsVisitor.ParamList;

            // Swap QueryParams with ParameterExpressions
            var visitor = new ParamSwapperVisitor<T>(queryName);            
            var replacedExpression2 = (Expression<Func<T, bool>>)visitor.Visit(replacedExpression1);


            var paramList2 = visitor.ParamList;

            paramList = paramList1;
            paramList.AddRange(paramList2);

            paramList = paramList.OrderBy(p => p.ParamNo).ToList();
            ValidateParamNos<T>(queryName, paramList);
            hasGreaterLessThanOp = visitor.HasGreaterLessThanOp;

            return replacedExpression2;
        }

        private static void ValidateParamNos<T>(string queryName, List<QueryParamDetail> paramList)
        {

            for (int i = 0; i < paramList.Count; i++)
            {
                if (paramList[i].PropertyName != null)
                {
                    int paramNo = paramList[i].ParamNo;

                    if (paramNo != (i + 1))
                    {
                        throw new Exception($"Query {queryName} missing param no.{(i + 1)}");
                    }
                }
            }
        }

    }
}


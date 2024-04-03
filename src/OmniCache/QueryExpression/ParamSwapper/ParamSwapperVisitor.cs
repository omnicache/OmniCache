using System;
using System.Linq.Expressions;
using System.Reflection;
using OmniCache;
using OmniCache.QueryExpression.Utils;
using OmniCache.Utils;

namespace OmniCache.QueryExpression.ParamSwapper
{
	
    public class ParamSwapperVisitor<T> : ExpressionVisitor
    {

        public List<QueryParamDetail> ParamList = new List<QueryParamDetail>();
        public bool HasGreaterLessThanOp = false;

        private string _QueryName;

        public ParamSwapperVisitor(string queryName)
        {
            _QueryName = queryName;
        }

        protected override Expression VisitBinary(BinaryExpression node)
        {
             if(node.NodeType == ExpressionType.GreaterThan
                                                    || node.NodeType == ExpressionType.LessThan
                                                    || node.NodeType == ExpressionType.GreaterThanOrEqual
                                                    || node.NodeType == ExpressionType.LessThanOrEqual)
            {
                HasGreaterLessThanOp = true;
            }
            
            var isQueryParamLeft = ExpressionUtils.IsQueryParam(node.Left);
            var isQueryParamRight = ExpressionUtils.IsQueryParam(node.Right);

            if (isQueryParamLeft && isQueryParamRight)
            {
                throw new Exception($"Query {_QueryName} QueryParam cannot be on both sides of the condition in class");
            }
            else if (isQueryParamLeft)
            {
                var newRight = ExpressionUtils.RemoveConvertFromParam(node.Left);
                var newParam = getExpressionParam(node.Left, newRight);

                return Expression.MakeBinary(node.NodeType, newParam, newRight);
            }
            else if (isQueryParamRight)
            {
                var newLeft = ExpressionUtils.RemoveConvertFromParam(node.Left);
                var newParam = getExpressionParam(node.Right, newLeft);

                return Expression.MakeBinary(node.NodeType, newLeft, newParam);
            }

            return base.VisitBinary(node);
        }



        private void ValidateOtherExpressionIsClassProperty(Expression otherExpression)
        {
            Type classType = typeof(T);

            (Type propertyClassType, string propertyName) = ParamSwapperUtils.GetClassAndPropertyName(_QueryName, otherExpression);

            if (propertyClassType.IsAssignableFrom(classType))
            {
                return;
            }
            
            throw new Exception($"Query {_QueryName} QueryParam must be against object property. Eg. book => book.name == new QueryParam(1)");
        }

        private string GetParameterName(Expression exp, Expression otherExpression)
        {
            NewExpression nexp = (NewExpression)exp;
            int paramNo = QueryParamNodeUtils.GetNodeQueryParamNo<T>(_QueryName, nexp);

            (Type propertyClassType, string propertyName) = ParamSwapperUtils.GetClassAndPropertyName(_QueryName, otherExpression);
            if (propertyName == null)
            {
                Type classType = typeof(T);
                throw new Exception($"Query {_QueryName} QueryParam must be against object property. Eg. book => book.name == new QueryParam(1).");
            }

            ParamList.Add(new QueryParamDetail() { ParamNo = paramNo, PropertyName = propertyName, ParamType = QueryParamDetailType.Comparison });

            return QueryParamNameUtils.GetParamName(propertyName, paramNo);
        }

        private ParameterExpression getExpressionParam(Expression exp, Expression otherExpression)
        {
            ValidateOtherExpressionIsClassProperty(otherExpression);

            Type type = otherExpression.Type;

            string paramName = GetParameterName(exp, otherExpression);
            var newParam = Expression.Parameter(type, paramName);
            return newParam;
        }

    }
}


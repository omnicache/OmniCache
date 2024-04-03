using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Xml.Linq;
using OmniCache.QueryExpression.Utils;
using OmniCache.Utils;

namespace OmniCache.QueryExpression.ParamSwapper
{	
    public class ContainsParamSwapperVisitor<T> : ExpressionVisitor
    {
        public List<QueryParamDetail> ParamList = new List<QueryParamDetail>();

        protected ParameterExpression _MainObject;

        private string _QueryName;

        public ContainsParamSwapperVisitor(string queryName)
        {
            _QueryName = queryName;
        }

        public override Expression Visit(Expression node)
        {
            if (node == null)
            {
                return null;
            }

            if (_MainObject == null)
            {
                _MainObject = ExpressionUtils.GetObjectFromLambdaExpression(node);
            }

            Type returnType = node.Type;

            if (returnType != typeof(Boolean))
            {
                return base.Visit(node);
            }

            if(node.NodeType != ExpressionType.Call)
            {
                return base.Visit(node);
            }

            var method = ReflectionUtils.GetPropertyVal(node, "Method");
            if (method == null)
            {
                return base.Visit(node);
            }

            string methodName = ReflectionUtils.GetPropertyVal(method, "Name") as string;

            if (methodName == null || methodName != "Contains")
            {
                return base.Visit(node);
            }

            /***** Get calling object *****/

            var callerObj = ReflectionUtils.GetPropertyVal(node, "Object");

            Type callerType = ReflectionUtils.GetTypeFromObject(callerObj);

            
            /***** Get arguments *****/

            ReadOnlyCollection<Expression> arguments = ReflectionUtils.GetPropertyVal(node, "Arguments") as ReadOnlyCollection<Expression>;

            if (arguments == null || arguments.Count == 0)
            {
                throw new Exception($"Query {_QueryName} argument required for Contains() method");
            }

            if (arguments.Count > 1)
            {
                throw new Exception($"Query {_QueryName} contains too many arguments for Contains() method");
            }

            Expression argument = arguments[0];

            Expression unconvertedArgument = ExpressionUtils.RemoveConvertsFromParam(argument);

            /***** If neither calling object or argument is a QueryParam, then just ignore *****/

            bool callerIsQueryParam = callerType == typeof(QueryParam);
            bool argumentIsQueryParam = unconvertedArgument.Type == typeof(QueryParam);

            if (!callerIsQueryParam && !argumentIsQueryParam)
            {
                return base.Visit(node);
            }

            Type paramType;
            NewExpression caller = null;

            if (callerIsQueryParam)
            {
                paramType = argument.Type;
                caller = (NewExpression)callerObj;
            }
            else
            {
                paramType = ReflectionUtils.GetListType(callerType);
                if(paramType==null)
                {
                    return base.Visit(node);            //not a generic list type
                }
            }

            if(paramType == typeof(QueryParam))
            {
                paramType = typeof(Object);
            }

            /****************************** All done checking, start processing ******************************/


            if (_MainObject == null)
            {
                throw new Exception($"Query {_QueryName} lambda param not found");
            }


            Expression callerExp = null;
            Expression argumentExp = null;

          
            if (argumentIsQueryParam)
            {
                string propertyName = OmniCacheConstants.PARAM_PROPNAME_CONSTANT;

                argumentExp = GetArgumentParamExpression(unconvertedArgument as NewExpression, propertyName, paramType);
            }
            else
            {
                argumentExp = argument;                
            }

            if (callerIsQueryParam)
            {
                string propertyName;
                if (argumentIsQueryParam || argument.NodeType == ExpressionType.Constant)
                {
                    propertyName = OmniCacheConstants.PARAM_PROPNAME_CONSTANT;
                }
                else
                {
                    (Type classType, propertyName) = ParamSwapperUtils.GetClassAndPropertyName(_QueryName, argument);
                    
                }
                

                callerExp = GetCallerParamExpression(caller, propertyName, paramType);
            }
            else
            {
                callerExp = (Expression)callerObj;
            }


            var containsMethod = typeof(Enumerable).GetMethods()
                .First(m => m.Name == "Contains" && m.GetParameters().Length == 2)
                .MakeGenericMethod(paramType);

            var containsCall = Expression.Call(containsMethod, callerExp, argumentExp);

            return containsCall;
            
        }

        
        protected Expression GetCallerParamExpression(NewExpression callerExpression, string propertyName, Type paramType)
        {
            int paramNo = QueryParamNodeUtils.GetNodeQueryParamNo<T>(_QueryName, callerExpression);
            string paramName = QueryParamNameUtils.GetParamName(propertyName, paramNo);

            Type listType = typeof(List<>).MakeGenericType(paramType);
            ParameterExpression listParam = Expression.Parameter(listType, paramName);

            ParamList.Add(new QueryParamDetail() { ParamNo = paramNo, PropertyName = propertyName, ParamType = QueryParamDetailType.ContainsCaller });
            return listParam;
        }

        protected Expression GetArgumentParamExpression(NewExpression argumentExpression, string propertyName, Type paramType)
        {
            int paramNo = QueryParamNodeUtils.GetNodeQueryParamNo<T>(_QueryName, argumentExpression);
            string paramName = QueryParamNameUtils.GetParamName(propertyName, paramNo);
            
            ParameterExpression listParam = Expression.Parameter(paramType, paramName);

            ParamList.Add(new QueryParamDetail() { ParamNo = paramNo, PropertyName = propertyName, ParamType = QueryParamDetailType.ContainsArgument });
            return listParam;
        }

    }
}


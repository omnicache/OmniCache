using System;
using System.Data;
using System.Linq.Expressions;
using System.Net.Mime;
using OmniCache.QueryExpression.Utils;
using OmniCache.Utils;

namespace OmniCache.QueryExpression.InputSetter
{
	
    public class InputSetterVisitor<T> : ExpressionVisitor
    {
        private readonly InputParameters _InputParams;

        private string _QueryName;

        public InputSetterVisitor(String queryName, InputParameters inputParams)
        {
            _QueryName = queryName;
            _InputParams = inputParams;
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            string paramName = node.Name;

            (string propertyName, int paramNo) = QueryParamNameUtils.ExtractParamNameDetails(paramName);
            if (paramNo == 0)
            {
                return base.VisitParameter(node);
            }
        
            object inputParam = _InputParams.GetParam(paramNo);
            QueryParamDetailType queryParamType = _InputParams.GetParamType(paramNo);

            Type inputParamType = null;

            if (inputParam != null)
            {
                inputParamType = inputParam.GetType();
            }

            Type constantType = node.Type;
            
            if(inputParam != null && inputParamType != constantType)
            {
                if (TypeUtils.CanConvert(inputParamType, constantType))
                {
                    inputParam = TypeUtils.Convert(inputParam, constantType);
                }
                else
                {
                    bool converted = false;
                    
                    if (queryParamType == QueryParamDetailType.ContainsCaller)      //QueryParam().contains(s.ID) requires array.
                    {
                        //convert to array
                        if (constantType.IsGenericType && constantType.GetGenericTypeDefinition() == typeof(List<>))
                        {
                            if (!(inputParamType.IsGenericType && inputParamType.GetGenericTypeDefinition() == typeof(List<>)))
                            {
                                Type listElementType = constantType.GetGenericArguments()[0];
                                if (TypeUtils.CanConvert(inputParamType, listElementType))
                                {
                                    Type listType = typeof(List<>).MakeGenericType(inputParamType);
                                    System.Collections.IList list = (System.Collections.IList)Activator.CreateInstance(listType);
                                    list.Add(inputParam);
                                    inputParam = list;
                                    converted = true;
                                }
                            }
                        }
                    }

                    if (!converted)
                    {
                        throw new Exception($"Query {_QueryName} parameters do not match. Expected {constantType.Name}, got {inputParamType.Name}");
                    }
                
                }
            }
           
            return Expression.Constant(inputParam, constantType);
        }

    }

}


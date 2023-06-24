using System;
using System.Linq.Expressions;
using System.Xml.Linq;
using OmniCache.QueryExpression;
using OmniCache.Utils;


namespace OmniCache
{
	public class Query<T>: IQuery
    {
		public Expression<Func<T, bool>> _Condition { get; set; }
        public int _Take { get; set; } = 0;
        public Expression<Func<T, object>> _OrderBy { get; set; }
        public bool _OrderByDesc { get; set; } = false;
        public Type Type { get; set; }        
        public List<QueryParamDetail> ParamList;
        public bool _HasGreaterLessThanOp = false;
        private bool _Initialised = false;
        public string _QueryName { get; set; }

        public Query()
        {
            Type = typeof(T);
        }

        public Query(Expression<Func<T, bool>> condition)
		{
            _Condition = condition;            
            Type = typeof(T);
            
        }

        public void Init(string name)
        {
            if(_Initialised)
            {
                return;
            }
            _Initialised = true;
            _QueryName = name;
            _Condition = QueryExpressionHelper.SwapQueryParamsWithParameterExpression(_QueryName, _Condition, out List<QueryParamDetail> paramList, out bool hasGreaterLessThanOp);
            ParamList = paramList;
            _HasGreaterLessThanOp = hasGreaterLessThanOp;
        }

        public Query<T> Take(int take)
        {
            if(_OrderBy==null)
            {
                throw new Exception($"Take must be after Order by in Query for class {Type.Name}");
            }

            _Take = take;
            return this;
        }

        public Query<T> OrderBy(Expression<Func<T, object>> orderBy)
		{
			_OrderBy = orderBy;
			return this;
		}

        public Query<T> OrderByDesc(Expression<Func<T, object>> orderBy)
        {
            _OrderBy = orderBy;
			_OrderByDesc = true;
            return this;
        }

        public Expression<Func<T, bool>> Generate(object[] queryParams)
        {
            if(ParamList==null)
            {
                throw new Exception("Query not loaded. Ensure all queries have been loaded");
            }
            var inputParams = new InputParameters(_QueryName, typeof(T), ParamList, queryParams);

            var condition = QueryExpressionHelper.SetParamsWithValues(_QueryName, _Condition, inputParams);
            return condition;
        }
    }
}


using System;
namespace OmniCache.QueryExpression
{
	public class InputParameters
	{
		protected object[] _InputParams = null;
        protected List<QueryParamDetail> _QueryParams;


        public InputParameters(string queryName, Type objectType, List<QueryParamDetail> queryParams, object[] inputParams)
		{
			_InputParams = inputParams;
			_QueryParams = queryParams;
            Load(queryName, objectType, queryParams);
        }

		private void Load(string queryName, Type objectType, List<QueryParamDetail> queryParams)
        {
            int requiredParamCount = queryParams.Count();
            
            if (_InputParams == null || _InputParams.Length == 0)
			{
				if(requiredParamCount > 0)
				{
                    throw new Exception($"Query {queryName} parameters required");
                }

				if(_InputParams == null)
				{
                    _InputParams = new object[0];
				}
            }

			if(requiredParamCount != _InputParams.Length)
			{
                throw new Exception($"Query {queryName} parameter count({requiredParamCount}) doesn't match query call({_InputParams.Length})");
            }

		}

		public object GetParam(int paramNo)
		{

			return _InputParams[paramNo - 1];			
        }

		public QueryParamDetailType GetParamType(int paramNo)
		{
			return _QueryParams[paramNo - 1].ParamType;

        }
    }
}


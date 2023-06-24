using System;
namespace OmniCache.QueryExpression
{
    public enum QueryParamDetailType
    {
        Comparison,
        ContainsCaller,
        ContainsArgument
    }

    public class QueryParamDetail
    {
        public int ParamNo { get; set; }
        public string PropertyName { get; set; }
        public QueryParamDetailType ParamType { get; set; }
    }
}


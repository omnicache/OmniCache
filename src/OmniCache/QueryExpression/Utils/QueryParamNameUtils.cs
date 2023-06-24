using System;
namespace OmniCache.QueryExpression.Utils
{
	public class QueryParamNameUtils
	{
		private QueryParamNameUtils()
		{
		}


        public static string GetParamName(string propertyName, int paramNo)
        {
            return $"{propertyName}.{paramNo}";
        }

        //returns (propertyName, paramNo)
        public static (string, int) ExtractParamNameDetails(string paramName)
        {
            if (paramName != null)
            {
                string[] parts = paramName.Split('.');
                if (parts.Length == 2)
                {
                    if (int.TryParse(parts[1], out int paramNo))
                    {
                        return (parts[0], paramNo);
                    }
                }
            }
            return (null, 0);
        }


    }
}


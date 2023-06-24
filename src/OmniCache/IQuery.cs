using System;
using System.Linq.Expressions;

namespace OmniCache
{
	public interface IQuery
	{
        Type Type { get; set; }
        void Init(string name);
    }
}


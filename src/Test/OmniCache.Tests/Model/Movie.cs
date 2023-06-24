using System;
using System.Linq.Expressions;
using OmniCache;

namespace OmniCache.IntegrationTests.Model
{
	public enum Category
	{
		Action,
		Thriller,
		Horror,
		SciFi,
		Documentary,
		Kids
	}

	[Cacheable]
	public class Movie
	{		
		public long Id { get; set; }
		public string Name { get; set; }
		public DateTime ReleaseDate { get; set; }
		public bool IsNewRelease { get; set; }
		public Category Category { get; set; }
		public char Active { get; set; }
		public int? AgeRestriction { get; set; }

        public Movie()
		{
		}


    }
}


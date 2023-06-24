using System;
using OmniCache;
using System.ComponentModel.DataAnnotations;

namespace RepoDemo.Data.Model
{	
    [Cacheable]
    public class Subject
    {
        [Key]
        public long Id { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public Subject()
        {
        }
    }
}


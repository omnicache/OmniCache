using System;
using OmniCache;
using System.ComponentModel.DataAnnotations;

namespace RepoDemo.Data.Model
{	
    [Cacheable]
    public class Enrolment
    {
        [Key]
        public long Id { get; set; }

        public long StudentId { get; set; }

        public long SubjectId { get; set; }

        public Enrolment()
        {
        }
    }
}


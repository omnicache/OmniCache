using System;
using OmniCache;
using System.ComponentModel.DataAnnotations;

namespace RepoDemo.Data.Model
{	
    public enum StudentStatus
    {
        Active = 1,
        Inactive
    }

    [Cacheable]
    public class Student
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public String Name { get; set; }

        [Required]
        public String Email { get; set; }

        public DateTime DOB { get; set; }

        public StudentStatus Status { get; set; }

        public Student()
        {
        }
    }
}


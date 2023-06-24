using System;
using System.ComponentModel.DataAnnotations;

namespace RepoDemo.Api.Requests
{
	public class AddStudentRequest
	{        
        public String Name { get; set; }
        public String Email { get; set; }
        public DateTime DOB { get; set; }
    }
}


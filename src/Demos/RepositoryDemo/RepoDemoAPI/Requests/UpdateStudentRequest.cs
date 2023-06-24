using System;
namespace RepoDemo.API.Requests
{
	public class UpdateStudentRequest
	{
        public long Id { get; set; }
        public String Name { get; set; }
        public String Email { get; set; }
        public DateTime DOB { get; set; }
    }
}


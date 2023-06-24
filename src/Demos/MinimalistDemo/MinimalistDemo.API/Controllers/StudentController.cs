using System;
using Microsoft.AspNetCore.Mvc;
using MinimalistDemo.Data;
using MinimalistDemo.Data.Entity;

namespace MinimalistDemo.API.Controllers
{
    [Route("api/[controller]")]
    public class StudentController : Controller
    {		
        private ICachedDBSession _cachedDBSession;

        public StudentController(ICachedDBSession cachedDBSession)
        {
            _cachedDBSession = cachedDBSession;
        }


        [HttpGet]
        [Route("GetAll")]
        public async Task<ActionResult<IEnumerable<Student>>> GetAllStudents()
        {

            var students = await Student.GetAllAsync(_cachedDBSession);
            return Ok(students);

        }

        public class AddStudentRequest
        {
            public String Name { get; set; }
            public String Email { get; set; }
            public DateTime DOB { get; set; }
        }

        [HttpPost]
        [Route("Add")]
        public async Task<ActionResult<Student>> AddStudentsAsync([FromBody] AddStudentRequest request)
        {
            Student student = new Student();
            student.Name = request.Name;
            student.Email = request.Email;
            student.DOB = request.DOB;
            student.Status = StudentStatus.Active;

            await student.InsertAsync(_cachedDBSession);

            return Ok(student);
        }

        public class UpdateStudentRequest
        {
            public long Id { get; set; }
            public String Name { get; set; }
            public String Email { get; set; }
            public DateTime DOB { get; set; }
        }

        [HttpPut]
        [Route("Update")]
        public async Task<ActionResult<Student>> UpdateStudentsAsync([FromBody] UpdateStudentRequest request)
        {
            var student = await Student.GetByKeyAsync(_cachedDBSession, request.Id);

            if (student == null)
            {
                throw new Exception($"Student with Id {request.Id} not found");
            }

            student.Name = request.Name;
            student.Email = request.Email;
            student.DOB = request.DOB;
            student.Status = StudentStatus.Active;

            await student.UpdateAsync(_cachedDBSession);

            return Ok(student);
        }
    }
}


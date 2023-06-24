using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RepoDemo.Data;
using RepoDemo.Data.Repository.Contract;
using RepoDemo.Data.Model;
using Microsoft.AspNetCore.Mvc;
using RepoDemo.Api.Requests;
using RepoDemo.Services.Contract;
using RepoDemo.API.Requests;

namespace RepoDemo.Api.Controllers
{
    [Route("api/[controller]")]
    public class StudentController : Controller
    {
        private IStudentService _studentService;

        public StudentController(IStudentService studentService)
        {
            _studentService = studentService;
        }
        
        [HttpGet]
        [Route("GetAll")]
        public async Task<ActionResult<IEnumerable<Student>>> GetAllStudents()
        {            
            
            var students = await _studentService.GetAllAsync();
            return Ok(students);
            
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

            await _studentService.AddAsync(student);

            return Ok(student);
        }

        [HttpPut]
        [Route("Update")]
        public async Task<ActionResult<Student>> UpdateStudentsAsync([FromBody] UpdateStudentRequest request)
        {
            var student = await _studentService.GetById(request.Id);

            if(student == null)
            {
                throw new Exception($"Student with Id {request.Id} not found");
            }

            student.Name = request.Name;
            student.Email = request.Email;
            student.DOB = request.DOB;
            student.Status = StudentStatus.Active;

            await _studentService.UpdateAsync(student);

            return Ok(student);
        }
    }
}


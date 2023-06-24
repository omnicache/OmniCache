using System;
using Microsoft.AspNetCore.Mvc;
using RepoDemo.Api.Requests;
using RepoDemo.API.Requests;
using RepoDemo.Data.Model;
using RepoDemo.Services.Contract;
using RepoDemo.Services.Impl;

namespace RepoDemo.API.Controllers
{
    [Route("api/[controller]")]
    public class EnrolmentController : Controller
    {		
        private IEnrolmentService _enrolmentService;
        private ISubjectService _subjectService;
        private IStudentService _studentService;

        public EnrolmentController(IEnrolmentService enrolmentService, ISubjectService subjectService, IStudentService studentService)
        {
            _enrolmentService = enrolmentService;
            _subjectService = subjectService;
            _studentService = studentService;
        }

        
        [HttpGet]        
        [Route("students/{studentID}")]
        public async Task<ActionResult<IEnumerable<Subject>>> GetEnrolmentsForStudentAsync(long studentID)
        {
            
            var enrolments = await _enrolmentService.GetEnrolmentsForStudentAsync(studentID);
            var subjects = await _subjectService.GetAllSubjectsAsync();

            return Ok(enrolments);
            
        }

        [HttpGet]
        [Route("subjects/{subjectID}")]
        public async Task<ActionResult<IEnumerable<Student>>> GetEnrolmentsFoSubjectAsync(long subjectID)
        {
            
            var enrolments = await _enrolmentService.GetEnrolmentsFoSubjectAsync(subjectID);
            return Ok(enrolments);
            
        }

        [HttpPost]
        [Route("Add")]
        public async Task<ActionResult<Student>> AddEnrolmentsAsync([FromBody] AddEnrolmentRequest request)
        {
            Enrolment enrolment = await _enrolmentService.GetEnrolmentAsync(request.StudentId, request.SubjectId);
            if(enrolment!=null)
            {
                throw new Exception("Enrolment already exists");
            }

            enrolment = new Enrolment();
            enrolment.StudentId = request.StudentId;
            enrolment.SubjectId = request.SubjectId;
            
            await _enrolmentService.AddAsync(enrolment);

            return Ok(enrolment);
        }

        [HttpDelete]
        [Route("Delete")]
        public async Task<ActionResult<Student>> DeleteEnrolmentsAsync([FromBody] DeleteEnrolmentRequest request)
        {
            Enrolment enrolment = await _enrolmentService.GetEnrolmentAsync(request.StudentId, request.SubjectId);
            if(enrolment==null)
            {
                throw new Exception("Enrolment not found");
            }

            await _enrolmentService.DeleteAsync(enrolment);

            return Ok(enrolment);
        }
    }
}


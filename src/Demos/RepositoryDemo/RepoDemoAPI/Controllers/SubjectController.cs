using System;
using Microsoft.AspNetCore.Mvc;
using RepoDemo.Api.Requests;
using RepoDemo.API.Requests;
using RepoDemo.Data.Model;
using RepoDemo.Services.Contract;

namespace RepoDemo.API.Controllers
{
    [Route("api/[controller]")]
    public class SubjectController : Controller
    {		
        private ISubjectService _subjectService;

        public SubjectController(ISubjectService subjectService)
        {
            _subjectService = subjectService;
        }

        [HttpGet]
        [Route("GetAll")]
        public async Task<ActionResult<IEnumerable<Subject>>> GetAllSubjects()
        {
            
            var subjects = await _subjectService.GetAllSubjectsAsync();
            return Ok(subjects);
            
        }

        [HttpPost]
        [Route("AddSubject")]
        public async Task<ActionResult<Student>> AddSubjectsAsync([FromBody] AddSubjectRequest request)
        {
            Subject subject = new Subject();
            subject.Code = request.Code;
            subject.Name = request.Name;
            
            await _subjectService.AddAsync(subject);

            return Ok(subject);
        }
    }
}


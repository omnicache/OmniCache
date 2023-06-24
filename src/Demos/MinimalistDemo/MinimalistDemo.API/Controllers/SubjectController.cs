using System;
using Microsoft.AspNetCore.Mvc;
using MinimalistDemo.Data;
using MinimalistDemo.Data.Entity;

namespace MinimalistDemo.API.Controllers
{
    [Route("api/[controller]")]
    public class SubjectController : Controller
    {		
        private ICachedDBSession _cachedDBSession;

        public SubjectController(ICachedDBSession cachedDBSession)
        {
            _cachedDBSession = cachedDBSession;
        }

        [HttpGet]
        [Route("GetAll")]
        public async Task<ActionResult<IEnumerable<Subject>>> GetAllSubjects()
        {

            var subjects = await Subject.GetAllSubjectsAsync(_cachedDBSession);
            return Ok(subjects);

        }

        public class AddSubjectRequest
        {
            public string Code { get; set; }
            public string Name { get; set; }
        }

        [HttpPost]
        [Route("AddSubject")]
        public async Task<ActionResult<Student>> AddSubjectsAsync([FromBody] AddSubjectRequest request)
        {
            Subject subject = new Subject();
            subject.Code = request.Code;
            subject.Name = request.Name;

            await subject.InsertAsync(_cachedDBSession);

            return Ok(subject);
        }
    }
}


using System;
using Microsoft.AspNetCore.Mvc;
using MinimalistDemo.Data;
using MinimalistDemo.Data.Entity;

namespace MinimalistDemo.API.Controllers
{
    [Route("api/[controller]")]
    public class EnrolmentController : Controller
    {
        private ICachedDBSession _cachedDBSession;

        public EnrolmentController(ICachedDBSession cachedDBSession)
		{
            _cachedDBSession = cachedDBSession;
        }

        [HttpGet]
        [Route("students/{studentID}")]
        public async Task<ActionResult<IEnumerable<Subject>>> GetEnrolmentsForStudentAsync(long studentID)
        {

            var enrolments = await Enrolment.GetEnrolmentsForStudentAsync(_cachedDBSession, studentID);
            var subjects = await Subject.GetSubjectsAsync(_cachedDBSession, enrolments.Select(e=>e.SubjectId).ToList());

            return Ok(enrolments);

        }

        [HttpGet]
        [Route("subjects/{subjectID}")]
        public async Task<ActionResult<IEnumerable<Student>>> GetEnrolmentsFoSubjectAsync(long subjectID)
        {

            var enrolments = await Enrolment.GetEnrolmentsFoSubjectAsync(_cachedDBSession, subjectID);
            return Ok(enrolments);

        }


        public class AddEnrolmentRequest
        {
            public long StudentId { get; set; }
            public long SubjectId { get; set; }
        }

        [HttpPost]
        [Route("Add")]
        public async Task<ActionResult<Student>> AddEnrolmentsAsync([FromBody] AddEnrolmentRequest request)
        {
            Enrolment enrolment = await Enrolment.GetEnrolmentAsync(_cachedDBSession, request.StudentId, request.SubjectId);
            if (enrolment != null)
            {
                throw new Exception("Enrolment already exists");
            }

            enrolment = new Enrolment();
            enrolment.StudentId = request.StudentId;
            enrolment.SubjectId = request.SubjectId;

            await enrolment.InsertAsync(_cachedDBSession);

            return Ok(enrolment);
        }

        public class DeleteEnrolmentRequest
        {
            public long StudentId { get; set; }
            public long SubjectId { get; set; }
        }

        [HttpDelete]
        [Route("Delete")]
        public async Task<ActionResult<Student>> DeleteEnrolmentsAsync([FromBody] DeleteEnrolmentRequest request)
        {
            Enrolment enrolment = await Enrolment.GetEnrolmentAsync(_cachedDBSession, request.StudentId, request.SubjectId);
            if (enrolment == null)
            {
                throw new Exception("Enrolment not found");
            }

            await enrolment.DeleteAsync(_cachedDBSession);

            return Ok(enrolment);
        }
    }
}


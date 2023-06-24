using System;
using RepoDemo.Data.Model;

namespace RepoDemo.Data.Repository.Contract
{
	public interface IEnrolmentRepository : ICachedDBRepository<Enrolment>
    {
        Task<List<Enrolment>> GetEnrolmentsForStudentAsync(long studentID);
        Task<List<Enrolment>> GetEnrolmentsFoSubjectAsync(long subjectID);
        Task<Enrolment> GetEnrolmentAsync(long studentId, long subjectID);
    }
}


using System;
using RepoDemo.Data.Model;

namespace RepoDemo.Services.Contract
{
	public interface IEnrolmentService
	{
        Task AddAsync(Enrolment enrolment);
        Task<List<Subject>> GetEnrolmentsForStudentAsync(long studentID);
        Task<List<Student>> GetEnrolmentsFoSubjectAsync(long subjectID);
        Task<Enrolment> GetEnrolmentAsync(long studentId, long subjectID);
        Task DeleteAsync(Enrolment enrolment);
    }
}


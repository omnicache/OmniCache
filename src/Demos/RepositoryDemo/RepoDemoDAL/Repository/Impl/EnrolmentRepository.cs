using System;
using OmniCache;
using RepoDemo.Data.Model;
using RepoDemo.Data.Repository.Contract;

namespace RepoDemo.Data.Repository.Impl
{
	public class EnrolmentRepository: CachedDBRepository<Enrolment>, IEnrolmentRepository
    {		
        public EnrolmentRepository(ApplicationDbContext context) : base(context)
        {
        }

        public static Query<Enrolment> getStudentEnrolmentsQuery = new Query<Enrolment>(
                                e => e.StudentId == new QueryParam(1)
                                ).OrderByDesc(e => e.Id);

        
        public async Task<List<Enrolment>> GetEnrolmentsForStudentAsync(long studentID)
        {
            return await GetMultipleAsync(getStudentEnrolmentsQuery, studentID);
        }

        public static Query<Enrolment> getSubjectEnrolmentsQuery = new Query<Enrolment>(
                                e => e.SubjectId == new QueryParam(1)
                                ).OrderByDesc(e => e.Id);


        public async Task<List<Enrolment>> GetEnrolmentsFoSubjectAsync(long subjectID)
        {
            return await GetMultipleAsync(getSubjectEnrolmentsQuery, subjectID);
        }

        public static Query<Enrolment> getEnrolmentQuery = new Query<Enrolment>(
                                e => e.StudentId == new QueryParam(1)
                                       && e.SubjectId == new QueryParam(2)
                                ).OrderByDesc(e => e.Id);


        public async Task<Enrolment> GetEnrolmentAsync(long studentId, long subjectID)
        {
            return await GetAsync(getEnrolmentQuery, studentId, subjectID);
        }
    }
}


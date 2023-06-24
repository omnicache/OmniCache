using System;
using System.ComponentModel.DataAnnotations;
using OmniCache;

namespace MinimalistDemo.Data.Entity
{
    [Cacheable]
    public class Enrolment :BaseEntity<Enrolment>
	{
        [Key]
        public long Id { get; set; }

        public long StudentId { get; set; }

        public long SubjectId { get; set; }


        public Enrolment()
		{
		}


        public static Query<Enrolment> getStudentEnrolmentsQuery = new Query<Enrolment>(
                               e => e.StudentId == new QueryParam(1)
                               ).OrderByDesc(e => e.Id);


        public static async Task<List<Enrolment>> GetEnrolmentsForStudentAsync(ICachedDBSession db, long studentID)
        {
            return await db.GetMultipleAsync(getStudentEnrolmentsQuery, studentID);
        }

        public static Query<Enrolment> getSubjectEnrolmentsQuery = new Query<Enrolment>(
                                e => e.SubjectId == new QueryParam(1)
                                ).OrderByDesc(e => e.Id);


        public static async Task<List<Enrolment>> GetEnrolmentsFoSubjectAsync(ICachedDBSession db, long subjectID)
        {
            return await db.GetMultipleAsync(getSubjectEnrolmentsQuery, subjectID);
        }

        public static Query<Enrolment> getEnrolmentQuery = new Query<Enrolment>(
                                e => e.StudentId == new QueryParam(1)
                                       && e.SubjectId == new QueryParam(2)
                                ).OrderByDesc(e => e.Id);


        public static async Task<Enrolment> GetEnrolmentAsync(ICachedDBSession db, long studentId, long subjectID)
        {
            return await db.GetAsync(getEnrolmentQuery, studentId, subjectID);
        }
    }
}


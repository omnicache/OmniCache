using System;
using OmniCache;
using RepoDemo.Data.Model;
using RepoDemo.Data.Repository.Contract;
using RepoDemo.Data.Repository.Impl;
using RepoDemo.Services.Contract;
using OmniCache.Extension;

namespace RepoDemo.Services.Impl
{
	public class EnrolmentService: IEnrolmentService
	{		
        private IEnrolmentRepository _enrolmentRepository;
        private ISubjectRepository _subjectRepository;
        private IStudentRepository _studentRepository;

        public EnrolmentService(IEnrolmentRepository enrolmentRepository,
                                ISubjectRepository subjectRepository,
                                IStudentRepository studentRepository)
        {
            _enrolmentRepository = enrolmentRepository;
            _subjectRepository = subjectRepository;
            _studentRepository = studentRepository;
        }

        public async Task<List<Subject>> GetEnrolmentsForStudentAsync(long studentID)
        {
            var enrolments = await _enrolmentRepository.GetEnrolmentsForStudentAsync(studentID);
            var subjects = await _subjectRepository.GetSubjectsAsync(enrolments.GetList(e=>e.SubjectId));            
            return subjects;
        }

        public async Task<List<Student>> GetEnrolmentsFoSubjectAsync(long subjectID)
        {
            var enrolments = await _enrolmentRepository.GetEnrolmentsFoSubjectAsync(subjectID);
            var students = await _studentRepository.GetStudentsAsync(enrolments.Select(e => e.StudentId).ToList());
            return students;
        }

        public async Task AddAsync(Enrolment enrolment)
        {
            await _enrolmentRepository.AddAsync(enrolment);
        }

        public async Task<Enrolment> GetEnrolmentAsync(long studentId, long subjectID)
        {
            return await _enrolmentRepository.GetEnrolmentAsync(studentId, subjectID);
        }

        public async Task DeleteAsync(Enrolment enrolment)
        {
            await _enrolmentRepository.DeleteAsync(enrolment);
        }
    }
}


using System;
using RepoDemo.Data.Model;
using RepoDemo.Data.Repository.Contract;
using RepoDemo.Data.Repository.Impl;
using RepoDemo.Services.Contract;

namespace RepoDemo.Services.Impl
{
	public class SubjectService:ISubjectService
	{
		
        private ISubjectRepository _subjectRepository;

        public SubjectService(ISubjectRepository subjectRepository)
        {
            _subjectRepository = subjectRepository;
        }

        public async Task<List<Subject>> GetAllSubjectsAsync()
        {
            return await _subjectRepository.GetAllSubjectsAsync();
        }

        public async Task AddAsync(Subject subject)
        {
            await _subjectRepository.AddAsync(subject);
        }
    }
}


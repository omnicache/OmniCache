using System;
using RepoDemo.Data.Model;

namespace RepoDemo.Services.Contract
{
	public interface ISubjectService
	{
        Task<List<Subject>> GetAllSubjectsAsync();
        Task AddAsync(Subject subject);

    }
}


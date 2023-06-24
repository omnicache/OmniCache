using System;
using System.Linq.Expressions;
using RepoDemo.Data.Repository.Contract;
using RepoDemo.Data.Model;
using OmniCache;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace RepoDemo.Data.Repository.Impl
{

    
    public class SubjectRepository : CachedDBRepository<Subject>, ISubjectRepository
    {
        public SubjectRepository(ApplicationDbContext context) : base(context)
        {
        }

        public static Query<Subject> getAllSubjectsQuery = new Query<Subject>()
                                .OrderByDesc(b => b.Name);    
        
        public async Task<List<Subject>> GetAllSubjectsAsync()
        {
            return await GetMultipleAsync(getAllSubjectsQuery);            
        }


        public static Query<Subject> getSubjectsQuery = new Query<Subject>(
                                b => new QueryParam(1).Contains(b.Id) 
                                )
                                .OrderByDesc(b => b.Name);

        public async Task<List<Subject>> GetSubjectsAsync(List<long> subjectIds)
        {
            return await GetMultipleAsync(getSubjectsQuery, subjectIds);
        }

    }
}


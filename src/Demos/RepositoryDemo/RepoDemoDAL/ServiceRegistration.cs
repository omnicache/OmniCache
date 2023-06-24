using System;
using Microsoft.Extensions.DependencyInjection;
using RepoDemo.Data.Repository.Contract;
using RepoDemo.Data.Repository.Impl;

namespace RepoDemo.Data
{
	public static class ServiceRegistration
    {
        public static IServiceCollection AddDataServices(this IServiceCollection services)
        {           
            services.AddTransient<IStudentRepository, StudentRepository>();
            services.AddTransient<ISubjectRepository, SubjectRepository>();
            services.AddTransient<IEnrolmentRepository, EnrolmentRepository>();

            return services;
        }
    }
}


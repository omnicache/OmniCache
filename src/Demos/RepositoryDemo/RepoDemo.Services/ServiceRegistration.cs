using System;
using Microsoft.Extensions.DependencyInjection;
using RepoDemo.Data.Repository.Contract;
using RepoDemo.Data.Repository.Impl;
using RepoDemo.Services.Contract;
using RepoDemo.Services.Impl;

namespace RepoDemo.Services
{
	public static class ServiceRegistration
    {
		
        public static IServiceCollection AddServiceServices(this IServiceCollection services)
        {
            services.AddTransient<IStudentService, StudentService>();
            services.AddTransient<ISubjectService, SubjectService>();
            services.AddTransient<IEnrolmentService, EnrolmentService>();

            return services;
        }
    }
}


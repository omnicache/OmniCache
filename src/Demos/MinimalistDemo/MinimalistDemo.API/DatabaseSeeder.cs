using System;
using MinimalistDemo.Data;
using MinimalistDemo.Data.Entity;

namespace MinimalistDemo.API
{
	public class DatabaseSeeder
	{
        private readonly IConfiguration _configuration;
        private ApplicationDbContext _dbContext;

        public DatabaseSeeder(IConfiguration configuration, ApplicationDbContext dbContext)
        {
            _configuration = configuration;
            _dbContext = dbContext;
        }

        public async Task SeedAsync()
        {
            bool useInMemoryDB = _configuration.GetValue<bool>("UseInMemoryDatabase");
            if (useInMemoryDB)
            {
                await InsertData();
            }
        }

        private async Task InsertData()
        {
            var students = new List<Student>
            {
                new Student {Id=1, Name = "Abe Anderson", Email = "Abe@email.com", DOB = new DateTime(1990, 4, 14), Status=StudentStatus.Active },
                new Student {Id=2, Name = "Bob Becker", Email = "Bob@email.com", DOB = new DateTime(1990, 1, 10), Status=StudentStatus.Active },
                new Student {Id=3, Name = "Cathy Clark", Email = "Cathy@email.com", DOB = new DateTime(1995, 5, 20), Status=StudentStatus.Active }
            };

            var subjects = new List<Subject>
            {
                new Subject {Id=1, Code="MA", Name="Maths" },
                new Subject {Id=2, Code="EN", Name="English" },
                new Subject {Id=3, Code="MU", Name="Music" },
                new Subject {Id=4, Code="PH", Name="Physics" }
            };

            var enrolments = new List<Enrolment>
            {
                new Enrolment {StudentId=1,SubjectId=1 },
                new Enrolment {StudentId=1,SubjectId=2 },
                new Enrolment {StudentId=1,SubjectId=3 },
                new Enrolment {StudentId=1,SubjectId=4 },
                new Enrolment {StudentId=2,SubjectId=1 },
                new Enrolment {StudentId=2,SubjectId=2 },
                new Enrolment {StudentId=2,SubjectId=3 },
                new Enrolment {StudentId=3,SubjectId=1 },
                new Enrolment {StudentId=3,SubjectId=2 },
                new Enrolment {StudentId=4,SubjectId=1 }
            };

            await _dbContext.Students.AddRangeAsync(students);
            await _dbContext.Subjects.AddRangeAsync(subjects);
            await _dbContext.Enrolments.AddRangeAsync(enrolments);

            _dbContext.SaveChanges();
        }
    }
}


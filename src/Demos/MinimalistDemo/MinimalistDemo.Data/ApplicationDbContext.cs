using System;
using System.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MinimalistDemo.Data.Entity;

namespace MinimalistDemo.Data
{
	public class ApplicationDbContext : DbContext
    {                
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<Student> Students { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<Enrolment> Enrolments { get; set; }
    }
}


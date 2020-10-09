using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Test1Api.Models
{
    public class ManagerContext: DbContext
    {
        public ManagerContext(DbContextOptions<ManagerContext> options) : base(options) { }
        //mapping model class
        public DbSet<Course> Courses { get; set; }
        public DbSet<Student> Students { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Student>()
                .HasOne<Course>(s => s.Course)
                .WithMany(c => c.Students)
                .HasForeignKey(s => s.CourseId);
        }
    }
}

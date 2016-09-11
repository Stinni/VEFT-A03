using Microsoft.EntityFrameworkCore;
using A03.Entities;

namespace A03.Services
{
    /// <summary>
    /// A custom DbContext for the database used in this project
    /// </summary>
    public class AppDataContext : DbContext
    {
        /// <summary>
        /// DbSet Courses, links the Course entity to the Courses table in the database
        /// </summary>
        public DbSet<Course> Courses { get; set; }

        /// <summary>
        /// DbSet Students, links the Student entity to the Students table in the database
        /// </summary>
        public DbSet<Student> Students { get; set; }

        /// <summary>
        /// DbSet CourseTemplates, links the CourseTemplate entity to the CourseTemplates table in the database
        /// </summary>
        public DbSet<CourseTemplate> CourseTemplates { get; set; }

        /// <summary>
        /// DbSet StudentConnections, links the StudentConnection entity to the StudentConnections table in the database
        /// </summary>
        public DbSet<StudentCourseRelation> StudentCourseRelations { get; set; }

        /// <summary>
        /// DbSet StudentWaitinglistRelations, links the StudentWaitinglistRelation entity to the StudentWaitinglistRelations
        /// table in the database
        /// </summary>
        public DbSet<StudentWaitinglistRelation> StudentWaitinglistRelations { get; set; }

        /// <exclude />
        public AppDataContext(DbContextOptions<AppDataContext> options)
            : base(options)
        {
        }

        /// <exclude />
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }
    }
}

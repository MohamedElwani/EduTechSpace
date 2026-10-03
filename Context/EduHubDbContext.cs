using EduTechSpace.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EduTechSpace.Context
{
	public class EduHubDbContext : IdentityDbContext<ApplicationUser ,IdentityRole<int>,int>
	{
		public DbSet<Attendance> Attendances { get; set; }
		public DbSet<Courses> Courses { get; set; }
		public DbSet<Lecture> lectures { get; set; }
		public DbSet<Exam> exams { get; set; }
		public DbSet<StudentExam> StudentExams { get; set; }
		public DbSet<UserCourse> UserCourses { get; set; }
		public DbSet<UserYear> UserYears { get; set; }
		public DbSet<RefreshToken> RefreshTokens { get; set; }
		//30/5/2026
		public DbSet<EduTechSpace.Models.Errors> Errors { get; set; }
		//30/5/2026
		public EduHubDbContext()
		{
			
		}
		public EduHubDbContext(DbContextOptions options):base(options) 
		{
			
		}
		protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);

			// UserCourse
			builder.Entity<UserCourse>()
				.HasKey(uc => new { uc.UserId, uc.CourseId });

			builder.Entity<UserCourse>()
				.HasOne(uc => uc.User)
				.WithMany(u => u.UserCourses)
				.HasForeignKey(uc => uc.UserId);

			builder.Entity<UserCourse>()
				.HasOne(uc => uc.Course)
				.WithMany(c => c.UsersCourse)
				.HasForeignKey(uc => uc.CourseId);

			// StudentExam
			builder.Entity<StudentExam>()
				.HasKey(se => new { se.StudentId, se.ExamId });

			builder.Entity<StudentExam>()
				.HasOne(se => se.Student)
				.WithMany(u => u.ExamResults)
				.HasForeignKey(se => se.StudentId);

			builder.Entity<StudentExam>()
				.HasOne(se => se.Exam)
				.WithMany(e => e.StudentExams)
				.HasForeignKey(se => se.ExamId);

			// UserYear
			builder.Entity<UserYear>()
				.HasKey(uy => new { uy.UserId, uy.id });

			// RefreshToken
			builder.Entity<RefreshToken>()
				.HasOne(rt => rt.User)
				.WithMany()
				.HasForeignKey(rt => rt.UserId)
				.OnDelete(DeleteBehavior.Cascade);

			builder.Entity<RefreshToken>()
				.HasIndex(rt => rt.Token)
				.IsUnique(); // fast lookup + guarantees uniqueness
		}
	}
}

using EduTechSpace.Context;
using EduTechSpace.interfaces;
using EduTechSpace.interfaces.ServicesInterfaces;
using EduTechSpace.Interfaces;
using EduTechSpace.Middlewares;
using EduTechSpace.Models;
using EduTechSpace.Repositories;
using EduTechSpace.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EduTechSpace
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            
            // Add services to the container.

            builder.Services.AddControllers();
            builder.Services.AddHttpContextAccessor(); // allows IHttpContextAccessor injection in services
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();
            //add database injection 
            builder.Services.AddDbContext<EduHubDbContext>(
                options => options/*.UseLazyLoadingProxies()*/.UseSqlServer(builder.Configuration.GetConnectionString("cloud"))
                );
            //add swagger values to string in enum 
            builder.Services.AddControllers().AddJsonOptions(
                options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                }
                );
            
            //add identity injection 
            builder.Services.AddIdentity<ApplicationUser, IdentityRole<int>>().AddEntityFrameworkStores<EduHubDbContext>();
            //Add swagger 
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            //replace cookie with jwt
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(option =>
            {
                option.RequireHttpsMetadata = false;
                option.SaveToken = true;
                option.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = false,
                    ValidateIssuerSigningKey = true,
                    ClockSkew = TimeSpan.Zero,
                    ValidateLifetime = true,
                    ValidIssuer = builder.Configuration["Jwt:Issur"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecurityKey"]))

                };
            });
			//Services injects
			builder.Services.AddScoped<AccountService>();
            builder.Services.AddScoped<AdminService>();
            builder.Services.AddScoped<ICourseRepository, CourseRepository>();
            builder.Services.AddScoped<ICourseService,CourseService>();
            builder.Services.AddScoped<ILectureRepository, LectureRepository>();
            builder.Services.AddScoped<ILectureService, LectureService>();
            builder.Services.AddScoped<IAttendanceRepository, AttendanceRepository>();
            builder.Services.AddScoped<IAttendanceService, AttendanceService>();
            builder.Services.AddScoped<IExamRepository, ExamRepository>();
            builder.Services.AddScoped<IStudentExamRepository, StudentExamRepository>();
            builder.Services.AddScoped<IExamService, ExamService>();
            builder.Services.AddScoped<IExamResultService, ExamResultService>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IEnrollmentRepository, EnrollmentRepository>();
            builder.Services.AddScoped<IEnrollmentService, EnrollmentService>();    
			//Add Cors Policy 
			builder.Services.AddCors(options =>
			{
				options.AddPolicy("EduStuHubPloicy 1.0.0v", policyOptions =>
				{
					policyOptions.AllowAnyHeader().AllowAnyOrigin().AllowAnyMethod();
				});
			});
            //register automapper
            builder.Services.AddAutoMapper(typeof(Program));


           
			var app = builder.Build();
            //make exception handeler last layer so it could handel the exceptions when outComes
            //30/5/2026
            app.UseMiddleware<ExceptionMiddleware>();


            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }
            //Add Auto Migrations Updata 
            using var scope = app.Services.CreateScope();
            var services = scope.ServiceProvider;
            var _dbContext = services.GetRequiredService<EduHubDbContext>();
            try
            {
                await _dbContext.Database.MigrateAsync();
            }
            catch (Exception ex)
            {
                var loggerFactory = services.GetRequiredService<ILoggerFactory>();
                var logger = loggerFactory.CreateLogger<Program>();
                logger.LogError(ex, "Error accured while update DB");
            }
            app.UseStatusCodePagesWithReExecute("/errors/{0}");//handel Notfound , and Unoutharized,one request noredirect
            app.UseStaticFiles();
            app.UseHttpsRedirection();
            app.UseCors("EduStuHubPloicy 1.0.0v");
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseSwagger();
            app.UseSwaggerUI();

            app.MapControllers();

            app.Run();
        }
    }
}

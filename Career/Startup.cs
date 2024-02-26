using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using Career.Data;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Career
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            // Adding the db context to the services
            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            // Adding the cookies based authantications 
            services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>();

            // Configuring login/password options
            services.Configure<IdentityOptions>(options =>
            {
                // Make really weak passwords possible
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 6;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;


            });

            // Alter application cookie info
            services.ConfigureApplicationCookie(options =>
            {
                // Redirect to /login 
                options.LoginPath = "/Account/Login";

                // Access denied path
                options.AccessDeniedPath = "/Account/Login";

                // Change cookie timeout to expire after 7 days 
                options.ExpireTimeSpan = TimeSpan.FromDays(7);
            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider serviceProvider)
        {
            // Setup identity 
            app.UseAuthentication();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            // Making sure that the database and roles are created
            Task.Run(() => this.CreateRolesandAdminUser(serviceProvider)).Wait();

        }

        /// <summary>
        /// Creating the roles for our application and adding the admin 
        /// </summary>
        /// <returns></returns>
        private async Task CreateRolesandAdminUser(IServiceProvider serviceProvider)
        {
            // Getting the required services
            var mContext = serviceProvider.GetService<ApplicationDbContext>();
            var mRoleManager = serviceProvider.GetService<RoleManager<IdentityRole>>();
            var mUserManager = serviceProvider.GetService<UserManager<ApplicationUser>>();

            // Check if we have the database ..
            if (mContext.Database.GetService<IRelationalDatabaseCreator>().Exists())
                return;

            // if note
            // Make sure that our database exist
            mContext.Database.EnsureCreated();

            // Check if the (Admin) role exist
            bool x = await mRoleManager.RoleExistsAsync("Admin");

            // If not
            if (!x)
            {
                // We make the (Admin) role   
                await mRoleManager.CreateAsync(new IdentityRole() { Name = "Admin" });

                // Making the admin user       
                var user = new ApplicationUser();
                user.UserName = "HTUAdmin";
                user.Email = "Admin@com";
                string userPWD = "AdminPassward";

                // Register the admin user
                IdentityResult chkUser = await mUserManager.CreateAsync(user, userPWD);

                // If the register succeed  
                if (chkUser.Succeeded)
                {
                    // Give him the (Admin) role
                    var result1 = await mUserManager.AddToRoleAsync(user, "Admin");
                }
            }

            // Check if the (User) role exist
            x = await mRoleManager.RoleExistsAsync("User");

            // If not
            if (!x)
            {
                // Making the (User) role
                await mRoleManager.CreateAsync(new IdentityRole() { Name = "User" });
            }

            // Check if the (Employer) role exist
            x = await mRoleManager.RoleExistsAsync("Employer");

            // If not
            if (!x)
            {
                // Making the (Employer) role
                await mRoleManager.CreateAsync(new IdentityRole() { Name = "Employer" });
            }

            // Check if the (Student) role exist
            x = await mRoleManager.RoleExistsAsync("Student");

            // If not
            if (!x)
            {
                // Making the (Student) role
                await mRoleManager.CreateAsync(new IdentityRole() { Name = "Student" });
            }


            #region Dummy Data

            // Adding dummy data
            await mUserManager.CreateAsync(new ApplicationUser() { UserName = "StudentTest", Email = "ST@com", IsVerified = true }, "st123456");

            var stUser = await mUserManager.FindByEmailAsync("ST@com");

            await mUserManager.AddToRoleAsync(stUser, "User");
            await mUserManager.AddToRoleAsync(stUser, "Student");

            var student = new Student()
            {
                User = stUser,
                FirstName = "dummy",
                LastName = "student",
                Age = 22,
                City = "Amman",
                Country = "Jordan",
                Image = "DefaultStudent.png",
                Major = "IT",
                Skills = new List<Skill>() {
                     new Skill() {SkillName = "C#" },
                     new Skill() {SkillName = "MVC" },
                     new Skill() {SkillName = "ASP.Net" },
                     }
            };

            mContext.Students.Add(student);

            await mUserManager.CreateAsync(new ApplicationUser() { UserName = "EmployerTest", Email = "ET@com", IsVerified = true }, "et123456");

            var etUser = await mUserManager.FindByEmailAsync("ET@com");

            await mUserManager.AddToRoleAsync(etUser, "User");
            await mUserManager.AddToRoleAsync(etUser, "Employer");

            var employer = new Employer()
            {
                User = etUser,
                Address = "Amman",
                Describtion = "For it porposses",
                EmployerName = "HTU Gaint",
                MobileNumber = "0000000000",
                Image = "DefaultEmployers.png",

            };
            employer.Jobs = new List<Job>()
            {
                new Job()
                {
                    JobName="Jenior Develovor",
                    JobOwner=employer,
                    MaxAge=50,
                    MinAge=18,
                    IsApproved=true,
                    IsExpired=false,
                    Skills=new List<Skill>()
                    {
                        new Skill(){ SkillName="MVC"}
                    }
                },
                new Job()
                {
                    JobName="Jenior Designer",
                    JobOwner=employer,
                    MaxAge=50,
                    MinAge=18,
                    IsApproved=true,
                    IsExpired=false,
                    Skills=new List<Skill>()
                    {
                        new Skill(){ SkillName="ASP.Net"}
                    }
                }
            };

            mContext.Employers.Add(employer);

            mContext.Discussions.Add(new Discussion()
            {
                From = stUser,
                To = etUser,
                SendingDate = DateTime.Now,
                Message = "Hello, Will you hire me, i'm more than enough for this job, you won't regret it...",
            });

            mContext.SaveChanges();

            #endregion

        }


    }
}

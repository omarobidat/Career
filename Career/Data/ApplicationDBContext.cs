using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Web;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Career.Data
{
    public class ApplicationDbContext:IdentityDbContext<ApplicationUser>
    {
        /// <summary>
        /// Default contructor
        /// </summary>
        /// <param name="options">the options to be passed to the base constructor</param>
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options):base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // using fluent API rules for the database 
            // Every user should be able to only be linked to one student
            builder.Entity<Student>().HasIndex(s => s.UserId).IsUnique();

            // Or linked to only one Employer
            builder.Entity<Employer>().HasIndex(e => e.UserId).IsUnique();

        }

        /// <summary>
        /// Students Table
        /// </summary>
        public DbSet<Student> Students { get; set; }

        /// <summary>
        /// Employers Table
        /// </summary>
        public DbSet<Employer> Employers { get; set; }

        /// <summary>
        /// Skills Table
        /// </summary>
        public DbSet<Skill> Skills { get; set; }

        /// <summary>
        /// Jobs table
        /// </summary>
        public DbSet<Job> Jobs { get; set; }

        /// <summary>
        /// Discussion table
        /// </summary>
        public DbSet<Discussion> Discussions { get; set; }
    }

}

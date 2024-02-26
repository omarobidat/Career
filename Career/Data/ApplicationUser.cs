using Microsoft.AspNetCore.Identity;

namespace Career.Data
{
    /// <summary>
    /// The user model for every user regestered in this site
    /// </summary>
    public class ApplicationUser:IdentityUser
    {
        /// <summary>
        /// Is verified by the admin 
        /// </summary>
        public bool IsVerified { get; set; }

        /// <summary>
        /// If the admin rejected this user
        /// </summary>
        public bool IsRejected { get; set; }

        /// <summary>
        /// The reason why the admin rejected this user
        /// </summary>
        public string RejectionReason { get; set; }

        /// <summary>
        /// If this user is linked to a student, this is the student
        /// </summary>
        public Student Student { get; set; }

        /// <summary>
        /// If this user is linked to an employer, this is the employer
        /// </summary>
        public Employer Employer { get; set; }
    }
}

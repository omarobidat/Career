using Career.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Career.Models
{
    /// <summary>
    /// The view model used in the student profile actions 
    /// </summary>
    public class StudentProfileViewModel
    {
        /// <summary>
        /// The current user
        /// </summary>
        public Student Student { get; set; }

        /// <summary>
        /// The discussion that the current user involved in 
        /// </summary>
        public List<Discussion> Discussions { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Career.Data
{
    /// <summary>
    /// Jobs proposed by the employers and their informations
    /// </summary>
    public class Job
    {
        /// <summary>
        /// Primery key, the ID
        /// </summary>
        [Key]
        public int JobID { get; set; }

        /// <summary>
        /// The job position or the name of the job
        /// </summary>
        [Required]
        public string JobName { get; set; }

        /// <summary>
        /// The employer who proposed this job
        /// </summary>
        [Required]
        public Employer JobOwner { get; set; }

        /// <summary>
        /// Age restriction, if any, the minimum age
        /// </summary>
        public int MinAge { get; set; }

        /// <summary>
        /// Age restriction, if any, the maximum age
        /// </summary>
        public int MaxAge { get; set; }

        /// <summary>
        /// The skills required for this job
        /// </summary>
        [Required]
        public ICollection<Skill> Skills { get; set; }

        /// <summary>
        /// If the job was approved by the admin
        /// </summary>
        public bool IsApproved { get; set; }

        /// <summary>
        /// If the job was taken back by the employer
        /// </summary>
        public bool IsExpired { get; set; }


    }
}

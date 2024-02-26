using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Career.Data
{
    /// <summary>
    /// The employer model which containt the information about the employer
    /// </summary>
    public class Student
    {
        /// <summary>
        /// Primery key, the ID
        /// </summary>
        [Key]
        public int StudentId { get; set; }
                
        /// <summary>
        /// The first name of the student
        /// </summary>
        [StringLength(255)]
        [Required(AllowEmptyStrings = false, ErrorMessage = "First Name is Required.")]
        public string FirstName { get; set; }

        /// <summary>
        /// The last name of the student
        /// </summary>
        [StringLength(255)]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Last Name is Required.")]
        public string LastName { get; set; }

        /// <summary>
        /// The age of the student
        /// </summary>
        [Range(18, 70)]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Age is Required.")]
        public int Age { get; set; }

        /// <summary>
        /// The major of the student
        /// </summary>
        [StringLength(255)]
        [Required(AllowEmptyStrings = false, ErrorMessage = " Major is Required.")]
        public string Major { get; set; }

        /// <summary>
        /// The city address of the student
        /// </summary>
        [StringLength(255)]
        [Required(AllowEmptyStrings = false, ErrorMessage = "City is Required.")]
        public string City { get; set; }

        /// <summary>
        /// The country address of the student
        /// </summary>
        [StringLength(255)]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Country is Required.")]
        public string Country { get; set; }

        /// <summary>
        /// The name of the cv file saved on the web server
        /// </summary>
        public string CV { get; set; }
        
        /// <summary>
        /// The name of the profile picture file of the student
        /// </summary>
        public string Image { get; set; }

        /// <summary>
        /// The list of skills this user have
        /// </summary>
        public ICollection<Skill> Skills { get; set; }

        /// <summary>
        /// The user id of the user linked to this student
        /// </summary>
        [Required]
        public string UserId { get; set; }

        /// <summary>
        /// The user linked to this student
        /// </summary>
        [Required]
        //[ForeignKey("UserId")]
        public ApplicationUser User { get; set; }

    }
}

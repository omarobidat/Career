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
    public class Employer
    {
        /// <summary>
        /// Primery key, the ID
        /// </summary>
        [Key]
        public int EmployerID { get; set; }

        /// <summary>
        /// The name of the employer, company or what ever the employer is 
        /// </summary>
        [StringLength(255)]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Employer Name is Required.")]
        public string EmployerName { get; set; }

        /// <summary>
        /// Basic describtion about the employer
        /// </summary>
        [StringLength(255)]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Description is Required.")]
        public string Describtion { get; set; }

        /// <summary>
        /// The address of the employer
        /// </summary>
        [StringLength(255)]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Address is Required.")]
        public string Address { get; set; }

        /// <summary>
        /// The contact number of the employer
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessage = "Mobile Number is Required.")]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Invalid Mobile Number.")]
        public string MobileNumber { get; set; }

        /// <summary>
        /// The name of the  profile picture file of the employer
        /// </summary>
        public string Image { get; set; }

        /// <summary>
        /// The jobs proposed by this employer
        /// </summary>
        public ICollection<Job> Jobs { get; set; }

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

        //TODO Notification

    }
}

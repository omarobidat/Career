using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Career.Models
{
    /// <summary>
    /// The view model to be filled with the login form, with all the validations
    /// </summary>
    public class LoginViewModel
    {
        /// <summary>
        /// The email of the user
        /// </summary>
        [Required]
        [Display(Name = "Email")]
        [EmailAddress]
        public string Email { get; set; }

        /// <summary>
        /// The passward of the user
        /// </summary>
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        /// <summary>
        /// To stay logged in after closing the browser
        /// </summary>
        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }

    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Career.Data
{
    /// <summary>
    /// All the skills, 
    /// </summary>
    public class Skill
    {
        /// <summary>
        /// Primery key, the ID
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// The skill name
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessage = "Skill Name is Required.")]
        [StringLength(255)]
        public string SkillName { get; set; }
    }    
}

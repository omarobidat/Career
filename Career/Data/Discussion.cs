using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Career.Data
{
    /// <summary>
    /// The discussion model which allow the users to converse with eachothers
    /// </summary>
    public class Discussion
    {
        /// <summary>
        /// Primery key, the ID
        /// </summary>
        [Key]
        public int MessageID { get; set; }

        /// <summary>
        /// The user who send the message
        /// </summary>
        public ApplicationUser From { get; set; }

        /// <summary>
        /// The user who is supposed to receive this message
        /// </summary>
        public ApplicationUser To { get; set; }

        /// <summary>
        /// The message body
        /// </summary>
        [Required]
        public string Message { get; set; }

        /// <summary>
        /// The date this message was sent
        /// </summary>
        [Required]
        public DateTime SendingDate { get; set; }

    }
}

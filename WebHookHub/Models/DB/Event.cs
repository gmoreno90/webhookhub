using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebHookHub.Models.DB
{
    /// <summary>
    /// Event
    /// </summary>
    public class Event
    {
        /// <summary>
        /// ID
        /// </summary>
        [Key]
        public int ID { get; set; }
        /// <summary>
        /// Code
        /// </summary>
        [Required]
        [MaxLength(50),MinLength(2)]
        public string Code { get; set; }
        /// <summary>
        /// Description
        /// </summary>
        [Required]
        [MaxLength(100), MinLength(2)]
        public string Description { get; set; }
        /// <summary>
        /// Regex to find ID
        /// </summary>
        public string RegexID { get; set; }
        /// <summary>
        /// Regex to find ID 2
        /// </summary>
        public string RegexIDExtra { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebHookHub.Models.DB
{
    /// <summary>
    /// Data To Post
    /// </summary>
    public class DataToPost
    {
        /// <summary>
        /// ID
        /// </summary>
        [Key]
        public string ID { get; set; }
        /// <summary>
        /// Content
        /// </summary>
        [Required]
        [MaxLength(500000)]
        public string Content { get; set; }
        /// <summary>
        /// request Date
        /// </summary>
        [Required]
        public DateTime RequestDate { get; set; }

    }
}

using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;

namespace WebHookHub.Models.DB
{
    /// <summary>
    /// CustomJobID
    /// </summary>
    [Index(nameof(ExternalJobID), IsUnique = true)]
    public class CustomJobID
    {
        /// <summary>
        /// ID
        /// </summary>
        [Key]
        public int ID { get; set; }
        /// <summary>
        /// ExternalJobID
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string ExternalJobID { get; set; }
        /// <summary>
        /// InternalJobID
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string InternalJobID { get; set; }
        /// <summary>
        /// CreationDateTime
        /// </summary>
        [Required]
        public DateTime CreationDateTime { get; set; }
    }
}

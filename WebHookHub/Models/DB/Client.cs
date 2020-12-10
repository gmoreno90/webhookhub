using System.ComponentModel.DataAnnotations;

namespace WebHookHub.Models.DB
{
    /// <summary>
    /// Client
    /// </summary>
    public class Client
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
        [MaxLength(50), MinLength(2)]
        public string Code { get; set; }
        /// <summary>
        /// Description
        /// </summary>
        [Required]
        [MaxLength(100), MinLength(2)]
        public string Description { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

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
        
    }
}

using System;
using System.ComponentModel.DataAnnotations;

namespace WebHookHub.Models.DB
{
    /// <summary>
    /// Api log Item
    /// </summary>
    public class ApiLogItem
    {
        /// <summary>
        /// Id
        /// </summary>
        [Key]
        public string Id { get; set; }

        /// <summary>
        /// Request Time
        /// </summary>
        [Required]
        public DateTime RequestTime { get; set; }
        /// <summary>
        /// Response time in ms
        /// </summary>
        [Required]
        public long ResponseMillis { get; set; }
        /// <summary>
        /// Status code
        /// </summary>
        [Required]
        public int StatusCode { get; set; }
        /// <summary>
        /// Method
        /// </summary>
        [Required]
        [MaxLength(500)]
        public string Method { get; set; }
        /// <summary>
        /// Path
        /// </summary>
        [Required]
        [MaxLength(1000)]
        public string Path { get; set; }
        /// <summary>
        /// Query string
        /// </summary>
        [MaxLength(5000)]
        public string QueryString { get; set; }
        /// <summary>
        /// Request body
        /// </summary>
        [MaxLength(50000)]
        public string RequestBody { get; set; }
        /// <summary>
        /// Response body
        /// </summary>
        [MaxLength(50000)]
        public string ResponseBody { get; set; }
        /// <summary>
        /// Reques token
        /// </summary>
        [MaxLength(200)]
        [Required]
        public string RequestToken { get; set; }
    }
}

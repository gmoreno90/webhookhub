using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebHookHub.Models.DB
{
    /// <summary>
    /// 
    /// </summary>
    public class ClientEvent
    {
        /// <summary>
        /// ID
        /// </summary>
        [Key]
        public int ID { get; set; }
        /// <summary>
        /// ClientId
        /// </summary>
        [Required]
        public int ClientId { get; set; }
        /// <summary>
        /// EventId
        /// </summary>
        [Required]
        public int EventId { get; set; }
        /// <summary>
        /// Client
        /// </summary>
        public virtual Client Client { get; set; }
        /// <summary>
        /// Event
        /// </summary>
        public virtual Event Event { get; set; }
        /// <summary>
        /// PostUrl
        /// </summary>
        [Required]
        [MaxLength(5000), MinLength(2)]
        public string PostUrl { get; set; }
        /// <summary>
        /// UserName
        /// </summary>
        [MaxLength(500)]
        public string UserName { get; set; }
        /// <summary>
        /// PassWord
        /// </summary>
        [MaxLength(500)]
        public string PassWord { get; set; }
        /// <summary>
        /// Enable
        /// </summary>

        public bool Enable { get; set; }

    }
}

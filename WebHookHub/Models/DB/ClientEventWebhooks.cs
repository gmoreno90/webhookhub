﻿using System.ComponentModel.DataAnnotations;

namespace WebHookHub.Models.DB
{
    /// <summary>
    /// ClientEventWebhooks
    /// </summary>
    public class ClientEventWebhooks
    {
        /// <summary>
        /// ID
        /// </summary>
        [Key]
        public int ID { get; set; }
        /// <summary>
        /// ClientEvent
        /// </summary>
        [Required]
        public int ClientEventId { get; set; }
        /// <summary>
        /// Client Event
        /// </summary>
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual ClientEvent ClientEvent { get; set; }
        /// <summary>
        /// PostUrl
        /// </summary>
        [Required]
        [MaxLength(5000), MinLength(2)]
        public string PostUrl { get; set; }
        
        /// <summary>
        /// Enable
        /// </summary>
        public bool Enable { get; set; }
        /// <summary>
        /// ExpectedContentResulto
        /// </summary>
        [MaxLength(5000)]
        public string ExpectedContentResult { get; set; }

        /// <summary>
        /// Header Authorization Value
        /// </summary>
        [MaxLength(4000)]
        public string HeaderAuthorizationValue { get; set; }

    }
}

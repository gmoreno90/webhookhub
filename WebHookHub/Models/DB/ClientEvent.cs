using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

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
        /// Client Information
        /// </summary>
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual Client Client { get; set; }
        /// <summary>
        /// EventId
        /// </summary>
        [Required]
        public int EventId { get; set; }
        /// <summary>
        /// Event Information
        /// </summary>
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual Event Event { get; set; }

        /// <summary>
        /// Content Reponse OK
        /// </summary>
        [MaxLength(5000)]
        public string ContentReponseOk { get; set; }
        /// <summary>
        /// Content Reponse OK
        /// </summary>
        [MaxLength(5000)]
        public string ContentReponseError { get; set; }

        /// <summary>
        /// Enable
        /// </summary>
        public bool Enable { get; set; }
        /// <summary>
        /// Client Event Webhooks
        /// </summary>
        [System.Text.Json.Serialization.JsonIgnore]
        public IEnumerable<ClientEventWebhooks> ClientEventWebhooks { get; set; }

    }
}

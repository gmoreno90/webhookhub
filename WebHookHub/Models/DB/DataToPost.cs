using Microsoft.EntityFrameworkCore;
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
    [Index(nameof(ContentID), nameof(ContentExtraID), nameof(EventCode), nameof(ClientCode))]
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
        [MaxLength(500000)]
        public string Content { get; set; }
        /// <summary>
        /// request Date
        /// </summary>
        [Required]
        public DateTime RequestDate { get; set; }
        /// <summary>
        /// ContentBinary
        /// </summary>
        [Required]
        public byte[] ContentBinary { get; set; }
        /// <summary>
        /// ContentID
        /// </summary>
        public string ContentID { get; set; }
        /// <summary>
        /// ContentExtraID
        /// </summary>
        public string ContentExtraID { get; set; }
        /// <summary>
        /// EventCode
        /// </summary>
        public string EventCode { get; set; }
        /// <summary>
        /// ClientCode
        /// </summary>
        public string ClientCode { get; set; }

    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebHookHub.Models.DB
{
    public class ClientEvent
    {
        [Key]
        public int ID { get; set; }
        [Required]
        public int ClientId { get; set; }

        [Required]
        public int EventId { get; set; }
        public virtual Client Client { get; set; }
        public virtual Event Event { get; set; }
        [Required]
        [MaxLength(5000), MinLength(2)]
        public string PostUrl { get; set; }
        [MaxLength(500)]
        public string UserName { get; set; }
        [MaxLength(500)]
        public string PassWord { get; set; }

        public bool Enable { get; set; }

    }
}

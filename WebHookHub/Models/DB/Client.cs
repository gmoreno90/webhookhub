using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebHookHub.Models.DB
{
    public class Client
    {
        [Key]
        public int ID { get; set; }
        [Required]
        [MaxLength(50),MinLength(2)]
        public string Code { get; set; }
        [Required]
        [MaxLength(100), MinLength(2)]
        public string Description { get; set; }
    }
}

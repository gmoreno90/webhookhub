using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebHookHub.Models
{
    public class DataToPostRQ
    {
        public string ID { get; set; }
        public string ContentValue { get; set; }
        public string ContentID { get; set; }
        public string ContentExtraID { get; set; }
        public string EventCode { get; set; }
        public string ClientCode { get; set; }
    }
}

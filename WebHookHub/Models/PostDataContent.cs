using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebHookHub.Models
{
    public class PostDataContent
    {
        public string EventCode { get; set; }
        public string ClientCode { get; set; }
        public string PostData { get; set; }
        public string ContentType { get; set; }
        public override string ToString()
        {
            string strReturn = "";
            strReturn += "Event Code: " + EventCode + "\n";
            strReturn += "Client Code: " + ClientCode + "\n";
            strReturn += "Post Data: " + PostData + "\n";
            strReturn += "Content Type: " + ContentType + "\n";
            return strReturn;
        }
    }
}

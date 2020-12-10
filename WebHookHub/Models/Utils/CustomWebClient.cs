using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace WebHookHub.Models.Utils
{
    /// <summary>
    /// CustomWebClient
    /// </summary>
    public class CustomWebClient : WebClient
    {
        /// <summary>
        /// _overrideTimeOut
        /// </summary>
        private int OverrideTimeOut { get; set; }
        /// <summary>
        /// CustomWebClient
        /// </summary>
        /// <param name="TimeOutInSeconds"></param>
        public CustomWebClient(int TimeOutInSeconds = 0) : base()
        {
            OverrideTimeOut = TimeOutInSeconds;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        protected override WebRequest GetWebRequest(Uri uri)
        {
            WebRequest w = base.GetWebRequest(uri);
            if (OverrideTimeOut != 0)
                w.Timeout = OverrideTimeOut;
            return w;
        }
    }
}

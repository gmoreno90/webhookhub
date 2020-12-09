using System;
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
        private int _overrideTimeOut { get; set; }
        /// <summary>
        /// CustomWebClient
        /// </summary>
        /// <param name="TimeOutInSeconds"></param>
        public CustomWebClient(int TimeOutInSeconds = 0) : base()
        {
            _overrideTimeOut = TimeOutInSeconds;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        protected override WebRequest GetWebRequest(Uri uri)
        {
            WebRequest w = base.GetWebRequest(uri);
            if (_overrideTimeOut != 0)
                w.Timeout = _overrideTimeOut;
            return w;
        }
    }
}

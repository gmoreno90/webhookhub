using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace WebHookHub.Models.Utils
{
    public class CustomWebClient : WebClient
    {
        private int _overrideTimeOut { get; set; }
        public CustomWebClient(int TimeOutInSeconds = 0) : base()
        {
            _overrideTimeOut = TimeOutInSeconds;
        }
        protected override WebRequest GetWebRequest(Uri uri)
        {
            WebRequest w = base.GetWebRequest(uri);
            if (_overrideTimeOut != 0)
                w.Timeout = _overrideTimeOut;
            return w;
        }
    }
}

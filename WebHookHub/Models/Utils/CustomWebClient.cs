using System.Net;

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
        /// <param name="address"></param>
        /// <returns></returns>
        protected override WebRequest GetWebRequest(System.Uri address)
        {
            WebRequest w = base.GetWebRequest(address);
            if (OverrideTimeOut != 0)
                w.Timeout = OverrideTimeOut;
            return w;
        }
    }
}

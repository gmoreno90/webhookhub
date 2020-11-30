using Hangfire;
using Hangfire.States;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace WebHookHub.Services
{
    /// <summary>
    /// Inotification Service
    /// </summary>
    public interface INotificationService
    {
        /// <summary>
        /// Post Data
        /// </summary>
        /// <param name="postDataContent"></param>
        /// <returns></returns>
        Task<bool> PostData(Models.PostDataContent postDataContent);
    }
    /// <summary>
    /// NotificationService
    /// </summary>
    public class NotificationService : INotificationService
    {
        private readonly ILogger<NotificationService> _logger;
        private readonly Models.DB.WebHookHubContext _context;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="context"></param>
        public NotificationService(ILogger<NotificationService> logger, Models.DB.WebHookHubContext context)
        {
            _logger = logger;
            _context = context;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="postDataContent"></param>
        /// <returns></returns>
        public async Task<bool> PostData(Models.PostDataContent postDataContent)
        {
            try
            {
                List<string> listJobIds = new List<string>();
                var webhooks = _context.ClientEvents.Where(x => x.Event.Code == postDataContent.EventCode && x.Client.Code == postDataContent.ClientCode && x.Enable).ToList();
                if (webhooks != null && webhooks.Count > 0)
                {
                    foreach (var item in webhooks)
                    {
                        EnqueuedState queue = new EnqueuedState(postDataContent.ClientCode.ToLower()); 
                        //var jobId = BackgroundJob.Enqueue(() => SendData(item.PostUrl, item.UserName, item.PassWord, postDataContent.PostData, postDataContent.ContentType));
                        var jobId = new BackgroundJobClient().Create<NotificationService>(x => x.SendData(postDataContent.ClientCode, postDataContent.EventCode, item.PostUrl, item.UserName, item.PassWord, postDataContent.PostData, postDataContent.ContentType), queue);
                        listJobIds.Add(jobId);
                        _logger.LogInformation(postDataContent.ToString());
                    }
                }
                else
                {
                    _logger.LogInformation("Not Webhooks Configured");
                }
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + " : " + ex.StackTrace);
                return false;
            }
            
        }
        /// <summary>
        /// Send Data to WebHooks
        /// </summary>
        /// <param name="ClientCode"></param>
        /// <param name="EventCode"></param>
        /// <param name="urlToPost"></param>
        /// <param name="userName"></param>
        /// <param name="passWord"></param>
        /// <param name="dataToPost"></param>
        /// <param name="contentType"></param>
        [DisplayName("SendData: [{1}:{0}]")]
        public void SendData(string ClientCode, string EventCode, string urlToPost, string userName, string passWord, string dataToPost, string contentType)
        {
            using (WebClient wc = new WebClient())
            {
                if(string.IsNullOrEmpty(userName) && string.IsNullOrEmpty(passWord))
                {
                    NetworkCredential myCreds = new NetworkCredential(userName, passWord);
                    wc.Credentials = myCreds;
                }
                wc.Headers[HttpRequestHeader.ContentType] = contentType;
                string HtmlResult = wc.UploadString(new Uri(urlToPost), dataToPost);
            }
        }
    }
}

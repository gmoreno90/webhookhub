using Hangfire;
using Hangfire.States;
using Hangfire.Tags.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using WebHookHub.Models.Utils;

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
        Task<string> PostData(Models.PostDataContent postDataContent);
        /// <summary>
        /// Delete Job
        /// </summary>
        /// <param name="JobId"></param>
        /// <returns></returns>
        Task<bool> DeleteJob(string JobId);
    }
    /// <summary>
    /// NotificationService
    /// </summary>
    public class NotificationService : INotificationService
    {
        private readonly ILogger<NotificationService> _logger;
        private readonly Models.DB.WebHookHubContext _context;
        private IConfiguration _config { get; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="context"></param>
        /// <param name="config"></param>
        public NotificationService(ILogger<NotificationService> logger, Models.DB.WebHookHubContext context, IConfiguration config)
        {
            _logger = logger;
            _context = context;
            _config = config;
        }
        /// <summary>
        /// Post Data
        /// </summary>
        /// <param name="postDataContent"></param>
        /// <returns></returns>
        public async Task<string> PostData(Models.PostDataContent postDataContent)
        {
            try
            {
                //await Task.FromResult(true);
                int TimeOutInMsSecs = _config.GetValue<int>("DefaultTimeOutInMiliSeconds");
                var idData = Guid.NewGuid().ToString();
                _context.DataToPosts.Add(new Models.DB.DataToPost() { ID = idData, Content = postDataContent.PostData });
                await _context.SaveChangesAsync();
                List<string> listJobIds = new List<string>();
                var eventClient = _context.ClientEvents
                    .Include(x => x.ClientEventWebhooks)
                    .Include(x => x.Event)
                    .FirstOrDefault(x => x.Event.Code == postDataContent.EventCode && x.Client.Code == postDataContent.ClientCode && x.Enable);
                if (eventClient != null)
                {
                    //Get IDS
                    var ID = "";
                    var IDExtra = "";
                    if (!string.IsNullOrEmpty(eventClient.Event.RegexID))
                        ID = SearchContentUtil.SearchContent(postDataContent.PostData, eventClient.Event.RegexID);
                    if (!string.IsNullOrEmpty(eventClient.Event.RegexIDExtra))
                        IDExtra = SearchContentUtil.SearchContent(postDataContent.PostData, eventClient.Event.RegexIDExtra);

                    try
                    {
                        if (eventClient.ClientEventWebhooks != null && eventClient.ClientEventWebhooks.Any())
                        {
                            foreach (var item in eventClient.ClientEventWebhooks)
                            {
                                
                                if (postDataContent.DelayMode == Models.DelayModeEnum.Instant)
                                {
                                   
                                    listJobIds.Add(EnqueueRequest(postDataContent.ClientCode, postDataContent.EventCode, item.PostUrl, item.UserName, item.PassWord, postDataContent.PostData, postDataContent.ContentType, TimeOutInMsSecs, ID, IDExtra, item.ExpectedContentResult));
                                }
                                else {
                                    var jobId = new BackgroundJobClient().Schedule<NotificationService>(x => x.EnqueueRequest(postDataContent.ClientCode, postDataContent.EventCode, item.PostUrl, item.UserName, item.PassWord, postDataContent.PostData, postDataContent.ContentType, TimeOutInMsSecs, ID, IDExtra, item.ExpectedContentResult), TimeSpan.FromSeconds(postDataContent.DelayValue.Value));
                                    listJobIds.Add(jobId);
                                }
                                
                                _logger.LogInformation(postDataContent.ToString());
                            }
                            return eventClient.ContentReponseOk;
                        }
                        else
                        {
                            _logger.LogInformation("Not Webhooks Configured");
                            return eventClient.ContentReponseError;
                        }
                    }
                    catch (Exception exevent)
                    {
                        _logger.LogError(exevent.Message);
                        throw new Exception(eventClient.ContentReponseError);
                    }


                }
                else
                {
                    _logger.LogError("Not Webhooks Configured");
                    return "[error][configuration not found]";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + " : " + ex.StackTrace);
                return "[error][" + ex.Message + " : " + ex.StackTrace + "]";
            }

        }


        /// <summary>
        /// DeleteJob
        /// </summary>
        /// <param name="JobId"></param>
        /// <returns></returns>
        public async Task<bool> DeleteJob(string JobId)
        {
            try
            {
                await Task.FromResult(true);
                var jobId = new BackgroundJobClient().Delete(JobId);
                return jobId;
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
        /// <param name="TimeOutInMsSecs"></param>
        /// <param name="ID"></param>
        /// <param name="IDExtra"></param>
        /// <param name="expectedResult"></param>
        [DisplayName("SendData: [{1}:{0}][{8} | {9}]")]
        [Tag("SendData", "{0}", "{1}")]
        public void SendData(string ClientCode, string EventCode, string urlToPost, string userName, string passWord, string dataToPost, string contentType, int TimeOutInMsSecs, string ID, string IDExtra, string expectedResult = "")
        {

            using (CustomWebClient wc = new CustomWebClient(TimeOutInMsSecs))
            {
                if (string.IsNullOrEmpty(userName) && string.IsNullOrEmpty(passWord))
                {
                    NetworkCredential myCreds = new NetworkCredential(userName, passWord);
                    wc.Credentials = myCreds;
                }
                wc.Headers[HttpRequestHeader.ContentType] = contentType;
                wc.Headers.Add("WebhookHub_ClientCode", ClientCode);
                wc.Headers.Add("WebhookHub_EventCode", EventCode);
                wc.Headers.Add("WebhookHub_ID", ID);
                wc.Headers.Add("WebhookHub_IDExtra", IDExtra);
                string HtmlResult = wc.UploadString(new Uri(urlToPost), dataToPost);

                if (!string.IsNullOrEmpty(expectedResult))
                {
                    if (HtmlResult != expectedResult)
                    {
                        throw new Exception("[Unexpected content result]");
                    }
                }

            }
        }

        /// <summary>
        /// Enqueue Request
        /// </summary>
        /// <param name="ClientCode"></param>
        /// <param name="EventCode"></param>
        /// <param name="urlToPost"></param>
        /// <param name="userName"></param>
        /// <param name="passWord"></param>
        /// <param name="dataToPost"></param>
        /// <param name="contentType"></param>
        /// <param name="TimeOutInMsSecs"></param>
        /// <param name="ID"></param>
        /// <param name="IDExtra"></param>
        /// <param name="expectedResult"></param>
        [DisplayName("EnqueueRequest: [{1}:{0}][{8} | {9}]")]
        [Tag("EnqueueRequest", "{0}", "{1}")]
        public string EnqueueRequest(string ClientCode, string EventCode, string urlToPost, string userName, string passWord, string dataToPost, string contentType, int TimeOutInMsSecs, string ID, string IDExtra, string expectedResult = "")
        {
            EnqueuedState queue = new EnqueuedState(ClientCode.ToLower());
            var jobId = new BackgroundJobClient().Create<NotificationService>(x => x.SendData(ClientCode, EventCode, urlToPost, userName, passWord, dataToPost, contentType, TimeOutInMsSecs, ID, IDExtra, expectedResult), queue);
            return jobId;
        }
    }
}

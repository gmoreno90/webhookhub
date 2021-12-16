using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StatsdClient;
using System;
using System.Threading.Tasks;

namespace WebHookHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UtilsController : ControllerBase
    {
        private readonly ILogger<UtilsController> _logger;
        private IConfiguration _config { get; }
        private IHostApplicationLifetime _appLifetime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="config"></param>
        /// <param name="appLifetime"></param>
        public UtilsController(ILogger<UtilsController> logger, IConfiguration config, IHostApplicationLifetime appLifetime)
        {
            _appLifetime = appLifetime;
            _logger = logger;
            _config = config;
        }

        /// <summary>
        /// Version Information
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("Version")]
        public async Task<ActionResult> Version()
        {
            await Task.FromResult(true);
            var strVersion = _config.GetValue<string>("VersionNumber");
            string svgContent = Models.Utils.GeneralUtils.getVersionImage(strVersion);
            _logger.LogInformation(strVersion);
            return Content(svgContent, "image/svg+xml; charset=utf-8");
        }
        /// <summary>
        /// Version Information
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("VersionCode")]
        public async Task<ActionResult> VersionCode()
        {
            await Task.FromResult(true);
            var strVersion = "1.26.2";
            string svgContent = Models.Utils.GeneralUtils.getVersionImage(strVersion);
            _logger.LogInformation(strVersion);
            return Content(svgContent, "image/svg+xml; charset=utf-8");
        }
        /// <summary>
        /// Restart Environment
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("RestartEnvironment")]
        public async Task<ActionResult> RestartEnvironment()
        {
            _appLifetime.StopApplication();
            await Task.FromResult(true);
            return Content("Restarting server");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="EventName"></param>
        /// <param name="Value"></param>
        /// <param name="environment"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("TestEventsDataDog")]
        public async Task<ActionResult> TestEventsDataDog(string EventName, string Value, string environment = "dev")
        {
            try
            {
                DogStatsd.Event(EventName, Value, alertType:"info", tags: new[] { "environment:" + environment });
                //var dogstatsdConfig = new StatsdConfig
                //{
                //    StatsdServerName = "127.0.0.1",
                //    StatsdPort = 8125,
                //};

                //using (var service = new DogStatsdService())
                //{
                    
                //    //service.Configure(dogstatsdConfig);
                //    //service.Increment("example_metric.increment", tags: new[] { "environment:dev" });
                //    //service.Decrement("example_metric.decrement", tags: new[] { "environment:dev" });
                //    //service.Counter("example_metric.count", 2, tags: new[] { "environment:dev" });

                //    //var random = new Random(0);

                //    //for (int i = 0; i < 10; i++)
                //    //{
                //    //    service.Gauge("example_metric.gauge", i, tags: new[] { "environment:dev" });
                //    //    service.Set("example_metric.set", i, tags: new[] { "environment:dev" });
                //    //    service.Histogram("example_metric.histogram", random.Next(20), tags: new[] { "environment:dev" });
                //    //    System.Threading.Thread.Sleep(random.Next(10000));
                //    //}
                //}
                return Content("OK");
            }
            catch (System.Exception ex)
            {

                throw;
            }
            
        }
    }
}

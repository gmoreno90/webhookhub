using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
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


    }
}

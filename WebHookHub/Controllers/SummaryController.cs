using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebHookHub.Models.DB;

namespace WebHookHub.Controllers
{
    public class SummaryController : Controller
    {
        private readonly WebHookHubContext _context;
        private readonly ILogger<SummaryController> _logger;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="logger"></param>
        public SummaryController(WebHookHubContext context, ILogger<SummaryController> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task<IActionResult> Index()
        {

            return PartialView();
        }

        public async Task<Models.NodeStructure> GetSummary()
        {
            var infoSummary = new Models.NodeStructure()
            {
                text = new Models.Text()
                {
                    name = "Web Hook Hub Structure",
                    title = "",
                    desc = "",
                    innerHTML = ""
                },
                children = new List<Models.Child>(),
                HTMLclass = "the-parent"
            };
            var infoEventos = await _context.Events.ToListAsync();
            foreach (var item in infoEventos)
            {
                var childrenEvento = new Models.Child()
                {
                    text = new Models.Text()
                    {
                        name = item.Description,
                        title = item.Code,
                        desc = "",
                        innerHTML = ""// "RegexID: <small>"+ item.RegexID + "</small><br/>RegexIDExtra: " + item.RegexIDExtra
                    },
                    children = new List<Models.Child>(),
                    childrenDropLevel = 1,
                    HTMLclass = "the-parent",
                    collapsable = true,
                    collapsed = false,
                    pseudo = false
                };
                var infoEventosClientes = await _context.ClientEvents.Include(s => s.Client).Include(s => s.ClientEventWebhooks).Where(x => x.EventId == item.ID).ToListAsync();
                foreach (var infoEC in infoEventosClientes)
                {
                    var childreninfoEC = new Models.Child()
                    {
                        text = new Models.Text()
                        {
                            name = infoEC.Client.Description,
                            title = infoEC.Client.Code,
                            desc = "",
                            innerHTML = ""
                        },
                        children = new List<Models.Child>(),
                        childrenDropLevel = 2,
                        HTMLclass = "the-parent",
                        collapsable = true,
                        collapsed = false,
                        pseudo = false,
                        stackChildren = true
                    };
                    foreach (var itemWH in infoEC.ClientEventWebhooks)
                    {
                        var childrenWH = new Models.Child()
                        {
                            text = new Models.Text()
                            {
                                name = itemWH.PostUrl + " (" + (itemWH.Enable ? "Enable" : "No Enable") + ")",
                                title = "",
                                desc = "",
                                innerHTML = ""
                            },
                            pseudo = false,
                            collapsable = false,
                            collapsed = false
                        };
                        childreninfoEC.children.Add(childrenWH);
                    }

                    childrenEvento.children.Add(childreninfoEC);
                }
                infoSummary.children.Add(childrenEvento);
            }
            return infoSummary;
        }
    }
}

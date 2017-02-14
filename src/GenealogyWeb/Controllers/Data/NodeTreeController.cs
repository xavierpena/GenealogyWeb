using GenealogyWeb.Core.Business.DownwardTree;
using GenealogyWeb.Core.Business.UpwardTree;
using GenealogyWeb.Core.Models;
using GenealogyWeb.Core.Repositories;
using GenealogyWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GenealogyWeb.Controllers
{
    [Authorize]
    public class NodeTreeController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger _logger;

        private DownwardTreeBuilder _downwardTreeBuilder;
        private UpwardTreeBuilder _upwardTreeBuilder;
        
        public NodeTreeController(
            UserManager<ApplicationUser> userManager,
            ILoggerFactory loggerFactory,
            DownwardTreeBuilder downwardTreeBuilder,
            UpwardTreeBuilder upwardTreeBuilder)
        {
            _userManager = userManager;
            _logger = loggerFactory.CreateLogger<ManageController>();

            _downwardTreeBuilder = downwardTreeBuilder;
            _upwardTreeBuilder = upwardTreeBuilder;
        }

        private ActionResult NodeTree(Core.Business.JsonItem result)
        {
            string json = SerializeObject(result);
            var encodedJson = new HtmlString(json);
            ViewData["Title"] = "Tree";
            ViewData["json"] = encodedJson;
            return View(nameof(NodeTreeController.NodeTree));
        }

        private static string SerializeObject(Object result)
             => JsonConvert.SerializeObject(
                    result,
                    Formatting.None,
                    new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }
                );

        [HttpGet]
        public IActionResult DownwardTree()
            => NodeTree(_downwardTreeBuilder.GetResult());

        [HttpGet]
        public IActionResult PersonDownwardTree(int id)
            => NodeTree(_downwardTreeBuilder.GetResult(id));

        [HttpGet]
        public IActionResult PersonUpwardTree(int id)
            => NodeTree(_upwardTreeBuilder.GetResult(id));
        
        [HttpGet]
        public JsonResult FullDownwardTree()
        {
            var result = _downwardTreeBuilder.GetResult();
            return new JsonResult(result);
        }

    }

}

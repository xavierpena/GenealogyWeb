using GenealogyWeb.Core.Repositories;
using GenealogyWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GenealogyWeb.Controllers
{
    [Authorize]
    public class DataController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger _logger;
        PersonaRepository _personaRepository;

        public DataController(
        UserManager<ApplicationUser> userManager,
        ILoggerFactory loggerFactory,
        PersonaRepository personaRepository)
        {
            _userManager = userManager;
            _logger = loggerFactory.CreateLogger<ManageController>();
            _personaRepository = personaRepository;
        }

        public IActionResult Index()
        {
            var user = _userManager.GetUserAsync(HttpContext.User).Result;
            if (user != null)
            {
                // AUTHORIZE
                if (user.Email == "xavierpenya@gmail.com")
                {
                    var header = $"[\"search_key\",\"id\",\"nom\",\"llinatge_1\",\"llinatge_2\",\"home\",\"naixement_lloc\",\"naixement_data\",\"mort_lloc\",\"mort_data\",\"info\",\"observacions\"]";
                    var rows = default(IEnumerable<string>);
                    var persones = _personaRepository.GetAll();
                    rows = persones.Select(x => $"[\"{x.GetSearchKey()}\",\"{x.id}\",\"{x.nom}\",\"{x.llinatge_1}\",\"{x.llinatge_2}\",\"{x.home}\",\"{x.naixement_lloc}\",\"{x.naixement_data}\",\"{x.mort_lloc}\",\"{x.mort_data}\",\"{x.info}\",\"{x.observacions}\"]");

                    ViewData["data"] = new HtmlString("[" + string.Join(",", rows) + "]");
                    ViewData["colHeaders"] = new HtmlString(header);
                    ViewData["Title"] = "Persones";
                    
                    return View();
                }
            }
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

    }
}

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
    public class CommentModel
    {
        public int Id { get; set; }
        public string Author { get; set; }
        public string Text { get; set; }
    }

    [Authorize]
    public class DataController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger _logger;

        private PersonaRepository _personaRepository;
        private MatrimoniRepository _matrimoniRepository;
        private FillRepository _fillRepository;

        public DataController(
        UserManager<ApplicationUser> userManager,
        ILoggerFactory loggerFactory,
        PersonaRepository personaRepository,
        MatrimoniRepository matrimoniRepository,
        FillRepository fillRepository)
        {
            _userManager = userManager;
            _logger = loggerFactory.CreateLogger<ManageController>();

            _personaRepository = personaRepository;
            _matrimoniRepository = matrimoniRepository;
            _fillRepository = fillRepository;
        }

        public IActionResult Index()
        {
            var user = _userManager.GetUserAsync(HttpContext.User).Result;
            if (user != null)
            {
                // AUTHORIZE
                if (user.Email == "xavierpenya@gmail.com")
                {
                    ViewData["Title"] = "Genealogia";

                    var persones = _personaRepository.GetAll();
                    var personesRows = persones.Select(x => $"[\"{x.GetSearchKey()}\",\"{x.id}\",\"{x.nom}\",\"{x.llinatge_1}\",\"{x.llinatge_2}\",\"{x.home}\",\"{x.naixement_lloc}\",\"{x.naixement_data}\",\"{x.mort_lloc}\",\"{x.mort_data}\",\"{x.info}\",\"{x.observacions}\"]");
                    ViewData["title1"] = "Persones";
                    ViewData["data1"] = new HtmlString("[" + string.Join(",", personesRows) + "]");
                    ViewData["colHeaders1"] = new HtmlString($"[\"search_key\",\"id\",\"nom\",\"llinatge_1\",\"llinatge_2\",\"home\",\"naixement_lloc\",\"naixement_data\",\"mort_lloc\",\"mort_data\",\"info\",\"observacions\"]");

                    var matrimonis = _matrimoniRepository.GetAll();
                    var matrimonisRows = matrimonis.Select(x => $"[\"{x.GetSearchKey()}\",\"{x.id}\",\"{x.home_id}\",\"{x.dona_id}\",\"{x.lloc}\",\"{x.data}\",\"{x.observacions}\"]");
                    ViewData["title2"] = "Matrimonis";
                    ViewData["data2"] = new HtmlString("[" + string.Join(",", matrimonisRows) + "]");
                    ViewData["colHeaders2"] = new HtmlString($"[\"search_key\",\"id\",\"home_id\",\"dona_id\",\"lloc\",\"data\",\"observacions\"]");

                    var fills = _fillRepository.GetAll();
                    var fillsRows = fills.Select(x => $"[\"{x.GetSearchKey()}\",\"{x.id}\",\"{x.matrimoni_id}\",\"{x.persona_id}\",\"{x.observacions}\"]");
                    ViewData["title3"] = "Fills";
                    ViewData["data3"] = new HtmlString("[" + string.Join(",", fillsRows) + "]");
                    ViewData["colHeaders3"] = new HtmlString($"[\"search_key\",\"id\",\"matrimoni_id\",\"persona_id\",\"observacions\"]");

                    return View();
                }
            }
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

    }
}

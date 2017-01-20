using GenealogyWeb.Core.Models;
using GenealogyWeb.Core.Repositories;
using GenealogyWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
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

    //[Authorize]
    public class DataController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger _logger;

        private PersonaRepository _personRepository;
        private MatrimoniRepository _marriageRepository;
        private FillRepository _sonRepository;

        public DataController(
        UserManager<ApplicationUser> userManager,
        ILoggerFactory loggerFactory,
        PersonaRepository personRepository,
        MatrimoniRepository marriageRepository,
        FillRepository sonRepository)
        {
            _userManager = userManager;
            _logger = loggerFactory.CreateLogger<ManageController>();

            _personRepository = personRepository;
            _marriageRepository = marriageRepository;
            _sonRepository = sonRepository;
        }

        public IActionResult Index()
        {
            // Tutorial: handsontable load & save
            // http://docs.handsontable.com/0.17.0/tutorial-load-and-save.html

            var user = _userManager.GetUserAsync(HttpContext.User).Result;
            if (user != null)
            {
                // AUTHORIZE
                if (user.Email == "testuser@gmail.com")
                {
                    // TODO
                }

                ViewData["Title"] = "Genealogia";
                                        
                var persons = _personRepository.GetAll();
                var marriages = _marriageRepository.GetAll();
                var sons = _sonRepository.GetAll();

                var homesPerMatrimoni = marriages.ToDictionary(m => m, m => persons.Where(p => m.home_id == p.id).SingleOrDefault());
                var donesPerMatrimoni = marriages.ToDictionary(m => m, m => persons.Where(p => m.dona_id == p.id).SingleOrDefault());

                var personaPerFills = sons.ToDictionary(f => f, f => persons.Where(p => f.persona_id == p.id).Single());
                var matrimoniPerFills = sons.ToDictionary(f => f, f => marriages.Where(m => f.matrimoni_id == m.id).Single());

                var searchKeyPerMarriage = marriages.ToDictionary(m => m, m => m.GetSearchKey() + " | home=" + homesPerMatrimoni[m]?.GetSearchKey() + " + dona=" + donesPerMatrimoni[m]?.GetSearchKey());
                var searchKeyPerSon = sons.ToDictionary(f => f, f => f.GetSearchKey() + " | fill=" + personaPerFills[f].GetSearchKey() + " | matrimoni=(" + searchKeyPerMarriage[matrimoniPerFills[f]] + ")");
                    
                // Persons:
                var personesRows = persons.Select(x => $"[\"{x.GetSearchKey()}\",\"{x.id}\",\"{x.nom}\",\"{x.llinatge_1}\",\"{x.llinatge_2}\",\"{x.home}\",\"{x.naixement_lloc}\",\"{x.naixement_data}\",\"{x.mort_lloc}\",\"{x.mort_data}\",\"{x.info}\",\"{x.observacions}\"]");
                ViewData["title_1"] = "Persones";
                ViewData["data_1"] = new HtmlString("[" + string.Join(",", personesRows) + "]");
                ViewData["colHeaders_1"] = new HtmlString($"[\"search_key\",\"id\",\"nom\",\"llinatge_1\",\"llinatge_2\",\"home\",\"naixement_lloc\",\"naixement_data\",\"mort_lloc\",\"mort_data\",\"info\",\"observacions\"]");

                // Marriages:                 
                var matrimonisRows = marriages.Select(x => $"[\"{ searchKeyPerMarriage[x] }\",\"{x.id}\",\"{x.home_id}\",\"{x.dona_id}\",\"{x.lloc}\",\"{x.data}\",\"{x.observacions}\"]");
                ViewData["title_2"] = "Matrimonis";
                ViewData["data_2"] = new HtmlString("[" + string.Join(",", matrimonisRows) + "]");
                ViewData["colHeaders_2"] = new HtmlString($"[\"search_key\",\"id\",\"home_id\",\"dona_id\",\"lloc\",\"data\",\"observacions\"]");

                // Sons:                    
                var fillsRows = sons.Select(x => $"[\"{searchKeyPerSon[x]}\",\"{x.id}\",\"{x.matrimoni_id}\",\"{x.persona_id}\",\"{x.observacions}\"]");
                ViewData["title_3"] = "Fills";
                ViewData["data_3"] = new HtmlString("[" + string.Join(",", fillsRows) + "]");
                ViewData["colHeaders_3"] = new HtmlString($"[\"search_key\",\"id\",\"matrimoni_id\",\"persona_id\",\"observacions\"]");

                return View();                
            }
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        public JsonResult Save([FromBody] DataToSaveModel dataToSaveModel)
        {
            var persons = dataToSaveModel.persons?
                .Select(x => Persona.PersonaFromArray(x.Skip(1).ToArray()))
                .ToList();

            var marriages = dataToSaveModel.marriages?
                .Select(x => Matrimoni.MatrimoniFromArray(x.Skip(1).ToArray()))
                .ToList();

            var sons = dataToSaveModel.sons?
                .Select(x => Fill.FillFromArray(x.Skip(1).ToArray()))
                .ToList();

            return new JsonResult("ok");
        }

    }

    public class DataToSaveModel
    {
        public List<List<string>> persons { get; set; }
        public List<List<string>> marriages { get; set; }
        public List<List<string>> sons { get; set; }
    }
}

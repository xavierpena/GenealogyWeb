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

        private DownwardTreeBuilder _downwardTreeBuilder;
        private UpwardTreeBuilder _upwardTreeBuilder;

        public DataController(
        UserManager<ApplicationUser> userManager,
        ILoggerFactory loggerFactory,
        PersonaRepository personRepository,
        MatrimoniRepository marriageRepository,
        FillRepository sonRepository,
        DownwardTreeBuilder downwardTreeBuilder,
        UpwardTreeBuilder upwardTreeBuilder)
        {
            _userManager = userManager;
            _logger = loggerFactory.CreateLogger<ManageController>();

            _personRepository = personRepository;
            _marriageRepository = marriageRepository;
            _sonRepository = sonRepository;

            _downwardTreeBuilder = downwardTreeBuilder;
            _upwardTreeBuilder = upwardTreeBuilder;
        }

        public IActionResult Index()
        {
            var persons = _personRepository.GetAll();

            ViewBag.persons = persons
                .Select(x => new SelectListItem { Text = x.FullName, Value = x.id.ToString() })
                .OrderBy(x => x.Text)
                .ToList();

            var result = _downwardTreeBuilder.GetResult();
            var json = JsonConvert.SerializeObject(
                            result,
                            Formatting.None,
                            new JsonSerializerSettings
                            {
                                NullValueHandling = NullValueHandling.Ignore
                            });
            var encodedJson = new HtmlString(json);
            ViewBag.json = encodedJson;

            return View();
        }

        public IActionResult DownwardTree()
        {
            var result = _downwardTreeBuilder.GetResult();
            var json = JsonConvert.SerializeObject(
                            result,
                            Formatting.None,
                            new JsonSerializerSettings
                            {
                                NullValueHandling = NullValueHandling.Ignore
                            });
            var encodedJson = new HtmlString(json);
            ViewData["Title"] = "Tree";
            ViewData["json"] = encodedJson;
            return View("NodeTree");
        }

        public IActionResult PersonDownwardTree(int id)
        {
            var result = _downwardTreeBuilder.GetResult(id);
            var json = JsonConvert.SerializeObject(
                            result,
                            Formatting.None,
                            new JsonSerializerSettings
                            {
                                NullValueHandling = NullValueHandling.Ignore
                            });
            var encodedJson = new HtmlString(json);
            ViewData["Title"] = "Tree";
            ViewData["json"] = encodedJson;
            return View("NodeTree");
        }

        public IActionResult PersonUpwardTree(int id)
        {
            var result = _upwardTreeBuilder.GetResult(id);
            var json = JsonConvert.SerializeObject(
                            result,
                            Formatting.None,
                            new JsonSerializerSettings
                            {
                                NullValueHandling = NullValueHandling.Ignore
                            });
            var encodedJson = new HtmlString(json);
            ViewData["Title"] = "Tree";
            ViewData["json"] = encodedJson;
            return View("NodeTree");
        }

        public ActionResult PersonById(int id)
        {
            var person = _personRepository.GetById(id);
            return Person(person);
        }

        public ActionResult Person(Persona person)
        {
            //if (!ModelState.IsValid)
            //    return View("Person", person);

            //if (person == null)
            //    person = new Persona();

            return View("Person", person);            
        }

        public ActionResult MarriageByPersonId(int id)
        {
            var person = _personRepository.GetById(id);
            if(person.home)
            {
                // MAN:
                var marriagesByHusband = _marriageRepository.GetAllByHusbandId(id);
                if (marriagesByHusband.Any())
                {
                    if (marriagesByHusband.Count() == 1)
                        return Marriage(marriagesByHusband.Single());
                    else
                        return BadRequest("Error: more than one marriage");
                }
                else
                {
                    return Marriage(new Matrimoni { home_id = id });
                }
            }
            else
            {
                // WOMAN:
                var marriagesByWife = _marriageRepository.GetAllByWifeId(id);
                if (marriagesByWife.Any())
                {
                    if (marriagesByWife.Count() == 1)
                        return Marriage(marriagesByWife.Single());
                    else
                        return BadRequest("Error: more than one marriage");
                }
                else
                {
                    return Marriage(new Matrimoni { dona_id = id });
                }
            }            
        }

        public ActionResult SonById(int id)
        {
            var son = _sonRepository.GetById(id);
            return Son(son);
        }

        public ActionResult Son(Fill son)
        {
            var persons = _personRepository.GetAll();
            var marriages = _marriageRepository.GetAll();

            ViewBag.persons = Utils.GetPersonsSelectList(persons);           
            ViewBag.marriages = Utils.GetMarriagesSelectList(marriages, persons);

            return View(son);
        }
        
        public ActionResult MarriageById(int id)
        {
            var marriage = _marriageRepository.GetById(id);
            return Marriage(marriage);
        }

        public ActionResult Marriage(Matrimoni marriage)
        {
            var persons = _personRepository.GetAll();

            ViewBag.men = Utils.GetPersonsSelectListByGender(persons, isMale: true);
            ViewBag.women = Utils.GetPersonsSelectListByGender(persons, isMale: false);

            //if (!ModelState.IsValid)
            //    return View("Marriage", marriage);

            return View("Marriage", marriage);
        }

        public IActionResult Tables()
        {
            // Tutorial: handsontable load & save
            // http://docs.handsontable.com/0.17.0/tutorial-load-and-save.html

            //var user = _userManager.GetUserAsync(HttpContext.User).Result;
            //if (user != null)
            //{
            //    // AUTHORIZE
            //    if (user.Email == "testuser@gmail.com")
            //    {
            //        // TODO
            //    }
            //}
            //return RedirectToAction(nameof(HomeController.Index), "Home");

            ViewData["Title"] = "Full overview";

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
            ViewData["title_1"] = "Persons";
            ViewData["data_1"] = new HtmlString("[" + string.Join(",", personesRows) + "]");
            ViewData["colHeaders_1"] = new HtmlString($"[\"search_key\",\"id\",\"nom\",\"llinatge_1\",\"llinatge_2\",\"home\",\"naixement_lloc\",\"naixement_data\",\"mort_lloc\",\"mort_data\",\"info\",\"observacions\"]");

            // Marriages:                 
            var matrimonisRows = marriages.Select(x => $"[\"{ searchKeyPerMarriage[x] }\",\"{x.id}\",\"{x.home_id}\",\"{x.dona_id}\",\"{x.lloc}\",\"{x.data}\",\"{x.observacions}\"]");
            ViewData["title_2"] = "Marriages";
            ViewData["data_2"] = new HtmlString("[" + string.Join(",", matrimonisRows) + "]");
            ViewData["colHeaders_2"] = new HtmlString($"[\"search_key\",\"id\",\"home_id\",\"dona_id\",\"lloc\",\"data\",\"observacions\"]");

            // Sons:                    
            var fillsRows = sons.Select(x => $"[\"{searchKeyPerSon[x]}\",\"{x.id}\",\"{x.matrimoni_id}\",\"{x.persona_id}\",\"{x.observacions}\"]");
            ViewData["title_3"] = "Sons";
            ViewData["data_3"] = new HtmlString("[" + string.Join(",", fillsRows) + "]");
            ViewData["colHeaders_3"] = new HtmlString($"[\"search_key\",\"id\",\"matrimoni_id\",\"persona_id\",\"observacions\"]");

            return View();

        }

    }

}

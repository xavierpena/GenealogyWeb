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
        private const string NodeTreeViewName = "NodeTree";

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
            var marriages = _marriageRepository.GetAll();
            var sons = _sonRepository.GetAll();

            ViewBag.persons = Utils.GetPersonsSelectList(persons);
            ViewBag.marriages = Utils.GetMarriagesSelectList(marriages, persons);
            ViewBag.sons = Utils.GetSonsSelectList(sons, marriages, persons);

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
            return View(NodeTreeViewName);
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
            return View(NodeTreeViewName);
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
            return View(NodeTreeViewName);
        }        

        public ActionResult PersistPerson(Persona person)
        {
            if (!ModelState.IsValid)
                return BadRequest("State is invalid");

            if (!person.id.HasValue)
                person = _personRepository.Add(person);
            else
                _personRepository.Update(person);

            return RedirectToAction(nameof(Person), person);
        }

        public ActionResult PersistMarriage(Matrimoni marriage)
        {
            if (!ModelState.IsValid)
                return BadRequest("State is invalid");

            if (!marriage.id.HasValue)
                marriage = _marriageRepository.Add(marriage);
            else
                _marriageRepository.Update(marriage);

            return RedirectToAction(nameof(Marriage), marriage);
        }

        public ActionResult PersistSon(Fill son)
        {
            if (!ModelState.IsValid)
                return BadRequest("State is invalid");

            if (!son.id.HasValue)
                son = _sonRepository.Add(son);
            else
                _sonRepository.Update(son);

            return RedirectToAction(nameof(Son), son);
        }

        public ActionResult PersonById(int id)
        {
            var person = _personRepository.GetById(id);
            return RedirectToAction(nameof(Person), person);
        }

        public ActionResult Person(Persona person)
        {
            return View(person);            
        }

        public ActionResult MarriageBySonId(int id)
        {
            var son = _sonRepository.GetById(id);
            var marriage = _marriageRepository.GetById(son.matrimoni_id);
            return Marriage(marriage);
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
                        return RedirectToAction(nameof(Marriage), marriagesByHusband.Single());
                    else
                        return Message("Error: more than one marriage", false);
                }
                else
                {
                    return RedirectToAction(nameof(Marriage), new Matrimoni { home_id = id });
                }
            }
            else
            {
                // WOMAN:
                var marriagesByWife = _marriageRepository.GetAllByWifeId(id);
                if (marriagesByWife.Any())
                {
                    if (marriagesByWife.Count() == 1)
                        return RedirectToAction(nameof(Marriage), marriagesByWife.Single());
                    else
                        return Message("Error: more than one marriage", false);
                }
                else
                {
                    return RedirectToAction(nameof(Marriage), new Matrimoni { dona_id = id });
                }
            }            
        }

        public ActionResult SonById(int id)
        {
            var son = _sonRepository.GetById(id);
            return RedirectToAction(nameof(Son), son);
        }

        public ActionResult SonByPersonId(int id)
        {
            var son = _sonRepository.GetByPersonId(id);
            if (son == null)
                son = new Fill { persona_id = id };
            return RedirectToAction(nameof(Son), son);
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
            return RedirectToAction(nameof(Marriage), marriage);
        }

        public ActionResult Marriage(Matrimoni marriage)
        {
            var persons = _personRepository.GetAll();

            ViewBag.men = Utils.GetPersonsSelectListByGender(persons, isMale: true);
            ViewBag.women = Utils.GetPersonsSelectListByGender(persons, isMale: false);

            //if (!ModelState.IsValid)
            //    return View("Marriage", marriage);

            return View(marriage);
        }

        public ActionResult DeletePerson(int id)
        {
            try
            {
                _personRepository.RemoveById(id);
                return Message("Person deleted succesfully", true);
            }
            catch (Exception ex)
            {
                return Message("Error deleting person: " + ex.Message, false);
            }
        }

        public ActionResult DeleteMarriage(int id)
        {
            try
            {
                _marriageRepository.RemoveById(id);
                return Message("Marriage deleted succesfully", true);
            }
            catch (Exception ex)
            {
                return Message("Error deleting marriage: " + ex.Message, false);
            }
        }

        public ActionResult DeleteSon(int id)
        {
            try
            {
                _sonRepository.RemoveById(id);
                return Message("Son deleted succesfully", true);
            }
            catch (Exception ex)
            {
                return Message("Error deleting son: " + ex.Message, false);
            }
        }

        private ActionResult Message(string message, bool success)
        {
            ViewBag.Success = success;
            ViewBag.Message = message;
            return View(nameof(Message));
        }

    }

}

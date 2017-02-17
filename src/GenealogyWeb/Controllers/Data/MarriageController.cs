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
    [Authorize(Roles = "Admin")]
    public class MarriageController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger _logger;

        private PersonRepository _personRepository;
        private MarriageRepository _marriageRepository;
        private SonRepository _sonRepository;
        
        public MarriageController(
            UserManager<ApplicationUser> userManager,
            ILoggerFactory loggerFactory,
            PersonRepository personRepository,
            MarriageRepository marriageRepository,
            SonRepository sonRepository)
        {
            _userManager = userManager;
            _logger = loggerFactory.CreateLogger<ManageController>();

            _personRepository = personRepository;
            _marriageRepository = marriageRepository;
            _sonRepository = sonRepository;
        }

        private ActionResult Marriage(Marriage marriage)
        {
            var people = _personRepository.GetAll();

            ViewBag.men = Utils.GetPersonsSelectListByGender(people, isMale: true);
            ViewBag.women = Utils.GetPersonsSelectListByGender(people, isMale: false);

            return View(nameof(MarriageController.Marriage), marriage);
        }

        [HttpGet]
        public ActionResult AddMarriage()
            => Marriage(new Marriage());

        private ActionResult RedirectToMessage(string message, bool success)
            => RedirectToAction(
                    nameof(MessageController.Message),
                    "Message",
                    new { message = message, success = success }
                );

        [HttpPost]
        public ActionResult PersistMarriage(Marriage marriage)
        {
            if (!ModelState.IsValid)
                return BadRequest("State is invalid");

            if (!marriage.id.HasValue)
                marriage = _marriageRepository.Add(marriage);
            else
                _marriageRepository.Update(marriage);

            return Marriage(marriage);
        }

        [HttpGet]
        public ActionResult MarriageBySonId(int id)
        {
            var son = _sonRepository.GetById(id);
            var marriage = _marriageRepository.GetById(son.marriage_id);
            return Marriage(marriage);
        }     

        [HttpGet]
        public ActionResult MarriageByPersonId(int id)
        {
            var person = _personRepository.GetById(id);
            if(person.is_male)
            {
                // MAN:
                var marriagesByHusband = _marriageRepository.GetAllByHusbandId(id);
                if (marriagesByHusband.Any())
                {
                    if (marriagesByHusband.Count() == 1)
                        return Marriage(marriagesByHusband.Single());
                    else
                        return RedirectToMessage("Error: more than one marriage", false);
                }
                else
                {
                    var marriage = new Marriage { husband_id = id };
                    return Marriage(marriage);
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
                        return RedirectToMessage("Error: more than one marriage", false);
                }
                else
                {
                    var marriage = new Marriage { wife_id = id };
                    return Marriage(marriage);
                }
            }            
        }
        
        [HttpGet]
        public ActionResult MarriageById(int id)
        {
            var marriage = _marriageRepository.GetById(id);
            return Marriage(marriage);
        }

        [HttpGet]
        public ActionResult DeleteMarriage(int id)
        {
            try
            {
                _marriageRepository.RemoveById(id);
                return RedirectToMessage("Marriage deleted succesfully", true);
            }
            catch (Exception ex)
            {
                return RedirectToMessage("Error deleting marriage: " + ex.Message, false);
            }
        }

    }

}

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
    public class SonController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger _logger;

        private PersonRepository _personRepository;
        private MarriageRepository _marriageRepository;
        private SonRepository _sonRepository;
        
        public SonController(
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

        private ActionResult Son(Son son)
        {
            var people = _personRepository.GetAll();
            var marriages = _marriageRepository.GetAll();

            ViewBag.people = Utils.GetPersonsSelectList(people);
            ViewBag.marriages = Utils.GetMarriagesSelectList(marriages, people);

            return View(nameof(SonController.Son), son);
        }

        [HttpGet]
        public ActionResult AddSon()
            => Son(new Son());

        private ActionResult RedirectToMessage(string message, bool success)
            => RedirectToAction(
                    nameof(MessageController.Message),
                    "Message",
                    new { message = message, success = success }
                );

        [HttpPost]
        public ActionResult PersistSon(Son son)
        {
            if (!ModelState.IsValid)
                return BadRequest("State is invalid");

            if (!son.id.HasValue)
                son = _sonRepository.Add(son);
            else
                _sonRepository.Update(son);

            ModelState.Clear();

            return Son(son);
        }

        [HttpGet]
        public ActionResult SonById(int id)
        {
            var son = _sonRepository.GetById(id);
            return Son(son);
        }

        [HttpGet]
        public ActionResult SonByPersonId(int personId)
        {
            var son = _sonRepository.GetByPersonId(personId);
            if (son == null)
                son = new Son { person_id = personId };
            return Son(son);
        }
        
        [HttpGet]
        public ActionResult DeleteSon(int id)
        {
            try
            {
                _sonRepository.RemoveById(id);
                return RedirectToMessage("Son deleted succesfully", true);
            }
            catch (Exception ex)
            {
                return RedirectToMessage("Error deleting son: " + ex.Message, false);
            }
        }
        
    }

}

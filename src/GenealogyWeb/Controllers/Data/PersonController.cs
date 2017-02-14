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
    public class PersonController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger _logger;

        private PersonRepository _personRepository;
        private MarriageRepository _marriageRepository;
        private SonRepository _sonRepository;
        
        public PersonController(
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

        private ActionResult Person(Person person)
            => View(nameof(PersonController.Person), person);
        
        private ActionResult RedirectToMessage(string message, bool success)
            => RedirectToAction(
                    nameof(MessageController.Message),
                    "Message",
                    new { message = message, success = success }
                );

        [HttpGet]
        public ActionResult AddPerson()
            => Person(new Person());

        [HttpPost]
        public ActionResult PersistPerson(Person person)
        {
            if (!ModelState.IsValid)
                return BadRequest("State is invalid");

            if (!person.id.HasValue)
                person = _personRepository.Add(person);
            else
                _personRepository.Update(person);

            return Person(person);
        }

        [HttpGet]
        public ActionResult PersonById(int id)
        {
            var person = _personRepository.GetById(id);
            return Person(person);
        }

        [HttpGet]
        public ActionResult DeletePerson(int id)
        {
            try
            {
                _personRepository.RemoveById(id);
                return RedirectToMessage("Person deleted succesfully", true);
            }
            catch (Exception ex)
            {
                return RedirectToMessage("Error deleting person: " + ex.Message, false);
            }
        }        

    }

}

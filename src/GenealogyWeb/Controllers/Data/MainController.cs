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
    public class MainController : Controller
    {
        private const string NodeTreeViewName = "NodeTree";

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger _logger;

        private PersonRepository _personRepository;
        private MarriageRepository _marriageRepository;
        private SonRepository _sonRepository;
        
        public MainController(
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

        public IActionResult Index()
        {
            var people = _personRepository.GetAll();
            var marriages = _marriageRepository.GetAll();
            var sons = _sonRepository.GetAll();

            ViewBag.people = Utils.GetPersonsSelectList(people);
            ViewBag.marriages = Utils.GetMarriagesSelectList(marriages, people);
            ViewBag.sons = Utils.GetSonsSelectList(sons, marriages, people);           

            return View();
        }

    }

}

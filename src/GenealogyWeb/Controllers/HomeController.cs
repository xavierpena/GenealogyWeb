using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace GenealogyWeb.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            var isAuthenticated = User.Identity.IsAuthenticated;

            var isAllowed = false;
            if(isAuthenticated)
            {
                if (User.Identity.Name == "xavierpenya@gmail.com" || User.Identity.Name == "joana.pinya@gmail.com")
                    isAllowed = true;
            }

            if (isAllowed)
                return RedirectToAction(nameof(MainController.Index), "Main");
            else
                return View();            
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}

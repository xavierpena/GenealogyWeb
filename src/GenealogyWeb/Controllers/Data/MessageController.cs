using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GenealogyWeb.Controllers
{
    public class MessageController : Controller
    {
        [HttpGet]
        public ActionResult Message(string message, bool success)
        {
            ViewBag.Success = success;
            ViewBag.Message = message;
            return View(nameof(Message));
        }
    }
}

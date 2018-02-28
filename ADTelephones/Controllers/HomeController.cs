using ADTelephones.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ADTelephones.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index(string p_text="")
        {
            
            return View(Telefon.GetContacts(p_text.ToString()));
                        
        }

        


    }
}
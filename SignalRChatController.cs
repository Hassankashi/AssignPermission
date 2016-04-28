using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Treeview.Controllers
{
    public class SignalRChatController : Controller
    {
        //
        // GET: /SignalRChat/

        public ActionResult Chat()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

    }
}

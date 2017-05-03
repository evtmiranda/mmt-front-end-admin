using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace marmitex_admin.Controllers
{
    public class ParceiroController : BaseController
    {
        // GET: Parceiro
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Adicionar()
        {
            return View();
        }
    }
}
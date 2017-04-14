using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace marmitex_admin.Controllers
{
    public class ProdutoController : Controller
    {
        // GET: Produtos
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
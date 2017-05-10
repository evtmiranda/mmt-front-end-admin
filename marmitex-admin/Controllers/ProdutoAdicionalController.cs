using ClassesMarmitex;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace marmitex_admin.Controllers
{
    public class ProdutoAdicionalController : Controller
    {
        // GET: ProdutoAdicional
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Editar()
        {
            return View();
        }

        public ActionResult Detalhes(int id)
        {
            DadosProdutoAdicional dadosProdutoAdicional = new DadosProdutoAdicional();

            return View(dadosProdutoAdicional);
        }
    }
}
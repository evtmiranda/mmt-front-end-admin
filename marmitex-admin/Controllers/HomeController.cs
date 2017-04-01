using ClassesMarmitex;
using System.Web.Mvc;

namespace marmitex_admin.Controllers
{
    public class HomeController : BaseController
    {
        // GET: Home
        public ActionResult Index()
        {
            UsuarioLoja usuarioLogado = (UsuarioLoja)Session["usuarioLogado"];

            return View();
        }
    }
}
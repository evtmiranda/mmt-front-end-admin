using ClassesMarmitex;
using System.Web.Mvc;

namespace marmitex_admin.Controllers
{
    public class HomeController : BaseController
    {
        // GET: Home
        public ActionResult Index()
        {
            //se a sessão de usuário não estiver preenchida, direciona para a tela de login
            if (Session["UsuarioLogado"] == null)
            {
                Session["MensagemAutenticacao"] = "estamos com dificuldade em buscar dados no servidor. por favor, tente novamente";
                return RedirectToAction("Index", "Login");
            }

            //recebe o usuário logado
            usuarioLogado = (UsuarioLoja)(Session["UsuarioLogado"]);
            usuarioLogado.UrlLoja = BuscarUrlLoja();

            return View();
        }
    }
}
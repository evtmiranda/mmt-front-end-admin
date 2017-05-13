using ClassesMarmitex;
using System.Web.Mvc;

namespace marmitex_admin.Controllers
{
    public class FormaPagamentoController : BaseController
    {
        // GET: FormasPagamento
        public ActionResult Index()
        {
            #region validacao usuario logado

            //se a sessão de usuário não estiver preenchida, direciona para a tela de login
            if (Session["UsuarioLogado"] == null)
                return RedirectToAction("Index", "Login");

            //recebe o usuário logado
            usuarioLogado = (UsuarioLoja)(Session["UsuarioLogado"]);

            #endregion

            return View();
        }
    }
}
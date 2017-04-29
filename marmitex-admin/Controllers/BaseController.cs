using System.Web.Mvc;
using ClassesMarmitex;

namespace marmitex_admin.Controllers
{
    public class BaseController : Controller
    {
        public UsuarioLoja usuario;

        public BaseController()
        {
            usuario = new UsuarioLoja();
        }

        //sempre que uma requisição é feita em uma classe que herda esta, 
        //esse método é executado para validar se o usuário está logado
        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            //se estiver na tela de login, não faz as verificações seguintes
            if (Request.Url.AbsolutePath.Contains("login") || Request.Url.AbsolutePath.Contains("Login"))
                return;

            //verifica a sessão de usuario
            if (Session["UsuarioLogado"] == null)
                filterContext.HttpContext.Response.Redirect("/Login/Index");

            //preenche o objeto de usuario
            usuario = (UsuarioLoja)Session["UsuarioLogado"];

            if (Session["urlBase"] == null)
                //cria sessão para armazenar a url base
                Session["urlBase"] = Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/') + "/";
        }
    }
}
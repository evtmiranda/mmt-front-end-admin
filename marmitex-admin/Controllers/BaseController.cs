using System.Web.Mvc;

namespace marmitex_admin.Controllers
{
    public class BaseController : Controller
    {
        public BaseController()
        {
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

            //cria sessão para armazenar a url base
            if (Session["urlBase"] == null)
                Session["urlBase"] = Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/') + "/";
        }
    }
}
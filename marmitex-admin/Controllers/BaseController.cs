using ClassesMarmitex;
using System.Web.Mvc;

namespace marmitex_admin.Controllers
{
    public class BaseController : Controller
    {
        public UsuarioLoja usuarioLogado;

        //sempre que um controller for carregado irá passar antes por este método
        //aqui o usuárioLogado é preenchido
        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (Session["urlBase"] == null)
                //cria sessão para armazenar a url base
                Session["urlBase"] = Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/') + "/";
        }

        /// <summary>
        /// busca a url da página atual
        /// </summary>
        /// <returns></returns>
        public string BuscarUrlLoja()
        {
            //captura o host atual
            return Request.Url.Host.Replace('"', ' ').Trim();
        }
    }
}
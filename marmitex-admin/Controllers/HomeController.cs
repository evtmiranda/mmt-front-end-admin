using ClassesMarmitex;
using System.Web.Mvc;
using System.Net;
using Newtonsoft.Json;
using System;

namespace marmitex_admin.Controllers
{
    public class HomeController : BaseController
    {
        private RequisicoesREST rest;
        private DadosRequisicaoRest retornoRequest;

        //O Ninject é o responsável por cuidar da criação de todos esses objetos
        public HomeController(RequisicoesREST rest)
        {
            this.rest = rest;
        }

        public ActionResult Index()
        {
            try
            {
                #region validacao usuario logado

                //se a sessão de usuário não estiver preenchida, direciona para a tela de login
                if (Session["UsuarioLogado"] == null)
                    return RedirectToAction("Index", "Login");

                //recebe o usuário logado
                usuarioLogado = (UsuarioLoja)(Session["UsuarioLogado"]);

                #endregion

                #region limpa as viewbags de mensagem

                ViewBag.MensagemDashboard = null;

                #endregion

                Dashboard dash = new Dashboard();
                retornoRequest = rest.Get("/Dashboard/" + usuarioLogado.IdLoja);

                if (retornoRequest.HttpStatusCode != HttpStatusCode.OK)
                {
                    ViewBag.MensagemDashboard = "não foi possível carregar o dashboard. por favor, tente atualizar a página ou entre em contato com o administrador do sistema.";
                    return View();
                }

                string jsonRetorno = retornoRequest.objeto.ToString();

                dash = JsonConvert.DeserializeObject<Dashboard>(jsonRetorno);

                return View(dash);
            }
            catch (Exception)
            {
                ViewBag.MensagemDashboard = "não foi possível carregar o dashboard. por favor, tente atualizar a página ou entre em contato com o administrador do sistema.";
                return View();
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Mvc;
using ClassesMarmitex;
using marmitex_admin.Utils;
using Newtonsoft.Json;

namespace marmitex_admin.Controllers
{
    public class UsuarioController : BaseController
    {
        private RequisicoesREST rest;
        private DadosRequisicaoRest retornoRequest;

        //O Ninject é o responsável por cuidar da criação de todos esses objetos
        public UsuarioController(RequisicoesREST rest)
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

                ViewBag.MensagemUsuario = null;

                #endregion

                List<UsuarioLoja> listaUsuarios = new List<UsuarioLoja>();

                //busca todos os cardápios da loja
                retornoRequest = rest.Get(string.Format("/usuario/listar/{0}/{1}", TipoUsuario.Loja, usuarioLogado.IdLoja));

                //se ocorrer algum erro
                if (retornoRequest.HttpStatusCode != HttpStatusCode.OK)
                {
                    ViewBag.MensagemUsuario = "não foi possível consultar os usuários. por favor, tente atualizar a página ou entre em contato com o administrador do sistema...";
                    return View();
                }

                string jsonRetorno = retornoRequest.objeto.ToString();

                listaUsuarios = JsonConvert.DeserializeObject<List<UsuarioLoja>>(jsonRetorno);

                return View(listaUsuarios);
            }
            catch (Exception)
            {
                ViewBag.MensagemUsuario = "não foi possível consultar os usuários. por favor, tente atualizar a página ou entre em contato com o administrador do sistema...";
                return View();
            }

        }

        public ActionResult Adicionar()
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

        public ActionResult AdicionarUsuario(UsuarioLoja usuarioLoja)
        {
            #region validacao usuario logado

            //se a sessão de usuário não estiver preenchida, direciona para a tela de login
            if (Session["UsuarioLogado"] == null)
                return RedirectToAction("Index", "Login");

            //recebe o usuário logado
            usuarioLogado = (UsuarioLoja)(Session["UsuarioLogado"]);

            #endregion

            #region limpa as viewbags de mensagem

            ViewBag.MensagemCadUsuario = null;

            #endregion

            #region validação dos campos

            //validação dos campos
            if (!ModelState.IsValid)
            {
                return View("Adicionar", usuarioLoja);
            }

            #endregion

            DadosRequisicaoRest retornoRequest = new DadosRequisicaoRest();

            try
            {
                string urlPost = "/Usuario/Cadastrar/UsuarioLoja/" + usuarioLogado.UrlLoja;

                usuarioLoja.IdLoja = usuarioLogado.IdLoja;
                retornoRequest = rest.Post(urlPost, usuarioLoja);

                if (retornoRequest.HttpStatusCode == HttpStatusCode.Unauthorized)
                {
                    ViewBag.MensagemCadUsuario = "Já existe um usuário com este e-mail. Por favor, verifique os dados digitados";
                    return View("Adicionar", usuarioLoja);
                }

                if (retornoRequest.HttpStatusCode != HttpStatusCode.Created)
                {
                    ViewBag.MensagemCadUsuario = "Não foi possivel cadastrar o usuário. Por favor, tente novamente ou entre em contato com nosso suporte.";
                    return View("Adicionar", usuarioLoja);
                }

                return RedirectToAction("Index", "Usuario");
            }
            catch (Exception)
            {
                ViewBag.MensagemCadUsuario = "Não foi possivel cadastrar o usuário. Por favor, tente novamente ou entre em contato com nosso suporte.";
                return View("Adicionar", usuarioLoja);
            }
        }

        public ActionResult Editar(int id)
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

                ViewBag.MensagemCarregamentoEditarUsuario = null;

                #endregion

                UsuarioLoja usuarioLoja = new UsuarioLoja();

                //busca todos os cardápios da loja
                retornoRequest = rest.Get("/Usuario/BuscarUsuarioLoja/" + id);

                //se ocorrer algum erro
                if (retornoRequest.HttpStatusCode != HttpStatusCode.OK)
                {
                    ViewBag.MensagemCarregamentoEditarUsuario = retornoRequest.objeto.ToString();
                    return View();
                }

                string jsonRetorno = retornoRequest.objeto.ToString();

                usuarioLoja = JsonConvert.DeserializeObject<UsuarioLoja>(jsonRetorno);

                return View(usuarioLoja);
            }
            catch (Exception)
            {
                ViewBag.MensagemCarregamentoEditarUsuario = "Não foi consultar o usuário. Por favor, tente novamente ou entre em contato com nosso suporte.";
                return View();
            }

        }

        public ActionResult EditarUsuario(UsuarioLoja usuarioLoja)
        {
            #region validacao usuario logado

            //se a sessão de usuário não estiver preenchida, direciona para a tela de login
            if (Session["UsuarioLogado"] == null)
                return RedirectToAction("Index", "Login");

            //recebe o usuário logado
            usuarioLogado = (UsuarioLoja)(Session["UsuarioLogado"]);

            #endregion

            #region limpa as viewbags de mensagem

            ViewBag.MensagemEditarUsuario = null;

            #endregion

            #region validação dos campos

            //validação dos campos
            if (!ModelState.IsValid)
                return View("Editar", usuarioLoja);

            #endregion

            //variável para armazenar o retorno da requisição
            DadosRequisicaoRest retornoRequest = new DadosRequisicaoRest();

            try
            {
                string urlPost = string.Format("/Usuario/AtualizarUsuarioLoja");

                usuarioLoja.IdLoja = usuarioLogado.IdLoja;
                retornoRequest = rest.Post(urlPost, usuarioLoja);

                if (retornoRequest.HttpStatusCode != HttpStatusCode.OK)
                {
                    ViewBag.MensagemEditarUsuario = retornoRequest.objeto.ToString();
                    return View("Editar", usuarioLoja);
                }

                return RedirectToAction("Index", "Usuario");
            }
            catch (Exception)
            {
                ViewBag.MensagemEditarUsuario = "Não foi possível atualizar o usuario. Por favor, tente novamente ou entre em contato com nosso suporte.";
                return View("Editar", usuarioLoja);
            }
        }

        [MyErrorHandler]
        public ActionResult Excluir(int id)
        {
            #region validacao usuario logado

            //se a sessão de usuário não estiver preenchida, direciona para a tela de login
            if (Session["UsuarioLogado"] == null)
                return RedirectToAction("Index", "Login");

            //recebe o usuário logado
            usuarioLogado = (UsuarioLoja)(Session["UsuarioLogado"]);

            #endregion

            UsuarioLoja usuarioLoja = new UsuarioLoja()
            {
                Id = id,
                IdLoja = usuarioLogado.IdLoja,
                Ativo = false
            };

            string urlPost = "/Usuario/ExcluirUsuarioLoja";

            retornoRequest = rest.Post(urlPost, usuarioLoja);

            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }

        [MyErrorHandler]
        public ActionResult Desativar(int id)
        {
            #region validacao usuario logado

            //se a sessão de usuário não estiver preenchida, direciona para a tela de login
            if (Session["UsuarioLogado"] == null)
                return RedirectToAction("Index", "Login");

            //recebe o usuário logado
            usuarioLogado = (UsuarioLoja)(Session["UsuarioLogado"]);

            #endregion

            UsuarioLoja usuarioLoja = new UsuarioLoja()
            {
                Id = id,
                IdLoja = usuarioLogado.IdLoja,
                Ativo = false
            };

            string urlPost = "/Usuario/DesativarUsuarioLoja";

            retornoRequest = rest.Post(urlPost, usuarioLoja);

            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }
    }
}
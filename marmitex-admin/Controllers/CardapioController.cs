using System;
using ClassesMarmitex;
using System.Web.Mvc;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Net;
using marmitex_admin.Utils;

namespace marmitex_admin.Controllers
{
    public class CardapioController : BaseController
    {
        private DadosRequisicaoRest retornoRequest;
        private RequisicoesREST rest;
        private List<MenuCardapio> listaCardapio;

        public CardapioController(RequisicoesREST rest)
        {
            this.rest = rest;
            this.listaCardapio = new List<MenuCardapio>();
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

                ViewBag.MensagemCardapio = null;

                #endregion

                //busca todos os cardápios da loja
                retornoRequest = rest.Get("/menucardapio/listar/" + usuarioLogado.IdLoja);

                //se não encontrar cardápios para a loja
                if (retornoRequest.HttpStatusCode == HttpStatusCode.NoContent)
                {
                    ViewBag.MensagemCardapio = "não existem cardápios cadastrados";
                    return View("Index");
                }

                //se ocorrer algum erro
                if (retornoRequest.HttpStatusCode != HttpStatusCode.OK)
                {
                    ViewBag.MensagemCardapio = "não foi possível consultar os cardápios. por favor, tente atualizar a página ou entre em contato com o administrador do sistema...";
                    return View("Index");
                }

                string jsonPedidos = retornoRequest.objeto.ToString();

                listaCardapio = JsonConvert.DeserializeObject<List<MenuCardapio>>(jsonPedidos);

                return View(listaCardapio);
            }
            catch (Exception)
            {
                ViewBag.MensagemCardapio = "não foi possível consultar os cardápios. por favor, tente atualizar a página ou entre em contato com o administrador do sistema...";
                return View("Index");
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

        public ActionResult AdicionarCardapio(MenuCardapio cardapio)
        {
            #region validacao usuario logado

            //se a sessão de usuário não estiver preenchida, direciona para a tela de login
            if (Session["UsuarioLogado"] == null)
                return RedirectToAction("Index", "Login");

            //recebe o usuário logado
            usuarioLogado = (UsuarioLoja)(Session["UsuarioLogado"]);

            #endregion

            #region limpa as viewbags de mensagem

            ViewBag.MensagemCardapio = null;

            #endregion

            //validação dos campos
            if (!ModelState.IsValid)
                return View("Index", cardapio);

            //variável para armazenar o retorno da api
            DadosRequisicaoRest retornoRequest = new DadosRequisicaoRest();

            //tratamento de erros
            try
            {
                //monta a url de chamada na api
                string urlPost = "MenuCardapio/Cadastrar";

                //seta a loja
                cardapio.IdLoja = usuarioLogado.IdLoja;

                //realiza o post passando o usuário no body
                retornoRequest = rest.Post(urlPost, cardapio);

                //se o cadastro for adicionado com sucesso
                if (retornoRequest.HttpStatusCode == HttpStatusCode.Created)
                    return RedirectToAction("Index", "Cardapio");
                else
                {
                    ViewBag.MensagemCardapio = "não foi possível cadastrar o cardápio. por favor, tente novamente ou entre em contato com o administrador do sistema";
                    return View("Adicionar", cardapio);
                }
            }
            //se ocorrer algum erro inesperado
            catch
            {
                ViewBag.MensagemCardapio = "não foi possível cadastrar o cardápio. por favor, tente novamente ou entre em contato com o administrador do sistema";
                return View("Adicionar", cardapio);
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

                ViewBag.MensagemCarregamentoEditarCardapio = null;

                #endregion

                //busca os dados do parceiro
                MenuCardapio cardapio = new MenuCardapio();

                //busca o cardapio pelo id
                retornoRequest = rest.Get(string.Format("/Cardapio/BuscarCardapio/{0}/{1}", id, usuarioLogado.IdLoja));

                //se ocorrer algum erro
                if (retornoRequest.HttpStatusCode != HttpStatusCode.OK)
                {
                    ViewBag.MensagemCarregamentoEditarCardapio = "não foi possível carregar os dados do cardápio. por favor, tente atualizar a página ou entre em contato com o administrador do sistema...";
                    return View();
                }

                string json = retornoRequest.objeto.ToString();

                cardapio = JsonConvert.DeserializeObject<MenuCardapio>(json);

                return View(cardapio);
            }
            catch (Exception)
            {
                ViewBag.MensagemCarregamentoEditarCardapio = "não foi possível carregar os dados do cardápio. por favor, tente atualizar a página ou entre em contato com o administrador do sistema...";
                return View();
            }

        }

        public ActionResult EditarCardapio(MenuCardapio cardapio)
        {
            #region validacao usuario logado

            //se a sessão de usuário não estiver preenchida, direciona para a tela de login
            if (Session["UsuarioLogado"] == null)
                return RedirectToAction("Index", "Login");

            //recebe o usuário logado
            usuarioLogado = (UsuarioLoja)(Session["UsuarioLogado"]);

            #endregion

            #region limpa as viewbags de mensagem

            ViewBag.MensagemEditarCardapio = null;

            #endregion

            //variável para armazenar o retorno da requisição
            DadosRequisicaoRest retornoRequest = new DadosRequisicaoRest();

            try
            {
                string urlPost = string.Format("/MenuCardapio/Atualizar");

                //seta a loja
                cardapio.IdLoja = usuarioLogado.IdLoja;

                retornoRequest = rest.Post(urlPost, cardapio);

                //se o cardápio não for atualizado
                if (retornoRequest.HttpStatusCode != HttpStatusCode.OK)
                {
                    ViewBag.MensagemEditarCardapio = "não foi possível atualizar o cardápio. por favor, tente novamente";
                    return View("Editar", cardapio);
                }

                //se o parceiro for atualizado, direciona para a tela de visualização de parceiros
                return RedirectToAction("Index", "Cardapio");
            }
            catch (Exception)
            {
                ViewBag.MensagemEditarCardapio = "não foi possível atualizar o cardápio. por favor, tente novamente";
                return View("Editar", cardapio);
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

            //busca os dados do cardápio
            MenuCardapio cardapio = new MenuCardapio
            {
                Id = id,
                IdLoja = usuarioLogado.IdLoja,
                Ativo = false
            };

            string urlPost = string.Format("/MenuCardapio/Excluir");
            retornoRequest = rest.Post(urlPost, cardapio);
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

            //busca os dados do cardápio
            MenuCardapio cardapio = new MenuCardapio
            {
                Id = id,
                IdLoja = usuarioLogado.IdLoja,
                Ativo = false
            };

            string urlPost = string.Format("/MenuCardapio/Desativar");
            retornoRequest = rest.Post(urlPost, cardapio);
            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }

    }
}
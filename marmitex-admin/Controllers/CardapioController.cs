using System;
using ClassesMarmitex;
using System.Web.Mvc;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Net;

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

        // GET: Produtos
        public ActionResult Index()
        {
            try
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

        /// <summary>
        /// realiza o cadastro do cardapio
        /// </summary>
        /// <param name="cardapio">dados do cardápio que será cadastrado</param>
        /// <returns></returns>
        public ActionResult AdicionarCardapio(MenuCardapio cardapio)
        {
            //limpa a sessão de mensagens
            Session["MensagemAvisoCadastroCardapio"] = null;

            //validação dos campos
            if (!ModelState.IsValid)
                return View("Index", cardapio);

            //se a sessão de usuário não estiver preenchida, direciona para a tela de login
            if (Session["UsuarioLogado"] == null)
            {
                Session["MensagemAutenticacao"] = "estamos com dificuldade em buscar dados no servidor. por favor, tente novamente";
                return RedirectToAction("Index", "Login");
            }

            //recebe o usuário logado
            usuarioLogado = (UsuarioLoja)(Session["UsuarioLogado"]);
            usuarioLogado.UrlLoja = BuscarUrlLoja();

            //variável para armazenar o retorno da api
            DadosRequisicaoRest retornoRequest = new DadosRequisicaoRest();

            //tratamento de erros
            try
            {
                //monta a url de chamada na api
                string urlPost = string.Format("MenuCardapio/Cadastrar/{0}", usuarioLogado.UrlLoja);

                //realiza o post passando o usuário no body
                retornoRequest = rest.Post(urlPost, cardapio);

                //se o cadastro for adicionado com sucesso
                if (retornoRequest.HttpStatusCode == HttpStatusCode.Created)
                {
                    ViewBag.MensagemCardapio = "cardápio adicionado com sucesso!";
                    return RedirectToAction("Index", "Cardapio");
                }
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
            //se a sessão de usuário não estiver preenchida, direciona para a tela de login
            if (Session["UsuarioLogado"] == null)
            {
                Session["MensagemAutenticacao"] = "estamos com dificuldade em buscar dados no servidor. por favor, tente novamente";
                return RedirectToAction("Index", "Login");
            }

            //recebe o usuário logado
            usuarioLogado = (UsuarioLoja)(Session["UsuarioLogado"]);
            usuarioLogado.UrlLoja = BuscarUrlLoja();

            //busca os dados do parceiro
            MenuCardapio cardapio = new MenuCardapio();

            //busca o cardapio pelo id
            retornoRequest = rest.Get("/Cardapio/BuscarCardapio/" + id);

            //se não encontrar um cardápio com este id
            if (retornoRequest.HttpStatusCode == HttpStatusCode.NotFound)
            {
                ViewBag.MensagemEditarCardapio = "não foi possível carregar os dados do cardápio. por favor, tente atualizar a página ou entre em contato com o administrador do sistema...";
                return View();
            }

            //se ocorrer algum erro
            if (retornoRequest.HttpStatusCode != HttpStatusCode.OK)
            {
                ViewBag.MensagemEditarCardapio = "não foi possível carregar os dados do cardápio. por favor, tente atualizar a página ou entre em contato com o administrador do sistema...";
                return View();
            }

            string json = retornoRequest.objeto.ToString();

            cardapio = JsonConvert.DeserializeObject<MenuCardapio>(json);

            return View(cardapio);
        }

        [HttpPost]
        public ActionResult EditarCardapio(MenuCardapio cardapio)
        {
            //captura a loja em questão
            Session["dominioLoja"] = BuscarUrlLoja();

            //se não conseguir capturar a loja, direciona para a tela de erro
            if (Session["dominioLoja"] == null)
            {
                Session["MensagemAutenticacao"] = "estamos com dificuldade em buscar dados no servidor. por favor, tente atualizar a página";
                return View("Index");
            }

            string dominioLoja = Session["dominioLoja"].ToString();

            //variável para armazenar o retorno da requisição
            DadosRequisicaoRest retornoRequest = new DadosRequisicaoRest();

            try
            {
                string urlPost = string.Format("/MenuCardapio/Atualizar");

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

        public ActionResult Excluir(int id)
        {
            try
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

                //busca os dados do parceiro
                MenuCardapio cardapio = new MenuCardapio
                {
                    Id = id,
                    Ativo = false
                };

                //inativa o parceiro
                string urlPost = string.Format("/MenuCardapio/Excluir");

                retornoRequest = rest.Post(urlPost, cardapio);

                //se o cardápio não for atualizado
                if (retornoRequest.HttpStatusCode != HttpStatusCode.OK)
                {
                    ViewBag.MensagemExcluirCardapio = "não foi possível excluir o cardápio. por favor, tente novamente";
                    return View("Index");
                }

                //se o cardápio for inativado, direciona para a tela de visualização de cardápio
                return RedirectToAction("Index", "Cardapio");
            }
            catch (Exception)
            {
                ViewBag.MensagemExcluirCardapio = "não foi possível excluir o cardápio. por favor, tente novamente";
                return View("Index");
            }
        }

    }
}
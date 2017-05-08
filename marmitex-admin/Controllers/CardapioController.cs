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
                if(usuarioLogado == null)
                    return View("Index");

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

        /// <summary>
        /// realiza o cadastro do cardapio
        /// </summary>
        /// <param name="cardapio">dados do cardápio que será cadastrado</param>
        /// <returns></returns>
        public ActionResult Cadastrar(MenuCardapio cardapio)
        {
            //validação dos campos
            if (!ModelState.IsValid)
                return View("Index", cardapio);

            //variável para armazenar o retorno da api
            DadosRequisicaoRest retornoAutenticacao = new DadosRequisicaoRest();

            //tratamento de erros
            try
            {
                //monta a url de chamada na api
                string urlPost = string.Format("/api/MenuCardapio/Cadastrar/'{0}'", usuarioLogado.UrlLoja);

                //realiza o post passando o usuário no body
                retornoAutenticacao = rest.Post(urlPost, cardapio);

                //se o cadastro for adicionado com sucesso
                if (retornoAutenticacao.HttpStatusCode == HttpStatusCode.Created)
                {
                    Session["MensagemAvisoCadastroCardapio"] = "cardápio adicionado com sucesso!";
                    return View("Index");
                }
                else
                {
                    Session["MensagemAvisoCadastroCardapio"] = "não foi possível cadastrar o cardápio. por favor, tente novamente ou entre em contato com o administrador do sistema";
                    return View("Index");
                }
            }
            //se ocorrer algum erro inesperado
            catch
            {
                Session["MensagemAvisoCadastroCardapio"] = "não foi possível cadastrar o cardápio. por favor, tente novamente ou entre em contato com o administrador do sistema";
                return View("Index");
            }
        }


    }
}
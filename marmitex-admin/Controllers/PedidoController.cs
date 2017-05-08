using ClassesMarmitex;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;
using System.Web.Mvc;
using System;
using System.Web.Http;

namespace marmitex_admin.Controllers
{
    public class PedidoController : BaseController
    {
        private RequisicoesREST rest;
        private DadosRequisicaoRest retornoRequest;
        private List<Pedido> listaPedidos;

        //O Ninject é o responsável por cuidar da criação de todos esses objetos
        public PedidoController(RequisicoesREST rest)
        {
            this.rest = rest;
            this.listaPedidos = new List<Pedido>();
        }

        // GET: Pedido
        /// <summary>
        /// monta um objeto com o cabeçalho do pedido e outro com o pedido completo para ser exibido no modal
        /// </summary>
        /// <returns></returns>
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


                //busca todos os pedidos da loja com data de entrega == hoje
                retornoRequest = rest.Get("/Pedido/BuscarPedidos/" + usuarioLogado.IdLoja + "/true/false/false/false");

                //se não encontrar pedidos para a loja
                if (retornoRequest.HttpStatusCode == HttpStatusCode.NoContent)
                {
                    ViewBag.MensagemPedidos = "não existem pedidos para hoje";
                    return View("Index");
                }

                //se ocorrer algum erro
                if (retornoRequest.HttpStatusCode != HttpStatusCode.OK)
                {
                    ViewBag.MensagemPedidos = "não foi possível consultar os pedidos. por favor, tente atualizar a página ou entre em contato com o administrador do sistema...";
                    return View("Index");
                }

                string jsonPedidos = retornoRequest.objeto.ToString();

                listaPedidos = JsonConvert.DeserializeObject<List<Pedido>>(jsonPedidos);

                return View(listaPedidos);
            }
            catch (Exception)
            {
                ViewBag.MensagemPedidos = "não foi possível exibir os pedidos. por favor, tente atualizar a página ou entre em contato com o administrador do sistema...";
                return View("Index");
            }
        }

        public string AtualizarStatusPedido(string dadosJson)
        {
            DadosAtualizarStatusPedido dadosAtualizarPedido = new DadosAtualizarStatusPedido();

            dadosAtualizarPedido = JsonConvert.DeserializeObject<DadosAtualizarStatusPedido>(dadosJson);

            //se a sessão de usuário não estiver preenchida, direciona para a tela de login
            if (Session["UsuarioLogado"] == null)
                throw new Exception("por favor, faça o login e tente novamente");

            //recebe o usuário logado
            usuarioLogado = (UsuarioLoja)(Session["UsuarioLogado"]);
            usuarioLogado.UrlLoja = BuscarUrlLoja();

            //variável para armazenar o retorno da api
            DadosRequisicaoRest retornoAutenticacao = new DadosRequisicaoRest();

            //tratamento de erros
            try
            {
                //monta a url de chamada na api
                string urlPost = "/Pedido/AtualizarStatusPedido/";

                //realiza o post passando o usuário no body
                retornoAutenticacao = rest.Post(urlPost, dadosAtualizarPedido);

                //se o status não for OK, uma exception é lançada e sera exibida na tela via alert
                if (retornoAutenticacao.HttpStatusCode != HttpStatusCode.OK)
                    return "erro";

                return "sucesso";
            }
            //se ocorrer algum erro inesperado lança a exception
            catch(Exception)
            {
                return "erro";
            }
        }



    }
}
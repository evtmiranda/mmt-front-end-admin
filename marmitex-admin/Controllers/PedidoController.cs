using ClassesMarmitex;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;
using System.Web.Mvc;

namespace marmitex_admin.Controllers
{
    public class PedidoController : BaseController
    {
        private RequisicoesREST rest;
        private UsuarioLoja usuarioLogado;
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
            //recebe o usuário logado
            usuarioLogado = (UsuarioLoja)(Session["UsuarioLogado"]);

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
    }
}
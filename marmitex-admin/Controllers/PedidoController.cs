using ClassesMarmitex;
using System;
using System.Web.Mvc;

namespace marmitex_admin.Controllers
{
    public class PedidoController : Controller
    {
        private RequisicoesREST rest;
        private Requisicoes requisicoes;
        
        //O Ninject é o responsável por cuidar da criação de todos esses objetos
        public PedidoController(RequisicoesREST rest, Requisicoes requisicoes)
        {
            this.rest = rest;
            this.requisicoes = requisicoes;
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
                return View();
            }
            catch (Exception)
            {
                throw;
            }
            
        }
    }
}
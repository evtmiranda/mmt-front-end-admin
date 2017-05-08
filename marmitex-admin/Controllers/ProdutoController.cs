using ClassesMarmitex;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace marmitex_admin.Controllers
{
    public class ProdutoController : BaseController
    {
        private RequisicoesREST rest;
        private DadosRequisicaoRest retornoRequest;

        //O Ninject é o responsável por cuidar da criação de todos esses objetos
        public ProdutoController(RequisicoesREST rest)
        {
            this.rest = rest;
        }

        // GET: Produtos
        /// <summary>
        /// pesquisa os produtos cadastrados na base de dados para exibir na tela
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            if (usuarioLogado == null)
                return View("Index");

            return View();
        }

        /// <summary>
        /// Exibe 
        /// </summary>
        /// <returns></returns>
        public ActionResult Adicionar()
        {
            if (usuarioLogado == null)
                return View("Index");

            return View();
        }
    }
}
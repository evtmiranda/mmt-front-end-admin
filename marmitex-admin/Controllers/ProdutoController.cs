using ClassesMarmitex;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace marmitex_admin.Controllers
{
    public class ProdutoController : BaseController
    {
        private UsuarioLoja usuarioLogado;
        private RequisicoesREST rest;
        private Requisicoes requisicoes;

        //O Ninject é o responsável por cuidar da criação de todos esses objetos
        public ProdutoController(RequisicoesREST rest, Requisicoes requisicoes)
        {
            this.rest = rest;
            this.requisicoes = requisicoes;
        }


        // GET: Produtos
        /// <summary>
        /// pesquisa os produtos cadastrados na base de dados para exibir na tela
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            //recebe o usuário logado
            usuarioLogado = (UsuarioLoja)(Session["UsuarioLogado"]);

            DadosRequisicaoRest dadosRest = new DadosRequisicaoRest();

            try
            {
                //cria uma lista de cardápio
                List<MenuCardapio> listaMenuCardapio = requisicoes.ListarMenuCardapio(usuarioLogado.IdLoja);

                return View(listaMenuCardapio);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
        }

        /// <summary>
        /// Exibe 
        /// </summary>
        /// <returns></returns>
        public ActionResult Adicionar()
        {

            return View();
        }
    }
}
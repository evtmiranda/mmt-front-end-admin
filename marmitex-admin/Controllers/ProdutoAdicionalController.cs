using ClassesMarmitex;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Mvc;

namespace marmitex_admin.Controllers
{
    public class ProdutoAdicionalController : BaseController
    {
        private RequisicoesREST rest;
        private DadosRequisicaoRest retornoRequest;

        //O Ninject é o responsável por cuidar da criação de todos esses objetos
        public ProdutoAdicionalController(RequisicoesREST rest)
        {
            this.rest = rest;
        }

        // GET: ProdutoAdicional
        public ActionResult Index()
        {

            try
            {
                #region verificação de usuário logado

                if (Session["UsuarioLogado"] == null)
                {
                    Session["MensagemAutenticacao"] = "estamos com dificuldade em buscar dados no servidor. por favor, tente novamente";
                    return RedirectToAction("Index", "Login");
                }

                //recebe o usuário logado
                usuarioLogado = (UsuarioLoja)(Session["UsuarioLogado"]);
                usuarioLogado.UrlLoja = BuscarUrlLoja();

                #endregion

                #region busca os dados da loja

                Loja loja = new Loja();

                string urlPostLoja = string.Format("/Loja/BuscarLoja/{0}", usuarioLogado.UrlLoja);

                //busca o id da loja
                retornoRequest = rest.Get(urlPostLoja);

                //verifica se a loja foi encontrada
                if (retornoRequest.HttpStatusCode != HttpStatusCode.OK)
                    throw new Exception();

                loja = JsonConvert.DeserializeObject<Loja>(retornoRequest.objeto.ToString());

                #endregion

                #region busca os produtos adicionais

                List<DadosProdutoAdicional> listaDadosProdutoAdicional = new List<DadosProdutoAdicional>();

                //busca todos os cardápios da loja
                retornoRequest = rest.Get("/ProdutoAdicional/listar/" + loja.Id);

                string jsonPedidos = retornoRequest.objeto.ToString();

                listaDadosProdutoAdicional = JsonConvert.DeserializeObject<List<DadosProdutoAdicional>>(jsonPedidos);

                //retorna para a view "Index" com os produtos adicionais
                return View(listaDadosProdutoAdicional);

                #endregion
            }
            catch (Exception)
            {
                ViewBag.MensagemPedidos = "não foi possível exibir os produtos adicionais. por favor, tente atualizar a página ou entre em contato com o administrador do sistema...";
                return View("Index");
            }

        }

        public ActionResult Editar()
        {
            return View();
        }

        public ActionResult Detalhes(int id)
        {
            DadosProdutoAdicional dadosProdutoAdicional = new DadosProdutoAdicional();

            return View(dadosProdutoAdicional);
        }
    }
}
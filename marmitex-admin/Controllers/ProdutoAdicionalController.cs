using ClassesMarmitex;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
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
                #region validacao usuario logado

                //se a sessão de usuário não estiver preenchida, direciona para a tela de login
                if (Session["UsuarioLogado"] == null)
                    return RedirectToAction("Index", "Login");

                //recebe o usuário logado
                usuarioLogado = (UsuarioLoja)(Session["UsuarioLogado"]);

                #endregion

                #region busca os produtos adicionais

                List<DadosProdutoAdicional> listaDadosProdutoAdicional = new List<DadosProdutoAdicional>();

                //busca todos os cardápios da loja
                retornoRequest = rest.Get("/ProdutoAdicional/listar/" + usuarioLogado.IdLoja);

                string jsonPedidos = retornoRequest.objeto.ToString();

                listaDadosProdutoAdicional = JsonConvert.DeserializeObject<List<DadosProdutoAdicional>>(jsonPedidos);

                //retorna para a view "Index" com os produtos adicionais
                return View(listaDadosProdutoAdicional);

                #endregion
            }
            catch (Exception ex)
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
            try
            {
                #region validacao usuario logado

                //se a sessão de usuário não estiver preenchida, direciona para a tela de login
                if (Session["UsuarioLogado"] == null)
                    return RedirectToAction("Index", "Login");

                //recebe o usuário logado
                usuarioLogado = (UsuarioLoja)(Session["UsuarioLogado"]);

                #endregion

                #region busca os itens dos produtos adicionais

                List<DadosProdutoAdicional> listaDadosProdutoAdicional = new List<DadosProdutoAdicional>();

                //busca todos os cardápios da loja
                retornoRequest = rest.Get("/ProdutoAdicional/listar/" + usuarioLogado.IdLoja);

                string jsonPedidos = retornoRequest.objeto.ToString();

                listaDadosProdutoAdicional = JsonConvert.DeserializeObject<List<DadosProdutoAdicional>>(jsonPedidos);

                //filtra os produtos adicionais 
                listaDadosProdutoAdicional = listaDadosProdutoAdicional.Where(p => p.Id == id).ToList();

                //retorna para a view "Detalhes" com os produtos adicionais
                return View(listaDadosProdutoAdicional.FirstOrDefault());

                #endregion
            }
            catch (Exception)
            {
                ViewBag.MensagemPedidos = "não foi possível exibir os produtos adicionais. por favor, tente atualizar a página ou entre em contato com o administrador do sistema...";
                return View("Index");
            }

        }

        public ActionResult Adicionar()
        {
            return View();
        }

        public ActionResult AdicionarProdutoAdicional(DadosProdutoAdicional prodAdicional)
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

                #region adiciona o produto adicional

                string urlPost = string.Format("/ProdutoAdicional/Adicionar/{0}", usuarioLogado.IdLoja);

                retornoRequest = rest.Post(urlPost, prodAdicional);

                //se o produto adicional for cadastrado, direciona para a tela de visualização de produtos
                if (retornoRequest.HttpStatusCode == HttpStatusCode.Created)
                    return RedirectToAction("Index", "ProdutoAdicional");

                ViewBag.MensagemErroCadProdAdicional = "não foi possível cadastrar o produto adicional. por favor, tente novamente";
                return View("Adicionar", prodAdicional);

                #endregion
            }
            catch (Exception)
            {
                ViewBag.MensagemErroCadProdAdicional = "não foi possível cadastrar o produto adicional. por favor, tente novamente";
                return View("Index");
            }
        }
    }
}
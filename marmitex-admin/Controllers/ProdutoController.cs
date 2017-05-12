﻿using ClassesMarmitex;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
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

                #region busca os cardápios

                List<MenuCardapio> listaMenuCardapio = new List<MenuCardapio>();

                //busca todos os cardápios da loja
                retornoRequest = rest.Get("/menucardapio/listar/" + loja.Id);

                string jsonPedidos = retornoRequest.objeto.ToString();

                listaMenuCardapio = JsonConvert.DeserializeObject<List<MenuCardapio>>(jsonPedidos);

                //retorna para a view "Index" com os cardápios
                return View(listaMenuCardapio);

                #endregion
            }
            catch (Exception ex)
            {
                ViewBag.MensagemPedidos = "não foi possível exibir os produtos. por favor, tente atualizar a página ou entre em contato com o administrador do sistema...";
                return View("Index");
            }
        }

        /// <summary>
        /// Carrega a view de adição de produtos
        /// </summary>
        /// <returns></returns>
        public ActionResult Adicionar()
        {
            #region verificação de usuário logado

            //se a sessão de usuário não estiver preenchida, direciona para a tela de login
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

            #region busca os cardápios

            List<MenuCardapio> listaMenuCardapio = new List<MenuCardapio>();

            //busca todos os cardápios da loja
            retornoRequest = rest.Get("/menucardapio/listar/" + loja.Id);

            string jsonPedidos = retornoRequest.objeto.ToString();

            listaMenuCardapio = JsonConvert.DeserializeObject<List<MenuCardapio>>(jsonPedidos);

            //view bag com os cardápios
            ViewBag.MenuCardapio = listaMenuCardapio;

            //retorna para a view "Adicionar"
            return View();

            #endregion
        }

        public ActionResult Detalhes(int id)
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

                #region busca o produto

                Produto produto = new Produto();

                //busca todos os cardápios da loja
                retornoRequest = rest.Get("/Produto/" + id);

                string jsonRetorno = retornoRequest.objeto.ToString();

                produto = JsonConvert.DeserializeObject<Produto>(jsonRetorno);

                #endregion

                #region busca a relação de produtos adicionais do produto

                ProdutoDetalhes produtoDetalhes = new ProdutoDetalhes()
                {
                    Produto = produto
                };

                List<DadosProdutoAdicionalProduto> listaDadosProdutoAdicionalProduto = new List<DadosProdutoAdicionalProduto>();

                //busca os produtos adicionais deste produto
                retornoRequest = rest.Get("/ProdutoAdicional/BuscarProdutosAdicionaisDeUmProduto/" + id);

                jsonRetorno = retornoRequest.objeto.ToString();

                listaDadosProdutoAdicionalProduto = JsonConvert.DeserializeObject<List<DadosProdutoAdicionalProduto>>(jsonRetorno);

                produtoDetalhes.ProdutosAdicionais = listaDadosProdutoAdicionalProduto;

                #endregion

                return View(produtoDetalhes);
            }
            catch (Exception)
            {
                ViewBag.MensagemPedidos = "não foi possível exibir detalhes do produto  . por favor, tente atualizar a página ou entre em contato com o administrador do sistema...";
                return View("Index");
            }
        }
    }
}
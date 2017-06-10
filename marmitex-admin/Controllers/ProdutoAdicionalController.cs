using ClassesMarmitex;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using marmitex_admin.Utils;

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

                List<DadosProdutoAdicional> listaDadosProdutoAdicional = new List<DadosProdutoAdicional>();

                //busca todos os cardápios da loja
                retornoRequest = rest.Get("/ProdutoAdicional/listar/" + usuarioLogado.IdLoja);

                //se ocorrer algum erro
                if (retornoRequest.HttpStatusCode != HttpStatusCode.OK)
                {
                    ViewBag.MensagemProdutoAdicional = "não foi possível exibir os produtos adicionais. por favor, tente atualizar a página ou entre em contato com o administrador do sistema...";
                    return View();
                }

                string jsonPedidos = retornoRequest.objeto.ToString();

                listaDadosProdutoAdicional = JsonConvert.DeserializeObject<List<DadosProdutoAdicional>>(jsonPedidos);

                //retorna para a view "Index" com os produtos adicionais
                return View(listaDadosProdutoAdicional);
            }
            catch (Exception)
            {
                ViewBag.MensagemProdutoAdicional = "não foi possível exibir os produtos adicionais. por favor, tente atualizar a página ou entre em contato com o administrador do sistema...";
                return View("Index");
            }

        }

        public ActionResult Adicionar()
        {
            #region validacao usuario logado

            //se a sessão de usuário não estiver preenchida, direciona para a tela de login
            if (Session["UsuarioLogado"] == null)
                return RedirectToAction("Index", "Login");

            //recebe o usuário logado
            usuarioLogado = (UsuarioLoja)(Session["UsuarioLogado"]);

            #endregion

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

        public ActionResult Editar(int id)
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

                #region busca o produto adicional

                DadosProdutoAdicional produtoAdicional = new DadosProdutoAdicional();

                //busca os dados do produto adicional
                retornoRequest = rest.Get(string.Format("/ProdutoAdicional/{0}/{1}", id, usuarioLogado.IdLoja));

                //se ocorrer algum erro
                if (retornoRequest.HttpStatusCode != HttpStatusCode.OK)
                {
                    ViewBag.MensagemCarregamentoEditarProdutoAdicional = "não foi possível carregar os dados do produto adicional. por favor, tente atualizar a página ou entre em contato com o administrador do sistema...";
                    return View();
                }

                string jsonRetorno = retornoRequest.objeto.ToString();

                produtoAdicional = JsonConvert.DeserializeObject<DadosProdutoAdicional>(jsonRetorno);

                #endregion

                return View(produtoAdicional);
            }
            catch (Exception)
            {
                ViewBag.MensagemCarregamentoEditarProdutoAdicional = "não foi possível carregar os dados do produto adicional. por favor, tente atualizar a página ou entre em contato com o administrador do sistema...";
                return View();
            }

        }

        public ActionResult EditarProdutoAdicional(DadosProdutoAdicional prodAdicional)
        {
            #region validacao usuario logado

            //se a sessão de usuário não estiver preenchida, direciona para a tela de login
            if (Session["UsuarioLogado"] == null)
                return RedirectToAction("Index", "Login");

            //recebe o usuário logado
            usuarioLogado = (UsuarioLoja)(Session["UsuarioLogado"]);

            #endregion

            //variável para armazenar o retorno da requisição
            DadosRequisicaoRest retornoRequest = new DadosRequisicaoRest();

            try
            {
                string urlPost = string.Format("/ProdutoAdicional/Atualizar");

                //seta a loja
                prodAdicional.IdLoja = usuarioLogado.IdLoja;

                retornoRequest = rest.Post(urlPost, prodAdicional);

                //se o produto adicional não for atualizado
                if (retornoRequest.HttpStatusCode != HttpStatusCode.OK)
                {
                    ViewBag.MensagemEditarProdutoAdicional = "não foi possível atualizar o produto adicional. por favor, tente novamente";
                    return View("Editar", prodAdicional);
                }

                //se o produto adicional for atualizado, direciona para a tela de visualização de produtos adicionais
                return RedirectToAction("Index", "ProdutoAdicional");
            }
            catch (Exception)
            {
                ViewBag.MensagemEditarProdutoAdicional = "não foi possível atualizar o produto adicional. por favor, tente novamente";
                return View("Editar", prodAdicional);
            }
        }

        [MyErrorHandler]
        public ActionResult Excluir(int id)
        {
            #region validacao usuario logado

            //se a sessão de usuário não estiver preenchida, direciona para a tela de login
            if (Session["UsuarioLogado"] == null)
                return RedirectToAction("Index", "Login");

            //recebe o usuário logado
            usuarioLogado = (UsuarioLoja)(Session["UsuarioLogado"]);

            #endregion

            DadosProdutoAdicional produtoAdicional = new DadosProdutoAdicional()
            {
                Id = id,
                IdLoja = usuarioLogado.IdLoja,
                Ativo = false
            };

            //inativa o parceiro
            string urlPost = "/ProdutoAdicional/Excluir";
            retornoRequest = rest.Post(urlPost, produtoAdicional);
            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }

        [MyErrorHandler]
        public ActionResult Desativar(int id)
        {

            #region validacao usuario logado

            //se a sessão de usuário não estiver preenchida, direciona para a tela de login
            if (Session["UsuarioLogado"] == null)
                return RedirectToAction("Index", "Login");

            //recebe o usuário logado
            usuarioLogado = (UsuarioLoja)(Session["UsuarioLogado"]);

            #endregion

            DadosProdutoAdicional produtoAdicional = new DadosProdutoAdicional()
            {
                Id = id,
                IdLoja = usuarioLogado.IdLoja,
                Ativo = false
            };

            //inativa o parceiro
            string urlPost = "/ProdutoAdicional/Desativar";
            retornoRequest = rest.Post(urlPost, produtoAdicional);
            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
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

                List<DadosProdutoAdicional> listaDadosProdutoAdicional = new List<DadosProdutoAdicional>();

                //busca todos os cardápios da loja
                retornoRequest = rest.Get("/ProdutoAdicional/listar/" + usuarioLogado.IdLoja);

                if (retornoRequest.HttpStatusCode != HttpStatusCode.OK) { 
                    ViewBag.MensagemDetalhesProdutoAdicional = "não foi possível exibir detalhes do produto adicional. por favor, tente novamente ou entre em contato com o administrador do sistema.";
                    return View();
                }

                string jsonPedidos = retornoRequest.objeto.ToString();
                listaDadosProdutoAdicional = JsonConvert.DeserializeObject<List<DadosProdutoAdicional>>(jsonPedidos);

                //filtra os produtos adicionais 
                listaDadosProdutoAdicional = listaDadosProdutoAdicional.Where(p => p.Id == id).ToList();

                ////filtra os produtos adicionais ativos
                //listaDadosProdutoAdicional = listaDadosProdutoAdicional.Where(p => p.Ativo).ToList();

                //retorna para a view "Detalhes" com os produtos adicionais
                return View(listaDadosProdutoAdicional.FirstOrDefault());
            }
            catch (Exception)
            {
                ViewBag.MensagemDetalhesProdutoAdicional = "não foi possível exibir detalhes do produto adicional. por favor, tente novamente ou entre em contato com o administrador do sistema.";
                return View();
            }

        }

        #region itens dos produtos adicionais

        public ActionResult AdicionarItem(int id)
        {
            DadosProdutoAdicionalItem item = new DadosProdutoAdicionalItem()
            {
                IdProdutoAdicional = id
            };

            return View(item);
        }

        public ActionResult AdicionarItemProdutoAdicional(DadosProdutoAdicionalItem item)
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

                string urlPost = "/ProdutoAdicional/AdicionarItem";

                //seta a loja
                item.IdLoja = usuarioLogado.IdLoja;

                retornoRequest = rest.Post(urlPost, item);

                //se o item for cadastrado, direciona para a tela de visualização de itens do produto adicional
                if (retornoRequest.HttpStatusCode == HttpStatusCode.Created)
                    return RedirectToAction("Detalhes", "ProdutoAdicional", new { id = item.IdProdutoAdicional });

                ViewBag.MensagemErroCadProdAdicionalItem = "não foi possível cadastrar o item. por favor, tente novamente";
                return View("AdicionarItem", item);

                #endregion
            }
            catch (Exception)
            {
                ViewBag.MensagemErroCadProdAdicionalItem = "não foi possível cadastrar o item. por favor, tente novamente";
                return View("AdicionarItem", item);
            }
        }

        public ActionResult EditarItem(int id)
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

                #region busca o item

                DadosProdutoAdicionalItem item = new DadosProdutoAdicionalItem();

                //busca os dados do produto adicional
                retornoRequest = rest.Get(string.Format("/ProdutoAdicional/Item/{0}/{1}", id, usuarioLogado.IdLoja));

                if (retornoRequest.HttpStatusCode != HttpStatusCode.OK)
                {
                    ViewBag.MensagemCarregamentoItem = "não foi possível carregar os dados do item. por favor, tente atualizar a página ou entre em contato com o administrador do sistema...";
                    return View("Detalhes");
                }

                string jsonRetorno = retornoRequest.objeto.ToString();

                item = JsonConvert.DeserializeObject<DadosProdutoAdicionalItem>(jsonRetorno);

                #endregion

                return View(item);
            }
            catch (Exception)
            {
                ViewBag.MensagemCarregamentoItem = "não foi possível carregar os dados do item. por favor, tente atualizar a página ou entre em contato com o administrador do sistema...";
                return View("Detalhes");
            }

        }

        public ActionResult EditarItemProdutoAdicional(DadosProdutoAdicionalItem item)
        {
            #region validacao usuario logado

            //se a sessão de usuário não estiver preenchida, direciona para a tela de login
            if (Session["UsuarioLogado"] == null)
                return RedirectToAction("Index", "Login");

            //recebe o usuário logado
            usuarioLogado = (UsuarioLoja)(Session["UsuarioLogado"]);

            #endregion

            //variável para armazenar o retorno da requisição
            DadosRequisicaoRest retornoRequest = new DadosRequisicaoRest();

            try
            {
                string urlPost = string.Format("/ProdutoAdicional/AtualizarItem");

                //seta a loja
                item.IdLoja = usuarioLogado.IdLoja;

                retornoRequest = rest.Post(urlPost, item);

                //se o produto adicional não for atualizado
                if (retornoRequest.HttpStatusCode != HttpStatusCode.OK)
                {
                    ViewBag.MensagemEditarProdutoAdicionalItem = "não foi possível atualizar o item. por favor, tente novamente";
                    return View("EditarItem", item);
                }

                //se o item for atualizado, direciona para a tela de visualização de itens do produto adicional
                return RedirectToAction("Detalhes", "ProdutoAdicional", new { id = item.IdProdutoAdicional });
            }
            catch (Exception)
            {
                ViewBag.MensagemEditarProdutoAdicionalItem = "não foi possível atualizar o item. por favor, tente novamente";
                return View("EditarItem", item);
            }
        }

        [MyErrorHandler]
        public ActionResult ExcluirItem(int id)
        {
            #region validacao usuario logado

            //se a sessão de usuário não estiver preenchida, direciona para a tela de login
            if (Session["UsuarioLogado"] == null)
                return RedirectToAction("Index", "Login");

            //recebe o usuário logado
            usuarioLogado = (UsuarioLoja)(Session["UsuarioLogado"]);

            #endregion

            DadosProdutoAdicionalItem item = new DadosProdutoAdicionalItem()
            {
                Id = id,
                IdLoja = usuarioLogado.IdLoja,
                Ativo = false
            };

            //inativa o parceiro
            string urlPost = "/ProdutoAdicional/ExcluirItem";
            retornoRequest = rest.Post(urlPost, item);
            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }

        [MyErrorHandler]
        public ActionResult DesativarItem(int id)
        {
            #region validacao usuario logado

            //se a sessão de usuário não estiver preenchida, direciona para a tela de login
            if (Session["UsuarioLogado"] == null)
                return RedirectToAction("Index", "Login");

            //recebe o usuário logado
            usuarioLogado = (UsuarioLoja)(Session["UsuarioLogado"]);

            #endregion

            DadosProdutoAdicionalItem item = new DadosProdutoAdicionalItem()
            {
                Id = id,
                IdLoja = usuarioLogado.IdLoja,
                Ativo = false
            };

            string urlPost = "/ProdutoAdicional/DesativarItem";
            retornoRequest = rest.Post(urlPost, item);
            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}
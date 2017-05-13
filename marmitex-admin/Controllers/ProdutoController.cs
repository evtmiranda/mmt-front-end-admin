using ClassesMarmitex;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
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
                #region validacao usuario logado

                //se a sessão de usuário não estiver preenchida, direciona para a tela de login
                if (Session["UsuarioLogado"] == null)
                    return RedirectToAction("Index", "Login");

                //recebe o usuário logado
                usuarioLogado = (UsuarioLoja)(Session["UsuarioLogado"]);

                #endregion

                #region busca os cardápios

                List<MenuCardapio> listaMenuCardapio = new List<MenuCardapio>();

                //busca todos os cardápios da loja
                retornoRequest = rest.Get("/menucardapio/listar/" + usuarioLogado.IdLoja);

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
            #region validacao usuario logado

            //se a sessão de usuário não estiver preenchida, direciona para a tela de login
            if (Session["UsuarioLogado"] == null)
                return RedirectToAction("Index", "Login");

            //recebe o usuário logado
            usuarioLogado = (UsuarioLoja)(Session["UsuarioLogado"]);

            #endregion

            #region busca os cardápios

            List<MenuCardapio> listaMenuCardapio = new List<MenuCardapio>();

            //busca todos os cardápios da loja
            retornoRequest = rest.Get("/menucardapio/listar/" + usuarioLogado.IdLoja);

            string jsonPedidos = retornoRequest.objeto.ToString();

            listaMenuCardapio = JsonConvert.DeserializeObject<List<MenuCardapio>>(jsonPedidos);

            //view bag com os cardápios
            ViewBag.MenuCardapio = listaMenuCardapio;

            Produto produto = new Produto();

            if (Session["ProdutoCadastro"] != null)
                produto = Session["ProdutoCadastro"] as Produto;

            Session["ProdutoCadastro"] = null;

            //retorna para a view "Adicionar"
            return View(produto);

            #endregion
        }

        public ActionResult AdicionarProduto(Produto produto, HttpPostedFileBase file)
        {
            #region validacao usuario logado

            //se a sessão de usuário não estiver preenchida, direciona para a tela de login
            if (Session["UsuarioLogado"] == null)
                return RedirectToAction("Index", "Login");

            //recebe o usuário logado
            usuarioLogado = (UsuarioLoja)(Session["UsuarioLogado"]);

            #endregion

            DadosRequisicaoRest retornoRequest = new DadosRequisicaoRest();

            try
            {

                string urlPost = "/Produto/Adicionar/";


                //recebe a imagem do produto
                if (file != null)
                {
                    string pic = System.IO.Path.GetFileName(file.FileName);
                    string path = System.IO.Path.Combine(
                                           Server.MapPath("~/Images/" + usuarioLogado.IdLoja + "/"), pic);

                    string caminhoPasta = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory.ToString()) + "/Images/" + usuarioLogado.IdLoja + "/";

                    //se o diretório ainda não existir, cria um novo
                    if (!Directory.Exists(caminhoPasta))
                        Directory.CreateDirectory(caminhoPasta);

                    //salva a imagem na pasta
                    file.SaveAs(path);

                    //seta o nome da imagem no produto
                    produto.Imagem = pic;
                }

                // after successfully uploading redirect the user
                //return RedirectToAction("actionname", "controller name");

                retornoRequest = rest.Post(urlPost, produto);

                //se o produto for cadastrado, direciona para a tela de visualização de produtos
                if (retornoRequest.HttpStatusCode == HttpStatusCode.Created) {
                    Session["MensagemErroCadProduto"] = null;
                    return RedirectToAction("Index", "Produto");
                }

                Session["MensagemErroCadProduto"] = "não foi possível cadastrar o produto. por favor, tente novamente";
                Session["ProdutoCadastro"] = produto;
                return RedirectToAction("Adicionar", "Produto");
            }
            catch (Exception ex)
            {
                Session["MensagemErroCadProduto"] = "não foi possível cadastrar o produto. por favor, tente novamente";
                return View("Adicionar", produto);
            }
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

                #region busca o produto

                Produto produto = new Produto();

                //busca todos os cardápios da loja
                retornoRequest = rest.Get("/Produto/" + id);

                string jsonRetorno = retornoRequest.objeto.ToString();

                produto = JsonConvert.DeserializeObject<Produto>(jsonRetorno);

                #endregion

                #region busca a relação de produtos adicionais do produto

                List<DadosProdutoAdicionalProduto> listaDadosProdutoAdicionalProduto = new List<DadosProdutoAdicionalProduto>();

                //busca os produtos adicionais deste produto
                retornoRequest = rest.Get("/Produto/BuscarProdutosAdicionaisDeUmProduto/" + id);

                jsonRetorno = retornoRequest.objeto.ToString();

                listaDadosProdutoAdicionalProduto = JsonConvert.DeserializeObject<List<DadosProdutoAdicionalProduto>>(jsonRetorno);

                //se não existirem produtos adicionais para este produto, insere um produto na lista apenas para exibicao
                if (listaDadosProdutoAdicionalProduto.Count == 0)
                    listaDadosProdutoAdicionalProduto.Add(new DadosProdutoAdicionalProduto { IdProduto = produto.Id, NomeProduto = produto.Nome });

                #endregion

                return View(listaDadosProdutoAdicionalProduto);
            }
            catch (Exception)
            {
                ViewBag.MensagemPedidos = "não foi possível exibir detalhes do produto  . por favor, tente atualizar a página ou entre em contato com o administrador do sistema...";
                return View("Index");
            }
        }

        public ActionResult Excluir(int id)
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

                Produto produto = new Produto()
                {
                    Id = id,
                    Ativo = false
                };

                //inativa o parceiro
                string urlPost = "/Produto/Excluir";

                retornoRequest = rest.Post(urlPost, produto);

                //se o parceiro não for atualizado
                if (retornoRequest.HttpStatusCode != HttpStatusCode.OK)
                {
                    ViewBag.MensagemExcluirParceiro = "não foi possível excluir o produto. por favor, tente novamente";
                    return View("Index");
                }

                //se o produto for inativado, direciona para a tela de visualização de parceiros
                return RedirectToAction("Index", "Produto");
            }
            catch (Exception)
            {
                ViewBag.MensagemExcluirParceiro = "não foi possível excluir o produto. por favor, tente novamente";
                return View("Index");
            }
        }

        public ActionResult Editar(int id)
        {
            #region validacao usuario logado

            //se a sessão de usuário não estiver preenchida, direciona para a tela de login
            if (Session["UsuarioLogado"] == null)
                return RedirectToAction("Index", "Login");

            //recebe o usuário logado
            usuarioLogado = (UsuarioLoja)(Session["UsuarioLogado"]);

            #endregion

            #region busca os cardápios

            List<MenuCardapio> listaMenuCardapio = new List<MenuCardapio>();

            //busca todos os cardápios da loja
            retornoRequest = rest.Get("/menucardapio/listar/" + usuarioLogado.IdLoja);

            string jsonPedidos = retornoRequest.objeto.ToString();

            listaMenuCardapio = JsonConvert.DeserializeObject<List<MenuCardapio>>(jsonPedidos);

            //view bag com os cardápios
            ViewBag.MenuCardapio = listaMenuCardapio;

            #endregion

            //busca os dados do produto
            List<Produto> produtosCardapio = new List<Produto>();

            foreach (var cardapio in listaMenuCardapio)
            {
                foreach (var produtoCardapio in cardapio.Produtos)
                {
                    produtosCardapio.Add(produtoCardapio);
                }
            }

            Produto produto = new Produto();

            //busca o parceiro no cardapio pelo id
            produto = produtosCardapio.Where(p => p.Id == id).FirstOrDefault();

            return View(produto);
        }

        public ActionResult EditarProduto(Produto produto, HttpPostedFileBase file)
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
                //recebe a imagem do produto
                if (file != null)
                {
                    string pic = System.IO.Path.GetFileName(file.FileName);
                    string path = System.IO.Path.Combine(
                                           Server.MapPath("~/Images/" + usuarioLogado.IdLoja + "/"), pic);

                    string caminhoPasta = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory.ToString()) + "/Images/" + usuarioLogado.IdLoja + "/";

                    //se o diretório ainda não existir, cria um novo
                    if (!Directory.Exists(caminhoPasta))
                        Directory.CreateDirectory(caminhoPasta);

                    //salva a imagem na pasta
                    file.SaveAs(path);

                    //seta o nome da imagem no produto
                    produto.Imagem = pic;
                }

                string urlPost = string.Format("/Produto/Atualizar");

                retornoRequest = rest.Post(urlPost, produto);

                //se o produto não for atualizado
                if (retornoRequest.HttpStatusCode != HttpStatusCode.OK)
                {
                    Session["MensagemErroEditarProduto"] = "não foi possível atualizar o produto. por favor, tente novamente";
                    return RedirectToAction("Editar", "Produto", new { id = produto.Id });
                }

                //se o produto for atualizado, limpa a sessão de mensagem de edição
                Session["MensagemErroEditarProduto"] = null;

                //se o produto for atualizado, direciona para a tela de visualização de produtos
                return RedirectToAction("Index", "Produto");
            }
            catch (Exception)
            {
                Session["MensagemErroEditarProduto"] = "não foi possível atualizar o produto. por favor, tente novamente";
                return RedirectToAction("Editar", "Produto", new { id = produto.Id });
            }
        }

        public ActionResult AdicionarProdutoAdicional(int id)
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

            string json = retornoRequest.objeto.ToString();

            listaDadosProdutoAdicional = JsonConvert.DeserializeObject<List<DadosProdutoAdicional>>(json);

            ViewBag.ProdutosAdicionais = listaDadosProdutoAdicional;

            #endregion

            DadosProdutoAdicionalProduto dadosProdutoAdicionalProduto = new DadosProdutoAdicionalProduto()
            {
                IdProduto = id
            };

            if (Session["ProdutoAdicionalProdutoCadastro"] != null)
                dadosProdutoAdicionalProduto = Session["ProdutoAdicionalProdutoCadastro"] as DadosProdutoAdicionalProduto;

            Session["ProdutoAdicionalProdutoCadastro"] = null;

            return View(dadosProdutoAdicionalProduto);
        }

        public ActionResult AdicionarProdutoAdicionalProduto(DadosProdutoAdicionalProduto produtoAdicionalProduto)
        {
            #region validacao usuario logado

            //se a sessão de usuário não estiver preenchida, direciona para a tela de login
            if (Session["UsuarioLogado"] == null)
                return RedirectToAction("Index", "Login");

            //recebe o usuário logado
            usuarioLogado = (UsuarioLoja)(Session["UsuarioLogado"]);

            #endregion

            #region cadastra o produto adicional para este produto

            string urlPost = "/Produto/AdicionarProdutoAdicional";

            retornoRequest = rest.Post(urlPost, produtoAdicionalProduto);

            //se o produto adicional for cadastrado, direciona para a tela de visualização de produtos adicionais do produto
            if (retornoRequest.HttpStatusCode == HttpStatusCode.Created)
            {
                Session["MensagemErroCadProdutoAdicional"] = null;
                return RedirectToAction("Detalhes", "Produto", new { id = produtoAdicionalProduto.IdProduto });
            }

            Session["MensagemErroCadProdutoAdicional"] = "não foi possível cadastrar o produto adicional. por favor, tente novamente";
            Session["ProdutoAdicionalProdutoCadastro"] = produtoAdicionalProduto;
            return RedirectToAction("Adicionar", "Produto");

            #endregion
        }

        public ActionResult EditarProdutoAdicional(int id)
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

            string json = retornoRequest.objeto.ToString();

            listaDadosProdutoAdicional = JsonConvert.DeserializeObject<List<DadosProdutoAdicional>>(json);

            ViewBag.ProdutosAdicionais = listaDadosProdutoAdicional;

            #endregion

            #region busca o produto adicional do produto

            DadosProdutoAdicionalProduto dadosProdutoAdicionalProduto = new DadosProdutoAdicionalProduto();

            //busca todos os cardápios da loja
            retornoRequest = rest.Get("/Produto/BuscarProdutoAdicional/" + id);

            string jsonResult = retornoRequest.objeto.ToString();

            dadosProdutoAdicionalProduto = JsonConvert.DeserializeObject<DadosProdutoAdicionalProduto>(jsonResult);

            #endregion

            return View(dadosProdutoAdicionalProduto);
        }

        public ActionResult EditarProdutoAdicionalProduto(DadosProdutoAdicionalProduto produtoAdicional)
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
                string urlPost = string.Format("/Produto/ProdutoAdicional/Atualizar");

                retornoRequest = rest.Post(urlPost, produtoAdicional);

                //se o produto não for atualizado
                if (retornoRequest.HttpStatusCode != HttpStatusCode.OK)
                {
                    Session["MensagemErroEditarProdutoAdicional"] = "não foi possível atualizar o produto. por favor, tente novamente";
                    return RedirectToAction("EditarProdutoAdicional", "Produto", new { id = produtoAdicional.IdProduto });
                }

                //se o produto for atualizado, limpa a sessão de mensagem de edição
                Session["MensagemErroEditarProdutoAdicional"] = null;

                //se o produto for atualizado, direciona para a tela de visualização de produtos adicionais
                return RedirectToAction("Detalhes", "Produto", new { id = produtoAdicional.IdProduto });
            }
            catch (Exception)
            {
                Session["MensagemErroEditarProdutoAdicional"] = "não foi possível atualizar o produto. por favor, tente novamente";
                return RedirectToAction("EditarProdutoAdicional", "Produto", new { id = produtoAdicional.IdProduto });
            }
        }

        public ActionResult ExcluirProdutoAdicional(int id)
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

                //busca os dados do parceiro
                DadosProdutoAdicionalProduto dadosProdutoAdicionalProduto = new DadosProdutoAdicionalProduto
                {
                    Id = id,
                    Ativo = false
                };

                //inativa o parceiro
                string urlPost = string.Format("/Produto/ExcluirProdutoAdicional");

                retornoRequest = rest.Post(urlPost, dadosProdutoAdicionalProduto);

                //se o parceiro não for atualizado
                if (retornoRequest.HttpStatusCode != HttpStatusCode.OK)
                    ViewBag.MensagemExcluirProdutoAdicional = "não foi possível excluir o produto adicional. por favor, tente novamente";

                return RedirectToAction("Index", "Produto");

            }
            catch (Exception)
            {
                ViewBag.MensagemExcluirProdutoAdicional = "não foi possível excluir o produto adicional. por favor, tente novamente";
                return RedirectToAction("Index", "Produto");
            }
        }

    }
}
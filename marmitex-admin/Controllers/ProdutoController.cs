using ClassesMarmitex;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using marmitex_admin.Utils;

namespace marmitex_admin.Controllers
{
    public class ProdutoController : BaseController
    {
        private RequisicoesREST rest;
        private DadosRequisicaoRest retornoRequest = new DadosRequisicaoRest();
        private List<MenuCardapio> listaCardapio;

        //O Ninject é o responsável por cuidar da criação de todos esses objetos
        public ProdutoController(RequisicoesREST rest)
        {
            this.rest = rest;
        }

        #region produto

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

                #region limpa as viewbags de mensagem

                ViewBag.MensagemCardapio = null;

                #endregion

                //busca todos os cardápios da loja
                retornoRequest = rest.Get("/menucardapio/listar/" + usuarioLogado.IdLoja);

                //se não encontrar cardápios para a loja
                if (retornoRequest.HttpStatusCode == HttpStatusCode.NoContent)
                {
                    ViewBag.MensagemCardapio = "não existem cardápios cadastrados";
                    return View();
                }

                //se ocorrer algum erro
                if (retornoRequest.HttpStatusCode != HttpStatusCode.OK)
                {
                    ViewBag.MensagemCardapio = "não foi possível consultar os cardápios. por favor, tente atualizar a página ou entre em contato com o administrador do sistema...";
                    return View();
                }

                string jsonPedidos = retornoRequest.objeto.ToString();

                listaCardapio = JsonConvert.DeserializeObject<List<MenuCardapio>>(jsonPedidos);

                //monta a sessão com o caminho das imagens dos brindes
                string caminhoImagem = "http://" + usuarioLogado.UrlLoja + ":45237/Images/" + usuarioLogado.UrlLoja + "/Produtos/";
                Session["CaminhoImagensProdutos"] = caminhoImagem;

                return View(listaCardapio);
            }
            catch (Exception)
            {
                ViewBag.MensagemCardapio = "não foi possível consultar os cardápios. por favor, tente atualizar a página ou entre em contato com o administrador do sistema...";
                return View();
            }

        }

        public ActionResult Adicionar()
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

                #region limpa as viewbags de mensagem

                ViewBag.MensagemCarregamentoAdicionarProduto = null;

                #endregion

                List<MenuCardapio> listaMenuCardapio = new List<MenuCardapio>();

                //busca todos os cardápios da loja
                retornoRequest = rest.Get("/menucardapio/listar/" + usuarioLogado.IdLoja);

                //se não encontrar cardápios para a loja
                if (retornoRequest.HttpStatusCode == HttpStatusCode.NoContent)
                {
                    ViewBag.MensagemCarregamentoAdicionarProduto = "é necessário cadastrar um cardápio antes do produto";
                    return View();
                }

                //se ocorrer algum erro
                if (retornoRequest.HttpStatusCode != HttpStatusCode.OK)
                {
                    ViewBag.MensagemCarregamentoAdicionarProduto = "não foi possível consultar os cardápios. por favor, tente atualizar a página ou entre em contato com o administrador do sistema...";
                    return View();
                }

                string jsonPedidos = retornoRequest.objeto.ToString();

                listaMenuCardapio = JsonConvert.DeserializeObject<List<MenuCardapio>>(jsonPedidos);

                ViewBag.MenuCardapio = listaMenuCardapio;
                Session["MenuCardapio"] = listaMenuCardapio;

                List<DiasSemana> listaDiasSemana = new List<DiasSemana>
                {
                    new DiasSemana { Id = 0, DiaSemana = "Domingo" },
                    new DiasSemana { Id = 1, DiaSemana = "Segunda-feira" },
                    new DiasSemana { Id = 2, DiaSemana = "Terça-feira" },
                    new DiasSemana { Id = 3, DiaSemana = "Quarta-feira" },
                    new DiasSemana { Id = 4, DiaSemana = "Quinta-feira" },
                    new DiasSemana { Id = 5, DiaSemana = "Sexta-feira" },
                    new DiasSemana { Id = 6, DiaSemana = "Sábado" }
                };

                ViewBag.ListaDiasSemana = listaDiasSemana;
                Session["ListaDiasSemana"] = listaDiasSemana;

                return View();
            }
            catch (Exception)
            {
                ViewBag.MensagemCarregamentoAdicionarProduto = "não foi possível consultar os cardápios. por favor, tente atualizar a página ou entre em contato com o administrador do sistema...";
                return View();
            }

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

            #region limpa as viewbags de mensagem

            ViewBag.MensagemCadProduto = null;

            #endregion

            #region validação dos campos

            //validação dos campos
            if (!ModelState.IsValid)
            {
                ViewBag.ListaDiasSemana = (List<DiasSemana>)Session["ListaDiasSemana"];
                ViewBag.MenuCardapio = (List<MenuCardapio>)Session["MenuCardapio"];

                return View("Adicionar", produto);
            }
                
            #endregion


            DadosRequisicaoRest retornoRequest = new DadosRequisicaoRest();

            try
            {
                string urlPost = "/Produto/Adicionar/";

                //recebe a imagem do produto
                if (file != null)
                {
                    //valida o tamanho da imagem
                    //tamanho maximo permitido é 10 mb
                    if (file.ContentLength > 10000000)
                    {
                        ViewBag.MensagemCadProduto = "a imagem deve ter no máximo 10 megabytes";
                        return View("Adicionar", produto);
                    }

                    string pic = System.IO.Path.GetFileName(file.FileName);
                    string caminhoPasta = ConfigurationManager.AppSettings["PastaImagens"] + usuarioLogado.UrlLoja + "/Produtos/";

                    string path = caminhoPasta + pic;

                    //se o diretório ainda não existir, cria um novo
                    if (!Directory.Exists(caminhoPasta))
                        Directory.CreateDirectory(caminhoPasta);

                    //salva a imagem na pasta
                    file.SaveAs(path);

                    //seta o nome e caminho da imagem no produto
                    produto.Imagem = pic;
                }

                produto.IdLoja = usuarioLogado.IdLoja;
                retornoRequest = rest.Post(urlPost, produto);

                if (retornoRequest.HttpStatusCode != HttpStatusCode.Created)
                {
                    ViewBag.MensagemCadProduto = "não foi possível cadastrar o produto. por favor, tente novamente";
                    return View("Adicionar", produto);
                }

                return RedirectToAction("Index", "Produto");
            }
            catch (Exception)
            {
                ViewBag.MensagemCadProduto = "não foi possível cadastrar o produto. por favor, tente novamente";
                return View("Adicionar", produto);
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

                #region limpa as viewbags de mensagem

                ViewBag.MensagemCarregamentoEditarProduto = null;

                #endregion

                #region busca os cardápios

                List<MenuCardapio> listaMenuCardapio = new List<MenuCardapio>();

                //busca todos os cardápios da loja
                retornoRequest = rest.Get("/menucardapio/listar/" + usuarioLogado.IdLoja);

                //se ocorrer algum erro
                if (retornoRequest.HttpStatusCode != HttpStatusCode.OK)
                {
                    ViewBag.MensagemCarregamentoEditarProduto = "não foi possível carregar os dados do produto. por favor, tente atualizar a página ou entre em contato com o administrador do sistema...";
                    return View();
                }

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
                produto = produtosCardapio.Where(p => p.Id == id).FirstOrDefault();

                List<DiasSemana> listaDiasSemana = new List<DiasSemana>
                {
                    new DiasSemana { Id = 0, DiaSemana = "Domingo" },
                    new DiasSemana { Id = 1, DiaSemana = "Segunda-feira" },
                    new DiasSemana { Id = 2, DiaSemana = "Terça-feira" },
                    new DiasSemana { Id = 3, DiaSemana = "Quarta-feira" },
                    new DiasSemana { Id = 4, DiaSemana = "Quinta-feira" },
                    new DiasSemana { Id = 5, DiaSemana = "Sexta-feira" },
                    new DiasSemana { Id = 6, DiaSemana = "Sábado" }
                };

                ViewBag.ListaDiasSemana = listaDiasSemana;
                Session["ListaDiasSemana"] = listaDiasSemana;

                return View(produto);
            }
            catch (Exception)
            {
                ViewBag.MensagemCarregamentoEditarProduto = "não foi possível carregar os dados do produto. por favor, tente atualizar a página ou entre em contato com o administrador do sistema...";
                return View();
            }

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

            #region limpa as viewbags de mensagem

            ViewBag.MensagemEditarProduto = null;

            #endregion

            #region validação dos campos

            //validação dos campos
            if (!ModelState.IsValid)
                return View("Editar", produto);

            #endregion

            //variável para armazenar o retorno da requisição
            DadosRequisicaoRest retornoRequest = new DadosRequisicaoRest();

            try
            {
                //recebe a imagem do produto
                if (file != null)
                {
                    //valida o tamanho da imagem
                    //tamanho maximo permitido é 10 mb
                    if (file.ContentLength > 10000000)
                    {
                        ViewBag.MensagemEditarProduto = "a imagem deve ter no máximo 10 megabytes";
                        return View("Adicionar", produto);
                    }

                    string pic = System.IO.Path.GetFileName(file.FileName);
                    string caminhoPasta = ConfigurationManager.AppSettings["PastaImagens"] + usuarioLogado.UrlLoja + "/Produtos/";

                    string path = caminhoPasta + pic;

                    //se o diretório ainda não existir, cria um novo
                    if (!Directory.Exists(caminhoPasta))
                        Directory.CreateDirectory(caminhoPasta);

                    //salva a imagem na pasta
                    file.SaveAs(path);

                    //seta o nome da imagem no produto
                    produto.Imagem = pic;
                }

                string urlPost = string.Format("/Produto/Atualizar");

                produto.IdLoja = usuarioLogado.IdLoja;
                retornoRequest = rest.Post(urlPost, produto);

                //se o produto não for atualizado
                if (retornoRequest.HttpStatusCode != HttpStatusCode.OK)
                {
                    ViewBag.MensagemEditarProduto = "não foi possível atualizar o produto. por favor, tente novamente";
                    //return RedirectToAction("Editar", "Produto", new { id = produto.Id });
                    return View("Editar", produto);
                }

                //se o produto for atualizado, direciona para a tela de visualização de produtos
                return RedirectToAction("Index", "Produto");
            }
            catch (Exception)
            {
                ViewBag.MensagemEditarProduto = "não foi possível atualizar o produto. por favor, tente novamente";
                //return RedirectToAction("Editar", "Produto", new { id = produto.Id });
                return View("Editar", produto);
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

            Produto produto = new Produto()
            {
                Id = id,
                IdLoja = usuarioLogado.IdLoja,
                Ativo = false
            };

            //inativa o parceiro
            string urlPost = "/Produto/Excluir";

            retornoRequest = rest.Post(urlPost, produto);

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

            Produto produto = new Produto()
            {
                Id = id,
                IdLoja = usuarioLogado.IdLoja,
                Ativo = false
            };

            //inativa o parceiro
            string urlPost = "/Produto/Desativar";

            retornoRequest = rest.Post(urlPost, produto);

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

                #region limpa as viewbags de mensagem

                ViewBag.MensagemDetalhesProduto = null;

                #endregion

                #region busca o produto

                Produto produto = new Produto();
                string jsonRetorno = null;

                //busca todos os cardápios da loja
                retornoRequest = rest.Get(string.Format("/Produto/{0}/{1}", id, usuarioLogado.IdLoja));

                if (retornoRequest.HttpStatusCode != HttpStatusCode.OK)
                    ViewBag.MensagemDetalhesProduto = "não foi possível exibir detalhes do produto. por favor, tente atualizar a página ou entre em contato com o administrador do sistema...";
                else
                {
                    jsonRetorno = retornoRequest.objeto.ToString();
                    produto = JsonConvert.DeserializeObject<Produto>(jsonRetorno);
                }

                #endregion

                #region busca a relação de produtos adicionais do produto

                List<DadosProdutoAdicionalProduto> listaDadosProdutoAdicionalProduto = new List<DadosProdutoAdicionalProduto>();

                //busca os produtos adicionais deste produto
                retornoRequest = rest.Get(string.Format("/Produto/BuscarProdutosAdicionaisDeUmProduto/{0}/{1}", id, usuarioLogado.IdLoja));

                if (retornoRequest.HttpStatusCode != HttpStatusCode.OK)
                    ViewBag.MensagemDetalhesProduto = "não foi possível exibir detalhes do produto. por favor, tente atualizar a página ou entre em contato com o administrador do sistema...";
                else
                {
                    jsonRetorno = retornoRequest.objeto.ToString();
                    listaDadosProdutoAdicionalProduto = JsonConvert.DeserializeObject<List<DadosProdutoAdicionalProduto>>(jsonRetorno);

                    //se não existirem produtos adicionais para este produto, insere um produto na lista apenas para exibicao
                    if (listaDadosProdutoAdicionalProduto.Count == 0)
                        listaDadosProdutoAdicionalProduto.Add(new DadosProdutoAdicionalProduto { IdProduto = produto.Id, NomeProduto = produto.Nome });
                }

                #endregion

                return View(listaDadosProdutoAdicionalProduto);
            }
            catch (Exception)
            {
                ViewBag.MensagemDetalhesProduto = "não foi possível exibir detalhes do produto. por favor, tente atualizar a página ou entre em contato com o administrador do sistema...";
                return View();
            }

        }

        #endregion

        #region produto adicional produto

        public ActionResult AdicionarProdutoAdicional(int id)
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

                #region limpa as viewbags de mensagem

                Session["MensagemCarregamentoAdicionarProdutoAdicional"] = null;

                #endregion

                #region busca os produtos adicionais

                List<DadosProdutoAdicional> listaDadosProdutoAdicional = new List<DadosProdutoAdicional>();

                //busca todos os produtos adicionais da loja
                retornoRequest = rest.Get("/ProdutoAdicional/listar/" + usuarioLogado.IdLoja);

                //se não encontrar produtos adicionais
                if (retornoRequest.HttpStatusCode == HttpStatusCode.NoContent)
                {
                    Session["MensagemCarregamentoAdicionarProdutoAdicional"] = "é necessário cadastrar um produto adicional antes de atrela-lo ao produto";
                    return RedirectToAction("Detalhes", "Produto", new { id = id });
                }

                //se ocorrer algum erro
                if (retornoRequest.HttpStatusCode != HttpStatusCode.OK)
                {
                    Session["MensagemCarregamentoAdicionarProdutoAdicional"] = "não foi possível consultar os produtos adicionais. por favor, tente atualizar a página ou entre em contato com o administrador do sistema...";
                    return RedirectToAction("Detalhes", "Produto", new { id = id });
                }

                string json = retornoRequest.objeto.ToString();

                listaDadosProdutoAdicional = JsonConvert.DeserializeObject<List<DadosProdutoAdicional>>(json);

                ViewBag.ProdutosAdicionais = listaDadosProdutoAdicional;

                #endregion

                DadosProdutoAdicionalProduto dadosProdutoAdicionalProduto = new DadosProdutoAdicionalProduto()
                {
                    IdProduto = id,
                    IdLoja = usuarioLogado.IdLoja
                };

                //if (Session["ProdutoAdicionalProdutoCadastro"] != null)
                //    dadosProdutoAdicionalProduto = Session["ProdutoAdicionalProdutoCadastro"] as DadosProdutoAdicionalProduto;

                //Session["ProdutoAdicionalProdutoCadastro"] = null;

                return View(dadosProdutoAdicionalProduto);
            }
            catch (Exception)
            {
                Session["MensagemCarregamentoAdicionarProdutoAdicional"] = "é necessário cadastrar um produto adicional antes de atrela-lo ao produto";
                return RedirectToAction("Detalhes", "Produto", new { id = id });
            }

        }

        public ActionResult AdicionarProdutoAdicionalProduto(DadosProdutoAdicionalProduto produtoAdicionalProduto)
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

                #region limpa as viewbags de mensagem

                ViewBag.MensagemCadProdutoAdicionalProduto = null;

                #endregion

                #region validação dos campos

                //validação dos campos
                if (!ModelState.IsValid)
                    return View("AdicionarProdutoAdicional", produtoAdicionalProduto);

                #endregion

                string urlPost = "/Produto/AdicionarProdutoAdicional";
                produtoAdicionalProduto.IdLoja = usuarioLogado.IdLoja;
                retornoRequest = rest.Post(urlPost, produtoAdicionalProduto);

                if (retornoRequest.HttpStatusCode != HttpStatusCode.Created)
                {
                    ViewBag.MensagemCadProdutoAdicionalProduto = "não foi possível cadastrar o produto adicional para o produto. por favor, tente novamente";
                    return View("AdicionarProdutoAdicional", produtoAdicionalProduto);
                }

                return RedirectToAction("Detalhes", "Produto", new { id = produtoAdicionalProduto.IdProduto });
            }
            catch (Exception)
            {
                ViewBag.MensagemCadProdutoAdicionalProduto = "não foi possível cadastrar o produto adicional para o produto. por favor, tente novamente";
                return View("AdicionarProdutoAdicional", produtoAdicionalProduto);
            }

        }

        public ActionResult EditarProdutoAdicional(int id)
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

                #region limpa as viewbags de mensagem

                Session["MensagemCarregamentoEditarProdutoAdicionalProduto"] = null;

                #endregion

                #region busca os produtos adicionais

                List<DadosProdutoAdicional> listaDadosProdutoAdicional = new List<DadosProdutoAdicional>();

                //busca todos os cardápios da loja
                retornoRequest = rest.Get("/ProdutoAdicional/listar/" + usuarioLogado.IdLoja);

                //se ocorrer algum erro
                if (retornoRequest.HttpStatusCode != HttpStatusCode.OK)
                {
                    Session["MensagemCarregamentoEditarProdutoAdicionalProduto"] = "não foi possível carregar os dados dos produtos adicionais. por favor, tente atualizar a página ou entre em contato com o administrador do sistema...";
                    return RedirectToAction("Detalhes", "Produto", new { id = id });
                }

                string json = retornoRequest.objeto.ToString();

                listaDadosProdutoAdicional = JsonConvert.DeserializeObject<List<DadosProdutoAdicional>>(json);

                listaDadosProdutoAdicional = listaDadosProdutoAdicional.FindAll(p => p.Ativo);

                ViewBag.ProdutosAdicionais = listaDadosProdutoAdicional;

                #endregion

                #region busca o produto adicional do produto

                DadosProdutoAdicionalProduto dadosProdutoAdicionalProduto = new DadosProdutoAdicionalProduto();

                //busca os dados do produto adicional
                retornoRequest = rest.Get(string.Format("/Produto/BuscarProdutoAdicional/{0}/{1}", id, usuarioLogado.IdLoja));

                //se ocorrer algum erro
                if (retornoRequest.HttpStatusCode != HttpStatusCode.OK)
                {
                    Session["MensagemCarregamentoEditarProdutoAdicionalProduto"] = "não foi possível carregar os dados do produto adicional. por favor, tente atualizar a página ou entre em contato com o administrador do sistema...";
                    return RedirectToAction("Detalhes", "Produto", new { id = id });
                }

                string jsonResult = retornoRequest.objeto.ToString();

                dadosProdutoAdicionalProduto = JsonConvert.DeserializeObject<DadosProdutoAdicionalProduto>(jsonResult);

                #endregion

                return View(dadosProdutoAdicionalProduto);
            }
            catch (Exception)
            {
                Session["MensagemCarregamentoEditarProdutoAdicionalProduto"] = "não foi possível carregar os dados do produto adicional. por favor, tente atualizar a página ou entre em contato com o administrador do sistema...";
                return RedirectToAction("Detalhes", "Produto", new { id = id });
            }

        }

        public ActionResult EditarProdutoAdicionalProduto(DadosProdutoAdicionalProduto produtoAdicionalProduto)
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

                #region limpa as viewbags de mensagem

                ViewBag.MensagemEditarProdutoAdicionalProduto = null;

                #endregion

                //variável para armazenar o retorno da requisição
                DadosRequisicaoRest retornoRequest = new DadosRequisicaoRest();

                string urlPost = string.Format("/Produto/ProdutoAdicional/Atualizar");
                produtoAdicionalProduto.IdLoja = usuarioLogado.IdLoja;
                retornoRequest = rest.Post(urlPost, produtoAdicionalProduto);

                //se o produto não for atualizado
                if (retornoRequest.HttpStatusCode != HttpStatusCode.OK)
                {
                    ViewBag.MensagemEditarProdutoAdicionalProduto = "não foi possível atualizar o produto. por favor, tente novamente";
                    return View("EditarProdutoAdicional", produtoAdicionalProduto);
                }

                return RedirectToAction("Detalhes", "Produto", new { id = produtoAdicionalProduto.IdProduto });
            }
            catch (Exception)
            {
                ViewBag.MensagemEditarProdutoAdicionalProduto = "não foi possível atualizar o produto. por favor, tente novamente";
                return View("EditarProdutoAdicional", produtoAdicionalProduto);
            }

        }

        [MyErrorHandler]
        public ActionResult ExcluirProdutoAdicional(int id)
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
                IdLoja = usuarioLogado.IdLoja,
                Ativo = false
            };

            string urlPost = string.Format("/Produto/ExcluirProdutoAdicional");
            retornoRequest = rest.Post(urlPost, dadosProdutoAdicionalProduto);
            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }

        [MyErrorHandler]
        public ActionResult DesativarProdutoAdicional(int id)
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
                IdLoja = usuarioLogado.IdLoja,
                Ativo = false
            };

            string urlPost = string.Format("/Produto/ExcluirProdutoAdicional");
            retornoRequest = rest.Post(urlPost, dadosProdutoAdicionalProduto);
            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}
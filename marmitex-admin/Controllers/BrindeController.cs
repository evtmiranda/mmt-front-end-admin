using ClassesMarmitex;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Web;
using System.Web.Mvc;
using marmitex_admin.Utils;

namespace marmitex_admin.Controllers
{
    public class BrindeController : BaseController
    {
        private RequisicoesREST rest;
        private DadosRequisicaoRest retornoRequest;

        //O Ninject é o responsável por cuidar da criação de todos esses objetos
        public BrindeController(RequisicoesREST rest)
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

                //verifica se o usuário tem permissão na página
                if (usuarioLogado.NivelPermissao == 2)
                    return RedirectToAction("Index", "Pedido");

                #endregion

                #region limpa as viewbags de mensagem

                ViewBag.MensagemBrindes = null;

                #endregion

                List<Brinde> listaBrindes = new List<Brinde>();
                retornoRequest = rest.Get("/Brinde/ListarPorLoja/" + usuarioLogado.IdLoja);

                if (retornoRequest.HttpStatusCode == HttpStatusCode.NoContent)
                {
                    ViewBag.MensagemBrindes = "nenhum brinde encontrado";
                    return View("Index");
                }

                if (retornoRequest.HttpStatusCode != HttpStatusCode.OK)
                {
                    ViewBag.MensagemBrindes = "não foi possível consultar os brindes. por favor, tente atualizar a página ou entre em contato com o administrador do sistema...";
                    return View("Index");
                }

                string jsonBrindes = retornoRequest.objeto.ToString();

                listaBrindes = JsonConvert.DeserializeObject<List<Brinde>>(jsonBrindes);

                //monta a sessão com o caminho das imagens dos brindes
                string caminhoImagem = "http://" + usuarioLogado.UrlLoja + ".tasaindo.com.br/Images/" + usuarioLogado.UrlLoja + "/Brindes/";
                Session["CaminhoImagensBrindes"] = caminhoImagem;

                return View(listaBrindes);
            }
            catch (Exception)
            {
                ViewBag.MensagemBrindes = "não foi possível consultar os brindes. por favor, tente atualizar a página ou entre em contato com o administrador do sistema...";
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

        public ActionResult AdicionarBrinde(Brinde brindeCadastro, HttpPostedFileBase file)
        {
            #region validacao usuario logado

            //se a sessão de usuário não estiver preenchida, direciona para a tela de login
            if (Session["UsuarioLogado"] == null)
                return RedirectToAction("Index", "Login");

            //recebe o usuário logado
            usuarioLogado = (UsuarioLoja)(Session["UsuarioLogado"]);

            #endregion

            DadosRequisicaoRest retornoRequest = new DadosRequisicaoRest();

            #region validações form

            if (file == null)
            {
                ViewBag.MensagemErroCadBrinde = "insira uma imagem para o brinde";
                return View("Adicionar", brindeCadastro);
            }

            #endregion

            #region limpa as viewbags de mensagem

            ViewBag.MensagemErroCadBrinde = null;

            #endregion

            #region validação dos campos

            //validação dos campos
            if (!ModelState.IsValid)
                return View("Adicionar", brindeCadastro);

            #endregion

            try
            {
                //recebe a imagem do brinde
                if (file != null)
                {
                    //valida o tamanho da imagem
                    //tamanho maximo permitido é 10 mb
                    if (file.ContentLength > 10000000)
                    {
                        ViewBag.MensagemErroCadBrinde = "a imagem deve ter no máximo 10 megabytes";
                        return View("Adicionar", brindeCadastro);
                    }

                    string pic = System.IO.Path.GetFileName(file.FileName);
                    string caminhoPasta = ConfigurationManager.AppSettings["PastaImagens"] + usuarioLogado.UrlLoja + "/Brindes/";

                    string path = caminhoPasta + pic;

                    //se o diretório ainda não existir, cria um novo
                    if (!Directory.Exists(caminhoPasta))
                        Directory.CreateDirectory(caminhoPasta);

                    //salva a imagem na pasta
                    file.SaveAs(path);

                    //seta o nome e caminho da imagem no brinde
                    brindeCadastro.Imagem = pic;
                }

                Brinde brinde = new Brinde
                {
                    IdLoja = usuarioLogado.IdLoja,
                    Nome = brindeCadastro.Nome,
                    Descricao = brindeCadastro.Descricao,
                    Imagem = brindeCadastro.Imagem,
                    Ativo = brindeCadastro.Ativo
                };

                string urlPost = "/Brinde/Adicionar";

                retornoRequest = rest.Post(urlPost, brinde);

                if (retornoRequest.HttpStatusCode == HttpStatusCode.Created)
                    return RedirectToAction("Index", "Brinde");
                else
                {
                    ViewBag.MensagemErroCadBrinde = "não foi possível cadastrar o brinde. por favor, tente novamente";
                    return View("Adicionar", brindeCadastro);
                }
            }
            catch (Exception)
            {
                ViewBag.MensagemErroCadBrinde = "não foi possível cadastrar o brinde. por favor, tente novamente";
                return View("Adicionar", brindeCadastro);
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

                ViewBag.MensagemCarregamentoEditarBrinde = null;
                
                #endregion

                Brinde brinde = new Brinde();

                retornoRequest = rest.Get(string.Format("/Brinde/{0}/{1}", id, usuarioLogado.IdLoja));

                if (retornoRequest.HttpStatusCode != HttpStatusCode.OK)
                {
                    ViewBag.MensagemCarregamentoEditarBrinde = "não foi possível carregar os dados do brinde. por favor, tente atualizar a página ou entre em contato com o administrador do sistema...";
                    return View();
                }

                string jsonBrinde = retornoRequest.objeto.ToString();

                brinde = JsonConvert.DeserializeObject<Brinde>(jsonBrinde);

                return View(brinde);
            }
            catch (Exception)
            {
                ViewBag.MensagemCarregamentoEditarBrinde = "não foi possível carregar os dados do brinde. por favor, tente atualizar a página ou entre em contato com o administrador do sistema...";
                return View();
            }
        }

        public ActionResult EditarBrinde(Brinde brindeCadastro, HttpPostedFileBase file)
        {
            #region validacao usuario logado

            //se a sessão de usuário não estiver preenchida, direciona para a tela de login
            if (Session["UsuarioLogado"] == null)
                return RedirectToAction("Index", "Login");

            //recebe o usuário logado
            usuarioLogado = (UsuarioLoja)(Session["UsuarioLogado"]);

            #endregion

            #region validações form

            if (file == null)
            {
                ViewBag.MensagemEditarBrinde = "insira uma imagem para o brinde";
                return View("Editar", brindeCadastro);
            }

            #endregion

            #region validação dos campos

            //validação dos campos
            if (!ModelState.IsValid)
                return View("Editar", brindeCadastro);

            #endregion

            #region limpa as viewbags de mensagem

            ViewBag.MensagemEditarBrinde = null;

            #endregion

            //variável para armazenar o retorno da requisição
            DadosRequisicaoRest retornoRequest = new DadosRequisicaoRest();

            try
            {
                //recebe a imagem do brinde
                if (file != null)
                {
                    //valida o tamanho da imagem
                    //tamanho maximo permitido é 10 mb
                    if (file.ContentLength > 10000000)
                    {
                        ViewBag.MensagemEditarBrinde = "a imagem deve ter no máximo 10 megabytes";
                        return View("Editar", brindeCadastro);
                    }

                    string pic = System.IO.Path.GetFileName(file.FileName);
                    string caminhoPasta = ConfigurationManager.AppSettings["PastaImagens"] + usuarioLogado.UrlLoja + "/Brindes/";

                    string path = caminhoPasta + pic;

                    //se o diretório ainda não existir, cria um novo
                    if (!Directory.Exists(caminhoPasta))
                        Directory.CreateDirectory(caminhoPasta);

                    //salva a imagem na pasta
                    file.SaveAs(path);

                    //seta o nome e caminho da imagem no brinde
                    brindeCadastro.Imagem = pic;
                }

                Brinde brinde = new Brinde
                {
                    Id = brindeCadastro.Id,
                    Nome = brindeCadastro.Nome,
                    IdLoja = usuarioLogado.IdLoja,
                    Descricao = brindeCadastro.Descricao,
                    Imagem = brindeCadastro.Imagem,
                    Ativo = brindeCadastro.Ativo
                };

                string urlPost = string.Format("/Brinde/Atualizar");

                retornoRequest = rest.Post(urlPost, brinde);

                if (retornoRequest.HttpStatusCode != HttpStatusCode.OK)
                {
                    ViewBag.MensagemEditarBrinde = "não foi possível atualizar o brinde. por favor, tente novamente";
                    return View("Editar", brindeCadastro);
                }

                return RedirectToAction("Index", "Brinde");
            }
            catch (Exception)
            {
                ViewBag.MensagemEditarBrinde = "não foi possível atualizar o brinde. por favor, tente novamente";
                return View("Editar", brindeCadastro);
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

            Brinde brinde = new Brinde
            {
                Id = id,
                IdLoja = usuarioLogado.IdLoja,
                Ativo = false
            };

            string urlPost = string.Format("/Brinde/Excluir");

            retornoRequest = rest.Post(urlPost, brinde);

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

            Brinde brinde = new Brinde
            {
                Id = id,
                IdLoja = usuarioLogado.IdLoja
            };

            string urlPost = string.Format("/Brinde/Desativar");

            retornoRequest = rest.Post(urlPost, brinde);

            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }

    }
}
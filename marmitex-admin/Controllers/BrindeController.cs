using ClassesMarmitex;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Web;
using System.Web.Mvc;

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
            List<Brinde> listaBrindes = new List<Brinde>();

            #region validacao usuario logado

            //se a sessão de usuário não estiver preenchida, direciona para a tela de login
            if (Session["UsuarioLogado"] == null)
                return RedirectToAction("Index", "Login");

            //recebe o usuário logado
            usuarioLogado = (UsuarioLoja)(Session["UsuarioLogado"]);

            #endregion

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
            string caminhoImagem = "http://" + usuarioLogado.UrlLoja + ":45237/Images/" + usuarioLogado.UrlLoja + "/Brindes/";
            Session["CaminhoImagensBrindes"] = caminhoImagem;

            return View(listaBrindes);
        }

        #region brindes

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

            try
            {
                //recebe a imagem do brinde
                if (file != null)
                {
                    string pic = System.IO.Path.GetFileName(file.FileName);
                    string path = ConfigurationManager.AppSettings["PastaImagens"] + usuarioLogado.UrlLoja + "/" + pic;

                    string caminhoPasta = ConfigurationManager.AppSettings["PastaImagens"] + usuarioLogado.UrlLoja + "/";

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
            #region validacao usuario logado

            //se a sessão de usuário não estiver preenchida, direciona para a tela de login
            if (Session["UsuarioLogado"] == null)
                return RedirectToAction("Index", "Login");

            //recebe o usuário logado
            usuarioLogado = (UsuarioLoja)(Session["UsuarioLogado"]);

            #endregion

            Brinde brinde = new Brinde();

            retornoRequest = rest.Get(string.Format("/Brinde/{0}/{1}", id, usuarioLogado.IdLoja));

            if (retornoRequest.HttpStatusCode != HttpStatusCode.OK)
            {
                ViewBag.MensagemEditarParceiro = "não foi possível carregar os dados do brinde. por favor, tente atualizar a página ou entre em contato com o administrador do sistema...";
                return View();
            }

            string jsonBrinde = retornoRequest.objeto.ToString();

            brinde = JsonConvert.DeserializeObject<Brinde>(jsonBrinde);

            return View(brinde);
        }

        public ActionResult EditarBrinde(Brinde brindeCadastro)
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

                Brinde brinde = new Brinde
                {
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

                Brinde brinde = new Brinde
                {
                    Id = id,
                    IdLoja = usuarioLogado.IdLoja,
                    Ativo = false
                };

                string urlPost = string.Format("/Brinde/Excluir");

                retornoRequest = rest.Post(urlPost, brinde);

                if (retornoRequest.HttpStatusCode != HttpStatusCode.OK)
                {
                    ViewBag.MensagemExcluirBrinde = "não foi possível excluir o brinde. por favor, tente novamente";
                    return View("Index");
                }

                return RedirectToAction("Index", "Brinde");
            }
            catch (Exception)
            {
                ViewBag.MensagemExcluirBrinde = "não foi possível excluir o brinde. por favor, tente novamente";
                return View("Index");
            }
        }

        public ActionResult Desativar(int id)
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

                Brinde brinde = new Brinde
                {
                    Id = id,
                    IdLoja = usuarioLogado.IdLoja
                };

                string urlPost = string.Format("/Brinde/Desativar");

                retornoRequest = rest.Post(urlPost, brinde);

                if (retornoRequest.HttpStatusCode != HttpStatusCode.OK)
                {
                    ViewBag.MensagemDesativarBrinde = "não foi possível desativar o brinde. por favor, tente novamente";
                    return View("Index");
                }

                return RedirectToAction("Index", "Brinde");
            }
            catch (Exception)
            {
                ViewBag.MensagemDesativarBrinde = "não foi possível desativar o brinde. por favor, tente novamente";
                return View("Index");
            }
        }

        #endregion
    }
}
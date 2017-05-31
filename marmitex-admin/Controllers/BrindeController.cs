using ClassesMarmitex;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
                ViewBag.MensagemParceiros = "nenhum brinde encontrado";
                return View("Index");
            }

            if (retornoRequest.HttpStatusCode != HttpStatusCode.OK)
            {
                ViewBag.MensagemParceiros = "não foi possível consultar os brindes. por favor, tente atualizar a página ou entre em contato com o administrador do sistema...";
                return View("Index");
            }

            string jsonBrindes = retornoRequest.objeto.ToString();

            listaBrindes = JsonConvert.DeserializeObject<List<Brinde>>(jsonBrindes);

            return View(listaBrindes);
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

        public ActionResult AdicionarBrinde(Brinde brindeCadastro)
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
                Brinde brinde = new Brinde
                {
                    IdLoja = usuarioLogado.IdLoja,
                    IdParceiro = brindeCadastro.IdParceiro,
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
                    ViewBag.MensagemBrinde = "não foi possível cadastrar o brinde. por favor, tente novamente";
                    return View("Adicionar", brindeCadastro);
                }
            }
            catch (Exception)
            {
                ViewBag.MensagemBrinde = "não foi possível cadastrar o brinde. por favor, tente novamente";
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

        [HttpPost]
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
    }
}
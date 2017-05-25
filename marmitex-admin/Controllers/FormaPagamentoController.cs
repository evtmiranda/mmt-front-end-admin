using ClassesMarmitex;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Mvc;

namespace marmitex_admin.Controllers
{
    public class FormaPagamentoController : BaseController
    {
        private RequisicoesREST rest;
        private DadosRequisicaoRest retornoRequest;


        //O Ninject é o responsável por cuidar da criação de todos esses objetos
        public FormaPagamentoController(RequisicoesREST rest)
        {
            this.rest = rest;
        }

        // GET: FormasPagamento
        public ActionResult Index()
        {
            #region validacao usuario logado

            //se a sessão de usuário não estiver preenchida, direciona para a tela de login
            if (Session["UsuarioLogado"] == null)
                return RedirectToAction("Index", "Login");

            //recebe o usuário logado
            usuarioLogado = (UsuarioLoja)(Session["UsuarioLogado"]);

            #endregion

            List<FormaDePagamento> listaFormasPagamento = new List<FormaDePagamento>();

            //busca as formas de pagamento da loja
            retornoRequest = rest.Get("/FormaPagamento/Listar/" + usuarioLogado.IdLoja);

            //se não encontrar pedidos para este cliente
            if (retornoRequest.HttpStatusCode == HttpStatusCode.NotFound)
            {
                ViewBag.MensagemFormaPagamento = "nenhuma forma de pagamento encontrada";
                return View("Index");
            }

            //se ocorrer algum erro
            if (retornoRequest.HttpStatusCode != HttpStatusCode.OK)
            {
                ViewBag.MensagemFormaPagamento = "não foi possível consultar as formas de pagamento. por favor, tente atualizar a página ou entre em contato com o administrador do sistema...";
                return View("Index");
            }

            string jsonRetorno = retornoRequest.objeto.ToString();

            listaFormasPagamento = JsonConvert.DeserializeObject<List<FormaDePagamento>>(jsonRetorno);

            return View(listaFormasPagamento);
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

        public ActionResult AdicionarPagamento(FormaDePagamento pagamento)
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
                string urlPost = "/FormaPagamento/Adicionar";

                pagamento.IdLoja = usuarioLogado.IdLoja;

                retornoRequest = rest.Post(urlPost, pagamento);

                //se não for cadastrado
                if (retornoRequest.HttpStatusCode != HttpStatusCode.Created)
                {
                    ViewBag.MensagemFormaPagamento = "não foi possível cadastrar. por favor, tente novamente";
                    return View("Adicionar", pagamento);
                }

                return RedirectToAction("Index", "FormaPagamento");
            }
            catch (Exception)
            {
                ViewBag.MensagemFormaPagamento = "não foi possível cadastrar. por favor, tente novamente";
                return View("Adicionar", pagamento);
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

            FormaDePagamento pagamento = new FormaDePagamento();

            retornoRequest = rest.Get(string.Format("/FormaPagamento/{0}/{1}", id, usuarioLogado.IdLoja));

            //se não encontrar com este id
            if (retornoRequest.HttpStatusCode == HttpStatusCode.NoContent)
            {
                ViewBag.MensagemEditarParceiro = "não foi possível carregar os dados. por favor, tente atualizar a página ou entre em contato com o administrador do sistema...";
                return View();
            }

            //se ocorrer algum erro
            if (retornoRequest.HttpStatusCode != HttpStatusCode.OK)
            {
                ViewBag.MensagemEditarParceiro = "não foi possível carregar os dados. por favor, tente atualizar a página ou entre em contato com o administrador do sistema...";
                return View();
            }

            string jsonRetorno = retornoRequest.objeto.ToString();

            pagamento = JsonConvert.DeserializeObject<FormaDePagamento>(jsonRetorno);

            return View(pagamento);
        }

        [HttpPost]
        public ActionResult EditarPagamento(FormaDePagamento pagamento)
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
                pagamento.IdLoja = usuarioLogado.IdLoja;

                string urlPost = string.Format("/FormaPagamento/Atualizar");

                retornoRequest = rest.Post(urlPost, pagamento);

                //se não for atualizado
                if (retornoRequest.HttpStatusCode != HttpStatusCode.OK)
                {
                    ViewBag.MensagemEditarFormaPagamento = "não foi possível atualizar. por favor, tente novamente";
                    return View("Editar", pagamento);
                }

                //se for atualizado, direciona para a tela de visualização
                return RedirectToAction("Index", "FormaPagamento");
            }
            catch (Exception)
            {
                ViewBag.MensagemEditarFormaPagamento = "não foi possível atualizar. por favor, tente novamente";
                return View("Editar", pagamento);
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

                //busca os dados do parceiro
                FormaDePagamento pagamento = new FormaDePagamento
                {
                    Id = id,
                    IdLoja = usuarioLogado.IdLoja,
                    Ativo = false
                };

                //inativa a forma de pagamento
                string urlPost = string.Format("/FormaPagamento/Excluir");

                retornoRequest = rest.Post(urlPost, pagamento);

                //se a forma de pagamento não for atualizada
                if (retornoRequest.HttpStatusCode != HttpStatusCode.OK)
                {
                    ViewBag.MensagemExcluirFormaPagamento = "não foi possível excluir a forma de pagamento. por favor, tente novamente";
                    return View("Index");
                }

                //se a forma de pagamento for inativada, direciona para a tela de visualização de formas de pagamento
                return RedirectToAction("Index", "FormaPagamento");
            }
            catch (Exception)
            {
                ViewBag.MensagemExcluirFormaPagamento = "não foi possível excluir a forma de pagamento. por favor, tente novamente";
                return View("Index");
            }
        }
    }
}
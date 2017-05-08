using ClassesMarmitex;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace marmitex_admin.Controllers
{
    public class ParceiroController : BaseController
    {
        private RequisicoesREST rest;
        private DadosRequisicaoRest retornoRequest;
        private List<Parceiro> listaParceiros;

        //O Ninject é o responsável por cuidar da criação de todos esses objetos
        public ParceiroController(RequisicoesREST rest)
        {
            this.rest = rest;
            this.listaParceiros = new List<Parceiro>();
        }

        // GET: Parceiro
        public ActionResult Index()
        {
            //se a sessão de usuário não estiver preenchida, direciona para a tela de login
            if (Session["UsuarioLogado"] == null)
            {
                Session["MensagemAutenticacao"] = "estamos com dificuldade em buscar dados no servidor. por favor, tente novamente";
                return RedirectToAction("Index", "Login");
            }

            //recebe o usuário logado
            usuarioLogado = (UsuarioLoja)(Session["UsuarioLogado"]);
            usuarioLogado.UrlLoja = BuscarUrlLoja();

            //busca todos os parceiros da loja
            retornoRequest = rest.Get("/Parceiro/BuscarParceiroPorLoja/" + usuarioLogado.IdLoja);

            //se não encontrar pedidos para este cliente
            if (retornoRequest.HttpStatusCode == HttpStatusCode.NoContent)
            {
                ViewBag.MensagemParceiros = "nenhum parceiro encontrado";
                return View("Index");
            }

            //se ocorrer algum erro
            if (retornoRequest.HttpStatusCode != HttpStatusCode.OK)
            {
                ViewBag.MensagemParceiros = "não foi possível consultar os parceiros. por favor, tente atualizar a página ou entre em contato com o administrador do sistema...";
                return View("Index");
            }

            string jsonParceiros = retornoRequest.objeto.ToString();

            listaParceiros = JsonConvert.DeserializeObject<List<Parceiro>>(jsonParceiros);

            return View(listaParceiros);
        }

        public ActionResult Adicionar()
        {
            //se a sessão de usuário não estiver preenchida, direciona para a tela de login
            if (Session["UsuarioLogado"] == null)
            {
                Session["MensagemAutenticacao"] = "estamos com dificuldade em buscar dados no servidor. por favor, tente novamente";
                return RedirectToAction("Index", "Login");
            }

            //recebe o usuário logado
            usuarioLogado = (UsuarioLoja)(Session["UsuarioLogado"]);
            usuarioLogado.UrlLoja = BuscarUrlLoja();

            return View();
        }

        [HttpPost]
        public ActionResult AdicionarParceiro(ParceiroCadastro parceiroCadastro)
        {
            //captura a loja em questão
            Session["dominioLoja"] = BuscarUrlLoja();

            //se não conseguir capturar a loja, direciona para a tela de erro
            if (Session["dominioLoja"] == null)
            {
                Session["MensagemAutenticacao"] = "estamos com dificuldade em buscar dados no servidor. por favor, tente atualizar a página";
                return View("Index");
            }

            string dominioLoja = Session["dominioLoja"].ToString();

            DadosRequisicaoRest retornoRequest = new DadosRequisicaoRest();
           

            try
            {
                Parceiro parceiro = new Parceiro()
                {

                    //cria um parceiro com os dados vindos da tela
                    Nome = parceiroCadastro.Nome,
                    Descricao = parceiroCadastro.Descricao,
                    Codigo = Guid.NewGuid().ToString().Substring(0, 5).ToUpper(),
                    Endereco = new Endereco
                    {
                        Cep = parceiroCadastro.Cep,
                        UF = parceiroCadastro.UF,
                        Cidade = parceiroCadastro.Cidade,
                        Bairro = parceiroCadastro.Bairro,
                        Logradouro = parceiroCadastro.Logradouro,
                        NumeroEndereco = parceiroCadastro.NumeroEndereco,
                        ComplementoEndereco = parceiroCadastro.ComplementoEndereco
                    }
                };

                string urlPost = string.Format("/Parceiro/Adicionar/{0}", dominioLoja);

                retornoRequest = rest.Post(urlPost, parceiro);

                //se o parceiro for cadastrado, direciona para a tela de visualização de parceiros
                if (retornoRequest.HttpStatusCode == HttpStatusCode.Created)
                    return RedirectToAction("Index", "Parceiro");

                //se já existir um parceiro com este nome
                if (retornoRequest.HttpStatusCode == HttpStatusCode.Unauthorized)
                {
                    ViewBag.MensagemParceiro = "já existe um parceiro com este nome.";
                    return View("Adicionar", parceiroCadastro);
                }
                //se for algum outro erro
                else
                {
                    ViewBag.MensagemParceiro = "não foi possível cadastrar o parceiro. por favor, tente novamente";
                    return View("Adicionar", parceiroCadastro);
                }
            }
            catch (Exception)
            {
                ViewBag.MensagemParceiro = "não foi possível cadastrar o parceiro. por favor, tente novamente";
                return View("Adicionar", parceiroCadastro);
            }
        }
    }
}
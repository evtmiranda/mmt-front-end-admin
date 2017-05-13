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


        //O Ninject é o responsável por cuidar da criação de todos esses objetos
        public ParceiroController(RequisicoesREST rest)
        {
            this.rest = rest;
        }

        // GET: Parceiro
        public ActionResult Index()
        {
            List<Parceiro> listaParceiros = new List<Parceiro>();

            #region validacao usuario logado

            //se a sessão de usuário não estiver preenchida, direciona para a tela de login
            if (Session["UsuarioLogado"] == null)
                return RedirectToAction("Index", "Login");

            //recebe o usuário logado
            usuarioLogado = (UsuarioLoja)(Session["UsuarioLogado"]);

            #endregion

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

            //mantém somente os parceiros ativos
            listaParceiros = listaParceiros.Where(p => p.Ativo).ToList();

            return View(listaParceiros);
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

        public ActionResult AdicionarParceiro(ParceiroCadastro parceiroCadastro)
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

                string urlPost = string.Format("/Parceiro/Adicionar/{0}", usuarioLogado.IdLoja);

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

        public ActionResult Editar(int id)
        {
            #region validacao usuario logado

            //se a sessão de usuário não estiver preenchida, direciona para a tela de login
            if (Session["UsuarioLogado"] == null)
                return RedirectToAction("Index", "Login");

            //recebe o usuário logado
            usuarioLogado = (UsuarioLoja)(Session["UsuarioLogado"]);

            #endregion

            //busca os dados do parceiro
            Parceiro parceiro = new Parceiro();

            //busca o parceiro pelo id
            retornoRequest = rest.Get("/Parceiro/BuscarParceiro/" + id);

            //se não encontrar um parceiro com este id
            if (retornoRequest.HttpStatusCode == HttpStatusCode.NoContent)
            {
                ViewBag.MensagemEditarParceiro = "não foi possível carregar os dados do parceiro. por favor, tente atualizar a página ou entre em contato com o administrador do sistema...";
                return View();
            }

            //se ocorrer algum erro
            if (retornoRequest.HttpStatusCode != HttpStatusCode.OK)
            {
                ViewBag.MensagemEditarParceiro = "não foi possível carregar os dados do parceiro. por favor, tente atualizar a página ou entre em contato com o administrador do sistema...";
                return View();
            }

            string jsonParceiro = retornoRequest.objeto.ToString();

            parceiro = JsonConvert.DeserializeObject<Parceiro>(jsonParceiro);

            ParceiroCadastro parceiroCadastro = new ParceiroCadastro
            {
                Id = parceiro.Id,
                Nome = parceiro.Nome,
                Descricao = parceiro.Descricao,
                Ativo = parceiro.Ativo,
                IdEndereco = parceiro.Endereco.Id,
                Cep = parceiro.Endereco.Cep,
                UF = parceiro.Endereco.UF,
                Cidade = parceiro.Endereco.Cidade,
                Bairro = parceiro.Endereco.Bairro,
                Logradouro = parceiro.Endereco.Logradouro,
                NumeroEndereco = parceiro.Endereco.NumeroEndereco,
                ComplementoEndereco = parceiro.Endereco.ComplementoEndereco
            };

            return View(parceiroCadastro);
        }

        [HttpPost]
        public ActionResult EditarParceiro(ParceiroCadastro parceiroCadastro)
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

                Parceiro parceiro = new Parceiro()
                {

                    //cria um parceiro com os dados vindos da tela
                    Id = parceiroCadastro.Id,
                    Nome = parceiroCadastro.Nome,
                    Descricao = parceiroCadastro.Descricao,
                    Ativo = parceiroCadastro.Ativo,
                    Endereco = new Endereco
                    {
                        Id = parceiroCadastro.IdEndereco,
                        Cep = parceiroCadastro.Cep,
                        UF = parceiroCadastro.UF,
                        Cidade = parceiroCadastro.Cidade,
                        Bairro = parceiroCadastro.Bairro,
                        Logradouro = parceiroCadastro.Logradouro,
                        NumeroEndereco = parceiroCadastro.NumeroEndereco,
                        ComplementoEndereco = parceiroCadastro.ComplementoEndereco
                    }
                };

                string urlPost = string.Format("/Parceiro/Atualizar");

                retornoRequest = rest.Post(urlPost, parceiro);

                //se o parceiro não for atualizado
                if (retornoRequest.HttpStatusCode != HttpStatusCode.OK)
                {
                    ViewBag.MensagemEditarParceiro = "não foi possível atualizar o parceiro. por favor, tente novamente";
                    return View("Editar", parceiroCadastro);
                }

                //se o parceiro for atualizado, direciona para a tela de visualização de parceiros
                return RedirectToAction("Index", "Parceiro");
            }
            catch (Exception)
            {
                ViewBag.MensagemEditarParceiro = "não foi possível atualizar o parceiro. por favor, tente novamente";
                return View("Editar", parceiroCadastro);
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
                Parceiro parceiro = new Parceiro
                {
                    Id = id,
                    Ativo = false
                };

                //inativa o parceiro
                string urlPost = string.Format("/Parceiro/Excluir");

                retornoRequest = rest.Post(urlPost, parceiro);

                //se o parceiro não for atualizado
                if (retornoRequest.HttpStatusCode != HttpStatusCode.OK)
                {
                    ViewBag.MensagemExcluirParceiro = "não foi possível excluir o parceiro. por favor, tente novamente";
                    return View("Index");
                }

                //se o parceiro for inativado, direciona para a tela de visualização de parceiros
                return RedirectToAction("Index", "Parceiro");
            }
            catch (Exception)
            {
                ViewBag.MensagemExcluirParceiro = "não foi possível excluir o parceiro. por favor, tente novamente";
                return View("Index");
            }
        }
    }
}
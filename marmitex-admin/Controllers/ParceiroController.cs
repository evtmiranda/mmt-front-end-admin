using ClassesMarmitex;
using marmitex_admin.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Web.Mvc;
using System.Linq;

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

                List<Parceiro> listaParceiros = new List<Parceiro>();

                //busca todos os parceiros da loja
                retornoRequest = rest.Get("/Parceiro/BuscarParceiroPorLoja/" + usuarioLogado.IdLoja);

                //se não encontrar pedidos para este cliente
                if (retornoRequest.HttpStatusCode == HttpStatusCode.NoContent)
                {
                    ViewBag.MensagemParceiros = "nenhum parceiro encontrado";
                    return View();
                }

                //se ocorrer algum erro
                if (retornoRequest.HttpStatusCode != HttpStatusCode.OK)
                {
                    ViewBag.MensagemParceiros = "não foi possível consultar os parceiros. por favor, tente novamente ou entre em contato com o administrador do sistema.";
                    return View();
                }

                string jsonParceiros = retornoRequest.objeto.ToString();

                listaParceiros = JsonConvert.DeserializeObject<List<Parceiro>>(jsonParceiros);

                return View(listaParceiros);
            }
            catch (Exception)
            {
                ViewBag.MensagemParceiros = "não foi possível consultar os parceiros. por favor, tente novamente ou entre em contato com o administrador do sistema.";
                return View();
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
                    IdLoja = usuarioLogado.IdLoja,
                    TaxaEntrega = parceiroCadastro.TaxaEntrega,
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
                    ViewBag.MensagemParceiro = "não foi possível cadastrar o parceiro. por favor, tente novamente ou entre em contato com o administrador do sistema.";
                    return View("Adicionar", parceiroCadastro);
                }
            }
            catch (Exception)
            {
                ViewBag.MensagemParceiro = "não foi possível cadastrar o parceiro. por favor, tente novamente ou entre em contato com o administrador do sistema.";
                return View("Adicionar", parceiroCadastro);
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

                //busca os dados do parceiro
                Parceiro parceiro = new Parceiro();

                //busca o parceiro pelo id
                retornoRequest = rest.Get(string.Format("/Parceiro/BuscarParceiro/{0}/{1}", id, usuarioLogado.IdLoja));

                //se ocorrer algum erro
                if (retornoRequest.HttpStatusCode != HttpStatusCode.OK)
                {
                    ViewBag.MensagemCarregamentoEditarParceiro = "não foi possível carregar os dados do parceiro. por favor, tente atualizar a página ou entre em contato com o administrador do sistema...";
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
                    ComplementoEndereco = parceiro.Endereco.ComplementoEndereco,
                    TaxaEntrega = parceiro.TaxaEntrega
                };

                return View(parceiroCadastro);
            }
            catch (Exception)
            {
                ViewBag.MensagemCarregamentoEditarParceiro = "não foi possível carregar os dados do parceiro. por favor, tente atualizar a página ou entre em contato com o administrador do sistema...";
                return View();
            }

        }

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
                    IdLoja = usuarioLogado.IdLoja,
                    TaxaEntrega = parceiroCadastro.TaxaEntrega,
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

        /// <summary>
        /// Faz o post para excluir o parceiro. Se ocorrer algum erro a classe MyErrorHandler irá transformar em json e o método 'Excluir'
        /// do arquivo marmitex-admin.js irá exibir uma mensagem amigável. Em caso de sucesso, o método 'Excluir' também irá 
        /// exibir uma mensagem amigável
        /// </summary>
        /// <param name="id">id do parceiro que deve ser excluido</param>
        /// <returns></returns>
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

            //busca os dados do parceiro
            Parceiro parceiro = new Parceiro
            {
                Id = id,
                IdLoja = usuarioLogado.IdLoja,
                Ativo = false
            };

            string urlPost = string.Format("/Parceiro/Excluir");
            retornoRequest = rest.Post(urlPost, parceiro);
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

            //busca os dados do parceiro
            Parceiro parceiro = new Parceiro
            {
                Id = id,
                IdLoja = usuarioLogado.IdLoja,
                Ativo = false
            };

            string urlPost = string.Format("/Parceiro/Desativar");
            retornoRequest = rest.Post(urlPost, parceiro);
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

                DadosBrindeParceiro dadosBrindeParceiro = new DadosBrindeParceiro();

                //busca todos os brindes do parceiro
                retornoRequest = rest.Get(string.Format("/BrindeParceiro/ListarPorParceiro/{0}/{1}", id, usuarioLogado.IdLoja));

                if (retornoRequest.HttpStatusCode == HttpStatusCode.NoContent)
                {
                    //ViewBag.MensagemDetalhesParceiro = "nenhum brinde cadastrado";
                }
                else if (retornoRequest.HttpStatusCode != HttpStatusCode.OK)
                {
                    ViewBag.MensagemDetalhesParceiro = "não foi possível exibir detalhes do parceiro. por favor, tente atualizar a página ou entre em contato com o administrador do sistema...";
                    return View();
                }

                string jsonRetorno = retornoRequest.objeto.ToString();
                dadosBrindeParceiro = JsonConvert.DeserializeObject<DadosBrindeParceiro>(jsonRetorno);

                Session["IdParceiro"] = id;

                return View(dadosBrindeParceiro);
            }
            catch (Exception)
            {
                ViewBag.MensagemDetalhesParceiro = "não foi possível exibir detalhes do parceiro. por favor, tente atualizar a página ou entre em contato com o administrador do sistema...";
                return View();
            }
        }

        #region brindes parceiros

        public ActionResult AdicionarBrinde(int id)
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

                #region busca os brindes

                List<Brinde> listaBrindes = new List<Brinde>();
                DadosBrindeParceiro dadosBrindeParceiro = new DadosBrindeParceiro();
                BrindeParceiro brindeParceiro = new BrindeParceiro();

                //busca todos os produtos brindes da loja
                retornoRequest = rest.Get(string.Format("/Brinde/ListarPorLoja/{0}", usuarioLogado.IdLoja));

                //se não encontrar produtos adicionais
                if (retornoRequest.HttpStatusCode == HttpStatusCode.NoContent)
                {
                    Session["MensagemCarregamentoAdicionarBrinde"] = "é necessário cadastrar um produto adicional antes de atrela-lo ao produto";
                    return RedirectToAction("Detalhes", "Parceiro", new { id = id });
                }

                if (retornoRequest.HttpStatusCode != HttpStatusCode.OK)
                {
                    Session["MensagemCarregamentoAdicionarBrinde"] = "não foi possível carregar os brindes. por favor, tente novamente ou entre em contato com o administrador do sistema.";
                    return RedirectToAction("Detalhes", "Parceiro", new { id = id });
                }

                string json = retornoRequest.objeto.ToString();

                listaBrindes = JsonConvert.DeserializeObject<List<Brinde>>(json);

                dadosBrindeParceiro.Brindes = listaBrindes.Where(p => p.Ativo).ToList();
                dadosBrindeParceiro.IdParceiro = id;

                #endregion

                return View(dadosBrindeParceiro);
            }
            catch (Exception)
            {
                Session["MensagemCarregamentoAdicionarBrinde"] = "não foi possível carregar os brindes. por favor, tente novamente ou entre em contato com o administrador do sistema.";
                return RedirectToAction("Detalhes", "Parceiro", new { id = id });
            }

        }

        public ActionResult AdicionarBrindeParceiro(DadosBrindeParceiro dadosBrindeParceiro)
        {
            BrindeParceiro brindeParceiro = new BrindeParceiro();

            try
            {
                #region validacao usuario logado

                //se a sessão de usuário não estiver preenchida, direciona para a tela de login
                if (Session["UsuarioLogado"] == null)
                    return RedirectToAction("Index", "Login");

                //recebe o usuário logado
                usuarioLogado = (UsuarioLoja)(Session["UsuarioLogado"]);

                #endregion

                #region cadastra o brinde para o parceiro

                if (dadosBrindeParceiro.IdBrinde == 0)
                    return null;

                brindeParceiro.IdBrinde = dadosBrindeParceiro.IdBrinde;
                brindeParceiro.IdLoja = usuarioLogado.IdLoja;
                brindeParceiro.IdParceiro = dadosBrindeParceiro.IdParceiro;
                brindeParceiro.Ativo = dadosBrindeParceiro.Ativo;

                string urlPost = "/BrindeParceiro/Adicionar";
                retornoRequest = rest.Post(urlPost, brindeParceiro);

                //se o brinde for cadastrado, direciona para a tela de visualização de brindes do parceiro
                if (retornoRequest.HttpStatusCode == HttpStatusCode.Created)
                    return RedirectToAction("Detalhes", "Parceiro", new { id = brindeParceiro.IdParceiro });
                
                ViewBag.MensagemErroCadBrindeParceiro = "não foi possível cadastrar o brinde. por favor, tente novamente";
                return View("AdicionarBrinde", brindeParceiro);

                #endregion
            }
            catch (Exception)
            {
                ViewBag.MensagemErroCadBrindeParceiro = "não foi possível cadastrar o brinde. por favor, tente novamente";
                return View("AdicionarBrinde", brindeParceiro);
            }

        }

        /// <summary>
        /// Faz o post para excluir o brinde do parceiro. Se ocorrer algum erro a classe MyErrorHandler irá 
        /// transformar em json e o método 'Excluir' do arquivo marmitex-admin.js irá exibir uma mensagem amigável. 
        /// Em caso de sucesso, o método 'Excluir' também irá exibir uma mensagem amigável
        /// </summary>
        /// <param name="id">id do brinde que deve ser excluido</param>
        /// <returns></returns>
        [MyErrorHandler]
        public ActionResult ExcluirBrindeParceiro(int id)
        {
            #region validacao usuario logado

            //se a sessão de usuário não estiver preenchida, direciona para a tela de login
            if (Session["UsuarioLogado"] == null)
                return RedirectToAction("Index", "Login");

            //recebe o usuário logado
            usuarioLogado = (UsuarioLoja)(Session["UsuarioLogado"]);

            #endregion

            //busca o id do parceiro
            Int32.TryParse(Session["IdParceiro"].ToString(), out int idParceiro);

            //busca os dados do brinde
            BrindeParceiro brindeParceiro = new BrindeParceiro
            {
                Id = id,
                IdLoja = usuarioLogado.IdLoja,
                IdParceiro = idParceiro
            };

            //inativa o parceiro
            string urlPost = string.Format("/BrindeParceiro/Excluir");

            retornoRequest = rest.Post(urlPost, brindeParceiro);

            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }

        [MyErrorHandler]
        public ActionResult DesativarBrindeParceiro(int id)
        {
            #region validacao usuario logado

            //se a sessão de usuário não estiver preenchida, direciona para a tela de login
            if (Session["UsuarioLogado"] == null)
                return RedirectToAction("Index", "Login");

            //recebe o usuário logado
            usuarioLogado = (UsuarioLoja)(Session["UsuarioLogado"]);

            #endregion

            //busca o id do parceiro
            Int32.TryParse(Session["IdParceiro"].ToString(), out int idParceiro);
            Session["IdParceiro"] = null;

            //busca os dados do brinde
            BrindeParceiro brindeParceiro = new BrindeParceiro
            {
                Id = id,
                IdLoja = usuarioLogado.IdLoja,
                IdParceiro = idParceiro
            };

            //inativa o parceiro
            string urlPost = string.Format("/BrindeParceiro/Desativar");

            retornoRequest = rest.Post(urlPost, brindeParceiro);

            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}
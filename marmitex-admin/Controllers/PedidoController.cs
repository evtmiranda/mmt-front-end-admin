using ClassesMarmitex;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;
using System.Web.Mvc;
using System;
using System.Web.Http;

namespace marmitex_admin.Controllers
{
    public class PedidoController : BaseController
    {
        private RequisicoesREST rest;
        private DadosRequisicaoRest retornoRequest;
        private List<Pedido> listaPedidos;

        //O Ninject é o responsável por cuidar da criação de todos esses objetos
        public PedidoController(RequisicoesREST rest)
        {
            this.rest = rest;
            this.listaPedidos = new List<Pedido>();
        }

        // GET: Pedido
        /// <summary>
        /// monta um objeto com o cabeçalho do pedido e outro com o pedido completo para ser exibido no modal
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

                #region limpa as viewbags de mensagem

                ViewBag.MensagemPedidos = null;

                #endregion

                //busca todos os pedidos da loja com data de entrega == hoje
                retornoRequest = rest.Get(string.Format("/Pedido/BuscarPedidos/{0}/{1}", usuarioLogado.IdLoja, "true"));

                //se não encontrar pedidos para a loja
                if (retornoRequest.HttpStatusCode == HttpStatusCode.NoContent)
                {
                    ViewBag.MensagemPedidos = "não existem pedidos para hoje";
                    return View("Index");
                }

                //se ocorrer algum erro
                if (retornoRequest.HttpStatusCode != HttpStatusCode.OK)
                {
                    ViewBag.MensagemPedidos = "não foi possível consultar os pedidos. por favor, tente atualizar a página ou entre em contato com o administrador do sistema...";
                    return View("Index");
                }

                string jsonPedidos = retornoRequest.objeto.ToString();

                listaPedidos = JsonConvert.DeserializeObject<List<Pedido>>(jsonPedidos);

                return View(listaPedidos);
            }
            catch (Exception)
            {
                ViewBag.MensagemPedidos = "não foi possível exibir os pedidos. por favor, tente atualizar a página ou entre em contato com o administrador do sistema...";
                return View("Index");
            }
        }

        public string AtualizarStatusPedido(string dadosJson)
        {
            #region validacao usuario logado

            //se a sessão de usuário não estiver preenchida, direciona para a tela de login
            if (Session["UsuarioLogado"] == null)
                return "erro";

            //recebe o usuário logado
            usuarioLogado = (UsuarioLoja)(Session["UsuarioLogado"]);

            #endregion

            DadosAtualizarStatusPedido dadosAtualizarPedido = new DadosAtualizarStatusPedido();
            dadosAtualizarPedido = JsonConvert.DeserializeObject<DadosAtualizarStatusPedido>(dadosJson);

            //variável para armazenar o retorno da api
            DadosRequisicaoRest retornoRequest = new DadosRequisicaoRest();

            //tratamento de erros
            try
            {
                //monta a url de chamada na api
                string urlPost = string.Format("/Pedido/AtualizarStatusPedido/{0}/", usuarioLogado.IdLoja);

                //realiza o post passando o usuário no body
                retornoRequest = rest.Post(urlPost, dadosAtualizarPedido);

                //se o status não for OK, uma exception é lançada e sera exibida na tela via alert
                if (retornoRequest.HttpStatusCode != HttpStatusCode.OK)
                    return "erro";

                //se o status do pedido for "na fila", não envia e-mail
                if (dadosAtualizarPedido.IdStatusPedido == 0)
                    return "sucesso";

                #region envia e-mail com o status

                string emailUsuario = "";

                #region busca o e-mail do usuário que realizou o pedido

                //monta a url de chamada na api
                string urlGet = string.Format("/Pedido/BuscarEmailUsuarioPedido/{0}/{1}", dadosAtualizarPedido.IdPedido, usuarioLogado.IdLoja);

                //busca o e-mail do usuário
                retornoRequest = rest.Get(urlGet);

                if(retornoRequest.HttpStatusCode == HttpStatusCode.OK)
                {
                    emailUsuario = JsonConvert.DeserializeObject<string>(retornoRequest.objeto.ToString());
                }
                else
                    return "erro email";


                #endregion

                DadosEnvioEmailUnitario dadosEmail = new DadosEnvioEmailUnitario
                {
                    From = string.Format("{0} <naoresponder@tasaindo.com.br>", usuarioLogado.NomeLoja),
                    To = emailUsuario
                };

                if(dadosAtualizarPedido.IdStatusPedido == 0)
                {
                    dadosEmail.Subject = "Pedido na fila";
                    dadosEmail.Text = string.Format(@"<html lang=""pt-br"" style=""margin-left: 10%; width: 80%; font-family: 'Open Sans';"">

                                                    <head>
                                                    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
                                                    </head>

                                                    <body>

                                                    <div class=""barra-brand"" style=""background-color: #bc2026; height: 50px; text-align: left; border: #fff;"" align=""left"">
	                                                    <p class=""brand"" style=""color: #fff; letter-spacing: 2px; text-transform: uppercase; padding: 13px;"">{0}</p>
                                                    </div>

                                                    <div class=""body"" style=""margin-top: 3em; text-align: center; border-left-width: 1px; border-left-color: #bc2026; border-left-style: dotted; border-right-width: 1px; border-right-color: #bc2026; border-right-style: dotted;"" align=""center"">
	                                                    <p>Olá, o seu pedido está na fila.</p>
                                                    </div>

                                                    <div class=""rodape"" style=""margin-top: 5em; background-color: #FCFCFF; padding: 2em;"">
	                                                    <p>Obrigado!</p>
	                                                    <p>Equipe {0}</p>
                                                    </div>

                                                    </body>

                                                    </html>", usuarioLogado.NomeLoja.ToUpper());
                }

                if (dadosAtualizarPedido.IdStatusPedido == 1)
                {
                    dadosEmail.Subject = "Pedido em andamento";
                    string mensagemBody = "Olá, o seu pedido está em andamento.";

                    dadosEmail.Text = string.Format(@"<html lang=""pt-br"" style=""margin-left: 10%; width: 80%; font-family: 'Open Sans';"">

                                                    <head>
                                                    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
                                                    </head>

                                                    <body>

                                                    <div class=""barra-brand"" style=""background-color: #bc2026; height: 50px; text-align: left; border: #fff;"" align=""left"">
	                                                    <p class=""brand"" style=""color: #fff; letter-spacing: 2px; text-transform: uppercase; padding: 13px;"">{0}</p>
                                                    </div>

                                                    <div class=""body"" style=""margin-top: 3em; text-align: center; border-left-width: 1px; border-left-color: #bc2026; border-left-style: dotted; border-right-width: 1px; border-right-color: #bc2026; border-right-style: dotted;"" align=""center"">
	                                                    <p>{1}</p>
                                                    </div>

                                                    <div class=""rodape"" style=""margin-top: 5em; background-color: #FCFCFF; padding: 2em;"">
	                                                    <p>Obrigado!</p>
	                                                    <p>Equipe {0}</p>
                                                    </div>

                                                    </body>

                                                    </html>", usuarioLogado.NomeLoja.ToUpper(), mensagemBody);
                }

                if (dadosAtualizarPedido.IdStatusPedido == 2)
                {
                    dadosEmail.Subject = "Pedido entregue";
                    string mensagemBody = "Olá, o seu pedido foi entregue.";

                    dadosEmail.Text = string.Format(@"<html lang=""pt-br"" style=""margin-left: 10%; width: 80%; font-family: 'Open Sans';"">

                                                    <head>
                                                    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
                                                    </head>

                                                    <body>

                                                    <div class=""barra-brand"" style=""background-color: #bc2026; height: 50px; text-align: left; border: #fff;"" align=""left"">
	                                                    <p class=""brand"" style=""color: #fff; letter-spacing: 2px; text-transform: uppercase; padding: 13px;"">{0}</p>
                                                    </div>

                                                    <div class=""body"" style=""margin-top: 3em; text-align: center; border-left-width: 1px; border-left-color: #bc2026; border-left-style: dotted; border-right-width: 1px; border-right-color: #bc2026; border-right-style: dotted;"" align=""center"">
	                                                    <p>{1}</p>
                                                    </div>

                                                    <div class=""rodape"" style=""margin-top: 5em; background-color: #FCFCFF; padding: 2em;"">
	                                                    <p>Obrigado!</p>
	                                                    <p>Equipe {0}</p>
                                                    </div>

                                                    </body>

                                                    </html>", usuarioLogado.NomeLoja.ToUpper(), mensagemBody);
                }

                if (dadosAtualizarPedido.IdStatusPedido == 3)
                {
                    dadosEmail.Subject = "Pedido cancelado";
                    string mensagemBody = "Olá, o seu pedido foi cancelado. Motivo do cancelamento: " + dadosAtualizarPedido.MotivoCancelamento;

                    dadosEmail.Text = string.Format(@"<html lang=""pt-br"" style=""margin-left: 10%; width: 80%; font-family: 'Open Sans';"">

                                                    <head>
                                                    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
                                                    </head>

                                                    <body>

                                                    <div class=""barra-brand"" style=""background-color: #bc2026; height: 50px; text-align: left; border: #fff;"" align=""left"">
	                                                    <p class=""brand"" style=""color: #fff; letter-spacing: 2px; text-transform: uppercase; padding: 13px;"">{0}</p>
                                                    </div>

                                                    <div class=""body"" style=""margin-top: 3em; text-align: center; border-left-width: 1px; border-left-color: #bc2026; border-left-style: dotted; border-right-width: 1px; border-right-color: #bc2026; border-right-style: dotted;"" align=""center"">
	                                                    <p>{1}</p>
                                                    </div>

                                                    <div class=""rodape"" style=""margin-top: 5em; background-color: #FCFCFF; padding: 2em;"">
	                                                    <p>Obrigado!</p>
	                                                    <p>Equipe {0}</p>
                                                    </div>

                                                    </body>

                                                    </html>", usuarioLogado.NomeLoja.ToUpper(), mensagemBody);
                }

                retornoRequest = rest.Post("Email/EnviarEmailUnitario", dadosEmail);

                if (retornoRequest.HttpStatusCode == HttpStatusCode.OK)
                    return "sucesso";
                else
                    return "erro email";

                #endregion
            }
            //se ocorrer algum erro inesperado lança a exception
            catch(Exception)
            {
                return "erro";
            }
        }

    }
}
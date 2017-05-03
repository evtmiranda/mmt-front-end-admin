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
        private UsuarioLoja usuarioLogado;
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
            //recebe o usuário logado
            usuarioLogado = (UsuarioLoja)(Session["UsuarioLogado"]);

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
            return View();
        }
    }
}
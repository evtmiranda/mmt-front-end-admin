using ClassesMarmitex;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Web.Mvc;

namespace marmitex_admin.Controllers
{
    public class HorarioEntregaController : BaseController
    {
        private RequisicoesREST rest;
        private DadosRequisicaoRest retornoRequest;


        //O Ninject é o responsável por cuidar da criação de todos esses objetos
        public HorarioEntregaController(RequisicoesREST rest)
        {
            this.rest = rest;
        }

        // GET: HorarioEntrega
        public ActionResult Index()
        {
            #region validacao usuario logado

            //se a sessão de usuário não estiver preenchida, direciona para a tela de login
            if (Session["UsuarioLogado"] == null)
                return RedirectToAction("Index", "Login");

            //recebe o usuário logado
            usuarioLogado = (UsuarioLoja)(Session["UsuarioLogado"]);

            #endregion

            DadosHorarioEntrega dadosHorarioEntrega = new DadosHorarioEntrega();

            //busca os horários de entrega
            retornoRequest = rest.Get("/HorarioEntrega/Listar/" + usuarioLogado.IdLoja);

            //se não encontrar
            if (retornoRequest.HttpStatusCode == HttpStatusCode.NotFound)
            {
                ViewBag.MensagemHorarioEntrega = "nenhum horário de entrega encontrado";
                return View("Index");
            }

            //se ocorrer algum erro
            if (retornoRequest.HttpStatusCode != HttpStatusCode.OK)
            {
                ViewBag.MensagemHorarioEntrega = "não foi possível consultar os horários de entrega. por favor, tente atualizar a página ou entre em contato com o administrador do sistema...";
                return View("Index");
            }

            string jsonRetorno = retornoRequest.objeto.ToString();

            dadosHorarioEntrega = JsonConvert.DeserializeObject<DadosHorarioEntrega>(jsonRetorno);

            return View(dadosHorarioEntrega);
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

        public ActionResult AdicionarHorario(HorarioEntrega horarioEntrega)
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
                string urlPost = "/HorarioEntrega/Adicionar";

                horarioEntrega.IdLoja = usuarioLogado.IdLoja;

                retornoRequest = rest.Post(urlPost, horarioEntrega);

                //se não for cadastrado
                if (retornoRequest.HttpStatusCode != HttpStatusCode.Created)
                {
                    ViewBag.MensagemHorarioEntrega = "não foi possível cadastrar. por favor, tente novamente";
                    return View("Adicionar", horarioEntrega);
                }

                return RedirectToAction("Index", "HorarioEntrega");
            }
            catch (Exception)
            {
                ViewBag.MensagemHorarioEntrega = "não foi possível cadastrar. por favor, tente novamente";
                return View("Adicionar", horarioEntrega);
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

            HorarioEntrega horarioEntrega = new HorarioEntrega();

            retornoRequest = rest.Get(string.Format("/HorarioEntrega/{0}/{1}", id, usuarioLogado.IdLoja));

            //se não encontrar com este id
            if (retornoRequest.HttpStatusCode == HttpStatusCode.NoContent)
            {
                ViewBag.MensagemEditarHorarioEntrega = "não foi possível carregar os dados. por favor, tente atualizar a página ou entre em contato com o administrador do sistema...";
                return View();
            }

            //se ocorrer algum erro
            if (retornoRequest.HttpStatusCode != HttpStatusCode.OK)
            {
                ViewBag.MensagemEditarHorarioEntrega = "não foi possível carregar os dados. por favor, tente atualizar a página ou entre em contato com o administrador do sistema...";
                return View();
            }

            string jsonRetorno = retornoRequest.objeto.ToString();

            horarioEntrega = JsonConvert.DeserializeObject<HorarioEntrega>(jsonRetorno);

            return View(horarioEntrega);
        }

        [HttpPost]
        public ActionResult EditarHorario(HorarioEntrega horarioEntrega)
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
                horarioEntrega.IdLoja = usuarioLogado.IdLoja;

                string urlPost = string.Format("/HorarioEntrega/Atualizar");

                retornoRequest = rest.Post(urlPost, horarioEntrega);

                //se não for atualizado
                if (retornoRequest.HttpStatusCode != HttpStatusCode.OK)
                {
                    ViewBag.MensagemEditarHorarioEntrega = "não foi possível atualizar. por favor, tente novamente";
                    return View("Editar", horarioEntrega);
                }

                //se for atualizado, direciona para a tela de visualização
                return RedirectToAction("Index", "HorarioEntrega");
            }
            catch (Exception)
            {
                ViewBag.MensagemEditarHorarioEntrega = "não foi possível atualizar. por favor, tente novamente";
                return View("Editar", horarioEntrega);
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

                HorarioEntrega horarioEntrega = new HorarioEntrega
                {
                    Id = id,
                    IdLoja = usuarioLogado.IdLoja,
                    Ativo = false
                };

                //recurso do post
                string urlPost = string.Format("/HorarioEntrega/Excluir");

                //faz o post
                retornoRequest = rest.Post(urlPost, horarioEntrega);

                //se não for atualizado
                if (retornoRequest.HttpStatusCode != HttpStatusCode.OK)
                {
                    ViewBag.MensagemExcluirHorarioEntrega = "não foi possível excluir. por favor, tente novamente";
                    return View("Index");
                }

                //se for inativado, direciona para a tela de visualização
                return RedirectToAction("Index", "HorarioEntrega");
            }
            catch (Exception)
            {
                ViewBag.MensagemExcluirHorarioEntrega = "não foi possível excluir. por favor, tente novamente";
                return View("Index");
            }
        }

        public ActionResult EditarTempoAntecedencia(int id)
        {
            #region validacao usuario logado

            //se a sessão de usuário não estiver preenchida, direciona para a tela de login
            if (Session["UsuarioLogado"] == null)
                return RedirectToAction("Index", "Login");

            //recebe o usuário logado
            usuarioLogado = (UsuarioLoja)(Session["UsuarioLogado"]);

            #endregion

            TempoAntecedenciaEntrega tempoAntecedenciaEntrega = new TempoAntecedenciaEntrega();

            retornoRequest = rest.Get(string.Format("/HorarioEntrega/TempoAntecedencia/{0}/{1}", id, usuarioLogado.IdLoja));

            //se não encontrar com este id
            if (retornoRequest.HttpStatusCode == HttpStatusCode.NoContent)
            {
                ViewBag.MensagemEditarHorarioEntrega = "não foi possível carregar os dados. por favor, tente atualizar a página ou entre em contato com o administrador do sistema...";
                return View();
            }

            //se ocorrer algum erro
            if (retornoRequest.HttpStatusCode != HttpStatusCode.OK)
            {
                ViewBag.MensagemEditarHorarioEntrega = "não foi possível carregar os dados. por favor, tente atualizar a página ou entre em contato com o administrador do sistema...";
                return View();
            }

            string jsonRetorno = retornoRequest.objeto.ToString();

            tempoAntecedenciaEntrega = JsonConvert.DeserializeObject<TempoAntecedenciaEntrega>(jsonRetorno);

            return View(tempoAntecedenciaEntrega);
        }

        public ActionResult EditarTempoAntecedenciaHorario(TempoAntecedenciaEntrega tempoAntecedenciaEntrega)
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
                tempoAntecedenciaEntrega.IdLoja = usuarioLogado.IdLoja;
                tempoAntecedenciaEntrega.Ativo = true;

                string urlPost = string.Format("/HorarioEntrega/TempoAntecedencia/Atualizar");

                retornoRequest = rest.Post(urlPost, tempoAntecedenciaEntrega);

                //se não for atualizado
                if (retornoRequest.HttpStatusCode != HttpStatusCode.OK)
                {
                    ViewBag.MensagemEditarHorarioEntrega = "não foi possível atualizar. por favor, tente novamente";
                    return View("EditarTempoAntecedencia", tempoAntecedenciaEntrega);
                }

                //se for atualizado, direciona para a tela de visualização
                return RedirectToAction("Index", "HorarioEntrega");
            }
            catch (Exception)
            {
                ViewBag.MensagemEditarHorarioEntrega = "não foi possível atualizar. por favor, tente novamente";
                return View("EditarTempoAntecedencia", tempoAntecedenciaEntrega);
            }
        }


    }
}
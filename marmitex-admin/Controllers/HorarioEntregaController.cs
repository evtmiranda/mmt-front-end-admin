using ClassesMarmitex;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Web.Mvc;
using marmitex_admin.Utils;

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

                DadosHorarioEntrega dadosHorarioEntrega = new DadosHorarioEntrega();

                //busca os horários de entrega
                retornoRequest = rest.Get("/HorarioEntrega/Listar/" + usuarioLogado.IdLoja);

                //se não encontrar
                if (retornoRequest.HttpStatusCode == HttpStatusCode.NotFound)
                {
                    ViewBag.MensagemHorarioEntrega = "nenhum horário de entrega encontrado";
                    return View();
                }

                //se ocorrer algum erro
                if (retornoRequest.HttpStatusCode != HttpStatusCode.OK)
                {
                    ViewBag.MensagemHorarioEntrega = "não foi possível consultar os horários de entrega. por favor, tente atualizar a página ou entre em contato com o administrador do sistema...";
                    return View();
                }

                string jsonRetorno = retornoRequest.objeto.ToString();

                dadosHorarioEntrega = JsonConvert.DeserializeObject<DadosHorarioEntrega>(jsonRetorno);

                return View(dadosHorarioEntrega);
            }
            catch (Exception)
            {
                ViewBag.MensagemHorarioEntrega = "não foi possível consultar os horários de entrega. por favor, tente atualizar a página ou entre em contato com o administrador do sistema...";
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
                    ViewBag.MensagemCadHorarioEntrega = "não foi possível cadastrar o horário de entrega. por favor, tente novamente";
                    return View("Adicionar", horarioEntrega);
                }

                return RedirectToAction("Index", "HorarioEntrega");
            }
            catch (Exception)
            {
                ViewBag.MensagemCadHorarioEntrega = "não foi possível cadastrar o horário de entrega. por favor, tente novamente";
                return View("Adicionar", horarioEntrega);
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

                HorarioEntrega horarioEntrega = new HorarioEntrega();

                retornoRequest = rest.Get(string.Format("/HorarioEntrega/{0}/{1}", id, usuarioLogado.IdLoja));

                //se não encontrar com este id
                if (retornoRequest.HttpStatusCode == HttpStatusCode.NoContent)
                {
                    ViewBag.MensagemCarregamentoEditarHorarioEntrega = "não foi possível carregar os dados. por favor, tente atualizar a página ou entre em contato com o administrador do sistema...";
                    return View();
                }

                //se ocorrer algum erro
                if (retornoRequest.HttpStatusCode != HttpStatusCode.OK)
                {
                    ViewBag.MensagemCarregamentoEditarHorarioEntrega = "não foi possível carregar os dados. por favor, tente atualizar a página ou entre em contato com o administrador do sistema...";
                    return View();
                }

                string jsonRetorno = retornoRequest.objeto.ToString();

                horarioEntrega = JsonConvert.DeserializeObject<HorarioEntrega>(jsonRetorno);

                return View(horarioEntrega);
            }
            catch (Exception)
            {
                ViewBag.MensagemCarregamentoEditarHorarioEntrega = "não foi possível carregar os dados. por favor, tente atualizar a página ou entre em contato com o administrador do sistema...";
                return View();
            }

        }

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

            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }

        #region tempo antecedencia

        public ActionResult EditarTempoAntecedencia(int id)
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

                TempoAntecedenciaEntrega tempoAntecedenciaEntrega = new TempoAntecedenciaEntrega();

                retornoRequest = rest.Get(string.Format("/HorarioEntrega/TempoAntecedencia/{0}/{1}", id, usuarioLogado.IdLoja));

                //se não encontrar com este id
                if (retornoRequest.HttpStatusCode == HttpStatusCode.NoContent)
                {
                    ViewBag.MensagemCarregamentoEditarHorarioEntrega = "não foi possível carregar os dados do tempo de antecedência. por favor, tente atualizar a página ou entre em contato com o administrador do sistema...";
                    return View();
                }

                //se ocorrer algum erro
                if (retornoRequest.HttpStatusCode != HttpStatusCode.OK)
                {
                    ViewBag.MensagemCarregamentoEditarHorarioEntrega = "não foi possível carregar os dados do tempo de antecedência. por favor, tente atualizar a página ou entre em contato com o administrador do sistema...";
                    return View();
                }

                string jsonRetorno = retornoRequest.objeto.ToString();

                tempoAntecedenciaEntrega = JsonConvert.DeserializeObject<TempoAntecedenciaEntrega>(jsonRetorno);

                return View(tempoAntecedenciaEntrega);
            }
            catch (Exception)
            {
                ViewBag.MensagemCarregamentoEditarHorarioEntrega = "não foi possível carregar os dados do tempo de antecedência. por favor, tente atualizar a página ou entre em contato com o administrador do sistema...";
                return View();
            }

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
                //tempoAntecedenciaEntrega.Ativo = true;

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

        #endregion

        #region tempo antecedencia cancelamento

        public ActionResult EditarTempoAntecedenciaCancelamento(int id)
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

                TempoAntecedenciaCancelamentoEntrega tempoAntecedenciaCancelamentoEntrega = new TempoAntecedenciaCancelamentoEntrega();

                retornoRequest = rest.Get(string.Format("/HorarioEntrega/TempoAntecedenciaCancelamento/{0}/{1}", id, usuarioLogado.IdLoja));

                //se não encontrar com este id
                if (retornoRequest.HttpStatusCode == HttpStatusCode.NoContent)
                {
                    ViewBag.MensagemCarregamentoEditarHorarioEntrega = "não foi possível carregar os dados do tempo de antecedência de cancelamento. por favor, tente atualizar a página ou entre em contato com o administrador do sistema...";
                    return View();
                }

                //se ocorrer algum erro
                if (retornoRequest.HttpStatusCode != HttpStatusCode.OK)
                {
                    ViewBag.MensagemCarregamentoEditarHorarioEntrega = "não foi possível carregar os dados do tempo de antecedência de cancelamento. por favor, tente atualizar a página ou entre em contato com o administrador do sistema...";
                    return View();
                }

                string jsonRetorno = retornoRequest.objeto.ToString();

                tempoAntecedenciaCancelamentoEntrega = JsonConvert.DeserializeObject<TempoAntecedenciaCancelamentoEntrega>(jsonRetorno);

                return View(tempoAntecedenciaCancelamentoEntrega);
            }
            catch (Exception)
            {
                ViewBag.MensagemCarregamentoEditarHorarioEntrega = "não foi possível carregar os dados do tempo de antecedência de cancelamento. por favor, tente atualizar a página ou entre em contato com o administrador do sistema...";
                return View();
            }

        }

        public ActionResult EditarTempoAntecedenciaCancelamentoHorario(TempoAntecedenciaEntrega tempoAntecedenciaEntrega)
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
                //tempoAntecedenciaEntrega.Ativo = true;

                string urlPost = string.Format("/HorarioEntrega/TempoAntecedenciaCancelamento/Atualizar");

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

        #endregion

        #region dias de funcionamento

        [MyErrorHandler]
        public ActionResult DesativarDiaFuncionamento(int id)
        {
            #region validacao usuario logado

            //se a sessão de usuário não estiver preenchida, direciona para a tela de login
            if (Session["UsuarioLogado"] == null)
                return RedirectToAction("Index", "Login");

            //recebe o usuário logado
            usuarioLogado = (UsuarioLoja)(Session["UsuarioLogado"]);

            #endregion

            DiasDeFuncionamento diaFuncionamento = new DiasDeFuncionamento
            {
                Id = id,
                IdLoja = usuarioLogado.IdLoja,
                Ativo = true
            };

            //recurso do post
            string urlPost = string.Format("/HorarioEntrega/DiaFuncionamento/Desativar");

            //faz o post
            retornoRequest = rest.Post(urlPost, diaFuncionamento);

            return Json(new { success = true }, JsonRequestBehavior.AllowGet);

        }

        [MyErrorHandler]
        public ActionResult ExcluirDiaFuncionamento(int id)
        {
            #region validacao usuario logado

            //se a sessão de usuário não estiver preenchida, direciona para a tela de login
            if (Session["UsuarioLogado"] == null)
                return RedirectToAction("Index", "Login");

            //recebe o usuário logado
            usuarioLogado = (UsuarioLoja)(Session["UsuarioLogado"]);

            #endregion

            DiasDeFuncionamento diaFuncionamento = new DiasDeFuncionamento
            {
                Id = id,
                IdLoja = usuarioLogado.IdLoja,
                Ativo = false
            };

            //recurso do post
            string urlPost = string.Format("/HorarioEntrega/DiaFuncionamento/Excluir");

            //faz o post
            retornoRequest = rest.Post(urlPost, diaFuncionamento);

            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }

        #endregion

    }
}
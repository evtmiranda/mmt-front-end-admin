    namespace marmitex_admin.Controllers
{
    using System.Web.Mvc;
    using ClassesMarmitex;
    using System.Net;
    using System;
    using Newtonsoft.Json;

    public class LoginController : BaseLoginController
    {

        private RequisicoesREST rest;

        //O Ninject é o responsável por cuidar da criação de todos esses objetos
        public LoginController(RequisicoesREST rest)
        {
            this.rest = rest;
        }

        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 1. Verifica se o usuário existe na base
        /// 2. Busca os dados do usuário e preenche a sessão "usuarioLogado"
        /// 3. Direciona para a view "Index" do controller "Home"
        /// </summary>
        /// <param name="usuario"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Autenticar(Usuario usuario)
        {
            //limpa a sessão de mensagem
            //Session["MensagemAutenticacao"] = null;

            #region limpa as viewbags de mensagem

            ViewBag.MensagemAutenticacao = null;

            #endregion

            //captura a loja em questão
            Session["dominioLoja"] = BuscarUrlLoja();

            //se não conseguir capturar a loja, direciona para a tela de erro
            if (Session["dominioLoja"] == null) {
                ViewBag.MensagemAutenticacao = "estamos com dificuldade em buscar dados no servidor. por favor, tente atualizar a página";
                return View("Index", usuario);
            }

            if(string.IsNullOrEmpty(usuario.Email) || string.IsNullOrEmpty(usuario.Senha))
            {
                ViewBag.MensagemAutenticacao = "preencha o login e senha";
                return View("Index", usuario);
            }


            string dominioLoja = Session["dominioLoja"].ToString();

            DadosRequisicaoRest retornoAutenticacao = new DadosRequisicaoRest();
            DadosRequisicaoRest retornoDadosUsuario = new DadosRequisicaoRest();

            try
            {
                string urlPost = string.Format("/usuario/autenticar/{0}/'{1}'", TipoUsuario.Loja, dominioLoja);

                retornoAutenticacao = rest.Post(urlPost, usuario);

                //se o usuário for autenticado, direciona para a tela home
                if (retornoAutenticacao.HttpStatusCode == HttpStatusCode.Accepted)
                {
                    UsuarioLoja usuarioLogado = new UsuarioLoja();

                    try
                    {
                        //busca os dados do usuário
                        retornoDadosUsuario = rest.Post(string.Format("usuario/buscarPorEmail/{0}", TipoUsuario.Loja), usuario);

                        //verifica se os dados do usuário foram encontrados
                        if (retornoDadosUsuario.HttpStatusCode != HttpStatusCode.OK)
                            throw new Exception();

                        //converte o usuario em objeto e seta a url da loja
                        usuarioLogado = JsonConvert.DeserializeObject<UsuarioLoja>(retornoDadosUsuario.objeto.ToString());
                        usuarioLogado.UrlLoja = dominioLoja;

                        //armazena o usuário na sessão "usuarioLogado"
                        Session["usuarioLogado"] = usuarioLogado;

                        //limpa a sessão "usuarioLogin"
                        Session["usuarioLogin"] = null;

                        return RedirectToAction("Index", "Home");
                    }
                    //se não for possível consultar os dados do usuário
                    catch (Exception)
                    {
                        ViewBag.MensagemAutenticacao = "estamos com dificuldade em buscar dados no servidor. por favor, tente novamente";
                        return View("Index", usuario);
                    }
                }
                else if (retornoAutenticacao.HttpStatusCode == HttpStatusCode.Unauthorized)
                {
                    ViewBag.MensagemAutenticacao = "usuário ou senha inválida";
                    return View("Index", usuario);
                }

                //se for algum outro erro
                else
                {
                    ViewBag.MensagemAutenticacao = "não foi possível realizar o login. por favor, tente novamente";
                    return View("Index", usuario);
                }
            }
            catch (Exception)
            {
                ViewBag.MensagemAutenticacao = "não foi possível realizar o login. por favor, tente novamente";
                return View("Index", usuario);
            }
        }

        /// <summary>
        /// Limpa todas as sessões e direciona para a tela de login
        /// </summary>
        /// <returns></returns>
        public ActionResult Deslogar()
        {
            //Limpa todas as sessões
            Session.Clear();

            //Direciona para a tela de login
            return View("Index");
        }

    }
}
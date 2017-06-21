using System;
using System.Web.Mvc;
using System.Web.Routing;

namespace marmitex_admin
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }

        //protected void Application_Error(object sender, EventArgs e)
        //{
        //    Response.Redirect("~/Login");
        //}
    }
}

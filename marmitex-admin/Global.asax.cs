using System;
using System.Web;
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

        /* protected void Application_Error(object sender, EventArgs e)
         {
             var app = (MvcApplication)sender;
             var context = app.Context;
             var ex = app.Server.GetLastError();
             context.Response.Clear();
             context.ClearError();

             var httpException = ex as HttpException;

             //se for exception por conta do href do drag and drop, segue o fluxo
             //pois não impacta a operação
             if (ex != null)
                 if (ex.Message.Contains("drag"))
                     return;

             Response.Redirect("~/Login");
         }*/
    }
}

using System.Web.Mvc;
using System.Web.Routing;

namespace IdeoneClient.Web
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Paste/GetLanguages",
                "languages",
                new { controller = "Paste", action = "GetLanguages" }
            );

            routes.MapRoute(
                "Paste/Create",
                "pastes",
                new { controller = "Paste", action = "Create" }
            );

            routes.MapRoute(
                "Paste/GetStatus",
                "paste/{link}/status",
                new { controller = "Paste", action = "GetStatus" }
            );

            routes.MapRoute(
                "Paste/Detail",
                "paste/{link}",
                new { controller = "Paste", action = "GetDetail" }
            );

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            RegisterRoutes(RouteTable.Routes);
        }
    }
}
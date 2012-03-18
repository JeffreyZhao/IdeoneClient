using System.Web.Mvc;

namespace IdeoneClient.Web.Controllers
{
    public class HttpStatusCodeResult : ActionResult
    {
        public HttpStatusCodeResult(int statusCode)
        {
            this.StatusCode = statusCode;
        }

        public int StatusCode { get; private set; }

        public override void ExecuteResult(ControllerContext context)
        {
            context.HttpContext.Response.StatusCode = this.StatusCode;
        }
    }
}
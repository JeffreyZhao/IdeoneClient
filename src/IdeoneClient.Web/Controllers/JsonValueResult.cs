using System.Json;
using System.Web.Mvc;

namespace IdeoneClient.Web.Controllers
{
    public class JsonValueResult : ActionResult
    {
        public JsonValueResult(JsonValue value)
        {
            this.Value = value;
        }

        public JsonValue Value { get; private set; }

        public override void ExecuteResult(ControllerContext context)
        {
            context.HttpContext.Response.ContentType = "application/json";
            this.Value.Save(context.HttpContext.Response.Output);
        }
    }
}
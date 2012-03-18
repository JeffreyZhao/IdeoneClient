using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Json;

namespace IdeoneClient.Web
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
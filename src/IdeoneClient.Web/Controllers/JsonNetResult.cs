using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Converters;
using System.Web.Mvc;

namespace IdeoneClient.Web.Controllers
{
    internal class JsonNetResult : ActionResult
    {
        private readonly JsonSerializerSettings DefaultSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            NullValueHandling = NullValueHandling.Ignore,
            Converters = new List<JsonConverter>()
            {
                new StringEnumConverter(),
                new IsoDateTimeConverter()
            }
        };

        public JsonNetResult(object value, int statusCode = 200, JsonSerializerSettings settings = null)
        {
            this.Value = value;
            this.StatusCode = statusCode;
            this.Settings = settings ?? DefaultSettings;
        }

        public object Value { get; private set; }

        public int StatusCode { get; private set; }

        public JsonSerializerSettings Settings { get; private set; }

        public override void ExecuteResult(ControllerContext context)
        {
            var response = context.HttpContext.Response;
            response.StatusCode = this.StatusCode;
            response.ContentType = "application/json";
            response.Write(JsonConvert.SerializeObject(this.Value, Formatting.None, this.Settings));
        }
    }
}
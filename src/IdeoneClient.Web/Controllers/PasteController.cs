using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace IdeoneClient.Web.Controllers
{
    public class PasteController : Controller
    {
        private IdeoneService IdeoneService
        {
            get
            {
                var base64 = this.HttpContext.Request.Headers["Authorization"].Substring(6);
                var token = Encoding.UTF8.GetString(Convert.FromBase64String(base64));
                var index = token.IndexOf(':');
                var username = token.Substring(0, index);
                var password = token.Substring(index + 1);
                return new IdeoneService(username, password);
            }
        }

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

        private ActionResult JsonEx(object obj, JsonSerializerSettings settings = null)
        {
            var json = JsonConvert.SerializeObject(obj, Formatting.None, settings ?? DefaultSettings);
            return Content(json, "application/json");
        }

        [HttpGet]
        public ActionResult GetLanguages()
        {
            var languages = this.IdeoneService.GetLanguages();
            return JsonEx(languages);
        }

        [HttpPost]
        public ActionResult Create(int languageId, string sourceCode, string input = "", bool run = true, bool isPrivate = true)
        {
            var link = this.IdeoneService.CreateSubmission(languageId, sourceCode, input, run, isPrivate);
            return JsonEx(link);
        }

        [HttpGet]
        public ActionResult GetStatus(string link)
        {
            var status = this.IdeoneService.GetSubmissionStatus(link);
            return JsonEx(status);
        }

        [HttpGet]
        public ActionResult GetDetail(string link, bool withSource = false, bool withInput = false, bool withOutput = true, bool withStdErr = true, bool withCompileInfo = true)
        {
            var detail = this.IdeoneService.GetSubmissionDetail(link, withSource, withInput, withOutput, withStdErr, withCompileInfo);
            return JsonEx(detail);
        }

        private ActionResult ErrorToResult(Exception ex)
        {
            if (ex is AuthenticationFailedException)
            {
                return new HttpUnauthorizedResult();
            }
            else if (ex is PasteNotFoundException || ex is LanguageNotFoundException)
            {
                return new HttpStatusCodeResult(404);
            }
            else if (ex is AccessDeniedException)
            {
                return new HttpStatusCodeResult(403);
            }
            else if (ex is MonthlyLimitExceededException)
            {
                return new HttpStatusCodeResult(402);
            }
            else
            {
                return new HttpStatusCodeResult(500);
            }
        }

        protected override void OnException(ExceptionContext filterContext)
        {
            filterContext.Result = ErrorToResult(filterContext.Exception);
            filterContext.ExceptionHandled = true;
        }
    }
}

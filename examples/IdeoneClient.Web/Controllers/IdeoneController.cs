using System;
using System.Json;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace IdeoneClient.Web.Controllers
{
    public class IdeoneController : Controller
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

        public ActionResult Languages()
        {
            var languages = this.IdeoneService.GetLanguages();
            return Json(languages.ToDictionary(p => p.Key.ToString(), p => p.Value), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Create(int languageId, string sourceCode, string input = "", bool run = true, bool isPrivate = true)
        {
            var link = this.IdeoneService.CreateSubmission(languageId, sourceCode, input, run, isPrivate);
            return new JsonValueResult(link);
        }

        [HttpGet]
        public ActionResult GetStatus(string link)
        {
            var status = this.IdeoneService.GetSubmissionStatus(link);
            var json = new JsonObject
            {
                { "state", status.State.ToString() },
                { "result", status.Result.ToString() }
            };

            return new JsonValueResult(json);
        }

        public ActionResult GetDetail(string link, bool withSource = false, bool withInput = false, bool withOutput = true, bool withStdErr = true, bool withCompileInfo = true)
        {
            var detail = this.IdeoneService.GetSubmissionDetail(link, withSource, withInput, withOutput, withStdErr, withCompileInfo);
            var json = new JsonObject
            {
                { "time", detail.Time },
                { "memory", detail.Memory },
                { "error", detail.Error },
                { "compileInfo", detail.CompileInfo },
                { "output", detail.Output }
            };

            return new JsonValueResult(json);
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

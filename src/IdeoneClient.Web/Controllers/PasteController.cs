using System;
using System.Collections.Generic;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace IdeoneClient.Web.Controllers
{
    public class PasteController : AsyncController
    {
        private IdeoneService IdeoneService
        {
            get
            {
                try
                {
                    var base64 = this.HttpContext.Request.Headers["Authorization"].Substring(6);
                    var token = Encoding.UTF8.GetString(Convert.FromBase64String(base64));
                    var index = token.IndexOf(':');
                    var username = token.Substring(0, index);
                    var password = token.Substring(index + 1);
                    return new IdeoneService(username, password);
                }
                catch (Exception ex)
                {
                    throw new AuthenticationException("Invalid authentication information.", ex);
                }
            }
        }

        #region Helpers

        private ActionResult CreateErrorResult(int statusCode, string message)
        {
            return new JsonNetResult(new { message = message }, statusCode);
        }

        private ActionResult ErrorToResult(Exception ex)
        {
            while (true)
            {
                var aggEx = ex as AggregateException;
                if (aggEx != null)
                {
                    ex = aggEx.InnerExceptions[0];
                }
                else
                {
                    break;
                }
            }

            if (ex is AuthenticationFailedException || ex is AuthenticationException)
            {
                return CreateErrorResult(401, ex.Message);
            }
            else if (ex is PasteNotFoundException || ex is LanguageNotFoundException)
            {
                return CreateErrorResult(404, ex.Message);
            }
            else if (ex is AccessDeniedException)
            {
                return CreateErrorResult(403, ex.Message);
            }
            else if (ex is MonthlyLimitExceededException)
            {
                return CreateErrorResult(402, ex.Message);
            }
            else if (ex is IdeoneException)
            {
                return CreateErrorResult(500, ex.Message);
            }
            else
            {
                return CreateErrorResult(500, "Unexpected server error occurred.");
            }
        }

        private void ActionStart<T>(string resultName, Task<T> task)
        {
            this.AsyncManager.OutstandingOperations.Increment();

            task.ContinueWith(t =>
            {
                if (t.Exception != null)
                {
                    this.AsyncManager.Parameters["error"] = t.Exception;
                }
                else
                {
                    this.AsyncManager.Parameters[resultName] = t.Result;
                }

                this.AsyncManager.OutstandingOperations.Decrement();
            });
        }

        private ActionResult ActionCompleted<T>(Exception error, T result)
        {
            return error == null ? new JsonNetResult(result) : ErrorToResult(error);
        }

        #endregion

        [HttpGet]
        public void TestAsync()
        {
            this.ActionStart("result", this.IdeoneService.TestAsync());
        }

        public ActionResult TestCompleted(Exception error, Dictionary<string, object> result)
        {
            return ActionCompleted(error, result);
        }

        [HttpGet]
        public void GetLanguagesAsync()
        {
            this.ActionStart("languages", this.IdeoneService.GetLanguagesAsync());
        }

        public ActionResult GetLanguagesCompleted(Exception error, Dictionary<int, string> languages)
        {
            return this.ActionCompleted(error, languages);
        }

        [HttpPost]
        [ValidateInput(false)]
        public void CreateAsync(int languageId, string sourceCode, string input = "", bool run = true, bool isPrivate = true)
        {
            this.ActionStart("link", this.IdeoneService.CreateSubmissionAsync(languageId, sourceCode, input, run, isPrivate));
        }

        public ActionResult CreateCompleted(Exception error, string link)
        {
            return ActionCompleted(error, link);
        }

        [HttpGet]
        public void GetStatusAsync(string link)
        {
            this.ActionStart("status", this.IdeoneService.GetSubmissionStatusAsync(link));
        }

        public ActionResult GetStatusCompleted(Exception error, SubmissionStatus status)
        {
            return error == null ? new JsonNetResult(status) : ErrorToResult(error);
        }

        [HttpGet]
        public void GetDetailAsync(string link, bool withSource = false, bool withInput = false, bool withOutput = true, bool withStdErr = true, bool withCompileInfo = true)
        {
            this.ActionStart("detail", this.IdeoneService.GetSubmissionDetailAsync(link, withSource, withInput, withOutput, withStdErr, withCompileInfo));
        }

        public ActionResult GetDetailCompleted(Exception error, SubmissionDetail detail)
        {
            return this.ActionCompleted(error, detail);
        }

        protected override void OnException(ExceptionContext filterContext)
        {
            filterContext.Result = ErrorToResult(filterContext.Exception);
            filterContext.ExceptionHandled = true;
        }
    }
}

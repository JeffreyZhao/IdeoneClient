using System;
using System.Collections.Generic;
using System.Security.Authentication;
using System.Text;
using System.Web.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

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

        [HttpGet]
        public void GetLanguagesAsync()
        {
            this.AsyncManager.OutstandingOperations.Increment();

            this.IdeoneService
                .GetLanguagesAsync()
                .ContinueWith(t =>
                {
                    if (t.Exception != null)
                    {
                        this.AsyncManager.Parameters["error"] = t.Exception;
                    }
                    else
                    {
                        this.AsyncManager.Parameters["languages"] = t.Result;
                    }
                    
                    this.AsyncManager.OutstandingOperations.Decrement();
                });
        }

        public ActionResult GetLanguagesCompleted(Exception error, Dictionary<int, string> languages)
        {
            return error == null ? new JsonNetResult(languages) : ErrorToResult(error);
        }

        [HttpPost]
        [ValidateInput(false)]
        public void CreateAsync(int languageId, string sourceCode, string input = "", bool run = true, bool isPrivate = true)
        {
            this.AsyncManager.OutstandingOperations.Increment();

            this.IdeoneService
                .CreateSubmissionAsync(languageId, sourceCode, input, run, isPrivate)
                .ContinueWith(t =>
                {
                    if (t.Exception != null)
                    {
                        this.AsyncManager.Parameters["error"] = t.Exception;
                    }
                    else
                    {
                        this.AsyncManager.Parameters["link"] = t.Result;
                    }

                    this.AsyncManager.OutstandingOperations.Decrement();
                });
        }

        public ActionResult CreateCompleted(Exception error, string link)
        {
            return error == null ? new JsonNetResult(link) : ErrorToResult(error);
        }

        [HttpGet]
        public void GetStatusAsync(string link)
        {
            this.AsyncManager.OutstandingOperations.Increment();

            this.IdeoneService
                .GetSubmissionStatusAsync(link)
                .ContinueWith(t =>
                {
                    if (t.Exception != null)
                    {
                        this.AsyncManager.Parameters["error"] = t.Exception;
                    }
                    else
                    {
                        this.AsyncManager.Parameters["status"] = t.Result;
                    }

                    this.AsyncManager.OutstandingOperations.Decrement();
                });
        }

        public ActionResult GetStatusCompleted(Exception error, SubmissionStatus status)
        {
            return error == null ? new JsonNetResult(status) : ErrorToResult(error);
        }

        [HttpGet]
        public void GetDetailAsync(string link, bool withSource = false, bool withInput = false, bool withOutput = true, bool withStdErr = true, bool withCompileInfo = true)
        {
            this.AsyncManager.OutstandingOperations.Increment();

            this.IdeoneService
                .GetSubmissionDetailAsync(link, withSource, withInput, withOutput, withStdErr, withCompileInfo)
                .ContinueWith(t =>
                {
                    if (t.Exception != null)
                    {
                        this.AsyncManager.Parameters["error"] = t.Exception;
                    }
                    else
                    {
                        this.AsyncManager.Parameters["detail"] = t.Result;
                    }

                    this.AsyncManager.OutstandingOperations.Decrement();
                });
        }

        public ActionResult GetDetailCompleted(Exception error, SubmissionDetail detail)
        {
            return error == null ? new JsonNetResult(detail) : ErrorToResult(error);
        }

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

        protected override void OnException(ExceptionContext filterContext)
        {
            filterContext.Result = ErrorToResult(filterContext.Exception);
            filterContext.ExceptionHandled = true;
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using IdeoneClient.Ideone;
using System.Threading.Tasks;

namespace IdeoneClient
{
    public class IdeoneService
    {
        private readonly string _username;
        private readonly string _password;
        private readonly IIdeoneSoapService _soapService;

        internal IdeoneService(IIdeoneSoapService soapService, string username, string password)
        {
            this._soapService = soapService;
            this._username = username;
            this._password = password;
        }

        public IWebProxy Proxy
        {
            get
            {
                return this._soapService.Proxy;
            }
            set
            {
                this._soapService.Proxy = value;
            }
        }

        public IdeoneService(string url, string username, string password)
            : this(new IdeoneSoapService(url), username, password)
        { }

        public IdeoneService(string username, string password)
            : this(new IdeoneSoapService(), username, password)
        { }

        #region Helpers

        private T Handle<T>(Func<Hashtable> dataFactory, Func<Hashtable, T> dataProcessor)
        {
            try
            {
                var data = dataFactory();
                this.CheckError(data);
                return dataProcessor(data);
            }
            catch (IdeoneException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new IdeoneException(ex.Message, ex);
            }
        }

        private Task<T> HandleAsync<T>(Action<Action<IdeoneSoapResult>> caller, Func<Hashtable, T> dataProcessor)
        {
            var tcs = new TaskCompletionSource<T>();

            caller(result =>
            {
                if (result.Cancelled)
                {
                    tcs.SetCanceled();
                }
                else if (result.Error != null)
                {
                    if (result.Error is IdeoneException)
                    {
                        tcs.SetException(result.Error);
                    }
                    else
                    {
                        tcs.SetException(new IdeoneException(result.Error.Message, result.Error));
                    }
                }
                else
                {
                    try
                    {
                        var data = result.Data;
                        this.CheckError(data);

                        tcs.SetResult(dataProcessor(result.Data));
                    }
                    catch (IdeoneException ex)
                    {
                        tcs.SetException(ex);
                    }
                    catch (Exception ex)
                    {
                        tcs.SetException(new IdeoneException(ex.Message, ex));
                    }
                }
            });

            return tcs.Task;
        }

        #endregion

        #region GetLanguages

        private Dictionary<int, string> ProcessGetLangaugesData(Hashtable data)
        {
            var languages = (Hashtable)data["languages"];
            return languages.Cast<DictionaryEntry>().ToDictionary(
                p => (int)p.Key,
                p => (string)p.Value);
        }

        public Dictionary<int, string> GetLanguages()
        {
            return this.Handle(
                () => this._soapService.GetLanguages(this._username, this._password),
                this.ProcessGetLangaugesData);
        }

        public Task<Dictionary<int, string>> GetLanguagesAsync()
        {
            return this.HandleAsync(
                cb => this._soapService.GetLanguagesAsync(this._username, this._password, cb),
                this.ProcessGetLangaugesData);
        }

        #endregion

        #region CreateSubmission

        private string ProcessCreateSubmissionData(Hashtable data)
        {
            return (string)data["link"];
        }

        public string CreateSubmission(int languageId, string sourceCode, string input, bool run, bool isPrivate)
        {
            return this.Handle(
                () => this._soapService.CreateSubmission(this._username, this._password, sourceCode, languageId, input, run, isPrivate),
                this.ProcessCreateSubmissionData);
        }

        public Task<string> CreateSubmissionAsync(int languageId, string sourceCode, string input, bool run, bool isPrivate)
        {
            return this.HandleAsync(
                cb => this._soapService.CreateSubmissionAsync(this._username, this._password, sourceCode, languageId, input, run, isPrivate, cb),
                this.ProcessCreateSubmissionData);
        }

        #endregion

        #region GetSubmissionStatus

        private SubmissionStatus ProcessGetSubmissionStatusData(Hashtable data)
        {
            return new SubmissionStatus
            {
                State = (SubmissionState)(int)data["status"],
                Result = (SubmissionResult)(int)data["result"]
            };
        }

        public SubmissionStatus GetSubmissionStatus(string link)
        {
            return this.Handle(
                () => this._soapService.GetSubmissionStatus(this._username, this._password, link),
                this.ProcessGetSubmissionStatusData);
        }

        public Task<SubmissionStatus> GetSubmissionStatusAsync(string link)
        {
            return this.HandleAsync(
                cb => this._soapService.GetSubmissionStatusAsync(this._username, this._password, link, cb),
                this.ProcessGetSubmissionStatusData);
        }

        #endregion

        private DateTime ConvertTime(string time)
        {
            var utc = DateTime.ParseExact(time, "yyyy-MM-dd HH:mm:ss", null).AddHours(-1);
            return new DateTime(utc.Year, utc.Month, utc.Day, utc.Hour, utc.Minute, utc.Second, DateTimeKind.Utc);
        }

        public SubmissionDetail GetSubmissionDetail(string link, bool withSource, bool withInput, bool withOutput, bool withStdErr, bool withCompileInfo)
        {
            return this.Handle(() =>
            {
                var data = this._soapService.GetSubmissionDetail(this._username, this._password, link, withSource, withInput, withOutput, withStdErr, withCompileInfo);
                this.CheckError(data);

                return new SubmissionDetail()
                {
                    Time = (float)data["time"],
                    IsPublic = (bool)data["public"],
                    State = (SubmissionState)(int)data["status"],
                    Signal = (int)data["signal"],
                    CreateAt = ConvertTime((string)data["date"]),
                    LanguageID = (int)data["langId"],
                    LanguageName = (string)data["langName"],
                    Result = (SubmissionResult)(int)data["result"],
                    Memory = (int)data["memory"],
                    // Optional
                    Source = (string)data["source"],
                    Input = (string)data["input"],
                    Output = (string)data["output"],
                    Error = (string)data["stderr"],
                    CompileInfo = (string)data["cmpinfo"]
                };
            });
        }

        public Dictionary<string, object> Test()
        {
            return this.Handle(() =>
            {
                var result = this._soapService.TestFunction(this._username, this._password);
                this.CheckError(result);

                result.Remove("error");
                return result.Cast<DictionaryEntry>().ToDictionary(
                    p => (string)p.Key,
                    p => p.Value);
            });
        }

        private T Handle<T>(Func<T> body)
        {
            try
            {
                return body();
            }
            catch (IdeoneException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new IdeoneException("Ideone API goes wrong: " + ex.Message, ex);
            }
        }

        private void CheckError(Hashtable result)
        {
            var status = (string)result["error"];
            switch (status)
            {
                case "OK":
                    return;
                case "AUTH_ERROR":
                    throw new AuthenticationFailedException(this._username);
                case "PASTE_NOT_FOUND":
                    throw new PasteNotFoundException();
                case "WRONG_LANG_ID":
                    throw new LanguageNotFoundException();
                case "ACCESS_DENIED":
                    throw new AccessDeniedException();
                case "CANNOT_SUBMIT_THIS_ MONTH_ANYMORE":
                    throw new MonthlyLimitExceededException();
                default:
                    throw new IdeoneException("Unexpected status: " + status);
            }
        }
    }
}
